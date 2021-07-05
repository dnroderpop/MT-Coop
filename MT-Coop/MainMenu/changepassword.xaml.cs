using System;
using System.Collections.Generic;
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

namespace MT_Coop.MainMenu
{
    /// <summary>
    /// Interaction logic for changepassword.xaml
    /// </summary>
    public partial class changepassword : Window
    {
        Database_Connection.connn connection;
        public changepassword()
        {
            InitializeComponent();
            connection = new Database_Connection.connn();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            string passcur, passnew, passretry, user, currentpass;
            passcur = connection.MD5Hash(pass_cur.Password);
            passnew = connection.MD5Hash(pass_new.Password);
            passretry = connection.MD5Hash(pass_retry.Password);
            user = Properties.Settings.Default.userid;
            currentpass = connection.Sql_getstringofCustomSQL("user", "pword", "where uname = '" + user + "'");
            if (currentpass == passcur)
            {
                if (passnew == passretry)
                {
                    if (connection.sql_updatedatafromtable("user", "pword = '" + passnew + "'", "where uname = '" + user + "'"))
                        this.Close();
                }
                else
                {
                    connection.showmessage("Your New password doesnt match with retry password", MessageBoxButton.OK);
                }
            }
            else
            {
                connection.showmessage("Your Current password doesn't match with your Database password", MessageBoxButton.OK);
            }


        }
    }
}
