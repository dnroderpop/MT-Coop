using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MT_Coop.Database_Connection;
using MT_Coop.MainMenu;

namespace MT_Coop
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int i;
        database_settings ds = new database_settings();
        private String exs = "failed";
        private String result = "";


        private connn connection = new connn();
        private BackgroundWorker backgroundWorker1;

        public MainWindow()
        {

            InitializeComponent();
            this.panel_login.Visibility = Visibility.Hidden;
            connecttodb();
            text_username.Focus();
        }

        private void reconnect_doubleclick(object sender, MouseButtonEventArgs e)
        {
            connecttodb();
            this.panel_login.Visibility = Visibility.Hidden;
            this.lbl_loading_cloud_database.Content = "Resetting Connection.";
            this.lbl_loading_cloud_result.Content = "Connecting please wait";
            this.lbl_loading_cloud_result.ToolTip = "Connecting please wait";
        }

        private void connecttodb()
        {
            i = 0;
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            onStart();
        }

        public void onStart()
        {
            bool resultbol = false;
            try
            {
                resultbol = connection.check();
                if (resultbol == true)
                {
                    result = "success";
                    i = 100;
                    backgroundWorker1.ReportProgress(100);
                }
                else
                {
                    while (i <= 99)
                    {
                        resultbol = connection.check();
                        if (resultbol == true)
                        {
                            result = "success";
                            backgroundWorker1.ReportProgress(100);
                            i = 100;
                        }
                        else
                        {
                            result = "failed";
                            exs = "Something went wrong on query";
                            i = i + 1;
                            Thread.Sleep(1);
                            backgroundWorker1.ReportProgress(i / 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                while (i <= 100)
                {
                    resultbol = connection.check();
                    if (resultbol == true)
                    {
                        result = "success";
                        backgroundWorker1.ReportProgress(100);
                        i = 100;
                    }
                    else
                    {
                        this.exs = ex.ToString();
                        result = "failed";
                        i = i + 1;
                        Thread.Sleep(10);
                        backgroundWorker1.ReportProgress(i / 1);
                    }
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (i == 40)
            {
                this.lbl_loading_cloud_database.Content = "Loading cloud database.";
            }
            else if (i == 70)
            {
                this.lbl_loading_cloud_database.Content = "Loading cloud database..";
            }
            else if (i == 90)
            {
                this.lbl_loading_cloud_database.Content = "Loading cloud database...";
            }
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (result == "failed")
            {
                this.lbl_loading_cloud_database.Content = "Failed to Connect to database";
                this.lbl_loading_cloud_result.Content = exs.Substring(0, 18) + "...";
                this.lbl_loading_cloud_result.ToolTip = exs + "";
            }
            else if (result == "success")
            {
                this.panel_login.Visibility = Visibility.Visible;
                this.lbl_loading_cloud_database.Content = "Successfully Connected";
                this.lbl_loading_cloud_result.Content = "Connection to Database Est...";
                this.lbl_loading_cloud_result.ToolTip = "Connection to Database Established and ready to use.";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit this application?", "Hoop System", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
                ds.Close();
                this.Close();
            }
        }

        private void activatesettings(object sender, MouseWheelEventArgs e)
        {
            try
            {
                ds.ShowDialog();
            }
            catch (Exception)
            {
                database_settings ds = new database_settings();
                ds.Show();
            }
        }

        private void Button_login(object sender, RoutedEventArgs e)
        {
            String username, password;
            username = this.text_username.Text.ToLower();
            password = connection.MD5Hash(this.text_password.Password);

            DataTable d = connection.sql_gettabledata("user", "uname", "where able = 1 and not utype = 'request' and uname = '" + username + "' and pword = '" + password + "'");
            if (d.Rows.Count != 0)
            {
                Properties.Settings.Default.userid = d.Rows[0][0].ToString();
                dropsite menu = new dropsite();
                menu.Show();
                this.Close();
            }
            else
            {
                connection.showmessage("Username or Password does not match to any 'Verified' accounts", MessageBoxButton.OK);
            }
        }

        private void Button_register_MouseUp(object sender, MouseButtonEventArgs e)
        {
            module_registration reg = new module_registration();
            reg.ShowDialog();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_login(sender, e);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
