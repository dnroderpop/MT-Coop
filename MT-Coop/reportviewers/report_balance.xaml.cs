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
using System.Windows.Shapes;
using MT_Coop.Database_Connection;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SAPBusinessObjects.WPF.Viewer;
using CrystalDecisions.Shared;
using MT_Coop.Database_Connection;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SAPBusinessObjects.WPF.Viewer;
using CrystalDecisions.Shared;
using System.Data;

namespace MT_Coop.reportviewers
{
    /// <summary>
    /// Interaction logic for report_balance.xaml
    /// </summary>
    public partial class report_balance : Window
    {
        public report_balance(DataTable dataTable)
        {
            InitializeComponent();
            CrystalReportsViewer report_viewers = report_viewer;
            ReportDocument report = new ReportDocument();
            report.Load("../../report/crystal_balance.rpt");
            report.SetDataSource(dataTable);

            report_viewers.ViewerCore.ReportSource = report;
        }
    }
}
