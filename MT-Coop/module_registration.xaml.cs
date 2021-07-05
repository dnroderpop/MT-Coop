using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MT_Coop.Database_Connection;

namespace MT_Coop
{
    /// <summary>
    /// Interaction logic for module_registration.xaml
    /// </summary>
    public partial class module_registration : Window
    {
        private connn connection = new connn();
        private string fname,lname,username,password;

        private void usernametype(object sender, KeyEventArgs e)
        {
            if (t_username.Text == "")
            {
                t_username.Background = Brushes.White;
            }else if (checkforusername()) {
                t_username.Background = Brushes.Magenta;
            }
            else
            {
                t_username.Background = Brushes.Green;
            }

        }

        public module_registration()
        {
            InitializeComponent();
        }

        private bool checkforusername()
        {
            bool result = false;

            DataTable data = connection.sql_gettabledata("user", "uname", "Where uname = '"+ t_username.Text +"'");

            if (data.Rows.Count != 0)
            {
                result = true;
            }

            return result;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fname = this.t_fname.Text;
            lname = this.t_lname.Text;
            username = this.t_username.Text;
            password = connection.MD5Hash(this.t_password.Password);

            if (fname == "" || lname == "" || username == "" || password == "")
            { 
                connection.showmessage("Please fill up all other textboxes",MessageBoxButton.OK);
            }
            else if(t_username.Background == Brushes.Magenta)
            {
                connection.showmessage("Username is already been taken", MessageBoxButton.OK);
            }
            else
            {
                if (connection.sql_insertdatatable("user", "`fn`, `ln`, `uname`, `pword`, `utype`, `able`",
                    "'"+fname+"','"+lname+"','" + username + "','"+ password+"','request',1"))
                {
                    connection.showmessage("Account has been send, Please wait for approval of the admin", MessageBoxButton.OK);
                    this.t_fname.Text = "";
                    this.t_lname.Text = "";
                    this.t_username.Text = "";
                    this.t_password.Password = "";
                }
                else
                {
                    connection.showmessage("failed", MessageBoxButton.OK);
                }
            }
        }
    }
}
