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
    public partial class UserForm : Form
    {
        MainClass mc = new MainClass();
        public UserForm()
        {
            InitializeComponent();
        }
        private void Reset()
        {
            mc.query = "select * from TblUser";
            mc.Display(mc.query,dataGridView1);
            mc.Clear(lbl,txtUsername,txtPassword,txtConfirm,txtSearch);
            comboBoxType.SelectedIndex = 0;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                if (comboBoxType.SelectedIndex != 0 && comboBoxType.Text!="")
                {
                    if (txtPassword.Text == txtConfirm.Text)
                    {
                        mc.query = $"insert into TblUser values ('{txtUsername.Text}','{txtPassword.Text}','{comboBoxType.Text}')";
                        mc.ProcessData2(mc.query, "");
                        MessageBox.Show(mc.insertAlert, "Insert user",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        Reset();
                    }
                    else
                    {
                        MessageBox.Show("Password mismatch!", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select user type.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please fill blank spaces.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            Reset();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {      
            mc.GetDataFromDGV2(dataGridView1, e,lbl, txtUsername, txtPassword, comboBoxType);
            txtConfirm.Text = txtPassword.Text;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                if (comboBoxType.SelectedIndex != 0 && comboBoxType.Text != "")
                {
                    if (txtPassword.Text == txtConfirm.Text)
                    {
                        mc.query = $"update TblUser set UserName = '{txtUsername.Text}', _password ='{txtPassword.Text}', UserType='{comboBoxType.Text}' where UserId = {lbl.Text}";
                        mc.ProcessData2(mc.query,lbl.Text);
                        MessageBox.Show(mc.updateAlert,"Update User",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        Reset();
                    }
                    else
                    {
                        MessageBox.Show("Password mismatch!", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select user type.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please fill blank spaces.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            mc.query = $"delete from TblUser where UserId = {lbl.Text}";
            mc.ProcessData2(mc.query, "");
            MessageBox.Show(mc.deleteAlert,"delete user",MessageBoxButtons.OK,MessageBoxIcon.Information);
            Reset();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            mc.query = $"select * from Tbluser where userName like '%{txtSearch.Text}%'";
            mc.Display(mc.query, dataGridView1);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
