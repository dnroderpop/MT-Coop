using MT_Coop.Database_Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MT_Coop.MainMenu.Resources
{
    /// <summary>
    /// Interaction logic for ManageResources_Product.xaml
    /// </summary>
    public partial class ManageResources_Product : UserControl
    {
        private connn connection;
        Pagination_ex pagination;
        int pagenumber_unverified = 1;
        string selecteduser = "";

        public ManageResources_Product()
        {
            InitializeComponent();

            connection = new connn();
            pagination = new Pagination_ex();
            refreshtable();
        }

        public void refreshtable()
        {
            string searchsql = text_search.Text;
            if(searchsql != "") {
                searchsql = "and `Product Name` like '%" + text_search.Text + "%'";
            }
            string result = pagination.datagridpagination_refresh(this.datagrid_list_products, pagenumber_unverified, "products", "`ID`, `Product Name`, `Unit`, `Volume`, `Value`, `retailerPrice`", "Where able = 1 " + searchsql, this.pagination_prev_btn, this.pagination_next_btn);
            this.pagination_text.Text = result;
        }

        public void pagination_next(object sender, RoutedEventArgs e)
        {
            pagenumber_unverified++;
            refreshtable();
        }

        public void pagination_prev(object sender, RoutedEventArgs e)
        {
            pagenumber_unverified--;
            refreshtable();
        }

        private void datatable_click(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataRowView gridRow = dataGrid.SelectedItem as DataRowView;
            if (gridRow != null)
            {
                selecteduser = gridRow[0].ToString();
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

        private void btn_product_add(object sender, RoutedEventArgs e)
        {
            string name, uom, vol, val,srp;
            name = new_name.Text;
            uom = new_unit.Text;
            vol = new_volume.Text;
            val = new_value.Text;
            srp = new_price.Text;
            if (name == "" || uom == "" || vol == "" || val == "")
            {
                connection.showmessage("Please, please, please, input some text for me to save", MessageBoxButton.OK);
            }
            else
            {
                if (connection.sql_insertdatatable("products", "`Product Name`, `Unit`, `Volume`, `Value`,`retailerPrice`,`able`", "'" + name + "','" + uom + "'," + vol + "," + val + ","+srp+",1"))
                {
                    name = "";
                    new_name.Text = "";
                    uom = "";
                    new_unit.Text = "";
                    vol = "";
                    new_volume.Text = "";
                    val = "";
                    new_value.Text = "";
                    srp = "";
                    new_price.Text = "";

                    refreshtable();
                }
                else
                {
                    connection.showmessage("Sorry something went wrong, Please try contacting your IT", MessageBoxButton.OK);
                }
            }
        }

        private void MenuItem_Edit(object sender, RoutedEventArgs e)
        {
            DialogueHst.IsOpen = true;
            string ename, eunit, evol, eval,esrp;
            DataTable data = connection.sql_gettabledata("products", "`Product Name`, `Unit`, `Volume`, `Value`, `retailerPrice`", "where ID = " + selecteduser);
            ename = data.Rows[0][0].ToString();
            eunit = data.Rows[0][1].ToString();
            evol = data.Rows[0][2].ToString();
            eval = data.Rows[0][3].ToString();
            esrp = data.Rows[0][4].ToString();
            edit_price.Text = esrp;
            edit_name.Text = ename;
            edit_unit.Text = eunit;
            edit_volume.Text = evol;
            edit_value.Text = eval;
        }

        private void btn_edit_close(object sender, RoutedEventArgs e)
        {
            if (DialogueHst.IsOpen == true)
            {
                DialogueHst.IsOpen = false;
            }
        }

        private void MenuItem_Remove(object sender, RoutedEventArgs e)
        {
            if (connection.showmessage("Are you sure you want to delete this? this could be undone", MessageBoxButton.YesNo) == true)
            {
                if (connection.sql_removeitemfromtable("products", "where ID = " + selecteduser))
                {
                    refreshtable();
                }
                else
                    connection.showmessage("Something went wrong!", MessageBoxButton.OK);
            }
        }

        private void btn_edit_update(object sender, RoutedEventArgs e)
        {
            String sqlquery = "`Product Name`='[value-2]',`Unit`='[value-3]',`Volume`=[value-4],`Value`=[value-5], `retailerPrice` = [value-6] ";
            sqlquery = sqlquery.Replace("[value-2]", edit_name.Text);
            sqlquery = sqlquery.Replace("[value-3]", edit_unit.Text);
            sqlquery = sqlquery.Replace("[value-4]", edit_volume.Text);
            sqlquery = sqlquery.Replace("[value-5]", edit_value.Text);
            sqlquery = sqlquery.Replace("[value-6]", edit_price.Text);

            if (connection.sql_updatedatafromtable("products", sqlquery, "where ID = " + selecteduser))
            {
                refreshtable();
                DialogueHst.IsOpen = false;
            }
            else
            {
                if (connection.showmessage("Something went wrong, Do you want to close this form or continue trying ?", MessageBoxButton.YesNo))
                {
                    DialogueHst.IsOpen = false;
                }
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

        private void Text_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            pagenumber_unverified = 1;
            refreshtable();
        }
    }
}
