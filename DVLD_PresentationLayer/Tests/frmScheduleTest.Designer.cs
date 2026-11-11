namespace DVLD_PresentationLayer.Tests
{
    partial class frmScheduleTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClsoe = new System.Windows.Forms.Button();
            this.crlScheduleTest1 = new DVLD_PresentationLayer.Tests.Control.crlScheduleTest();
            this.SuspendLayout();
            // 
            // btnClsoe
            // 
            this.btnClsoe.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnClsoe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClsoe.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClsoe.Image = global::DVLD_PresentationLayer.Properties.Resources.Close_32;
            this.btnClsoe.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClsoe.Location = new System.Drawing.Point(304, 663);
            this.btnClsoe.Name = "btnClsoe";
            this.btnClsoe.Size = new System.Drawing.Size(110, 52);
            this.btnClsoe.TabIndex = 1;
            this.btnClsoe.Text = "Close";
            this.btnClsoe.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClsoe.UseVisualStyleBackColor = false;
            this.btnClsoe.Click += new System.EventHandler(this.btnClsoe_Click);
            // 
            // crlScheduleTest1
            // 
            this.crlScheduleTest1.Location = new System.Drawing.Point(7, -72);
            this.crlScheduleTest1.Name = "crlScheduleTest1";
            this.crlScheduleTest1.Size = new System.Drawing.Size(729, 735);
            this.crlScheduleTest1.TabIndex = 0;
            this.crlScheduleTest1.TestTypeID = DVLD_BussnessLayer.clsTestType.enTestType.VisionTest;
            // 
            // frmScheduleTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 733);
            this.Controls.Add(this.btnClsoe);
            this.Controls.Add(this.crlScheduleTest1);
            this.Name = "frmScheduleTest";
            this.Text = "frmScheduleTest";
            this.Load += new System.EventHandler(this.frmScheduleTest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Control.crlScheduleTest crlScheduleTest1;
        private System.Windows.Forms.Button btnClsoe;
    }
}