using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Studiu_Individual_1
{
    public partial class InsertPlati : Form
    {
        public InsertPlati()
        {
            InitializeComponent();
            IncarcaComenziInComboBox(); 
            SetPlaceholder(); 
            textBoxDeleteIDPlata.TextChanged += textBoxDeleteIDPlata_TextChanged;
        }

        private void LoadPlati()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT ID_Plata, ID_Comanda, Metoda_Plata, Suma, Data_Plata, Status_Plata " +
                               "FROM Vanzari.Plati";

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
                        MessageBox.Show("Nu exista plati in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewPlati.AutoGenerateColumns = true;
                    dataGridViewPlati.DataSource = tabelDate;
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
            dataGridViewPlati.RowHeadersVisible = false;
            dataGridViewPlati.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewPlati.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewPlati.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewPlati.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewPlati.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewPlati.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void CurataCampurile()
        {
            numericUpDownID_Plata.Text = "";
            comboBoxIDComanda.SelectedIndex = -1;
            comboBoxMetodaPlata.SelectedIndex = -1;
            textBoxSuma.Text = "";
            comboBoxStatus.SelectedIndex = -1;
            textBoxDeleteIDPlata.Text = "";
        }

        private void IncarcaComenziInComboBox()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT ID_Comanda FROM Vanzari.Comenzi ORDER BY ID_Comanda";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);

                    comboBoxIDComanda.DataSource = tabelDate;
                    comboBoxIDComanda.DisplayMember = "ID_Comanda";
                    comboBoxIDComanda.ValueMember = "ID_Comanda";
                    comboBoxIDComanda.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea comenzilor: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertPlati_Load(object sender, EventArgs e)
        {
            LoadPlati();
        }

        private void SetPlaceholder()
        {
            labelIDPlata.Text = "Introduceti ID-ul";
            labelIDPlata.ForeColor = Color.Gray;
            labelIDPlata.Visible = string.IsNullOrEmpty(textBoxDeleteIDPlata.Text);
        }

        private void textBoxDeleteIDPlata_TextChanged(object sender, EventArgs e)
        {
            labelIDPlata.Visible = string.IsNullOrEmpty(textBoxDeleteIDPlata.Text);
        }

        private void buttonAdaugaPlata_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(numericUpDownID_Plata.Text))
            {
                MessageBox.Show("Introduceti ID-ul platii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idPlata;
            if (!int.TryParse(numericUpDownID_Plata.Text, out idPlata) || idPlata <= 0)
            {
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Plata FROM Vanzari.Plati WHERE ID_Plata = @ID_Plata";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Plata", idPlata);
                    object result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        MessageBox.Show("Plata cu ID-ul " + idPlata + " exista deja!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            int idComanda = (int)comboBoxIDComanda.SelectedValue;

            string metodaPlata = comboBoxMetodaPlata.Text.Trim();

            if (!decimal.TryParse(textBoxSuma.Text, out decimal suma) || suma <= 0)
            {
                MessageBox.Show("Suma trebuie sa fie un numar valid si pozitiv!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string statusPlata = comboBoxStatus.Text.Trim();

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand("INSERT INTO Vanzari.Plati (ID_Plata, ID_Comanda, Metoda_Plata, Suma, Status_Plata) VALUES (@ID_Plata, @ID_Comanda, @Metoda_Plata, @Suma, @Status_Plata)", conexiune);
                    comanda.Parameters.AddWithValue("@ID_Plata", idPlata);
                    comanda.Parameters.AddWithValue("@ID_Comanda", idComanda);
                    comanda.Parameters.AddWithValue("@Metoda_Plata", metodaPlata);
                    comanda.Parameters.AddWithValue("@Suma", suma);
                    comanda.Parameters.AddWithValue("@Status_Plata", statusPlata);

                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Plata adaugata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CurataCampurile();
                    LoadPlati();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdatePlata_Click_1(object sender, EventArgs e)
        {

            int idPlata;
            if (!int.TryParse(textBoxDeleteIDPlata.Text, out idPlata))
            {
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Plata FROM Vanzari.Plati WHERE ID_Plata = @ID_Plata";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Plata", idPlata);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Plata cu ID-ul " + idPlata + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            StringBuilder queryBuilder = new StringBuilder("UPDATE Vanzari.Plati SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();

            if (comboBoxIDComanda.SelectedIndex != -1)
            {
                int idComanda = (int)comboBoxIDComanda.SelectedValue;
                queryBuilder.Append("ID_Comanda = @ID_Comanda, ");
                parametri.Add(new SqlParameter("@ID_Comanda", idComanda));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(comboBoxMetodaPlata.Text))
            {
                string metodaPlata = comboBoxMetodaPlata.Text.Trim();
  
                queryBuilder.Append("Metoda_Plata = @Metoda_Plata, ");
                parametri.Add(new SqlParameter("@Metoda_Plata", metodaPlata));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(textBoxSuma.Text))
            {
                decimal suma;
                if (!decimal.TryParse(textBoxSuma.Text, out suma) || suma <= 0)
                {
                    return;
                }
                queryBuilder.Append("Suma = @Suma, ");
                parametri.Add(new SqlParameter("@Suma", suma));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(comboBoxStatus.Text))
            {
                string statusPlata = comboBoxStatus.Text.Trim();

                queryBuilder.Append("Status_Plata = @Status_Plata, ");
                parametri.Add(new SqlParameter("@Status_Plata", statusPlata));
                areCampuri = true;
            }

            queryBuilder.Length -= 2;
            queryBuilder.Append(" WHERE ID_Plata = @ID_Plata");
            parametri.Add(new SqlParameter("@ID_Plata", idPlata));

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(queryBuilder.ToString(), conexiune);
                    comanda.Parameters.AddRange(parametri.ToArray());
                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Plata actualizata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CurataCampurile();
                    textBoxDeleteIDPlata.Text = "";
                    LoadPlati();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeletePlata_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDPlata.Text))
            {
                MessageBox.Show("Introduceti ID-ul platii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idPlata;
            if (!int.TryParse(textBoxDeleteIDPlata.Text, out idPlata))
            {
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Plata FROM Vanzari.Plati WHERE ID_Plata = @ID_Plata";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Plata", idPlata);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Plata cu ID-ul " + idPlata + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("Sigur doriti sa stergeti plata cu ID-ul " + idPlata + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string query = "DELETE FROM Vanzari.Plati WHERE ID_Plata = @ID_Plata";
                        SqlCommand comanda = new SqlCommand(query, conexiune);
                        comanda.Parameters.AddWithValue("@ID_Plata", idPlata);
                        comanda.ExecuteNonQuery();

                        MessageBox.Show("Plata stearsa cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDPlata.Text = "";
                        LoadPlati();
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