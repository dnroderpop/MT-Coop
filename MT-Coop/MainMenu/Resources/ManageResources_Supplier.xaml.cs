using MT_Coop.Database_Connection;
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

namespace MT_Coop.MainMenu.Resources
{
    /// <summary>
    /// Interaction logic for ManageResources_Supplier.xaml
    /// </summary>
    public partial class ManageResources_Supplier : UserControl
    {
        private connn connection;
        Pagination_ex pagination;
        int pagenumber_unverified = 1;
        string selecteduser = "";


        public ManageResources_Supplier()
        {
            InitializeComponent();

            connection = new connn();
            pagination = new Pagination_ex();
            refreshtable();
        }

        public void refreshtable()
        {
            string result = pagination.datagridpagination_refresh(this.datagrid_list_supplier, pagenumber_unverified, "supplier", "`ID`, `Name`, `Description`", "Where able = 1", this.pagination_prev_btn, this.pagination_next_btn);
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

            if (DialogueHst.IsOpen == true)
            {
                DialogueHst.IsOpen = false;
            }
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


        private void btn_edit_close(object sender, RoutedEventArgs e)
        {
            if (DialogueHst.IsOpen == true)
            {
                DialogueHst.IsOpen = false;
            }
        }

        private void btn_suppler_add(object sender, RoutedEventArgs e)
        {
            string name, desc;
            name = new_name.Text;
            desc = new_desc.Text;
            if (name == "" || desc == "")
            {
                connection.showmessage("Please, please, please, input some text for me to save", MessageBoxButton.OK);
            }
            else
            {
                if (connection.sql_insertdatatable("supplier", "`Name`, `Description`, `able`", "'" + name + "','" + desc + "',1"))
                {
                    new_name.Text = "";
                    name = "";
                    desc = "";
                    new_desc.Text = "";
                    refreshtable();
                }
                else
                {
                    connection.showmessage("Sorry something went wrong, Please try contacting your IT", MessageBoxButton.OK);
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DialogueHst.IsOpen = true;
            string ename, edec;
            DataTable data = connection.sql_gettabledata("supplier", "`Name`, `Description`", "where ID = " + selecteduser);
            ename = data.Rows[0][0].ToString();
            edec = data.Rows[0][1].ToString();
            edit_name.Text = ename;
            edit_decs.Text = edec;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (connection.showmessage("Are you sure you want to delete this? this could be undone", MessageBoxButton.YesNo) == true)
            {
                if (connection.sql_removeitemfromtable("supplier", "where ID = " + selecteduser))
                {
                    refreshtable();
                }
                else
                    connection.showmessage("Something went wrong!", MessageBoxButton.OK);
            }
        }

        private void btn_edit_update(object sender, RoutedEventArgs e)
        {

            if (connection.sql_updatedatafromtable("supplier", "Name = '" + edit_name.Text + "', Description = '" + edit_decs.Text + "'", "where ID = " + selecteduser))
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

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DialogueHst.IsOpen == true)
            {
                DialogueHst.IsOpen = false;
            }
        }
    }
}
