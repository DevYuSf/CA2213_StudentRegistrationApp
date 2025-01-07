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
    public partial class StudentsInfoReport : Form
    {
        MainClass mc = new MainClass();
        public StudentsInfoReport()
        {
            InitializeComponent();
        }

        private void StudentsInfoReport_Load(object sender, EventArgs e)
        {

            GetReport();
            LoadSubjects();
            LoadClasses();
        }
        private void GetReport(string qu = "select*from StudentInfo")
        {
            try
            {
                mc.query = qu;
                using (SqlDataAdapter da = new SqlDataAdapter(mc.query, mc.con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "StudentInfo");
                    ReportDataSource reportDataSource = new ReportDataSource("StudentsInfoDataSet1", ds.Tables[0]);
                    string rptPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\StudentsInfoReport.rdlc";
                    reportViewer1.LocalReport.ReportPath = rptPath;
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    
                }
                this.reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {MessageBox.Show(ex.Message,"error...",MessageBoxButtons.OK,MessageBoxIcon.Error);
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
        private void LoadClasses()
        {
            mc.query = "SELECT ClassName FROM TblClass"; // Use the existing query variable

            using (var cmd = new SqlCommand(mc.query, mc.con))
            {
                try
                {
                    mc.Connect();
                    SqlDataReader reader = cmd.ExecuteReader();
                    comboClass.Items.Clear(); // Clear existing items

                    while (reader.Read())
                    {
                        comboClass.Items.Add(reader["ClassName"].ToString());
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
            GetReport($"Select*from StudentInfo where StdName like '%{txtSearch.Text}%'");
        }

        private void comboClass_SelectedValueChanged(object sender, EventArgs e)
        {
            GetReport($"Select*from StudentInfo where Classes like '%{comboClass.Text}%'");

        }

        private void comboSubject_SelectedValueChanged(object sender, EventArgs e)
        {
            GetReport($"Select*from StudentInfo where Subjects like '%{comboSubject.Text}%'");

        }
    }
}
