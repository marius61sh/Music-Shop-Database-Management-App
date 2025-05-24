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
    public partial class InsertDetalii_Comenzi : Form
    {
        public InsertDetalii_Comenzi()
        {
            InitializeComponent();
            IncarcaComenziInComboBox(); 
            IncarcaProduseInComboBox(); 
            SetPlaceholder(); 
            textBoxDeleteIDDetaliu.TextChanged += textBoxDeleteIDDetaliu_TextChanged; 
        }

        private void LoadDetaliiComenzi()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT dc.ID_Detaliu, dc.ID_Comanda, dc.ID_Produs, p.Nume_Produs, dc.Cantitate, dc.Pret_Total " +
                               "FROM Vanzari.Detalii_Comenzi dc " +
                               "INNER JOIN Produse.Produse p ON dc.ID_Produs = p.ID_Produs";

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
                        MessageBox.Show("Nu exista detalii de comenzi in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewDetaliiComenzi.AutoGenerateColumns = true;
                    dataGridViewDetaliiComenzi.DataSource = tabelDate;
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
            dataGridViewDetaliiComenzi.RowHeadersVisible = false;
            dataGridViewDetaliiComenzi.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewDetaliiComenzi.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewDetaliiComenzi.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewDetaliiComenzi.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewDetaliiComenzi.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewDetaliiComenzi.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void CurataCampurile()
        {
            numericUpDownIDDetaliu.Text = "";
            comboBoxIDComanda.SelectedIndex = -1;
            comboBoxIDProdus.SelectedIndex = -1;
            textBoxCantitate.Text = "";
            textBoxPret.Text = "";
            textBoxDeleteIDDetaliu.Text = "";
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

        private void IncarcaProduseInComboBox()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT ID_Produs, Nume_Produs FROM Produse.Produse ORDER BY Nume_Produs";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);

                    comboBoxIDProdus.DataSource = tabelDate;
                    comboBoxIDProdus.DisplayMember = "Nume_Produs";
                    comboBoxIDProdus.ValueMember = "ID_Produs";
                    comboBoxIDProdus.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea produselor: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertDetalii_Comenzi_Load(object sender, EventArgs e)
        {
            LoadDetaliiComenzi();
        }

        private void SetPlaceholder()
        {
            labelIDDetaliu.Text = "Introduceti ID-ul";
            labelIDDetaliu.ForeColor = Color.Gray;
            labelIDDetaliu.Visible = string.IsNullOrEmpty(textBoxDeleteIDDetaliu.Text);
        }

        private void textBoxDeleteIDDetaliu_TextChanged(object sender, EventArgs e)
        {
            labelIDDetaliu.Visible = string.IsNullOrEmpty(textBoxDeleteIDDetaliu.Text);
        }

        private void buttonAdaugaDetaliu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(numericUpDownIDDetaliu.Text))
            {
                MessageBox.Show("Introduceti ID-ul detaliului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idDetaliu;
            if (!int.TryParse(numericUpDownIDDetaliu.Text, out idDetaliu) || idDetaliu <= 0)
            {
                MessageBox.Show("ID-ul detaliului trebuie sa fie un numar valid si pozitiv!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Detaliu FROM Vanzari.Detalii_Comenzi WHERE ID_Detaliu = @ID_Detaliu";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Detaliu", idDetaliu);
                    object result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        MessageBox.Show("Detaliul cu ID-ul " + idDetaliu + " exista deja!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            int idProdus = (int)comboBoxIDProdus.SelectedValue;

            if (!int.TryParse(textBoxCantitate.Text, out int cantitate) || cantitate <= 0)
            {
                MessageBox.Show("Cantitatea trebuie sa fie un numar valid si pozitiv!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(textBoxPret.Text, out decimal pretTotal) || pretTotal <= 0)
            {
                MessageBox.Show("Pretul total trebuie sa fie un numar valid si pozitiv!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    string query = "INSERT INTO Vanzari.Detalii_Comenzi (ID_Detaliu, ID_Comanda, ID_Produs, Cantitate, Pret_Total) " +
                                  "VALUES (@ID_Detaliu, @ID_Comanda, @ID_Produs, @Cantitate, @Pret_Total)";
                    SqlCommand comanda = new SqlCommand(query, conexiune);
                    comanda.Parameters.AddWithValue("@ID_Detaliu", idDetaliu);
                    comanda.Parameters.AddWithValue("@ID_Comanda", idComanda);
                    comanda.Parameters.AddWithValue("@ID_Produs", idProdus);
                    comanda.Parameters.AddWithValue("@Cantitate", cantitate);
                    comanda.Parameters.AddWithValue("@Pret_Total", pretTotal);

                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Detaliu comanda adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CurataCampurile();
                    LoadDetaliiComenzi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateDetaliu_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDDetaliu.Text))
            {
                MessageBox.Show("Introduceti ID-ul detaliului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idDetaliu;
            if (!int.TryParse(textBoxDeleteIDDetaliu.Text, out idDetaliu))
            {
                MessageBox.Show("ID-ul detaliului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Detaliu FROM Vanzari.Detalii_Comenzi WHERE ID_Detaliu = @ID_Detaliu";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Detaliu", idDetaliu);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Detaliul cu ID-ul " + idDetaliu + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            StringBuilder queryBuilder = new StringBuilder("UPDATE Vanzari.Detalii_Comenzi SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();

            if (comboBoxIDComanda.SelectedIndex != -1)
            {
                int idComanda = (int)comboBoxIDComanda.SelectedValue;
                queryBuilder.Append("ID_Comanda = @ID_Comanda, ");
                parametri.Add(new SqlParameter("@ID_Comanda", idComanda));
                areCampuri = true;
            }

            if (comboBoxIDProdus.SelectedIndex != -1)
            {
                int idProdus = (int)comboBoxIDProdus.SelectedValue;
                queryBuilder.Append("ID_Produs = @ID_Produs, ");
                parametri.Add(new SqlParameter("@ID_Produs", idProdus));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(textBoxCantitate.Text))
            {
                int cantitate;
                if (!int.TryParse(textBoxCantitate.Text, out cantitate) || cantitate <= 0)
                {
                    return;
                }
                queryBuilder.Append("Cantitate = @Cantitate, ");
                parametri.Add(new SqlParameter("@Cantitate", cantitate));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(textBoxPret.Text))
            {
                decimal pretTotal;
                if (!decimal.TryParse(textBoxPret.Text, out pretTotal) || pretTotal <= 0)
                {
                    return;
                }
                queryBuilder.Append("Pret_Total = @Pret_Total, ");
                parametri.Add(new SqlParameter("@Pret_Total", pretTotal));
                areCampuri = true;
            }

            queryBuilder.Length -= 2;
            queryBuilder.Append(" WHERE ID_Detaliu = @ID_Detaliu");
            parametri.Add(new SqlParameter("@ID_Detaliu", idDetaliu));

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(queryBuilder.ToString(), conexiune);
                    comanda.Parameters.AddRange(parametri.ToArray());
                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Detaliu comanda actualizat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CurataCampurile();
                    textBoxDeleteIDDetaliu.Text = "";
                    LoadDetaliiComenzi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteDetaliu_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDDetaliu.Text))
            {
                MessageBox.Show("Introduceti ID-ul detaliului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idDetaliu;
            if (!int.TryParse(textBoxDeleteIDDetaliu.Text, out idDetaliu))
            {
                MessageBox.Show("ID-ul detaliului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Detaliu FROM Vanzari.Detalii_Comenzi WHERE ID_Detaliu = @ID_Detaliu";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Detaliu", idDetaliu);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Detaliul cu ID-ul " + idDetaliu + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("Sigur doriti sa stergeti detaliul cu ID-ul " + idDetaliu + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string query = "DELETE FROM Vanzari.Detalii_Comenzi WHERE ID_Detaliu = @ID_Detaliu";
                        SqlCommand comanda = new SqlCommand(query, conexiune);
                        comanda.Parameters.AddWithValue("@ID_Detaliu", idDetaliu);
                        comanda.ExecuteNonQuery();

                        MessageBox.Show("Detaliu comanda sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDDetaliu.Text = "";
                        LoadDetaliiComenzi();
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