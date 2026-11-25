using DVLD_BussnessLayer;
using DVLD_PresentationLayer.Applications.ApplicationTypes;
using DVLD_PresentationLayer.Applications.International_License;
using DVLD_PresentationLayer.Applications.Local_Driving_License;
using DVLD_PresentationLayer.Applications.Renew_Local_License;
using DVLD_PresentationLayer.Applications.ReplaceLostOrDamagedLicense;
using DVLD_PresentationLayer.Applications.Rlease_Detained_License;
using DVLD_PresentationLayer.Drivers;
using DVLD_PresentationLayer.GlobalClasses;
using DVLD_PresentationLayer.Licenses.Detain_License;
using DVLD_PresentationLayer.Login;
using DVLD_PresentationLayer.People;
using DVLD_PresentationLayer.Tests.TestTypes;
using DVLD_PresentationLayer.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer
{
    public partial class frmMain : Form
    {
        frmLogin _frmLogin;

        public frmMain(frmLogin frm)
        {
            InitializeComponent();
            _frmLogin = frm;
            _ApplyPermissions();

        }

        private void peopleToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            Form f1 = new frmPeopleList();
            f1.ShowDialog();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f1 = new frmListUsers();
            f1.ShowDialog();
        }

        private void currentUserInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUserInfo frm = new frmUserInfo(clsGlobal.CurrentUser.UserID);
            frm.ShowDialog();

        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChangePaasword frm = new frmChangePaasword(clsGlobal.CurrentUser.UserID);
            frm.ShowDialog();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsGlobal.CurrentUser = null;
            _frmLogin.Show();
            this.Close();
        }

      
        private void localLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f1 = new frmAddUpdateLocalDrivingLicesnseApplication();
            f1.ShowDialog();
        }

        private void localDrivingLicenseApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmListLocalDrivingLicesnseApplications frm = new frmListLocalDrivingLicesnseApplications();
            frm.ShowDialog();
        }

        private void retakeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmListLocalDrivingLicesnseApplications frm = new frmListLocalDrivingLicesnseApplications();
            frm.ShowDialog();
        }

        private void renewDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRenewLocalDrivingLicenseApplication frm = new frmRenewLocalDrivingLicenseApplication();
            frm.ShowDialog();
        }

        private void repleacementForLostOrDamageLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReplaceLostOrDamagedLicenseApplication frm = new frmReplaceLostOrDamagedLicenseApplication();
            frm.ShowDialog();
        }

        private void driversToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListDrivers frm = new frmListDrivers();
            frm.ShowDialog();
        }

       

        private void internationalLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmNewInternationalLicenseApplication frm = new frmNewInternationalLicenseApplication();
            frm.ShowDialog();

        }

        private void internationalLicenseApplicalionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListInternationalLicesnseApplications frm = new frmListInternationalLicesnseApplications();
            frm.ShowDialog();
        }

        private void releaseDetainedDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmReleaseDetainedLicenseApplication frm = new frmReleaseDetainedLicenseApplication();
            frm.ShowDialog();
        }

        private void manaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListApplicationTypes f1 = new frmListApplicationTypes();
            f1.ShowDialog();
        }

        private void manageTestTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListTestTypes frm = new frmListTestTypes();
            frm.ShowDialog();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void _ApplyPermissions()
        {
            if (clsGlobal.CurrentUser == null)
                return;

            if (clsGlobal.CurrentUser.RoleInfo == null)
                clsGlobal.CurrentUser.RoleInfo = clsRole.Find(clsGlobal.CurrentUser.RoleID);

            if (clsGlobal.CurrentUser.RoleInfo == null)
                return;

            var role = clsGlobal.CurrentUser.RoleInfo;

            // 1. القوائم الرئيسية في الشريط العلوي
            applicationsToolStripMenuItem.Enabled = (role.CanManageLicensesAndApps || role.CanManageTests);
            peopleToolStripMenuItem.Enabled = (role.CanManageLicensesAndApps || role.CanManageUsersAndSettings);
            driversToolStripMenuItem.Enabled = (role.CanManageLicensesAndApps || role.CanManageVehicles);
            usersToolStripMenuItem.Enabled = role.CanManageUsersAndSettings;
            vehiclesToolStripMenuItem.Enabled = role.CanManageVehicles;
            // 2. القوائم الفرعية داخل Applications (مفعّلة لموظف الرخص والمدير)
            drivingLicensesServicesToolStripMenuItem.Enabled = role.CanManageLicensesAndApps;
            manageApplicationsToolStripMenuItem.Enabled = (role.CanManageLicensesAndApps || role.CanManageTests);
            detainLicensesToolStripMenuItem.Enabled = role.CanManageLicensesAndApps;
            applicationsToolStripMenuItem.DropDownItems[3].Enabled = (role.CanManageLicensesAndApps || role.CanManageUsersAndSettings); // Manage Application Types
            manageTestTypesToolStripMenuItem.Enabled = (role.CanManageLicensesAndApps || role.CanManageUsersAndSettings); // Manage Test Types
        }
    }
}
