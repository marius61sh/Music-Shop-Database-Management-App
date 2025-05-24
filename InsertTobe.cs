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
    public partial class InsertTobe : Form
    {
        private bool comboBoxInitializat = false;

        public InsertTobe()
        {
            InitializeComponent();
            IncarcaFurnizoriInComboBox();
            comboBoxInitializat = true;
            SetPlaceholder();
            textBoxDeleteIDToba.TextChanged += textBoxDeleteIDToba_TextChanged;
            dataGridViewTobe.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewTobe.MultiSelect = false;
            dataGridViewTobe.Enabled = true;
            LoadTobe();
            dataGridViewTobe.Focus();
        }

        private void LoadTobe()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT t.ID_Produs, t.ID_Toba, t.Nume_Toba, t.Pret, t.Stoc, t.Tip_Toba, t.Numar_Piese, t.Diametru_Toba_Principal, t.Material, p.ID_Furnizor " +
                               "FROM Produse.Tobe t LEFT JOIN Produse.Produse p ON t.ID_Produs = p.ID_Produs";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);
                    if (tabelDate.Rows.Count == 0)
                    {
                        MessageBox.Show("Nu exista tobe in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewTobe.AutoGenerateColumns = true;
                    dataGridViewTobe.DataSource = tabelDate;
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
            dataGridViewTobe.RowHeadersVisible = false;
            dataGridViewTobe.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewTobe.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewTobe.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewTobe.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewTobe.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewTobe.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void ClearFormFields()
        {
            textBoxNumeToba.Text = "";
            textBoxPret.Text = "";
            textBoxStoc.Text = "";
            comboBoxTipToba.SelectedIndex = -1;
            textBoxNumarPiese.Text = "";
            textBoxDiametru.Text = "";
            textBoxMaterial.Text = "";
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

        private void InsertTobe_Load(object sender, EventArgs e)
        {

        }

        private void SetPlaceholder()
        {
            labelIDToba.Text = "Introduceti ID-ul";
            labelIDToba.ForeColor = Color.Gray;
            labelIDToba.Visible = string.IsNullOrEmpty(textBoxDeleteIDToba.Text);
        }

        private void textBoxDeleteIDToba_TextChanged(object sender, EventArgs e)
        {
            labelIDToba.Visible = string.IsNullOrEmpty(textBoxDeleteIDToba.Text);
        }

        private void labelIDProdus_Click(object sender, EventArgs e)
        {

        }

        private void buttonAdaugaTobe_Click(object sender, EventArgs e)
        {
            string numeToba = textBoxNumeToba.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipToba = comboBoxTipToba.SelectedItem != null ? comboBoxTipToba.SelectedItem.ToString() : "";
            string numarPieseText = textBoxNumarPiese.Text;
            string diametruTobaPrincipal = textBoxDiametru.Text;
            string material = textBoxMaterial.Text;

            if (string.IsNullOrEmpty(numeToba) || string.IsNullOrEmpty(tipToba) || string.IsNullOrEmpty(material))
            {
                MessageBox.Show("Completati Nume, Tip Toba si Material!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal pret;
            if (!decimal.TryParse(pretText, out pret) || pret < 0 || pret > 99999999)
            {
                MessageBox.Show("Pretul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int stoc;
            if (!int.TryParse(stocText, out stoc) || stoc < 0 || stoc > 9999)
            {
                MessageBox.Show("Stocul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int numarPiese;
            if (!int.TryParse(numarPieseText, out numarPiese) || numarPiese < 1 || numarPiese > 99)
            {
                MessageBox.Show("Numarul de piese trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (diametruTobaPrincipal.Length > 50)
            {
                MessageBox.Show("Diametrul tobei principale nu poate depasi 50 de caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxFurnizor.SelectedValue == null)
            {
                MessageBox.Show("Selectati un furnizor!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "InsertToba";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(interogare, conexiune);
                    comanda.CommandType = CommandType.StoredProcedure;
                    comanda.Parameters.AddWithValue("@Nume_Toba", numeToba);
                    comanda.Parameters.AddWithValue("@Pret", pret);
                    comanda.Parameters.AddWithValue("@Stoc", stoc);
                    comanda.Parameters.AddWithValue("@Tip_Toba", tipToba);
                    comanda.Parameters.AddWithValue("@Numar_Piese", numarPiese);
                    comanda.Parameters.AddWithValue("@Diametru_Toba_Principal", diametruTobaPrincipal);
                    comanda.Parameters.AddWithValue("@Material", material);
                    comanda.Parameters.AddWithValue("@ID_Furnizor", comboBoxFurnizor.SelectedValue);

                    comanda.ExecuteNonQuery();
                    MessageBox.Show("Toba adaugata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    LoadTobe();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateTobe_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDToba.Text))
            {
                MessageBox.Show("Introduceti ID-ul tobei!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idToba;
            if (!int.TryParse(textBoxDeleteIDToba.Text, out idToba))
            {
                MessageBox.Show("ID-ul tobei trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Tobe WHERE ID_Toba = @ID_Toba";
            int idProdus = 0;

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Toba", idToba);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Toba cu ID-ul " + idToba + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            string numeToba = textBoxNumeToba.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipToba = comboBoxTipToba.SelectedItem != null ? comboBoxTipToba.SelectedItem.ToString() : "";
            string numarPieseText = textBoxNumarPiese.Text;
            string diametruTobaPrincipal = textBoxDiametru.Text;
            string material = textBoxMaterial.Text;

            StringBuilder queryTobeBuilder = new StringBuilder("UPDATE Produse.Tobe SET ");
            List<SqlParameter> parametriTobe = new List<SqlParameter>();
            bool areCampuriTobe = false;

            if (!string.IsNullOrEmpty(numeToba))
            {
                queryTobeBuilder.Append("Nume_Toba = @Nume, ");
                parametriTobe.Add(new SqlParameter("@Nume", numeToba));
                areCampuriTobe = true;
            }

            if (!string.IsNullOrEmpty(pretText))
            {
                decimal pret;
                if (!decimal.TryParse(pretText, out pret) || pret < 0 || pret > 99999999)
                {
                    MessageBox.Show("Pretul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryTobeBuilder.Append("Pret = @Pret, ");
                parametriTobe.Add(new SqlParameter("@Pret", pret));
                areCampuriTobe = true;
            }

            if (!string.IsNullOrEmpty(stocText))
            {
                int stoc;
                if (!int.TryParse(stocText, out stoc) || stoc < 0 || stoc > 9999)
                {
                    MessageBox.Show("Stocul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryTobeBuilder.Append("Stoc = @Stoc, ");
                parametriTobe.Add(new SqlParameter("@Stoc", stoc));
                areCampuriTobe = true;
            }

            if (!string.IsNullOrEmpty(tipToba))
            {
                queryTobeBuilder.Append("Tip_Toba = @Tip, ");
                parametriTobe.Add(new SqlParameter("@Tip", tipToba));
                areCampuriTobe = true;
            }

            if (!string.IsNullOrEmpty(numarPieseText))
            {
                int numarPiese;
                if (!int.TryParse(numarPieseText, out numarPiese) || numarPiese < 1 || numarPiese > 99)
                {
                    MessageBox.Show("Numarul de piese trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryTobeBuilder.Append("Numar_Piese = @NumarPiese, ");
                parametriTobe.Add(new SqlParameter("@NumarPiese", numarPiese));
                areCampuriTobe = true;
            }

            if (diametruTobaPrincipal != null)
            {
                if (diametruTobaPrincipal.Length > 50)
                {
                    MessageBox.Show("Diametrul tobei principale nu poate depasi 50 de caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryTobeBuilder.Append("Diametru_Toba_Principal = @Diametru, ");
                parametriTobe.Add(new SqlParameter("@Diametru", diametruTobaPrincipal));
                areCampuriTobe = true;
            }

            if (!string.IsNullOrEmpty(material))
            {
                queryTobeBuilder.Append("Material = @Material, ");
                parametriTobe.Add(new SqlParameter("@Material", material));
                areCampuriTobe = true;
            }

            StringBuilder queryProduseBuilder = new StringBuilder("UPDATE Produse.Produse SET ");
            List<SqlParameter> parametriProduse = new List<SqlParameter>();
            bool areCampuriProduse = false;

            if (!string.IsNullOrEmpty(numeToba))
            {
                queryProduseBuilder.Append("Nume_Produs = @Nume, ");
                parametriProduse.Add(new SqlParameter("@Nume", numeToba));
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

            if (!areCampuriTobe && !areCampuriProduse)
            {
                MessageBox.Show("Nu au fost introduse date pentru actualizare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (areCampuriTobe)
            {
                queryTobeBuilder.Length -= 2;
                queryTobeBuilder.Append(" WHERE ID_Toba = @ID_Toba");
                parametriTobe.Add(new SqlParameter("@ID_Toba", idToba));
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
                    if (areCampuriTobe)
                    {
                        SqlCommand comandaTobe = new SqlCommand(queryTobeBuilder.ToString(), conexiune);
                        comandaTobe.Parameters.AddRange(parametriTobe.ToArray());
                        comandaTobe.ExecuteNonQuery();
                    }

                    if (areCampuriProduse)
                    {
                        SqlCommand comandaProduse = new SqlCommand(queryProduseBuilder.ToString(), conexiune);
                        comandaProduse.Parameters.AddRange(parametriProduse.ToArray());
                        comandaProduse.ExecuteNonQuery();
                    }

                    MessageBox.Show("Toba actualizata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    textBoxDeleteIDToba.Text = "";
                    LoadTobe();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteTobe_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDToba.Text))
            {
                MessageBox.Show("Introduceti ID-ul tobei!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idToba;
            if (!int.TryParse(textBoxDeleteIDToba.Text, out idToba))
            {
                MessageBox.Show("ID-ul tobei trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Tobe WHERE ID_Toba = @ID_Toba";
            int idProdus = 0;

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Toba", idToba);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Toba cu ID-ul " + idToba + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (MessageBox.Show("Sigur doriti sa stergeti toba cu ID-ul " + idToba + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string queryTobe = "DELETE FROM Produse.Tobe WHERE ID_Toba = @ID_Toba";
                        SqlCommand comandaTobe = new SqlCommand(queryTobe, conexiune);
                        comandaTobe.Parameters.AddWithValue("@ID_Toba", idToba);
                        comandaTobe.ExecuteNonQuery();

                        string queryProduse = "DELETE FROM Produse.Produse WHERE ID_Produs = @ID_Produs";
                        SqlCommand comandaProduse = new SqlCommand(queryProduse, conexiune);
                        comandaProduse.Parameters.AddWithValue("@ID_Produs", idProdus);
                        comandaProduse.ExecuteNonQuery();

                        MessageBox.Show("Toba stersa cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDToba.Text = "";
                        LoadTobe();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void textBoxDiametru_TextChanged(object sender, EventArgs e)
        {

        }
    }
}