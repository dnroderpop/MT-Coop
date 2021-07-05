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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MT_Coop.Database_Connection;
using System.Text.RegularExpressions;
using System.Data;
using System.Globalization;

namespace MT_Coop.MainMenu.Resources
{
    /// <summary>
    /// Interaction logic for ManagerResources_Client.xaml
    /// </summary>    datagrid_list_client
    public partial class ManagerResources_Client : UserControl
    {
        private connn connection;
        Pagination_ex pagination;
        int pagenumber_unverified = 1;
        string selecteduser = "";
        public ManagerResources_Client()
        {
            InitializeComponent();
            connection = new connn();
            pagination = new Pagination_ex();
            refreshtable();
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

        public void refreshtable()
        {
            string searchsql = text_search.Text;
            if (searchsql != "")
            {
                searchsql = " and (`Name` like '%" + text_search.Text + "%' or `Branch` like '%"+ text_search.Text +"%')";
            }
            string result = pagination.datagridpagination_refresh(this.datagrid_list_client, pagenumber_unverified, "client", "*", "Where able = 1" + searchsql + " order by name", this.pagination_prev_btn, this.pagination_next_btn);
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
                selecteduser = gridRow[0].ToString();
            }
        }


        private void btn_edit_close(object sender, RoutedEventArgs e)
        {
            if (DialogueHst.IsOpen == true)
            {
                DialogueHst.IsOpen = false;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (new_name.Text == "" || new_branch.Text == "" || new_balance.Text == "" || new_date.Text == "")
            {
                connection.showmessage("Please input all required information to proceed", MessageBoxButton.OK);
            }
            else if (connection.showmessage("Are you sure to commit to this action?", MessageBoxButton.YesNo))
            {
                string name, branch, date, value;
                double balance;
                name = new_name.Text;
                branch = new_branch.Text;
                date = DateTime.Parse(new_date.Text).ToString("yyyy-MM-dd");
                balance = double.Parse(new_balance.Text);
                value = "'" + name + "','" + branch + "',0,1";
                if (connection.sql_insertdatatable("client", "`Name`, `Branch`, `Balance`, able", value))
                {
                    new_name.Text = "";
                    new_branch.Text = "";
                    new_date.Text = "";
                    new_balance.Text = "";

                    if (balance >= 0)
                    {
                        string id = connection.Sql_getstringofCustomSQL("client", "ID", " where Name = '" + name + "'");
                        Commit_Transaction commit = new Commit_Transaction();

                        String Transnumber = commit.transaction_number(date, int.Parse(id));

                        //transaction
                        commit.Commit_Transaction_Main(Transnumber, int.Parse(id), 0, date, date, balance);
                        //transaction_details
                        commit.Commit_Transaction_Details(Transnumber, "Beginning", "N/A", 1, balance,DateTime.Parse(date));
                        //balance_logs
                        commit.Commit_Balance_logs("Expense", int.Parse(id), Transnumber, date, balance);
                        refreshtable();

                    }
                }
                else
                {
                    connection.showmessage("Something went wrong Please contact the IT for Help", MessageBoxButton.OK);
                }
            }
            else
            {

            }

        }

        private void Menu_click_Edit(object sender, RoutedEventArgs e)
        {
            DialogueHst.IsOpen = true;
            string ename, ebranch;
            DataTable data = connection.sql_gettabledata("client", "`Name`, `Branch`", "where ID = " + selecteduser);
            ename = data.Rows[0][0].ToString();
            ebranch = data.Rows[0][1].ToString();
            edit_name.Text = ename;
            edit_branch.Text = ebranch;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (connection.sql_updatedatafromtable("client", "Name = '" + edit_name.Text + "', Branch = '" + edit_branch.Text + "'", "where ID = " + selecteduser))
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

        private void Menu_click_Remove(object sender, RoutedEventArgs e)
        {
            if (connection.showmessage("Are you sure you want to delete this? this could be undone", MessageBoxButton.YesNo) == true)
            {
                if (connection.sql_removeitemfromtable("client", "where ID = " + selecteduser))
                {
                    refreshtable();
                }
                else
                    connection.showmessage("Something went wrong!", MessageBoxButton.OK);
            }
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DialogueHst.IsOpen == true)
            {
                DialogueHst.IsOpen = false;
            }
        }

        private void Text_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.pagenumber_unverified = 1;
            refreshtable();
        }
    }
}
