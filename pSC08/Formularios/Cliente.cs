using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pSC08
{
    public partial class Cliente : Form
    {
        public Cliente()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnUsuario_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            if (frm.existeVar == true) 
            {
                txtRNC.Text = frm.IDcliente;
                txtNombre.Text = frm.Nombre;
                txtTelefono.Text = frm.Telefono.ToString();
                txtWhatsApp.Text = frm.Whatsappt.ToString();
                txtCorreo.Text = frm.Correo.ToString();
                comboBoxEstatus.Text = frm.Estatus.ToString();
                comboBoxPaga.Text = frm.PagaImpuesto.ToString();
            }
        }
    }
}
