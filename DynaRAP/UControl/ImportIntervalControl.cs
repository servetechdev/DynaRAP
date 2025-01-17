﻿using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynaRAP.UControl
{
    public partial class ImportIntervalControl : DevExpress.XtraEditors.XtraUserControl
    {
        public event EventHandler DeleteBtnClicked;

        object min;
        object max;
        int recordCnt;

        public string PartName
        {
            get { return cboType.Text; }
        }

        public object Min
        {
            get { return this.min; }
        }

        public object Max
        {
            get { return this.max; }
        }

        public string EndTime
        {
            get { return edtEndTime.Text; }
        }

        public string Title
        {
            set
            {
            }
        }

        public ImportIntervalControl()
        {
            InitializeComponent();
        }

        public ImportIntervalControl(object min, object max, int recordCnt) : this()
        {
            this.min = min;
            this.max = max;
            this.recordCnt = recordCnt;

            edtStartTime.Text = min.ToString();
            edtEndTime.Text = max.ToString();
            edtCount.Text = recordCnt.ToString();
        }

        private void ImportIntervalControl_Load(object sender, EventArgs e)
        {
            cboType.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            edtStartTime.Enabled = false;
            edtEndTime.Enabled = false;
            InitNameList();
        }

        private void InitNameList()
        {
            string importType = ConfigurationManager.AppSettings["ImportType"];

            string[] types = importType.Split(',');

            cboType.Properties.Items.Clear();
            foreach (string type in types)
            {
                cboType.Properties.Items.Add(type);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.DeleteBtnClicked != null)
                this.DeleteBtnClicked(this, new EventArgs());
        }
    }
}
