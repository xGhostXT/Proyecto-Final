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
    public partial class frmFactura : Form
    {
        Boolean ExisteLaData;

        double zImpuesto;
        double zTotal;
        double zSubtotal;
        double lnImpuesto;
        double tnImpuesto;

        public frmFactura()
        {
            InitializeComponent();
            EstiloDataGridView();
        }

        private void frmFactura_Load(object sender, EventArgs e)
        {
            this.Text = "Factura";
            this.KeyPreview = true;

            lblFechaFactura.Text = DateTime.Now.ToString("ddMMyyyy");

            ExisteLaData = false;

            lblFactura.Text = Busco.BuscaUltimoNumero("1");  // AQUI TRAE EL ULTIMO NUMERO DE FACTURA
        }

        private void frmFactura_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)  // pregunta si presionaste la tecla de ESC
            {
                this.Close();  // cierra el formulario
            }
        }

        #region -->> BOTONES <<--
        // -----------------------------------------------------------------
        // BOTONES
        // -----------------------------------------------------------------
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();

            lblFactura.Text = Busco.BuscaUltimoNumero("1");
            txtCliente.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (lblTotal.Text.Trim() != string.Empty)  
            {
                BorrarData(lblFactura.Text);
                InsertarData();

                LimpiarFormulario();
                txtCliente.Focus();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCONFACT_Click(object sender, EventArgs e)
        {
            frmFACTCTE frm = new frmFACTCTE();
            frm.ShowDialog();

            if (frm.existeVar == true)
            {
                frm.existeVar = false;
                BuscarFactura(frm.var2);
            }
        }

        private void btnVENCTE_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();  // creamos un objeto llamado frm y le asignamos un forms
            DialogResult res = frm.ShowDialog();

            if (frm.existeVar == true)
            {
                txtCliente.Text = frm.var1;
                lblNombre.Text = frm.var2;
                lblPagaImpuesto.Text = frm.var3;
            }
        }

        private void btnInsertarLn_Click(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty)
            {
                if (txtCantidad.Text.Trim() != string.Empty)
                {
                    InsertaLinea();
                    TotalizarFactura();
                    LimpiarDetalle();
                }
            }
        }
        #endregion

        #region --> TextBox <<--
        // -----------------------------------------------------------------
        // TEXTBOX
        // -----------------------------------------------------------------

        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)  
            {
                e.Handled = true;
                if (txtCliente.Text.Trim() != string.Empty)  
                {
                    txtArticulo.Focus(); 
                }
            }
        }

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)   
            {
                btnVENCTE.PerformClick();
            }
        }

        private void txtCliente_Leave(object sender, EventArgs e)
        {
            if (txtCliente.Text.Trim() != string.Empty)  
            {
                BuscarCliente(txtCliente.Text);
            }
        }

        private void txtArticulo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtArticulo.Text.Trim() != string.Empty)
                {
                    txtCantidad.Focus();
                }
            }
        }

        private void txtArticulo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnArticulo.PerformClick();
            }
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty)
            {
                BuscarArticulo(txtArticulo.Text);
            }
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCantidad.Text.Trim() != string.Empty)
                {
                    btnInsertarLn.Focus();
                }
            }
        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            double nmCant = Convert.ToDouble(txtCantidad.Text);
            double nmPrec = Convert.ToDouble(lblPrecio.Text);

            if (nmCant > 0 )
            {
                if (nmPrec > 0)
                {
                    double price = Convert.ToDouble(lblPrecio.Text);
                    double quantity = Convert.ToDouble(txtCantidad.Text);
                    double total = price * quantity;   // total sin impuesto

                    if (lblPagaImpuesto.Text == "1")   // calculara el impuesto     100 * 0.18 == 118
                        lblImpuestoLn.Text = (total * lnImpuesto).ToString();

                    if (lblPagaImpuesto.Text == "0")   // no calculara el impuesto
                        lblImpuestoLn.Text = "0";

                    lblTotalLn.Text = total.ToString();  // total sin impuesto
                }
            }
        }
        #endregion

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            BorrarData(lblFactura.Text);  // borra factura completa
        }

        private void btnBorrarLn_Click(object sender, EventArgs e)
        {
            BorrarLineaDelDataGridView();  // borra una linea de la grilla
        }

        #region -->> METODOS <<--
        // -----------------------------------------------------------------
        // METODOS
        // -----------------------------------------------------------------

        private void BorrarLineaDelDataGridView()
        {
            int CuantasLineasTengo = Convert.ToInt32(dgv.RowCount);

            if (CuantasLineasTengo == 1)
            {
                dgv.Rows.RemoveAt(dgv.RowCount - 1);
            }
            else
            {
                dgv.Rows.Remove(dgv.CurrentRow);
            }

            txtArticulo.Focus();
        }

        private void BuscarFactura(string nmrFactura)
        {
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string stQuery = "SELECT A.FACTURA " +
                             "     , A.CLIENTE " +
                             "     , B.NOMBRE " +
                             "     , A.FECHA " +
                             "     , A.SUBTOTAL " +
                             "     , A.IMPUESTO " +
                             "     , A.MONTOFACTURADO" +
                             "     , A.IMPUESTO " +
                             "     , A.ACTIVO " +
                             "  FROM HFACTURA A INNER JOIN CLIENTES B ON A.CLIENTE = B.IDCLIENTE " +
                             " WHERE A.FACTURA ='" + nmrFactura + "'";

            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                lblFactura.Text = Convert.ToString(rdr["FACTURA"]);
                txtCliente.Text = Convert.ToString(rdr["CLIENTE"]);
                lblNombre.Text = Convert.ToString(rdr["NOMBRE"]);
                lblImpuesto.Text = Convert.ToString(rdr["IMPUESTO"]);
                lblFechaFactura.Text = Convert.ToString(rdr["FECHA"]);

                lblSubtotal.Text = Convert.ToString(rdr["SUBTOTAL"]);
                lblImpuesto.Text = Convert.ToString(rdr["IMPUESTO"]);
                lblTotal.Text = Convert.ToString(rdr["TOTAL"]);

                BuscaDetalle(nmrFactura);   // busca el detalle de la factura
            }

            cmd.Dispose();
            cnx.Close();
        }

        private void BuscaDetalle(string nmrFactura)
        {
            string stQuery = "SELECT A.FACTURA " +
                             "     , A.CLIENTE " +
                             "     , A.FECHA " +
                             "     , A.ARTICULO " +
                             "     , B.DESCRIPCION " +
                             "     , A.CANTIDAD " +
                             "     , A.PRECIODEVENTA " +
                             "     , A.IMPUESTO" +
                             "     , A.MONTOPORLINEA " +
                             "  FROM DFACTURA A INNER JOIN PRODUCTOS B ON A.ARTICULO = B.ITEM " +
                             " WHERE A.FACTURA ='" + nmrFactura + "'";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgv.Rows.Add();   // le suma uno al contador del datagridview
                int xRows = dgv.Rows.Count - 1;

                dgv[0, xRows].Value = Convert.ToString(rdr["ARTICULO"]);
                dgv[1, xRows].Value = Convert.ToString(rdr["DESCRIPCION"]);
                dgv[2, xRows].Value = Convert.ToString(rdr["CANTIDAD"]);
                dgv[3, xRows].Value = Convert.ToString(rdr["PRECIODEVENTA"]);
                dgv[4, xRows].Value = Convert.ToString(rdr["IMPUESTO"]);
                dgv[5, xRows].Value = Convert.ToString(rdr["MONTOPORLINEA"]);
            }

            cmd.Dispose();
            cnx.Close();
        }

        private void BuscarArticulo(string nmrArticulo)
        {
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string tsQuery = "SELECT ITEM, " +
                             "       DESCRIPCION, " +
                             "       PRECIODEVENTA, " +
                             "       IMPUESTO, " +
                             "  FROM PRODUCTOS " +
                             " WHERE ITEM ='" + nmrArticulo + "'";
            SqlCommand cmd = new SqlCommand(tsQuery, cnx);
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                lblArticulo.Text = Convert.ToString(rdr["DESCRIPCION"]);
                lblPrecio.Text = Convert.ToString(rdr["PRECIODEVENTA"]);
                lnImpuesto = Convert.ToDouble(rdr["IMPUESTO"]);
            }

            cmd.Dispose();
            cnx.Close();

        }

        private void BuscarCliente(string nmrCliente)
        {
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string tsQuery = "SELECT NOMBRE, PAGAIMPUESTO FROM CLIENTES WHERE  IDCLIENTE = " + nmrCliente;
            SqlCommand cmd = new SqlCommand(tsQuery, cnx);
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                lblNombre.Text = Convert.ToString(rdr["NOMBRE"]);
                lblPagaImpuesto.Text = Convert.ToString(rdr["PAGAIMPUESTO"]);
            }

            cmd.Dispose();
            cnx.Close();
        }

        private void TotalizarFactura()
        {
            try
            {
                zImpuesto = 0;
                zSubtotal = 0;
                zTotal = 0;

                lblSubtotal.Text = "";
                lblImpuesto.Text = "";
                lblTotal.Text = "";

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    Double nImpuesto = Convert.ToDouble(dgv.CurrentRow.Cells[4].Value.ToString());
                    Double nSubtotal = Convert.ToDouble(dgv.CurrentRow.Cells[5].Value.ToString());
                    Double nTotal = nSubtotal + nImpuesto;

                    zImpuesto = zImpuesto + nImpuesto;
                    zSubtotal = zSubtotal + nSubtotal;
                    zTotal = zTotal + nTotal;
                }

                lblSubtotal.Text = Convert.ToString(zSubtotal);
                lblImpuesto.Text = Convert.ToString(zImpuesto);
                lblTotal.Text = Convert.ToString(zTotal);
            }
            catch
            {
                //
            }
        }

        private void EstiloDataGridView()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = false;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("Col00", "");
            this.dgv.Columns.Add("Col01", "");
            this.dgv.Columns.Add("Col02", "");
            this.dgv.Columns.Add("Col03", "");
            this.dgv.Columns.Add("Col04", "");
            this.dgv.Columns.Add("Col05", "");

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 100;
            column = dgv.Columns[01]; column.Width = 420;
            column = dgv.Columns[02]; column.Width = 100;
            column = dgv.Columns[03]; column.Width = 100;
            column = dgv.Columns[04]; column.Width = 100;
            column = dgv.Columns[05]; column.Width = 100;

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

        private void InsertarData()
        {
            // ------------------------------------------------------------
            // INSERTA DATA
            // ------------------------------------------------------------



        }

        private void BorrarData(string numFactura)
        {
            if (ExisteLaData == true)
            {
                SqlConnection cns = new SqlConnection(cnn.db); cns.Open();
                string ssQuery = "DELETE FROM HFACTURA WHERE FACTURA ='" + numFactura + "'";
                SqlCommand cms = new SqlCommand(ssQuery, cns);
                cms.ExecuteNonQuery();

                SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
                string tsQuery = "DELETE FROM DFACTURA WHERE FACTURA ='" + numFactura + "'";
                SqlCommand cmd = new SqlCommand(tsQuery, cnx);
                cmd.ExecuteNonQuery();
            }
        }

        private void LimpiarDetalle()
        {
            txtArticulo.Clear();
            lblArticulo.Text = "";
            txtCantidad.Clear();
            lblImpuestoLn.Text = "";
            lblTotalLn.Text = "";
            lblPrecio.Text = "";
        }

        private void LimpiarFormulario()
        {
            ExisteLaData = false;

            this.dgv.Rows.Clear();  // limpia el datagridview
            this.dgv.Refresh();     // refresca y le devuelve las especificaciones anteriores

            LimpiarDetalle();

        }

        private void InsertaLinea()
        {
            dgv.Rows.Add();
            int xRows = dgv.Rows.Count - 1;

            dgv[0, xRows].Value = txtArticulo.Text;
            dgv[1, xRows].Value = lblArticulo.Text;
            dgv[2, xRows].Value = txtCantidad.Text;
            dgv[3, xRows].Value = lblPrecio.Text;
            dgv[4, xRows].Value = lblImpuestoLn.Text;
            dgv[5, xRows].Value = lblTotalLn.Text;

        }

        #endregion

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                txtArticulo.Text = dgv.CurrentRow.Cells[0].Value.ToString();  
                lblArticulo.Text = dgv.CurrentRow.Cells[1].Value.ToString();  
                txtCantidad.Text = dgv.CurrentRow.Cells[2].Value.ToString();  
                lblPrecio.Text = dgv.CurrentRow.Cells[3].Value.ToString();
                lblImpuesto.Text = dgv.CurrentRow.Cells[4].Value.ToString();
                lblTotalLn.Text = dgv.CurrentRow.Cells[5].Value.ToString();
            }
        }
    }
}
