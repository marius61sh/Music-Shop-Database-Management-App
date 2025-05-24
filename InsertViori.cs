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
    public partial class InsertViori : Form
    {
        private bool comboBoxInitializat = false; 


        public InsertViori()
        {
            InitializeComponent();
            IncarcaFurnizoriInComboBox();
            comboBoxInitializat = true;
            SetPlaceholder();
            textBoxDeleteIDVioara.TextChanged += textBoxDeleteIDVioara_TextChanged;
            dataGridViewViori.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewViori.MultiSelect = false;
            dataGridViewViori.Enabled = true;
            LoadViori();
            dataGridViewViori.Focus();
        }

        private void LoadViori()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT v.ID_Produs, v.ID_Vioara, v.Nume_Vioara, v.Pret, v.Stoc, v.Tip_Vioara, v.Material, p.ID_Furnizor " +
                               "FROM Produse.Viori v LEFT JOIN Produse.Produse p ON v.ID_Produs = p.ID_Produs";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);
                    if (tabelDate.Rows.Count == 0)
                    {
                        MessageBox.Show("Nu exista viori in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewViori.AutoGenerateColumns = true;
                    dataGridViewViori.DataSource = tabelDate;
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
            dataGridViewViori.RowHeadersVisible = false;
            dataGridViewViori.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewViori.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewViori.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewViori.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewViori.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewViori.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void ClearFormFields()
        {
            textBoxNumeVioara.Text = "";
            textBoxPret.Text = "";
            textBoxStoc.Text = "";
            comboBoxTipVioara.SelectedIndex = -1;
            textBoxMaterial.Text = "";
            comboBoxFurnizor.SelectedIndex = -1;
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

        private void InsertViori_Load(object sender, EventArgs e)
        {

        }

        private void SetPlaceholder()
        {
            labelIDVioara.Text = "Introduceti ID-ul";
            labelIDVioara.ForeColor = Color.Gray;
            labelIDVioara.Visible = string.IsNullOrEmpty(textBoxDeleteIDVioara.Text);
        }

        private void textBoxDeleteIDVioara_TextChanged(object sender, EventArgs e)
        {
            labelIDVioara.Visible = string.IsNullOrEmpty(textBoxDeleteIDVioara.Text);
        }

        private void buttonAdaugaVioara_Click_1(object sender, EventArgs e)
        {
            string numeVioara = textBoxNumeVioara.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipVioara = comboBoxTipVioara.SelectedItem != null ? comboBoxTipVioara.SelectedItem.ToString() : "";
            string material = textBoxMaterial.Text;

            if (string.IsNullOrEmpty(numeVioara) || string.IsNullOrEmpty(tipVioara) || string.IsNullOrEmpty(material))
            {
                MessageBox.Show("Completati Nume, Tip Vioara si Material!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (material.Length > 100)
            {
                MessageBox.Show("Materialul nu poate depasi 100 de caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxFurnizor.SelectedValue == null)
            {
                MessageBox.Show("Selectati un furnizor!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "InsertVioara";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(interogare, conexiune);
                    comanda.CommandType = CommandType.StoredProcedure;
                    comanda.Parameters.AddWithValue("@Nume_Vioara", numeVioara);
                    comanda.Parameters.AddWithValue("@Pret", pret);
                    comanda.Parameters.AddWithValue("@Stoc", stoc);
                    comanda.Parameters.AddWithValue("@Tip_Vioara", tipVioara);
                    comanda.Parameters.AddWithValue("@Material", material);
                    comanda.Parameters.AddWithValue("@ID_Furnizor", comboBoxFurnizor.SelectedValue);

                    comanda.ExecuteNonQuery();
                    MessageBox.Show("Vioara adaugata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    LoadViori();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateVioara_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDVioara.Text))
            {
                MessageBox.Show("Introduceti ID-ul viorii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idVioara;
            if (!int.TryParse(textBoxDeleteIDVioara.Text, out idVioara))
            {
                MessageBox.Show("ID-ul viorii trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Viori WHERE ID_Vioara = @ID_Vioara";
            int idProdus = 0;

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Vioara", idVioara);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Vioara cu ID-ul " + idVioara + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            string numeVioara = textBoxNumeVioara.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipVioara = comboBoxTipVioara.SelectedItem != null ? comboBoxTipVioara.SelectedItem.ToString() : "";
            string material = textBoxMaterial.Text;

            StringBuilder queryVioriBuilder = new StringBuilder("UPDATE Produse.Viori SET ");
            List<SqlParameter> parametriViori = new List<SqlParameter>();
            bool areCampuriViori = false;

            if (!string.IsNullOrEmpty(numeVioara))
            {
                queryVioriBuilder.Append("Nume_Vioara = @Nume, ");
                parametriViori.Add(new SqlParameter("@Nume", numeVioara));
                areCampuriViori = true;
            }

            if (!string.IsNullOrEmpty(pretText))
            {
                decimal pret;
                if (!decimal.TryParse(pretText, out pret) || pret < 0 || pret > 99999999)
                {
                    MessageBox.Show("Pretul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryVioriBuilder.Append("Pret = @Pret, ");
                parametriViori.Add(new SqlParameter("@Pret", pret));
                areCampuriViori = true;
            }

            if (!string.IsNullOrEmpty(stocText))
            {
                int stoc;
                if (!int.TryParse(stocText, out stoc) || stoc < 0 || stoc > 9999)
                {
                    MessageBox.Show("Stocul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryVioriBuilder.Append("Stoc = @Stoc, ");
                parametriViori.Add(new SqlParameter("@Stoc", stoc));
                areCampuriViori = true;
            }

            if (!string.IsNullOrEmpty(tipVioara))
            {
                queryVioriBuilder.Append("Tip_Vioara = @Tip, ");
                parametriViori.Add(new SqlParameter("@Tip", tipVioara));
                areCampuriViori = true;
            }

            if (!string.IsNullOrEmpty(material))
            {
                if (material.Length > 100)
                {
                    MessageBox.Show("Materialul nu poate depasi 100 de caractere!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryVioriBuilder.Append("Material = @Material, ");
                parametriViori.Add(new SqlParameter("@Material", material));
                areCampuriViori = true;
            }

            StringBuilder queryProduseBuilder = new StringBuilder("UPDATE Produse.Produse SET ");
            List<SqlParameter> parametriProduse = new List<SqlParameter>();
            bool areCampuriProduse = false;

            if (!string.IsNullOrEmpty(numeVioara))
            {
                queryProduseBuilder.Append("Nume_Produs = @Nume, ");
                parametriProduse.Add(new SqlParameter("@Nume", numeVioara));
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

            if (!areCampuriViori && !areCampuriProduse)
            {
                MessageBox.Show("Nu au fost introduse date pentru actualizare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (areCampuriViori)
            {
                queryVioriBuilder.Length -= 2;
                queryVioriBuilder.Append(" WHERE ID_Vioara = @ID_Vioara");
                parametriViori.Add(new SqlParameter("@ID_Vioara", idVioara));
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
                    if (areCampuriViori)
                    {
                        SqlCommand comandaViori = new SqlCommand(queryVioriBuilder.ToString(), conexiune);
                        comandaViori.Parameters.AddRange(parametriViori.ToArray());
                        comandaViori.ExecuteNonQuery();
                    }

                    if (areCampuriProduse)
                    {
                        SqlCommand comandaProduse = new SqlCommand(queryProduseBuilder.ToString(), conexiune);
                        comandaProduse.Parameters.AddRange(parametriProduse.ToArray());
                        comandaProduse.ExecuteNonQuery();
                    }

                    MessageBox.Show("Vioara actualizata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    textBoxDeleteIDVioara.Text = "";
                    LoadViori();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteVioara_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDVioara.Text))
            {
                MessageBox.Show("Introduceti ID-ul viorii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idVioara;
            if (!int.TryParse(textBoxDeleteIDVioara.Text, out idVioara))
            {
                MessageBox.Show("ID-ul viorii trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Viori WHERE ID_Vioara = @ID_Vioara";
            int idProdus = 0;

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Vioara", idVioara);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Vioara cu ID-ul " + idVioara + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (MessageBox.Show("Sigur doriti sa stergeti vioara cu ID-ul " + idVioara + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string queryViori = "DELETE FROM Produse.Viori WHERE ID_Vioara = @ID_Vioara";
                        SqlCommand comandaViori = new SqlCommand(queryViori, conexiune);
                        comandaViori.Parameters.AddWithValue("@ID_Vioara", idVioara);
                        comandaViori.ExecuteNonQuery();

                        string queryProduse = "DELETE FROM Produse.Produse WHERE ID_Produs = @ID_Produs";
                        SqlCommand comandaProduse = new SqlCommand(queryProduse, conexiune);
                        comandaProduse.Parameters.AddWithValue("@ID_Produs", idProdus);
                        comandaProduse.ExecuteNonQuery();

                        MessageBox.Show("Vioara stersa cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDVioara.Text = "";
                        LoadViori();
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