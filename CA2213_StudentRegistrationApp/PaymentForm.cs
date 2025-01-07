using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CA2213_StudentRegistrationApp
{
    public partial class PaymentForm : Form
    {
        MainClass mc = new MainClass();
        public PaymentForm()
        {
            InitializeComponent();

            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }


        private void PaymentForm_Load(object sender, EventArgs e)
        {
            Reset();
            UpdateStudentCounts();
        }
        private void Reset()
        {
            mc.query = "select*from StudentPayment";
            mc.Display2(mc.query, dataGridView1);
        
            comboYear.SelectedIndex = 0;
            comboSelectMonth.SelectedIndex = 0;
            comboSelectMonth.Focus();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
            if (e.ColumnIndex == dataGridView1.Columns["Paid"].Index && e.RowIndex >= 0)
            {
                // Get the current row
                var currentRow = dataGridView1.Rows[e.RowIndex];

                // Get the current value of the checkbox
                bool currentValue = (bool)currentRow.Cells["Paid"].Value;

                // Toggle the checkbox value
                bool newValue = !currentValue;
                currentRow.Cells["Paid"].Value = newValue;

                // Update the database with the new value
                UpdatePaymentStatus(currentRow.Cells["StudentId"].Value.ToString(), newValue);
              //  Reset();
                UpdateStudentCounts();
            }
        }
            private void UpdatePaymentStatus(string studentId, bool isPaid)
        {

            mc.Connect();
                string query = "UPDATE TblMonth SET paid = @Paid WHERE StudentId = @StudentId";
                using (var cmd = new SqlCommand(query, mc.con))
                {
                    cmd.Parameters.AddWithValue("@Paid", isPaid ? "yes" : "no");
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.ExecuteNonQuery();
                }
          
        }
        private void UpdateStudentCounts()
        {
            int totalStudents = dataGridView1.Rows.Count;
            int paidStudents = dataGridView1.Rows.Cast<DataGridViewRow>()
                                 .Count(row => row.Cells["Paid"].Value != null && (bool)row.Cells["Paid"].Value);
            int unpaidStudents = totalStudents - paidStudents;
            // Update UI elements
            totalStd.Text = totalStudents.ToString();
            lblPaid.Text = paidStudents.ToString();
            lblUnpaid.Text = unpaidStudents.ToString();
        }

        private void btnAddMonth_Click(object sender, EventArgs e)
        {
            List<int> StdInt = new List<int>();
            DateTime dateTime = DateTime.Now;
            int month = dateTime.Month;
            int year = dateTime.Year;
            try
            {
                mc.query = "SELECT COUNT(*) FROM TblMonth WHERE MONTH(MonthDate) = @Month AND YEAR(MonthDate) = @Year";
                
                using (SqlCommand command = new SqlCommand(mc.query, mc.con))
                {
                    mc.Connect();
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("This month and year already exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        mc.Disconnect();
                        return; // Exit the function if the month and year exist
                    }
                }
                mc.query = "select StudentId from TblStudent where status = 'Active'";
                using (var cmd = new SqlCommand(mc.query, mc.con))
                {
                    try
                    {
                        mc.Connect();
                        SqlDataReader reader = cmd.ExecuteReader();
                        // comboClass.Items.Clear(); // Clear existing items

                        while (reader.Read())
                        {
                            // comboClass.Items.Add(reader["ClassName"].ToString());
                            StdInt.Add(reader.GetInt32(0));
                            // mc.query = $"insert into TblMonth (StudentId,paid) values({StudentId},'no');";
                            // mc.ProcessData(mc.query,mc.insertAlert,"");
                        }
                        // 
                        mc.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        mc.Disconnect();
                        MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                for (int i = 0; i < StdInt.Count; i++)
                {
                    mc.query = $"insert into TblMonth (StudentId,paid) values({StdInt[i]},'no');";
                    mc.ProcessData2(mc.query, "");
                }
                MessageBox.Show("Month Successfully Created");
                Reset();
                UpdateStudentCounts();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "error..", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            
            
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                // Commit the edit to trigger the CellContentClick event
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void comboSelectMonth_SelectedValueChanged(object sender, EventArgs e)
        {
           
            mc.query = $"SELECT stu.StudentId,stu.StdName,  STRING_AGG(sub.SubjectName, ', ') AS Subjects, STRING_AGG(c.ClassName, ', ') AS Classes, SUM(sub.Fee) - SUM(sd.DiscountAmount) AS Amount, (CASE WHEN m.paid = 'yes' THEN 1 ELSE 0 END) AS Paid FROM TblMonth m JOIN TblStudent stu ON m.StudentId = stu.StudentId JOIN TblStudentSubject ts ON ts.StudentId = stu.StudentId LEFT JOIN TblSubject sub ON ts.SubjectId = sub.SubjectId LEFT JOIN TblClass c ON sub.ClassId = c.ClassId LEFT JOIN TblStudentDiscount sd ON stu.StudentId = sd.StudentId AND ts.SubjectId = sd.SubjectId WHERE  FORMAT(m.MonthDate, 'MMMM') ='{comboSelectMonth.Text}' AND YEAR(m.MonthDate) ={comboYear.Text} GROUP BY stu.StdName, stu.StudentId, m.Paid  order by m.paid desc;";

            mc.Display2(mc.query, dataGridView1);
            UpdateStudentCounts();
           
        }

        private void comboYear_SelectedValueChanged(object sender, EventArgs e)
        {
   
            mc.query = $"SELECT stu.StudentId,stu.StdName,  STRING_AGG(sub.SubjectName, ', ') AS Subjects, STRING_AGG(c.ClassName, ', ') AS Classes, SUM(sub.Fee) - SUM(sd.DiscountAmount) AS Amount, (CASE WHEN m.paid = 'yes' THEN 1 ELSE 0 END) AS Paid FROM TblMonth m JOIN TblStudent stu ON m.StudentId = stu.StudentId JOIN TblStudentSubject ts ON ts.StudentId = stu.StudentId LEFT JOIN TblSubject sub ON ts.SubjectId = sub.SubjectId LEFT JOIN TblClass c ON sub.ClassId = c.ClassId LEFT JOIN TblStudentDiscount sd ON stu.StudentId = sd.StudentId AND ts.SubjectId = sd.SubjectId WHERE  FORMAT(m.MonthDate, 'MMMM') ='{comboSelectMonth.Text}' AND YEAR(m.MonthDate) ={comboYear.Text} GROUP BY stu.StdName, stu.StudentId, m.Paid  order by m.paid desc;";

            mc.Display2(mc.query, dataGridView1);
            UpdateStudentCounts();

        }
   

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            mc.query = $"SELECT stu.StudentId,stu.StdName,  STRING_AGG(sub.SubjectName, ', ') AS Subjects, STRING_AGG(c.ClassName, ', ') AS Classes, SUM(sub.Fee) - SUM(sd.DiscountAmount) AS Amount, (CASE WHEN m.paid = 'yes' THEN 1 ELSE 0 END) AS Paid FROM TblMonth m JOIN TblStudent stu ON m.StudentId = stu.StudentId JOIN TblStudentSubject ts ON ts.StudentId = stu.StudentId LEFT JOIN TblSubject sub ON ts.SubjectId = sub.SubjectId LEFT JOIN TblClass c ON sub.ClassId = c.ClassId LEFT JOIN TblStudentDiscount sd ON stu.StudentId = sd.StudentId AND ts.SubjectId = sd.SubjectId WHERE  FORMAT(m.MonthDate, 'MMMM') ='{comboSelectMonth.Text}' AND YEAR(m.MonthDate) ={comboYear.Text} and StdName like '%{txtSearch.Text}%' GROUP BY stu.StdName, stu.StudentId, m.Paid  order by m.paid desc;";

            mc.Display2(mc.query, dataGridView1);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
