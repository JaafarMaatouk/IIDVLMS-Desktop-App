using DVLD_PresentationLayer.GlobalClasses;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmVehiclesDashboard : Form
    {
        private Panel pnlHeader;
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Button _activeButton;

        private Button btnOverview;
        private Button btnVehiclesList;
        private Button btnRegisterVehicle;
        private Button btnTransferOwnership;
        private Button btnInspections;
        private Button btnViolations;
        private Button btnDetainRelease;
        private Button btnFullHistory;

        // Palette harmonized with the Vehicle Management Department building image
        private readonly Color _clrDarkCharcoal = Color.FromArgb(28, 34, 42);
        private readonly Color _clrSidebarHover = Color.FromArgb(42, 50, 62);
        private readonly Color _clrWarmGold = Color.FromArgb(212, 163, 89);
        private readonly Color _clrIvoryBg = Color.FromArgb(247, 246, 242);

        public frmVehiclesDashboard()
        {
            InitializeComponent();
            _BuildDashboardLayout();
            _NavigateTo("Overview");
        }

        private void _BuildDashboardLayout()
        {
            this.Text = "IIDVLMS - Vehicle Management Department";
            this.Size = new Size(1260, 730);
            this.MinimumSize = new Size(1060, 630);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = _clrIvoryBg;

            // 1. Top Header Bar
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.White
            };
            pnlHeader.Paint += (s, e) =>
            {
                using (Pen goldLine = new Pen(_clrWarmGold, 2))
                    e.Graphics.DrawLine(goldLine, 0, pnlHeader.Height - 2, pnlHeader.Width, pnlHeader.Height - 2);
            };

            Label lblSystemTitle = new Label
            {
                Text = "IIDVLMS  |  Vehicle Management Department",
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(16, 14),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblSystemTitle);

            string userName = clsGlobal.CurrentUser != null ? clsGlobal.CurrentUser.UserName : "Officer";
            Label lblCurrentUser = new Label
            {
                Text = $"Officer: {userName}",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(166, 120, 46),
                Location = new Point(1100, 15),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblCurrentUser);

            // 2. Left Navigation Sidebar
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 225,
                BackColor = _clrDarkCharcoal
            };
            pnlSidebar.Paint += (s, e) =>
            {
                using (Pen border = new Pen(_clrWarmGold, 2))
                    e.Graphics.DrawLine(border, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
            };

            Button btnClose = _CreateSidebarButton("Close Portal", (s, e) => this.Close());
            btnClose.Dock = DockStyle.Bottom;
            btnClose.BackColor = Color.FromArgb(20, 25, 31);

            btnFullHistory = _CreateSidebarButton("Vehicle Full History", (s, e) => _NavigateTo("FullHistory"));
            btnDetainRelease = _CreateSidebarButton("Detain & Release", (s, e) => _NavigateTo("DetainRelease"));
            btnViolations = _CreateSidebarButton("Traffic Violations", (s, e) => _NavigateTo("Violations"));
            btnInspections = _CreateSidebarButton("Technical Inspections", (s, e) => _NavigateTo("Inspections"));
            btnTransferOwnership = _CreateSidebarButton("Transfer Ownership", (s, e) => _NavigateTo("TransferOwnership"));
            btnRegisterVehicle = _CreateSidebarButton("Register Vehicle", (s, e) => _NavigateTo("RegisterVehicle"));
            btnVehiclesList = _CreateSidebarButton("Vehicles List", (s, e) => _NavigateTo("VehiclesList"));
            btnOverview = _CreateSidebarButton("Department Overview", (s, e) => _NavigateTo("Overview"));

            pnlSidebar.Controls.Add(btnFullHistory);
            pnlSidebar.Controls.Add(btnDetainRelease);
            pnlSidebar.Controls.Add(btnViolations);
            pnlSidebar.Controls.Add(btnInspections);
            pnlSidebar.Controls.Add(btnTransferOwnership);
            pnlSidebar.Controls.Add(btnRegisterVehicle);
            pnlSidebar.Controls.Add(btnVehiclesList);
            pnlSidebar.Controls.Add(btnOverview);
            pnlSidebar.Controls.Add(btnClose);

            // 3. Right Content Panel (Flush next to sidebar)
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _clrIvoryBg,
                Padding = new Padding(0)
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlHeader);
        }

        private Button _CreateSidebarButton(string text, EventHandler onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Font = new Font("Segoe UI", 9.8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(230, 228, 222),
                BackColor = _clrDarkCharcoal,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = _clrSidebarHover;
            btn.Click += onClick;
            return btn;
        }

        private void _HighlightActiveButton(Button selectedBtn)
        {
            if (_activeButton != null)
            {
                _activeButton.BackColor = _clrDarkCharcoal;
                _activeButton.ForeColor = Color.FromArgb(230, 228, 222);
                _activeButton.Font = new Font("Segoe UI", 9.8F, FontStyle.Regular);
            }

            _activeButton = selectedBtn;
            if (_activeButton != null)
            {
                _activeButton.BackColor = _clrWarmGold;
                _activeButton.ForeColor = Color.FromArgb(20, 25, 31);
                _activeButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            }
        }

        private void _OpenChildForm(Form childForm)
        {
            pnlContent.Controls.Clear();
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(childForm);
            childForm.Show();
        }

        private void _ShowComingSoonModule(string moduleTitle, string description)
        {
            pnlContent.Controls.Clear();
            Panel pnl = new Panel { Dock = DockStyle.Fill, BackColor = _clrIvoryBg };
            Label lbl = new Label
            {
                Text = $"{moduleTitle}\n\n{description}",
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(16, 16),
                AutoSize = true
            };
            pnl.Controls.Add(lbl);
            pnlContent.Controls.Add(pnl);
        }

        private void _NavigateTo(string route, int parameterID = -1)
        {
            switch (route)
            {
                case "Overview":
                    _HighlightActiveButton(btnOverview);
                    _OpenChildForm(new frmVehiclesOverview());
                    break;

                case "VehiclesList":
                    _HighlightActiveButton(btnVehiclesList);
                    _OpenChildForm(new frmListVehicles(
                        () => _NavigateTo("RegisterVehicle"),
                        vehicleID => _NavigateTo("EditVehicle", vehicleID)));
                    break;

                case "RegisterVehicle":
                    _HighlightActiveButton(btnRegisterVehicle);
                    _OpenChildForm(new frmAddUpdateVehicle(() => _NavigateTo("VehiclesList")));
                    break;

                case "EditVehicle":
                    _HighlightActiveButton(btnVehiclesList);
                    _OpenChildForm(new frmAddUpdateVehicle(parameterID, () => _NavigateTo("VehiclesList")));
                    break;

                case "TransferOwnership":
                    _HighlightActiveButton(btnTransferOwnership);
                    _ShowComingSoonModule("Vehicle Ownership Transfer", "Will be added in the next branch.");
                    break;

                case "Inspections":
                    _HighlightActiveButton(btnInspections);
                    _ShowComingSoonModule("Technical Inspections", "Will be added in the next branch.");
                    break;

                case "Violations":
                    _HighlightActiveButton(btnViolations);
                    _ShowComingSoonModule("Traffic Violations", "Will be added in the next branch.");
                    break;

                case "DetainRelease":
                    _HighlightActiveButton(btnDetainRelease);
                    _ShowComingSoonModule("Detain & Release Vehicles", "Will be added in the next branch.");
                    break;

                case "FullHistory":
                    _HighlightActiveButton(btnFullHistory);
                    _ShowComingSoonModule("Vehicle Full History", "Will be added in the final branch.");
                    break;
            }
        }
    }
}