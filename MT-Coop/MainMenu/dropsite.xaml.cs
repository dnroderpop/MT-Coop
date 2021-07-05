using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
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

namespace MT_Coop.MainMenu
{
    /// <summary>
    /// Interaction logic for dropsite.xaml
    /// </summary>
    public partial class dropsite : Window
    {
        connn connection;
        public dropsite()
        {
            InitializeComponent();
            connection = new connn();
            SetUserWelcomeName();

            this.frame_change.Content = new Home();
        }

        private void SetUserWelcomeName()
        {
            string strname = connection.get_user_name();
            this.text_userCurrent.Text = strname;
            this.text_userCurrent.ToolTip = strname;
            if (strname.Contains("Master"))
            {
                mediaPlayer.Open(new Uri("pack://siteoforigin:,,,/sound.mp3"));
                mediaPlayer.Play();
                Masterx(); 
            }else if (strname.Contains("Admin"))
            {
                dx.IsEnabled = false;
            }else if (strname.Contains("Coop"))
            {
                b4.IsEnabled = false;
                dx.IsEnabled = false;
            }
                this.text_userCurrent.Width = double.NaN;
        }
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private async void Masterx()
        {
            while(true)
            {
                text_userCurrent.Foreground = PickBrush();
                await Task.Delay(1000);
            }
        }

        private Brush PickBrush()
        {
            Brush result = Brushes.Transparent;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);

            return result;
        }

        private void button_closewindows(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void button_clickontopbar(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void Ontransactionclick(object sender, RoutedEventArgs e)
        {
            closeallexpander(Ex_Transaction);
            if (this.Ex_Transaction.IsExpanded == true)
                this.Ex_Transaction.IsExpanded = false;
            else
                this.Ex_Transaction.IsExpanded = true;
        }
        private void OnInventoryclick(object sender, RoutedEventArgs e)
        {
            closeallexpander(Ex_Inventory);
            if (this.Ex_Inventory.IsExpanded)
                this.Ex_Inventory.IsExpanded = false;
            else
                this.Ex_Inventory.IsExpanded = true;
        }
        private void OnReportclick(object sender, RoutedEventArgs e)
        {
            closeallexpander(Ex_Reports);
            if (this.Ex_Reports.IsExpanded)
                this.Ex_Reports.IsExpanded = false;
            else
                this.Ex_Reports.IsExpanded = true;
        }
        private void OnAdjustement(object sender, RoutedEventArgs e)
        {
            closeallexpander(Ex_Adjustments);
            if (this.Ex_Adjustments.IsExpanded)
                this.Ex_Adjustments.IsExpanded = false;
            else
                this.Ex_Adjustments.IsExpanded = true;

        }
        private void OnManagerClick(object sender, RoutedEventArgs e)
        {
            closeallexpander(Ex_Manage);
            if (this.Ex_Manage.IsExpanded)
                this.Ex_Manage.IsExpanded = false;
            else
                this.Ex_Manage.IsExpanded = true;
        }
        private void closeallexpander(Expander exs)
        {
            bool b = exs.IsExpanded;

            this.Ex_Manage.IsExpanded = false;
            this.Ex_Adjustments.IsExpanded = false;
            this.Ex_Reports.IsExpanded = false;
            this.Ex_Inventory.IsExpanded = false;
            this.Ex_Transaction.IsExpanded = false;

            exs.IsExpanded = b;
        }
        private void OnLogOut_Press(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to logout?", "Log out", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
        private void OnAccountClick(Object sender, RoutedEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.FlowDirection = FlowDirection.RightToLeft;
            cm.IsOpen = true;
        }
        private void OnTabPress(Object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            Button button = (Button)e.Source;
            TextBlock textBlock = (TextBlock)button.Content;
            this.title_text.Text = textBlock.Text;
            Color randomColor = Color.FromRgb(byte.Parse(rnd.Next(256) + ""), byte.Parse(rnd.Next(256) + ""), byte.Parse(rnd.Next(256) + ""));
            while (this.frame_change.NavigationService.RemoveBackEntry()!=null);
            switch (textBlock.Text)
            {
                case "Manage Users":
                    this.frame_change.Content = new Resources.ManageResources_User();
                    break;
                case "Manage Clients":
                    this.frame_change.Content = new Resources.ManagerResources_Client();
                    break;
                case "Manage Suppliers":
                    this.frame_change.Content = new Resources.ManageResources_Supplier();
                    break;
                case "Manage Products":
                    this.frame_change.Content = new Resources.ManageResources_Product();
                    break;
                case "Credit Request slip":
                    this.frame_change.Content = new Transactions.CreditRequestSlip();
                    break;
                case "Deduct Balance":
                    this.frame_change.Content = new Transactions.DeductBalanceCredit();
                    break;
                case "Client Requests Summary":
                    this.frame_change.Content = new Reports.Credit_Slip_Summary();
                    break;
                case "Delivery Receivables":
                    this.frame_change.Content = new Transactions.DeliveryRecievable();
                    break;
                case "DR Summary":
                    this.frame_change.Content = new Reports.DR_Summary();
                    break;
                case "View Inventory":
                    this.frame_change.Content = new Inventory.Inventory_management();
                    break;
                case "Adjust Transaction":
                    this.frame_change.Content = new Adjustments.adjustmnet_trans();
                    break;
            }
        }
        private void Onchangepassclick(Object sender, RoutedEventArgs e)
        {
            changepassword changepassword = new changepassword();
            changepassword.ShowDialog();
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.frame_change.Content = new Home();
        }
    }
}
