using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DVLD_BussnessLayer;

namespace DVLD_PresentationLayer.Vehicles
{
    public partial class frmListVehicles : Form
    {
        private DataTable _dtVehicles;
        private readonly Action<int> _editVehicleCallback;
        private readonly Action _addVehicleCallback;

        private ComboBox cmbFilterBy;
        private TextBox txtFilterValue;
        private Button btnAddNew;
        private Button btnRefresh;
        private DataGridView dgvVehicles;
        private Label lblRecordsCount;
        private ContextMenuStrip cmsVehicles;

        private readonly Color _clrDarkCharcoal = Color.FromArgb(28, 34, 42);
        private readonly Color _clrWarmGold = Color.FromArgb(212, 163, 89);
        private readonly Color _clrIvoryBg = Color.FromArgb(247, 246, 242);

        public frmListVehicles(Action addVehicleCallback = null, Action<int> editVehicleCallback = null)
        {
            InitializeComponent();
            _addVehicleCallback = addVehicleCallback;
            _editVehicleCallback = editVehicleCallback;
            _BuildUI();
            _LoadVehiclesData();
        }

        private void _BuildUI()
        {
            this.BackColor = _clrIvoryBg;
            this.Padding = new Padding(8, 10, 12, 10);

            // Top Header & Filter Bar
            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "Registered Vehicles Directory",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 4),
                AutoSize = true
            };
            pnlTop.Controls.Add(lblTitle);

            Label lblFilter = new Label
            {
                Text = "Filter By:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 47),
                AutoSize = true
            };
            pnlTop.Controls.Add(lblFilter);

            cmbFilterBy = new ComboBox
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(72, 44),
                Size = new Size(170, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterBy.Items.AddRange(new object[]
            {
                "None",
                "Plate Number",
                "Chassis Number",
                "Owner National No",
                "Owner Full Name",
                "Manufacturer (Make)"
            });
            cmbFilterBy.SelectedIndex = 0;
            cmbFilterBy.SelectedIndexChanged += (s, e) =>
            {
                txtFilterValue.Visible = cmbFilterBy.SelectedIndex != 0;
                txtFilterValue.Clear();
                _ApplyFilter();
            };
            pnlTop.Controls.Add(cmbFilterBy);

            txtFilterValue = new TextBox
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(250, 44),
                Size = new Size(210, 26),
                Visible = false
            };
            txtFilterValue.TextChanged += (s, e) => _ApplyFilter();
            pnlTop.Controls.Add(txtFilterValue);

            btnAddNew = new Button
            {
                Text = "+ Register Vehicle",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = _clrDarkCharcoal,
                ForeColor = _clrWarmGold,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(145, 32),
                Cursor = Cursors.Hand
            };
            btnAddNew.FlatAppearance.BorderColor = _clrWarmGold;
            btnAddNew.FlatAppearance.BorderSize = 1;
            btnAddNew.Click += (s, e) =>
            {
                if (_addVehicleCallback != null)
                    _addVehicleCallback.Invoke();
                else
                {
                    frmAddUpdateVehicle frm = new frmAddUpdateVehicle(_LoadVehiclesData);
                    frm.ShowDialog();
                }
            };
            pnlTop.Controls.Add(btnAddNew);

            btnRefresh = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = _clrDarkCharcoal,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(90, 32),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(180, 175, 165);
            btnRefresh.FlatAppearance.BorderSize = 1;
            btnRefresh.Click += (s, e) => _LoadVehiclesData();
            pnlTop.Controls.Add(btnRefresh);

            pnlTop.Resize += (s, e) =>
            {
                btnAddNew.Location = new Point(pnlTop.Width - btnAddNew.Width, 40);
                btnRefresh.Location = new Point(btnAddNew.Left - btnRefresh.Width - 8, 40);
            };

            // Bottom Footer Bar
            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.Transparent
            };

            lblRecordsCount = new Label
            {
                Text = "Total Records: 0",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _clrDarkCharcoal,
                Location = new Point(2, 8),
                AutoSize = true
            };
            pnlBottom.Controls.Add(lblRecordsCount);

            // Context Menu
            cmsVehicles = new ContextMenuStrip();
            ToolStripMenuItem mnuEdit = new ToolStripMenuItem("Edit Vehicle Info");
            mnuEdit.Click += (s, e) => _EditSelectedVehicle();

            ToolStripMenuItem mnuDelete = new ToolStripMenuItem("Delete Vehicle");
            mnuDelete.Click += (s, e) => _DeleteSelectedVehicle();

            ToolStripMenuItem mnuRefresh = new ToolStripMenuItem("Refresh List");
            mnuRefresh.Click += (s, e) => _LoadVehiclesData();

            cmsVehicles.Items.AddRange(new ToolStripItem[] { mnuEdit, mnuDelete, new ToolStripSeparator(), mnuRefresh });

            // DataGridView with Horizontal Scrollbar & Generous Column Widths
            dgvVehicles = new DataGridView
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
                ContextMenuStrip = cmsVehicles,
                Font = new Font("Segoe UI", 9.5F)
            };
            dgvVehicles.ColumnHeadersDefaultCellStyle.BackColor = _clrDarkCharcoal;
            dgvVehicles.ColumnHeadersDefaultCellStyle.ForeColor = _clrWarmGold;
            dgvVehicles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvVehicles.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvVehicles.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvVehicles.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 222, 196);
            dgvVehicles.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvVehicles.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 249, 246);
            dgvVehicles.EnableHeadersVisualStyles = false;
            dgvVehicles.ColumnHeadersHeight = 38;
            dgvVehicles.RowTemplate.Height = 30;
            dgvVehicles.DoubleClick += (s, e) => _EditSelectedVehicle();

            this.Controls.Add(dgvVehicles);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlTop);
        }

        public void _LoadVehiclesData()
        {
            _dtVehicles = clsVehicle.GetAllVehicles();
            dgvVehicles.DataSource = _dtVehicles;
            _FormatGridColumns();
            lblRecordsCount.Text = $"Total Records: {dgvVehicles.Rows.Count}";
        }

        private void _FormatGridColumns()
        {
            if (dgvVehicles.Columns.Count == 0) return;

            _SetColumn("VehicleID", "ID", 60);
            _SetColumn("PlateNumber", "Plate Number", 130);
            _SetColumn("ChassisNumber", "Chassis Number (VIN)", 190);
            _SetColumn("Make", "Manufacturer", 125);
            _SetColumn("Model", "Model", 125);
            _SetColumn("ManufactureYear", "Year", 75);
            _SetColumn("Color", "Color", 95);
            _SetColumn("FuelType", "Fuel Type", 100);
            _SetColumn("RequiredLicenseClass", "Required License Class", 240);
            _SetColumn("OwnerNationalNo", "Owner National No", 150);
            _SetColumn("OwnerFullName", "Current Owner Full Name", 230);
            _SetColumn("RegistrationDate", "Registration Date", 145);
            _SetColumn("IsActive", "Active", 75);
            _SetColumn("IsDetained", "Detained", 85);
            _SetColumn("UnpaidViolationsCount", "Unpaid Violations", 135);
        }

        private void _SetColumn(string colName, string headerText, int width)
        {
            if (dgvVehicles.Columns.Contains(colName))
            {
                dgvVehicles.Columns[colName].HeaderText = headerText;
                dgvVehicles.Columns[colName].Width = width;
            }
        }

        private void _ApplyFilter()
        {
            if (_dtVehicles == null) return;

            string filterColumn = "";
            switch (cmbFilterBy.SelectedItem.ToString())
            {
                case "Plate Number": filterColumn = "PlateNumber"; break;
                case "Chassis Number": filterColumn = "ChassisNumber"; break;
                case "Owner National No": filterColumn = "OwnerNationalNo"; break;
                case "Owner Full Name": filterColumn = "OwnerFullName"; break;
                case "Manufacturer (Make)": filterColumn = "Make"; break;
            }

            string val = txtFilterValue.Text.Trim().Replace("'", "''");
            if (string.IsNullOrEmpty(filterColumn) || string.IsNullOrEmpty(val))
                _dtVehicles.DefaultView.RowFilter = "";
            else
                _dtVehicles.DefaultView.RowFilter = $"{filterColumn} LIKE '%{val}%'";

            lblRecordsCount.Text = $"Total Records: {dgvVehicles.Rows.Count}";
        }

        private void _EditSelectedVehicle()
        {
            if (dgvVehicles.CurrentRow == null) return;
            int vehicleID = Convert.ToInt32(dgvVehicles.CurrentRow.Cells["VehicleID"].Value);

            if (_editVehicleCallback != null)
                _editVehicleCallback.Invoke(vehicleID);
            else
            {
                frmAddUpdateVehicle frm = new frmAddUpdateVehicle(vehicleID, _LoadVehiclesData);
                frm.ShowDialog();
            }
        }

        private void _DeleteSelectedVehicle()
        {
            if (dgvVehicles.CurrentRow == null) return;
            int vehicleID = Convert.ToInt32(dgvVehicles.CurrentRow.Cells["VehicleID"].Value);
            string plate = dgvVehicles.CurrentRow.Cells["PlateNumber"].Value.ToString();

            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to delete vehicle [{plate}]?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (clsVehicle.DeleteVehicle(vehicleID))
                {
                    MessageBox.Show("Vehicle deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadVehiclesData();
                }
                else
                {
                    MessageBox.Show("Cannot delete this vehicle because it has linked ownership, inspection, or violation records.", "Deletion Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}