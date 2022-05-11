﻿namespace DynaRAP.UControl
{
    partial class SBIntervalControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SBIntervalControl));
            this.edtSbName = new DevExpress.XtraEditors.TextEdit();
            this.edtStartTime = new DevExpress.XtraEditors.TextEdit();
            this.edtEndTime = new DevExpress.XtraEditors.TextEdit();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.edtSbName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // edtSbName
            // 
            this.edtSbName.Location = new System.Drawing.Point(1, 0);
            this.edtSbName.Name = "edtSbName";
            this.edtSbName.Size = new System.Drawing.Size(216, 22);
            this.edtSbName.TabIndex = 0;
            // 
            // edtStartTime
            // 
            this.edtStartTime.Location = new System.Drawing.Point(219, 0);
            this.edtStartTime.Name = "edtStartTime";
            this.edtStartTime.Size = new System.Drawing.Size(168, 22);
            this.edtStartTime.TabIndex = 0;
            // 
            // edtEndTime
            // 
            this.edtEndTime.Location = new System.Drawing.Point(390, 0);
            this.edtEndTime.Name = "edtEndTime";
            this.edtEndTime.Size = new System.Drawing.Size(168, 22);
            this.edtEndTime.TabIndex = 0;
            // 
            // btnDelete
            // 
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDelete.Location = new System.Drawing.Point(544, 1);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(17, 18);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // SBIntervalControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.edtEndTime);
            this.Controls.Add(this.edtStartTime);
            this.Controls.Add(this.edtSbName);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SBIntervalControl";
            this.Size = new System.Drawing.Size(569, 22);
            this.Load += new System.EventHandler(this.SBIntervalControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.edtSbName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit edtSbName;
        private DevExpress.XtraEditors.TextEdit edtStartTime;
        private DevExpress.XtraEditors.TextEdit edtEndTime;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
    }
}
