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

namespace Studiu_Individual_1
{
    public partial class InsertProduse : Form
    {
        public InsertProduse()
        {
            InitializeComponent();
            IncarcaFurnizoriInComboBox();
            SetPlaceholder();
            textBoxDeleteIDProdus.TextChanged += textBoxDeleteIDProdus_TextChanged; 

        }

        private void LoadProduse()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT p.ID_Produs, p.Nume_Produs, p.Tip_Produs, p.ID_Furnizor, f.Nume_Furnizor, p.Pret, p.Stoc " +
                               "FROM Produse.Produse p " +
                               "INNER JOIN Produse.Furnizori f ON p.ID_Furnizor = f.ID_Furnizor";

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
                        MessageBox.Show("Nu exista produse in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewProduse.AutoGenerateColumns = true;
                    dataGridViewProduse.DataSource = tabelDate;
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
            dataGridViewProduse.RowHeadersVisible = false;
            dataGridViewProduse.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewProduse.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewProduse.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewProduse.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewProduse.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewProduse.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void ClearFormFields()
        {
            textBoxNumeProdus.Text = "";
            comboBoxTipProdus.SelectedIndex = -1;
            comboBoxFurnizor.SelectedIndex = -1;
            textBoxPret.Text = "";
            textBoxStoc.Text = "";
        }

        private void IncarcaFurnizoriInComboBox()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT ID_Furnizor, Nume_Furnizor FROM Produse.Furnizori ORDER BY Nume_Furnizor";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);

                    comboBoxFurnizor.DataSource = tabelDate;
                    comboBoxFurnizor.DisplayMember = "Nume_Furnizor";
                    comboBoxFurnizor.ValueMember = "ID_Furnizor";
                    comboBoxFurnizor.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea furnizorilor: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertProduse_Load(object sender, EventArgs e)
        {
            LoadProduse();
        }

        private void SetPlaceholder()
        {
            labelIDProdus.Text = "Introduceti ID-ul";
            labelIDProdus.ForeColor = Color.Gray;
            labelIDProdus.Visible = string.IsNullOrEmpty(textBoxDeleteIDProdus.Text);
        }

        private void textBoxDeleteIDProdus_TextChanged(object sender, EventArgs e)
        {
            labelIDProdus.Visible = string.IsNullOrEmpty(textBoxDeleteIDProdus.Text);
        }

        private void buttonUpdateProdus_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDProdus.Text))
            {
                MessageBox.Show("Introduceti ID-ul produsului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idProdus;
            if (!int.TryParse(textBoxDeleteIDProdus.Text, out idProdus))
            {
                MessageBox.Show("ID-ul produsului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Produse WHERE ID_Produs = @ID_Produs";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Produs", idProdus);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Produsul cu ID-ul " + idProdus + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string numeProdus = textBoxNumeProdus.Text.Trim();
            string tipProdus = "";
            int idFurnizor = 0;
            decimal pret = 0;
            int stoc = 0;

            bool areCampuri = false;
            StringBuilder queryBuilder = new StringBuilder("UPDATE Produse.Produse SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(numeProdus))
            {
                queryBuilder.Append("Nume_Produs = @Nume_Produs, ");
                parametri.Add(new SqlParameter("@Nume_Produs", numeProdus));
                areCampuri = true;
            }

            if (comboBoxTipProdus.SelectedIndex != -1)
            {
                tipProdus = comboBoxTipProdus.SelectedItem.ToString();
                queryBuilder.Append("Tip_Produs = @Tip_Produs, ");
                parametri.Add(new SqlParameter("@Tip_Produs", tipProdus));
                areCampuri = true;
            }

            if (comboBoxFurnizor.SelectedIndex != -1)
            {
                idFurnizor = (int)comboBoxFurnizor.SelectedValue;
                queryBuilder.Append("ID_Furnizor = @ID_Furnizor, ");
                parametri.Add(new SqlParameter("@ID_Furnizor", idFurnizor));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(textBoxPret.Text))
            {
                decimal pretValue;
                if (!decimal.TryParse(textBoxPret.Text, out pretValue) || pretValue <= 0)
                {
                    return;
                }
                pret = pretValue;
                queryBuilder.Append("Pret = @Pret, ");
                parametri.Add(new SqlParameter("@Pret", pret));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(textBoxStoc.Text))
            {
                int stocValue;
                if (!int.TryParse(textBoxStoc.Text, out stocValue) || stocValue < 0)
                {
                    return;
                }
                stoc = stocValue;
                queryBuilder.Append("Stoc = @Stoc, ");
                parametri.Add(new SqlParameter("@Stoc", stoc));
                areCampuri = true;
            }

            if (!areCampuri)
            {
                MessageBox.Show("Nu au fost introduse date pentru actualizare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            queryBuilder.Length -= 2;
            queryBuilder.Append(" WHERE ID_Produs = @ID_Produs");
            parametri.Add(new SqlParameter("@ID_Produs", idProdus));

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(queryBuilder.ToString(), conexiune);
                    comanda.Parameters.AddRange(parametri.ToArray());
                    comanda.ExecuteNonQuery();

                    MessageBox.Show("Produs actualizat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    textBoxDeleteIDProdus.Text = "";
                    LoadProduse();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteProdus_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDProdus.Text))
            {
                MessageBox.Show("Introduceti ID-ul produsului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idProdus;
            if (!int.TryParse(textBoxDeleteIDProdus.Text, out idProdus))
            {
                MessageBox.Show("ID-ul produsului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Produse WHERE ID_Produs = @ID_Produs";

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Produs", idProdus);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Produsul cu ID-ul " + idProdus + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("Sigur doriti sa stergeti produsul cu ID-ul " + idProdus + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string query = "DELETE FROM Produse.Produse WHERE ID_Produs = @ID_Produs";
                        SqlCommand comanda = new SqlCommand(query, conexiune);
                        comanda.Parameters.AddWithValue("@ID_Produs", idProdus);
                        comanda.ExecuteNonQuery();

                        MessageBox.Show("Produs sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDProdus.Text = "";
                        LoadProduse();
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