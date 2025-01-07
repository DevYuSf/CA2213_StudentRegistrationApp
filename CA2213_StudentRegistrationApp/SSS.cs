using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CA2213_StudentRegistrationApp
{
    public partial class FacultyForm : Form
    {
        MainClass mc = new MainClass();
        public FacultyForm()
        {
            InitializeComponent();
        }
        private void Reset()
        {
            mc.query = "select * from TblFaculties";
            mc.Display(mc.query, dataGridView1);
            mc.Clear(txtId, txtFaculty, txtSearch);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtId.Text!="" && txtFaculty.Text!="")
            {
                mc.query = $"insert into TblFaculties values ({txtId.Text},'{txtFaculty.Text}')";
                mc.ProcessData(mc.query, mc.insertAlert, "");
                Reset();
            }
            else
            {
                MessageBox.Show("Please fill blank spaces", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtId.Text != "" && txtFaculty.Text != "")
            {
                mc.query = $"update TblFaculties set Faculty = '{txtFaculty.Text}' where FcId={txtId.Text}";
                mc.ProcessData(mc.query, mc.updateAlert, "");
                Reset();
            }
            else
            {
                MessageBox.Show("Please fill blank spaces", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            mc.query = $"delete from TblFaculties where FcId={txtId.Text}";
            mc.ProcessData(mc.query, mc.deleteAlert, "");
            Reset();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            mc.GetDataFromDGV(dataGridView1, e, txtId, txtFaculty);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            mc.query = $"select * from TblFaculties where Faculty like '%{txtSearch.Text}%'";
            mc.Display(mc.query, dataGridView1);
        }

        private void FacultyForm_Load(object sender, EventArgs e)
        {

        }
    }
}
