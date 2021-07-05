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
using MaterialDesignThemes.Wpf;
using MT_Coop.Database_Connection;

namespace MT_Coop.MainMenu.Resources
{
    /// <summary>
    /// Interaction logic for ManageResources_User.xaml
    /// </summary>
    public partial class ManageResources_User : UserControl
    {
        private connn connection;
        Pagination_ex pagination;
        int pagenumber_unverified = 1;
        string selecteduser;

        public ManageResources_User()
        {
            InitializeComponent();
            connection = new connn();
            pagination = new Pagination_ex();
            refreshtable();
        }

        public void refreshtable()
        {
            string result = pagination.datagridpagination_refresh(this.datagrid_sample, pagenumber_unverified, "user", "`fn`, `ln`, `uname`, `utype`", "Where able = 1", this.pagination_prev_btn, this.pagination_next_btn);
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
                if (DialogueHst.IsOpen == true)
                {
                    DialogueHst.IsOpen = false;
                }
                selecteduser = gridRow[2].ToString();
            }
        }

        private void Mouserightclick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null && e.ChangedButton == MouseButton.Right)
            {
                DataRowView selectedus = dg.SelectedItem as DataRowView;
                selecteduser = selectedus[2].ToString();
                ContextMenu cm = this.FindResource("datagrid1_rightclick") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.FlowDirection = FlowDirection.RightToLeft;
                cm.IsOpen = true;
            }
        }

        private void pending_reg_Approve(object sender, RoutedEventArgs e)
        {
            this.DialogueHst.IsOpen = true;
        }

        private void pending_reg_delete(object sender, RoutedEventArgs e)
        {
            if (selecteduser == "")
            {
                connection.showmessage("Please select a user", MessageBoxButton.OK);
            }
            else if (connection.showmessage("Are you sure to delete '" + selecteduser + "' from the list?", MessageBoxButton.YesNo))
            {
                connection.sql_removeitemfromtable("user", "Where uname = '" + selecteduser + "'");
                refreshtable();
            }
        }

        private void Pending_registration_click_accept(object sender, RoutedEventArgs e)
        {
            if (combobox_pending_reg.Text != "" && selecteduser != "")
            {
                connection.sql_updatedatafromtable("user", "utype = '" + combobox_pending_reg.Text + "'", " Where uname = '" + selecteduser + "'");
                this.DialogueHst.IsOpen = false;
                selecteduser = "";
                refreshtable();
            }
            else
            {
                connection.showmessage("Please select a user type", MessageBoxButton.OK);
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
