﻿using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DynaRAP.Data;
using DynaRAP.Forms;
using DynaRAP.TEST;
using DynaRAP.UTIL;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DynaRAP.UControl
{
    public partial class SBModuleControl : DevExpress.XtraEditors.XtraUserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        ChartArea myChartArea = new ChartArea("LineChartArea");
        //List<SBParamControl> sbParamList = new List<SBParamControl>();
        List<SBIntervalControl> sbIntervalList = new List<SBIntervalControl>();

        Dictionary<string, List<string>> dicData = new Dictionary<string, List<string>>();
        string selParam = String.Empty;
        List<double> chartData = new List<double>();

        List<ResponsePreset> presetList = null;
        List<ResponseParam> paramList = null;
        List<ResponseParam> presetParamList = null;
        List<PresetData> pComboList = null;
        List<PresetParamData> gridList = null;
        ResponsePartInfo partInfo = null;

        DateTime startTime = DateTime.Now;
        DateTime endTime = DateTime.Now;

        //Dictionary<string, List<string>> uploadList = new Dictionary<string, List<string>>();
        List<ResponseImport> uploadList = new List<ResponseImport>();
        List<ResponsePart> partList = new List<ResponsePart>();

        DataTable curDataTable = null;

        double sbLen = 1;
        double overlap = 10;

        string partSeq = String.Empty;
        string partType = String.Empty;
        public SBModuleControl()
        {
            InitializeComponent();

            XmlConfigurator.Configure(new FileInfo("log4net.xml"));
        }

        private void SBModuleControl_Load(object sender, EventArgs e)
        {
            cboFlying.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cboFlying.SelectedIndexChanged += CboFlying_SelectedIndexChanged;

            cboPart.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cboPart.SelectedIndexChanged += CboPart_SelectedIndexChanged;

            cboParameter.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cboParameter.Properties.DropDownRows = 15;
            cboParameter.SelectedIndexChanged += cboParameter_SelectedIndexChanged;

            edtSBLength.Text = "1";
            edtOverlap.Text = "10";
  
            luePresetList.Properties.DisplayMember = "PresetName";
            luePresetList.Properties.ValueMember = "PresetPack";
            luePresetList.Properties.NullText = "";

            uploadList = GetUploadList();
            InitializePreviewChart();
            InitializeFlyingList();
            InitializePresetList();

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
            btnAddParameter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            btnSaveSplittedParameter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            buttonEdit2.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            btnAddParameter.Properties.AllowFocused = false;
            btnSaveSplittedParameter.Properties.AllowFocused = false;
            buttonEdit2.Properties.AllowFocused = false;
            lblValidSBCount.Text = string.Format(Properties.Resources.StringValidSBCount, sbIntervalList.Count);

            paramList = GetParamList();

            InitializeGridControl();
            InitializeTagGridControl();

        }

        private List<ResponseParam> GetParamList()
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlParam"];
                string sendData = @"
            {
            ""command"":""list"",
            ""pageNo"":1,
            ""pageSize"":3000
            }";

                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                ListParamJsonData result = JsonConvert.DeserializeObject<ListParamJsonData>(responseText);

                return result.response;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        private void InitializeGridControl()
        {
            //paramList
            //repositoryItemComboBox1.TextEditStyle = TextEditStyles.DisableTextEditor;
            repositoryItemComboBox1.SelectedIndexChanged += RepositoryItemComboBox1_SelectedIndexChanged;
            repositoryItemComboBox1.BeforePopup += RepositoryItemComboBox1_BeforePopup;
            repositoryItemComboBox1.PopupFormMinSize = new System.Drawing.Size(0, 500);

            foreach (ResponseParam param in paramList)
            {
                repositoryItemComboBox1.Items.Add(param.paramKey);
            }

            //gridView1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            gridView1.OptionsView.ShowColumnHeaders = true;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ShowIndicator = true;
            gridView1.IndicatorWidth = 40;
            gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView1.OptionsView.ColumnAutoWidth = true;

            gridView1.OptionsBehavior.ReadOnly = false;
            //gridView1.OptionsBehavior.Editable = false;

            gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;

            gridView1.CustomDrawRowIndicator += GridView1_CustomDrawRowIndicator;



            GridColumn colName = gridView1.Columns["ParamKey"];
            colName.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colName.OptionsColumn.FixedWidth = true;
            colName.Width = 240;
            colName.Caption = "파라미터 이름";

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
        private void InitializeTagGridControl()
        {
            //gridView2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            gridView2.OptionsView.ShowColumnHeaders = false;
            gridView2.OptionsView.ShowGroupPanel = false;
            gridView2.OptionsView.ShowIndicator = false;
            gridView2.IndicatorWidth = 40;
            gridView2.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView2.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView2.OptionsView.ColumnAutoWidth = true;

            gridView2.OptionsBehavior.ReadOnly = true;
            gridView2.OptionsBehavior.Editable = false;

            gridView2.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView2.OptionsMenu.EnableColumnMenu = false;


        }

        private void RepositoryItemImageComboBox1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.StringDelete, Properties.Resources.StringConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (luePresetList.GetColumnValue("PresetName") == null)
                return;

            string presetPack = String.Empty;
            if (luePresetList.GetColumnValue("PresetPack") != null)
                presetPack = luePresetList.GetColumnValue("PresetPack").ToString();

            if (string.IsNullOrEmpty(presetPack) == false)
            {
                bool bResult = ParamRemove(presetPack);

                if (bResult)
                {
                    gridView1.DeleteRow(gridView1.FocusedRowHandle);
                }
            }
        }

        private bool ParamRemove(string presetPack)
        {
            try
            {
                int i = gridView1.FocusedRowHandle;
                string paramSeq = gridView1.GetRowCellValue(i, "Seq") == null ? "" : gridView1.GetRowCellValue(i, "Seq").ToString();
                string paramPack = gridView1.GetRowCellValue(i, "ParamPack") == null ? "" : gridView1.GetRowCellValue(i, "ParamPack").ToString();

                string url = ConfigurationManager.AppSettings["UrlPreset"];
                string sendData = string.Format(@"
                {{""command"":""param-remove"",
                ""presetPack"":""{0}"",
                ""presetSeq"":"""",
                ""paramPack"":""{1}"",
                ""paramSeq"":""{2}""
                }}"
                , presetPack, paramPack, paramSeq);

                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                JsonData result = JsonConvert.DeserializeObject<JsonData>(responseText);

                if (result != null)
                {
                    if (result.code != 200)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    //this.focusedNodeId = result.response.seq;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        private void GridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
                e.Info.DisplayText = e.RowHandle.ToString();
        }

        int prevSelected = -1;
        private void RepositoryItemComboBox1_BeforePopup(object sender, EventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            prevSelected = combo.SelectedIndex;
        }

        private void RepositoryItemComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo.SelectedIndex != -1)
            {
                string paramKey = combo.SelectedItem as string;
                if (string.IsNullOrEmpty(paramKey) == false)
                {
                    //ResponsePreset preset = presetList.Find(x => x.presetPack.Equals(presetPack));
                    ResponseParam param = paramList.Find(x => x.paramKey.Equals(paramKey));
                    if (param != null)
                    {
                        string adamsKey = param.adamsKey;
                        string zaeroKey = param.zaeroKey;
                        string grtKey = param.grtKey;
                        string fltpKey = param.fltpKey;
                        string fltsKey = param.fltsKey;
                        string partInfo = param.partInfo;
                        string partInfoSub = param.partInfoSub;
                        string seq = param.seq;
                        string paramPack = param.paramPack;

                        bool bFind = false;

                        for (int i = 0; i < gridView1.RowCount; i++)
                        {
                            string adams = gridView1.GetRowCellValue(i, "AdamsKey") == null ? "" : gridView1.GetRowCellValue(i, "AdamsKey").ToString();
                            string zaero = gridView1.GetRowCellValue(i, "ZaeroKey") == null ? "" : gridView1.GetRowCellValue(i, "ZaeroKey").ToString();
                            string grt = gridView1.GetRowCellValue(i, "GrtKey") == null ? "" : gridView1.GetRowCellValue(i, "GrtKey").ToString();
                            string fltp = gridView1.GetRowCellValue(i, "FltpKey") == null ? "" : gridView1.GetRowCellValue(i, "FltpKey").ToString();
                            string flts = gridView1.GetRowCellValue(i, "FltsKey") == null ? "" : gridView1.GetRowCellValue(i, "FltsKey").ToString();
                            //string part1 = gridView1.GetRowCellValue(i, "PartInfo") == null ? "" : gridView1.GetRowCellValue(i, "PartInfo").ToString();
                            //string part2 = gridView1.GetRowCellValue(i, "PartInfoSub") == null ? "" : gridView1.GetRowCellValue(i, "PartInfoSub").ToString();

                            if ((string.IsNullOrEmpty(adams) == false && adams.Equals(adamsKey))
                                || (string.IsNullOrEmpty(zaero) == false && zaero.Equals(zaeroKey))
                                || (string.IsNullOrEmpty(grt) == false && grt.Equals(grtKey))
                                || (string.IsNullOrEmpty(fltp) == false && fltp.Equals(fltpKey))
                                || (string.IsNullOrEmpty(flts) == false && flts.Equals(fltsKey))
                                //|| (string.IsNullOrEmpty(part1) == false && part1.Equals(partInfo))
                                //|| (string.IsNullOrEmpty(part2) == false && part2.Equals(partInfoSub))
                                )
                            {
                                bFind = true;
                                break;
                            }
                        }

                        if (bFind)
                        {
                            MessageBox.Show("항목의 중복이 허용되지 않습니다.");
                            combo.SelectedIndex = prevSelected;
                        }
                        else
                        {
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "AdamsKey", adamsKey);
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "ZaeroKey", zaeroKey);
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "GrtKey", grtKey);
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "FltpKey", fltpKey);
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "FltsKey", fltsKey);
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PartInfo", partInfo);
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PartInfoSub", partInfoSub);
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "Seq", seq);
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "ParamPack", paramPack);
                        }

                    }
                }
            }
        }

        private List<ResponseImport> GetUploadList()
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlImport"];
                string sendData = @"
            {
            ""command"":""upload-list""
            }";

                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                UploadListResponse result = JsonConvert.DeserializeObject<UploadListResponse>(responseText);

                if (result != null)
                {
                    if (result.code != 200)
                    {
                        return null;
                    }
                    else
                    {
                        return result.response;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }
            return null;

        }

        private void InitializePreviewChart()
        {
            myChartArea.BackColor = Color.FromArgb(37, 37, 38);
            myChartArea.AxisX.LabelStyle.ForeColor = Color.White;
            myChartArea.AxisY.LabelStyle.ForeColor = Color.White;
            myChartArea.AxisX.MajorGrid.Enabled = false;
            myChartArea.AxisX.MinorGrid.Enabled = false;
            myChartArea.AxisY.MajorGrid.Enabled = false;
            myChartArea.AxisY.MinorGrid.Enabled = false;

            myChartArea.AxisX.ScrollBar.Enabled = true;
            myChartArea.AxisX.ScaleView.Zoomable = true;
           
            myChartArea.CursorX.IsUserEnabled = true;
            myChartArea.CursorX.AutoScroll = true;
            myChartArea.CursorX.IsUserSelectionEnabled = true;

            myChartArea.AxisY.ScrollBar.Enabled = true;
            myChartArea.AxisY.ScaleView.Zoomable = true;
            myChartArea.CursorY.AutoScroll = true;
            myChartArea.CursorY.IsUserSelectionEnabled = true;

            myChartArea.AxisX.Enabled = AxisEnabled.True;
            myChartArea.AxisX.LabelStyle.Enabled = true;
            //myChartArea.AxisX.Title = "X Axis";

            myChartArea.AxisY.Enabled = AxisEnabled.True;
            myChartArea.AxisY.LabelStyle.Enabled = true;

            myChartArea.AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            myChartArea.AxisX.LabelStyle.Format = "HH:mm:ss.fff";
            myChartArea.AxisX.LabelStyle.Interval = 500;
            //myChartArea.AxisX.LabelStyle.IntervalOffset = 1;

            myChartArea.InnerPlotPosition.Auto = true;
            //myChartArea.InnerPlotPosition.Width = 100;
            //myChartArea.InnerPlotPosition.Height = 100;
            myChartArea.AxisY.IsStartedFromZero = false;

            myChartArea.Position.X = 0;
            myChartArea.Position.Y = 0;
            myChartArea.Position.Width = 100;
            myChartArea.Position.Height = 100;
            
            chart1.ChartAreas.RemoveAt(0);
            chart1.ChartAreas.Add(myChartArea);
            chart1.MouseWheel += Chart1_MouseWheel;
            /*
            chartPreview.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            chartPreview.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
            */
        }

        private void Chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }

        }

        private void InitializeFlyingList()
        {
            cboFlying.Properties.Items.Clear();

            if (uploadList == null)
                return;

            foreach (ResponseImport list in uploadList)
            {
                //Decoding
                byte[] byte64 = Convert.FromBase64String(list.uploadName);
                string decName = Encoding.UTF8.GetString(byte64);
                cboFlying.Properties.Items.Add(decName);
            }

            cboFlying.SelectedIndex = -1;

        }

        private void CboFlying_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit combo = sender as ComboBoxEdit;

            if (combo != null)
            {
                InitializePartList(combo.Text);

                // 프리셋 초기화
                luePresetList.EditValue = "";
                gridControl1.DataSource = null;
                gridView1.RefreshData();

                // 구간 미리보기 초기화
                cboParameter.Properties.Items.Clear();
                cboParameter.Text = String.Empty;

                chart1.Series.Clear();

                chart1.DataSource = null;
                chart1.BackColor = Color.FromArgb(37, 37, 38);
                chart1.DataBind();

                chart1.Update();

                // 분할 예상 구간 초기화
                intervalIndex = START_INT_INDEX;
                flowLayoutPanel2.Height = 22;
                flowLayoutPanel2.Controls.Clear();
           
                sbIntervalList.Clear();
                lblValidSBCount.Text = string.Format(Properties.Resources.StringValidSBCount, sbIntervalList.Count);


                //Encoding
                byte[] basebyte = System.Text.Encoding.UTF8.GetBytes(combo.Text);
                string encName = Convert.ToBase64String(basebyte);

                string uploadSeq = "";
                ResponseImport import = uploadList.Find(x => x.uploadName.Equals(encName));
                if (import != null)
                {
                    uploadSeq = import.seq;
                    string[] tags = GetTagList(uploadSeq);

                    if (tags != null)
                    {
                        DataTable dt = new DataTable();

                        dt.Columns.Add("Tag", typeof(string));

                        foreach (string tag in tags)
                        {
                            dt.Rows.Add(tag);
                        }

                        gridControl2.DataSource = dt;
                        gridView2.RefreshData();
                    }
                }
            }
        }

        private void InitializePartList(string flyingName)
        {
            cboPart.Properties.Items.Clear();
            cboPart.Text = String.Empty;

            partList = null;
            partList = GetPartList(flyingName);

            foreach (ResponsePart part in partList)
            {
                //Decoding
                byte[] byte64 = Convert.FromBase64String(part.partName);
                string decName = Encoding.UTF8.GetString(byte64);

                cboPart.Properties.Items.Add(decName);
            }

            cboPart.SelectedIndex = -1;

        }

        private List<ResponsePart> GetPartList(string flyingName)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlPart"];

                //Encoding
                byte[] basebyte = System.Text.Encoding.UTF8.GetBytes(flyingName);
                string encName = Convert.ToBase64String(basebyte);

                string uploadSeq = "";
                ResponseImport import = uploadList.Find(x => x.uploadName.Equals(encName));
                if (import != null)
                {
                    uploadSeq = import.seq;
                }

                string sendData = string.Format(@"
            {{
            ""command"":""list"",
            ""registerUid"":"""",
            ""uploadSeq"":""{0}"",
            ""pageNo"":1,
            ""pageSize"":3000
            }}"
                , uploadSeq);

                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                PartListResponse result = JsonConvert.DeserializeObject<PartListResponse>(responseText);

                if (result != null)
                {
                    if (result.code != 200)
                    {
                        return null;
                    }
                    else
                    {
                        return result.response;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }
            return null;

        }

        private void CboPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit combo = sender as ComboBoxEdit;

            if (combo != null)
            {
                //ReadCsvFile();
                //InitComboParamList();

                InitializePartInfoList(combo.Text);
            }
        }

        private void InitializePartInfoList(string partName)
        {
            MainForm mainForm = this.ParentForm as MainForm;

            cboParameter.Properties.Items.Clear();
            cboParameter.Text = String.Empty;

            mainForm.ShowSplashScreenManager("ShortBlock 데이터를 불러오는 중입니다. 잠시만 기다려주십시오.");
            partInfo = null;
            partInfo = GetPartInfo(partName);

            if (partInfo != null)
            {
                foreach (ParamSet param in partInfo.paramSet)
                {
                    cboParameter.Properties.Items.Add(param.paramKey);
                }

                cboParameter.SelectedIndex = 0;
            }
            mainForm.HideSplashScreenManager();
        }

        private ResponsePartInfo GetPartInfo(string seq)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlPart"];

                //Encoding
                byte[] basebyte = System.Text.Encoding.UTF8.GetBytes(seq);
                string encName = Convert.ToBase64String(basebyte);

                ResponsePart part = partList.Find(x => x.partName.Equals(encName));
                if (part != null)
                {
                    partSeq = part.seq;
                    partType = part.dataType;
                }

                string sendData = string.Format(@"
            {{
            ""command"":""row-data"",
            ""partSeq"":""{0}"",
            ""julianRange"":["""", """"]
            }}"
                , partSeq);

                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                PartInfoResponse result = JsonConvert.DeserializeObject<PartInfoResponse>(responseText);

                if (result != null)
                {
                    if (result.code != 200)
                    {
                        return null;
                    }
                    else
                    {
                        return result.response;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }
            return null;

        }

        private void ReadCsvFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.InitialDirectory = "C:\\";
            dlg.Filter = "Excel files (*.xls, *.xlsx)|*.xls; *.xlsx|Comma Separated Value files (CSV)|*.csv|모든 파일 (*.*)|*.*";
            //dlg.Filter = "Comma Separated Value files (CSV)|*.csv";

#if !DEBUG
            if (dlg.ShowDialog() == DialogResult.OK)
#endif
            {
#if DEBUG
                StreamReader sr = new StreamReader(@"C:\temp\a.xls");
#else
                StreamReader sr = new StreamReader(dlg.FileName);
#endif

                int idx = 0;

                // 스트림의 끝까지 읽기
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] data = line.Split(',');

                    if (string.IsNullOrEmpty(data[0]))
                        continue;

                    int i = 0;
                    if (idx == 0)
                    {
                        dicData.Clear();
                        for (i = 0; i < data.Length; i++)
                        {
                            if (dicData.ContainsKey(data[i]) == false)
                            {
                                if (string.IsNullOrEmpty(data[i]) == false)
                                    dicData.Add(data[i], new List<string>());
                            }
                        }
                        idx++;
                        continue;
                    }

                    i = 0;
                    foreach (string key in dicData.Keys)
                    {
                        if (dicData.ContainsKey(key))
                        {
                            if (string.IsNullOrEmpty(data[i]) == false)
                                dicData[key].Add(data[i++]);
                        }
                    }
                }


            }
        }

        private void InitComboParamList()
        {
            cboParameter.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cboParameter.Properties.DropDownRows = 15;
            cboParameter.SelectedIndexChanged += cboParameter_SelectedIndexChanged;

            List<string> paramList = dicData.Keys.ToList();
            //cboParameter.Properties.Items.Add("SIN");
            //cboParameter.Properties.Items.Add("COS");
            cboParameter.Properties.Items.AddRange(paramList);
            cboParameter.Properties.Items.Remove("DATE");

            cboParameter.SelectedIndex = -1;
        }

        private void cboParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboParameter.EditValue != null)
            {
                selParam = cboParameter.EditValue.ToString();
                AddChartData(selParam);
            }
        }

        private void AddChartData(string strParam)
        {
            chart1.Series.Clear();

            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series("Series1");
            series1.ChartType = SeriesChartType.Line;
            series1.Name = strParam;
            series1.XValueType = ChartValueType.DateTime;
            series1.IsValueShownAsLabel = false;
            series1.IsVisibleInLegend = false;
            series1.LabelForeColor = Color.Red;

            //series1.MarkerStyle = MarkerStyle.Square;
            //series1.MarkerSize = 3;
            //series1.MarkerColor = Color.Black;

            series1.XValueMember = "Argument";
            series1.YValueMembers = "Value";

            chart1.Series.Add(series1);

            curDataTable = GetChartValues(strParam);
            chart1.DataSource = curDataTable;
            chart1.BackColor = Color.FromArgb(37, 37, 38);
            chart1.DataBind();

            chart1.Update();
            chart1.ChartAreas[0].RecalculateAxesScale();

            AddIntervalList();
            AddStripLines();

        }


        private DataTable GetChartValues(string strParam)
        {
            // Create an empty table.
            DataTable table = new DataTable("Table1");

            // Add two columns to the table.
            //table.Columns.Add("Argument", typeof(Int32));
            table.Columns.Add("Argument", typeof(DateTime));
            table.Columns.Add("Value", typeof(double));
            table.Columns.Add("OffSetTime", typeof(string));

            DataRow row = null;
            int i = 0;
            chartData.Clear();

            for(i = 0; i < partInfo.paramSet.Count; i++)
            {
                if(partInfo.paramSet[i].paramKey.Equals(strParam))
                {
                    int j = 0;
                    foreach(List<double> dataArr in partInfo.data)
                    {
                        row = table.NewRow();
                        string day = partInfo.julianSet[0][j];
                        DateTime dt = Utils.GetDateFromJulian(day);

                        if (j == 0)
                        {
                            this.startTime = dt;    
                        }
                        this.endTime = dt;

                        double data = dataArr[i];
                        chartData.Add(data);
                        row["Argument"] = dt;
                        //row["Argument"] = i;
                        row["Value"] = data;

                        row["OffSetTime"] = day;
                        table.Rows.Add(row);

                        j++;
                    }
                    break;
                }
            }
            Console.WriteLine(string.Format("StartTime : {0}, EndTime : {1}", string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", startTime), string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", endTime)));

            return table;
        }

        private void AddIntervalList()
        {
            sbIntervalList.Clear();

            intervalIndex = START_INT_INDEX;
            int reducedHeight = (PARAM_HEIGHT * flowLayoutPanel2.Controls.Count);
            flowLayoutPanel2.Height -= reducedHeight;
            flowLayoutPanel2.Controls.Clear();

            double.TryParse(edtSBLength.Text, out sbLen);
            double.TryParse(edtOverlap.Text, out overlap);

            overlap *= 0.01;

            //sbLen = 0.1;//test

            DateTime t1 = startTime;
            DateTime t2 = t1.AddSeconds(sbLen);
            int i = 0;
            while (t1 < endTime)
            {
                //Console.WriteLine(i + string.Format(" - StartTime : {0}, EndTime : {1}", string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", t1), string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", t2)));
                if(t2 > endTime)
                {
                    t2 = endTime;
                    t1 = t2.AddSeconds(-sbLen);
                    if(t1 < startTime)
                    {
                        t1 = startTime;
                    }
                    if (partType == "zaero")
                    {
                        var temp1 = Utils.GetZaeroJulianFromDate(t1);
                        var temp2 = Utils.GetZaeroJulianFromDate(t2);
                        SplittedSB sb3 = new SplittedSB(string.Format("ShortBlock#{0}", i),temp1, temp2, 0);
                        i++;
                        AddSplittedInterval(sb3);
                        break;
                    }
                    SplittedSB sb2 = new SplittedSB(string.Format("ShortBlock#{0}", i), string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", t1), string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", t2), 0);
                    i++;

                    AddSplittedInterval(sb2);
                    break;
                }
                SplittedSB sb = new SplittedSB(string.Format("ShortBlock#{0}", i), string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", t1), string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", t2), 0);
                i++;

                AddSplittedInterval(sb);

                t1 = t2.AddSeconds(-(sbLen * overlap));
                t2 = t1.AddSeconds(sbLen);
            }

            lblValidSBCount.Text = string.Format(Properties.Resources.StringValidSBCount, sbIntervalList.Count);

        }
        private void AddStripLines()
        {
            double.TryParse(edtSBLength.Text, out sbLen);
            double.TryParse(edtOverlap.Text, out overlap);

            //sbLen *= 0.00001;
            //sbLen *= 0.1;
            overlap *= 0.01;

            if (sbLen <= 0 || overlap <= 0)
                return;

            //Axis ax = chart1.ChartAreas[0].AxisX;
            System.Windows.Forms.DataVisualization.Charting.Axis ax = myChartArea.AxisX;
            List<Color> colors = new List<Color>()  {   Color.FromArgb(75, 44, 44), Color.FromArgb(98, 41, 41)
                                                        , Color.FromArgb(64, Color.LightSeaGreen), Color.FromArgb(64, Color.LightGoldenrodYellow)};

            double hrange = ax.Maximum - ax.Minimum;

            if (double.IsNaN(hrange))
                return;

            TimeSpan spanStart = new TimeSpan(startTime.Day, startTime.Hour, startTime.Minute, startTime.Second, startTime.Millisecond);
            TimeSpan spanEnd = new TimeSpan(endTime.Day, endTime.Hour, endTime.Minute, endTime.Second, endTime.Millisecond);
            TimeSpan spanGap = spanEnd.Subtract(spanStart);

            sbLen = sbLen * hrange / spanGap.TotalSeconds;

            ax.StripLines.Clear();

            StripLine sl = new StripLine();
            sl.Interval = hrange;
            sl.StripWidth = hrange;            // width, 너비
            sl.IntervalOffset = 0;  // x-position, 시작점
            sl.BackColor = colors[0];
            ax.StripLines.Add(sl);

            double offset = sbLen * (1 - overlap);
            //double startTime = 0;
            while (offset < hrange)
            {
                if(offset + sbLen > hrange)
                {
                    StripLine sl3 = new StripLine();
                    sl3.Interval = hrange;
                    sl3.IntervalOffset = hrange - sbLen;    // 시작점
                    sl3.StripWidth = sbLen * overlap;   // 너비
                    sl3.BackColor = colors[1];
                    ax.StripLines.Add(sl3);
                    break;
                }
                StripLine sl2 = new StripLine();
                sl2.Interval = hrange;
                sl2.IntervalOffset = offset;    // 시작점
                sl2.StripWidth = sbLen * overlap;   // 너비
                sl2.BackColor = colors[1];
                ax.StripLines.Add(sl2);

                //AddSplittedInterval(new SplittedSB("", startTime, endTime, 0));
                //Console.WriteLine(string.Format("starttime : {0}, endtime : {1}", string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", startTime), string.Format("{0:yyyy-MM-dd hh:mm:ss.ffffff}", endTime)));
                
                offset += sbLen*(1 - overlap);
            }
        }

        private void InitializePresetList()
        {
            luePresetList.Properties.DataSource = null;

            presetList = GetPresetList();
            pComboList = new List<PresetData>();

            foreach (ResponsePreset list in presetList)
            {
                //Decoding
                byte[] byte64 = Convert.FromBase64String(list.presetName);
                string decName = Encoding.UTF8.GetString(byte64);

                pComboList.Add(new PresetData(decName, list.presetPack));
            }
            luePresetList.Properties.DataSource = pComboList;
#if !DEBUG
            luePresetList.Properties.PopulateColumns();
            luePresetList.Properties.ShowHeader = false;
            luePresetList.Properties.Columns["PresetPack"].Visible = false;
            luePresetList.Properties.ShowFooter = false;
#else
            luePresetList.Properties.PopulateColumns();
            luePresetList.Properties.Columns["PresetName"].Width = 800;
#endif

            //luePresetList.EditValue = edtParamName.Text;
            luePresetList.EditValue = "";
            gridControl1.DataSource = null;
            gridView1.RefreshData();
        }

        private List<ResponsePreset> GetPresetList()
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlPreset"];
                string sendData = @"
            {
            ""command"":""list"",
            ""pageNo"":1,
            ""pageSize"":3000
            }";

                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                ListPresetJsonData result = JsonConvert.DeserializeObject<ListPresetJsonData>(responseText);

                return result.response;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        private void CboSBParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        const int START_PARAM_INDEX = 0;
        const int MAX_PARAM_CNT = 10;
        const int PARAM_HEIGHT = 22;
        int paramIndex = START_PARAM_INDEX;

        //private void AddParameter(ResponseParam param)
        //{
        //    SBParamControl ctrl = new SBParamControl(param);
        //    //ctrl.Title = "Parameter " + (paramIndex- startParamIndex).ToString();
        //    ctrl.DeleteBtnClicked += new EventHandler(SBParam_DeleteBtnClicked);
        //    flowLayoutPanel1.Controls.Add(ctrl);
        //    flowLayoutPanel1.Controls.SetChildIndex(ctrl, paramIndex++);
        //    sbParamList.Add(ctrl);

        //    if (paramIndex <= MAX_PARAM_CNT)
        //    {
        //        flowLayoutPanel1.Height += PARAM_HEIGHT;
        //    }
        //}

        //private void SBParam_DeleteBtnClicked(object sender, EventArgs e)
        //{
        //    SBParamControl ctrl = sender as SBParamControl;
        //    flowLayoutPanel1.Controls.Remove(ctrl);
        //    sbParamList.Remove(ctrl);
        //    ctrl.Dispose();

        //    paramIndex--;
        //    if (paramIndex <= MAX_PARAM_CNT)
        //    {
        //        flowLayoutPanel1.Height -= PARAM_HEIGHT;
        //    }
        //}

        const int START_INT_INDEX = 0;
        const int MAX_INT_CNT = 10;
        int intervalIndex = START_INT_INDEX;

        private void AddSplittedInterval(SplittedSB sb)
        {
            SBIntervalControl ctrl = new SBIntervalControl(sb, curDataTable);
            //ctrl.ViewBtnClicked += new EventHandler(SB_ViewBtnClicked);
            flowLayoutPanel2.Controls.Add(ctrl);
            flowLayoutPanel2.Controls.SetChildIndex(ctrl, intervalIndex);
            sbIntervalList.Add(ctrl);

            intervalIndex++;
            if (intervalIndex <= MAX_INT_CNT)
            {
                flowLayoutPanel2.Height += PARAM_HEIGHT;
            }
        }

        void SB_ViewBtnClicked(object sender, EventArgs e)
        {
            SBIntervalControl ctrl = sender as SBIntervalControl;

            string strKey = selParam;
            DateTime sTime = DateTime.ParseExact(ctrl.Sb.StartTime, "yyyy-MM-dd HH:mm:ss.ffffff", null);
            DateTime eTime = DateTime.ParseExact(ctrl.Sb.EndTime, "yyyy-MM-dd HH:mm:ss.ffffff", null);

            DataTable dt = GetShortBlockData(ctrl.Sb.SbName, sTime, eTime);

            if (dt != null)
            {
                SBViewForm form = new SBViewForm(dt);
                form.Text = strKey;
                form.Show();
            }

            //TestChartForm2 form2 = new TestChartForm2(dt);
            //form2.Text = strKey;
            //form2.Show();

        }

        private void PanelChart_ClosedPanel(object sender, DockPanelEventArgs e)
        {
            throw new NotImplementedException();
        }

        private DataTable GetShortBlockData(string strKey, DateTime sTime, DateTime eTime)
        {
            //string t1 = Utils.GetJulianFromDate(sTime);
            //string t2 = Utils.GetJulianFromDate(eTime);

            DataRow[] result = curDataTable.Select(String.Format("Argument >= #{0}# AND Argument <= #{1}#", sTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), eTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff")));

            DataTable table = new DataTable("Table1");
            table.Columns.Add("Argument", typeof(DateTime));
            table.Columns.Add("Value", typeof(double));

            foreach (DataRow row in result)
            {
                table.ImportRow(row);
            }
            return table;

        }
        
        private DataTable GetShortBlockData2(string strKey, DateTime sTime, DateTime eTime)
        {
            // Create an empty table.
            DataTable table = new DataTable("Table1");

            // Add two columns to the table.
            //table.Columns.Add("Argument", typeof(Int32));
            table.Columns.Add("Argument", typeof(DateTime));
            table.Columns.Add("Value", typeof(double));

            DataRow row = null;
            int i = 0;
            chartData.Clear();
            foreach (string value in dicData[strKey])
            {
                row = table.NewRow();
                string day = dicData["DATE"][i];
                DateTime dt = Utils.GetDateFromJulian(day);

                int result1 = DateTime.Compare(dt, sTime);
                int result2 = DateTime.Compare(dt, eTime);

                if(result1 < 0 || result2 > 0)
                {
                    continue;
                }

                double data = double.Parse(value);
                chartData.Add(data);
                row["Argument"] = dt;
                //row["Argument"] = i;
                row["Value"] = data;
                table.Rows.Add(row);
                i++;
            }

            return table;
        }

        void InvalidSB_DeleteBtnClicked(object sender, EventArgs e)
        {
            SBIntervalControl ctrl = sender as SBIntervalControl;
            flowLayoutPanel2.Controls.Remove(ctrl);
            sbIntervalList.Remove(ctrl);
            ctrl.Dispose();

            intervalIndex--;

            lblValidSBCount.Text = string.Format(Properties.Resources.StringValidSBCount, sbIntervalList.Count);
        }

        private void btnSaveSplittedParameter_ButtonClick(object sender, EventArgs e)
        {
            CreateShortBlock();
        }

        private bool CreateShortBlock()
        {
            try
            {
                double.TryParse(edtSBLength.Text, out sbLen);
                double.TryParse(edtOverlap.Text, out overlap);

                CreateShortBlockRequest req = new CreateShortBlockRequest();
                req.command = "create-shortblock";
                req.blockMetaSeq = "";
                req.partSeq = this.partSeq;
                req.sliceTime = sbLen;
                req.overlap = overlap;

                //string presetPack = string.Empty;
                //string presetSeq = string.Empty;
                //if (luePresetList.GetColumnValue("PresetPack") != null)
                //{
                //    presetPack = luePresetList.GetColumnValue("PresetPack").ToString();
                //    ResponsePreset preset = presetList.Find(x => x.presetPack.Equals(presetPack));

                //    if (preset != null)
                //    {
                //        presetSeq = preset.seq;
                //    }
                //}

                //req.presetPack = presetPack;
                //req.presetSeq = presetSeq;

                req.parameters = new List<Parameter>();
                int i = 0;

                for (i = 0; i < gridView1.RowCount; i++)
                {
                    string paramKey = gridView1.GetRowCellValue(i, "ParamKey") == null ? "" : gridView1.GetRowCellValue(i, "ParamKey").ToString();
                    string adams = gridView1.GetRowCellValue(i, "AdamsKey") == null ? "" : gridView1.GetRowCellValue(i, "AdamsKey").ToString();
                    string zaero = gridView1.GetRowCellValue(i, "ZaeroKey") == null ? "" : gridView1.GetRowCellValue(i, "ZaeroKey").ToString();
                    string grt = gridView1.GetRowCellValue(i, "GrtKey") == null ? "" : gridView1.GetRowCellValue(i, "GrtKey").ToString();
                    string fltp = gridView1.GetRowCellValue(i, "FltpKey") == null ? "" : gridView1.GetRowCellValue(i, "FltpKey").ToString();
                    string flts = gridView1.GetRowCellValue(i, "FltsKey") == null ? "" : gridView1.GetRowCellValue(i, "FltsKey").ToString();
                    string propSeq = gridView1.GetRowCellValue(i, "PropSeq") == null ? "" : gridView1.GetRowCellValue(i, "PropSeq").ToString();
                    //string partInfoSub = gridView1.GetRowCellValue(i, "PartInfoSub") == null ? "" : gridView1.GetRowCellValue(i, "PartInfoSub").ToString();
                    string paramSeq = gridView1.GetRowCellValue(i, "Seq") == null ? "" : gridView1.GetRowCellValue(i, "Seq").ToString();
                    string paramPack = gridView1.GetRowCellValue(i, "ParamPack") == null ? "" : gridView1.GetRowCellValue(i, "ParamPack").ToString();
                
                    Parameter param = new Parameter();
                    param.paramSeq = paramSeq;
                    param.paramPack = paramPack;
                    param.paramKey = paramKey;
                    param.adamsKey = adams;
                    param.zaeroKey = zaero;
                    param.grtKey = grt;
                    param.fltpKey = fltp;
                    param.fltsKey = flts;
                    param.propSeq = propSeq;
               
                    req.parameters.Add(param);
                }

                req.shortBlocks = new List<ShortBlock>();
                i = 1;
                foreach (SBIntervalControl ctrl in sbIntervalList)
                {
                    ShortBlock sb = new ShortBlock();
                    SplittedSB splitSb = ctrl.Sb;
                    sb.blockNo = i++;
                    sb.blockName = splitSb.SbName;
                    if (partType != "zaero")
                    {
                        sb.julianStartAt = Utils.GetJulianFromDate(splitSb.StartTime);
                        sb.julianEndAt = Utils.GetJulianFromDate(splitSb.EndTime);
                    }
                    else
                    {
                        sb.julianStartAt = splitSb.StartTime;
                        sb.julianEndAt = splitSb.EndTime;

                    }
                    req.shortBlocks.Add(sb);

                }

                req.forcedCreate = true;

                string url = ConfigurationManager.AppSettings["UrlPart"];

                var json = JsonConvert.SerializeObject(req);

                //Console.WriteLine(json);
                log.Info("url : " + url);
                log.Info(json);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                CreateShortBlockResponse result = JsonConvert.DeserializeObject<CreateShortBlockResponse>(responseText);

                if (result != null)
                {
                    if (result.code != 200)
                    {
                        return false;
                    }
                    else
                    {
                        //MessageBox.Show("Success");
                        CreateSBProgressForm form = new CreateSBProgressForm(result.response.seq);
                        form.ShowDialog();
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;

        }

        private void edtSBLength_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            edtSBLength.Text = String.Empty;
        }

        private void edtOverlap_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            edtOverlap.Text = String.Empty;
        }

        private void edtSBLength_EditValueChanged(object sender, EventArgs e)
        {
            //AddIntervalList();
            //AddStripLines();
        }

        private void edtOverlap_EditValueChanged(object sender, EventArgs e)
        {
            //AddIntervalList();
            //AddStripLines();
        }

        private void btnAddParameter_ButtonClick(object sender, EventArgs e)
        {
            if (gridList == null)
            {
                gridList = new List<PresetParamData>();
            }
            gridList.Add(new PresetParamData("", "", "", "", "", "", "", "", "", "", "", 1));
            this.gridControl1.DataSource = gridList;
            //gridControl1.Update();
            gridView1.RefreshData();
        }

        private void luePresetList_EditValueChanged(object sender, EventArgs e)
        {
            gridList = null;
            gridControl1.DataSource = null;

            paramIndex = START_PARAM_INDEX;

            string presetPack = String.Empty;
            if (luePresetList.GetColumnValue("PresetPack") != null)
                presetPack = luePresetList.GetColumnValue("PresetPack").ToString();

            presetParamList = null;
            ResponsePreset preset = presetList.Find(x => x.presetPack.Equals(presetPack));

            string presetName = String.Empty;

            if (preset != null)
            {
                //Decoding
                byte[] byte64 = Convert.FromBase64String(preset.presetName);
                string decName = Encoding.UTF8.GetString(byte64);

                presetName = decName;

                presetParamList = GetPresetParamList(preset.presetPack);
            }

            if (presetParamList != null)
            {
                gridList = new List<PresetParamData>();
                foreach (ResponseParam param in presetParamList)
                {
                    //AddParameter(param);
                    gridList.Add(new PresetParamData(param.paramKey, param.adamsKey, param.zaeroKey, param.grtKey, param.fltpKey, param.fltsKey, param.propInfo.propType, param.partInfo, param.seq, param.propInfo.seq, param.paramPack, 1));
                }

                this.gridControl1.DataSource = gridList;
            }
            gridView1.RefreshData();
        }

        private List<ResponseParam> GetPresetParamList(string presetPack)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlPreset"];
                string sendData = string.Format(@"
            {{
            ""command"":""param-list"",
            ""presetPack"":""{0}"",
            ""presetSeq"":"""",
            ""paramPack"":"""",
            ""paramSeq"":"""",
            ""pageNo"":1,
            ""pageSize"":3000
            }}"
                , presetPack);

                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                ListParamJsonData result = JsonConvert.DeserializeObject<ListParamJsonData>(responseText);

                return result.response;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        Point? prevPosition = null;
        ToolTip tooltip = new ToolTip();
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos;
            var results = chart1.HitTest(pos.X, pos.Y, false,
                                            ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop = result.Object as DataPoint;
                    if (prop != null)
                    {
                        var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                        var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                        // check if the cursor is really close to the point (2 pixels around the point)
                        if (Math.Abs(pos.X - pointXPixel) < 2 &&
                            Math.Abs(pos.Y - pointYPixel) < 2)
                        {
                            DateTime dt1 = DateTime.FromOADate(prop.XValue);
                            tooltip.Show("X=" + dt1 + ", Y=" + prop.YValues[0], this.chart1,
                                            pos.X, pos.Y - 15);
                            //Console.WriteLine(string.Format("X = {0}", prop.XValue));
                            //Console.WriteLine(string.Format("X-time = {0}", dt1));
                        }
                    }
                }
            }
        }

        private void edtSBLength_Leave(object sender, EventArgs e)
        {
            AddIntervalList();
            AddStripLines();
        }

        private void edtOverlap_Leave(object sender, EventArgs e)
        {
            AddIntervalList();
            AddStripLines();
        }




        private string[] GetTagList(string uploadSeq)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlDataProp"];

                string sendData = string.Format(@"
                {{
                ""command"":""list"",
                ""referenceType"": ""upload"",
                ""referenceKey"": ""{0}""
                }}"
                , uploadSeq);

                log.Info("url : " + url);
                log.Info(sendData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 30 * 1000;
                //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

                // POST할 데이타를 Request Stream에 쓴다
                byte[] bytes = Encoding.ASCII.GetBytes(sendData);
                request.ContentLength = bytes.Length; // 바이트수 지정

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                //Console.WriteLine(responseText);
                DataPropResponse result = JsonConvert.DeserializeObject<DataPropResponse>(responseText);

                if (result != null)
                {
                    if (result.code != 200)
                    {
                    }
                    else
                    {
                        foreach (ResponseDataProp data in result.response)
                        {
                            // 서버에서는 List<ResponseDataProp> 로 주지만 실제로는 값이 하나임.
                            // 첫 데이터만 이용하고 빠져나감.
                            //Decoding
                            byte[] byte64 = Convert.FromBase64String(data.propValue);
                            string decName = Encoding.UTF8.GetString(byte64);
                            string[] tagArr = decName.Split(',');
                            return tagArr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }
            return null;
        }

        private void buttonEdit2_Click(object sender, EventArgs e)
        {
            if (sbIntervalList == null || sbIntervalList.Count == 0)
            {
                MessageBox.Show("변경할 ShortBlock이 없습니다. 다시 확인해주세요.");
                return;
            }
            int i = 0;
            foreach (SBIntervalControl sbInterval in sbIntervalList)
            {
                if (string.IsNullOrEmpty(changeName.Text))
                {
                    sbInterval.Sb.SbName = string.Format("ShortBlock#{1}", changeName.Text, i);
                    sbInterval.Title = string.Format("ShortBlock#{1}", changeName.Text, i);
                }
                else
                {
                    sbInterval.Sb.SbName = string.Format("{0}_ShortBlock#{1}", changeName.Text, i);
                    sbInterval.Title = string.Format("{0}_ShortBlock#{1}", changeName.Text, i);
                }
                i++;
            }
        }

        private void changeName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            changeName.Text = String.Empty;
        }
    }


    


}
