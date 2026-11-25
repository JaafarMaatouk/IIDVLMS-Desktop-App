using DVLD_BussnessLayer;
using DVLD_PresentationLayer.GlobalClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmTransferVehicleOwnership : Form
    {
        private clsVehicle _selectedVehicle;
        private clsPerson _buyerPerson;

        private readonly Color _clrDarkCharcoal = Color.FromArgb(28, 34, 42);
        private readonly Color _clrWarmGold = Color.FromArgb(212, 163, 89);
        private readonly Color _clrDeepGold = Color.FromArgb(166, 120, 46);
        private readonly Color _clrIvoryBg = Color.FromArgb(247, 246, 242);

        private TextBox txtPlateSearch;
        private Button btnFindVehicle;
        private Label lblVehicleInfoVal;
        private Label lblCurrentOwnerVal;
        private Label lblRequiredClassVal;

        private TextBox txtBuyerSearch;
        private Button btnFindBuyer;
        private Label lblBuyerNameVal;
        private Label lblBuyerNationalNoVal;

        private Label lblRule1Status;
        private Label lblRule2Status;
        private Label lblRule3Status;
        private Label lblRule4Status;
        private Label lblRule5Status;

        private NumericUpDown numSalePrice;
        private TextBox txtNotes;
        private Button btnTransfer;
        private DataGridView dgvHistory;

        public frmTransferVehicleOwnership()
        {
            InitializeComponent();
            _BuildUI();
            _LoadAllHistory();
        }

        private void _BuildUI()
        {
            this.BackColor = _clrIvoryBg;
            this.AutoScroll = true;
            this.Padding = new Padding(8, 10, 12, 10);

            Panel pnlTitle = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = Color.Transparent };
            Label lblTitle = new Label
            {
                Text = "Vehicle Ownership Transfer & Legal Compliance Check",
                Font = new Font("Segoe UI", 14.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 4),
                AutoSize = true
            };
            pnlTitle.Controls.Add(lblTitle);

            // Top Selection Panel (Vehicle + Buyer)
            Panel pnlSelection = new Panel
            {
                Dock = DockStyle.Top,
                Height = 155,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Left Half: Vehicle Search
            Label lblVehHdr = new Label
            {
                Text = "1. Select Vehicle (Plate Number)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(14, 10),
                AutoSize = true
            };
            pnlSelection.Controls.Add(lblVehHdr);

            txtPlateSearch = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(14, 38), Size = new Size(175, 26) };
            pnlSelection.Controls.Add(txtPlateSearch);

            btnFindVehicle = _CreateGoldButton("Find Vehicle", 196, 37, 110, BtnFindVehicle_Click);
            pnlSelection.Controls.Add(btnFindVehicle);

            lblVehicleInfoVal = _AddStatusLine(pnlSelection, "Vehicle:", "Not Selected", 14, 74);
            lblCurrentOwnerVal = _AddStatusLine(pnlSelection, "Current Owner:", "---", 14, 98);
            lblRequiredClassVal = _AddStatusLine(pnlSelection, "Required Class:", "---", 14, 122);

            // Vertical Divider
            Panel pnlDiv = new Panel { Location = new Point(465, 10), Size = new Size(1, 132), BackColor = Color.LightGray };
            pnlSelection.Controls.Add(pnlDiv);

            // Right Half: Buyer Search
            Label lblBuyerHdr = new Label
            {
                Text = "2. Select New Owner / Buyer (National No)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(485, 10),
                AutoSize = true
            };
            pnlSelection.Controls.Add(lblBuyerHdr);

            txtBuyerSearch = new TextBox { Font = new Font("Segoe UI", 9.5F), Location = new Point(485, 38), Size = new Size(185, 26) };
            pnlSelection.Controls.Add(txtBuyerSearch);

            btnFindBuyer = _CreateGoldButton("Find Buyer", 678, 37, 110, BtnFindBuyer_Click);
            pnlSelection.Controls.Add(btnFindBuyer);

            lblBuyerNameVal = _AddStatusLine(pnlSelection, "Buyer Name:", "Not Selected", 485, 78);
            lblBuyerNationalNoVal = _AddStatusLine(pnlSelection, "National No:", "---", 485, 106);

            Panel pnlSpacer1 = new Panel { Dock = DockStyle.Top, Height = 8, BackColor = Color.Transparent };

            // Middle Panel: 5 Legal Constraints + Transfer Action
            Panel pnlRules = new Panel
            {
                Dock = DockStyle.Top,
                Height = 175,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblRulesHdr = new Label
            {
                Text = "3. Five Mandatory Legal Transfer Validations",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _clrDeepGold,
                Location = new Point(14, 10),
                AutoSize = true
            };
            pnlRules.Controls.Add(lblRulesHdr);

            lblRule1Status = _AddRuleLabel(pnlRules, "Rule 1: Buyer must be different from Current Owner", 14, 38);
            lblRule2Status = _AddRuleLabel(pnlRules, "Rule 2: Vehicle must be active and NOT detained", 14, 62);
            lblRule3Status = _AddRuleLabel(pnlRules, "Rule 3: Vehicle must have ZERO unpaid traffic violations", 14, 86);
            lblRule4Status = _AddRuleLabel(pnlRules, "Rule 4: Vehicle must hold a valid Technical Inspection", 14, 110);
            lblRule5Status = _AddRuleLabel(pnlRules, "Rule 5: Buyer must hold an active License of Required Class", 14, 134);

            // Right Side of Middle Panel: Sale Price & Confirm Button
            Label lblPrice = new Label
            {
                Text = "Sale Price ($):",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(540, 38),
                AutoSize = true
            };
            numSalePrice = new NumericUpDown
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(665, 35),
                Size = new Size(170, 26),
                Minimum = 0,
                Maximum = 10000000,
                DecimalPlaces = 2,
                Value = 5000
            };
            pnlRules.Controls.Add(lblPrice);
            pnlRules.Controls.Add(numSalePrice);

            Label lblNotes = new Label
            {
                Text = "Transfer Notes:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(540, 74),
                AutoSize = true
            };
            txtNotes = new TextBox
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(665, 71),
                Size = new Size(270, 26)
            };
            pnlRules.Controls.Add(lblNotes);
            pnlRules.Controls.Add(txtNotes);

            btnTransfer = _CreateGoldButton("Confirm Ownership Transfer", 665, 115, 270, BtnTransfer_Click);
            btnTransfer.Height = 36;
            pnlRules.Controls.Add(btnTransfer);

            Panel pnlSpacer2 = new Panel { Dock = DockStyle.Top, Height = 8, BackColor = Color.Transparent };

            // Bottom Grid: Ownership Transfer Records
            dgvHistory = _CreateStyledGrid();

            this.Controls.Add(dgvHistory);
            this.Controls.Add(pnlSpacer2);
            this.Controls.Add(pnlRules);
            this.Controls.Add(pnlSpacer1);
            this.Controls.Add(pnlSelection);
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

        private Label _AddStatusLine(Panel parent, string title, string initialVal, int x, int y)
        {
            Label lblT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                Location = new Point(x, y),
                AutoSize = true
            };
            Label lblV = new Label
            {
                Text = initialVal,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(x + 108, y),
                AutoSize = true
            };
            parent.Controls.Add(lblT);
            parent.Controls.Add(lblV);
            return lblV;
        }

        private Label _AddRuleLabel(Panel parent, string ruleText, int x, int y)
        {
            Label lbl = new Label
            {
                Text = $"[PENDING]  {ruleText}",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                Location = new Point(x, y),
                AutoSize = true
            };
            parent.Controls.Add(lbl);
            return lbl;
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
                lblVehicleInfoVal.Text = "Not Found";
                lblCurrentOwnerVal.Text = "---";
                lblRequiredClassVal.Text = "---";
                MessageBox.Show("No vehicle found with this Plate Number.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _EvaluateRulesPreview();
                return;
            }

            lblVehicleInfoVal.Text = $"{_selectedVehicle.PlateNumber} - {_selectedVehicle.Make} {_selectedVehicle.Model} ({_selectedVehicle.ManufactureYear})";
            clsPerson owner = _selectedVehicle.OwnerInfo;
            lblCurrentOwnerVal.Text = owner != null ? $"{owner.FirstName} {owner.LastName} ({owner.NationalNo})" : $"Person ID: {_selectedVehicle.CurrentOwnerPersonID}";

            clsLicenseClass lc = clsLicenseClass.Find(_selectedVehicle.RequiredLicenseClassID);
            lblRequiredClassVal.Text = lc != null ? lc.ClassName : $"Class ID: {_selectedVehicle.RequiredLicenseClassID}";

            _LoadVehicleHistory(_selectedVehicle.VehicleID);
            _EvaluateRulesPreview();
        }

        private void BtnFindBuyer_Click(object sender, EventArgs e)
        {
            string query = txtBuyerSearch.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Please enter Buyer National No or Person ID.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsPerson p = clsPerson.Find(query);
            if (p == null && int.TryParse(query, out int id))
                p = clsPerson.Find(id);

            if (p == null)
            {
                _buyerPerson = null;
                lblBuyerNameVal.Text = "Not Found";
                lblBuyerNationalNoVal.Text = "---";
                MessageBox.Show("No person found matching this identifier.", "Buyer Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _EvaluateRulesPreview();
                return;
            }

            _buyerPerson = p;
            lblBuyerNameVal.Text = $"{p.FirstName} {p.SecondName} {p.LastName}".Trim();
            lblBuyerNationalNoVal.Text = p.NationalNo;
            _EvaluateRulesPreview();
        }

        private void _SetRuleDisplay(Label lbl, bool? passed, string text)
        {
            if (passed == null)
            {
                lbl.Text = $"[PENDING]  {text}";
                lbl.ForeColor = Color.DimGray;
            }
            else if (passed.Value)
            {
                lbl.Text = $"[PASS]     {text}";
                lbl.ForeColor = Color.FromArgb(22, 101, 52);
            }
            else
            {
                lbl.Text = $"[FAIL]     {text}";
                lbl.ForeColor = Color.FromArgb(185, 28, 28);
            }
        }

        private List<string> _EvaluateRulesPreview()
        {
            List<string> failedReasons = new List<string>();

            // Rule 1: Different Owner
            if (_selectedVehicle == null || _buyerPerson == null)
            {
                _SetRuleDisplay(lblRule1Status, null, "Rule 1: Buyer must be different from Current Owner");
            }
            else
            {
                bool r1 = (_selectedVehicle.CurrentOwnerPersonID != _buyerPerson.PersonID);
                _SetRuleDisplay(lblRule1Status, r1, "Rule 1: Buyer must be different from Current Owner");
                if (!r1) failedReasons.Add("- Rule 1 Failed: The selected buyer is already the current owner of this vehicle.");
            }

            // Rule 2: Active and Not Detained
            if (_selectedVehicle == null)
            {
                _SetRuleDisplay(lblRule2Status, null, "Rule 2: Vehicle must be active and NOT detained");
            }
            else
            {
                bool r2 = _selectedVehicle.IsActive && !clsDetainedVehicle.IsVehicleDetained(_selectedVehicle.VehicleID);
                _SetRuleDisplay(lblRule2Status, r2, "Rule 2: Vehicle must be active and NOT detained");
                if (!r2) failedReasons.Add("- Rule 2 Failed: Vehicle is either inactive or currently detained.");
            }

            // Rule 3: Zero Unpaid Violations
            if (_selectedVehicle == null)
            {
                _SetRuleDisplay(lblRule3Status, null, "Rule 3: Vehicle must have ZERO unpaid traffic violations");
            }
            else
            {
                bool r3 = !clsTrafficViolation.HasUnpaidViolations(_selectedVehicle.VehicleID);
                _SetRuleDisplay(lblRule3Status, r3, "Rule 3: Vehicle must have ZERO unpaid traffic violations");
                if (!r3) failedReasons.Add("- Rule 3 Failed: Vehicle has unpaid traffic violations that must be settled first.");
            }

            // Rule 4: Valid Technical Inspection
            if (_selectedVehicle == null)
            {
                _SetRuleDisplay(lblRule4Status, null, "Rule 4: Vehicle must hold a valid Technical Inspection");
            }
            else
            {
                bool r4 = clsTechnicalInspection.HasValidTechnicalInspection(_selectedVehicle.VehicleID);
                _SetRuleDisplay(lblRule4Status, r4, "Rule 4: Vehicle must hold a valid Technical Inspection");
                if (!r4) failedReasons.Add("- Rule 4 Failed: Vehicle does not have a valid, unexpired technical inspection.");
            }

            // Rule 5: Buyer holds active License of Required Class
            if (_selectedVehicle == null || _buyerPerson == null)
            {
                _SetRuleDisplay(lblRule5Status, null, "Rule 5: Buyer must hold an active License of Required Class");
            }
            else
            {
                int activeLicenseID = clsLicense.GetActiveLicenseIDByPersonID(_buyerPerson.PersonID, _selectedVehicle.RequiredLicenseClassID);
                bool r5 = (activeLicenseID != -1);
                _SetRuleDisplay(lblRule5Status, r5, "Rule 5: Buyer must hold an active License of Required Class");
                if (!r5) failedReasons.Add("- Rule 5 Failed: Buyer does not hold an active driving license matching the vehicle's required class.");
            }

            return failedReasons;
        }

        private void BtnTransfer_Click(object sender, EventArgs e)
        {
            if (_selectedVehicle == null || _buyerPerson == null)
            {
                MessageBox.Show("Please select both a Vehicle and a Buyer before transferring ownership.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> failedReasons = _EvaluateRulesPreview();
            if (failedReasons.Count > 0)
            {
                MessageBox.Show(
                    "Ownership Transfer Blocked due to the following legal rules:\n\n" + string.Join(Environment.NewLine, failedReasons),
                    "Legal Validation Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int userID = clsGlobal.CurrentUser != null ? clsGlobal.CurrentUser.UserID : 1;
            if (clsVehicleOwnership.TransferOwnership(
                _selectedVehicle.VehicleID,
                _buyerPerson.PersonID,
                numSalePrice.Value,
                txtNotes.Text.Trim(),
                userID,
                out _))
            {
                MessageBox.Show("Vehicle ownership transferred and ownership history updated successfully!", "Transfer Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _selectedVehicle = clsVehicle.Find(_selectedVehicle.VehicleID);
                lblCurrentOwnerVal.Text = $"{_buyerPerson.FirstName} {_buyerPerson.LastName} ({_buyerPerson.NationalNo})";
                _LoadVehicleHistory(_selectedVehicle.VehicleID);
                _EvaluateRulesPreview();
            }
            else
            {
                MessageBox.Show("Ownership transfer could not be completed. Please verify all legal requirements.", "Transfer Rejected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _LoadAllHistory()
        {
            DataTable dtAll = null;
            DataTable dtVehicles = clsVehicle.GetAllVehicles();
            foreach (DataRow vRow in dtVehicles.Rows)
            {
                int vid = Convert.ToInt32(vRow["VehicleID"]);
                DataTable dtVehHist = clsVehicleOwnership.GetOwnershipHistory(vid);
                if (dtAll == null)
                    dtAll = dtVehHist.Clone();
                foreach (DataRow hRow in dtVehHist.Rows)
                    dtAll.ImportRow(hRow);
            }

            if (dtAll != null)
            {
                dgvHistory.DataSource = dtAll;
                _FormatHistoryColumns();
            }
        }

        private void _LoadVehicleHistory(int vehicleID)
        {
            dgvHistory.DataSource = clsVehicleOwnership.GetOwnershipHistory(vehicleID);
            _FormatHistoryColumns();
        }

        private void _FormatHistoryColumns()
        {
            foreach (DataGridViewColumn col in dgvHistory.Columns)
                col.Width = 155;
            if (dgvHistory.Columns.Contains("OwnershipID")) dgvHistory.Columns["OwnershipID"].Width = 95;
            if (dgvHistory.Columns.Contains("OwnerFullName")) dgvHistory.Columns["OwnerFullName"].Width = 220;
            if (dgvHistory.Columns.Contains("Notes")) dgvHistory.Columns["Notes"].Width = 240;
        }
    }
}