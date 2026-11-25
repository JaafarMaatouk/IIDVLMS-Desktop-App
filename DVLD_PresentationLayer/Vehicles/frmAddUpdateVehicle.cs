using DVLD_BussnessLayer;
using DVLD_PresentationLayer.GlobalClasses;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmAddUpdateVehicle : Form
    {
        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _mode = enMode.AddNew;

        private int _vehicleID = -1;
        private clsVehicle _vehicle;
        private clsPerson _selectedOwner;

        private readonly Action _onSavedCallback;

        private readonly Color _clrDarkCharcoal = Color.FromArgb(28, 34, 42);
        private readonly Color _clrWarmGold = Color.FromArgb(212, 163, 89);
        private readonly Color _clrIvoryBg = Color.FromArgb(247, 246, 242);

        private Label lblFormTitle;
        private TextBox txtOwnerSearch;
        private Button btnFindOwner;
        private Label lblOwnerNameVal;
        private Label lblOwnerNationalNoVal;
        private Label lblOwnerPhoneVal;

        private TextBox txtPlateNumber;
        private TextBox txtChassisNumber;
        private TextBox txtMake;
        private TextBox txtModel;
        private NumericUpDown numYear;
        private TextBox txtColor;
        private ComboBox cmbFuelType;
        private ComboBox cmbLicenseClass;
        private CheckBox chkIsActive;
        private Button btnSave;
        private Button btnReset;

        public frmAddUpdateVehicle(Action onSavedCallback = null)
        {
            InitializeComponent();
            _mode = enMode.AddNew;
            _onSavedCallback = onSavedCallback;
            _BuildUI();
            _LoadLicenseClasses();
            _ResetDefaultValues();
        }

        public frmAddUpdateVehicle(int vehicleID, Action onSavedCallback = null)
        {
            InitializeComponent();
            _mode = enMode.Update;
            _vehicleID = vehicleID;
            _onSavedCallback = onSavedCallback;
            _BuildUI();
            _LoadLicenseClasses();
            _LoadVehicleData();
        }

        private void _BuildUI()
        {
            this.BackColor = _clrIvoryBg;
            this.AutoScroll = true;
            this.Padding = new Padding(8, 10, 12, 10);

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 42,
                BackColor = Color.Transparent
            };

            lblFormTitle = new Label
            {
                Text = _mode == enMode.AddNew ? "Register New Vehicle" : $"Update Vehicle Info (ID: {_vehicleID})",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 4),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblFormTitle);

            // 1. Owner Card
            Panel pnlOwner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 138,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblOwnerHeader = new Label
            {
                Text = "1. Vehicle Owner Identification",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(166, 120, 46),
                Location = new Point(15, 12),
                AutoSize = true
            };
            pnlOwner.Controls.Add(lblOwnerHeader);

            Label lblSearchPrompt = new Label
            {
                Text = "Owner National No or Person ID:",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(15, 48),
                AutoSize = true
            };
            pnlOwner.Controls.Add(lblSearchPrompt);

            txtOwnerSearch = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(220, 45),
                Size = new Size(200, 27)
            };
            pnlOwner.Controls.Add(txtOwnerSearch);

            btnFindOwner = new Button
            {
                Text = "Find Owner",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = _clrDarkCharcoal,
                ForeColor = _clrWarmGold,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(430, 44),
                Size = new Size(115, 29),
                Cursor = Cursors.Hand
            };
            btnFindOwner.FlatAppearance.BorderColor = _clrWarmGold;
            btnFindOwner.FlatAppearance.BorderSize = 1;
            btnFindOwner.Click += BtnFindOwner_Click;
            pnlOwner.Controls.Add(btnFindOwner);

            lblOwnerNameVal = _AddInfoField(pnlOwner, "Full Name:", "Not Selected", 15, 92, 340);
            lblOwnerNationalNoVal = _AddInfoField(pnlOwner, "National No:", "---", 380, 92, 210);
            lblOwnerPhoneVal = _AddInfoField(pnlOwner, "Phone:", "---", 610, 92, 220);

            Panel pnlSpacer = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };

            // 2. Vehicle Specifications Card
            Panel pnlSpecs = new Panel
            {
                Dock = DockStyle.Top,
                Height = 315,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblSpecsHeader = new Label
            {
                Text = "2. Technical & Registration Specifications",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(166, 120, 46),
                Location = new Point(15, 12),
                AutoSize = true
            };
            pnlSpecs.Controls.Add(lblSpecsHeader);

            txtPlateNumber = _AddTextInput(pnlSpecs, "Plate Number *:", 15, 50);
            txtChassisNumber = _AddTextInput(pnlSpecs, "Chassis Number (VIN) *:", 15, 98);
            txtMake = _AddTextInput(pnlSpecs, "Manufacturer (Make) *:", 15, 146);
            txtModel = _AddTextInput(pnlSpecs, "Model *:", 15, 194);

            Label lblYear = new Label
            {
                Text = "Manufacture Year *:",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(460, 53),
                AutoSize = true
            };
            numYear = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(635, 50),
                Size = new Size(220, 27),
                Minimum = 1970,
                Maximum = DateTime.Now.Year + 1,
                Value = DateTime.Now.Year
            };
            pnlSpecs.Controls.Add(lblYear);
            pnlSpecs.Controls.Add(numYear);

            txtColor = _AddTextInput(pnlSpecs, "Color *:", 460, 98);

            Label lblFuel = new Label
            {
                Text = "Fuel Type *:",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(460, 149),
                AutoSize = true
            };
            cmbFuelType = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(635, 146),
                Size = new Size(220, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFuelType.Items.AddRange(new object[] { "Gasoline", "Diesel", "Hybrid", "Electric" });
            cmbFuelType.SelectedIndex = 0;
            pnlSpecs.Controls.Add(lblFuel);
            pnlSpecs.Controls.Add(cmbFuelType);

            Label lblClass = new Label
            {
                Text = "Required License Class *:",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(460, 197),
                AutoSize = true
            };
            cmbLicenseClass = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(635, 194),
                Size = new Size(220, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            pnlSpecs.Controls.Add(lblClass);
            pnlSpecs.Controls.Add(cmbLicenseClass);

            chkIsActive = new CheckBox
            {
                Text = "Vehicle Registration is Active",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(15, 255),
                Checked = true,
                AutoSize = true
            };
            pnlSpecs.Controls.Add(chkIsActive);

            btnSave = new Button
            {
                Text = _mode == enMode.AddNew ? "Save Vehicle" : "Update Vehicle",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = _clrDarkCharcoal,
                ForeColor = _clrWarmGold,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(715, 250),
                Size = new Size(140, 35),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderColor = _clrWarmGold;
            btnSave.FlatAppearance.BorderSize = 1;
            btnSave.Click += BtnSave_Click;
            pnlSpecs.Controls.Add(btnSave);

            btnReset = new Button
            {
                Text = "Clear Form",
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                ForeColor = _clrDarkCharcoal,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(595, 250),
                Size = new Size(110, 35),
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(180, 175, 165);
            btnReset.FlatAppearance.BorderSize = 1;
            btnReset.Click += (s, e) => _ResetDefaultValues();
            pnlSpecs.Controls.Add(btnReset);

            this.Controls.Add(pnlSpecs);
            this.Controls.Add(pnlSpacer);
            this.Controls.Add(pnlOwner);
            this.Controls.Add(pnlHeader);
        }

        private TextBox _AddTextInput(Panel parent, string labelText, int x, int y)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(x, y + 3),
                AutoSize = true
            };
            TextBox txt = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(x + 175, y),
                Size = new Size(220, 27)
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            return txt;
        }

        private Label _AddInfoField(Panel parent, string title, string initialVal, int x, int y, int maxWidth)
        {
            Label lblT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(x, y),
                AutoSize = true
            };
            Label lblV = new Label
            {
                Text = initialVal,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(x + 85, y),
                MaximumSize = new Size(maxWidth - 85, 25),
                AutoSize = true
            };
            parent.Controls.Add(lblT);
            parent.Controls.Add(lblV);
            return lblV;
        }

        private void _LoadLicenseClasses()
        {
            DataTable dtClasses = clsLicenseClass.GetAllLicenseClasses();
            cmbLicenseClass.DataSource = dtClasses;
            cmbLicenseClass.DisplayMember = "ClassName";
            cmbLicenseClass.ValueMember = "LicenseClassID";
        }

        private void _ResetDefaultValues()
        {
            _vehicle = new clsVehicle();
            _selectedOwner = null;
            txtOwnerSearch.Clear();
            lblOwnerNameVal.Text = "Not Selected";
            lblOwnerNationalNoVal.Text = "---";
            lblOwnerPhoneVal.Text = "---";

            txtPlateNumber.Clear();
            txtChassisNumber.Clear();
            txtMake.Clear();
            txtModel.Clear();
            numYear.Value = DateTime.Now.Year;
            txtColor.Clear();
            cmbFuelType.SelectedIndex = 0;
            if (cmbLicenseClass.Items.Count > 2)
                cmbLicenseClass.SelectedIndex = 2;
            chkIsActive.Checked = true;
        }

        private void _LoadVehicleData()
        {
            _vehicle = clsVehicle.Find(_vehicleID);
            if (_vehicle == null)
            {
                MessageBox.Show("Vehicle record was not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _selectedOwner = _vehicle.OwnerInfo;
            if (_selectedOwner != null)
            {
                txtOwnerSearch.Text = _selectedOwner.NationalNo;
                lblOwnerNameVal.Text = _GetFullName(_selectedOwner);
                lblOwnerNationalNoVal.Text = _selectedOwner.NationalNo;
                lblOwnerPhoneVal.Text = _selectedOwner.Phone;
            }

            txtOwnerSearch.Enabled = false;
            btnFindOwner.Enabled = false;

            txtPlateNumber.Text = _vehicle.PlateNumber;
            txtChassisNumber.Text = _vehicle.ChassisNumber;
            txtMake.Text = _vehicle.Make;
            txtModel.Text = _vehicle.Model;
            numYear.Value = _vehicle.ManufactureYear;
            txtColor.Text = _vehicle.Color;
            cmbFuelType.SelectedItem = _vehicle.FuelType;
            cmbLicenseClass.SelectedValue = _vehicle.RequiredLicenseClassID;
            chkIsActive.Checked = _vehicle.IsActive;
        }

        private string _GetFullName(clsPerson p)
        {
            return $"{p.FirstName} {p.SecondName} {(string.IsNullOrWhiteSpace(p.ThirdName) ? "" : p.ThirdName + " ")}{p.LastName}".Trim();
        }

        private void BtnFindOwner_Click(object sender, EventArgs e)
        {
            string query = txtOwnerSearch.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Please enter an Owner National No or Person ID.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsPerson person = clsPerson.Find(query);
            if (person == null && int.TryParse(query, out int personID))
                person = clsPerson.Find(personID);

            if (person == null)
            {
                _selectedOwner = null;
                lblOwnerNameVal.Text = "Not Found";
                lblOwnerNationalNoVal.Text = "---";
                lblOwnerPhoneVal.Text = "---";
                MessageBox.Show("No person found matching the provided identifier.", "Owner Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _selectedOwner = person;
            lblOwnerNameVal.Text = _GetFullName(person);
            lblOwnerNationalNoVal.Text = person.NationalNo;
            lblOwnerPhoneVal.Text = person.Phone;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_selectedOwner == null)
            {
                MessageBox.Show("Please select a valid vehicle owner first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOwnerSearch.Focus();
                return;
            }

            string plate = txtPlateNumber.Text.Trim().ToUpper();
            string chassis = txtChassisNumber.Text.Trim().ToUpper();
            string make = txtMake.Text.Trim();
            string model = txtModel.Text.Trim();
            string color = txtColor.Text.Trim();

            if (string.IsNullOrEmpty(plate) || string.IsNullOrEmpty(chassis) ||
                string.IsNullOrEmpty(make) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(color))
            {
                MessageBox.Show("Please fill in all required fields marked with (*).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int excludeID = _mode == enMode.Update ? _vehicle.VehicleID : -1;

            if (clsVehicle.IsPlateNumberExist(plate, excludeID))
            {
                MessageBox.Show("This Plate Number is already registered to another vehicle.", "Duplicate Plate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPlateNumber.Focus();
                return;
            }

            if (clsVehicle.IsChassisNumberExist(chassis, excludeID))
            {
                MessageBox.Show("This Chassis Number (VIN) already exists in the system.", "Duplicate Chassis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtChassisNumber.Focus();
                return;
            }

            _vehicle.PlateNumber = plate;
            _vehicle.ChassisNumber = chassis;
            _vehicle.Make = make;
            _vehicle.Model = model;
            _vehicle.ManufactureYear = (int)numYear.Value;
            _vehicle.Color = color;
            _vehicle.FuelType = cmbFuelType.SelectedItem.ToString();
            _vehicle.RequiredLicenseClassID = Convert.ToInt32(cmbLicenseClass.SelectedValue);
            _vehicle.CurrentOwnerPersonID = _selectedOwner.PersonID;
            _vehicle.IsActive = chkIsActive.Checked;

            if (_mode == enMode.AddNew)
            {
                _vehicle.RegistrationDate = DateTime.Now;
                _vehicle.CreatedByUserID = clsGlobal.CurrentUser != null ? clsGlobal.CurrentUser.UserID : 1;
            }

            if (_vehicle.Save())
            {
                _mode = enMode.Update;
                _vehicleID = _vehicle.VehicleID;
                lblFormTitle.Text = $"Update Vehicle Info (ID: {_vehicleID})";
                btnSave.Text = "Update Vehicle";
                txtOwnerSearch.Enabled = false;
                btnFindOwner.Enabled = false;

                MessageBox.Show($"Vehicle saved successfully!\nVehicle ID: {_vehicle.VehicleID}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _onSavedCallback?.Invoke();
            }
            else
            {
                MessageBox.Show("Failed to save vehicle record.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}