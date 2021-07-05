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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MT_Coop.Database_Connection;

namespace MT_Coop.MainMenu.Adjustments
{
    /// <summary>
    /// Interaction logic for adjustmnet_trans.xaml
    /// </summary>
    public partial class adjustmnet_trans : UserControl
    {
        inventoryManager inv = new inventoryManager();
        Commit_Transaction commit = new Commit_Transaction();
        connn connection = new connn();
        //selectedUserOnTable
        string selecteduser = "", selectNum;
        //search
        double oldamount = 0, diff, oldqty, qdiff;
        double prc, qty, amt;
        int prodid;
        bool con = false;
        public adjustmnet_trans()
        {
            InitializeComponent();
            refreshtable();
        }

        private void refreshtable()
        {
            DataTable data = connection.sql_gettabledata("transaction_details", "*", "Where `Transaction Number` = '" + text_numb.Text + "'");
            datagrid_products.ItemsSource = data.DefaultView;

            if(text_numb.Text != "") { 
            DataTable data2 = connection.sql_gettabledata("transaction_disp", "`Transaction Number`, `Name`, `Balance`", "Where `Transaction Number` = '" + text_numb.Text + "'");
                if(data2.Rows.Count != 0) {
                    Boton.IsEnabled = true;
                    isCash.IsEnabled = true;
                    selectNum = data2.Rows[0][0].ToString();
                    text_client.Text = data2.Rows[0][1].ToString();
                    text_balace.Text = data2.Rows[0][2].ToString();
                    
                    //search if cash
                    if (connection.Sql_getstringofCustomSQL("transactions","Cash", "where `Transaction Number` = '"+ selectNum + "'").Equals("True"))
                        isCash.IsChecked = true;
                    else
                        isCash.IsChecked = false;

                    recalculateAmount();
                }
                else
                {
                    Boton.IsEnabled = false;
                    isCash.IsEnabled = false;
                }
            }
            
        }


        private void Mouserightclick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null && e.ChangedButton == MouseButton.Right)
            {
                DataRowView selectedus = dg.SelectedItem as DataRowView;
                selecteduser = selectedus[0].ToString();
                ContextMenu cm = this.FindResource("datagrid1_rightclick") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.FlowDirection = FlowDirection.RightToLeft;
                cm.IsOpen = true;
            }

        }
        String prebal = "";
        private void Text_balace_LostFocus(object sender, RoutedEventArgs e)
        {
            if (connection.showmessage("Do you want to commit to the changes you input? this will change CRS Value", MessageBoxButton.YesNo))
            {
                connection.sql_updatedatafromtable("transactions", "Balance = " + text_balace.Text, "where `Transaction Number` = '" + selectNum + "'");
            }
            else
                text_balace.Text = prebal;

            text_balace.IsEnabled = false;
        }

        private void Text_balace_MouseDown(object sender, RoutedEventArgs e)
        {
            prebal = "";
            if (connection.showmessage("Do you want to edit the Balance? After editing click outside the textbox to confirm changes", MessageBoxButton.YesNo))
            {
                text_balace.IsEnabled = true;
                prebal = text_balace.Text;
                text_balace.Text = "";
                text_balace.Focus();
            }
        }
        
        private void IsCash_Click(object sender, RoutedEventArgs e)
        {
            if(isCash.IsChecked == false) { 
            if (connection.showmessage("Are you sure to mark this as Credit? this will Deduct the Client's Balance", MessageBoxButton.YesNo))
            {
                    connection.sql_updatedatafromtable("transactions", "Cash = 0", "where `Transaction Number` = '" + selectNum + "'");
                    string client = connection.Sql_getstringofCustomSQL("transactions", "Client", "where `Transaction Number` = '" + selectNum + "'");
                    commit.Commit_Balance_logs("Adjustment", int.Parse(client), "Adjustment made by " + Properties.Settings.Default.userid, DateTime.Now.ToString("yyyy-MM-dd"), double.Parse(text_amount.Text.ToString()) * 1);
            }
            else
                isCash.IsChecked = true;
            }

            else { 
            if (connection.showmessage("Are you sure to mark this as Cash? this will Add the Client's Balance", MessageBoxButton.YesNo))
            {
                    connection.sql_updatedatafromtable("transactions", "Cash = 1", "where `Transaction Number` = '" + selectNum + "'");
                    string client = connection.Sql_getstringofCustomSQL("transactions", "Client", "where `Transaction Number` = '" + selectNum + "'");
                    commit.Commit_Balance_logs("Adjustment", int.Parse(client), "Adjustment made by " + Properties.Settings.Default.userid, DateTime.Now.ToString("yyyy-MM-dd"), double.Parse(text_amount.Text.ToString()) * -1);
                }
            else
                isCash.IsChecked = false;

            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            TextBox ob = (TextBox)sender;
            double number;
            if (Double.TryParse(ob.Text + ch, out number))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedus = datagrid_products.SelectedItem as DataRowView;


            con = false;
            if (connection.sql_CheckStringInTable(selectedus[2].ToString(), "products", "`Product Name`"))
            {
                prodid = int.Parse(connection.Sql_getstringofCustomSQL("products", "ID", "where `Product Name` = '" + selectedus[2].ToString() + "'"));
                con = true;
            }
            else
            {
                AskDialog.IsOpen = true;
            }

            if (con)
            {
                Double Price = double.Parse(selectedus[5].ToString());
                Double Quatity = double.Parse(selectedus[4].ToString());
                Double Amount = double.Parse(selectedus[6].ToString());
                oldamount = Amount;
                oldqty = Quatity;
                selectNum = selectedus[1].ToString();
                text_Price.Text = Price + "";
                text_Qty.Text = Quatity + "";
                text_Reason.Text = "";
                text_name.Text = "Selected Item is " + selectedus[2].ToString();
                diagloghost.IsOpen = true;
            }
            else
            {

            }
        }

        private void accept_search_Click(object sender, RoutedEventArgs e)
        {
            DataTable datax = connection.sql_gettabledata("products", "*", "Where `Product Name` like '%" + search_prod.Text + "%'");
            if (datax.Rows.Count == 0)
            {
                connection.showmessage("No Product Found!", MessageBoxButton.OK);
            }
            else if (datax.Rows.Count > 1)
            {
                connection.showmessage("Product Name too Broad!, Specify", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Selected item is " + datax.Rows[0][0]);
                prodid = int.Parse(datax.Rows[0][0].ToString());
                AskDialog.IsOpen = false;
                search_prod.Text = "";
                con = true;
            }
        }


        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            refreshtable();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(text_Price.Text, out prc) && double.TryParse(text_Qty.Text, out qty))
            {
                amt = prc * qty;
                diff = (oldamount - amt) * -1;
                commit.Commit_Balance_logs("Adjustment", int.Parse(connection.Sql_getstringofCustomSQL("transactions", "Client", "Where `Transaction Number` = '" + selectNum + "'")), selectNum + "/from " + oldamount, DateTime.Now.ToString("yyyy-MM-dd"), diff);
                qdiff = oldqty - qty;
                inv.insertInventory(prodid, qdiff, DateTime.Now, "Adjustment", text_Reason.Text);
                connection.sql_updatedatafromtable("transaction_details", "`Quantity`=" + qty + ",`Price`=" + prc + ",`Amount`=" + amt, "where ID = " + selecteduser);
                diagloghost.IsOpen = false;

                refreshtable();
            }
        }

        private void recalculateAmount()
        {
            double grandtotal = 0;
            try
            {
                foreach (DataRowView row in datagrid_products.Items)
                {
                    grandtotal += double.Parse(row[6].ToString());
                }
                connection.sql_updatedatafromtable("transaction", "`Total Amount`=" + grandtotal, "Where `Transaction Number`='" + selectNum + "'");
            }
            catch (Exception)
            {
            }

            text_amount.Text = grandtotal + "";
        }
    }
}
