using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Studiu_Individual_1
{
    public partial class InsertUsers : Form
    {
        public InsertUsers()
        {
            InitializeComponent();
            IncarcaRoluriInComboBox(); 
        }

        private void LoadUsers()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT Username, CONVERT(VARCHAR(50), DecryptByPassPhrase('paroladecriptare', Password)) AS Password, Role " +
                               "FROM Users";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);
                    if (tabelDate.Rows.Count == 0)
                    {
                        MessageBox.Show("Nu exista utilizatori in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    foreach (DataRow row in tabelDate.Rows)
                    {
                        if (row["Password"] != DBNull.Value)
                        {
                            string password = row["Password"].ToString();
                            row["Password"] = password.Replace("\0", "").Trim();
                        }
                    }
                    dataGridViewUsers.AutoGenerateColumns = true;
                    dataGridViewUsers.DataSource = tabelDate;
                    CustomizeDataGridView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CustomizeDataGridView()
        {
            dataGridViewUsers.RowHeadersVisible = false;
            dataGridViewUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewUsers.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewUsers.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewUsers.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void IncarcaRoluriInComboBox()
        {
            string[] roluri = { "db_owner", "ManagerVanzari", "ManagerStocuri", "Contabil", "UtilizatorVizualizare", "ManagerClienti" };
            comboBoxRol.DataSource = roluri;
            comboBoxRol.SelectedIndex = -1;
        }

        private bool VerifyPassword(string username, string inputPassword)
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "SELECT CONVERT(VARCHAR(50), DecryptByPassPhrase('paroladecriptare', Password)) AS Password " +
                          "FROM Users WHERE Username = @Username";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(query, conexiune);
                    comanda.Parameters.AddWithValue("@Username", username);
                    object result = comanda.ExecuteScalar();
                    if (result == null)
                    {
                        return false;
                    }
                    string storedPassword = result.ToString().Replace("\0", "").Trim();
                    return storedPassword == inputPassword;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private string TestDecryption(string username)
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "SELECT CONVERT(VARCHAR(50), DecryptByPassPhrase('paroladecriptare', Password)) AS Password " +
                          "FROM Users WHERE Username = @Username";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(query, conexiune);
                    comanda.Parameters.AddWithValue("@Username", username);
                    object result = comanda.ExecuteScalar();
                    if (result == null)
                    {
                        return "Utilizator inexistent";
                    }
                    string password = result.ToString().Replace("\0", "").Trim();
                    return $"Parola decriptata: [{password}]";
                }
                catch (Exception ex)
                {
                    return $"Eroare: {ex.Message}";
                }
            }
        }

        private void buttonAdaugaUtilizator_Click(object sender, EventArgs e)
        {
            string username = textBoxNume.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Introduceti numele utilizatorului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT Username FROM Users WHERE Username = @Username";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    object result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        MessageBox.Show("Utilizatorul cu numele " + username + " exista deja!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string password = textBoxParola.Text;
            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                MessageBox.Show("Parola trebuie sa aiba cel putin 6 caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxRol.SelectedIndex == -1)
            {
                MessageBox.Show("Selectati un rol!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string role = comboBoxRol.SelectedItem.ToString();

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    string query = "INSERT INTO Users (Username, Password, Role) " +
                                  "VALUES (@Username, EncryptByPassPhrase('paroladecriptare', @Password), @Role)";
                    SqlCommand comanda = new SqlCommand(query, conexiune);
                    comanda.Parameters.AddWithValue("@Username", username);
                    comanda.Parameters.AddWithValue("@Password", password);
                    comanda.Parameters.AddWithValue("@Role", role);

                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Utilizatorul a fost adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBoxNume.Clear();
                    textBoxParola.Clear();
                    comboBoxRol.SelectedIndex = -1;
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateUtilizator_Click(object sender, EventArgs e)
        {
            string username = textBoxDeleteIDUtilizator.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Introduceti Username-ul!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT Username FROM Users WHERE Username = @Username";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Utilizatorul cu numele " + username + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            bool areCampuri = false;
            StringBuilder queryBuilder = new StringBuilder("UPDATE Users SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(textBoxParola.Text))
            {
                string password = textBoxParola.Text;
                if (password.Length < 6)
                {
                    MessageBox.Show("Parola trebuie sa aiba cel putin 6 caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryBuilder.Append("Password = EncryptByPassPhrase('paroladecriptare', @Password), ");
                parametri.Add(new SqlParameter("@Password", password));
                areCampuri = true;
            }

            if (comboBoxRol.SelectedIndex != -1)
            {
                string role = comboBoxRol.SelectedItem.ToString();
                queryBuilder.Append("Role = @Role, ");
                parametri.Add(new SqlParameter("@Role", role));
                areCampuri = true;
            }

            if (!areCampuri)
            {
                MessageBox.Show("Nu au fost introduse date pentru actualizare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            queryBuilder.Length -= 2;
            queryBuilder.Append(" WHERE Username = @Username");
            parametri.Add(new SqlParameter("@Username", username));

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(queryBuilder.ToString(), conexiune);
                    comanda.Parameters.AddRange(parametri.ToArray());
                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Utilizatorul a fost actualizat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBoxNume.Clear();
                    textBoxParola.Clear();
                    comboBoxRol.SelectedIndex = -1;
                    textBoxDeleteIDUtilizator.Clear();
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteUtilizator_Click(object sender, EventArgs e)
        {
            string username = textBoxDeleteIDUtilizator.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Introduceti Username-ul!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT Username FROM Users WHERE Username = @Username";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Utilizatorul cu numele " + username + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("Sigur doriti sa stergeti utilizatorul cu numele " + username + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string query = "DELETE FROM Users WHERE Username = @Username";
                        SqlCommand comanda = new SqlCommand(query, conexiune);
                        comanda.Parameters.AddWithValue("@Username", username);
                        comanda.ExecuteNonQuery();

                        MessageBox.Show("Utilizatorul a fost sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDUtilizator.Clear();
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void InsertUsers_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }
    }
}