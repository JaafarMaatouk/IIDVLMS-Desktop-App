using DVLD_BussnessLayer;
using DVLD_PresentationLayer.GlobalClasses;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmManageViolations : Form
    {
        private clsVehicle _selectedVehicle;
        private DataTable _dtViolations;

        private readonly Color _clrDarkCharcoal = Color.FromArgb(28, 34, 42);
        private readonly Color _clrWarmGold = Color.FromArgb(212, 163, 89);
        private readonly Color _clrDeepGold = Color.FromArgb(166, 120, 46);
        private readonly Color _clrIvoryBg = Color.FromArgb(247, 246, 242);

        private TextBox txtPlateSearch;
        private Button btnFindVehicle;
        private Label lblVehicleVal;

        private ComboBox cmbViolationType;
        private TextBox txtLocation;
        private NumericUpDown numFineAmount;
        private TextBox txtNotes;
        private Button btnIssueViolation;

        private ComboBox cmbStatusFilter;
        private Button btnPaySelected;
        private Button btnRefresh;
        private DataGridView dgvViolations;
        private ContextMenuStrip cmsViolations;

        public frmManageViolations()
        {
            InitializeComponent();
            _BuildUI();
            _LoadViolations();
        }

        private void _BuildUI()
        {
            this.BackColor = _clrIvoryBg;
            this.Padding = new Padding(8, 10, 12, 10);

            Panel pnlTitle = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = Color.Transparent };
            Label lblTitle = new Label
            {
                Text = "Traffic Violations & Fines Management",
                Font = new Font("Segoe UI", 14.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 4),
                AutoSize = true
            };
            pnlTitle.Controls.Add(lblTitle);

            // Issue Violation Card
            Panel pnlIssue = new Panel
            {
                Dock = DockStyle.Top,
                Height = 165,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblHdr = new Label
            {
                Text = "Issue New Traffic Violation",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(14, 10),
                AutoSize = true
            };
            pnlIssue.Controls.Add(lblHdr);

            Label lblPlate = new Label { Text = "Plate Number *:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 40), AutoSize = true };
            txtPlateSearch = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(120, 37), Size = new Size(150, 26) };
            btnFindVehicle = _CreateGoldButton("Find", 278, 36, 75, BtnFindVehicle_Click);

            lblVehicleVal = new Label
            {
                Text = "Vehicle: Not Selected",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(370, 40),
                AutoSize = true
            };

            Label lblType = new Label { Text = "Violation Type *:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 80), AutoSize = true };
            cmbViolationType = new ComboBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(120, 77), Size = new Size(233, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbViolationType.Items.AddRange(new object[]
            {
                "Speeding Violation",
                "Red Light Crossing",
                "Illegal Parking",
                "Expired Technical Inspection",
                "Driving Without Seatbelt",
                "Reckless Driving",
                "Other Traffic Offense"
            });
            cmbViolationType.SelectedIndex = 0;

            Label lblLoc = new Label { Text = "Location *:", Font = new Font("Segoe UI", 9.5F), Location = new Point(370, 80), AutoSize = true };
            txtLocation = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(445, 77), Size = new Size(215, 26) };

            Label lblFine = new Label { Text = "Fine ($):", Font = new Font("Segoe UI", 9.5F), Location = new Point(680, 80), AutoSize = true };
            numFineAmount = new NumericUpDown { Font = new Font("Segoe UI", 9.5F), Location = new Point(740, 77), Size = new Size(130, 26), DecimalPlaces = 2, Minimum = 1, Maximum = 100000, Value = 100 };

            Label lblNotes = new Label { Text = "Notes:", Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 120), AutoSize = true };
            txtNotes = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(120, 117), Size = new Size(540, 26) };

            btnIssueViolation = _CreateGoldButton("Issue Violation", 680, 115, 190, BtnIssueViolation_Click);
            btnIssueViolation.Height = 32;

            pnlIssue.Controls.AddRange(new Control[] { lblPlate, txtPlateSearch, btnFindVehicle, lblVehicleVal, lblType, cmbViolationType, lblLoc, txtLocation, lblFine, numFineAmount, lblNotes, txtNotes, btnIssueViolation });

            // Filter & Pay Toolbar
            Panel pnlBar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.Transparent };

            Label lblFilter = new Label { Text = "Show:", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Location = new Point(2, 14), AutoSize = true };
            cmbStatusFilter = new ComboBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(55, 11), Size = new Size(175, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatusFilter.Items.AddRange(new object[] { "All Violations", "Unpaid Violations Only", "Paid Violations Only" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) => _ApplyFilter();

            btnPaySelected = _CreateGoldButton("Pay Selected Violation", 250, 9, 195, (s, e) => _PaySelectedViolation());
            btnPaySelected.Height = 30;

            btnRefresh = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = _clrDarkCharcoal,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(455, 9),
                Size = new Size(95, 30),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.Gray;
            btnRefresh.FlatAppearance.BorderSize = 1;
            btnRefresh.Click += (s, e) => _LoadViolations();

            pnlBar.Controls.AddRange(new Control[] { lblFilter, cmbStatusFilter, btnPaySelected, btnRefresh });

            // Context Menu
            cmsViolations = new ContextMenuStrip();
            ToolStripMenuItem mnuPay = new ToolStripMenuItem("Pay Selected Violation");
            mnuPay.Click += (s, e) => _PaySelectedViolation();
            cmsViolations.Items.Add(mnuPay);

            dgvViolations = _CreateStyledGrid();
            dgvViolations.ContextMenuStrip = cmsViolations;

            this.Controls.Add(dgvViolations);
            this.Controls.Add(pnlBar);
            this.Controls.Add(pnlIssue);
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
                lblVehicleVal.Text = "Vehicle: Not Found";
                MessageBox.Show("Vehicle not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clsPerson owner = _selectedVehicle.OwnerInfo;
            string ownerName = owner != null ? $"{owner.FirstName} {owner.LastName}" : "Unknown";
            lblVehicleVal.Text = $"Vehicle: {_selectedVehicle.PlateNumber} ({_selectedVehicle.Make} {_selectedVehicle.Model}) | Owner: {ownerName}";
        }

        private void BtnIssueViolation_Click(object sender, EventArgs e)
        {
            if (_selectedVehicle == null)
            {
                MessageBox.Show("Please select a vehicle first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter the violation location.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsDriver ownerDriver = clsDriver.FindByPersonID(_selectedVehicle.CurrentOwnerPersonID);

            clsTrafficViolation violation = new clsTrafficViolation
            {
                VehicleID = _selectedVehicle.VehicleID,
                DriverID = ownerDriver != null ? (int?)ownerDriver.DriverID : null,
                ViolationDate = DateTime.Now,
                ViolationType = cmbViolationType.SelectedItem.ToString(),
                Location = txtLocation.Text.Trim(),
                FineAmount = numFineAmount.Value,
                IsPaid = false,
                Notes = txtNotes.Text.Trim(),
                CreatedByUserID = clsGlobal.CurrentUser != null ? clsGlobal.CurrentUser.UserID : 1
            };

            if (violation.AddNew())
            {
                MessageBox.Show("Traffic violation issued successfully.", "Violation Recorded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLocation.Clear();
                txtNotes.Clear();
                _LoadViolations();
            }
            else
            {
                MessageBox.Show("Failed to issue traffic violation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string _FindViolationsColumn(params string[] candidates)
        {
            if (_dtViolations == null) return null;
            foreach (DataColumn col in _dtViolations.Columns)
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

        private void _PaySelectedViolation()
        {
            if (dgvViolations.CurrentRow == null || _dtViolations == null)
            {
                MessageBox.Show("Please select a violation row from the table.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string statusCol = _FindViolationsColumn("IsPaid", "Paid", "PaymentStatus", "Status", "PaymentDate");
            bool isPaid = false;

            if (statusCol != null)
            {
                object rawVal = dgvViolations.CurrentRow.Cells[statusCol].Value;
                Type colType = _dtViolations.Columns[statusCol].DataType;

                if (rawVal != null && rawVal != DBNull.Value)
                {
                    if (colType == typeof(bool))
                        isPaid = Convert.ToBoolean(rawVal);
                    else if (colType == typeof(DateTime))
                        isPaid = true;
                    else
                    {
                        string s = rawVal.ToString().Trim();
                        isPaid = s.Equals("Paid", StringComparison.OrdinalIgnoreCase)
                              || s.Equals("True", StringComparison.OrdinalIgnoreCase)
                              || s.Equals("Yes", StringComparison.OrdinalIgnoreCase)
                              || s == "1";
                    }
                }
            }

            if (isPaid)
            {
                MessageBox.Show("This violation is already paid.", "Already Paid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string idCol = _FindViolationsColumn("ViolationID", "ID");
            object idVal = idCol != null ? dgvViolations.CurrentRow.Cells[idCol].Value : dgvViolations.CurrentRow.Cells[0].Value;
            int violationID = Convert.ToInt32(idVal);

            string fineCol = _FindViolationsColumn("FineAmount", "Fine", "Amount", "Fees");
            string fine = (fineCol != null && dgvViolations.CurrentRow.Cells[fineCol].Value != null)
                ? dgvViolations.CurrentRow.Cells[fineCol].Value.ToString()
                : "";

            string confirmMsg = string.IsNullOrEmpty(fine)
                ? $"Confirm payment for Violation ID #{violationID}?"
                : $"Confirm payment of ${fine} for Violation ID #{violationID}?";

            if (MessageBox.Show(confirmMsg, "Confirm Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (clsTrafficViolation.PayViolation(violationID))
                {
                    MessageBox.Show("Violation fine paid successfully!", "Paid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadViolations();
                }
                else
                {
                    MessageBox.Show("Failed to record violation payment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _LoadViolations()
        {
            _dtViolations = clsTrafficViolation.GetAllViolations();
            dgvViolations.DataSource = _dtViolations;
            foreach (DataGridViewColumn col in dgvViolations.Columns)
                col.Width = 145;
            if (dgvViolations.Columns.Contains("ViolationID")) dgvViolations.Columns["ViolationID"].Width = 90;
            if (dgvViolations.Columns.Contains("ViolationType")) dgvViolations.Columns["ViolationType"].Width = 210;
            if (dgvViolations.Columns.Contains("Location")) dgvViolations.Columns["Location"].Width = 185;
            if (dgvViolations.Columns.Contains("Notes")) dgvViolations.Columns["Notes"].Width = 220;
            _ApplyFilter();
        }

        private void _ApplyFilter()
        {
            if (_dtViolations == null) return;

            string statusCol = _FindViolationsColumn("IsPaid", "Paid", "PaymentStatus", "Status", "PaymentDate");
            if (statusCol == null || cmbStatusFilter.SelectedIndex == 0)
            {
                _dtViolations.DefaultView.RowFilter = "";
                return;
            }

            Type colType = _dtViolations.Columns[statusCol].DataType;
            if (cmbStatusFilter.SelectedIndex == 1) // Unpaid
            {
                if (colType == typeof(bool))
                    _dtViolations.DefaultView.RowFilter = $"[{statusCol}] = false";
                else if (colType == typeof(DateTime))
                    _dtViolations.DefaultView.RowFilter = $"[{statusCol}] IS NULL";
                else
                    _dtViolations.DefaultView.RowFilter = $"[{statusCol}] <> 'Paid'";
            }
            else if (cmbStatusFilter.SelectedIndex == 2) // Paid
            {
                if (colType == typeof(bool))
                    _dtViolations.DefaultView.RowFilter = $"[{statusCol}] = true";
                else if (colType == typeof(DateTime))
                    _dtViolations.DefaultView.RowFilter = $"[{statusCol}] IS NOT NULL";
                else
                    _dtViolations.DefaultView.RowFilter = $"[{statusCol}] = 'Paid'";
            }
        }
    }
}