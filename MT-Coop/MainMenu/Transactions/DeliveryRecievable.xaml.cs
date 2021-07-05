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
    /// Interaction logic for DeliveryRecievable.xaml
    /// </summary>
    public partial class DeliveryRecievable : UserControl
    {
        int selecteduser = -1;
        double grandtotal;
        public DataTable datacart;
        connn connection = new connn();
        inventoryManager inv = new inventoryManager();
        string productid, productname, productuom, productprice;
        public DeliveryRecievable()
        {
            InitializeComponent();
            datacart = new dataset_transaction.CartDataTable();
            datagrid_Cart.DataContext = datacart;
            load_search();

            datacart.RowChanged += new DataRowChangeEventHandler(Row_Changed);
        }

        public void Row_Changed(object sender, DataRowChangeEventArgs e)
        {
            grandtotal = 0;
            foreach (DataRow row in datacart.Rows)
            {
                grandtotal += Double.Parse(row[5].ToString());
            }

            drtotal.Text = "Grand Total (₱) " + grandtotal;
        }

        private void load_search()
        {
            drname.Items.Clear(); drprod.Items.Clear();
            DataTable companynames, productlist;
            companynames = connection.sql_Customgettabledata("Select Name from supplier where able = 1 order by Name");
            foreach (DataRow row in companynames.Rows)
            {
                drname.Items.Add(row[0].ToString());
            }
            productlist = connection.sql_Customgettabledata("Select `Product Name` from products where able = 1 order by `Product Name`");
            foreach (DataRow row in productlist.Rows)
            {
                drprod.Items.Add(row[0].ToString());
            }
        }

        private bool checkdetails()
        {
            DateTime test;
            bool cname = false, cdate = false, creb = false, cdrn = false, cdata = false, result = false;
            if (drname.Text != "")
            {
                if (connection.sql_CheckStringInTable(drname.Text, "supplier", "Name"))
                    cname = true;
                else
                    connection.showmessage("The Supplier Name doesnt match any Registered Suppliers", MessageBoxButton.OK);
            }
            if (DateTime.TryParse(drdate.Text, out test))
            {
                cdate = true;
            }
            if (drreb.Text != "")
            {
                creb = true;
            }
            if (datagrid_Cart.Items.Count != 0)
            {
                cdata = true;
            }
            if (drnum.Text != "")
            {
                cdrn = true;
            }

            if (cname == false || cdate == false || creb == false || cdrn == false || cdata == false)
            {
                connection.showmessage("Please input all details of the DR", MessageBoxButton.OK);
            }
            else
            {
                result = true;
            }

            return result;
        }

        private void datatable_click(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataRowView gridRow = dataGrid.SelectedItem as DataRowView;
            if (gridRow != null)
            {
                selecteduser = datagrid_Cart.SelectedIndex;
            }
            else
            {
                selecteduser = -1;
            }
        }

        private void Mouserightclick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null && e.ChangedButton == MouseButton.Right)
            {
                DataRowView selectedus = dg.SelectedItem as DataRowView;
                selecteduser = datagrid_Cart.SelectedIndex;
                ContextMenu cm = this.FindResource("datagrid1_rightclick") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.FlowDirection = FlowDirection.RightToLeft;
                cm.IsOpen = true;
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            datacart.Rows.RemoveAt(selecteduser);
            datagrid_Cart.DataContext = datacart.DefaultView;
            Row_Changed(sender, null);
        }

        private void addProductCommit(object sender, RoutedEventArgs e)
        {
            if (!drprodqt.Text.Trim().Equals("0") && drprodqt.Text.Trim() != "")
            {
                try
                {
                    datacart.Rows.Add(new object[] { productid, productname, productuom, drprodqt.Text, drprodprice.Text, double.Parse(drprodqt.Text) * double.Parse(drprodprice.Text) });
                    DialogueHst.IsOpen = false;
                    datagrid_Cart.DataContext = datacart.DefaultView;
                }
                catch (Exception ex)
                {
                    connection.showmessage(ex.Message, MessageBoxButton.OK);
                }

            }
            else
            {
                connection.showmessage("You can not proceed with 0 value on Quantity", MessageBoxButton.OK);
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

        private void addProductbutton(object sender, RoutedEventArgs e)
        {
            String selectedProduct = "";
            if (connection.sql_CheckStringInTable(drprod.Text, "products", "`product name`"))
                selectedProduct = drprod.Text;
            else
                selectedProduct = "";

            //-----------------------------------------------------

            if (selectedProduct != "")
            {
                productid = connection.Sql_getstringofCustomSQL("products", "ID", " where `Product Name` = '" + selectedProduct + "'");
                productname = selectedProduct;
                productprice = connection.Sql_getstringofCustomSQL("products", "retailerPrice", " where `ID` = " + productid);
                productuom = connection.Sql_getstringofCustomSQL("products", "Unit", " where `ID` = " + productid);
                //-------------------------------------------------------
                drprodname.Text = productname + "(" + productuom + ")";
                drprodprice.Text = productprice;
                drprodqt.Text = 0 + "";
                //-------------------------------------------------------
                DialogueHst.IsOpen = true;
            }
            else
            {
                connection.showmessage("Please properly Select A product", MessageBoxButton.OK);
            }



        }

        private void addTrans(object sender, RoutedEventArgs e)
        {
            if (connection.showmessage("Are you sure you want to commit to transaction?", MessageBoxButton.YesNo))
            {
                if (checkdetails())
                {
                    string supid = connection.Sql_getstringofCustomSQL("supplier", "ID", "Where Name ='" + drname.Text + "'");
                    string values = "'" + drnum.Text + "'," + supid + ",'" + connection.Date_to_Mysql(drdate.SelectedDate.ToString()) + "','" + drreb.Text.ToUpper() + "'";
                    if (connection.sql_insertdatatable("delivery_receipt", "DRNumber,Companyid,Date,Received,notes", values + ",'"+text_note.Text+"'"))
                    {
                        String drid = connection.Sql_getstringofCustomSQL("delivery_receipt", "ID", "order by ID desc");
                        foreach (DataRow row in datacart.Rows)
                        {
                            string val = "[value-1],[value-2],[value-3],[value-4]";
                            val = val.Replace("[value-1]",drid);
                            val = val.Replace("[value-2]", row[0].ToString());
                            val = val.Replace("[value-3]", row[3].ToString());
                            val = val.Replace("[value-4]", row[4].ToString());
                            connection.sql_insertdatatable("delivery_receipt_details", "`DRID`, `Prodid`, `Qty`, `Price`", val);

                            //inventory Commit
                            inv.insertInventory(int.Parse(row[0].ToString()), double.Parse(row[3].ToString()), drdate.SelectedDate.Value, "Delivery", drid);
                        }
                        drdate.Text = "";
                        drname.Text = "";
                        drprod.Text = "";
                        drnum.Text = "";
                        drreb.Text = "";
                        datacart.Clear();
                        datagrid_Cart.DataContext = datacart.DefaultView;
                    }
                }
            }
        }
    }
}
