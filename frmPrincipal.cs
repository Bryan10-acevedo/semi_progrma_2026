using System;
using System.Windows.Forms;

namespace SistemaAcademico
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
            alumnosToolStripMenuItem.Click += alumnosToolStripMenuItem_Click;
            materiasToolStripMenuItem.Click += materiasToolStripMenuItem_Click;
            periodosToolStripMenuItem.Click += periodosToolStripMenuItem_Click;
            notasToolStripMenuItem.Click += notasToolStripMenuItem_Click;
        }

        private void AbrirFormularioHijo<T>() where T : Form, new()
        {
            foreach (Form child in this.MdiChildren)
            {
                if (child is T)
                {
                    child.BringToFront();
                    child.WindowState = FormWindowState.Normal;
                    return;
                }
            }

            T frm = new T();
            frm.MdiParent = this;
            frm.Show();
        }

        private void alumnosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo<frmAlumnos>();
        }

        private void materiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo<frmMaterias>();
        }

        private void periodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo<frmPeriodos>();
        }

        private void notasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo<frmNotas>();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
        }

        // Este método resuelve el enlace que busca el Designer
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }
    }
}