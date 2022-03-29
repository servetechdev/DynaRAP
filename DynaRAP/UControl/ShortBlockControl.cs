﻿using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DynaRAP.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DynaRAP.UControl
{
    public partial class ShortBlockControl : DevExpress.XtraEditors.XtraUserControl
    {
        string selectedFuselage = string.Empty;
        Series series1 = new Series();
        ChartArea myChartArea = new ChartArea("LineChartArea");

        public ShortBlockControl()
        {
            InitializeComponent();
        }

        private void ShortBlockControl_Load(object sender, EventArgs e)
        {
            InitializeFlyingList();
            InitializeSBParamComboList();
            InitializeSBParamList();
            InitializeSplittedList();
            InitializePreviewChart();

            //DateTime dtNow = DateTime.Now;
            //string strNow = string.Format("{0:yyyy-MM-dd}", dtNow);
            //dateScenario.Text = strNow;

            panelData.AutoScroll = true;
            panelData.WrapContents = false;
            panelData.HorizontalScroll.Visible = false;
            panelData.VerticalScroll.Visible = true;

            edtSBLength.Properties.AutoHeight = false;
            edtOverlap.Properties.AutoHeight = false;
            edtSBLength.Dock = DockStyle.Fill;
            edtOverlap.Dock = DockStyle.Fill;

            edtSBLength.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            edtSBLength.Properties.Mask.EditMask = @"f2";
            //edtSBLength.Properties.Mask.PlaceHolder = '0';
            //edtSBLength.Properties.Mask.SaveLiteral = true;
            //edtSBLength.Properties.Mask.ShowPlaceHolders = true;
            edtSBLength.Properties.Mask.UseMaskAsDisplayFormat = true;

            edtOverlap.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            edtOverlap.Properties.Mask.EditMask = @"d2";
            edtOverlap.Properties.Mask.UseMaskAsDisplayFormat = true;

        }

        private void InitializePreviewChart()
        {
            ////
            myChartArea.CursorX.IsUserEnabled = true;
            myChartArea.CursorX.IsUserSelectionEnabled = true;
            myChartArea.AxisX.ScaleView.Zoomable = false;
            ////


            chartPreview.BackColor = Color.FromArgb(45, 45, 48);

            chartPreview.ChartAreas.RemoveAt(0);
            chartPreview.ChartAreas.Add(myChartArea);
            chartPreview.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            chartPreview.ChartAreas[0].AxisY.LabelStyle.Enabled = false;


            series1.ChartType = SeriesChartType.Line;
            series1.Name = "VAS";
            series1.XValueType = ChartValueType.DateTime;
            series1.IsValueShownAsLabel = false;
            //series1.IsVisibleInLegend = false;
            series1.LabelForeColor = Color.Red;
            series1.MarkerStyle = MarkerStyle.None;
            series1.MarkerSize = 3;
            series1.MarkerColor = Color.Red;

            SettingMyData();
            chartPreview.Series.Add(series1);
        }

        private void SettingMyData()
        {
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 1, 00), 4);
            Push_Data(series1, new DateTime(2021, 1, 2, 1, 1, 01), 3);
            Push_Data(series1, new DateTime(2021, 1, 3, 1, 1, 02), 5);
            Push_Data(series1, new DateTime(2021, 1, 4, 1, 1, 03), 3);
            Push_Data(series1, new DateTime(2021, 1, 5, 1, 1, 04), 4);
            Push_Data(series1, new DateTime(2021, 1, 6, 1, 1, 05), 2);
            Push_Data(series1, new DateTime(2021, 1, 7, 1, 1, 06), 4);
            Push_Data(series1, new DateTime(2021, 1, 8, 1, 1, 07), 5);
            Push_Data(series1, new DateTime(2021, 1, 9, 1, 1, 08), 4);
            Push_Data(series1, new DateTime(2021, 1, 10, 1, 1, 09), 3);
            Push_Data(series1, new DateTime(2021, 1, 11, 1, 1, 10), 1);
            Push_Data(series1, new DateTime(2021, 1, 12, 1, 1, 11), 3);
            Push_Data(series1, new DateTime(2021, 1, 13, 1, 1, 12), 5);
            Push_Data(series1, new DateTime(2021, 1, 14, 1, 1, 13), 3);
            Push_Data(series1, new DateTime(2021, 1, 15, 1, 1, 14), 4);
            Push_Data(series1, new DateTime(2021, 1, 16, 1, 1, 15), 2);
            Push_Data(series1, new DateTime(2021, 1, 17, 1, 1, 16), 4);
            Push_Data(series1, new DateTime(2021, 1, 18, 1, 1, 17), 5);
            Push_Data(series1, new DateTime(2021, 1, 19, 1, 1, 18), 4);
            Push_Data(series1, new DateTime(2021, 1, 20, 1, 1, 19), 3);
            Push_Data(series1, new DateTime(2021, 1, 21, 1, 1, 20), 1);
            Push_Data(series1, new DateTime(2021, 1, 22, 1, 1, 21), 2);
            Push_Data(series1, new DateTime(2021, 1, 23, 1, 1, 22), 5);
            Push_Data(series1, new DateTime(2021, 1, 24, 1, 1, 23), 3);
            Push_Data(series1, new DateTime(2021, 1, 25, 1, 1, 24), 5);
            Push_Data(series1, new DateTime(2021, 1, 26, 1, 1, 25), 3);
            Push_Data(series1, new DateTime(2021, 1, 27, 1, 1, 26), 4);
            Push_Data(series1, new DateTime(2021, 1, 28, 1, 1, 27), 2);
            Push_Data(series1, new DateTime(2021, 1, 29, 1, 1, 28), 4);
            Push_Data(series1, new DateTime(2021, 1, 30, 1, 1, 29), 5);
            Push_Data(series1, new DateTime(2021, 1, 31, 1, 1, 30), 4);
            Push_Data(series1, new DateTime(2021, 2, 1, 1, 1, 31), 3);
            Push_Data(series1, new DateTime(2021, 2, 2, 1, 1, 32), 5);
            Push_Data(series1, new DateTime(2021, 2, 3, 1, 1, 33), 3);
            Push_Data(series1, new DateTime(2021, 2, 4, 1, 1, 34), 4);
            Push_Data(series1, new DateTime(2021, 2, 5, 1, 1, 35), 2);
            Push_Data(series1, new DateTime(2021, 2, 6, 1, 1, 36), 4);
            Push_Data(series1, new DateTime(2021, 2, 7, 1, 1, 37), 5);
            Push_Data(series1, new DateTime(2021, 2, 8, 1, 1, 38), 4);
            Push_Data(series1, new DateTime(2021, 2, 9, 1, 1, 39), 3);
            Push_Data(series1, new DateTime(2021, 2, 10, 1, 1, 40), 1);
            Push_Data(series1, new DateTime(2021, 2, 11, 1, 1, 41), 3);
            Push_Data(series1, new DateTime(2021, 2, 12, 1, 1, 42), 5);
            Push_Data(series1, new DateTime(2021, 2, 13, 1, 1, 43), 3);
            Push_Data(series1, new DateTime(2021, 2, 14, 1, 1, 44), 4);
            Push_Data(series1, new DateTime(2021, 2, 15, 1, 1, 45), 2);
            Push_Data(series1, new DateTime(2021, 2, 16, 1, 1, 46), 4);
            Push_Data(series1, new DateTime(2021, 2, 17, 1, 1, 47), 5);
            Push_Data(series1, new DateTime(2021, 2, 18, 1, 1, 48), 4);
            Push_Data(series1, new DateTime(2021, 2, 19, 1, 1, 49), 3);
            Push_Data(series1, new DateTime(2021, 2, 20, 1, 1, 50), 1);
            Push_Data(series1, new DateTime(2021, 2, 21, 1, 1, 51), 2);
            Push_Data(series1, new DateTime(2021, 2, 22, 1, 1, 52), 5);
            Push_Data(series1, new DateTime(2021, 2, 23, 1, 1, 53), 3);
            Push_Data(series1, new DateTime(2021, 2, 24, 1, 1, 54), 5);
            Push_Data(series1, new DateTime(2021, 2, 25, 1, 1, 55), 3);
            Push_Data(series1, new DateTime(2021, 2, 26, 1, 1, 56), 4);
            Push_Data(series1, new DateTime(2021, 2, 27, 1, 1, 57), 2);
            Push_Data(series1, new DateTime(2021, 2, 28, 1, 1, 58), 4);

            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 00), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 01), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 02), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 03), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 04), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 05), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 06), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 07), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 08), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 09), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 10), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 11), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 12), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 13), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 14), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 15), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 16), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 17), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 18), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 19), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 20), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 21), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 22), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 23), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 24), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 25), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 26), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 27), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 28), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 29), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 30), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 31), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 32), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 33), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 34), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 35), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 36), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 37), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 38), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 39), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 40), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 41), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 42), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 43), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 44), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 45), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 46), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 47), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 48), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 49), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 50), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 51), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 52), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 53), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 54), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 55), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 56), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 57), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 58), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 2, 59), 5);

            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 00), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 01), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 02), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 03), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 04), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 05), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 06), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 07), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 08), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 09), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 10), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 11), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 12), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 13), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 14), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 15), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 16), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 17), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 18), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 19), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 20), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 21), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 22), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 23), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 24), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 25), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 26), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 27), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 28), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 29), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 30), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 31), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 32), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 33), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 34), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 35), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 36), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 37), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 38), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 39), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 40), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 41), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 42), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 43), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 44), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 45), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 46), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 47), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 48), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 49), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 50), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 51), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 52), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 53), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 54), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 55), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 56), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 57), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 58), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 3, 59), 5);

            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 00), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 01), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 02), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 03), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 04), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 05), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 06), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 07), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 08), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 09), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 10), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 11), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 12), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 13), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 14), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 15), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 16), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 17), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 18), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 19), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 20), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 21), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 22), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 23), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 24), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 25), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 26), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 27), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 28), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 29), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 30), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 31), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 32), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 33), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 34), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 35), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 36), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 37), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 38), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 39), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 40), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 41), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 42), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 43), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 44), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 45), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 46), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 47), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 48), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 49), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 50), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 51), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 52), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 53), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 54), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 55), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 56), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 57), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 58), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 4, 59), 5);

            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 00), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 01), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 02), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 03), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 04), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 05), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 06), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 07), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 08), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 09), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 10), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 11), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 12), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 13), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 14), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 15), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 16), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 17), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 18), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 19), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 20), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 21), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 22), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 23), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 24), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 25), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 26), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 27), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 28), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 29), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 30), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 31), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 32), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 33), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 34), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 35), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 36), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 37), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 38), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 39), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 40), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 41), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 42), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 43), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 44), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 45), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 46), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 47), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 48), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 49), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 50), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 51), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 52), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 53), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 54), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 55), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 56), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 57), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 58), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 5, 59), 5);

            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 00), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 01), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 02), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 03), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 04), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 05), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 06), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 07), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 08), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 09), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 10), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 11), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 12), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 13), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 14), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 15), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 16), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 17), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 18), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 19), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 20), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 21), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 22), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 23), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 24), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 25), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 26), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 27), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 28), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 29), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 30), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 31), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 32), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 33), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 34), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 35), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 36), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 37), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 38), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 39), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 40), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 41), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 42), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 43), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 44), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 45), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 46), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 47), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 48), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 49), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 50), 1);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 51), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 52), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 53), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 54), 5);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 55), 3);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 56), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 57), 2);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 58), 4);
            Push_Data(series1, new DateTime(2021, 1, 1, 1, 6, 59), 5);


        }

        private void Push_Data(Series series, DateTime dt, int data)
        {
            DataPoint dp = new DataPoint(); //데이타 기록하기 정도
            dp.SetValueXY(dt, data);
            series.Points.Add(dp);

        }

        private void InitializeFlyingList()
        {
            cboFlying.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

            cboFlying.SelectedIndexChanged += CboFlying_SelectedIndexChanged;

            cboFlying.Properties.Items.Add("비행분할 #1");
            cboFlying.Properties.Items.Add("비행분할 #2");
            cboFlying.Properties.Items.Add("비행분할 #3");

            cboFlying.SelectedIndex = 0;

        }

        private void CboFlying_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void InitializeSBParamComboList()
        {
            cboSBParameter.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

            cboSBParameter.SelectedIndexChanged += CboSBParameter_SelectedIndexChanged;

            cboSBParameter.Properties.Items.Add("ShortBlock 파라미너 Preset #1");
            cboSBParameter.Properties.Items.Add("ShortBlock 파라미너 Preset #2");
            cboSBParameter.Properties.Items.Add("ShortBlock 파라미너 Preset #3");

            cboSBParameter.SelectedIndex = 0;
        }

        private void CboSBParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void InitializeSBParamList()
        {
            List<SBParameter> list = new List<SBParameter>();

            repositoryItemComboBox1.TextEditStyle = TextEditStyles.DisableTextEditor;
            repositoryItemComboBox1.Items.Add("MACH");
            repositoryItemComboBox1.Items.Add("동압");
            repositoryItemComboBox1.Items.Add("고도");
            repositoryItemComboBox1.Items.Add("AOA");
            repositoryItemComboBox1.Items.Add("AOS");
            repositoryItemComboBox1.Items.Add("NZ");
            repositoryItemComboBox1.Items.Add("ROLL RATE");
            repositoryItemComboBox1.Items.Add("PITCH RATE");

            repositoryItemComboBox2.TextEditStyle = TextEditStyles.DisableTextEditor;
            repositoryItemComboBox2.Items.Add("SW903_NM RATE");
            repositoryItemComboBox2.Items.Add("SW904_NM RATE");
            repositoryItemComboBox2.Items.Add("SW905_NM RATE");
            repositoryItemComboBox2.Items.Add("SW906_NM RATE");

            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("동압", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("고도", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("AOA", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));
            list.Add(new SBParameter("MACH", "SW903_NM", 0, 0, 0, 1));

            this.gridControl1.DataSource = list;

            gridView1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            gridView1.OptionsView.ShowColumnHeaders = true;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ShowIndicator = false;
            gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView1.OptionsView.ColumnAutoWidth = true;

            gridView1.OptionsBehavior.ReadOnly = false;
            //gridView1.OptionsBehavior.Editable = false;

            gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;

            GridColumn colType = gridView1.Columns["ParameterType"];
            colType.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colType.OptionsColumn.FixedWidth = true;
            colType.Width = 120;
            colType.Caption = "파라미터 구분";

            GridColumn colName = gridView1.Columns["ParameterName"];
            colName.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colName.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colName.OptionsColumn.FixedWidth = true;
            colName.Width = 150;
            colName.Caption = "파라미터 이름";

            GridColumn colMin = gridView1.Columns["Min"];
            colMin.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colMin.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colMin.OptionsColumn.FixedWidth = true;
            colMin.Width = 60;
            colMin.Caption = "MIN";
            colMin.OptionsColumn.ReadOnly = true;

            GridColumn colMax = gridView1.Columns["Max"];
            colMax.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colMax.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colMax.OptionsColumn.FixedWidth = true;
            colMax.Width = 60;
            colMax.Caption = "MAX";
            colMax.OptionsColumn.ReadOnly = true;

            GridColumn colAvg = gridView1.Columns["Avg"];
            colAvg.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colAvg.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colAvg.OptionsColumn.FixedWidth = true;
            colAvg.Width = 60;
            colAvg.Caption = "AVG";
            colAvg.OptionsColumn.ReadOnly = true;

            GridColumn colDel = gridView1.Columns["Del"];
            colDel.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colDel.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colDel.OptionsColumn.FixedWidth = true;
            colDel.Width = 40;
            colDel.Caption = "삭제";
            colDel.OptionsColumn.ReadOnly = true;

            this.repositoryItemImageComboBox1.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(0, 0));
            this.repositoryItemImageComboBox1.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(1, 1));

            this.repositoryItemImageComboBox1.GlyphAlignment = HorzAlignment.Center;
            this.repositoryItemImageComboBox1.Buttons[0].Visible = false;

            this.repositoryItemImageComboBox1.Click += RepositoryItemImageComboBox1_Click;

        }

        private void InitializeSplittedList()
        {
            List<SplittedSB> list = new List<SplittedSB>();

            DateTime dtNow = DateTime.Now;
            string strNow = string.Format("{0:HH:mm:ss}", dtNow);

            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));
            list.Add(new SplittedSB("ShortBlock #1", strNow, strNow, 1));

            this.gridControl2.DataSource = list;

            gridView2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            gridView2.OptionsView.ShowColumnHeaders = true;
            gridView2.OptionsView.ShowGroupPanel = false;
            gridView2.OptionsView.ShowIndicator = false;
            gridView2.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView2.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView2.OptionsView.ColumnAutoWidth = true;

            gridView2.OptionsBehavior.ReadOnly = true;
            //gridView2.OptionsBehavior.Editable = false;

            gridView2.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;

            GridColumn colName = gridView2.Columns["SbName"];
            colName.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colName.OptionsColumn.FixedWidth = true;
            colName.Width = 120;
            colName.Caption = "구간이름";

            GridColumn colStartTime = gridView2.Columns["StartTime"];
            colStartTime.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colStartTime.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colStartTime.OptionsColumn.FixedWidth = true;
            colStartTime.Width = 160;
            colStartTime.Caption = "시작시간";

            GridColumn colEndTime = gridView2.Columns["EndTime"];
            colEndTime.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colEndTime.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colEndTime.OptionsColumn.FixedWidth = true;
            colEndTime.Width = 60;
            colEndTime.Caption = "종료시간";


            GridColumn colView = gridView2.Columns["View"];
            colView.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colView.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colView.OptionsColumn.FixedWidth = true;
            colView.Width = 40;
            colView.Caption = "동작";

            this.repositoryItemImageComboBox2.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(0, 0));
            this.repositoryItemImageComboBox2.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(1, 1));

            this.repositoryItemImageComboBox2.GlyphAlignment = HorzAlignment.Center;
            this.repositoryItemImageComboBox2.Buttons[0].Visible = false;

            this.repositoryItemImageComboBox2.Click += RepositoryItemImageComboBox2_Click;

        }

        private void RepositoryItemImageComboBox1_Click(object sender, EventArgs e)
        {
            RepositoryItemImageComboBox combo = sender as RepositoryItemImageComboBox;

        }

        private void RepositoryItemImageComboBox2_Click(object sender, EventArgs e)
        {
            RepositoryItemImageComboBox combo = sender as RepositoryItemImageComboBox;

        }


        private void edtScenarioName_ClearButtonClick(object sender, ButtonPressedEventArgs e)
        {
            //edtScenarioName.Text = String.Empty;

        }


        private void btnViewData_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private void btnViewData_ButtonClick(object sender, EventArgs e)
        {

        }

        private void btnAddParameter_ButtonClick(object sender, EventArgs e)
        {
            AddParameter();
        }

        int index = 23;

        private void AddParameter()
        {
            //ParameterControl ctrl = new ParameterControl();
            //ctrl.Title = "Parameter " + index.ToString();
            //panelData.Controls.Add(ctrl);
            //panelData.Controls.SetChildIndex(ctrl, index++);

        }

        private void btnAddSplittedParameter_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private void btnAddSplittedParameter_ButtonClick(object sender, EventArgs e)
        {

        }


        private void btnSaveSplittedParameter_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private void btnSaveSplittedParameter_ButtonClick(object sender, EventArgs e)
        {

        }

        private void edtSBLength_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            edtSBLength.Text = String.Empty;
        }

        private void edtOverlap_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            edtOverlap.Text = String.Empty;
        }
    }

    
}