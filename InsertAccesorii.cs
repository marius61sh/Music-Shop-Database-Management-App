using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Studiu_Individual_1
{
    public partial class InsertAccesorii : Form
    {
        // Variabila pentru a verifica daca ComboBox-ul a fost initializat
        private bool comboBoxInitializat = false;

        // Constructorul formularului
        public InsertAccesorii()
        {
            InitializeComponent();
            IncarcaFurnizoriInComboBox();
            comboBoxInitializat = true;
            SetPlaceholder();
            textBoxDeleteIDAccesoriu.TextChanged += textBoxDeleteIDAccesoriu_TextChanged;
            LoadAccesorii();
        }

        // Incarca accesoriile in DataGridView
        private void LoadAccesorii()
        {
            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "SELECT ID_Accesoriu, Nume_Accesoriu, Pret, Stoc, Tip_Accesoriu, Categorie_Instrument, Specificatii, Material, ID_Furnizor FROM Produse.Accesorii";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridViewAngajati.DataSource = dataTable;
                    CustomizeDataGridView();
                }
                catch
                {
                    MessageBox.Show("Eroare la incarcarea accesoriilor");
                }
            }
        }

        // Personalizeaza aspectul DataGridView
        private void CustomizeDataGridView()
        {
            dataGridViewAngajati.RowHeadersVisible = false;
            dataGridViewAngajati.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridViewAngajati.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewAngajati.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewAngajati.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewAngajati.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewAngajati.ColumnHeadersDefaultCellStyle.Font = new Font("Cambria", 12, FontStyle.Bold);
        }

        // Incarca furnizorii in ComboBox
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
                    comboBoxIDFurnizor.DisplayMember = "Nume_Furnizor";
                    comboBoxIDFurnizor.ValueMember = "ID_Furnizor";
                    comboBoxIDFurnizor.DataSource = dataTable;
                    comboBoxIDFurnizor.SelectedIndex = -1;
                }
                catch
                {
                    MessageBox.Show("Eroare la incarcarea furnizorilor");
                }
            }
        }

        // Gestionarea selectiei in ComboBox
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxInitializat && comboBoxIDFurnizor.SelectedItem != null && comboBoxIDFurnizor.SelectedIndex != -1)
            {
                DataRowView selectedRow = comboBoxIDFurnizor.SelectedItem as DataRowView;
                if (selectedRow != null)
                {
                    int supplierId = Convert.ToInt32(selectedRow["ID_Furnizor"]);
                    MessageBox.Show("Furnizorul selectat are ID-ul: " + supplierId.ToString());
                }
            }
        }

        // Seteaza placeholder-ul pentru textBoxDeleteIDAccesoriu
        private void SetPlaceholder()
        {
            labelIDClient.Text = "Introduceti ID-ul";
            labelIDClient.ForeColor = Color.Gray;
            labelIDClient.Visible = string.IsNullOrEmpty(textBoxDeleteIDAccesoriu.Text);
        }

        // Actualizeaza vizibilitatea placeholder-ului
        private void textBoxDeleteIDAccesoriu_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDAccesoriu.Text))
            {
                labelIDClient.Visible = true;
            }
            else
            {
                labelIDClient.Visible = false;
            }
        }

        // Curata toate campurile formularului
        private void ClearFormFields()
        {
            numericUpDownIDAccesoriu.Value = 0;
            textBoxNume.Clear();
            textBoxPret.Clear();
            textBoxStoc.Clear();
            textBoxTip.Clear();
            textBoxCategorie.Clear();
            textBoxSpecificatii.Clear();
            textBoxMaterial.Clear();
            comboBoxIDFurnizor.SelectedIndex = -1;
            numericUpDownIDAccesoriu.ReadOnly = false;
        }

        // Populeaza campurile cu datele accesoriului selectat
        private void UpdateAccesoriu(string idAccesoriu)
        {
            var row = dataGridViewAngajati.SelectedRows[0];
            numericUpDownIDAccesoriu.Text = row.Cells["ID_Accesoriu"].Value.ToString().TrimStart('0');
            textBoxNume.Text = row.Cells["Nume_Accesoriu"].Value.ToString();
            textBoxPret.Text = row.Cells["Pret"].Value.ToString();
            textBoxStoc.Text = row.Cells["Stoc"].Value.ToString();
            textBoxTip.Text = row.Cells["Tip_Accesoriu"].Value.ToString();
            textBoxCategorie.Text = row.Cells["Categorie_Instrument"].Value.ToString();
            textBoxSpecificatii.Text = row.Cells["Specificatii"].Value.ToString();
            textBoxMaterial.Text = row.Cells["Material"].Value.ToString();
            comboBoxIDFurnizor.SelectedValue = row.Cells["ID_Furnizor"].Value != DBNull.Value ? row.Cells["ID_Furnizor"].Value : -1;
            numericUpDownIDAccesoriu.ReadOnly = true;
        }

        // Adauga un accesoriu in baza de date
        private void buttonAddAccesoriu_Click(object sender, EventArgs e)
        {
            string idAccesoriu = numericUpDownIDAccesoriu.Text;
            string nume = textBoxNume.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tip = textBoxTip.Text;
            string categorie = textBoxCategorie.Text;
            string specificatii = textBoxSpecificatii.Text;
            string material = textBoxMaterial.Text;

            if (string.IsNullOrEmpty(idAccesoriu) || string.IsNullOrEmpty(nume) || string.IsNullOrEmpty(tip) || string.IsNullOrEmpty(categorie))
            {
                MessageBox.Show("Completati toate campurile obligatorii (ID, Nume, Tip, Categorie)");
                return;
            }

            decimal pret;
            if (!decimal.TryParse(pretText, out pret))
            {
                MessageBox.Show("Pretul trebuie sa fie un numar valid");
                return;
            }

            int stoc;
            if (!int.TryParse(stocText, out stoc))
            {
                MessageBox.Show("Stocul trebuie sa fie un numar valid");
                return;
            }

            // Adauga un 0 in fata ID-ului
            string formattedId = "0" + idAccesoriu;

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "INSERT INTO Produse.Accesorii (ID_Accesoriu, Nume_Accesoriu, Pret, Stoc, Tip_Accesoriu, Categorie_Instrument, Specificatii, Material, ID_Furnizor) " +
                           "VALUES (@ID, @Nume, @Pret, @Stoc, @Tip, @Categorie, @Specificatii, @Material, @Furnizor)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID", formattedId);
                    command.Parameters.AddWithValue("@Nume", nume);
                    command.Parameters.AddWithValue("@Pret", pret);
                    command.Parameters.AddWithValue("@Stoc", stoc);
                    command.Parameters.AddWithValue("@Tip", tip);
                    command.Parameters.AddWithValue("@Categorie", categorie);
                    command.Parameters.AddWithValue("@Specificatii", specificatii);
                    command.Parameters.AddWithValue("@Material", material);
                    command.Parameters.AddWithValue("@Furnizor", comboBoxIDFurnizor.SelectedValue != null ? comboBoxIDFurnizor.SelectedValue : DBNull.Value);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Accesoriul a fost adaugat");
                    ClearFormFields();
                    LoadAccesorii();
                }
                catch
                {
                    MessageBox.Show("Eroare la adaugarea accesoriului");
                }
            }
        }

        // Editeaza un accesoriu existent
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewAngajati.SelectedRows.Count > 0)
            {
                UpdateAccesoriu(dataGridViewAngajati.SelectedRows[0].Cells["ID_Accesoriu"].Value.ToString());
            }
            else
            {
                MessageBox.Show("Selectati un accesoriu pentru editare");
            }
        }

        // Actualizeaza un accesoriu in baza de date
        private void buttonUpdateAccesoriu_Click(object sender, EventArgs e)
        {
            string idAccesoriu = numericUpDownIDAccesoriu.Text;
            string nume = textBoxNume.Text;
            string pretText = textBoxPret.Text;
            string stocText = textBoxStoc.Text;
            string tip = textBoxTip.Text;
            string categorie = textBoxCategorie.Text;
            string specificatii = textBoxSpecificatii.Text;
            string material = textBoxMaterial.Text;

            if (string.IsNullOrEmpty(idAccesoriu))
            {
                MessageBox.Show("ID-ul accesoriului este obligatoriu pentru actualizare");
                return;
            }

            StringBuilder constructorInterogare = new StringBuilder("UPDATE Produse.Accesorii SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();
            bool areCampuri = false;

            if (!string.IsNullOrEmpty(nume))
            {
                constructorInterogare.Append("Nume_Accesoriu = @Nume, ");
                parametri.Add(new SqlParameter("@Nume", nume));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(pretText))
            {
                decimal pret;
                if (!decimal.TryParse(pretText, out pret))
                {
                    MessageBox.Show("Pretul trebuie sa fie un numar valid");
                    return;
                }
                constructorInterogare.Append("Pret = @Pret, ");
                parametri.Add(new SqlParameter("@Pret", pret));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(stocText))
            {
                int stoc;
                if (!int.TryParse(stocText, out stoc))
                {
                    MessageBox.Show("Stocul trebuie sa fie un numar valid");
                    return;
                }
                constructorInterogare.Append("Stoc = @Stoc, ");
                parametri.Add(new SqlParameter("@Stoc", stoc));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(tip))
            {
                constructorInterogare.Append("Tip_Accesoriu = @Tip, ");
                parametri.Add(new SqlParameter("@Tip", tip));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(categorie))
            {
                constructorInterogare.Append("Categorie_Instrument = @Categorie, ");
                parametri.Add(new SqlParameter("@Categorie", categorie));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(specificatii))
            {
                constructorInterogare.Append("Specificatii = @Specificatii, ");
                parametri.Add(new SqlParameter("@Specificatii", specificatii));
                areCampuri = true;
            }

            if (!string.IsNullOrEmpty(material))
            {
                constructorInterogare.Append("Material = @Material, ");
                parametri.Add(new SqlParameter("@Material", material));
                areCampuri = true;
            }

            if (comboBoxIDFurnizor.SelectedValue != null)
            {
                constructorInterogare.Append("ID_Furnizor = @Furnizor, ");
                parametri.Add(new SqlParameter("@Furnizor", comboBoxIDFurnizor.SelectedValue));
                areCampuri = true;
            }

            if (!areCampuri)
            {
                MessageBox.Show("Nu au fost introduse date pentru actualizare");
                return;
            }

            constructorInterogare.Length -= 2;
            constructorInterogare.Append(" WHERE ID_Accesoriu = @ID_Accesoriu");
            parametri.Add(new SqlParameter("@ID_Accesoriu", "0" + idAccesoriu));

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(constructorInterogare.ToString(), connection);
                    command.Parameters.AddRange(parametri.ToArray());
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Accesoriul a fost actualizat cu succes");
                        ClearFormFields();
                        LoadAccesorii();
                    }
                    else
                    {
                        MessageBox.Show("Accesoriul nu a fost gasit");
                    }
                }
                catch
                {
                    MessageBox.Show("Eroare la actualizarea accesoriului");
                }
            }
        }

        // Sterge un accesoriu din baza de date
        private void buttonDeleteAccesoriu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteIDAccesoriu.Text))
            {
                MessageBox.Show("Introduceti ID-ul accesoriului pentru a-l sterge");
                return;
            }

            string idAccesoriu = "0" + textBoxDeleteIDAccesoriu.Text;

            if (MessageBox.Show("Sunteti sigur ca doriti sa stergeti accesoriul cu ID-ul " + idAccesoriu + "?", "Confirmare stergere", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
                string query = "DELETE FROM Produse.Accesorii WHERE ID_Accesoriu = @ID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", idAccesoriu);
                        int rows = command.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Accesoriul a fost sters");
                            textBoxDeleteIDAccesoriu.Clear();
                            LoadAccesorii();
                        }
                        else
                        {
                            MessageBox.Show("Accesoriul nu a fost gasit");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Eroare la stergerea accesoriului");
                    }
                }
            }
        }

        // Eveniment pentru incarcarea formularului
        private void InsertAccesorii_Load(object sender, EventArgs e)
        {
        }

        // Eveniment pentru click pe celulele DataGridView
        private void dataGridViewAngajati_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        // Eveniment pentru desenarea panelului
        private void panel7_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}