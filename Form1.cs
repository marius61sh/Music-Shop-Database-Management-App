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
using System.Security.Cryptography;

namespace Studiu_Individual_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Username))
            {
                textBox1.Text = Properties.Settings.Default.Username;
            }

            SetPlaceholder();
        }

        private void SetPlaceholder()
        {
            label6.Text = "Introduceti login-ul...";
            label6.ForeColor = Color.Gray;
            label6.Visible = string.IsNullOrEmpty(textBox1.Text);

            label7.Text = "Introduceti parola...";
            label7.ForeColor = Color.Gray;
            label7.Visible = string.IsNullOrEmpty(textBox2.Text);
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            SetPlaceholder();
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            SetPlaceholder();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label6.Visible = string.IsNullOrEmpty(textBox1.Text);
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            label7.Visible = string.IsNullOrEmpty(textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.UseSystemPasswordChar)
            {
                textBox2.UseSystemPasswordChar = false;
                button2.BackgroundImage = Properties.Resources.ShowPass;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
                button2.BackgroundImage = Properties.Resources.HidePass;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (checkBox1.Checked)
            {
                Properties.Settings.Default.Username = textBox1.Text.Trim();
            }
            else
            {
                Properties.Settings.Default.Username = "";
            }

            Properties.Settings.Default.Save();

            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                label5.Text = "Introduceti login-ul si parola!";
                label5.ForeColor = Color.Red;
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                label5.Text = "Introduceti numele de utilizator!";
                label5.ForeColor = Color.Red;
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                label5.Text = "Introduceti parola!";
                label5.ForeColor = Color.Red;
                return;
            }

            if (password.Length < 6)
            {
                label5.Text = "Parola trebuie sa aiba cel putin 6 caractere!";
                label5.ForeColor = Color.Red;
                return;
            }

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT CONVERT(VARCHAR, DecryptByPassPhrase('paroladecriptare', password)) AS DecryptedPassword, Role FROM Users WHERE Username = @Username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string decryptedPassword = reader["DecryptedPassword"].ToString();
                        string storedRole = reader["Role"].ToString();

                        if (password == decryptedPassword)
                        {
                            label5.Text = $"Autentificare reusita! Bine ai venit, {username}!";
                            label5.ForeColor = Color.Green;
                            MessageBox.Show($"Autentificare reusita! Bun venit, {username}!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Properties.Settings.Default.UserRole = storedRole;
                            Properties.Settings.Default.Save();

                            this.Hide();
                            DatabaseForm dbForm = new DatabaseForm();
                            dbForm.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            label5.Text = "Nume de utilizator sau parola incorecte!";
                            label5.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        label5.Text = "Nume de utilizator incorect!";
                        label5.ForeColor = Color.Red;
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    label5.Text = "Eroare la conectarea la baza de date!";
                    label5.ForeColor = Color.Red;
                    MessageBox.Show("Eroare la conectarea la baza de date: " + ex.Message);
                }
            }
        }

        public string GetSHA256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}