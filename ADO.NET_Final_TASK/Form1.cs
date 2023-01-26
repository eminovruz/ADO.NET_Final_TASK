using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ADO.NET_Final_TASK
{
    public partial class Form1 : Form
    {
        SqlConnection? conn = null;
        SqlDataAdapter? adapter = null;
        DataSet? dataSet = null;
        IConfigurationRoot? root = null;
        bool flag = true;

        public Form1()
        {
            InitializeComponent();
            root = new ConfigurationBuilder()
.AddJsonFile("appsettings.json")
.Build();
            string? conStr = root.GetConnectionString("DataBaseTask");
            conn = new(conStr);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            LoadApp();
        }

        private void LoadApp()
        {
            listView1.Items.Clear();

            try
            {
                conn?.Open();

                using SqlCommand command = new("SELECT * FROM DataBaseTask", conn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    listView1.Items.Add(reader[1]);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally { conn?.Close(); }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string? commandText = @"SELECT * FROM DataBaseTask
                WHERE DataBaseTask.[ProductName] = @p1";

                adapter = new SqlDataAdapter(commandText, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@p1", textBox1.Text);
                dataSet = new DataSet();
                adapter.Fill(dataSet, "SearchedBooks");

                if (flag == true)
                {
                    listView1.Items.Clear();
                    flag = false;
                }

                foreach (DataRow item in dataSet.Tables["SearchedBooks"].Rows)
                {
                    listView1.Items.Add(item[0] + " " + item[1] + " " + item[2]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
            => LoadApp();

        private void button3_Click(object sender, EventArgs e) // Delete
        {
            try
            {
                string commandText = "DELETE FROM DataBaseTask WHERE ProductName = @p1 ";
                conn?.Open();
                SqlCommand command = new(commandText, conn);
                command.Parameters.AddWithValue("@p1", listView1.SelectedItem.ToString());
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddView av = new();
            av.Show();
        }


        private void FillComboBox()
        {
            string commandText = "";

            adapter = new SqlDataAdapter(commandText, conn);

            adapter.DeleteCommand.Parameters.AddWithValue("@p1", listView1.SelectedItems);
        }
    }
}