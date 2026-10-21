using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SistemaAcademico
{
    public partial class frmNotas : Form
    {
        private int idNotaSeleccionada = 0;

        public frmNotas()
        {
            InitializeComponent();
        }

        private void frmNotas_Load(object sender, EventArgs e)
        {
            CargarCombos();
            CargarNotas();
        }

        private void CargarCombos()
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                cn.Open();

                // Cargar Alumnos
                SqlDataAdapter daA = new SqlDataAdapter("SELECT idAlumno, nombre + ' ' + apellido AS NombreCompleto FROM alumnos", cn);
                DataTable dtA = new DataTable();
                daA.Fill(dtA);
                cmbAlumno.DataSource = dtA;
                cmbAlumno.DisplayMember = "NombreCompleto";
                cmbAlumno.ValueMember = "idAlumno";

                // Cargar Materias
                SqlDataAdapter daM = new SqlDataAdapter("SELECT idMateria, nombre FROM materias", cn);
                DataTable dtM = new DataTable();
                daM.Fill(dtM);
                cmbMateria.DataSource = dtM;
                cmbMateria.DisplayMember = "nombre";
                cmbMateria.ValueMember = "idMateria";

                // Cargar Periodos Activos
                SqlDataAdapter daP = new SqlDataAdapter("SELECT idPeriodo, nombre FROM periodos WHERE estado = 1", cn);
                DataTable dtP = new DataTable();
                daP.Fill(dtP);
                cmbPeriodo.DataSource = dtP;
                cmbPeriodo.DisplayMember = "nombre";
                cmbPeriodo.ValueMember = "idPeriodo";
            }
        }

        private void CargarNotas()
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                string query = @"SELECT n.idNota, 
                                        a.nombre + ' ' + a.apellido AS [Alumno], 
                                        m.nombre AS [Materia], 
                                        p.nombre AS [Periodo], 
                                        n.nota1 AS [Nota 1], 
                                        n.nota2 AS [Nota 2], 
                                        n.nota3 AS [Nota 3], 
                                        ROUND((n.nota1 + n.nota2 + n.nota3)/3.0, 2) AS [Promedio]
                                 FROM notas n
                                 INNER JOIN alumnos a ON n.idAlumno = a.idAlumno
                                 INNER JOIN materias m ON n.idMateria = m.idMateria
                                 INNER JOIN periodos p ON n.idPeriodo = p.idPeriodo";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvNotas.DataSource = dt;

                if (dgvNotas.Columns["idNota"] != null)
                {
                    dgvNotas.Columns["idNota"].Visible = false;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbAlumno.SelectedValue == null || cmbMateria.SelectedValue == null || cmbPeriodo.SelectedValue == null)
            {
                MessageBox.Show("Seleccione los datos requeridos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                cn.Open();
                SqlCommand cmd;

                if (idNotaSeleccionada == 0)
                {
                    string query = "INSERT INTO notas (idAlumno, idMateria, idPeriodo, nota1, nota2, nota3) VALUES (@alumno, @materia, @periodo, @n1, @n2, @n3)";
                    cmd = new SqlCommand(query, cn);
                }
                else
                {
                    string query = "UPDATE notas SET idAlumno = @alumno, idMateria = @materia, idPeriodo = @periodo, nota1 = @n1, nota2 = @n2, nota3 = @n3 WHERE idNota = @id";
                    cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", idNotaSeleccionada);
                }

                cmd.Parameters.AddWithValue("@alumno", cmbAlumno.SelectedValue);
                cmd.Parameters.AddWithValue("@materia", cmbMateria.SelectedValue);
                cmd.Parameters.AddWithValue("@periodo", cmbPeriodo.SelectedValue);
                cmd.Parameters.AddWithValue("@n1", nudNota1.Value);
                cmd.Parameters.AddWithValue("@n2", nudNota2.Value);
                cmd.Parameters.AddWithValue("@n3", nudNota3.Value);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show(idNotaSeleccionada == 0 ? "Nota registrada correctamente." : "Nota actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
            CargarNotas();
        }

        private void dgvNotas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvNotas.CurrentRow != null)
            {
                idNotaSeleccionada = Convert.ToInt32(dgvNotas.Rows[e.RowIndex].Cells["idNota"].Value);

                nudNota1.Value = Convert.ToDecimal(dgvNotas.Rows[e.RowIndex].Cells["Nota 1"].Value);
                nudNota2.Value = Convert.ToDecimal(dgvNotas.Rows[e.RowIndex].Cells["Nota 2"].Value);
                nudNota3.Value = Convert.ToDecimal(dgvNotas.Rows[e.RowIndex].Cells["Nota 3"].Value);

                btnGuardar.Text = "Actualizar";
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idNotaSeleccionada == 0)
            {
                MessageBox.Show("Seleccione una nota de la lista para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este registro de notas?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    string query = "DELETE FROM notas WHERE idNota = @id";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", idNotaSeleccionada);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Registro de notas eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarNotas();
            }
        }

        private void LimpiarCampos()
        {
            idNotaSeleccionada = 0;
            nudNota1.Value = 0;
            nudNota2.Value = 0;
            nudNota3.Value = 0;
            btnGuardar.Text = "Guardar";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}