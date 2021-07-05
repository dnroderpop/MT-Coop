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
using System.Windows.Shapes;
using MT_Coop.Database_Connection;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SAPBusinessObjects.WPF.Viewer;
using CrystalDecisions.Shared;

namespace MT_Coop.reportviewers
{
    /// <summary>
    /// Interaction logic for report_credit_request_slip.xaml
    /// </summary>
    public partial class report_credit_request_slip : Window
    {
        connn connection = new connn();
        /// <summary>
        /// Please before you call this module, ready the values and information to be passed
        /// </summary>
        /// <param name="Data">use for name, balance, branch, date raw, due raw</param>
        /// <param name="dataTable">use table Cart ("product id, name uom quantity price total")</param>
        public report_credit_request_slip(String[] Data, DataTable dataTable)
        {
            bool cash = false;
            InitializeComponent();
            String id = Properties.Settings.Default.userid;
            string user = connection.Sql_getstringofCustomSQL("user", "fn", "where uname = '" + id + "'").ToUpper();
            user += " " + connection.Sql_getstringofCustomSQL("user", "ln", "where uname = '" + id + "'").ToUpper();
            String name, balance, branch, date, due;
            CrystalReportsViewer report_viewers = report_viewer;
            ReportDocument report = new ReportDocument();
            report.Load("../../report/crystal_credit_request_slip.rpt");
            report.SetDataSource(dataTable);
            name = Data[0].ToString();
            balance = Data[1];
            branch = Data[2];
            date = DateTime.Parse(Data[3]).ToString("dd MMMM yyyy");
            due = DateTime.Parse(Data[4]).ToString("dd MMMM yyyy");

            cash = bool.Parse(connection.Sql_getstringofCustomSQL("transactions", "Cash", "Where `Transaction Number` = '" + Data[5] + "'"));

            //replace name balance branch date due user
            TextObject replace_name = (TextObject)report.ReportDefinition.ReportObjects["replace_name"];
            TextObject replace_balance = (TextObject)report.ReportDefinition.ReportObjects["replace_balance"];
            TextObject replace_branch = (TextObject)report.ReportDefinition.ReportObjects["replace_branch"];
            TextObject replace_date = (TextObject)report.ReportDefinition.ReportObjects["replace_date"];
            TextObject replace_due = (TextObject)report.ReportDefinition.ReportObjects["replace_due"];
            TextObject replace_user = (TextObject)report.ReportDefinition.ReportObjects["replace_user"];
            TextObject replace_cn = (TextObject)report.ReportDefinition.ReportObjects["replace_cn"];
            TextObject replace_cash = (TextObject)report.ReportDefinition.ReportObjects["replace_cash"];

            if (cash)
            {
                replace_cash.Text = "COOP CASH REQUEST SLIP";
            }
            else
            {
                replace_cash.Text = "COOP CREDIT REQUEST SLIP";
            }

            replace_name.Text = name;
            replace_balance.Text = balance;
            replace_branch.Text = branch;
            replace_date.Text = date;
            replace_due.Text = due;
            replace_user.Text = user;
            replace_cn.Text = Data[5];

            report_viewers.ViewerCore.ReportSource = report;



        }
    }
}
