using CrystalDecisions.CrystalReports.Engine;
using SAPBusinessObjects.WPF.Viewer;
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

namespace MT_Coop.reportviewers
{
    /// <summary>
    /// Interaction logic for report_inventory.xaml
    /// </summary>
    public partial class report_inventory : Window
    {
        public report_inventory(string[] data, DataTable dataTable)
        {
            InitializeComponent();


            CrystalReportsViewer report_viewers = report_viewer;
            ReportDocument report = new ReportDocument();
            report.Load("../../report/crystal_inventory.rpt");
            report.SetDataSource(dataTable);
            
            TextObject replace_date = (TextObject)report.ReportDefinition.ReportObjects["Text_date"];
            replace_date.Text = "" + data[0];


            report_viewers.ViewerCore.ReportSource = report;
        }
    }
}
