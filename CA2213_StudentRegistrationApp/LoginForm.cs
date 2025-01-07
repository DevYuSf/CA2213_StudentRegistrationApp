using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CA2213_StudentRegistrationApp
{
    public partial class LoginForm : Form
    {
        MainClass mc = new MainClass();
        public bool IsLoginSuccessful = false;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUsername.Text != "" && txtPassword.Text != "")
                {
                    mc.query = $"select UserName,UserType from TblUser where UserName='{txtUsername.Text}' and _password='{txtPassword.Text}'";
                    using (mc.cmd = new SqlCommand(mc.query, mc.con))
                    {
                        mc.Connect();
                        SqlDataReader dr = mc.cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            MainClass.username = dr.GetValue(0).ToString();
                            MainClass.usertype = dr.GetValue(1).ToString();

                            IsLoginSuccessful = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        mc.Disconnect();
                    }
                }
                else
                {
                    MessageBox.Show("Username and password are required.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                mc.Disconnect();
                MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
