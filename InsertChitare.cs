using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;

namespace Studiu_Individual_1
{
    public partial class InsertChitare : Form
    {
        private bool comboBoxInitializat = false;

        public InsertChitare()
        {
            InitializeComponent();
            IncarcaFurnizoriInComboBox();
            comboBoxInitializat = true;
            SetPlaceholder();
            textBoxDeleteIDChitara.TextChanged += textBoxDeleteIDChitara_TextChanged;
            dataGridViewChitare.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewChitare.MultiSelect = false;
            dataGridViewChitare.Enabled = true;
            LoadChitare();
            dataGridViewChitare.Focus();
        }

        private void LoadChitare()
        {
            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "SELECT c.ID_Produs, c.ID_Chitara, c.Nume_Chitara, c.Pret, c.Stoc, c.Tip_Chitara, c.Numar_Corzi, c.Material_Corp, c.Material_Gat, c.Lungime_Scala, p.ID_Furnizor " +
                           "FROM Produse.Chitare c LEFT JOIN Produse.Produse p ON c.ID_Produs = p.ID_Produs";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Nu exista chitare in baza de date.", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    dataGridViewChitare.AutoGenerateColumns = true;
                    dataGridViewChitare.DataSource = dataTable;
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
            dataGridViewChitare.RowHeadersVisible = false;
            dataGridViewChitare.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewChitare.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewChitare.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewChitare.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewChitare.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewChitare.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        private void IncarcaFurnizoriInComboBox()
        {
            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "SELECT ID_Furnizor, Nume_Furnizor FROM Produse.Furnizori";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    comboBoxFurnizor.DisplayMember = "Nume_Furnizor";
                    comboBoxFurnizor.ValueMember = "ID_Furnizor";
                    comboBoxFurnizor.DataSource = dataTable;
                    comboBoxFurnizor.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetPlaceholder()
        {
            labelIDChitara.Text = "ID Chitara";
            labelIDChitara.ForeColor = Color.Gray;
            labelIDChitara.Visible = true;
        }

        private void textBoxDeleteIDChitara_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDeleteIDChitara.Text == "")
            {
                labelIDChitara.Visible = true;
            }
            else
            {
                labelIDChitara.Visible = false;
            }
        }

        private void ClearFormFields()
        {
            textBoxNumeChitara.Text = "";
            textBoxPret.Text = "";
            textBoxStoc.Text = "";
            comboBoxTipChitara.SelectedIndex = -1;
            comboBoxNrCorzi.SelectedIndex = -1;
            textBoxMaterialCorp.Text = "";
            textBoxMaterialGat.Text = "";
            textBoxLungimeScala.Text = "";
            comboBoxFurnizor.SelectedIndex = -1;
        }

        private void UpdateChitara()
        {
            if (dataGridViewChitare.SelectedRows.Count == 0)
            {
                ClearFormFields();
                return;
            }

            DataGridViewRow row = dataGridViewChitare.SelectedRows[0];

            textBoxNumeChitara.Text = row.Cells["Nume_Chitara"].Value != null ? row.Cells["Nume_Chitara"].Value.ToString() : "";
            textBoxPret.Text = row.Cells["Pret"].Value != null ? row.Cells["Pret"].Value.ToString() : "";
            textBoxStoc.Text = row.Cells["Stoc"].Value != null ? row.Cells["Stoc"].Value.ToString() : "";

            string tipChitara = row.Cells["Tip_Chitara"].Value != null ? row.Cells["Tip_Chitara"].Value.ToString() : "";
            if (comboBoxTipChitara.Items.Contains(tipChitara))
            {
                comboBoxTipChitara.SelectedItem = tipChitara;
            }
            else
            {
                comboBoxTipChitara.SelectedIndex = -1;
            }

            string numarCorzi = row.Cells["Numar_Corzi"].Value != null ? row.Cells["Numar_Corzi"].Value.ToString() : "";
            if (comboBoxNrCorzi.Items.Contains(numarCorzi))
            {
                comboBoxNrCorzi.SelectedItem = numarCorzi;
            }
            else
            {
                comboBoxNrCorzi.SelectedIndex = -1;
            }

            textBoxMaterialCorp.Text = row.Cells["Material_Corp"].Value != null ? row.Cells["Material_Corp"].Value.ToString() : "";
            textBoxMaterialGat.Text = row.Cells["Material_Gat"].Value != null ? row.Cells["Material_Gat"].Value.ToString() : "";
            textBoxLungimeScala.Text = row.Cells["Lungime_Scala"].Value != null ? row.Cells["Lungime_Scala"].Value.ToString() : "";

            if (row.Cells["ID_Furnizor"].Value != null)
            {
                comboBoxFurnizor.SelectedValue = row.Cells["ID_Furnizor"].Value;
            }
            else
            {
                comboBoxFurnizor.SelectedIndex = -1;
            }
        }

        private void dataGridViewChitare_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void InsertChitare_Load(object sender, EventArgs e)
        {
        }

        private void buttonAdaugaChitara_Click(object sender, EventArgs e)
        {
            string numeChitara = textBoxNumeChitara.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipChitara = comboBoxTipChitara.SelectedItem != null ? comboBoxTipChitara.SelectedItem.ToString() : "";
            string numarCorziText = comboBoxNrCorzi.SelectedItem != null ? comboBoxNrCorzi.SelectedItem.ToString() : "";
            string materialCorp = textBoxMaterialCorp.Text;
            string materialGat = textBoxMaterialGat.Text;
            string lungimeScalaText = textBoxLungimeScala.Text;

            if (numeChitara == "" || tipChitara == "" || numarCorziText == "")
            {
                MessageBox.Show("Completati Nume, Tip si Numar Corzi!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            int numarCorzi;
            if (!int.TryParse(numarCorziText, out numarCorzi) || numarCorzi < 4 || numarCorzi > 12)
            {
                MessageBox.Show("Numarul de corzi trebuie sa fie intre 4 si 12!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal lungimeScala;
            if (!decimal.TryParse(lungimeScalaText, out lungimeScala) || lungimeScala < 0 || lungimeScala > 999)
            {
                MessageBox.Show("Lungimea scalei trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxFurnizor.SelectedValue == null)
            {
                MessageBox.Show("Selectati un furnizor!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "InsertChitara";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nume_Chitara", numeChitara);
                    command.Parameters.AddWithValue("@Pret", pret);
                    command.Parameters.AddWithValue("@Stoc", stoc);
                    command.Parameters.AddWithValue("@Tip_Chitara", tipChitara);
                    command.Parameters.AddWithValue("@Numar_Corzi", numarCorzi);
                    if (materialCorp == "")
                        command.Parameters.AddWithValue("@Material_Corp", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Material_Corp", materialCorp);
                    if (materialGat == "")
                        command.Parameters.AddWithValue("@Material_Gat", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Material_Gat", materialGat);
                    command.Parameters.AddWithValue("@Lungime_Scala", lungimeScala);
                    command.Parameters.AddWithValue("@ID_Furnizor", comboBoxFurnizor.SelectedValue);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Chitara adaugata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    LoadChitare();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdateChitara_Click_1(object sender, EventArgs e)
        {
            if (textBoxDeleteIDChitara.Text == "")
            {
                MessageBox.Show("Introduceti ID-ul chitarei!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idChitara;
            if (!int.TryParse(textBoxDeleteIDChitara.Text, out idChitara))
            {
                MessageBox.Show("ID-ul chitarei trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Chitare WHERE ID_Chitara = @ID_Chitara";
            int idProdus = 0;

            using (SqlConnection checkConn = new SqlConnection(connectionString))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Chitara", idChitara);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Chitara cu ID-ul " + idChitara + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            string numeChitara = textBoxNumeChitara.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tipChitara = comboBoxTipChitara.SelectedItem != null ? comboBoxTipChitara.SelectedItem.ToString() : "";
            string numarCorziText = comboBoxNrCorzi.SelectedItem != null ? comboBoxNrCorzi.SelectedItem.ToString() : "";
            string materialCorp = textBoxMaterialCorp.Text;
            string materialGat = textBoxMaterialGat.Text;
            string lungimeScalaText = textBoxLungimeScala.Text;

            StringBuilder queryChitareBuilder = new StringBuilder("UPDATE Produse.Chitare SET ");
            List<SqlParameter> parametriChitare = new List<SqlParameter>();
            bool areCampuriChitare = false;

            if (!string.IsNullOrEmpty(numeChitara))
            {
                queryChitareBuilder.Append("Nume_Chitara = @Nume, ");
                parametriChitare.Add(new SqlParameter("@Nume", numeChitara));
                areCampuriChitare = true;
            }

            if (!string.IsNullOrEmpty(pretText))
            {
                decimal pret;
                if (!decimal.TryParse(pretText, out pret) || pret < 0 || pret > 99999999)
                {
                    MessageBox.Show("Pretul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryChitareBuilder.Append("Pret = @Pret, ");
                parametriChitare.Add(new SqlParameter("@Pret", pret));
                areCampuriChitare = true;
            }

            if (!string.IsNullOrEmpty(stocText))
            {
                int stoc;
                if (!int.TryParse(stocText, out stoc) || stoc < 0 || stoc > 9999)
                {
                    MessageBox.Show("Stocul trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryChitareBuilder.Append("Stoc = @Stoc, ");
                parametriChitare.Add(new SqlParameter("@Stoc", stoc));
                areCampuriChitare = true;
            }

            if (!string.IsNullOrEmpty(tipChitara))
            {
                queryChitareBuilder.Append("Tip_Chitara = @Tip, ");
                parametriChitare.Add(new SqlParameter("@Tip", tipChitara));
                areCampuriChitare = true;
            }

            if (!string.IsNullOrEmpty(numarCorziText))
            {
                int numarCorzi;
                if (!int.TryParse(numarCorziText, out numarCorzi) || numarCorzi < 4 || numarCorzi > 12)
                {
                    MessageBox.Show("Numarul de corzi trebuie sa fie intre 4 si 12!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryChitareBuilder.Append("Numar_Corzi = @NumarCorzi, ");
                parametriChitare.Add(new SqlParameter("@NumarCorzi", numarCorzi));
                areCampuriChitare = true;
            }

            queryChitareBuilder.Append("Material_Corp = @MaterialCorp, ");
            if (materialCorp == "")
                parametriChitare.Add(new SqlParameter("@MaterialCorp", DBNull.Value));
            else
                parametriChitare.Add(new SqlParameter("@MaterialCorp", materialCorp));
            areCampuriChitare = true;

            queryChitareBuilder.Append("Material_Gat = @MaterialGat, ");
            if (materialGat == "")
                parametriChitare.Add(new SqlParameter("@MaterialGat", DBNull.Value));
            else
                parametriChitare.Add(new SqlParameter("@MaterialGat", materialGat));
            areCampuriChitare = true;

            if (!string.IsNullOrEmpty(lungimeScalaText))
            {
                decimal lungimeScala;
                if (!decimal.TryParse(lungimeScalaText, out lungimeScala) || lungimeScala < 0 || lungimeScala > 999)
                {
                    MessageBox.Show("Lungimea scalei trebuie sa fie un numar valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                queryChitareBuilder.Append("Lungime_Scala = @LungimeScala, ");
                parametriChitare.Add(new SqlParameter("@LungimeScala", lungimeScala));
                areCampuriChitare = true;
            }

            StringBuilder queryProduseBuilder = new StringBuilder("UPDATE Produse.Produse SET ");
            List<SqlParameter> parametriProduse = new List<SqlParameter>();
            bool areCampuriProduse = false;

            if (!string.IsNullOrEmpty(numeChitara))
            {
                queryProduseBuilder.Append("Nume_Produs = @Nume, ");
                parametriProduse.Add(new SqlParameter("@Nume", numeChitara));
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

            if (!areCampuriChitare && !areCampuriProduse)
            {
                MessageBox.Show("Nu au fost introduse date pentru actualizare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (areCampuriChitare)
            {
                queryChitareBuilder.Length -= 2;
                queryChitareBuilder.Append(" WHERE ID_Chitara = @ID_Chitara");
                parametriChitare.Add(new SqlParameter("@ID_Chitara", idChitara));
            }

            if (areCampuriProduse)
            {
                queryProduseBuilder.Length -= 2;
                queryProduseBuilder.Append(" WHERE ID_Produs = @ID_Produs");
                parametriProduse.Add(new SqlParameter("@ID_Produs", idProdus));
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    if (areCampuriChitare)
                    {
                        SqlCommand commandChitare = new SqlCommand(queryChitareBuilder.ToString(), connection);
                        commandChitare.Parameters.AddRange(parametriChitare.ToArray());
                        commandChitare.ExecuteNonQuery();
                    }

                    if (areCampuriProduse)
                    {
                        SqlCommand commandProduse = new SqlCommand(queryProduseBuilder.ToString(), connection);
                        commandProduse.Parameters.AddRange(parametriProduse.ToArray());
                        commandProduse.ExecuteNonQuery();
                    }

                    MessageBox.Show("Chitara actualizata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    LoadChitare();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDeleteChitara_Click(object sender, EventArgs e)
        {
            if (textBoxDeleteIDChitara.Text == "")
            {
                MessageBox.Show("Introduceti ID-ul chitarei!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idChitara;
            if (!int.TryParse(textBoxDeleteIDChitara.Text, out idChitara))
            {
                MessageBox.Show("ID-ul chitarei trebuie sa fie un numar!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string checkQuery = "SELECT ID_Produs FROM Produse.Chitare WHERE ID_Chitara = @ID_Chitara";
            int idProdus = 0;

            using (SqlConnection checkConn = new SqlConnection(connectionString))
            {
                try
                {
                    checkConn.Open();
                    SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn);
                    checkCmd.Parameters.AddWithValue("@ID_Chitara", idChitara);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Chitara cu ID-ul " + idChitara + " nu exista!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (MessageBox.Show("Sigur doriti sa stergeti chitara cu ID-ul " + idChitara + "?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string queryChitare = "DELETE FROM Produse.Chitare WHERE ID_Chitara = @ID_Chitara";
                        SqlCommand commandChitare = new SqlCommand(queryChitare, connection);
                        commandChitare.Parameters.AddWithValue("@ID_Chitara", idChitara);
                        commandChitare.ExecuteNonQuery();

                        string queryProduse = "DELETE FROM Produse.Produse WHERE ID_Produs = @ID_Produs";
                        SqlCommand commandProduse = new SqlCommand(queryProduse, connection);
                        commandProduse.Parameters.AddWithValue("@ID_Produs", idProdus);
                        commandProduse.ExecuteNonQuery();

                        MessageBox.Show("Chitara stearsa cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxDeleteIDChitara.Text = "";
                        LoadChitare();
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