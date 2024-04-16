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
    public partial class frmVENDEPTO : Form
    {
        // ----------------------------------
        // las variables declarada aqui se pueden usar en cualquier evento
        public string var1;
        public string var2;
        public Boolean seleccionedata;

        public frmVENDEPTO()
        {
            InitializeComponent();
            EstiloDataGridView();  // definimos la grilla
        }

        private void frmVENDEPTO_Load(object sender, EventArgs e)
        {
            this.Text = "Consulta Departamento";
            this.KeyPreview = true;

            
            seleccionedata = false;
        }

        private void frmVENDEPTO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) // pregunta si es la tecla de escape
            {
                this.Close();  // cierra el form
            }
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)  // aqui pregunta si la tecla que presionaste es igual ENTER
            {
                e.Handled = true;
                btnAceptar.PerformClick();  // aqui va a ejecutar el evento del boton aceptar
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            BuscarData();  // metodo para buscar la data en la tabla
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BuscarData()
        {
            seleccionedata = false;

            this.dgv.Rows.Clear();  // limpia el datagridview
            this.dgv.Refresh();     // refresca y le devuelve las especificaciones anteriores

            SqlConnection cnx = new SqlConnection(cnn.db);  
            cnx.Open();

            string stQuery = "     SELECT IDDEPARTAMENTO, NOMBREDEPARTAMENTO" +
                             "       FROM DEPARTAMENTO " +
                             "      WHERE NOMBREDEPARTAMENTO LIKE '%" + txtBuscar.Text + 
                             "%' ORDER BY NOMBREDEPARTAMENTO ASC";
            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            SqlDataReader rcd = cmd.ExecuteReader();

            while (rcd.Read())
            {
                dgv.Rows.Add();   // le suma uno al contador del datagridview
                int xRows = dgv.Rows.Count - 1;

                dgv[0, xRows].Value = Convert.ToString(rcd["IDDEPARTAMENTO"]);
                dgv[1, xRows].Value = Convert.ToString(rcd["NOMBREDEPARTAMENTO"]);
            }
        }

        private void EstiloDataGridView()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("Col00", "Departamento");
            this.dgv.Columns.Add("Col01", "Nombre Departameto");

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 100;
            column = dgv.Columns[01]; column.Width = 590;

            this.dgv.BorderStyle = BorderStyle.None;
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgv.BackgroundColor = Color.LightGray;

            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSelecciona.PerformClick();
        }

        private void btnSelecciona_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)  // pregunta si el contador de lineas es mayor de cero
            {
                var1 = dgv.CurrentRow.Cells[0].Value.ToString();  // DEPARTAMETO
                var2 = dgv.CurrentRow.Cells[1].Value.ToString();  // NOMBRE DEPARATAMENTO

                seleccionedata = true;
                this.Close();
            }
        }
    }
}
