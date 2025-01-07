using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CA2213_StudentRegistrationApp
{
    internal class MainClass
    {
        private static string conString = @"Data Source=DESKTOP-KCQ5JHF\SQLEXPRESS;Initial Catalog=studentFeeManagementDB;Integrated Security=SSPI";
        public SqlConnection con = new SqlConnection(conString);
        public SqlCommand cmd;
        public SqlDataAdapter da;
        public string query = "";

        public string insertAlert = "Data has been saved.";
        public string updateAlert = "Data has been updated.";
        public string deleteAlert = "Data has been deleted.";


        public static string username, usertype;

        //connect method
        public void Connect()
        {
            if(con.State!=ConnectionState.Open)
                con.Open();
        }
        //disconnect method
        public void Disconnect()
        {
            if (con.State != ConnectionState.Closed)
                con.Close();
        }
        //ProcessData method
        public void ProcessData(string _query, string _alert,string id)
        {
            try
            {
                using(cmd = new SqlCommand(_query,con))
                {
                   // Disconnect();
                    cmd.Parameters.AddWithValue("@StudentId", id);
                    Connect();//open connection
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        Disconnect();
                        MessageBox.Show(_alert, "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                    else
                    {
                        MessageBox.Show("Failed!", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Disconnect();//close connection
                    }
                        
                }
                Disconnect();
            }
            catch(Exception ex)
            {
                Disconnect();
                MessageBox.Show("inta "+ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ProcessData2(string _query, string id)
        {
            try
            {
                using (cmd = new SqlCommand(_query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", id);
                    Connect();//open connection
                    if (cmd.ExecuteNonQuery() > 0)
                        Disconnect();
                    // MessageBox.Show(_alert, "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    else
                       // MessageBox.Show("Failed!", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Disconnect();//close connection
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //display method
        public void Display(string _query, DataGridView dataGridView)
        {
            try
            {
                using(da=new SqlDataAdapter(_query, con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "tbl");
                    dataGridView.DataSource = ds.Tables["tbl"];
                }
            }
            catch { }
        }
        public void Display2(string query,DataGridView dataGridView)
        {
            // Clear existing columns if necessary
            dataGridView.Columns.Clear();

            // Add columns to DataGridView
            dataGridView.Columns.Add("StudentId", "StudentId");
            dataGridView.Columns.Add("StdName", "StdName");            
            dataGridView.Columns.Add("Subjects", "Subjects");
            dataGridView.Columns.Add("Classes", "Classes");
            dataGridView.Columns.Add("Amount", "Amount");

            // Add CheckBox column
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Paid",
                HeaderText = "Paid",
                TrueValue = true,
                FalseValue = false
            };
            dataGridView.Columns.Add(checkBoxColumn);

            // Execute the query and read data
            using (SqlConnection conn = new SqlConnection(conString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        // Create a new row
                        DataGridViewRow row = new DataGridViewRow();

                        // Add cells to the row
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = reader["StudentId"].ToString() });
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = reader["StdName"].ToString() });
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = reader["Subjects"].ToString() });
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = reader["Classes"].ToString() });
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = reader["Amount"].ToString() }); // Set amount

                        // Handle the Paid column
                        bool isPaid = false;

                        // Check the Paid column data type
                        if (reader["Paid"] != DBNull.Value)
                        {
                            if (reader["Paid"] is bool)
                            {
                                isPaid = (bool)reader["Paid"];
                            }
                            else if (reader["Paid"] is int)
                            {
                                isPaid = ((int)reader["Paid"]) == 1; // Assuming 1 is true
                            }
                            else if (reader["Paid"] is string)
                            {
                                isPaid = (string)reader["Paid"] == "true"; // Handle string "true"
                            }
                        }

                        // Add the checkbox cell with the correct value
                        row.Cells.Add(new DataGridViewCheckBoxCell { Value = isPaid });

                        // Add the row to the DataGridView
                        dataGridView.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"error..",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        /* public void Display2(string _query, DataGridView dataGridView)
         {
             try
             {
                 using(da=new SqlDataAdapter(_query, con))
                 {
                     DataSet ds = new DataSet();
                     da.Fill(ds, "tbl");
                     dataGridView.DataSource = ds.Tables["tbl"];

                     foreach (DataGridViewRow row in dataGridView.Rows)
                     {
                         if (row.Cells["Paid"].Value != null)
                         {
                             row.Cells["Paid"].Value = Convert.ToBoolean(row.Cells["Paid"].Value);
                         }
                     }
                 }

             }
             catch { }
         }*/
        //clear method
        public void Clear(params Control[] ctrl)
        {
            try
            {

                for(int i = 0; i < ctrl.Length;i++)
                {
                    ctrl[i].Text = string.Empty;
                }
                ctrl[0].Focus();                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //GetDataFromDGV method
        public void GetDataFromDGV(DataGridView dataGridView, DataGridViewCellEventArgs e, params Control[] ctrl)
        {
            try
            {

                int j = 1;
                for (int i = 0; i < ctrl.Length; i++)
                {
     
                    ctrl[i].Text = dataGridView.Rows[e.RowIndex].Cells[j].Value.ToString();
                    j++;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GetDataFromDGV2(DataGridView dataGridView, DataGridViewCellEventArgs e, params Control[] ctrl)
        {
            try
            {


                for (int i = 0; i < ctrl.Length; i++)
                {

                    ctrl[i].Text = dataGridView.Rows[e.RowIndex].Cells[i].Value.ToString();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
