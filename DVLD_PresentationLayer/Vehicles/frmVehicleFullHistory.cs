using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DVLD_BussnessLayer;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmVehicleFullHistory : Form
    {
        private clsVehicle _selectedVehicle;

        private readonly Color _clrDarkCharcoal = Color.FromArgb(28, 34, 42);
        private readonly Color _clrWarmGold = Color.FromArgb(212, 163, 89);
        private readonly Color _clrDeepGold = Color.FromArgb(166, 120, 46);
        private readonly Color _clrIvoryBg = Color.FromArgb(247, 246, 242);

        private TextBox txtPlateSearch;
        private Button btnSearch;

        private Label lblVehicleTitleVal;
        private Label lblOwnerVal;
        private Label lblSpecsVal;
        private Label lblStatusBadgesVal;

        private TabControl tabHistory;
        private TabPage tpOwnership;
        private TabPage tpInspections;
        private TabPage tpViolations;
        private TabPage tpDetentions;

        private DataGridView dgvOwnership;
        private DataGridView dgvInspections;
        private DataGridView dgvViolations;
        private DataGridView dgvDetentions;

        public frmVehicleFullHistory()
        {
            InitializeComponent();
            _BuildUI();
        }

        private void _BuildUI()
        {
            this.BackColor = _clrIvoryBg;
            this.Padding = new Padding(8, 10, 12, 10);

            // Top Title
            Panel pnlTitle = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = Color.Transparent };
            Label lblTitle = new Label
            {
                Text = "Vehicle Complete Lifecycle & Audit History",
                Font = new Font("Segoe UI", 14.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 4),
                AutoSize = true
            };
            pnlTitle.Controls.Add(lblTitle);

            // Search & Summary Card
            Panel pnlSummary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 165,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblSearchPrompt = new Label
            {
                Text = "Search Vehicle by Plate Number:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(14, 15),
                AutoSize = true
            };
            txtPlateSearch = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(225, 12),
                Size = new Size(195, 27)
            };
            txtPlateSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    _LoadFullVehicleHistory();
                }
            };

            btnSearch = new Button
            {
                Text = "Load Full History",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = _clrDarkCharcoal,
                ForeColor = _clrWarmGold,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(430, 11),
                Size = new Size(155, 29),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderColor = _clrWarmGold;
            btnSearch.FlatAppearance.BorderSize = 1;
            btnSearch.Click += (s, e) => _LoadFullVehicleHistory();

            Panel pnlLine = new Panel
            {
                Location = new Point(14, 48),
                Size = new Size(920, 1),
                BackColor = Color.LightGray
            };

            lblVehicleTitleVal = new Label
            {
                Text = "Vehicle: Enter a plate number above to view its complete record.",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(14, 58),
                AutoSize = true
            };

            lblOwnerVal = new Label
            {
                Text = "Current Owner: ---",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(14, 86),
                AutoSize = true
            };

            lblSpecsVal = new Label
            {
                Text = "Specifications: ---",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(14, 112),
                AutoSize = true
            };

            lblStatusBadgesVal = new Label
            {
                Text = "Compliance Summary: ---",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                Location = new Point(14, 136),
                AutoSize = true
            };

            pnlSummary.Controls.AddRange(new Control[]
            {
                lblSearchPrompt, txtPlateSearch, btnSearch, pnlLine,
                lblVehicleTitleVal, lblOwnerVal, lblSpecsVal, lblStatusBadgesVal
            });

            Panel pnlSpacer = new Panel { Dock = DockStyle.Top, Height = 8, BackColor = Color.Transparent };

            // 4 Categorized History Tabs
            tabHistory = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            tpOwnership = new TabPage("1. Ownership Transfers (0)");
            tpInspections = new TabPage("2. Technical Inspections (0)");
            tpViolations = new TabPage("3. Traffic Violations (0)");
            tpDetentions = new TabPage("4. Detain & Release Log (0)");

            dgvOwnership = _CreateStyledGrid();
            dgvInspections = _CreateStyledGrid();
            dgvViolations = _CreateStyledGrid();
            dgvDetentions = _CreateStyledGrid();

            tpOwnership.Controls.Add(dgvOwnership);
            tpInspections.Controls.Add(dgvInspections);
            tpViolations.Controls.Add(dgvViolations);
            tpDetentions.Controls.Add(dgvDetentions);

            tabHistory.TabPages.AddRange(new TabPage[] { tpOwnership, tpInspections, tpViolations, tpDetentions });

            this.Controls.Add(tabHistory);
            this.Controls.Add(pnlSpacer);
            this.Controls.Add(pnlSummary);
            this.Controls.Add(pnlTitle);
        }

        private DataGridView _CreateStyledGrid()
        {
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ScrollBars = ScrollBars.Both,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F)
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = _clrDarkCharcoal;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = _clrWarmGold;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 222, 196);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 249, 246);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 34;
            dgv.RowTemplate.Height = 28;
            return dgv;
        }

        private clsVehicle _FindVehicleByPlate(string plate)
        {
            DataTable dt = clsVehicle.GetAllVehicles();
            foreach (DataRow row in dt.Rows)
            {
                if (row["PlateNumber"].ToString().Equals(plate, StringComparison.OrdinalIgnoreCase))
                    return clsVehicle.Find(Convert.ToInt32(row["VehicleID"]));
            }
            return null;
        }

        private void _LoadFullVehicleHistory()
        {
            string plate = txtPlateSearch.Text.Trim();
            if (string.IsNullOrEmpty(plate))
            {
                MessageBox.Show("Please enter a Plate Number.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedVehicle = _FindVehicleByPlate(plate);
            if (_selectedVehicle == null)
            {
                lblVehicleTitleVal.Text = "Vehicle: Not Found";
                lblOwnerVal.Text = "Current Owner: ---";
                lblSpecsVal.Text = "Specifications: ---";
                lblStatusBadgesVal.Text = "Compliance Summary: ---";
                MessageBox.Show("No vehicle found matching this Plate Number.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int vid = _selectedVehicle.VehicleID;
            clsPerson owner = _selectedVehicle.OwnerInfo;
            clsLicenseClass lc = clsLicenseClass.Find(_selectedVehicle.RequiredLicenseClassID);

            lblVehicleTitleVal.Text = $"Vehicle #{vid}: [{_selectedVehicle.PlateNumber}]  {_selectedVehicle.Make} {_selectedVehicle.Model} ({_selectedVehicle.ManufactureYear})";
            lblOwnerVal.Text = owner != null
                ? $"Current Owner: {owner.FirstName} {owner.SecondName} {owner.LastName}  |  National No: {owner.NationalNo}  |  Phone: {owner.Phone}"
                : $"Current Owner ID: {_selectedVehicle.CurrentOwnerPersonID}";

            lblSpecsVal.Text = $"Chassis (VIN): {_selectedVehicle.ChassisNumber}   |   Color: {_selectedVehicle.Color}   |   Fuel: {_selectedVehicle.FuelType}   |   Required Class: {(lc != null ? lc.ClassName : "N/A")}";

            bool isDetained = clsDetainedVehicle.IsVehicleDetained(vid);
            bool hasUnpaid = clsTrafficViolation.HasUnpaidViolations(vid);
            bool hasInspection = clsTechnicalInspection.HasValidTechnicalInspection(vid);

            lblStatusBadgesVal.Text =
                $"Status: {(_selectedVehicle.IsActive ? "ACTIVE" : "INACTIVE")}   |   " +
                $"Impoundment: {(isDetained ? "DETAINED" : "CLEAR")}   |   " +
                $"Violations: {(hasUnpaid ? "UNPAID FINES EXIST" : "ALL CLEAR")}   |   " +
                $"Inspection: {(hasInspection ? "VALID" : "EXPIRED / NONE")}";
            lblStatusBadgesVal.ForeColor = (!isDetained && !hasUnpaid && hasInspection)
                ? Color.FromArgb(22, 101, 52)
                : Color.FromArgb(185, 28, 28);

            // Load the 4 tables
            DataTable dtOwn = clsVehicleOwnership.GetOwnershipHistory(vid);
            DataTable dtInsp = clsTechnicalInspection.GetVehicleInspections(vid);
            DataTable dtViol = clsTrafficViolation.GetVehicleViolations(vid);
            DataTable dtDet = clsDetainedVehicle.GetVehicleDetainHistory(vid);

            dgvOwnership.DataSource = dtOwn;
            dgvInspections.DataSource = dtInsp;
            dgvViolations.DataSource = dtViol;
            dgvDetentions.DataSource = dtDet;

            _FormatColumns(dgvOwnership);
            _FormatColumns(dgvInspections);
            _FormatColumns(dgvViolations);
            _FormatColumns(dgvDetentions);

            tpOwnership.Text = $"1. Ownership Transfers ({dtOwn.Rows.Count})";
            tpInspections.Text = $"2. Technical Inspections ({dtInsp.Rows.Count})";
            tpViolations.Text = $"3. Traffic Violations ({dtViol.Rows.Count})";
            tpDetentions.Text = $"4. Detain & Release Log ({dtDet.Rows.Count})";
        }

        private void _FormatColumns(DataGridView dgv)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
                col.Width = 160;
        }
    }
}