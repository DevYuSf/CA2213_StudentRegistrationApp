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
    public partial class ClassForm : Form
    {
        MainClass mc =new MainClass();
        public ClassForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtClass.Text != "")
            {
                mc.query = $"insert into TblClass (className) values ('{txtClass.Text}')";
                mc.ProcessData(mc.query, mc.insertAlert,"");
                Reset();
            }
            else
            {
                MessageBox.Show("Please fill blank spaces", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Reset()
        {
            mc.query = "select * from TblClass";
            mc.Display(mc.query, dataGridView1);
            txtClass.Text = "";
            txtClass.Focus();
        }

        private void ClassForm_Load(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            mc.query = $"update TblClass set ClassName ='{txtClass.Text}' where ClassId = {lbl.Text} ";
            mc.ProcessData(mc.query, mc.updateAlert,"");
            Reset();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            mc.GetDataFromDGV2(dataGridView1, e,lbl,txtClass);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<int> subId = new List<int>();
            DialogResult result = MessageBox.Show(
                "Please do not delete class \n otherwise you will lose more data \n  cancel please?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result == DialogResult.Yes)
            {
                //mc.Disconnect();
                mc.query = $"select subjectId from TblSubject where classId = {lbl.Text}";
                using (var cmd = new SqlCommand(mc.query, mc.con))
                {
                    try
                    {
                        mc.Connect();
                        SqlDataReader reader = cmd.ExecuteReader();
                        // checkedListBoxSubjects.Items.Clear(); // Clear existing items
                       
                            while (reader.Read())
                            {
                                subId.Add(reader.GetInt32(0));
                            }
                       
                       
                        mc.Disconnect();
                       

                        for (int i = 0; i < subId.Count; i++)
                        {
                            //MessageBox.Show(lbl.Text+" Hye hee "+i+" "+subId[i].ToString());
                            mc.query = $"delete from TblStudentDiscount where SubjectId = {subId[i]}";
                            mc.ProcessData2(mc.query, "");
                            mc.query = $"delete from TblStudentSubject where SubjectId ={subId[i]}";
                            mc.ProcessData2(mc.query, "");
                        }
                        mc.query = $"delete from TblSubject where ClassId = {lbl.Text}";
                        mc.ProcessData2(mc.query, "");
                        mc.query = $"delete from TblClass where ClassId = {lbl.Text}";
                        mc.ProcessData2(mc.query, "");
                        mc.Disconnect();
                        MessageBox.Show(mc.deleteAlert);
                      
                    }
                    catch (Exception ex)
                    {
                        mc.Disconnect();
                        MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // Call the method to delete the student
                //mc.query = $"DELETE FROM TblStudentDiscount WHERE StudentId = @StudentId;";
                //mc.ProcessData2(mc.query, id);
                //mc.query = $"DELETE FROM TblStudentSubject WHERE StudentId = @StudentId;";
                //mc.ProcessData2(mc.query, id);
                //mc.query = $"DELETE FROM TblStudent WHERE StudentId = @StudentId;";
                //mc.ProcessData(mc.query, mc.deleteAlert, id);
                Reset();
            }
            else
            {
                // Operation canceled, do nothing
                Reset();
                return;
            }
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            mc.query = $"select * from TblClass where ClassName like '%{txtSearch.Text}%'";
            mc.Display(mc.query, dataGridView1);
        }
    }
}
