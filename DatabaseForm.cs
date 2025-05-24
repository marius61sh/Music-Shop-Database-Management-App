using Studiu_Individual_1.Rapoarte.RapoarteForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Studiu_Individual_1
{
    public partial class DatabaseForm : Form
    {
        private string currentUsername;
        private string currentUserRole;
        private Timer animationTimer;
        private int targetHeight = 620; // Inaltimea dropdown-ului
        private int animationSpeed = 20; // Viteza animatiei dropdown-ului
        private bool isDropdownOpen = false; // Starea dropdown-ului (deschis/inchis)
        private string currentSelectedTable = string.Empty; // Pastreaza tabelul selectat curent

        public DatabaseForm()
        {
            InitializeComponent();
            panel2.Visible = false;
            InitializeDropdown();
        }

        private void LoadDashboardData()
        {
            if (!IsAdmin()) // Verifica permisiunea doar pentru admin
            {
                groupBoxDashboard.Visible = false;
                btnDashboard.Enabled = false;
                btnDashboard.Visible = false; // Ascunde butonul si dezactiveaza-l pentru non-admin
                return;
            }

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string interogareProduse = "SELECT COUNT(*) FROM Produse.Produse";
                    int nrProduse = (int)new SqlCommand(interogareProduse, conn).ExecuteScalar();
                    lblProduse.Text = $"Produse: {nrProduse}";

                    string interogareAngajat = "SELECT COUNT(*) FROM Administrare.Angajati";
                    int nrAngajati = (int)new SqlCommand(interogareAngajat, conn).ExecuteScalar();
                    lblAngajati.Text = $"Angajati: {nrAngajati}";

                    string interogareComenzi = "SELECT COUNT(*) FROM Vanzari.Comenzi";
                    int nrComenzi = (int)new SqlCommand(interogareComenzi, conn).ExecuteScalar();
                    lblComenzi.Text = $"Comenzi: {nrProduse}";

                    string interogareVenit = "SELECT SUM(Suma) FROM Vanzari.Plati";
                    decimal venit = 0;
                    object sumResult = new SqlCommand(interogareVenit, conn).ExecuteScalar();
                    if (sumResult != DBNull.Value)
                    {
                        venit = Convert.ToDecimal(sumResult);
                    }
                    lblVenit.Text = $"Venit: {venit:C}";

                    string interogareFurnizori = "SELECT COUNT(*) FROM Produse.Furnizori";
                    int nrFurnizori = (int)new SqlCommand(interogareComenzi, conn).ExecuteScalar();
                    lblFurnizori.Text = $"Furnizori: {nrFurnizori}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea datelor dashboard-ului: " + ex.Message);
                    lblProduse.Text = "Produse: Eroare!";
                    lblAngajati.Text = "Angajati: Eroare!";
                    lblComenzi.Text = "Comenzi: Eroare!";
                    lblFurnizori.Text = "Furnizori: Eroare!";
                }
            }
        }

        private void InitializeDropdown()
        {
            panel2.Height = 0;
            panel2.Visible = true;

            animationTimer = new Timer();
            animationTimer.Interval = 15;
            animationTimer.Tick += AnimateDropdown;
        }

        private void DatabaseForm_Load(object sender, EventArgs e)
        {
            currentUsername = Properties.Settings.Default.Username;

            CustomizeDataGridView();

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";
            string query = "SELECT Username, Role FROM Users WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Username", currentUsername);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string userName = reader["Username"].ToString();
                        currentUserRole = reader["Role"].ToString();

                        label1.Text = $"Bun venit, {userName}!";
                        label2.Text = $"Rol: {currentUserRole}";
                    }
                    else
                    {
                        MessageBox.Show("Utilizatorul nu a fost gasit in baza de date.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la conectarea la baza de date: " + ex.Message);
                }
            }
            RestrictButtons();
            ShowAdminButton(); // Verifica si afiseaza butonul pentru admin
            RestrictDashboardAccess(); // Restrictioneaza accesul la dashboard
            LoadDashboardData();
        }

        // Verifica dacă utilizatorul este admin (db_owner)
        private bool IsAdmin()
        {
            return currentUserRole == "db_owner";
        }

        // Restrictioneaza accesul la dashboard pentru non-admin
        private void RestrictDashboardAccess()
        {
            if (!IsAdmin())
            {
                groupBoxDashboard.Visible = false;
                btnDashboard.Enabled = false;
                btnDashboard.Visible = false; 
                pictureBox2.Visible = false;
            }
            else
            {
                groupBoxDashboard.Visible = true;
                btnDashboard.Enabled = true;
                btnDashboard.Visible = true;
                pictureBox2.Visible = true;

            }
        }

        // Verifica si afiseaza butonul de adaugare utilizatori pentru admin
        private void ShowAdminButton()
        {
            buttonAddUser.Visible = IsAdmin(); 
        }

        private void buttonDelogare_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserRole = "";
            Properties.Settings.Default.Save();

            this.Hide();
            Form1 loginForm = new Form1();
            loginForm.ShowDialog();
            this.Close();
        }

        private void MoveNavIndicator(Button btn)
        {
            panelNav.Height = btn.Height;
            panelNav.Visible = true;

            if (btn.Parent == panel2)
            {
                panelNav.Top = panel2.Top + btn.Top;
                panelNav.Left = panel2.Left + btn.Left - 5;
            }
            else
            {
                panelNav.Top = btn.Top;
                panelNav.Left = btn.Left - 5;
            }

            ResetButtonColors();
            btn.BackColor = Color.FromArgb(44, 53, 68);
        }

        private void ResetButtonColors()
        {
            button1.BackColor = Color.FromArgb(22, 33, 50);
            button2.BackColor = Color.FromArgb(22, 33, 50);
            button3.BackColor = Color.FromArgb(22, 33, 50);
            button4.BackColor = Color.FromArgb(22, 33, 50);
            button5.BackColor = Color.FromArgb(22, 33, 50);
            button6.BackColor = Color.FromArgb(22, 33, 50);
            button7.BackColor = Color.FromArgb(22, 33, 50);
            button8.BackColor = Color.FromArgb(22, 33, 50);
            button9.BackColor = Color.FromArgb(22, 33, 50);
            button10.BackColor = Color.FromArgb(22, 33, 50);
            button11.BackColor = Color.FromArgb(22, 33, 50);
            button12.BackColor = Color.FromArgb(22, 33, 50);
            button13.BackColor = Color.FromArgb(22, 33, 50);
            buttonAddUser.BackColor = Color.FromArgb(22, 33, 50);

        }

        private void LoadData(string tableName)
        {
            if (!UserHasPermission(tableName))
            {
                MessageBox.Show("Nu aveti permisiunea de a accesa acest tabel!", "Acces interzis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Data Source=DESKTOP-QUJ6APN\\SQLEXPRESS;Initial Catalog=MusicShop;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT * FROM {tableName}";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView2.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la incarcarea datelor: " + ex.Message);
                }
            }
        }

        private bool UserHasPermission(string tableName)
        {
            if (currentUserRole == "db_owner")
            {
                return true;
            }
            if (currentUserRole == "ManagerVanzari")
            {
                return tableName == "Vanzari.Comenzi" || tableName == "Vanzari.Detalii_Comenzi" || tableName == "Vanzari.Produse";
            }
            else if (currentUserRole == "ManagerStocuri")
            {
                return tableName == "Produse.Produse";
            }
            else if (currentUserRole == "Contabil")
            {
                return tableName == "Vanzari.Plati";
            }
            else if (currentUserRole == "UtilizatorVizualizare")
            {
                return tableName == "Produse.Produse" || tableName == "Vanzari.Comenzi" || tableName == "Vanzari.Clienti";
            }
            else if (currentUserRole == "ManagerClienti")
            {
                return tableName == "Vanzari.Clienti";
            }
            return false;
        }

        private void RestrictButtons()
        {
            if (currentUserRole == "db_owner")
            {
                foreach (Control control in this.Controls)
                {
                    if (control is Button btn)
                    {
                        btn.Enabled = true;
                    }
                }
                return;
            }

            button1.Enabled = UserHasPermission("Vanzari.Clienti");
            button2.Enabled = UserHasPermission("Produse.Accesorii");
            button3.Enabled = UserHasPermission("Produse.Chitare");
            button4.Enabled = UserHasPermission("Produse.Produse");
            button5.Enabled = UserHasPermission("Vanzari.Detalii_Comenzi");
            button6.Enabled = UserHasPermission("Administrare.Angajati");
            button7.Enabled = UserHasPermission("Produse.Furnizori");
            button8.Enabled = UserHasPermission("Produse.InstrumenteSuflat");
            button9.Enabled = UserHasPermission("Produse.Piane");
            button10.Enabled = UserHasPermission("Produse.Tobe");
            button11.Enabled = UserHasPermission("Produse.Viori");
            button12.Enabled = UserHasPermission("Vanzari.Comenzi");
            button13.Enabled = UserHasPermission("Vanzari.Plati");
        }

        private void CustomizeDataGridView()
        {
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.Font = new Font("Arial", 10);
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView2.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView2.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView2.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            dataGridView2.RowHeadersWidth = 40;
            dataGridView2.RowHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            dataGridView2.RowHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button1);
            LoadData("Vanzari.Clienti");
            currentSelectedTable = "Vanzari.Clienti";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button2);
            LoadData("Produse.Accesorii");
            currentSelectedTable = "Produse.Accesorii";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button3);
            LoadData("Produse.Chitare");
            currentSelectedTable = "Produse.Chitare";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button4);
            LoadData("Produse.Produse");
            currentSelectedTable = "Produse.Produse";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button5);
            LoadData("Vanzari.Detalii_Comenzi");
            currentSelectedTable = "Vanzari.Detalii_Comenzi";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button6);
            LoadData("Administrare.Angajati");
            currentSelectedTable = "Administrare.Angajati";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button7);
            LoadData("Produse.Furnizori");
            currentSelectedTable = "Produse.Furnizori";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button8);
            LoadData("Produse.InstrumenteSuflat");
            currentSelectedTable = "Produse.InstrumenteSuflat";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button9);
            LoadData("Produse.Piane");
            currentSelectedTable = "Produse.Piane";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button10);
            LoadData("Produse.Tobe");
            currentSelectedTable = "Produse.Tobe";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button11);
            LoadData("Produse.Viori");
            currentSelectedTable = "Produse.Viori";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button12);
            LoadData("Vanzari.Comenzi");
            currentSelectedTable = "Vanzari.Comenzi";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(button13);
            LoadData("Vanzari.Plati");
            currentSelectedTable = "Vanzari.Plati";
            groupBoxDashboard.Visible = false;
            dataGridView2.Visible = true;
        }

        private void panelNav_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonTabele_Click(object sender, EventArgs e)
        {
            MoveNavIndicator(buttonTabele);
            isDropdownOpen = !isDropdownOpen;
            animationTimer.Start();
        }

        private void AnimateDropdown(object sender, EventArgs e)
        {
            if (isDropdownOpen)
            {
                if (panel2.Height < targetHeight)
                {
                    panel2.Height += animationSpeed;
                }
                else
                {
                    animationTimer.Stop();
                }
            }
            else
            {
                if (panel2.Height > 0)
                {
                    panel2.Height -= animationSpeed;
                }
                else
                {
                    animationTimer.Stop();
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Nu aveti permisiunea de a accesa dashboard-ul!", "Acces interzis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            groupBoxDashboard.Visible = true;
            dataGridView2.Visible = false;
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            switch (currentSelectedTable)
            {
                case "Vanzari.Clienti":
                    InsertClienti insertClientiForm = new InsertClienti();
                    insertClientiForm.ShowDialog();
                    break;
                case "Produse.Accesorii":
                    InsertAccesorii insertAccesoriiForm = new InsertAccesorii();
                    insertAccesoriiForm.ShowDialog();
                    break;
                case "Produse.Chitare":
                    InsertChitare insertChitareForm = new InsertChitare();
                    insertChitareForm.ShowDialog();
                    break;
                case "Produse.Produse":
                    InsertProduse insertProduseForm = new InsertProduse();
                    insertProduseForm.ShowDialog();
                    break;
                case "Vanzari.Detalii_Comenzi":
                    InsertDetalii_Comenzi insertDetalii_ComenziForm = new InsertDetalii_Comenzi();
                    insertDetalii_ComenziForm.ShowDialog();
                    break;
                case "Administrare.Angajati":
                    InsertAngajati insertAngajatiForm = new InsertAngajati();
                    insertAngajatiForm.ShowDialog();
                    break;
                case "Produse.Furnizori":
                    InsertFurnizori insertFurnizoriForm = new InsertFurnizori();
                    insertFurnizoriForm.ShowDialog();
                    break;
                case "Produse.InstrumenteSuflat":
                    InsertInstrumenteSuflat insertInstrumenteSuflatForm = new InsertInstrumenteSuflat();
                    insertInstrumenteSuflatForm.ShowDialog();
                    break;
                case "Produse.Piane":
                    InsertPiane insertPianeForm = new InsertPiane();
                    insertPianeForm.ShowDialog();
                    break;
                case "Produse.Tobe":
                    InsertTobe insertTobeForm = new InsertTobe();
                    insertTobeForm.ShowDialog();
                    break;
                case "Produse.Viori":
                    InsertViori insertVioriForm = new InsertViori();
                    insertVioriForm.ShowDialog();
                    break;
                case "Vanzari.Comenzi":
                    InsertComenzi insertComenziForm = new InsertComenzi();
                    insertComenziForm.ShowDialog();
                    break;
                case "Vanzari.Plati":
                    InsertPlati insertPlatiForm = new InsertPlati();
                    insertPlatiForm.ShowDialog();
                    break;
                default:
                    MessageBox.Show("Nu exista actiuni definite pentru tabelul selectat.");
                    break;
            }
        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {

        }

        private void lblVenit_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox22_Click(object sender, EventArgs e)
        {

        }

        private void lblFurnizori_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            InsertUsers insertUsersForm = new InsertUsers();
            insertUsersForm.ShowDialog();
            MoveNavIndicator(buttonAddUser);

        }

        private void lblAngajati_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            RapAngForm RapAng = new RapAngForm();
            RapAng.ShowDialog();

        }
    }
}