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
using System.Data;

namespace MT_Coop.MainMenu.Reports
{
    /// <summary>
    /// Interaction logic for DR_Summary.xaml
    /// </summary>
    public partial class DR_Summary : UserControl
    {
        //pagenation
        private connn connection;
        Pagination_ex pagination;
        int pagenumber_unverified = 1;
        //selectedUserOnTable
        string selecteduser = "";
        //search
        string searchstring = "where `date` = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";

        public DR_Summary()
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
            string result = pagination.datagridpagination_refresh(this.datagrid_list_client, pagenumber_unverified, "dr", "*", searchstring, this.pagination_prev_btn, this.pagination_next_btn);
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
                seconddate = date_s.SelectedDate.Value.ToString("yyyy-MM-dd");

                date_s.DisplayDateStart = date_f.SelectedDate.Value;
            }
            catch (Exception)
            {
                firstdate = DateTime.Now.ToString("yyyy-MM-dd");
                seconddate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (search != "")
            {
                searchstring = "Where (`date` between '" + firstdate + "' and '" + seconddate + "')and( `DRNumber` like '%" + search + "%' or Company like '%" + search + "%' or `Received` like '%" + search + "%' or `Note` like '%" + search + "%')";
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataTable temp = connection.sql_gettabledata("dr", "*", "where id = " + selecteduser);
            DataTable dataTable = new dataset_transaction.CartDataTable();
            String[] data = new string[5];
            data[0] = temp.Rows[0][2].ToString();
            data[1] = temp.Rows[0][1].ToString();
            data[2] = temp.Rows[0][3].ToString();
            data[3] = temp.Rows[0][4].ToString();
            data[4] = temp.Rows[0][5].ToString();
            DataTable temp2 = connection.sql_gettabledata("dr_details", "prodid,`Product Name`,unit,qty,price", "where drid = " + selecteduser);
            foreach (DataRow rows in temp2.Rows)
            {
                int id = int.Parse(rows[0].ToString());
                string name = rows[1].ToString();
                String uom = rows[2].ToString();
                double qty = double.Parse(rows[3].ToString());
                double price = double.Parse(rows[4].ToString());
                double total = double.Parse(rows[3].ToString()) * double.Parse(rows[4].ToString());
                dataTable.Rows.Add(new Object[] { id, name,uom, qty, price , total });
            }


            reportviewers.report_dr report = new reportviewers.report_dr(data, dataTable);
            report.ShowDialog();
        }

        private void Date_f_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            refreshtable();
            if (date_f.SelectedDate != null)
            {
                date_s.DisplayDateStart = date_f.SelectedDate;
            }
        }

        private void Text_search_KeyUp(object sender, KeyEventArgs e)
        {
            pagenumber_unverified = 1;
            refreshtable();
        }


    }
}