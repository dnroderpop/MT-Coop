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

namespace MT_Coop.MainMenu
{
    /// <summary>
    /// Interaction logic for TestControl_design.xaml
    /// </summary>
    public partial class TestControl_design : UserControl
    {
        private connn connection;
        Pagination_ex pagination;
        int pagenumber = 1;

        public TestControl_design()
        {
            InitializeComponent();
            connection = new connn();
            pagination = new Pagination_ex();
            string result = pagination.datagridpagination_refresh(this.datagrid_sample, pagenumber, "checking", "*", "", this.pagination_prev_btn, this.pagination_next_btn);
            this.pagination_text.Text = result;
        }


        public void pagination_next(object sender, RoutedEventArgs e)
        {
            pagenumber++;
            string result = pagination.datagridpagination_refresh(this.datagrid_sample, pagenumber, "checking", "*", "", this.pagination_prev_btn, this.pagination_next_btn);
            this.pagination_text.Text = result;
        }

        public void pagination_prev(object sender, RoutedEventArgs e)
        {
            pagenumber--;
            string result = pagination.datagridpagination_refresh(this.datagrid_sample, pagenumber, "checking", "*", "", this.pagination_prev_btn, this.pagination_next_btn);
            this.pagination_text.Text = result;
        }
    }
}


