using DVLD_BussnessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Tests.TestTypes
{
    public partial class frmListTestTypes : Form
    {

        private DataTable _dtAllTestTypes;
        public frmListTestTypes()
        {
            InitializeComponent();
        }

        private void frmListTestTypes_Load(object sender, EventArgs e)
        {

            _dtAllTestTypes = clsTestType.GetAllTestTypes();
            dgvTestTypes.DataSource = _dtAllTestTypes;
            lblRecord.Text = dgvTestTypes.Rows.Count.ToString();

            dgvTestTypes.Columns[0].HeaderText = "ID";
            dgvTestTypes.Columns[1].HeaderText = "Title";
            dgvTestTypes.Columns[2].HeaderText = "Description";
            dgvTestTypes.Columns[3].HeaderText = "Fees";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editTestTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmEditTestType frm = new frmEditTestType((clsTestType.enTestType)dgvTestTypes.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            frmListTestTypes_Load(null, null);
        }
    }
}
