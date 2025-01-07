using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CA2213_StudentRegistrationApp
{
    public partial class PaymentReport : Form
    {
        MainClass mc = new MainClass();
        public PaymentReport()
        {
            InitializeComponent();
        }

        private void PaymentReport_Load(object sender, EventArgs e)
        {
            GetReport();
            LoadSubjects();
            comboSubject.SelectedIndex = 0;
            comboPaid.SelectedIndex = 0;
           // this.reportViewer2.RefreshReport();
        }
        private void GetReport(string qu = "select*from StudentPayment")
        {
            try
            {
                mc.query = qu;
                using (SqlDataAdapter da = new SqlDataAdapter(mc.query, mc.con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "StudentPayment");
                    ReportDataSource reportDataSource = new ReportDataSource("paymentDataSet", ds.Tables[0]);
                    string rptPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\PaymentReport.rdlc";
                    reportViewer2.LocalReport.ReportPath = rptPath;
                    reportViewer2.LocalReport.DataSources.Clear();
                    reportViewer2.LocalReport.DataSources.Add(reportDataSource);

                }
                this.reportViewer2.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void LoadSubjects()
        {
            mc.query = "SELECT SubjectName FROM TblSubject"; // Use the existing query variable

            using (var cmd = new SqlCommand(mc.query, mc.con))
            {
                try
                {
                    mc.Connect();
                    SqlDataReader reader = cmd.ExecuteReader();
                    comboSubject.Items.Clear(); // Clear existing items
                    comboSubject.Items.Add("select one..");
                    while (reader.Read())
                    {
                        
                        comboSubject.Items.Add(reader["SubjectName"].ToString());
                    }
                    mc.Disconnect();
                }
                catch (Exception ex)
                {
                    mc.Disconnect();
                    MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
       


        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            GetReport($"Select*from StudentPayment where StdName like '%{txtSearch.Text}%'");
        }

        private void comboSubject_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboSubject.SelectedIndex == 0)
            {
                GetReport($"Select*from StudentPayment");
            }
            else
            {
                GetReport($"Select*from StudentPayment where Subjects like '%{comboSubject.Text}%'");
            }
        }

        private void comboPaid_SelectedValueChanged_1(object sender, EventArgs e)
        {
            if (comboPaid.SelectedIndex == 0)
            {
                GetReport($"Select*from StudentPayment");
            }
            else if(comboPaid.SelectedIndex == 1)
            {
                GetReport($"Select*from StudentPayment where paid = {1}");
            }else if(comboPaid.SelectedIndex == 2)
            {
                GetReport($"Select*from StudentPayment where paid = {0}");
            }
        }
    }
}
