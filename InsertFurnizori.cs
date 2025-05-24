using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Studiu_Individual_1
{
    public partial class InsertFurnizori : Form
    {
        public InsertFurnizori()
        {
            InitializeComponent();
            textBoxDeleteIDFurnizor.TextChanged += textBoxDeleteIDFurnizor_TextChanged;
            SetPlaceholder();
            LoadFurnizori();
        }

        private void LoadFurnizori()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT * FROM Produse.Furnizori";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);
                    if (tabelDate.Rows.Count == 0)
                    {
                        MessageBox.Show("Nu exista furnizori in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewFurnizori.AutoGenerateColumns = true;
                    dataGridViewFurnizori.DataSource = tabelDate;
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
            dataGridViewFurnizori.RowHeadersVisible = false;
            dataGridViewFurnizori.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewFurnizori.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewFurnizori.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewFurnizori.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewFurnizori.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewFurnizori.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void ClearFormFields()
        {
            textBoxNumeFurnizor.Text = "";
            textBoxContact.Text = "";
            textBoxTelefon.Text = "";
            textBoxEmail.Text = "";
            textBoxAdresa.Text = "";
            textBoxOras.Text = "";
            textBoxTara.Text = "";
        }

        private void InsertFurnizori_Load(object sender, EventArgs e)
        {

        }
        private void SetPlaceholder()
        {
            labelIDFurnizor.Text = "Introduceti ID-ul";
            labelIDFurnizor.ForeColor = Color.Gray;
            labelIDFurnizor.Visible = string.IsNullOrEmpty(textBoxDeleteIDFurnizor.Text);
        }

        private void textBoxDeleteIDFurnizor_TextChanged(object sender, EventArgs e)
        {
            labelIDFurnizor.Visible = string.IsNullOrEmpty(textBoxDeleteIDFurnizor.Text);
        }

        private void buttonAdaugaFurnizor_Click_1(object sender, EventArgs e)
        {
            string idFurnizor = numericUpDownIDFurnizor.Value.ToString();
            string numeFurnizor = textBoxNumeFurnizor.Text.Trim();
            string contact = textBoxContact.Text.Trim();
            string telefon = textBoxTelefon.Text.Trim();
            string email = textBoxEmail.Text.Trim();
            string adresa = textBoxAdresa.Text.Trim();
            string oras = textBoxOras.Text.Trim();
            string tara = textBoxTara.Text.Trim();

            if (string.IsNullOrEmpty(numeFurnizor))
            {
                MessageBox.Show("Completati Nume Furnizor!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "INSERT INTO Produse.Furnizori (ID_Furnizor, Nume_Furnizor, Contact, Telefon, Email, Adresa, Oras, Tara) " +
                               "VALUES (@ID_Furnizor, @Nume_Furnizor, @Contact, @Telefon, @Email, @Adresa, @Oras, @Tara)";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(interogare, conexiune);
                    comanda.Parameters.AddWithValue("@ID_Furnizor", idFurnizor);
                    comanda.Parameters.AddWithValue("@Nume_Furnizor", numeFurnizor);
                    comanda.Parameters.AddWithValue("@Contact", contact);
                    comanda.Parameters.AddWithValue("@Telefon", telefon);
                    comanda.Parameters.AddWithValue("@Email", email);
                    comanda.Parameters.AddWithValue("@Adresa", adresa);
                    comanda.Parameters.AddWithValue("@Oras", oras);
                    comanda.Parameters.AddWithValue("@Tara", tara);

                    comanda.ExecuteNonQuery();
                    MessageBox.Show("Furnizor adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    LoadFurnizori();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateFurnizor_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDFurnizor.Text))
            {
                MessageBox.Show("Introduceti ID-ul furnizorului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idFurnizor;
            if (!int.TryParse(textBoxDeleteIDFurnizor.Text, out idFurnizor))
            {
                MessageBox.Show("ID-ul furnizorului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Furnizor FROM Produse.Furnizori WHERE ID_Furnizor = @ID_Furnizor";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Furnizor", idFurnizor);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Furnizorul cu ID-ul " + idFurnizor + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string numeFurnizor = textBoxNumeFurnizor.Text.Trim();
            string contact = textBoxContact.Text.Trim();
            string telefon = textBoxTelefon.Text.Trim();
            string email = textBoxEmail.Text.Trim();
            string adresa = textBoxAdresa.Text.Trim();
            string oras = textBoxOras.Text.Trim();
            string tara = textBoxTara.Text.Trim();

            StringBuilder queryBuilder = new StringBuilder("UPDATE Produse.Furnizori SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();
            bool areCampuri = false;

            if (!string.IsNullOrEmpty(numeFurnizor))
            {
                queryBuilder.Append("Nume_Furnizor = @Nume_Furnizor, ");
                parametri.Add(new SqlParameter("@Nume_Furnizor", numeFurnizor));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(contact))
            {
                queryBuilder.Append("Contact = @Contact, ");
                parametri.Add(new SqlParameter("@Contact", contact));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(telefon))
            {
                queryBuilder.Append("Telefon = @Telefon, ");
                parametri.Add(new SqlParameter("@Telefon", telefon));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(email))
            {
                queryBuilder.Append("Email = @Email, ");
                parametri.Add(new SqlParameter("@Email", email));
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

            if (!areCampuri)
            {
                MessageBox.Show("Nu au fost introduse date pentru actualizare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            queryBuilder.Length -= 2;
            queryBuilder.Append(" WHERE ID_Furnizor = @ID_Furnizor");
            parametri.Add(new SqlParameter("@ID_Furnizor", idFurnizor));

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(queryBuilder.ToString(), conexiune);
                    comanda.Parameters.AddRange(parametri.ToArray());
                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Furnizor actualizat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    textBoxDeleteIDFurnizor.Text = "";
                    LoadFurnizori();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteFurnizor_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDFurnizor.Text))
            {
                MessageBox.Show("Introduceti ID-ul furnizorului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idFurnizor;
            if (!int.TryParse(textBoxDeleteIDFurnizor.Text, out idFurnizor))
            {
                MessageBox.Show("ID-ul furnizorului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Furnizor FROM Produse.Furnizori WHERE ID_Furnizor = @ID_Furnizor";
            string checkReferencesQuery = "SELECT COUNT(*) FROM Produse.Produse WHERE ID_Furnizor = @ID_Furnizor";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Furnizor", idFurnizor);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Furnizorul cu ID-ul " + idFurnizor + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    SqlCommand checkReferencesCmd = new SqlCommand(checkReferencesQuery, checkConn);
                    checkReferencesCmd.Parameters.AddWithValue("@ID_Furnizor", idFurnizor);
                    int referenceCount = (int)checkReferencesCmd.ExecuteScalar();
                    if (referenceCount > 0)
                    {
                        MessageBox.Show("Furnizorul nu poate fi sters deoarece este asociat cu produse in tabela Produse!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("Sigur doriti sa stergeti furnizorul cu ID-ul " + idFurnizor + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string query = "DELETE FROM Produse.Furnizori WHERE ID_Furnizor = @ID_Furnizor";
                        SqlCommand comanda = new SqlCommand(query, conexiune);
                        comanda.Parameters.AddWithValue("@ID_Furnizor", idFurnizor);
                        comanda.ExecuteNonQuery();

                        MessageBox.Show("Furnizor sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDFurnizor.Text = "";
                        LoadFurnizori();
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

