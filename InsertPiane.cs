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
    public partial class InsertPiane : Form
    {
        private bool comboBoxInitializat = false;

        public InsertPiane()
        {
            InitializeComponent();
            IncarcaFurnizoriInComboBox();
            comboBoxInitializat = true;
            SetPlaceholder();
            textBoxDeleteIDPian.TextChanged += textBoxDeleteIDPian_TextChanged;
            dataGridViewPiane.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewPiane.MultiSelect = false;
            dataGridViewPiane.Enabled = true;
            LoadPiane();
            dataGridViewPiane.Focus();
        }

        private void LoadPiane()
        {
            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "SELECT p.ID_Produs, p.ID_Pian, p.Nume_Pian, p.Pret, p.Stoc, p.Tip_Pian, p.Numar_Clape, p.Greutate, p.Dimensiuni, pr.ID_Furnizor " +
                               "FROM Produse.Piane p LEFT JOIN Produse.Produse pr ON p.ID_Produs = pr.ID_Produs";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    SqlDataAdapter adaptor = new SqlDataAdapter(interogare, conexiune);
                    DataTable tabelDate = new DataTable();
                    adaptor.Fill(tabelDate);
                    if (tabelDate.Rows.Count == 0)
                    {
                        MessageBox.Show("Nu exista piane in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewPiane.AutoGenerateColumns = true;
                    dataGridViewPiane.DataSource = tabelDate;
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
            dataGridViewPiane.RowHeadersVisible = false;
            dataGridViewPiane.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewPiane.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewPiane.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewPiane.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewPiane.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewPiane.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void ClearFormFields()
        {
            textBoxNumePian.Text = "";
            textBoxPret.Text = "";
            textBoxStoc.Text = "";
            comboBoxTipPian.SelectedIndex = -1;
            textBoxNrClape.Text = "";
            textBoxGreutate.Text = "";
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

        private void InsertPiane_Load(object sender, EventArgs e)
        {

        }

        private void SetPlaceholder()
        {
            labelIDPian.Text = "Introduceti ID-ul";
            labelIDPian.ForeColor = Color.Gray;
            labelIDPian.Visible = string.IsNullOrEmpty(textBoxDeleteIDPian.Text);
        }

        private void textBoxDeleteIDPian_TextChanged(object sender, EventArgs e)
        {
            labelIDPian.Visible = string.IsNullOrEmpty(textBoxDeleteIDPian.Text);
        }

        private void buttonAdaugaPian_Click_1(object sender, EventArgs e)
        {
            string numePian = textBoxNumePian.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipPian = comboBoxTipPian.SelectedItem != null ? comboBoxTipPian.SelectedItem.ToString() : "";
            string numarClapeText = textBoxNrClape.Text;
            string greutateText = textBoxGreutate.Text;
            string dimensiuni = textBoxDimensiuni.Text;

            if (string.IsNullOrEmpty(numePian) || string.IsNullOrEmpty(tipPian))
            {
                MessageBox.Show("Completati Nume si Tip Pian!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            int numarClape;
            if (!int.TryParse(numarClapeText, out numarClape) || numarClape < 0 || numarClape > 999)
            {
                MessageBox.Show("Numarul de clape trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal greutate;
            if (!decimal.TryParse(greutateText, out greutate) || greutate < 0 || greutate > 999)
            {
                MessageBox.Show("Greutatea trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(dimensiuni))
            {
                MessageBox.Show("Dimensiunile trebuie specificate!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxFurnizor.SelectedValue == null)
            {
                MessageBox.Show("Selectati un furnizor!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string interogare = "InsertPian";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(interogare, conexiune);
                    comanda.CommandType = CommandType.StoredProcedure;
                    comanda.Parameters.AddWithValue("@Nume_Pian", numePian);
                    comanda.Parameters.AddWithValue("@Pret", pret);
                    comanda.Parameters.AddWithValue("@Stoc", stoc);
                    comanda.Parameters.AddWithValue("@Tip_Pian", tipPian);
                    comanda.Parameters.AddWithValue("@Numar_Clape", numarClape);
                    comanda.Parameters.AddWithValue("@Greutate", greutate);
                    comanda.Parameters.AddWithValue("@Dimensiuni", dimensiuni);
                    comanda.Parameters.AddWithValue("@ID_Furnizor", comboBoxFurnizor.SelectedValue);

                    comanda.ExecuteNonQuery();
                    MessageBox.Show("Pian adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    LoadPiane();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdatePian_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDPian.Text))
            {
                MessageBox.Show("Introduceti ID-ul pianului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idPian;
            if (!int.TryParse(textBoxDeleteIDPian.Text, out idPian))
            {
                MessageBox.Show("ID-ul pianului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Piane WHERE ID_Pian = @ID_Pian";
            int idProdus = 0;

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Pian", idPian);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Pianul cu ID-ul " + idPian + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            string numePian = textBoxNumePian.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipPian = comboBoxTipPian.SelectedItem != null ? comboBoxTipPian.SelectedItem.ToString() : "";
            string numarClapeText = textBoxNrClape.Text;
            string greutateText = textBoxGreutate.Text;
            string dimensiuni = textBoxDimensiuni.Text;

            StringBuilder queryPianeBuilder = new StringBuilder("UPDATE Produse.Piane SET ");
            List<SqlParameter> parametriPiane = new List<SqlParameter>();
            bool areCampuriPiane = false;

            if (!string.IsNullOrEmpty(numePian))
            {
                queryPianeBuilder.Append("Nume_Pian = @Nume, ");
                parametriPiane.Add(new SqlParameter("@Nume", numePian));
                areCampuriPiane = true;
            }

            if (!string.IsNullOrEmpty(pretText))
            {
                decimal pret;
                if (!decimal.TryParse(pretText, out pret) || pret < 0 || pret > 99999999)
                {
                    MessageBox.Show("Pretul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryPianeBuilder.Append("Pret = @Pret, ");
                parametriPiane.Add(new SqlParameter("@Pret", pret));
                areCampuriPiane = true;
            }

            if (!string.IsNullOrEmpty(stocText))
            {
                int stoc;
                if (!int.TryParse(stocText, out stoc) || stoc < 0 || stoc > 9999)
                {
                    MessageBox.Show("Stocul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryPianeBuilder.Append("Stoc = @Stoc, ");
                parametriPiane.Add(new SqlParameter("@Stoc", stoc));
                areCampuriPiane = true;
            }

            if (!string.IsNullOrEmpty(tipPian))
            {
                queryPianeBuilder.Append("Tip_Pian = @Tip, ");
                parametriPiane.Add(new SqlParameter("@Tip", tipPian));
                areCampuriPiane = true;
            }

            if (!string.IsNullOrEmpty(numarClapeText))
            {
                int numarClape;
                if (!int.TryParse(numarClapeText, out numarClape) || numarClape < 0 || numarClape > 999)
                {
                    MessageBox.Show("Numarul de clape trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryPianeBuilder.Append("Numar_Clape = @NumarClape, ");
                parametriPiane.Add(new SqlParameter("@NumarClape", numarClape));
                areCampuriPiane = true;
            }

            if (!string.IsNullOrEmpty(greutateText))
            {
                decimal greutate;
                if (!decimal.TryParse(greutateText, out greutate) || greutate < 0 || greutate > 999)
                {
                    MessageBox.Show("Greutatea trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryPianeBuilder.Append("Greutate = @Greutate, ");
                parametriPiane.Add(new SqlParameter("@Greutate", greutate));
                areCampuriPiane = true;
            }

            if (!string.IsNullOrEmpty(dimensiuni))
            {
                queryPianeBuilder.Append("Dimensiuni = @Dimensiuni, ");
                parametriPiane.Add(new SqlParameter("@Dimensiuni", dimensiuni));
                areCampuriPiane = true;
            }

            StringBuilder queryProduseBuilder = new StringBuilder("UPDATE Produse.Produse SET ");
            List<SqlParameter> parametriProduse = new List<SqlParameter>();
            bool areCampuriProduse = false;

            if (!string.IsNullOrEmpty(numePian))
            {
                queryProduseBuilder.Append("Nume_Produs = @Nume, ");
                parametriProduse.Add(new SqlParameter("@Nume", numePian));
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

            if (areCampuriPiane)
            {
                queryPianeBuilder.Length -= 2;
                queryPianeBuilder.Append(" WHERE ID_Pian = @ID_Pian");
                parametriPiane.Add(new SqlParameter("@ID_Pian", idPian));
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
                    if (areCampuriPiane)
                    {
                        SqlCommand comandaPiane = new SqlCommand(queryPianeBuilder.ToString(), conexiune);
                        comandaPiane.Parameters.AddRange(parametriPiane.ToArray());
                        comandaPiane.ExecuteNonQuery();
                    }

                    if (areCampuriProduse)
                    {
                        SqlCommand comandaProduse = new SqlCommand(queryProduseBuilder.ToString(), conexiune);
                        comandaProduse.Parameters.AddRange(parametriProduse.ToArray());
                        comandaProduse.ExecuteNonQuery();
                    }

                    MessageBox.Show("Pian actualizat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    textBoxDeleteIDPian.Text = "";
                    LoadPiane();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeletePian_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDPian.Text))
            {
                MessageBox.Show("Introduceti ID-ul pianului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idPian;
            if (!int.TryParse(textBoxDeleteIDPian.Text, out idPian))
            {
                MessageBox.Show("ID-ul pianului trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Piane WHERE ID_Pian = @ID_Pian";
            int idProdus = 0;

            using (SqlConnection checkConn = new SqlConnection(sirConexiune))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Pian", idPian);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Pianul cu ID-ul " + idPian + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (MessageBox.Show("Sigur doriti sa stergeti pianul cu ID-ul " + idPian + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conexiune = new SqlConnection(sirConexiune))
                {
                    try
                    {
                        conexiune.Open();
                        string queryPiane = "DELETE FROM Produse.Piane WHERE ID_Pian = @ID_Pian";
                        SqlCommand comandaPiane = new SqlCommand(queryPiane, conexiune);
                        comandaPiane.Parameters.AddWithValue("@ID_Pian", idPian);
                        comandaPiane.ExecuteNonQuery();

                        string queryProduse = "DELETE FROM Produse.Produse WHERE ID_Produs = @ID_Produs";
                        SqlCommand comandaProduse = new SqlCommand(queryProduse, conexiune);
                        comandaProduse.Parameters.AddWithValue("@ID_Produs", idProdus);

                        MessageBox.Show("Pian sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDPian.Text = "";
                        LoadPiane();
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