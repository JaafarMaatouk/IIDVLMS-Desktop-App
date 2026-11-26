using DVLD_BussnessLayer;
using DVLD_PresentationLayer.GlobalClasses;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmManageDetainedVehicles : Form
    {
        private clsVehicle _vehicleToDetain;
        private int _selectedDetainID = -1;
        private DataTable _dtDetained;

        private readonly Color _clrDarkCharcoal = Color.FromArgb(28, 34, 42);
        private readonly Color _clrWarmGold = Color.FromArgb(212, 163, 89);
        private readonly Color _clrDeepGold = Color.FromArgb(166, 120, 46);
        private readonly Color _clrIvoryBg = Color.FromArgb(247, 246, 242);

        // Detain Controls
        private TextBox txtDetainPlate;
        private Button btnFindForDetain;
        private Label lblDetainVehicleVal;
        private TextBox txtDetainReason;
        private TextBox txtDetainNotes;
        private NumericUpDown numDetainFees;
        private Button btnDetainVehicle;

        // Release Controls
        private Label lblSelectedDetainVal;
        private Label lblReleaseHint;
        private Button btnReleaseVehicle;

        // Filter & Grid
        private ComboBox cmbFilter;
        private DataGridView dgvDetained;

        public frmManageDetainedVehicles()
        {
            InitializeComponent();
            _BuildUI();
            _LoadDetainedVehicles();
        }

        private void _BuildUI()
        {
            this.BackColor = _clrIvoryBg;
            this.Padding = new Padding(8, 10, 12, 10);

            Panel pnlTitle = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = Color.Transparent };
            Label lblTitle = new Label
            {
                Text = "Vehicle Impoundment (Detain & Release Control)",
                Font = new Font("Segoe UI", 14.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 4),
                AutoSize = true
            };
            pnlTitle.Controls.Add(lblTitle);

            // Split Top Operations Panel
            Panel pnlTopCards = new Panel
            {
                Dock = DockStyle.Top,
                Height = 220,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Left Section: Detain Vehicle
            Label lblDetainHdr = new Label
            {
                Text = "1. Detain / Impound a Vehicle",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(14, 10),
                AutoSize = true
            };
            pnlTopCards.Controls.Add(lblDetainHdr);

            Label lblPlate = new Label { Text = "Plate No:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 38), AutoSize = true };
            txtDetainPlate = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(95, 35), Size = new Size(145, 26) };
            btnFindForDetain = _CreateGoldButton("Find", 248, 34, 75, BtnFindForDetain_Click);

            lblDetainVehicleVal = new Label
            {
                Text = "Vehicle: Not Selected",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(14, 66),
                AutoSize = true
            };

            Label lblReason = new Label { Text = "Reason *:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 92), AutoSize = true };
            txtDetainReason = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(95, 89), Size = new Size(340, 26) };

            Label lblNotes = new Label { Text = "Notes:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 128), AutoSize = true };
            txtDetainNotes = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(95, 125), Size = new Size(340, 26) };

            Label lblFees = new Label { Text = "Fine ($):", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 168), AutoSize = true };
            numDetainFees = new NumericUpDown { Font = new Font("Segoe UI", 9.5F), Location = new Point(95, 165), Size = new Size(130, 26), DecimalPlaces = 2, Minimum = 0, Maximum = 100000, Value = 150 };

            btnDetainVehicle = _CreateGoldButton("Detain Vehicle", 250, 163, 185, BtnDetainVehicle_Click);
            btnDetainVehicle.Height = 32;

            pnlTopCards.Controls.AddRange(new Control[] { lblPlate, txtDetainPlate, btnFindForDetain, lblDetainVehicleVal, lblReason, txtDetainReason, lblNotes, txtDetainNotes, lblFees, numDetainFees, btnDetainVehicle });

            // Divider
            Panel pnlDiv = new Panel { Location = new Point(465, 10), Size = new Size(1, 196), BackColor = Color.LightGray };
            pnlTopCards.Controls.Add(pnlDiv);

            // Right Section: Release Detained Vehicle
            Label lblRelHdr = new Label
            {
                Text = "2. Release Detained Vehicle (Select Row Below)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(485, 10),
                AutoSize = true
            };
            pnlTopCards.Controls.Add(lblRelHdr);

            lblSelectedDetainVal = new Label
            {
                Text = "Selected Detain Record: None (Click an active detained row below)",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(485, 44),
                AutoSize = true
            };
            pnlTopCards.Controls.Add(lblSelectedDetainVal);

            lblReleaseHint = new Label
            {
                Text = "Releasing a vehicle records the release date and releasing officer,\nand automatically re-enables ownership transfer eligibility.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.DimGray,
                Location = new Point(485, 76),
                AutoSize = true
            };
            pnlTopCards.Controls.Add(lblReleaseHint);

            btnReleaseVehicle = _CreateGoldButton("Release Selected Vehicle", 485, 130, 225, BtnReleaseVehicle_Click);
            btnReleaseVehicle.Height = 36;
            pnlTopCards.Controls.Add(btnReleaseVehicle);

            // Filter Bar
            Panel pnlBar = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = Color.Transparent };
            Label lblFilter = new Label { Text = "Filter Registry:", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Location = new Point(2, 12), AutoSize = true };
            cmbFilter = new ComboBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(105, 9), Size = new Size(200, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilter.Items.AddRange(new object[] { "All Detention Records", "Currently Detained Only", "Released Vehicles Only" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => _ApplyFilter();
            pnlBar.Controls.AddRange(new Control[] { lblFilter, cmbFilter });

            // Grid
            dgvDetained = _CreateStyledGrid();
            dgvDetained.SelectionChanged += DgvDetained_SelectionChanged;

            this.Controls.Add(dgvDetained);
            this.Controls.Add(pnlBar);
            this.Controls.Add(pnlTopCards);
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

        private void BtnFindForDetain_Click(object sender, EventArgs e)
        {
            string plate = txtDetainPlate.Text.Trim();
            if (string.IsNullOrEmpty(plate))
            {
                MessageBox.Show("Please enter a Plate Number.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _vehicleToDetain = _FindVehicleByPlate(plate);
            if (_vehicleToDetain == null)
            {
                lblDetainVehicleVal.Text = "Vehicle: Not Found";
                MessageBox.Show("Vehicle not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool alreadyDetained = clsDetainedVehicle.IsVehicleDetained(_vehicleToDetain.VehicleID);
            lblDetainVehicleVal.Text = alreadyDetained
                ? $"Vehicle: {_vehicleToDetain.PlateNumber} [ALREADY DETAINED]"
                : $"Vehicle: {_vehicleToDetain.PlateNumber} ({_vehicleToDetain.Make} {_vehicleToDetain.Model}) - Ready";
            lblDetainVehicleVal.ForeColor = alreadyDetained ? Color.FromArgb(185, 28, 28) : Color.FromArgb(22, 101, 52);
        }

        private void BtnDetainVehicle_Click(object sender, EventArgs e)
        {
            if (_vehicleToDetain == null)
            {
                MessageBox.Show("Please find and select a vehicle first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clsDetainedVehicle.IsVehicleDetained(_vehicleToDetain.VehicleID))
            {
                MessageBox.Show("This vehicle is already detained.", "Already Detained", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDetainReason.Text))
            {
                MessageBox.Show("Please enter the reason for detaining this vehicle.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsDetainedVehicle detain = new clsDetainedVehicle
            {
                VehicleID = _vehicleToDetain.VehicleID,
                DetainDate = DateTime.Now,
                DetainReason = txtDetainReason.Text.Trim(),
                FineFees = numDetainFees.Value,
                IsReleased = false,
                Notes = txtDetainNotes.Text.Trim(),
                DetainedByUserID = clsGlobal.CurrentUser != null ? clsGlobal.CurrentUser.UserID : 1
            };

            if (detain.Detain())
            {
                MessageBox.Show("Vehicle detained successfully.", "Detained", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDetainReason.Clear();
                txtDetainNotes.Clear();
                lblDetainVehicleVal.Text = $"Vehicle: {_vehicleToDetain.PlateNumber} [ALREADY DETAINED]";
                lblDetainVehicleVal.ForeColor = Color.FromArgb(185, 28, 28);
                _LoadDetainedVehicles();
            }
            else
            {
                MessageBox.Show("Failed to detain vehicle.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string _FindColumnName(DataGridView dgv, params string[] candidates)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                string normalized = col.Name.Replace(" ", "").Replace("_", "");
                foreach (string candidate in candidates)
                {
                    if (normalized.Equals(candidate, StringComparison.OrdinalIgnoreCase))
                        return col.Name;
                }
            }
            return null;
        }

        private string _FindDataTableColumnName(DataTable dt, params string[] candidates)
        {
            if (dt == null) return null;
            foreach (DataColumn col in dt.Columns)
            {
                string normalized = col.ColumnName.Replace(" ", "").Replace("_", "");
                foreach (string candidate in candidates)
                {
                    if (normalized.Equals(candidate, StringComparison.OrdinalIgnoreCase))
                        return col.ColumnName;
                }
            }
            return null;
        }

        private void DgvDetained_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDetained.CurrentRow == null || dgvDetained.Columns.Count == 0) return;

            string releasedCol = _FindColumnName(dgvDetained, "IsReleased", "Released");
            string releaseDateCol = _FindColumnName(dgvDetained, "ReleaseDate");
            string detainIdCol = _FindColumnName(dgvDetained, "DetainID", "ID");
            string plateCol = _FindColumnName(dgvDetained, "PlateNumber", "PlateNo");
            string vehicleIdCol = _FindColumnName(dgvDetained, "VehicleID");

            bool isReleased = false;
            if (releasedCol != null && dgvDetained.CurrentRow.Cells[releasedCol].Value != null && dgvDetained.CurrentRow.Cells[releasedCol].Value != DBNull.Value)
            {
                isReleased = Convert.ToBoolean(dgvDetained.CurrentRow.Cells[releasedCol].Value);
            }
            else if (releaseDateCol != null)
            {
                object relDateVal = dgvDetained.CurrentRow.Cells[releaseDateCol].Value;
                isReleased = (relDateVal != null && relDateVal != DBNull.Value && !string.IsNullOrWhiteSpace(relDateVal.ToString()));
            }

            object idVal = detainIdCol != null ? dgvDetained.CurrentRow.Cells[detainIdCol].Value : dgvDetained.CurrentRow.Cells[0].Value;
            int detainID = (idVal != null && idVal != DBNull.Value) ? Convert.ToInt32(idVal) : -1;

            string plate;
            if (plateCol != null && dgvDetained.CurrentRow.Cells[plateCol].Value != null)
                plate = dgvDetained.CurrentRow.Cells[plateCol].Value.ToString();
            else if (vehicleIdCol != null && dgvDetained.CurrentRow.Cells[vehicleIdCol].Value != null)
                plate = $"Vehicle #{dgvDetained.CurrentRow.Cells[vehicleIdCol].Value}";
            else
                plate = "Selected Vehicle";

            if (!isReleased && detainID != -1)
            {
                _selectedDetainID = detainID;
                lblSelectedDetainVal.Text = $"Selected Detain ID #{detainID} | Plate: {plate} (Active Detention)";
                lblSelectedDetainVal.ForeColor = Color.FromArgb(185, 28, 28);
                btnReleaseVehicle.Enabled = true;
            }
            else
            {
                _selectedDetainID = -1;
                lblSelectedDetainVal.Text = $"Detain ID #{detainID} | Plate: {plate} (Already Released)";
                lblSelectedDetainVal.ForeColor = Color.FromArgb(22, 101, 52);
                btnReleaseVehicle.Enabled = false;
            }
        }

        private void BtnReleaseVehicle_Click(object sender, EventArgs e)
        {
            if (_selectedDetainID == -1)
            {
                MessageBox.Show("Please select an active detained vehicle row from the table.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Confirm releasing Detain Record #{_selectedDetainID}?", "Confirm Release", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int userID = clsGlobal.CurrentUser != null ? clsGlobal.CurrentUser.UserID : 1;
                if (clsDetainedVehicle.ReleaseByDetainID(_selectedDetainID, userID))
                {
                    MessageBox.Show("Vehicle released from impoundment successfully!", "Released", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _selectedDetainID = -1;
                    _LoadDetainedVehicles();
                }
                else
                {
                    MessageBox.Show("Failed to release vehicle.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _LoadDetainedVehicles()
        {
            _dtDetained = clsDetainedVehicle.GetAllDetainedVehicles();
            dgvDetained.DataSource = _dtDetained;
            foreach (DataGridViewColumn col in dgvDetained.Columns)
                col.Width = 145;
            _ApplyFilter();
        }

        private void _ApplyFilter()
        {
            if (_dtDetained == null) return;

            string releasedCol = _FindDataTableColumnName(_dtDetained, "IsReleased", "Released");
            string releaseDateCol = _FindDataTableColumnName(_dtDetained, "ReleaseDate");

            if (cmbFilter.SelectedIndex == 1)
            {
                if (releasedCol != null)
                    _dtDetained.DefaultView.RowFilter = $"[{releasedCol}] = false";
                else if (releaseDateCol != null)
                    _dtDetained.DefaultView.RowFilter = $"[{releaseDateCol}] IS NULL";
            }
            else if (cmbFilter.SelectedIndex == 2)
            {
                if (releasedCol != null)
                    _dtDetained.DefaultView.RowFilter = $"[{releasedCol}] = true";
                else if (releaseDateCol != null)
                    _dtDetained.DefaultView.RowFilter = $"[{releaseDateCol}] IS NOT NULL";
            }
            else
            {
                _dtDetained.DefaultView.RowFilter = "";
            }
        }
    }
}