using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studiu_Individual_1
{
    public partial class InsertClienti : Form
    {
        public InsertClienti()
        {
            InitializeComponent();
            SetPlaceholder();
            textBoxDeleteIDClient.TextChanged += textBoxDeleteIDClient_TextChanged;
        }

        private void LoadClienti()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT ID_Client, Prenume, Nume, Email, Telefon, Adresa, Oras, Tara, Data_Inregistrare " +
                               "FROM Vanzari.Clienti";

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
                        MessageBox.Show("Nu exista clienti in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewClienti.AutoGenerateColumns = true;
                    dataGridViewClienti.DataSource = tabelDate;
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
            dataGridViewClienti.RowHeadersVisible = false;
            dataGridViewClienti.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewClienti.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewClienti.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewClienti.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewClienti.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewClienti.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void ClearFormFields()
        {
            textBoxPrenume.Text = "";
            textBoxNume.Text = "";
            textBoxEmail.Text = "";
            textBoxTelefon.Text = "";
            textBoxAdresa.Text = "";
            textBoxOras.Text = "";
            textBoxTara.Text = "";
        }

        private void InsertClienti_Load(object sender, EventArgs e)
        {
            LoadClienti();
        }

        private void SetPlaceholder()
        {
            labelIDClient.Text = "Introduceti ID-ul";
            labelIDClient.ForeColor = Color.Gray;
            labelIDClient.Visible = string.IsNullOrEmpty(textBoxDeleteIDClient.Text);
        }

        private void textBoxDeleteIDClient_TextChanged(object sender, EventArgs e)
        {
            labelIDClient.Visible = string.IsNullOrEmpty(textBoxDeleteIDClient.Text);
        }

        private void buttonAdaugaClient_Click(object sender, EventArgs e)
        {
            string prenume = textBoxPrenume.Text.Trim();
            string nume = textBoxNume.Text.Trim();
            string email = textBoxEmail.Text.Trim();
            string telefon = textBoxTelefon.Text.Trim();
            string adresa = textBoxAdresa.Text.Trim();
            string oras = textBoxOras.Text.Trim();
            string tara = textBoxTara.Text.Trim();

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand("AdaugaClient", conexiune);
                    comanda.CommandType = CommandType.StoredProcedure;

                    comanda.Parameters.AddWithValue("@Prenume", prenume);
                    comanda.Parameters.AddWithValue("@Nume", nume);
                    comanda.Parameters.AddWithValue("@Email", email);
                    comanda.Parameters.AddWithValue("@Telefon", telefon);
                    comanda.Parameters.AddWithValue("@Adresa",adresa);
                    comanda.Parameters.AddWithValue("@Oras", oras);
                    comanda.Parameters.AddWithValue("@Tara",tara);

                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Client adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    LoadClienti();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateClient_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDClient.Text))
            {
                MessageBox.Show("Introduceti ID-ul clientului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idClient;
            if (!int.TryParse(textBoxDeleteIDClient.Text, out idClient))
            {
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Client FROM Vanzari.Clienti WHERE ID_Client = @ID_Client";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Client", idClient);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Clientul cu ID-ul " + idClient + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string prenume = textBoxPrenume.Text.Trim();
            string nume = textBoxNume.Text.Trim();
            string email = textBoxEmail.Text.Trim();
            string telefon = textBoxTelefon.Text.Trim();
            string adresa = textBoxAdresa.Text.Trim();
            string oras = textBoxOras.Text.Trim();
            string tara = textBoxTara.Text.Trim();

            bool areCampuri = false;
            StringBuilder queryBuilder = new StringBuilder("UPDATE Vanzari.Clienti SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(prenume))
            {
                queryBuilder.Append("Prenume = @Prenume, ");
                parametri.Add(new SqlParameter("@Prenume", prenume));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(nume))
            {
 
                queryBuilder.Append("Nume = @Nume, ");
                parametri.Add(new SqlParameter("@Nume", nume));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(email))
            {
                queryBuilder.Append("Email = @Email, ");
                parametri.Add(new SqlParameter("@Email", email));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(telefon))
            {
                queryBuilder.Append("Telefon = @Telefon, ");
                parametri.Add(new SqlParameter("@Telefon", telefon));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(adresa))
            {
                queryBuilder.Append("Adresa = @Adresa, ");
                parametri.Add(new SqlParameter("@Adresa", adresa));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(oras))
            {
                queryBuilder.Append("Oras = @Oras, ");
                parametri.Add(new SqlParameter("@Oras", oras));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(tara))
            {
                queryBuilder.Append("Tara = @Tara, ");
                parametri.Add(new SqlParameter("@Tara", tara));
                areCampuri = true;
            }


            queryBuilder.Length -= 2;
            queryBuilder.Append(" WHERE ID_Client = @ID_Client");
            parametri.Add(new SqlParameter("@ID_Client", idClient));

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(queryBuilder.ToString(), conexiune);
                    comanda.Parameters.AddRange(parametri.ToArray());
                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Client actualizat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    textBoxDeleteIDClient.Text = "";
                    LoadClienti();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteClient_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDClient.Text))
            {
                MessageBox.Show("Introduceti ID-ul clientului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idClient;
            if (!int.TryParse(textBoxDeleteIDClient.Text, out idClient))
            {
                MessageBox.Show("ID-ul clientului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Client FROM Vanzari.Clienti WHERE ID_Client = @ID_Client";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Client", idClient);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Clientul cu ID-ul " + idClient + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("Sigur doriti sa stergeti clientul cu ID-ul " + idClient + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string query = "DELETE FROM Vanzari.Clienti WHERE ID_Client = @ID_Client";
                        SqlCommand comanda = new SqlCommand(query, conexiune);
                        comanda.Parameters.AddWithValue("@ID_Client", idClient);
                        comanda.ExecuteNonQuery();

                        MessageBox.Show("Client sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDClient.Text = "";
                        LoadClienti();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}