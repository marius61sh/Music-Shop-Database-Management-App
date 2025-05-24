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
    public partial class InsertInstrumenteSuflat : Form
    {
        private bool comboBoxInitializat = false;

        public InsertInstrumenteSuflat()
        {
            InitializeComponent();
            IncarcaFurnizoriInComboBox();
            comboBoxInitializat = true;
            SetPlaceholder();
            textBoxDeleteIDInstrument.TextChanged += textBoxDeleteIDInstrument_TextChanged;
            dataGridViewInstrumenteSuflat.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewInstrumenteSuflat.MultiSelect = false;
            dataGridViewInstrumenteSuflat.Enabled = true;
            LoadInstrumente();
            dataGridViewInstrumenteSuflat.Focus();
        }

        private void LoadInstrumente()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT i.ID_Produs, i.ID_Instrument, i.Nume_Instrument, i.Pret, i.Stoc, i.Tip_Instrument, i.Material, i.Dimensiuni, p.ID_Furnizor " +
                               "FROM Produse.InstrumenteSuflat i LEFT JOIN Produse.Produse p ON i.ID_Produs = p.ID_Produs";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);
                    if (tabelDate.Rows.Count == 0)
                    {
                        MessageBox.Show("Nu exista instrumente de suflat in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewInstrumenteSuflat.AutoGenerateColumns = true;
                    dataGridViewInstrumenteSuflat.DataSource = tabelDate;
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
            dataGridViewInstrumenteSuflat.RowHeadersVisible = false;
            dataGridViewInstrumenteSuflat.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewInstrumenteSuflat.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewInstrumenteSuflat.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewInstrumenteSuflat.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewInstrumenteSuflat.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewInstrumenteSuflat.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void ClearFormFields()
        {
            textBoxNumeInstrument.Text = "";
            textBoxPret.Text = "";
            textBoxStoc.Text = "";
            comboBoxTipInstrumentSuflat.SelectedIndex = -1;
            textBoxMaterial.Text = "";
            textBoxDimensiuni.Text = "";
            comboBoxFurnizor.SelectedIndex = -1;
        }

        private void IncarcaFurnizoriInComboBox()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT ID_Furnizor, Nume_Furnizor FROM Produse.Furnizori";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);

                    comboBoxFurnizor.DisplayMember = "Nume_Furnizor";
                    comboBoxFurnizor.ValueMember = "ID_Furnizor";
                    comboBoxFurnizor.DataSource = tabelDate;

                    comboBoxFurnizor.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea furnizorilor: " + ex.Message);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxInitializat && comboBoxFurnizor.SelectedItem != null && comboBoxFurnizor.SelectedIndex != -1)
            {
                DataRowView randSelectat = comboBoxFurnizor.SelectedItem as DataRowView;
                if (randSelectat != null)
                {
                    int idFurnizorSelectat = Convert.ToInt32(randSelectat["ID_Furnizor"]);
                    MessageBox.Show($"Furnizorul selectat are ID-ul: {idFurnizorSelectat}");
                }
            }
        }

        private void comboBoxIDFurnizor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void InsertInstrumenteSuflat_Load(object sender, EventArgs e)
        {

        }

        private void SetPlaceholder()
        {
            labelIDInstrument.Text = "Introduceti ID-ul";
            labelIDInstrument.ForeColor = Color.Gray;
            labelIDInstrument.Visible = string.IsNullOrEmpty(textBoxDeleteIDInstrument.Text);
        }

        private void textBoxDeleteIDInstrument_TextChanged(object sender, EventArgs e)
        {
            labelIDInstrument.Visible = string.IsNullOrEmpty(textBoxDeleteIDInstrument.Text);
        }

        private void buttonAdaugaFurnizor_Click(object sender, EventArgs e)
        {
            string numeInstrument = textBoxNumeInstrument.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipInstrument = comboBoxTipInstrumentSuflat.SelectedItem != null ? comboBoxTipInstrumentSuflat.SelectedItem.ToString() : "";
            string material = textBoxMaterial.Text;
            string dimensiuni = textBoxDimensiuni.Text;

            decimal pret;
            if (!decimal.TryParse(pretText, out pret) || pret < 0 || pret > 99999999)
            {
                return;
            }

            int stoc;
            if (!int.TryParse(stocText, out stoc) || stoc < 0 || stoc > 9999)
            {
                return;
            }

            if (string.IsNullOrEmpty(dimensiuni))
            {
                return;
            }

            if (comboBoxFurnizor.SelectedValue == null)
            {
                MessageBox.Show("Selectati un furnizor!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "InsertInstrumentSuflat";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(interogare, conexiune);
                    comanda.CommandType = CommandType.StoredProcedure;
                    comanda.Parameters.AddWithValue("@Nume_Instrument", numeInstrument);
                    comanda.Parameters.AddWithValue("@Pret", pret);
                    comanda.Parameters.AddWithValue("@Stoc", stoc);
                    comanda.Parameters.AddWithValue("@Tip_Instrument", tipInstrument);
                    if (string.IsNullOrEmpty(material))
                        comanda.Parameters.AddWithValue("@Material", DBNull.Value);
                    else
                        comanda.Parameters.AddWithValue("@Material", material);
                    comanda.Parameters.AddWithValue("@Dimensiuni", dimensiuni);
                    comanda.Parameters.AddWithValue("@ID_Furnizor", comboBoxFurnizor.SelectedValue);

                    comanda.ExecuteNonQuery();
                    MessageBox.Show("Instrument adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    LoadInstrumente();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateInstrument_Click_1(object sender, EventArgs e)
        {
            int idInstrument;
            if (!int.TryParse(textBoxDeleteIDInstrument.Text, out idInstrument))
            {
                MessageBox.Show("ID-ul instrumentului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.InstrumenteSuflat WHERE ID_Instrument = @ID_Instrument";
            int idProdus = 0;

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conexiune);
                    checkCmd.Parameters.AddWithValue("@ID_Instrument", idInstrument);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Instrumentul cu ID-ul " + idInstrument + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    idProdus = Convert.ToInt32(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string numeInstrument = textBoxNumeInstrument.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipInstrument = comboBoxTipInstrumentSuflat.SelectedItem != null ? comboBoxTipInstrumentSuflat.SelectedItem.ToString() : "";
            string material = textBoxMaterial.Text;
            string dimensiuni = textBoxDimensiuni.Text;

            StringBuilder queryInstrumenteBuilder = new StringBuilder("UPDATE Produse.InstrumenteSuflat SET ");
            List<SqlParameter> parametriInstrumente = new List<SqlParameter>();
            bool areCampuriInstrumente = false;

            if (!string.IsNullOrEmpty(numeInstrument))
            {
                queryInstrumenteBuilder.Append("Nume_Instrument = @Nume, ");
                parametriInstrumente.Add(new SqlParameter("@Nume", numeInstrument));
                areCampuriInstrumente = true;
            }

            if (!string.IsNullOrEmpty(pretText))
            {
                decimal pret;
                if (!decimal.TryParse(pretText, out pret) || pret < 0 || pret > 99999999)
                {
                    MessageBox.Show("Pretul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryInstrumenteBuilder.Append("Pret = @Pret, ");
                parametriInstrumente.Add(new SqlParameter("@Pret", pret));
                areCampuriInstrumente = true;
            }

            if (!string.IsNullOrEmpty(stocText))
            {
                int stoc;
                if (!int.TryParse(stocText, out stoc) || stoc < 0 || stoc > 9999)
                {
                    MessageBox.Show("Stocul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryInstrumenteBuilder.Append("Stoc = @Stoc, ");
                parametriInstrumente.Add(new SqlParameter("@Stoc", stoc));
                areCampuriInstrumente = true;
            }

            if (!string.IsNullOrEmpty(tipInstrument))
            {
                queryInstrumenteBuilder.Append("Tip_Instrument = @Tip, ");
                parametriInstrumente.Add(new SqlParameter("@Tip", tipInstrument));
                areCampuriInstrumente = true;
            }

            queryInstrumenteBuilder.Append("Material = @Material, ");
            if (string.IsNullOrEmpty(material))
                parametriInstrumente.Add(new SqlParameter("@Material", DBNull.Value));
            else
                parametriInstrumente.Add(new SqlParameter("@Material", material));
            areCampuriInstrumente = true;

            if (!string.IsNullOrEmpty(dimensiuni))
            {
                queryInstrumenteBuilder.Append("Dimensiuni = @Dimensiuni, ");
                parametriInstrumente.Add(new SqlParameter("@Dimensiuni", dimensiuni));
                areCampuriInstrumente = true;
            }

            StringBuilder queryProduseBuilder = new StringBuilder("UPDATE Produse.Produse SET ");
            List<SqlParameter> parametriProduse = new List<SqlParameter>();
            bool areCampuriProduse = false;

            if (!string.IsNullOrEmpty(numeInstrument))
            {
                queryProduseBuilder.Append("Nume_Produs = @Nume, ");
                parametriProduse.Add(new SqlParameter("@Nume", numeInstrument));
                areCampuriProduse = true;
            }

            if (!string.IsNullOrEmpty(pretText))
            {
                decimal pret;
                if (!decimal.TryParse(pretText, out pret) || pret < 0 || pret > 99999999)
                {
                    MessageBox.Show("Pretul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryProduseBuilder.Append("Pret = @Pret, ");
                parametriProduse.Add(new SqlParameter("@Pret", pret));
                areCampuriProduse = true;
            }

            if (!string.IsNullOrEmpty(stocText))
            {
                int stoc;
                if (!int.TryParse(stocText, out stoc) || stoc < 0 || stoc > 9999)
                {
                    MessageBox.Show("Stocul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryProduseBuilder.Append("Stoc = @Stoc, ");
                parametriProduse.Add(new SqlParameter("@Stoc", stoc));
                areCampuriProduse = true;
            }

            if (comboBoxFurnizor.SelectedValue != null)
            {
                queryProduseBuilder.Append("ID_Furnizor = @Furnizor, ");
                parametriProduse.Add(new SqlParameter("@Furnizor", comboBoxFurnizor.SelectedValue));
                areCampuriProduse = true;
            }

            if (!areCampuriInstrumente && !areCampuriProduse)
            {
                MessageBox.Show("Nu au fost introduse date pentru actualizare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (areCampuriInstrumente)
            {
                queryInstrumenteBuilder.Length -= 2; 
                queryInstrumenteBuilder.Append(" WHERE ID_Instrument = @ID_Instrument");
                parametriInstrumente.Add(new SqlParameter("@ID_Instrument", idInstrument));
            }

            if (areCampuriProduse)
            {
                queryProduseBuilder.Length -= 2; 
                queryProduseBuilder.Append(" WHERE ID_Produs = @ID_Produs");
                parametriProduse.Add(new SqlParameter("@ID_Produs", idProdus));
            }

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    if (areCampuriInstrumente)
                    {
                        SqlCommand comandaInstrumente = new SqlCommand(queryInstrumenteBuilder.ToString(), conexiune);
                        comandaInstrumente.Parameters.AddRange(parametriInstrumente.ToArray());
                        comandaInstrumente.ExecuteNonQuery();
                    }

                    if (areCampuriProduse)
                    {
                        SqlCommand comandaProduse = new SqlCommand(queryProduseBuilder.ToString(), conexiune);
                        comandaProduse.Parameters.AddRange(parametriProduse.ToArray());
                        comandaProduse.ExecuteNonQuery();
                    }

                    MessageBox.Show("Instrument actualizat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    textBoxDeleteIDInstrument.Text = "";
                    LoadInstrumente();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteInstrument_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDInstrument.Text))
            {
                MessageBox.Show("Introduceti ID-ul instrumentului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idInstrument;
            if (!int.TryParse(textBoxDeleteIDInstrument.Text, out idInstrument))
            {
                MessageBox.Show("ID-ul instrumentului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.InstrumenteSuflat WHERE ID_Instrument = @ID_Instrument";
            int idProdus = 0;

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conexiune);
                    checkCmd.Parameters.AddWithValue("@ID_Instrument", idInstrument);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Instrumentul cu ID-ul " + idInstrument + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    idProdus = Convert.ToInt32(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("Sigur doriti sa stergeti instrumentul cu ID-ul " + idInstrument + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string queryInstrumente = "DELETE FROM Produse.InstrumenteSuflat WHERE ID_Instrument = @ID_Instrument";
                        SqlCommand comandaInstrumente = new SqlCommand(queryInstrumente, conexiune);
                        comandaInstrumente.Parameters.AddWithValue("@ID_Instrument", idInstrument);
                        comandaInstrumente.ExecuteNonQuery();

                        string queryProduse = "DELETE FROM Produse.Produse WHERE ID_Produs = @ID_Produs";
                        SqlCommand comandaProduse = new SqlCommand(queryProduse, conexiune);
                        comandaProduse.Parameters.AddWithValue("@ID_Produs", idProdus);
                        comandaProduse.ExecuteNonQuery();

                        MessageBox.Show("Instrument sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDInstrument.Text = "";
                        LoadInstrumente();
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