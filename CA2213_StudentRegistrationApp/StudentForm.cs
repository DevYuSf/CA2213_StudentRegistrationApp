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
    public partial class StudentForm : Form
    {
        MainClass mc = new MainClass();
        string id;
        public StudentForm()
        {
            InitializeComponent();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSaveStd_Click(object sender, EventArgs e)
        {
            // Calculate age
            int age = DateTime.Now.Year - dateOfBirth.Value.Year;
            // Prepare the SQL query with parameters
            string query = @"INSERT INTO TblStudent (StdName, Gender, Status, Mobile, Address, Age, DateOfReg, DateOfBirth) 
        VALUES (@StdName, @Gender, @Status, @Mobile, @Address, @Age, @DateOfReg, @DateOfBirth);SELECT SCOPE_IDENTITY()";

            using (var sqlCommand = new SqlCommand(query, mc.con))
            {
                // Add parameters
                sqlCommand.Parameters.AddWithValue("@StdName", txtName.Text);
                sqlCommand.Parameters.AddWithValue("@Gender", comboGender.Text);
                sqlCommand.Parameters.AddWithValue("@Status", comboStatus.Text);
                sqlCommand.Parameters.AddWithValue("@Mobile", txtMobile.Text);
                sqlCommand.Parameters.AddWithValue("@Address", txtAddress.Text);
                sqlCommand.Parameters.AddWithValue("@Age", age);
                sqlCommand.Parameters.AddWithValue("@DateOfReg", DateTime.Now.Date); // Today’s date
                sqlCommand.Parameters.AddWithValue("@DateOfBirth", dateOfBirth.Value.Date); // Only date portion                
               
                try
                {
                    if( decimal.Parse(txtFee.Text) <= totalFee() )
                    {
                        try
                        {
                            if (int.Parse(txtMobile.Text)>=0 && txtMobile.Text.Length >6){
                                try
                                {
                                    mc.Connect();
                                    // sqlCommand.ExecuteNonQuery();
                                    int studentId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                                    if (studentId > 0)
                                    {
                                        mc.Disconnect();
                                        SaveSelectedSubjects(studentId);
                                        discount(studentId);
                                        MessageBox.Show("Student saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        Reset();
                                        for (int i = 0; i < checkedListBoxSubjects.Items.Count; i++)
                                        {
                                            checkedListBoxSubjects.SetItemChecked(i, false); // Uncheck all items
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    mc.Disconnect();
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Fadlan Numberka iska sax", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                    else
                    {
                        MessageBox.Show("Lacagta aad soo gelisay wey kabadan tahay lacagta madoyinkan lagu dhigto!", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                catch
                {
                    MessageBox.Show("booska lacagta waxad so gelisay xaraf!", "error..", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
               
            }       
      
        }
        //
        private void SaveSelectedSubjects(int studentId)
        {
            foreach (var item in checkedListBoxSubjects.CheckedItems)
            {
                string subjectName = item.ToString();
                int subjectId = GetSubjectId(subjectName); // Method to fetch SubjectId based on name
                if (subjectId > 0)
                {
                    string query = "INSERT INTO TblStudentSubject (StudentId, SubjectId) VALUES (@StudentId, @SubjectId)";
                    using (var cmd = new SqlCommand(query, mc.con))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                        mc.Connect();
                        cmd.ExecuteNonQuery();
                        mc.Disconnect();
                    }
                }
            }
        }
        // get subject id
        private int GetSubjectId(string subjectName)
        {
            int subjectId = 0;
            string query = "SELECT SubjectId FROM TblSubject WHERE SubjectName = @SubjectName";
                using (var cmd = new SqlCommand(query, mc.con))
                {
                    cmd.Parameters.AddWithValue("@SubjectName", subjectName);
                    mc.Connect();
                    object result = cmd.ExecuteScalar();
                    mc.Disconnect();
                    if (result != null)
                    {
                        subjectId = Convert.ToInt32(result);
                    }
                } 
            return subjectId;
        }
        //get selected subject fee
        private decimal GetSelectedSubjectFee(string subjectName)
        {
            decimal fee = 0;
            string query = "SELECT fee FROM TblSubject WHERE SubjectName = @SubjectName";
            using (var cmd = new SqlCommand(query, mc.con))
            {
                cmd.Parameters.AddWithValue("@SubjectName", subjectName);
                mc.Connect();
                object result = cmd.ExecuteScalar();
                mc.Disconnect();
                if (result != null)
                {
                    fee = Convert.ToDecimal(result);
                }
            }
            return fee;
        }
        //get the total fee subjects
        private decimal totalFee()
        {
            decimal total = 0;
            foreach (var item in checkedListBoxSubjects.CheckedItems)
            {
                string subjectName = item.ToString();
                total += GetSelectedSubjectFee(subjectName); // Method to fetch fee based on name
               
            }
            return total;
        }
        private void Reset()
        {
            mc.query = "select * from StudentInfo";
            mc.Display(mc.query, dataGridView1);
            mc.Clear(txtName, txtAddress, txtMobile, txtSearch,txtFee);
            comboGender.SelectedIndex = 0;
            comboStatus.SelectedIndex = 0;
            dateOfBirth.Value = DateTime.Now;
            for (int i = 0; i < checkedListBoxSubjects.Items.Count; i++)
            {
                checkedListBoxSubjects.SetItemChecked(i, false);
            }
            txtName.Focus();    
            //  dateOfReg.Value = DateTime.Now;
        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            Reset();
            LoadSubjects();
            //test.Text = (DateTime.Now.Date.ToString("dd-MM-yyyy"));
        }

        private void test_Click(object sender, EventArgs e)
        {

        }
        //discount
        private void discount(int _id)
        {
            decimal totalFee = 0;
            decimal disco = 0;
            List<int> subID = new List<int>();
            List<decimal> stdFee = new List<decimal>();
          
            mc.query = $"SELECT s.SubjectId, s.fee FROM TblSubject s, TblStudentSubject ss WHERE ss.StudentId = {_id} AND ss.SubjectId = s.SubjectId";
             using (mc.cmd = new SqlCommand(mc.query, mc.con))
               {
                    mc.Connect() ;
                    using (SqlDataReader dr = mc.cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                            subID.Add(dr.GetInt32(0)); // First column (SubjectId)
                                stdFee.Add(dr.GetDecimal(1)); // Second column (Fee)
                                totalFee += dr.GetDecimal(1);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No records found for the specified Student ID.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            
            if (totalFee >= decimal.Parse(txtFee.Text))
            {
                decimal amount = totalFee - decimal.Parse(txtFee.Text);
                disco = decimal.Parse((amount / subID.Count).ToString("n2"));
              
            }
         
            for (int i = 0; subID.Count > i; i++)
            {
                mc.query = $"INSERT INTO TblStudentDiscount (StudentId, SubjectId, DiscountAmount) VALUES ({_id}, {subID[i]}, {disco})";
           
                    using (mc.cmd = new SqlCommand(mc.query, mc.con))
                    {
                        mc.Connect();
                        int result = mc.cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                           // MessageBox.Show("Successfuly updated fee" + subID[i], "updated subject discount", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            MessageBox.Show("Error", "Error fee", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        mc.Disconnect();
                    }
                }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Do you want to delete this student?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Check the user's response
            if (result == DialogResult.Yes)
            {
                // Call the method to delete the student
                mc.query = $"DELETE FROM TblStudentDiscount WHERE StudentId = @StudentId;";
                mc.ProcessData2(mc.query,  id);
                mc.query = $"DELETE FROM TblStudentSubject WHERE StudentId = @StudentId;";
                mc.ProcessData2(mc.query, id);
                mc.query = $"DELETE FROM TblMonth WHERE StudentId = @StudentId;";
                mc.ProcessData2(mc.query, id);
                mc.query = $"DELETE FROM TblStudent WHERE StudentId = @StudentId;";
                mc.ProcessData2(mc.query, id);
                MessageBox.Show(mc.deleteAlert, "delete",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Reset();
            }
            else
            {
                // Operation canceled, do nothing
                Reset();
                return;
            }
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                id = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                //TextBox subjects;
                TextBox test2SUb = new TextBox();
                TextBox test = new TextBox();
                mc.GetDataFromDGV(dataGridView1, e, txtName, comboGender, txtAddress, test, comboStatus, txtMobile, test, dateOfBirth, test2SUb, test, test, test, txtFee);

                for (int i = 0; i < checkedListBoxSubjects.Items.Count; i++)
                {
                    checkedListBoxSubjects.SetItemChecked(i, false);
                }

                foreach (var subject in test2SUb.Text.Split(','))
                {
                    string trimmedSubject = subject.Trim();
                    for (int i = 0; i < checkedListBoxSubjects.Items.Count; i++)
                    {
                        if (checkedListBoxSubjects.Items[i].ToString() == trimmedSubject)
                        {
                            checkedListBoxSubjects.SetItemChecked(i, true);
                            break; // Exit the loop once the item is found and checked
                        }
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            mc.Connect();
            DialogResult result = MessageBox.Show(
                "Do you want to update this student?",
                "Confirm Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Prepare the SQL query with parameters
            string query = @"
        UPDATE TblStudent 
        SET 
            StdName = @StdName,
            Gender = @Gender,
            Status = @Status,
            Mobile = @Mobile,
            Address = @Address,
            Age = @Age,
            DateOfReg = @DateOfReg,
            DateOfBirth = @DateOfBirth
        WHERE 
            StudentId = @StudentId"; // Assuming you have a primary key named StudentId

            using (var sqlCommand = new SqlCommand(query, mc.con))
            {
                // Add parameters
                sqlCommand.Parameters.AddWithValue("@StdName", txtName.Text);
                sqlCommand.Parameters.AddWithValue("@Gender", comboGender.Text);
                sqlCommand.Parameters.AddWithValue("@Status", comboStatus.Text);
                sqlCommand.Parameters.AddWithValue("@Mobile", txtMobile.Text);
                sqlCommand.Parameters.AddWithValue("@Address", txtAddress.Text);
                sqlCommand.Parameters.AddWithValue("@Age", DateTime.Now.Year - dateOfBirth.Value.Year);
                sqlCommand.Parameters.AddWithValue("@DateOfReg", DateTime.Now.Date); // Today's date
                sqlCommand.Parameters.AddWithValue("@DateOfBirth", dateOfBirth.Value.Date); // Only date portion
                sqlCommand.Parameters.AddWithValue("@StudentId", id); // Replace with your student ID variable

                if (result == DialogResult.Yes)
                {
                    if (decimal.Parse(txtFee.Text) <= totalFee())
                    {
                        try
                        {
                            if (int.Parse(txtMobile.Text) >= 0 && txtMobile.Text.Length > 6)
                            {
                                int rowsAffected = sqlCommand.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    mc.query = $"DELETE FROM TblStudentDiscount WHERE StudentId = @StudentId;";
                                    mc.ProcessData2(mc.query, id);
                                    mc.query = $"DELETE FROM TblStudentSubject WHERE StudentId = @StudentId;";
                                    mc.ProcessData2(mc.query, id);
                                    SaveSelectedSubjects(int.Parse(id));
                                    discount(int.Parse(id));
                                    MessageBox.Show(mc.updateAlert);

                                }
                                else
                                {
                                    MessageBox.Show("No record effected");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Numberka iska sax fadlan!");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}");
                        }
                        // Call the method to delete the student

                        Reset();
                    }
                }
                else
                {
                    // Operation canceled, do nothing
                    Reset();
                    return;
                }
            }
            Reset();
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
                        checkedListBoxSubjects.Items.Clear(); // Clear existing items

                        while (reader.Read())
                        {
                            checkedListBoxSubjects.Items.Add(reader["SubjectName"].ToString());
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            mc.query = $"select * from StudentInfo where Address like '%{txtSearch.Text}%' or Gender like '%{txtSearch.Text}%' or StdName like '%{txtSearch.Text}%'";
            mc.Display(mc.query, dataGridView1);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Reset();
        }
    }
}
