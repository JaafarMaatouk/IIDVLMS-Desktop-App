using DVLD_BussnessLayer;
using DVLD_PresentationLayer.GlobalClasses;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmManageInspections : Form
    {
        private clsVehicle _selectedVehicle;

        private readonly Color _clrDarkCharcoal = Color.FromArgb(28, 34, 42);
        private readonly Color _clrWarmGold = Color.FromArgb(212, 163, 89);
        private readonly Color _clrDeepGold = Color.FromArgb(166, 120, 46);
        private readonly Color _clrIvoryBg = Color.FromArgb(247, 246, 242);

        private TextBox txtPlateSearch;
        private Button btnFindVehicle;
        private Label lblVehicleDetailsVal;
        private Label lblValidityStatusVal;

        private TextBox txtCenterName;
        private ComboBox cmbResult;
        private NumericUpDown numFees;
        private TextBox txtNotes;
        private Button btnSaveInspection;
        private Button btnShowAll;
        private DataGridView dgvInspections;

        public frmManageInspections()
        {
            InitializeComponent();
            _BuildUI();
            _LoadInspections();
        }

        private void _BuildUI()
        {
            this.BackColor = _clrIvoryBg;
            this.Padding = new Padding(8, 10, 12, 10);

            Panel pnlTitle = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = Color.Transparent };
            Label lblTitle = new Label
            {
                Text = "Periodic Technical Inspections",
                Font = new Font("Segoe UI", 14.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 4),
                AutoSize = true
            };
            pnlTitle.Controls.Add(lblTitle);

            // Top Form Card
            Panel pnlForm = new Panel
            {
                Dock = DockStyle.Top,
                Height = 205,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblStep1 = new Label
            {
                Text = "1. Vehicle Lookup",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(14, 10),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblStep1);

            Label lblPlate = new Label { Text = "Plate Number:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 40), AutoSize = true };
            txtPlateSearch = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(115, 37), Size = new Size(170, 26) };
            btnFindVehicle = _CreateGoldButton("Find Vehicle", 295, 36, 115, BtnFindVehicle_Click);

            lblVehicleDetailsVal = new Label
            {
                Text = "Vehicle: Not Selected",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(430, 40),
                AutoSize = true
            };

            lblValidityStatusVal = new Label
            {
                Text = "Inspection Status: ---",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                Location = new Point(740, 40),
                AutoSize = true
            };

            pnlForm.Controls.AddRange(new Control[] { lblPlate, txtPlateSearch, btnFindVehicle, lblVehicleDetailsVal, lblValidityStatusVal });

            Panel pnlLine = new Panel { Location = new Point(14, 75), Size = new Size(920, 1), BackColor = Color.LightGray };
            pnlForm.Controls.Add(pnlLine);

            Label lblStep2 = new Label
            {
                Text = "2. Record Technical Inspection Result",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(14, 84),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblStep2);

            Label lblCenter = new Label { Text = "Inspection Center *:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 116), AutoSize = true };
            txtCenterName = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(150, 113), Size = new Size(220, 26), Text = "Central Technical Inspection Station" };

            Label lblRes = new Label { Text = "Test Result *:", Font = new Font("Segoe UI", 9.5F), Location = new Point(400, 116), AutoSize = true };
            cmbResult = new ComboBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(495, 113), Size = new Size(185, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbResult.Items.AddRange(new object[] { "Passed (Valid 1 Year)", "Failed" });
            cmbResult.SelectedIndex = 0;

            Label lblFees = new Label { Text = "Fees ($):", Font = new Font("Segoe UI", 9.5F), Location = new Point(705, 116), AutoSize = true };
            numFees = new NumericUpDown { Font = new Font("Segoe UI", 9.5F), Location = new Point(770, 113), Size = new Size(120, 26), DecimalPlaces = 2, Maximum = 50000, Value = 35 };

            Label lblNotes = new Label { Text = "Inspector Notes:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 158), AutoSize = true };
            txtNotes = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(150, 155), Size = new Size(440, 26) };

            btnSaveInspection = _CreateGoldButton("Save Inspection", 610, 153, 150, BtnSaveInspection_Click);
            btnSaveInspection.Height = 32;

            btnShowAll = new Button
            {
                Text = "Show All",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = _clrDarkCharcoal,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(770, 153),
                Size = new Size(120, 32),
                Cursor = Cursors.Hand
            };
            btnShowAll.FlatAppearance.BorderColor = Color.Gray;
            btnShowAll.FlatAppearance.BorderSize = 1;
            btnShowAll.Click += (s, e) => { _selectedVehicle = null; txtPlateSearch.Clear(); lblVehicleDetailsVal.Text = "Vehicle: All Vehicles"; lblValidityStatusVal.Text = "Inspection Status: ---"; _LoadInspections(); };

            pnlForm.Controls.AddRange(new Control[] { lblCenter, txtCenterName, lblRes, cmbResult, lblFees, numFees, lblNotes, txtNotes, btnSaveInspection, btnShowAll });

            Panel pnlSpacer = new Panel { Dock = DockStyle.Top, Height = 8, BackColor = Color.Transparent };

            dgvInspections = _CreateStyledGrid();

            this.Controls.Add(dgvInspections);
            this.Controls.Add(pnlSpacer);
            this.Controls.Add(pnlForm);
            this.Controls.Add(pnlTitle);
        }

        private Button _CreateGoldButton(string text, int x, int y, int width, EventHandler onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = _clrDarkCharcoal,
                ForeColor = _clrWarmGold,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, y),
                Size = new Size(width, 28),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = _clrWarmGold;
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += onClick;
            return btn;
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

        private void BtnFindVehicle_Click(object sender, EventArgs e)
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
                lblVehicleDetailsVal.Text = "Vehicle: Not Found";
                lblValidityStatusVal.Text = "Inspection Status: ---";
                MessageBox.Show("Vehicle not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblVehicleDetailsVal.Text = $"Vehicle: {_selectedVehicle.PlateNumber} ({_selectedVehicle.Make} {_selectedVehicle.Model})";
            bool isValid = clsTechnicalInspection.HasValidTechnicalInspection(_selectedVehicle.VehicleID);
            lblValidityStatusVal.Text = isValid ? "Status: VALID" : "Status: EXPIRED / NONE";
            lblValidityStatusVal.ForeColor = isValid ? Color.FromArgb(22, 101, 52) : Color.FromArgb(185, 28, 28);

            _LoadInspections(_selectedVehicle.VehicleID);
        }

        private void BtnSaveInspection_Click(object sender, EventArgs e)
        {
            if (_selectedVehicle == null)
            {
                MessageBox.Show("Please find and select a vehicle first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCenterName.Text))
            {
                MessageBox.Show("Please enter the Inspection Center name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsTechnicalInspection inspection = new clsTechnicalInspection
            {
                VehicleID = _selectedVehicle.VehicleID,
                InspectionDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1),
                InspectionResult = (cmbResult.SelectedIndex == 0),
                TestCenterName = txtCenterName.Text.Trim(),
                PaidFees = numFees.Value,
                Notes = txtNotes.Text.Trim(),
                CreatedByUserID = clsGlobal.CurrentUser != null ? clsGlobal.CurrentUser.UserID : 1
            };

            if (inspection.AddNew())
            {
                MessageBox.Show("Technical inspection recorded successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                bool isValid = clsTechnicalInspection.HasValidTechnicalInspection(_selectedVehicle.VehicleID);
                lblValidityStatusVal.Text = isValid ? "Status: VALID" : "Status: EXPIRED / NONE";
                lblValidityStatusVal.ForeColor = isValid ? Color.FromArgb(22, 101, 52) : Color.FromArgb(185, 28, 28);
                txtNotes.Clear();
                _LoadInspections(_selectedVehicle.VehicleID);
            }
            else
            {
                MessageBox.Show("Failed to save inspection record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _LoadInspections(int vehicleID = -1)
        {
            if (vehicleID != -1)
            {
                dgvInspections.DataSource = clsTechnicalInspection.GetVehicleInspections(vehicleID);
            }
            else
            {
                DataTable dtAll = null;
                DataTable dtVehicles = clsVehicle.GetAllVehicles();
                foreach (DataRow vRow in dtVehicles.Rows)
                {
                    int vid = Convert.ToInt32(vRow["VehicleID"]);
                    DataTable dtVehInsp = clsTechnicalInspection.GetVehicleInspections(vid);
                    if (dtAll == null)
                        dtAll = dtVehInsp.Clone();
                    foreach (DataRow iRow in dtVehInsp.Rows)
                        dtAll.ImportRow(iRow);
                }
                if (dtAll != null)
                    dgvInspections.DataSource = dtAll;
            }

            foreach (DataGridViewColumn col in dgvInspections.Columns)
                col.Width = 155;
            if (dgvInspections.Columns.Contains("InspectionID")) dgvInspections.Columns["InspectionID"].Width = 95;
            if (dgvInspections.Columns.Contains("TestCenterName")) dgvInspections.Columns["TestCenterName"].Width = 230;
            if (dgvInspections.Columns.Contains("Notes")) dgvInspections.Columns["Notes"].Width = 230;
        }
    }
}