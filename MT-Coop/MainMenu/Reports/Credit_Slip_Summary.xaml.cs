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
using LiveCharts;
using LiveCharts.Wpf;

namespace MT_Coop.MainMenu.Reports
{
    /// <summary>
    /// Interaction logic for Credit_Slip_Summary.xaml
    /// </summary>
    public partial class Credit_Slip_Summary : UserControl
    {
        //pagenation
        private connn connection;
        Pagination_ex pagination;
        int pagenumber_unverified = 1;
        //selectedUserOnTable
        string selecteduser = "";
        //search
        string searchstring = "where `date` = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";

        public Credit_Slip_Summary()
        {
            InitializeComponent();
            //pagination Of Table List
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
            dosearch();
            string result = pagination.datagridpagination_refresh(this.datagrid_list_client, pagenumber_unverified, "transaction_disp", "*", searchstring, this.pagination_prev_btn, this.pagination_next_btn);
            this.pagination_text.Text = result;

        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "MMMM dd, yyyy";
        }

        //searchstring
        private void dosearch()
        {
            string search = text_search.Text.TrimEnd(' ');
            String firstdate, seconddate;
            try
            {
                firstdate = date_f.SelectedDate.Value.ToString("yyyy-MM-dd");
                date_s.DisplayDateStart = date_f.SelectedDate.Value;
                seconddate = date_s.SelectedDate.Value.ToString("yyyy-MM-dd");

            }
            catch (Exception)
            {
                firstdate = DateTime.Now.ToString("yyyy-MM-dd");
                seconddate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (search != "")
            {
                searchstring = "Where (`date` between '" + firstdate + "' and '" + seconddate + "')and( `Name` like '%" + search + "%' or Branch like '%" + search + "%' or `Transaction Number` like '%" + search + "%')";
            }
            else
            {
                searchstring = "where (`date` between '" + firstdate + "' and '" + seconddate + "')";
            }

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

                selecteduser = gridRow[1].ToString();
            }
        }

        private void Mouserightclick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null && e.ChangedButton == MouseButton.Right)
            {
                DataRowView selectedus = dg.SelectedItem as DataRowView;
                selecteduser = selectedus[1].ToString();
                ContextMenu cm = this.FindResource("datagrid1_rightclick") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.FlowDirection = FlowDirection.RightToLeft;
                cm.IsOpen = true;
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selecteduser.Contains("C#"))
            {
                String check = 
                connection.Sql_getstringofCustomSQL("transaction_details", "Product", "where `Transaction Number` = '" + selecteduser + "'");
                if (!check.Contains("Beginning")) { 
                string transactionnumber = selecteduser;
                string clientid, clientname, balance, clientbranch, date1, date2;
                DataTable data = new dataset_transaction.CartDataTable();
                DataTable temp = connection.sql_gettabledata("datacart", "*", "where not Product = 'Beginning' and `Transaction Number` = '" + selecteduser + "'");

                foreach (DataRow row in temp.Rows)
                {
                    data.Rows.Add(new Object[]{row[2].ToString(),//product id
                                            row[3].ToString(),//product name
                                            row[4].ToString(),//UOM
                                            double.Parse(row[5].ToString()),//quantity
                                            double.Parse(row[6].ToString()),//price
                                            double.Parse(row[7].ToString())});//total
                }

                clientid = connection.Sql_getstringofCustomSQL("transactions", "Client", " where `Transaction Number` = '" + selecteduser + "'");
                clientname = connection.Sql_getstringofCustomSQL("client", "Name", " where ID = " + clientid);
                clientbranch = connection.Sql_getstringofCustomSQL("client", "Branch", " where ID = " + clientid);
                balance = connection.Sql_getstringofCustomSQL("transactions", "Balance", " where `Transaction Number` = '" + selecteduser + "'");
                date1 = connection.Sql_getstringofCustomSQL("transactions", "`Date`", " where `Transaction Number` = '" + selecteduser + "'");
                date2 = connection.Sql_getstringofCustomSQL("transactions", "`Date due`", " where `Transaction Number` = '" + selecteduser + "'");
                //report
                String[] strings = new string[6];
                strings[0] = clientname;
                strings[1] = balance;
                strings[2] = clientbranch;
                strings[3] = date1;
                strings[4] = date2;
                strings[5] = selecteduser;
                reportviewers.report_credit_request_slip report = new reportviewers.report_credit_request_slip(strings, data);

                report.ShowDialog();
                }
                else
                {
                    connection.showmessage("This does not contain any Credit Slip. This must be a beginning entry",MessageBoxButton.OK);
                }
        }
        }

        private void Date_f_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            refreshtable();
        }

        private void Text_search_KeyUp(object sender, KeyEventArgs e)
        {
            pagenumber_unverified = 1;
            refreshtable();
        }

        
    }
}
