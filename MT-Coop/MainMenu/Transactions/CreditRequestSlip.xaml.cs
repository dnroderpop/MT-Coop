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

namespace MT_Coop.MainMenu.Transactions
{
    /// <summary>
    /// Interaction logic for CreditRequestSlip.xaml
    /// </summary>
    public partial class CreditRequestSlip : UserControl
    {
        connn connection = new connn();
        Commit_Transaction commit = new Commit_Transaction();
        int productid = -1, clientid = -1;
        DataTable data = new dataset_transaction.CartDataTable();
        Double grandtotal = 0;

        int selecteduser;
        public CreditRequestSlip()
        {
            InitializeComponent();

            loadcomboboxes(cmbox_client, "client", "Name", "order by name");
            loadcomboboxes(cmbox_prod, "products", "Product Name", "order by `product name`");

            data.RowChanged += new DataRowChangeEventHandler(Row_Changed);

            datepicker_date.DisplayDateStart = DateTime.Now.AddDays(-2f);
            datepicker_date.DisplayDateEnd = DateTime.Now;

            datepicker_cutoff.DisplayDateStart = DateTime.Now;
        }

        public void Row_Changed(object sender, DataRowChangeEventArgs e)
        {
            grandtotal = 0;
            foreach (DataRow row in data.Rows)
            {
                grandtotal += Double.Parse(row[5].ToString());
            }

            text_total.Text = "" + grandtotal;
        }


        private void addProductToTable(object sender, RoutedEventArgs e)
        {
            //ID Name UoM Quantity Price Total
            double temp;
            if (!Double.TryParse(text_quantity.Text, out temp))
            {
                connection.showmessage("Please Specify the quantity of the product", MessageBoxButton.OK);
                text_quantity.Focus();
            }
            else if (productid == -1)
            {
                connection.showmessage("Please select a product first", MessageBoxButton.OK);
                cmbox_prod.Focus();
            }
            else
            {
                if (connection.showmessage("Are you sure to add this product?", MessageBoxButton.YesNo))
                {
                    if (!checkitemcart(productid))
                    {
                        if (checkiteminv(productid, Double.Parse(text_quantity.Text)))
                        {
                            String Name, UoM;
                            Double Qty, Price, Total;
                            Name = connection.Sql_getstringofCustomSQL("products", "`Product Name`", " Where ID = " + productid);
                            UoM = connection.Sql_getstringofCustomSQL("products", "`Unit`", " Where ID = " + productid);
                            Qty = Double.Parse(text_quantity.Text);
                            Price = Double.Parse(text_price.Text);
                            Total = Qty * Price;

                            data.Rows.Add(new object[] { productid, Name, UoM, Qty, Price, Total });

                            datagrid_list_products.DataContext = data.DefaultView;
                        }
                        else
                            connection.showmessage("Cant Proceed because there is no stock on hand", MessageBoxButton.OK);

                    }
                    else connection.showmessage("Cant Proceed because there is already an existing product on cart", MessageBoxButton.OK);
                }
            }

        }

        private bool checkitemcart(int product)
        {
            bool result = false;
            foreach (DataRow row in data.Rows)
            {
                if(row[0].ToString() == product + "")
                {
                    result = true;
                }
            }
            return result;
        }

        private bool checkiteminv(int id, double qties)
        {
            bool result = false;
            string date1 = DateTime.Now.ToString("yyyy-MM-dd");
            double qty;
            Double.TryParse(connection.Sql_getstringofCustomSQL("`inventory_logs`", "sum(Flow)", "where Product = " + id + " group by product"), out qty);
            if (qty > 0 && qties <= qty)
            {
                result = true;
            }
            return result;
        }

        private void MenuItem_removm(object sender, RoutedEventArgs e)
        {
            data.Rows.RemoveAt(selecteduser);
            datagrid_list_products.DataContext = data.DefaultView;
            Row_Changed(sender, null);
        }

        private void datatable_click(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataRowView gridRow = dataGrid.SelectedItem as DataRowView;
            if (gridRow != null)
            {
                selecteduser = datagrid_list_products.SelectedIndex;
            }
        }
        private void Mouserightclick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null && e.ChangedButton == MouseButton.Right)
            {
                DataRowView selectedus = dg.SelectedItem as DataRowView;
                if (selectedus != null)
                {
                    selecteduser = datagrid_list_products.SelectedIndex;
                }
                ContextMenu cm = this.FindResource("datagrid1_rightclick") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.FlowDirection = FlowDirection.RightToLeft;
                cm.IsOpen = true;
            }
        }

        private void loadcomboboxes(ComboBox combobox, string tablename, string Column, string where)
        {
            DataTable data = connection.sql_gettabledata(tablename, "`" + Column + "`", " where able = 1 " + where);
            foreach (DataRow row in data.Rows)
            {
                combobox.Items.Add(row[0]);
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            double number;
            TextBox obtext = (TextBox)e.Source;
            if (Double.TryParse(obtext.Text + ch, out number))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void resetproduct()
        {
            cmbox_prod.Text = "";
            text_clientbalance.Text = "Balance =";
            productid = -1;
        }
        private void resetClient()
        {
            cmbox_client.Text = "";
            text_uom.Text = "/PCS";
            text_price.Text = "0";
            clientid = -1;
        }

        private void resetControlNumber()
        {
            text_transnumber.Text = "C#00000000";
            text_transnumber.Foreground = Brushes.Gray;
        }

        private void getControlNumber()
        {
            if (clientid != -1 && datepicker_date.SelectedDate.HasValue)
            {
                String transnumber = commit.transaction_number(datepicker_date.Text, clientid);
                text_transnumber.Text = transnumber;
                text_transnumber.Foreground = Brushes.DarkBlue;
            }
            else
            {
                resetControlNumber();
            }
        }

        private void OnProductSelect(object sender, SelectionChangedEventArgs e)
        {
            if (cmbox_prod.SelectedItem != null)
            {
                double price;
                price = Double.Parse(connection.Sql_getstringofCustomSQL("products", "Value", " where `Product Name` = '" + cmbox_prod.SelectedValue + "'"));
                text_price.Text = price + "";
                string uom, volume;
                volume = connection.Sql_getstringofCustomSQL("products", "Volume", " where `Product Name` = '" + cmbox_prod.SelectedValue + "'");
                uom = connection.Sql_getstringofCustomSQL("products", "Unit", " where `Product Name` = '" + cmbox_prod.SelectedValue + "'");
                text_uom.Text = volume + "/" + uom;
                text_quantity.Text = "1";
                productid = int.Parse(connection.Sql_getstringofCustomSQL("products", "ID", " where `Product Name` = '" + cmbox_prod.SelectedValue + "'"));

            }
            else
            {
                cmbox_prod.Text = "";
            }
        }

        private void OnDateSelect(object sender, SelectionChangedEventArgs e)
        {
            getControlNumber();
            datepicker_cutoff.DisplayDateStart = datepicker_date.SelectedDate;
        }

        private void SubmitButton(object sender, RoutedEventArgs e)
        {
            if (text_transnumber.Text != "C#00000000" &&
            datepicker_cutoff.SelectedDate.HasValue &&
            data.Rows.Count != 0)
            {
                String transnumber, date, due, balance;
                int client = clientid;
                transnumber = text_transnumber.Text;
                double am = double.Parse(text_total.Text);
                due = datepicker_cutoff.SelectedDate.ToString();
                date = datepicker_date.SelectedDate.ToString();
                balance = text_clientbalance.Text.Substring(11);
                //commited Transaction
                commit.Commit_Transaction_Main(transnumber, client, double.Parse(balance), date, due, am);

                //commited transaction details
                foreach (DataRow row in data.Rows)
                {
                    commit.Commit_Transaction_Details(transnumber, row[1].ToString(), row[2].ToString(), Double.Parse(row[3].ToString()), Double.Parse(row[4].ToString()), DateTime.Parse(date));
                }

                //commited transaction Balance
                commit.Commit_Balance_logs("Expense", client, transnumber, DateTime.Parse(date).ToString("yyyy-MM-dd"), am);


                //report
                String[] strings = new string[6];
                strings[0] = cmbox_client.SelectedValue.ToString();
                strings[1] = balance;
                strings[2] = connection.Sql_getstringofCustomSQL("client", "Branch", " where ID = " + client);
                strings[3] = datepicker_date.SelectedDate.ToString();
                strings[4] = datepicker_cutoff.SelectedDate.ToString();
                strings[5] = transnumber;

                if (connection.showmessage("Cash?", MessageBoxButton.YesNo))
                {
                    connection.sql_updatedatafromtable("transactions", "`Cash`= 1", "where `Transaction Number` = '" + transnumber + "'");
                    commit.Commit_Balance_logs("Deduction", clientid, "from " + balance, connection.Date_to_Mysql(datepicker_date.SelectedDate.Value + ""), (am * -1));
                }


                reportviewers.report_credit_request_slip report = new reportviewers.report_credit_request_slip(strings, data);

                report.ShowDialog();

                while (report.IsActive)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                if (!report.IsActive)
                {
                    resetClient();
                    resetControlNumber();
                    resetproduct();
                    data.Rows.Clear();
                    Row_Changed(sender, null);
                }



            }
            else
            {
                MessageBox.Show("You cant proceed without completing data");
            }
        }

        private void OnClientSelect(object sender, SelectionChangedEventArgs e)
        {
            if (cmbox_client.SelectedItem != null)
            {
                Double balance;
                balance = Double.Parse(connection.Sql_getstringofCustomSQL("client", "Balance", " where Name = '" + cmbox_client.SelectedValue + "'"));
                text_clientbalance.Text = "Balance = ₱" + balance;
                clientid = int.Parse(connection.Sql_getstringofCustomSQL("client", "ID", " where Name = '" + cmbox_client.SelectedValue + "'"));

                getControlNumber();
            }
            else
            {
                resetClient();
            }
        }

    }
}
