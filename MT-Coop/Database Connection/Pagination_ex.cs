using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MT_Coop.Database_Connection
{
    public class Pagination_ex
    {
        private connn connection = new connn();
        //pagination starts here
        int totalitemsquery,                //10!!
            PageSize = 10,                   //3!!
            CurrentPage = 1,                //1!!
            EndPage,                        //4!!

            Currentquerypage;               //0 if currentpage = 1 else (currentpage * pagesize)
        public String datagridpagination_refresh(DataGrid dataGrid, int currentPage, string tablename, string col, string wherestate, Button prev, Button next)
        {
            this.CurrentPage = currentPage;
            totalitemsquery = connection.sql_gettabledata(tablename, "*", wherestate).Rows.Count; //gets totalnumber
            EndPage = totalitemsquery / PageSize;
            double temp = double.Parse(totalitemsquery + "") / double.Parse(PageSize + ""); 
            if (temp % 1 != 0)
            { 
                EndPage++;
            }                                                                                    //end page determined

            Currentquerypage = (currentPage * PageSize) - PageSize;                                                                                //Currentquery page determined


            string limit_query = " limit " + Currentquerypage + ", " + PageSize;
            DataTable data = connection.sql_gettabledata(tablename, col, wherestate + limit_query);

            dataGrid.DataContext = data.DefaultView;
            datagridpagination_checkforpos(prev, next, CurrentPage, EndPage);
            string ret = "Page " + CurrentPage + " / " + EndPage;
            Debug.Write(ret);
            return ret;
        }

        public void datagridpagination_checkforpos(Button prev, Button next, int Currentpage, int endpage)
        {
            if(endpage == 1 || endpage == 0)
            {
                prev.IsEnabled = false;
                next.IsEnabled = false;
            } else if  (CurrentPage == 1 && 1 == endpage)
            {
                prev.IsEnabled = false;
                next.IsEnabled = false;
            }
            else if (CurrentPage == 1 && endpage >1)
            {
                next.IsEnabled = true;
                prev.IsEnabled = false;
            }else if (CurrentPage == endpage)
            {
                next.IsEnabled = false;
                prev.IsEnabled = true;
            }
            else
            {
                prev.IsEnabled = true;
                next.IsEnabled = true;
            }
        }

    }
}
