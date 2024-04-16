using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace pSC08
{
    public partial class frmVENPROD : Form
    {
        public frmVENPROD()
        {
            InitializeComponent();
        }

        private void frmVENPROD_Load(object sender, EventArgs e)
        {
            this.Text = "Consulta de Productos";
            this.KeyPreview = true;
        }

        private void frmVENPROD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

        }

        private void btnSelecciona_Click(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
