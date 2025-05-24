using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studiu_Individual_1.Rapoarte.RapoarteForms
{
    public partial class RapAngForm : Form
    {
        public RapAngForm()
        {
            InitializeComponent();
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void RapAngForm_Load(object sender, EventArgs e)
        {
            this.angajatiTableAdapter.Fill(this.musicShopDataSet.Angajati);

        }

    }
}
