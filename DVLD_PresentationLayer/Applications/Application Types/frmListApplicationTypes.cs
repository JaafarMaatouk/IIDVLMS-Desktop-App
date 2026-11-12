using DVLD_BussnessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Applications.ApplicationTypes
{
    public partial class frmListApplicationTypes : Form
    {

        private DataTable _dtAllApplicationTypes;
        public frmListApplicationTypes()
        {
            InitializeComponent();
        }
        private void frmListApplicationTypes_Load(object sender, EventArgs e)
        {

            _dtAllApplicationTypes = clsApplicationType.GetAllApplicationTypes();
            dgvApplicationTypes.DataSource = _dtAllApplicationTypes;
            lblRecord.Text = dgvApplicationTypes.Rows.Count.ToString();

            dgvApplicationTypes.Columns[0].HeaderText = "ID";

            dgvApplicationTypes.Columns[1].HeaderText = "Title";

            dgvApplicationTypes.Columns[2].HeaderText = "Fees";


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editApplicationTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmEditApplicationType frm = new frmEditApplicationType((int)dgvApplicationTypes.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            frmListApplicationTypes_Load(null, null);
        }
    }
}
