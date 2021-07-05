using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace MT_Coop.MainMenu.Inventory
{

    public partial class Inventory_management : UserControl
    {
        connn connection = new connn();
        string date1 = "", date2 = "";
        string sqlstring = "";
        string selecteditem;
        bool isTrans = false;
        DataTable dataview = new dataset_transaction.InventoryDataTable();
        public Inventory_management()
        {
            InitializeComponent();
            refreshtable();
        }

        private void Date_beg_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            refreshtable();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            reportviewers.report_inventory reportinv = new reportviewers.report_inventory(new string[1] { text_datefrom.Text }, dataview);
            reportinv.ShowDialog();
        }

        private void Refresh_table_inv_Click(object sender, RoutedEventArgs e)
        {
            refreshtable();
        }

        private void Show_transaction_Click(object sender, RoutedEventArgs e)
        {
            isTrans = true;
            datagrid_inventory.ItemsSource = connection.sql_gettabledata("transaction_disp", "*", "where Date between '" + date1 + "' and '" + date2 + "'").DefaultView;
        }

        private void Show_adjustment_Click(object sender, RoutedEventArgs e)
        {
            isTrans = false;
            datagrid_inventory.ItemsSource = connection.sql_gettabledata("inv_log", "*", "where type = 'Adjustment' and Date between '" + date1 + "' and '" + date2 + "'").DefaultView;
        }

        private void Show_delivery_Click(object sender, RoutedEventArgs e)
        {
            isTrans = false;
            datagrid_inventory.ItemsSource = connection.sql_gettabledata("dr", "*", "where Date between '" + date1 + "' and '" + date2 + "'").DefaultView;
        }

        private void refreshtable()
        {
            isTrans = false;
            dataview = new dataset_transaction.InventoryDataTable();
            if (date_beg.Text != "")
                date1 = connection.Date_to_Mysql(date_beg.SelectedDate.Value.ToString());
            else
                date1 = connection.Date_to_Mysql(DateTime.Now.AddMonths(-1).ToString());

            if (date_end.Text != "")
                date2 = connection.Date_to_Mysql(date_end.SelectedDate.Value.ToString());
            else
                date2 = connection.Date_to_Mysql(DateTime.Now.ToString());

            text_datefrom.Text = "Date from " + date1 + " to " + date2;

            sqlstring = "select a.product, a.`Product Name`, a.`Price`, x.`Retail Price`, COALESCE(e.Beginning,0), COALESCE(b.`Transactions`,0) , COALESCE(c.`Delivery`,0), COALESCE(d.`Adjustment`,0) "
                  + "from(Select inv_log.Product, inv_log.`Product Name`, inv_log.`Price` from inv_log GROUP BY inv_log.Product) a left join (select ID,(products.retailerPrice) `Retail Price` from products) x on a.Product = x.ID "
                  + "left join(SELECT (inv_log.Product) prodid, sum(inv_log.Flow) Transactions from inv_log where inv_log.Type = 'Transaction' and `date` between '" + date1 + "' and '" + date2 + "' GROUP by inv_log.Product) b " //transaction
                  + "on a.product = b.prodid "
                  + "left join(SELECT (inv_log.Product) prodid, sum(inv_log.Flow) Delivery from inv_log where inv_log.Type = 'Delivery' and `date` between '" + date1 + "' and '" + date2 + "' GROUP by inv_log.Product) c " // delivery
                  + "on a.product = c.prodid "
                  + "left join(SELECT (inv_log.Product) prodid, sum(inv_log.Flow) Adjustment from inv_log where inv_log.Type = 'Adjustment' and `date` between '" + date1 + "' and '" + date2 + "' GROUP by inv_log.Product) d " //adjustment
                  + "on a.product = d.prodid "
                  + "left join(SELECT (inv_log.Product) prodid, sum(`Flow`) Beginning FROM `inv_log` WHERE Date < '" + date1 + "' GROUP by inv_log.Product) e " //beginning
                  + "on a.product = e.prodid "
                  + "group by a.product order by a.`product name`";


            DataTable data = connection.sql_Customgettabledata(sqlstring);


            foreach (DataRow row in data.Rows)
            {
                int id = int.Parse(row[0] + "");
                string name = row[1] + "";
                string price = "₱"+ row[2] + "";
                string price2 = "₱"+ row[3] + "";
                double beginning;
                beginning = double.Parse(row[4] + "");
                double transaction;
                transaction = double.Parse(row[5] + "");
                double delivery;
                delivery = double.Parse(row[6] + "");
                double adjustment;
                adjustment = double.Parse(row[7] + "");
                double total;
                total = beginning + transaction + delivery + adjustment;
                dataview.Rows.Add(new object[] { id, name, price, price2, beginning, transaction, delivery, adjustment, total });
            }

            datagrid_inventory.ItemsSource = dataview.DefaultView;

        }

        private void datatable_click(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataRowView gridRow = dataGrid.SelectedItem as DataRowView;
            if (gridRow != null)
            {

                selecteditem = gridRow[1].ToString();
            }
        }

        private void Mouserightclick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null && e.ChangedButton == MouseButton.Right)
            {
                if (isTrans)
                {
                    
                DataRowView selectedus = dg.SelectedItem as DataRowView;
                selecteditem = selectedus[1].ToString();
                ContextMenu cm = this.FindResource("datagrid1_rightclick") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.FlowDirection = FlowDirection.RightToLeft;
                cm.IsOpen = true;

                }
            }

        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selecteditem.Contains("C#"))
            {
                String check =
                connection.Sql_getstringofCustomSQL("transaction_details", "Product", "where `Transaction Number` = '" + selecteditem + "'");
                if (!check.Contains("Beginning"))
                {
                    string transactionnumber = selecteditem;
                    string clientid, clientname, balance, clientbranch, date1, date2;
                    DataTable data = new dataset_transaction.CartDataTable();
                    DataTable temp = connection.sql_gettabledata("datacart", "*", "where not Product = 'Beginning' and `Transaction Number` = '" + selecteditem + "'");

                    foreach (DataRow row in temp.Rows)
                    {
                        data.Rows.Add(new Object[]{row[2].ToString(),//product id
                                            row[3].ToString(),//product name
                                            row[4].ToString(),//UOM
                                            double.Parse(row[5].ToString()),//quantity
                                            double.Parse(row[6].ToString()),//price
                                            double.Parse(row[7].ToString())});//total
                    }

                    clientid = connection.Sql_getstringofCustomSQL("transactions", "Client", " where `Transaction Number` = '" + selecteditem + "'");
                    clientname = connection.Sql_getstringofCustomSQL("client", "Name", " where ID = " + clientid);
                    clientbranch = connection.Sql_getstringofCustomSQL("client", "Branch", " where ID = " + clientid);
                    balance = connection.Sql_getstringofCustomSQL("transactions", "Balance", " where `Transaction Number` = '" + selecteditem + "'");
                    date1 = connection.Sql_getstringofCustomSQL("transactions", "`Date`", " where `Transaction Number` = '" + selecteditem + "'");
                    date2 = connection.Sql_getstringofCustomSQL("transactions", "`Date due`", " where `Transaction Number` = '" + selecteditem + "'");
                    //report
                    String[] strings = new string[6];
                    strings[0] = clientname;
                    strings[1] = balance;
                    strings[2] = clientbranch;
                    strings[3] = date1;
                    strings[4] = date2;
                    strings[5] = selecteditem;
                    reportviewers.report_credit_request_slip report = new reportviewers.report_credit_request_slip(strings, data);

                    report.ShowDialog();
                }
                else
                {
                    connection.showmessage("This does not contain any Credit Slip. This must be a beginning entry", MessageBoxButton.OK);
                }
            }
        }




    }

}
