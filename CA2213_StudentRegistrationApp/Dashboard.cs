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
    public partial class Dashboard : Form
    {
       
        public Dashboard()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void createUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserForm myForm = new UserForm();
            myForm.MdiParent = this;
            myForm.Show();
        }

        private void facultyToolStripMenuItem_Click(object sender, EventArgs e)
        {
          ClassForm classForm = new ClassForm();    
            classForm.MdiParent = this;
            classForm.Show();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            this.Text = $"Dashboard";
           Welcome.Text += MainClass.username;
         name.Text = MainClass.usertype.ToString()+" Dashboard";
        }

        private void programToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SubjectForm subjectForm = new SubjectForm();
            subjectForm.MdiParent = this;
            subjectForm.Show();
        }

        private void studentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudentForm studentForm = new StudentForm();
            studentForm.MdiParent = this;
            studentForm.Show();
            studentForm.BringToFront();
        }

        private void paymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PaymentForm paymentForm = new PaymentForm();
            paymentForm.MdiParent = this;
            paymentForm.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void studentsReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudentsInfoReport studentsInfoReport = new StudentsInfoReport();
            studentsInfoReport.MdiParent = this;
            studentsInfoReport.Show();
        }

        private void paymentReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PaymentReport paymentReport = new PaymentReport();
            paymentReport.MdiParent = this;
            paymentReport.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
