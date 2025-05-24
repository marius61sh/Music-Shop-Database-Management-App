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
    public partial class InsertAngajati : Form
    {
        public InsertAngajati()
        {
            InitializeComponent();
            LoadAngajati();
            SetPlaceholder();
        }

        // Incarca datele angajatilor in DataGridView
        private void LoadAngajati()
        {
            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "SELECT ID_Angajat, Prenume, Nume, Email, Telefon, Data_Angajare, Pozitie, Salariu FROM Administrare.Angajati";

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
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea datelor angajatilor: " + ex.Message);
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

        private void InsertAngajati_Load(object sender, EventArgs e)
        {
        }

        // Apelat la click pe butonul de adaugare
        private void buttonAdaugaAngajati_Click_1(object sender, EventArgs e)
        {
            AddEmployee();
        }

        // Adauga un angajat nou in baza de date
        private void AddEmployee()
        {
            string idAngajat = numericUpDownID_Angajat.Text;
            string prenume = textBoxPrenume.Text;
            string nume = textBoxNume.Text;
            string email = textBoxEmail.Text;
            string telefon = textBoxTelefon.Text;
            DateTime dataAngajare = dateTimePickerDataAngajare.Value;
            string pozitie = textBoxPozitie.Text;
            decimal salariu;

            if (string.IsNullOrEmpty(idAngajat) || string.IsNullOrEmpty(prenume) || string.IsNullOrEmpty(nume) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(telefon) || string.IsNullOrEmpty(pozitie) ||
                !decimal.TryParse(textBoxSalariu.Text, out salariu))
            {
                MessageBox.Show("Te rugam sa completezi toate campurile corect.");
                return;
            }

            if (telefon.Length != 9 || !telefon.All(char.IsDigit))
            {
                MessageBox.Show("Numarul de telefon trebuie sa contina exact 9 cifre.");
                return;
            }

            if (dataAngajare > DateTime.Now)
            {
                MessageBox.Show("Data angajarii nu poate fi in viitor.");
                return;
            }

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "INSERT INTO Administrare.Angajati (ID_Angajat, Prenume, Nume, Email, Telefon, Data_Angajare, Pozitie, Salariu) " +
                           "VALUES (@ID_Angajat, @Prenume, @Nume, @Email, @Telefon, @Data_Angajare, @Pozitie, @Salariu)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID_Angajat", idAngajat);
                    cmd.Parameters.AddWithValue("@Prenume", prenume);
                    cmd.Parameters.AddWithValue("@Nume", nume);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Telefon", telefon);
                    cmd.Parameters.AddWithValue("@Data_Angajare", dataAngajare);
                    cmd.Parameters.AddWithValue("@Pozitie", pozitie);
                    cmd.Parameters.AddWithValue("@Salariu", salariu);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Datele au fost adaugate cu succes!");
                    ClearFormFields();
                    LoadAngajati();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la adaugarea datelor: " + ex.Message);
                }
            }
        }

        private void textBoxID_Angajat_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxPrenume_TextChanged(object sender, EventArgs e)
        {

        }

        // Curata toate campurile formularului
        private void ClearFormFields()
        {
            textBoxPrenume.Clear();
            textBoxNume.Clear();
            textBoxEmail.Clear();
            textBoxTelefon.Clear();
            textBoxPozitie.Clear();
            textBoxSalariu.Clear();
            dateTimePickerDataAngajare.Value = DateTime.Now;
        }

        // Sterge un angajat din baza de date
        private void DeleteEmployee(int idAngajat)
        {
            if (MessageBox.Show($"Sunteti sigur ca doriti sa stergeti angajatul cu ID-ul {idAngajat}?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
                string query = "DELETE FROM Administrare.Angajati WHERE ID_Angajat = @ID_Angajat";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID_Angajat", idAngajat);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Angajatul cu ID-ul {idAngajat} a fost sters cu succes!");
                            textBoxDeleteID.Clear();
                            LoadAngajati();
                        }
                        else
                        {
                            MessageBox.Show($"Nu s-a gasit niciun angajat cu ID-ul {idAngajat}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare la stergerea datelor: " + ex.Message);
                    }
                }
            }
        }

        // Actualizeaza datele unui angajat existent
        private void buttonUpdate_Click_1(object sender, EventArgs e)
        {
            string idAngajat = numericUpDownID_Angajat.Text;
            string prenume = textBoxPrenume.Text;
            string nume = textBoxNume.Text;
            string email = textBoxEmail.Text;
            string telefon = textBoxTelefon.Text;
            DateTime dataAngajare = dateTimePickerDataAngajare.Value;
            string pozitie = textBoxPozitie.Text;
            string salariuText = textBoxSalariu.Text;

            if (string.IsNullOrEmpty(idAngajat))
            {
                MessageBox.Show("ID-ul angajatului este obligatoriu pentru actualizare.");
                return;
            }

            StringBuilder constructorInterogare = new StringBuilder("UPDATE Administrare.Angajati SET ");
            List<SqlParameter> parametri = new List<SqlParameter>();
            bool areCampuri = false;

            if (!string.IsNullOrEmpty(prenume))
            {
                constructorInterogare.Append("Prenume = @Prenume, ");
                parametri.Add(new SqlParameter("@Prenume", prenume));
                areCampuri = true;
            }
            if (!string.IsNullOrEmpty(nume))
            {
                constructorInterogare.Append("Nume = @Nume, ");
                parametri.Add(new SqlParameter("@Nume", nume));
                areCampuri = true;
            }
            if (!string.IsNullOrEmpty(email))
            {
                constructorInterogare.Append("Email = @Email, ");
                parametri.Add(new SqlParameter("@Email", email));
                areCampuri = true;
            }
            if (!string.IsNullOrEmpty(telefon))
            {
                constructorInterogare.Append("Telefon = @Telefon, ");
                parametri.Add(new SqlParameter("@Telefon", telefon));
                areCampuri = true;
            }
            constructorInterogare.Append("Data_Angajare = @Data_Angajare, ");
            parametri.Add(new SqlParameter("@Data_Angajare", dataAngajare));
            areCampuri = true;

            if (!string.IsNullOrEmpty(pozitie))
            {
                constructorInterogare.Append("Pozitie = @Pozitie, ");
                parametri.Add(new SqlParameter("@Pozitie", pozitie));
                areCampuri = true;
            }
            if (!string.IsNullOrEmpty(salariuText))
            {
                if (!decimal.TryParse(salariuText, out decimal salariu))
                {
                    MessageBox.Show("Salariul trebuie sa fie un numar valid.");
                    return;
                }
                constructorInterogare.Append("Salariu = @Salariu, ");
                parametri.Add(new SqlParameter("@Salariu", salariu));
                areCampuri = true;
            }

            constructorInterogare.Length -= 2;
            constructorInterogare.Append(" WHERE ID_Angajat = @ID_Angajat");
            parametri.Add(new SqlParameter("@ID_Angajat", idAngajat));

            string sirConexiune = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";

            using (SqlConnection conexiune = new SqlConnection(sirConexiune))
            {
                try
                {
                    conexiune.Open();
                    SqlCommand comanda = new SqlCommand(constructorInterogare.ToString(), conexiune);
                    comanda.Parameters.AddRange(parametri.ToArray());

                    int randuriAfectate = comanda.ExecuteNonQuery();

                    if (randuriAfectate > 0)
                    {
                        MessageBox.Show("Angajatul a fost actualizat cu succes!");
                        ClearFormFields();
                        numericUpDownID_Angajat.ReadOnly = false;
                        LoadAngajati();
                    }
                    else
                    {
                        MessageBox.Show("Nu s-a gasit angajatul cu ID-ul specificat.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la actualizarea datelor: " + ex.Message);
                }
            }
        }

        // Sterge angajatul la click pe buton
        private void buttonDelete_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDeleteID.Text))
            {
                MessageBox.Show("Va rugam sa introduceti ID-ul angajatului pentru a-l sterge.");
                return;
            }

            if (!int.TryParse(textBoxDeleteID.Text, out int idAngajat))
            {
                MessageBox.Show("ID-ul trebuie sa fie un numar valid.");
                return;
            }

            DeleteEmployee(idAngajat);
        }

        // Seteaza textul placeholder pentru ID-ul de stergere
        private void SetPlaceholder()
        {
            label9.Text = "Introduceti ID-ul";
            label9.ForeColor = Color.Gray;
            label9.Visible = string.IsNullOrEmpty(textBoxDeleteID.Text);
        }

        // Actualizeaza vizibilitatea placeholder-ului la schimbarea textului
        private void textBoxDeleteID_TextChanged(object sender, EventArgs e)
        {
            label9.Visible = string.IsNullOrEmpty(textBoxDeleteID.Text);
        }
    }
}