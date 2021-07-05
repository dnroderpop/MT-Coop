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

namespace MT_Coop.Database_Connection
{
    /// <summary>
    /// Interaction logic for database_settings.xaml
    /// </summary>
    public partial class database_settings : Window
    {
        private String server, user, pass, db, key;
        
        private void onbuttonclocl(object sender, RoutedEventArgs e)
        {
            connn c = new connn();
            if (c.showmessage("Are you sure?", MessageBoxButton.YesNo))
            {
                savedatabasesettigns();
                c.showmessage("Successfully Saved", MessageBoxButton.OK);
                this.Hide();
            }
        }

        public database_settings()
        {
            InitializeComponent();

            loaddatabasesettings();
        }

        private void loaddatabasesettings()
        {
            this.server = Properties.Settings.Default.serverdb;
            this.user = Properties.Settings.Default.userdb;
            this.pass = Properties.Settings.Default.passdb;
            this.db = Properties.Settings.Default.namedb;
            this.key = Properties.Settings.Default.licenseKey;

            this.t_sever.Text = this.server;
            this.t_username.Text = this.user;
            this.t_password.Password = this.pass;
            this.t_database.Text = this.db;
            this.t_keygen.Text = this.key;
        }

        private void savedatabasesettigns()
        {

            this.server = this.t_sever.Text;
            this.user = this.t_username.Text;
            this.pass = this.t_password.Password;
            this.db = this.t_database.Text;
            this.key = this.t_keygen.Text;

            Properties.Settings.Default.serverdb = this.server;
            Properties.Settings.Default.userdb = this.user;
            Properties.Settings.Default.passdb = this.pass;
            Properties.Settings.Default.namedb = this.db;
            Properties.Settings.Default.licenseKey = this.key;

            loaddatabasesettings();
        }

        


    }////////////end of class

}
