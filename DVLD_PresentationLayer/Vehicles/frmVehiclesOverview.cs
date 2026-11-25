using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmVehiclesOverview : Form
    {
        public frmVehiclesOverview()
        {
            InitializeComponent();
            _BuildUI();
        }

        private void _BuildUI()
        {
            this.BackColor = Color.FromArgb(247, 246, 242);
            // Flush next to left sidebar (8px) and clean margin on right (12px)
            this.Padding = new Padding(8, 10, 12, 10);

            Panel pnlFrame = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(28, 34, 42),
                Padding = new Padding(2)
            };

            PictureBox picDepartment = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(28, 34, 42),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = Properties.Resources.VehicleDepartment
            };

            pnlFrame.Controls.Add(picDepartment);
            this.Controls.Add(pnlFrame);
        }
    }
}