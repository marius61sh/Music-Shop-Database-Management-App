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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Studiu_Individual_1
{
    public partial class InsertComenzi : Form
    {
        public InsertComenzi()
        {
            InitializeComponent();
            IncarcaClientiInComboBox();
            IncarcaAngajatiInComboBox(); 
            SetPlaceholder(); 
            textBoxDeleteIDComanda.TextChanged += textBoxDeleteIDComanda_TextChanged; 
        }

        private void LoadComenzi()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT c.ID_Comanda, c.ID_Client, cl.Nume AS Nume_Client, c.Data_Comanda, c.Suma_Totala, c.Status, c.ID_Angajat, a.Nume AS Nume_Angajat " +
                               "FROM Vanzari.Comenzi c " +
                               "INNER JOIN Vanzari.Clienti cl ON c.ID_Client = cl.ID_Client " +
                               "INNER JOIN Administrare.Angajati a ON c.ID_Angajat = a.ID_Angajat";

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
                        MessageBox.Show("Nu exista comenzi in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewComenzi.AutoGenerateColumns = true;
                    dataGridViewComenzi.DataSource = tabelDate;
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
            dataGridViewComenzi.RowHeadersVisible = false;
            dataGridViewComenzi.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewComenzi.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewComenzi.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewComenzi.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewComenzi.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewComenzi.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void CurataCampurile()
        {
            numericUpDownID_Comanda.Text = "";
            comboBoxClienti.SelectedIndex = -1;
            comboBoxAngajati.SelectedIndex = -1;
            textBoxSumaTotala.Text = "";
            comboBoxStatus.SelectedIndex = -1;
            textBoxDeleteIDComanda.Text = "";
        }

        private void IncarcaClientiInComboBox()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT ID_Client, Nume FROM Vanzari.Clienti ORDER BY Nume";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);

                    comboBoxClienti.DisplayMember = "Nume";
                    comboBoxClienti.ValueMember = "ID_Client";
                    comboBoxClienti.DataSource = tabelDate;
                    comboBoxClienti.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea clientilor: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void IncarcaAngajatiInComboBox()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT ID_Angajat, Nume FROM Administrare.Angajati ORDER BY Nume";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);

                    comboBoxAngajati.DisplayMember = "Nume";
                    comboBoxAngajati.ValueMember = "ID_Angajat";
                    comboBoxAngajati.DataSource = tabelDate;
                    comboBoxAngajati.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea angajatilor: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertComenzi_Load(object sender, EventArgs e)
        {
            LoadComenzi();
        }

        private void SetPlaceholder()
        {
            labelIDComanda.Text = "Introduceti ID-ul";
            labelIDComanda.ForeColor = Color.Gray;
            labelIDComanda.Visible = string.IsNullOrEmpty(textBoxDeleteIDComanda.Text);
        }

        private void textBoxDeleteIDComanda_TextChanged(object sender, EventArgs e)
        {
            labelIDComanda.Visible = string.IsNullOrEmpty(textBoxDeleteIDComanda.Text);
        }

        private void buttonAdaugaComanda_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(numericUpDownID_Comanda.Text))
            {
                MessageBox.Show("Introduceti ID-ul comenzii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idComanda;
            if (!int.TryParse(numericUpDownID_Comanda.Text, out idComanda))
            {
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Comanda FROM Vanzari.Comenzi WHERE ID_Comanda = @ID_Comanda";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Comanda", idComanda);
                    object result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        MessageBox.Show("Comanda cu ID-ul " + idComanda + " exista deja!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (comboBoxClienti.SelectedIndex == -1)
            {
                return;
            }
            int idClient = (int)comboBoxClienti.SelectedValue;

            if (comboBoxAngajati.SelectedIndex == -1)
            {
                return;
            }
            int idAngajat = (int)comboBoxAngajati.SelectedValue;

            if (!decimal.TryParse(textBoxSumaTotala.Text, out decimal sumaTotala) || sumaTotala <= 0)
            {
                return;
            }

            string status = comboBoxStatus.Text.Trim();
            if (string.IsNullOrEmpty(status))
            {
                return;
            }


            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand("INSERT INTO Vanzari.Comenzi (ID_Comanda, ID_Client, Suma_Totala, Status, ID_Angajat) VALUES (@ID_Comanda, @ID_Client, @Suma_Totala, @Status, @ID_Angajat)", conexiune);
                    comanda.Parameters.AddWithValue("@ID_Comanda", idComanda);
                    comanda.Parameters.AddWithValue("@ID_Client", idClient);
                    comanda.Parameters.AddWithValue("@Suma_Totala", sumaTotala);
                    comanda.Parameters.AddWithValue("@Status", status);
                    comanda.Parameters.AddWithValue("@ID_Angajat", idAngajat);

                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Comanda adaugata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CurataCampurile();
                    LoadComenzi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateComanda_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDComanda.Text))
            {
                MessageBox.Show("Introduceti ID-ul comenzii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idComanda;
            if (!int.TryParse(textBoxDeleteIDComanda.Text, out idComanda))
            {
                MessageBox.Show("ID-ul comenzii trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Comanda FROM Vanzari.Comenzi WHERE ID_Comanda = @ID_Comanda";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Comanda", idComanda);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Comanda cu ID-ul " + idComanda + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            StringBuilder queryBuilder = new StringBuilder("UPDATE Vanzari.Comenzi SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();

            if (comboBoxClienti.SelectedIndex != -1)
            {
                int idClient = (int)comboBoxClienti.SelectedValue;
                queryBuilder.Append("ID_Client = @ID_Client, ");
                parametri.Add(new SqlParameter("@ID_Client", idClient));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(textBoxSumaTotala.Text))
            {
                decimal sumaTotala;
                if (!decimal.TryParse(textBoxSumaTotala.Text, out sumaTotala) || sumaTotala <= 0)
                {
                    return;
                }
                queryBuilder.Append("Suma_Totala = @Suma_Totala, ");
                parametri.Add(new SqlParameter("@Suma_Totala", sumaTotala));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(comboBoxStatus.Text))
            {
                string status = comboBoxStatus.Text.Trim();
                queryBuilder.Append("Status = @Status, ");
                parametri.Add(new SqlParameter("@Status", status));
                areCampuri = true;
            }

            if (comboBoxAngajati.SelectedIndex != -1)
            {
                int idAngajat = (int)comboBoxAngajati.SelectedValue;
                queryBuilder.Append("ID_Angajat = @ID_Angajat, ");
                parametri.Add(new SqlParameter("@ID_Angajat", idAngajat));
                areCampuri = true;
            }
            queryBuilder.Length -= 2;
            queryBuilder.Append(" WHERE ID_Comanda = @ID_Comanda");
            parametri.Add(new SqlParameter("@ID_Comanda", idComanda));

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(queryBuilder.ToString(), conexiune);
                    comanda.Parameters.AddRange(parametri.ToArray());
                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Comanda actualizata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CurataCampurile();
                    textBoxDeleteIDComanda.Text = "";
                    LoadComenzi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteComanda_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDComanda.Text))
            {
                MessageBox.Show("Introduceti ID-ul comenzii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idComanda;
            if (!int.TryParse(textBoxDeleteIDComanda.Text, out idComanda))
            {
                MessageBox.Show("ID-ul comenzii trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Comanda FROM Vanzari.Comenzi WHERE ID_Comanda = @ID_Comanda";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Comanda", idComanda);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Comanda cu ID-ul " + idComanda + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("Sigur doriti sa stergeti comanda cu ID-ul " + idComanda + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string query = "DELETE FROM Vanzari.Comenzi WHERE ID_Comanda = @ID_Comanda";
                        SqlCommand comanda = new SqlCommand(query, conexiune);
                        comanda.Parameters.AddWithValue("@ID_Comanda", idComanda);
                        comanda.ExecuteNonQuery();

                        MessageBox.Show("Comanda stearsa cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDComanda.Text = "";
                        LoadComenzi();
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