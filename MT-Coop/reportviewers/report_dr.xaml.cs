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
    /// Interaction logic for report_dr.xaml
    /// </summary>
    public partial class report_dr : Window
    {
        /// <summary>
        /// replace_name.Text = data[0];
        /// replace_num.Text = data[1];
        /// replace_date.Text = data[2];
        /// replace_reb.Text = data[3];
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataTable"></param>
        public report_dr(string[] data, DataTable dataTable)
        {
            InitializeComponent();

            CrystalReportsViewer report_viewers = report_viewer;
            ReportDocument report = new ReportDocument();
            report.Load("../../report/crystal_delivery_receipt.rpt");
            report.SetDataSource(dataTable);

            TextObject replace_name =(TextObject)report.ReportDefinition.ReportObjects["Text_name"];
            TextObject replace_num =(TextObject)report.ReportDefinition.ReportObjects["Text_drnumber"];
            TextObject replace_date = (TextObject)report.ReportDefinition.ReportObjects["Text_date"];
            TextObject replace_reb = (TextObject)report.ReportDefinition.ReportObjects["Text_reb"];
            TextObject replace_note = (TextObject)report.ReportDefinition.ReportObjects["Text_note"];

            replace_name.Text = "Company Name: " + data[0];
            replace_num.Text = "DR #:  " + data[1];
            replace_date.Text = "Date: " + data[2];
            replace_reb.Text = "Received by: "+data[3];
            replace_note.Text = "Note: "+data[4];


            report_viewers.ViewerCore.ReportSource = report;
        }
    }
}
