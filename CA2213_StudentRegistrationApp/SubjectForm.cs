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
    public partial class SubjectForm : Form
    {
        MainClass mc = new MainClass();
        public SubjectForm()
        {
            InitializeComponent();
        }

        private void SubjectForm_Load(object sender, EventArgs e)
        {
            LoadClasses();
            Reset();
        }

        private void Reset()
        {
            mc.query = "select * from TblSubject";
            mc.Display(mc.query, dataGridView1);
            txtFee.Clear();
            txtSubject.Clear();
            comboClass.SelectedIndex = 0;
            txtSubject.Focus();
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
        private int GetClassId()
        {
            int ClassId = 0;
            mc.query = $"SELECT ClassId FROM TblClass where ClassName ='{comboClass.Text}' "; // Use the existing query variable

            using (var cmd = new SqlCommand(mc.query, mc.con))
            {
                try
                {
                    mc.Connect();
                    SqlDataReader reader = cmd.ExecuteReader();
                    comboClass.Items.Clear(); // Clear existing items

                    while (reader.Read())
                    {
                        ClassId = reader.GetInt32(0);
                        // comboClass.Items.Add(reader["ClassName"].ToString());
                    }
                    mc.Disconnect();
                }
                catch (Exception ex)
                {
                    mc.Disconnect();
                    MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return ClassId;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mc.query = $"insert into TblSubject(SubjectName,fee,classId) values ('{txtSubject.Text}','{txtFee.Text}',{GetClassId()})";
            mc.ProcessData(mc.query, mc.insertAlert, "");
            LoadClasses();
            Reset();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            mc.query = $"update TblSubject set SubjectName = '{txtSubject.Text}', fee = '{txtFee.Text}', classId = {GetClassId()} where SubjectId = {lbl.Text}";
            mc.ProcessData(mc.query, mc.updateAlert, "");
            LoadClasses();
            Reset();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                //lbl waa hidden label ku dhexjira designka dhanka hoose kaso heyo Id ga maaadada
                mc.GetDataFromDGV2(dataGridView1, e, lbl, txtSubject, comboClass, txtFee);
                GetClassName();
                LoadClasses();
            }
           
        }
        private void GetClassName()
        {
            string name = "";
            mc.query = $"SELECT ClassName FROM TblClass where ClassId ={comboClass.Text} "; // Use the existing query variable

            using (var cmd = new SqlCommand(mc.query, mc.con))
            {
                try
                {
                    mc.Connect();
                    SqlDataReader reader = cmd.ExecuteReader();
                    comboClass.Items.Clear(); // Clear existing items

                    while (reader.Read())
                    {
                        name = reader.GetString(0);
                        // comboClass.Items.Add(reader["ClassName"].ToString());
                    }
                    mc.Disconnect();
                }
                catch (Exception ex)
                {
                    mc.Disconnect();
                    MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            comboClass.Text = name;


        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
               "Do you want to delete this subject?",
               "Confirm Deletion",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question
           );
            if (result == DialogResult.Yes)
            {
                mc.query = $"delete TblStudentSubject where SubjectId = {lbl.Text}";
                mc.ProcessData2(mc.query, "");
                mc.query = $"delete TblStudentDiscount where SubjectId = {lbl.Text}";
                mc.ProcessData2(mc.query, "");
                mc.query = $"delete TblSubject where SubjectId = {lbl.Text}";
                mc.ProcessData2(mc.query, "");
                MessageBox.Show(mc.deleteAlert, "Deleting...");
                
                Reset();
            }
            else {
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
            mc.query = $"select * from TblSubject where SubjectName like '%{txtSearch.Text}%'";
            mc.Display(mc.query, dataGridView1);
        }
    }
}
