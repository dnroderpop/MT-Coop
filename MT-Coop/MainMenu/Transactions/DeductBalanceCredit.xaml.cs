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

namespace MT_Coop.MainMenu.Transactions
{
    /// <summary>
    /// Interaction logic for DeductBalanceCredit.xaml
    /// </summary>
    public partial class DeductBalanceCredit : UserControl
    {
        //pagenation
        private connn connection;
        Pagination_ex pagination;
        int pagenumber_unverified = 1;
        //selectedUserOnTable
        string selecteduser = "";
        //search
        string searchstring = "where able = 1";
        //trans
        Commit_Transaction commit = new Commit_Transaction();
        public DeductBalanceCredit()
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
            if (Double.TryParse(ob.Text+ ch, out number))
                e.Handled = false;
            else
                e.Handled = true;
        }

        public void refreshtable()
        {
            dosearch();
            string result = pagination.datagridpagination_refresh(this.datagrid_list_client, pagenumber_unverified, "client", "*", searchstring + " order by name", this.pagination_prev_btn, this.pagination_next_btn);
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

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DialogueHst.IsOpen == true)
            {
                DialogueHst.IsOpen = false;
            }
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

        // Everyotherthing starts here

       //searchstring
       private void dosearch()
        {
            string search = text_search.Text.TrimEnd(' ');
            if (search != "")
            {
                searchstring = "Where able = 1 and ( `Name` like '%"+search+"%' or Branch like '%"+search+"%')";
            }
            else
            {
                searchstring = "Where able = 1";
            }
        }

        private void Menu_click_Edit(object sender, RoutedEventArgs e)
        {
            DialogueHst.IsOpen = true;
            string ename, ebranch;
            DataTable data = connection.sql_gettabledata("client", "`Name`, `Branch`,`Balance`", "where ID = " + selecteduser);
            ename = data.Rows[0][0].ToString();
            ebranch = data.Rows[0][1].ToString();
            text_name.Text = ename;
            text_branch.Text = ebranch;
            text_balance.Text = data.Rows[0][2].ToString();
            text_date.SelectedDate = DateTime.Today;
        }


        private void Text_search_KeyUp(object sender, KeyEventArgs e)
        {
            pagenumber_unverified = 1;
            refreshtable();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(text_Payment.Text.TrimEnd() != "") { 
                double balance = 0, payment = 0, total = 0;
                Double.TryParse(text_balance.Text,out balance);
                Double.TryParse(text_Payment.Text, out payment);
                if(balance < payment)
                {
                    connection.showmessage("System Detected that Balance value is less than the payment", MessageBoxButton.OK);
                }
                else
                {
                    total = balance - payment;
                    if (connection.showmessage("The total is " + total + ", would you like to commit?",MessageBoxButton.YesNo)){
                        //Commit
                        commit.Commit_Balance_logs("Deduction",int.Parse(selecteduser),"from "+balance,text_date.SelectedDate.Value.ToString("yyyy-MM-dd"),(payment * -1));

                        MessageBox.Show("Done");
                        DialogueHst.IsOpen = false;
                        refreshtable();
                    }
                }
            }
            else
            {
                connection.showmessage("Please try to check the payment for valid value",MessageBoxButton.OK);
            }
        }

        private void Text_Payment_KeyUp(object sender, KeyEventArgs e)
        {
            double balance = 0, payment = 0, total = 0;
            Double.TryParse(text_balance.Text, out balance);
            Double.TryParse(text_Payment.Text, out payment);
            total = balance - payment;

            text_Total.Text = total.ToString();
        }

        private void PrintTable(object sender, RoutedEventArgs e)
        {
            dosearch();
            DataTable PrintData = connection.sql_gettabledata("client", "*", searchstring + " and not Balance = 0");

            if (PrintData.Rows.Count != 0)
            {
                reportviewers.report_balance rp= new reportviewers.report_balance(PrintData);
                rp.ShowDialog();
            }
            else
            {
                connection.showmessage("No Data to Show", MessageBoxButton.OK);
            }
        }

    }
}
