using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace MT_Coop.MainMenu
{
    /// <summary>
    /// select client.Name ,a.amt, a.date  from ( SELECT balance_logs.Client, sum(`Amount Total`) amt, Date FROM `balance_logs` GROUP by client, Date) a LEFT join `Client` on Client.ID = a.client
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        connn Connection = new connn();
        DataTable data, data2, data3;
        List<string> mDates = new List<string>(), titles = new List<string>();
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            SeriesCollection = new SeriesCollection { };
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-PH");
            YFormatter = value => value.ToString("C", culture);

            DateTime tempdate;

            //data = Connection.sql_gettabledata("transaction_disp", " `Date`", "Where Date between '2020-01-01' and '2020-12-31' Group by Date ASC");
            data = Connection.sql_gettabledata("balance_logs", " `Date`", "Where Date between '2020-01-01' and '2020-12-31' Group by Date ASC");

            foreach (DataRow row in data.Rows)
            {
                tempdate = DateTime.Parse(row[0].ToString());
                mDates.Add(tempdate.ToString("MMMM dd, yyyy"));
            }

            Labels = mDates.ToArray();


            //data2 = Connection.sql_gettabledata("transaction_disp", " Branch, sum(`Total Amount`) as Amount", "Where Date between '2020-01-01' and '2020-12-31' Group by Branch ASC");
            data2 = Connection.sql_gettabledata("balance_logs", " client.Branch, sum(`Amount Total`) as Amount", "left join `client` on balance_logs.client = Client.ID Where Date between '2020-01-01' and '2020-12-31' Group by Branch ASC");

            foreach (DataRow row in data2.Rows)
            {
                titles.Add(row[0].ToString());
                SeriesCollection.Add(new LineSeries
                {
                    Title = row[0].ToString(),
                    Values = new ChartValues<double> { },
                    PointGeometry = DefaultGeometries.Circle
                });
            }

            DataContext = this;
            //string sqlstring = "SELECT a.*, b.*  FROM (SELECT id,Branch, SUM(`total amount`) as s FROM `transaction_disp` WHERE date = '2020-06-04' GROUP by Branch) a RIGHT join (SELECT * FROM `transaction_disp` GROUP by Branch) b on a.branch = b.branch";
            livechartfillvalue();


            //string sqls = "SELECT b.branch, a.* FROM (SELECT id,Branch, SUM(`total amount`), Date as s FROM `transaction_disp` WHERE date between '2020-01-01' and '2020-12-30' GROUP by Branch,date) a RIGHT join (SELECT * FROM `transaction_disp` GROUP by Branch) b on a.branch = b.branch";
            //data3 = Connection.sql_Customgettabledata(sqls);
            //datatable_grid.DataContext = data3;
        }
        

        public Home()
        {
            InitializeComponent();
        }

        private void livechartfillvalue()
        {
            for (int i = 0; i < mDates.Count; i++)
            {

                //modifying any series values will also animate and update the chart
                //string sql = "SELECT b.branch, a.s FROM (SELECT id,Branch, SUM(`total amount`) as s FROM `transaction_disp` WHERE date = '" + DateTime.Parse(mDates[i]).ToString("yyyy-MM-dd") + "' GROUP by Branch) a RIGHT join (SELECT * FROM `transaction_disp` GROUP by Branch) b on a.branch = b.branch";
                string sql = "SELECT b.branch, a.s FROM (SELECT balance_logs.id,client.Branch, SUM(`amount total`) as s FROM `balance_logs` left join client on balance_logs.client = client.ID WHERE date = '" + DateTime.Parse(mDates[i]).ToString("yyyy-MM-dd") + "' GROUP by Branch) a RIGHT join (SELECT Branch FROM `client` GROUP by Branch) b on a.branch = b.branch";
                data3 = Connection.sql_Customgettabledata(sql);
                foreach (DataRow row in data3.Rows)
                {
                    int indexes = titles.IndexOf(row[0].ToString());

                    double val;
                    double a = 0;
                    try
                    {
                        double.TryParse(SeriesCollection[indexes].Values[SeriesCollection[indexes].Values.Count - 1].ToString(), out a);
                    }
                    catch (Exception)
                    {
                    }
                    
                    
                    if (Double.TryParse(row[1].ToString(), out val)) { 
                        SeriesCollection[indexes].Values.Add(a + val);
                    }
                    else
                        SeriesCollection[indexes].Values.Add(a);
                        //SeriesCollection[indexes].Values.Add(double.Parse("0"));

                }
                //await Task.Delay(100);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Success");
        }
    }
}
