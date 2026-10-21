using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SistemaAcademico
{
    public partial class frmPeriodos : Form
    {
        private int idPeriodoSeleccionado = 0;

        public frmPeriodos()
        {
            InitializeComponent();
        }

        private void frmPeriodos_Load(object sender, EventArgs e)
        {
            CargarPeriodos();
        }

        // Carga la lista de periodos en la tabla
        private void CargarPeriodos()
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                string query = "SELECT idPeriodo, nombre AS [Periodo], fechaInicio AS [Inicio], fechaFin AS [Fin], estado AS [Activo] FROM periodos";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPeriodos.DataSource = dt;

                if (dgvPeriodos.Columns["idPeriodo"] != null)
                {
                    dgvPeriodos.Columns["idPeriodo"].Visible = false;
                }
            }
        }

        // Guardar o Actualizar Periodo
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre del periodo académico.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                cn.Open();
                SqlCommand cmd;

                if (idPeriodoSeleccionado == 0)
                {
                    string query = "INSERT INTO periodos (nombre, fechaInicio, fechaFin, estado) VALUES (@nombre, @inicio, @fin, @estado)";
                    cmd = new SqlCommand(query, cn);
                }
                else
                {
                    string query = "UPDATE periodos SET nombre = @nombre, fechaInicio = @inicio, fechaFin = @fin, estado = @estado WHERE idPeriodo = @id";
                    cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", idPeriodoSeleccionado);
                }

                cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@inicio", dtpInicio.Value.Date);
                cmd.Parameters.AddWithValue("@fin", dtpFin.Value.Date);
                cmd.Parameters.AddWithValue("@estado", chkEstado.Checked);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show(idPeriodoSeleccionado == 0 ? "Periodo registrado correctamente." : "Periodo actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
            CargarPeriodos();
        }

        // Cargar fila seleccionada en los controles
        private void dgvPeriodos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CargarFilaSeleccionada(e.RowIndex);
        }

        private void dgvPeriodos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CargarFilaSeleccionada(e.RowIndex);
        }

        private void CargarFilaSeleccionada(int rowIndex)
        {
            if (rowIndex >= 0 && dgvPeriodos.CurrentRow != null)
            {
                idPeriodoSeleccionado = Convert.ToInt32(dgvPeriodos.Rows[rowIndex].Cells["idPeriodo"].Value);
                txtNombre.Text = dgvPeriodos.Rows[rowIndex].Cells["Periodo"].Value.ToString();
                dtpInicio.Value = Convert.ToDateTime(dgvPeriodos.Rows[rowIndex].Cells["Inicio"].Value);
                dtpFin.Value = Convert.ToDateTime(dgvPeriodos.Rows[rowIndex].Cells["Fin"].Value);
                chkEstado.Checked = Convert.ToBoolean(dgvPeriodos.Rows[rowIndex].Cells["Activo"].Value);

                btnGuardar.Text = "Actualizar";
            }
        }

        // Eliminar Periodo
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idPeriodoSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un periodo de la lista para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este periodo?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    string query = "DELETE FROM periodos WHERE idPeriodo = @id";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", idPeriodoSeleccionado);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Periodo eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarPeriodos();
            }
        }

        // Limpiar controles
        private void LimpiarCampos()
        {
            idPeriodoSeleccionado = 0;
            txtNombre.Clear();
            dtpInicio.Value = DateTime.Now;
            dtpFin.Value = DateTime.Now.AddMonths(5);
            chkEstado.Checked = true;
            btnGuardar.Text = "Guardar";
            txtNombre.Focus();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Método vacío para resolver la referencia pendiente del diseñador
        }
    }
}