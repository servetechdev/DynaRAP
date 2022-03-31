﻿namespace DynaRAP.UControl
{
    partial class SBParamControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SBParamControl));
            this.edtFlying = new DevExpress.XtraEditors.TextEdit();
            this.edtStartTime = new DevExpress.XtraEditors.TextEdit();
            this.edtEndTime = new DevExpress.XtraEditors.TextEdit();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.cboParamType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cboParamName = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.edtFlying.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboParamType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboParamName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // edtFlying
            // 
            this.edtFlying.Location = new System.Drawing.Point(212, 0);
            this.edtFlying.Name = "edtFlying";
            this.edtFlying.Size = new System.Drawing.Size(80, 20);
            this.edtFlying.TabIndex = 0;
            // 
            // edtStartTime
            // 
            this.edtStartTime.Location = new System.Drawing.Point(298, 0);
            this.edtStartTime.Name = "edtStartTime";
            this.edtStartTime.Size = new System.Drawing.Size(80, 20);
            this.edtStartTime.TabIndex = 0;
            // 
            // edtEndTime
            // 
            this.edtEndTime.Location = new System.Drawing.Point(384, 0);
            this.edtEndTime.Name = "edtEndTime";
            this.edtEndTime.Size = new System.Drawing.Size(80, 20);
            this.edtEndTime.TabIndex = 0;
            // 
            // btnDelete
            // 
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.btnDelete.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDelete.Location = new System.Drawing.Point(498, 1);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(17, 17);
            this.btnDelete.TabIndex = 1;
            // 
            // cboParamType
            // 
            this.cboParamType.Location = new System.Drawing.Point(0, 0);
            this.cboParamType.Name = "cboParamType";
            this.cboParamType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboParamType.Size = new System.Drawing.Size(100, 20);
            this.cboParamType.TabIndex = 2;
            // 
            // cboParamName
            // 
            this.cboParamName.Location = new System.Drawing.Point(106, 0);
            this.cboParamName.Name = "cboParamName";
            this.cboParamName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboParamName.Size = new System.Drawing.Size(100, 20);
            this.cboParamName.TabIndex = 3;
            // 
            // SBParamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboParamName);
            this.Controls.Add(this.cboParamType);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.edtEndTime);
            this.Controls.Add(this.edtStartTime);
            this.Controls.Add(this.edtFlying);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SBParamControl";
            this.Size = new System.Drawing.Size(527, 21);
            this.Load += new System.EventHandler(this.SBParamControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.edtFlying.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboParamType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboParamName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit edtFlying;
        private DevExpress.XtraEditors.TextEdit edtStartTime;
        private DevExpress.XtraEditors.TextEdit edtEndTime;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.ComboBoxEdit cboParamType;
        private DevExpress.XtraEditors.ComboBoxEdit cboParamName;
    }
}