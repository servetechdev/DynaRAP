﻿using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DynaRAP.Common;
using DynaRAP.Data;
using DynaRAP.EventData;
using DynaRAP.UTIL;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynaRAP.UControl
{
    public partial class ImportModuleControl : DevExpress.XtraEditors.XtraUserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string selectedFuselage = string.Empty;
        Dictionary<string, List<string>> dicData = new Dictionary<string, List<string>>();
        List<ImportParamControl> paramControlList = new List<ImportParamControl>();
        List<ResponseParam> paramList = new List<ResponseParam>();
        //List<ImportIntervalControl> splitList = new List<ImportIntervalControl>();
        List<ResponsePreset> presetList = null;
        List<PresetData> pComboList = null;
        List<ImportIntervalData> intervalList = null;
        string csvFilePath = string.Empty;
        object minValue = null;
        object maxValue = null;
        string headerRow = string.Empty;

        ImportType importType = ImportType.FLYING;
        string uploadSeq = string.Empty;

        public ImportModuleControl()
        {
            InitializeComponent();

            XmlConfigurator.Configure(new FileInfo("log4net.xml"));
        }

        public ImportModuleControl(ImportType importType) : this()
        {
            this.importType = importType;
        }

        private void ImportModuleControl_Load(object sender, EventArgs e)
        {
            cboImportType.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

            if (importType == ImportType.FLYING)
            {
                cboImportType.Properties.Items.Add("GRT");
                cboImportType.Properties.Items.Add("FLTP");
                cboImportType.Properties.Items.Add("FLTS");
                gridControl1.Height = 560;
                tableLayoutPanel6.Visible = false;
            }
            else
            {
                cboImportType.Properties.Items.Add("ADAMS");
                cboImportType.Properties.Items.Add("ZAERO");
                InitializeGridControl3();
            }

            //InitializeSplittedRegionList();

            luePresetList.Properties.DisplayMember = "PresetName";
            luePresetList.Properties.ValueMember = "PresetPack";
            luePresetList.Properties.NullText = "";

            InitializePresetList();

            DateTime dtNow = DateTime.Now;
            string strNow = string.Format("{0:yyyy-MM-dd}", dtNow);
            //dateScenario.Text = strNow;

            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.HorizontalScroll.Visible = false;
            flowLayoutPanel1.VerticalScroll.Visible = true;

            btnViewData.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            btnAddParameter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            btnAddSplittedInterval.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            btnSaveSplittedInterval.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            btnViewData.Properties.AllowFocused = false;
            btnAddParameter.Properties.AllowFocused = false;
            btnAddSplittedInterval.Properties.AllowFocused = false;
            btnSaveSplittedInterval.Properties.AllowFocused = false;

            btnAddParameter.Enabled = false;

            if (intervalList == null)
            {
                intervalList = new List<ImportIntervalData>();
            }
            lblSplitCount.Text = string.Format(Properties.Resources.StringSplitCount, intervalList.Count);
            InitializeGridControl1();
            InitializeGridControl2();

            edtLPFn.Text = "10";
            edtLPFcutoff.Text = "0.4";
            cboLPFbtype.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cboLPFbtype.Properties.Items.Add("");
            cboLPFbtype.Properties.Items.Add("low");
            cboLPFbtype.Properties.Items.Add("high");
            cboLPFbtype.SelectedIndex = 1;

            edtHPFn.Text = "10";
            edtHPFcutoff.Text = "0.02";
            cboHPFbtype.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cboHPFbtype.Properties.Items.Add("");
            cboHPFbtype.Properties.Items.Add("low");
            cboHPFbtype.Properties.Items.Add("high");
            cboHPFbtype.SelectedIndex = 2;

            btn_downloadAll.Visible = false;
        }

        private void InitializeGridControl1()
        {

            //gridView1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            repositoryItemComboBox1.TextEditStyle = TextEditStyles.DisableTextEditor;
            repositoryItemComboBox1.SelectedIndexChanged += RepositoryItemComboBox1_SelectedIndexChanged;
            repositoryItemComboBox1.BeforePopup += RepositoryItemComboBox1_BeforePopup;
            repositoryItemComboBox1.PopupFormMinSize = new System.Drawing.Size(0, 500);

            paramList = GetParamList();

            if (paramList == null)
                return;

            repositoryItemComboBox1.Items.Clear();
            repositoryItemComboBox1.Items.Add("skip");
            foreach (ResponseParam param in paramList)
            {
                repositoryItemComboBox1.Items.Add(param.paramKey);
            }

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

            GridColumn colName = gridView1.Columns["UnmappedParamName"];
            //colName.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            //colName.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colName.OptionsColumn.FixedWidth = true;
            colName.Width = 240;
            colName.Caption = "ParamName";
            colName.OptionsColumn.ReadOnly = true;
        }

        private void InitializeGridControl3()
        {


            gridView3.OptionsView.ShowColumnHeaders = true;
            gridView3.OptionsView.ShowGroupPanel = false;
            gridView3.OptionsView.ShowIndicator = true;
            gridView3.IndicatorWidth = 40;
            gridView3.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView3.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView3.OptionsView.ColumnAutoWidth = true;

            gridView3.OptionsBehavior.ReadOnly = false;

            gridView3.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            gridView3.OptionsSelection.EnableAppearanceFocusedCell = false;

        }

        private List<ResponseParam> GetParamList()
        {
            ListParamJsonData result = null;

            try
            {
                string url = ConfigurationManager.AppSettings["UrlParam"];
                //string sendData = @"
                //{
                //""command"":""list"",
                //""pageNo"":1,
                //""pageSize"":3000,
                //""resultDataType"": ""map""
                //}";
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
                result = JsonConvert.DeserializeObject<ListParamJsonData>(responseText);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }

            return result.response;

        }

        private void InitializeGridControl2()
        {

            //gridView2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            repositoryItemComboBox2.TextEditStyle = TextEditStyles.DisableTextEditor;
            repositoryItemComboBox2.SelectedIndexChanged += RepositoryItemComboBox2_SelectedIndexChanged;
            repositoryItemComboBox2.BeforePopup += RepositoryItemComboBox2_BeforePopup;

            //string importType = ConfigurationManager.AppSettings["ImportType"];

            //string[] types = importType.Split(',');

            repositoryItemComboBox2.Items.Clear();
            GetFlyingType();

            gridView2.OptionsView.ShowColumnHeaders = true;
            gridView2.OptionsView.ShowGroupPanel = false;
            gridView2.OptionsView.ShowIndicator = false;
            gridView2.IndicatorWidth = 40;
            gridView2.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView2.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView2.OptionsView.ColumnAutoWidth = true;

            gridView2.OptionsBehavior.ReadOnly = false;
            //gridView2.OptionsBehavior.Editable = false;

            gridView2.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;

            gridView2.CustomDrawRowIndicator += GridView2_CustomDrawRowIndicator;

            GridColumn colType = gridView2.Columns["ImportType"];
            colType.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colType.OptionsColumn.FixedWidth = true;
            colType.Width = 240;
            colType.Caption = "기동 이름";

            GridColumn colDel = gridView2.Columns["Del"];
            colDel.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colDel.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colDel.OptionsColumn.FixedWidth = true;
            colDel.Width = 40;
            colDel.Caption = "삭제";
            colDel.OptionsColumn.ReadOnly = true;

            GridColumn colDownloadAll = gridView2.Columns["Download_ALL"];
            colDownloadAll.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadAll.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadAll.OptionsColumn.FixedWidth = true;
            colDownloadAll.Width = 80;
            colDownloadAll.Caption = "CSV_All";
            colDownloadAll.OptionsColumn.ReadOnly = true;
            colDownloadAll.Visible = false;

            GridColumn colDownloadRaw = gridView2.Columns["Download_RAW"];
            colDownloadRaw.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadRaw.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadRaw.OptionsColumn.FixedWidth = true;
            colDownloadRaw.Width = 80;
            colDownloadRaw.Caption = "CSV_RAW";
            colDownloadRaw.OptionsColumn.ReadOnly = true;
            colDownloadRaw.Visible = false;

            GridColumn colDownloadLPF = gridView2.Columns["Download_LPF"];
            colDownloadLPF.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadLPF.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadLPF.OptionsColumn.FixedWidth = true;
            colDownloadLPF.Width = 80;
            colDownloadLPF.Caption = "CSV_LPF";
            colDownloadLPF.OptionsColumn.ReadOnly = true;
            colDownloadLPF.Visible = false;

            GridColumn colDownloadHPF = gridView2.Columns["Download_HPF"];
            colDownloadHPF.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadHPF.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadHPF.OptionsColumn.FixedWidth = true;
            colDownloadHPF.Width = 80;
            colDownloadHPF.Caption = "CSV_HPF";
            colDownloadHPF.OptionsColumn.ReadOnly = true;
            colDownloadHPF.Visible = false;

            GridColumn colDownloadBPF = gridView2.Columns["Download_BPF"];
            colDownloadBPF.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadBPF.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            colDownloadBPF.OptionsColumn.FixedWidth = true;
            colDownloadBPF.Width = 80;
            colDownloadBPF.Caption = "CSV_BPF";
            colDownloadBPF.OptionsColumn.ReadOnly = true;
            colDownloadBPF.Visible = false;

            this.repositoryItemImageComboBox1.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(0, 0));
            this.repositoryItemImageComboBox1.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(1, 1));

            this.repositoryItemImageComboBox1.GlyphAlignment = HorzAlignment.Center;
            this.repositoryItemImageComboBox1.Buttons[0].Visible = false;

            this.repositoryItemImageComboBox1.Click += RepositoryItemImageComboBox1_Click;


            this.repositoryItemImageComboBox2.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(0, 0));
            this.repositoryItemImageComboBox2.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(1, 1));

            this.repositoryItemImageComboBox2.GlyphAlignment = HorzAlignment.Center;
            this.repositoryItemImageComboBox2.Buttons[0].Visible = false;

            this.repositoryItemImageComboBox2.Click += RepositoryItemImageComboBox2_Click;

            this.repositoryItemImageComboBox3.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(0, 0));
            this.repositoryItemImageComboBox3.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(1, 1));

            this.repositoryItemImageComboBox3.GlyphAlignment = HorzAlignment.Center;
            this.repositoryItemImageComboBox3.Buttons[0].Visible = false;

            this.repositoryItemImageComboBox3.Click += RepositoryItemImageComboBox3_Click;

            this.repositoryItemImageComboBox4.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(0, 0));
            this.repositoryItemImageComboBox4.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(1, 1));

            this.repositoryItemImageComboBox4.GlyphAlignment = HorzAlignment.Center;
            this.repositoryItemImageComboBox4.Buttons[0].Visible = false;

            this.repositoryItemImageComboBox4.Click += RepositoryItemImageComboBox4_Click;


            this.repositoryItemImageComboBox5.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(0, 0));
            this.repositoryItemImageComboBox5.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(1, 1));

            this.repositoryItemImageComboBox5.GlyphAlignment = HorzAlignment.Center;
            this.repositoryItemImageComboBox5.Buttons[0].Visible = false;

            this.repositoryItemImageComboBox5.Click += RepositoryItemImageComboBox5_Click;

            this.repositoryItemImageComboBox6.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(0, 0));
            this.repositoryItemImageComboBox6.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(1, 1));

            this.repositoryItemImageComboBox6.GlyphAlignment = HorzAlignment.Center;
            this.repositoryItemImageComboBox6.Buttons[0].Visible = false;

            this.repositoryItemImageComboBox6.Click += RepositoryItemImageComboBox6_Click;
        }

        private void RepositoryItemImageComboBox1_Click(object sender, EventArgs e)
        {
            gridView2.DeleteRow(gridView2.FocusedRowHandle);
            lblSplitCount.Text = string.Format(Properties.Resources.StringSplitCount, intervalList.Count);
        }
        private void RepositoryItemImageComboBox2_Click(object sender, EventArgs e)
        {
            int row = gridView2.FocusedRowHandle;
            string importType = gridView2.GetRowCellValue(row, "ImportType") == null ? "" : gridView2.GetRowCellValue(row, "ImportType").ToString();
            string partSeq = gridView2.GetRowCellValue(row, "Seq") == null ? "" : gridView2.GetRowCellValue(row, "Seq").ToString();
            string partName = gridView2.GetRowCellValue(row, "SplitName") == null ? "" : gridView2.GetRowCellValue(row, "SplitName").ToString();

            GetPartData(partName, importType, partSeq, "A", true);
        }
        private void RepositoryItemImageComboBox3_Click(object sender, EventArgs e)
        {
            int row = gridView2.FocusedRowHandle;
            string importType = gridView2.GetRowCellValue(row, "ImportType") == null ? "" : gridView2.GetRowCellValue(row, "ImportType").ToString();
            string partSeq = gridView2.GetRowCellValue(row, "Seq") == null ? "" : gridView2.GetRowCellValue(row, "Seq").ToString();
            string partName = gridView2.GetRowCellValue(row, "SplitName") == null ? "" : gridView2.GetRowCellValue(row, "SplitName").ToString();

            GetPartData(partName, importType, partSeq, "N", true);
        }

        private void RepositoryItemImageComboBox4_Click(object sender, EventArgs e)
        {
            int row = gridView2.FocusedRowHandle;
            string importType = gridView2.GetRowCellValue(row, "ImportType") == null ? "" : gridView2.GetRowCellValue(row, "ImportType").ToString();
            string partSeq = gridView2.GetRowCellValue(row, "Seq") == null ? "" : gridView2.GetRowCellValue(row, "Seq").ToString();
            string partName = gridView2.GetRowCellValue(row, "SplitName") == null ? "" : gridView2.GetRowCellValue(row, "SplitName").ToString();

            GetPartData(partName, importType, partSeq, "L", true);
        }

        private void RepositoryItemImageComboBox5_Click(object sender, EventArgs e)
        {
            int row = gridView2.FocusedRowHandle;
            string importType = gridView2.GetRowCellValue(row, "ImportType") == null ? "" : gridView2.GetRowCellValue(row, "ImportType").ToString();
            string partSeq = gridView2.GetRowCellValue(row, "Seq") == null ? "" : gridView2.GetRowCellValue(row, "Seq").ToString();
            string partName = gridView2.GetRowCellValue(row, "SplitName") == null ? "" : gridView2.GetRowCellValue(row, "SplitName").ToString();

            GetPartData(partName, importType, partSeq, "H", true);
        }
        private void RepositoryItemImageComboBox6_Click(object sender, EventArgs e)
        {
            int row = gridView2.FocusedRowHandle;
            string importType = gridView2.GetRowCellValue(row, "ImportType") == null ? "" : gridView2.GetRowCellValue(row, "ImportType").ToString();
            string partSeq = gridView2.GetRowCellValue(row, "Seq") == null ? "" : gridView2.GetRowCellValue(row, "Seq").ToString();
            string partName = gridView2.GetRowCellValue(row, "SplitName") == null ? "" : gridView2.GetRowCellValue(row, "SplitName").ToString();

            GetPartData(partName, importType, partSeq, "B", true);
        }

        private void RepositoryItemComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo.SelectedIndex != -1)
            {
                string paramKey = combo.SelectedItem as string;
                if (string.IsNullOrEmpty(paramKey) == false)
                {
                    bool bFind = false;

                    for (int i = 0; i < gridView1.RowCount; i++)
                    {
                        string paramKey2 = gridView1.GetRowCellValue(i, "ParamKey") == null ? "" : gridView1.GetRowCellValue(i, "ParamKey").ToString();

                        if (i == gridView1.FocusedRowHandle || paramKey.Equals("skip"))
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(paramKey2) == false && paramKey2.Equals("skip") == false && paramKey2.Equals(paramKey))
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

                }
            }
        }

        int prevSelected = -1;
        private void RepositoryItemComboBox1_BeforePopup(object sender, EventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            prevSelected = combo.SelectedIndex;
        }

        private void RepositoryItemComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void RepositoryItemComboBox2_BeforePopup(object sender, EventArgs e)
        {
        }

        private void GridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
                e.Info.DisplayText = e.RowHandle.ToString();
        }

        private void GridView2_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
                e.Info.DisplayText = e.RowHandle.ToString();
        }

        public void GetFlyingType()
        {
            string sendData = string.Format(@"
                {{
                ""command"":""flight-type""
                }}");
            string responseData = Utils.GetPostData(ConfigurationManager.AppSettings["UrlImport"], sendData);
            if (responseData != null)
            {
                FlyingTpyeResponse flyingTpyeResponse = JsonConvert.DeserializeObject<FlyingTpyeResponse>(responseData);
                foreach (var list in flyingTpyeResponse.response)
                {
                    repositoryItemComboBox2.Items.Add(Utils.base64StringDecoding(list.typeName));
                }
            }
        }
        private void InitializePresetList()
        {
            try
            {
                luePresetList.Properties.DataSource = null;

                presetList = GetPresetList();

                if (presetList != null)
                {
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
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

            }
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
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        private void btnViewData_ButtonClick(object sender, EventArgs e)
        {
            if (File.Exists(csvFilePath) == false)
            {
                MessageBox.Show(Properties.Resources.FileNotExist);
                return;
            }

            MainForm mainForm = this.ParentForm as MainForm;
            mainForm.ShowSplashScreenManager("데이터 파일을 불러오는 중입니다.. 잠시만 기다려주십시오.");
            mainForm.PanelImportViewCsv.Show();
            mainForm.CsvTableControl.CsvFilePath = this.csvFilePath;
            mainForm.CsvTableControl.ImportType = this.importType;
           
            mainForm.CsvTableControl.FillGrid();
            mainForm.HideSplashScreenManager();
        }


        private void btnAddParameter_ButtonClick(object sender, EventArgs e)
        {
            AddParameter();
        }

        const int START_PARAM_INDEX = 0;
        const int PARAM_HEIGHT = 140;
        const int MAX_CHART_CNT = 3;
        int paramIndex = START_PARAM_INDEX;

        private void AddParameter()
        {
            ImportParamControl ctrl = new ImportParamControl();
            ctrl.Title = "Parameter " + paramIndex.ToString();
            ctrl.DeleteBtnClicked += new EventHandler(ImportParamControl_DeleteBtnClicked);
            ctrl.DicData = dicData;
            ctrl.OnSelectedRange += ChartControl_OnSelectedRange;
            //ctrl.Dock = DockStyle.Fill;
            flowLayoutPanel3.Controls.Add(ctrl);
            flowLayoutPanel3.Controls.SetChildIndex(ctrl, paramIndex);
            paramControlList.Add(ctrl);

            paramIndex++;
            if (paramIndex <= MAX_CHART_CNT)
            {
                flowLayoutPanel3.Height += PARAM_HEIGHT;
            }
        }

        void ImportParamControl_DeleteBtnClicked(object sender, EventArgs e)
        {
            ImportParamControl ctrl = sender as ImportParamControl;
            flowLayoutPanel3.Controls.Remove(ctrl);
            paramControlList.Remove(ctrl);
            ctrl.Dispose();

            paramIndex--;
            if (paramIndex < MAX_CHART_CNT)
            {
                flowLayoutPanel3.Height -= PARAM_HEIGHT;
            }
        }

        private void GetPartData(string partName, string importType, string partSeq, string filterType, bool bDownload = false)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlPart"];

                if (bDownload)
                {
                    url += "/d";
                }

                string sendData = string.Format(@"
                {{
                ""command"":""row-data"",
                ""partSeq"":""{0}"",
                ""julianRange"":["""", """"],
                ""filterType"": ""{1}""
                }}"
                , partSeq , filterType);

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

                //// Response 처리
                //string responseText = string.Empty;
                //using (WebResponse resp = request.GetResponse())
                //{
                //    Stream respStream = resp.GetResponseStream();
                //    using (StreamReader sr = new StreamReader(respStream))
                //    {
                //        responseText = sr.ReadToEnd();
                //    }
                //}

                SaveFileDialog dlg = new SaveFileDialog();
                string flyingName = lblFlyingData.Text;
                if (flyingName.IndexOf(".") != -1)
                {
                    flyingName = flyingName.Substring(0, flyingName.LastIndexOf("."));
                }
                if (flyingName.IndexOf("\\") != -1)
                {
                    flyingName = flyingName.Substring(flyingName.LastIndexOf("\\")+1);
                }
                string fileTypeName = null;
                switch (filterType)
                {
                    case "N":
                        fileTypeName = "RAW";
                        break;
                    case "L":
                        fileTypeName = "LPF";
                        break;
                    case "H":
                        fileTypeName = "HPF";
                        break;
                    case "B":
                        fileTypeName = "BPF";
                        break;
                    case "A":
                        fileTypeName = "ALL";
                        break;

                }
                dlg.FileName = string.Format("{0}_{1}_{2}_{3}", flyingName, importType, partName, fileTypeName);
                if (filterType == "A" || filterType == "Z")
                {
                    if (filterType == "Z")
                    {
                        dlg.FileName = string.Format("{0}_{1}", flyingName, importType);
                    }
                    dlg.Filter = "ZIP Folders(.ZIP)| *.zip";
                }
                else
                {
                    dlg.Filter = "Comma Separated Value files (CSV)|*.csv";
                }
                dlg.Title = "Save a CSV File";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dlg.FileName;

                    var buff = new byte[1024];
                    int pos = 0;
                    int count;
                    if (filterType == "A" || filterType == "Z")
                    {
                        using (WebResponse resp = request.GetResponse())
                        {
                            using (Stream respStream = resp.GetResponseStream())
                            {
                                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                                {
                                    do
                                    {
                                        count = respStream.Read(buff, pos, buff.Length);
                                        fs.Write(buff, 0, count);
                                    } while (count > 0);

                                    fs.Close();
                                }
                            }
                        }
                    }
                    else
                    {
                        string responseText = string.Empty;
                        using (WebResponse resp = request.GetResponse())
                        {
                            Stream respStream = resp.GetResponseStream();
                            using (StreamReader sr = new StreamReader(respStream))
                            {
                                responseText = sr.ReadToEnd();
                            }
                        }

                        FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                        sw.WriteLine(responseText);
                        sw.Close();
                        fs.Close();
                    }

                    //string fileName = dlg.FileName;

                    //FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                    //StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    //sw.WriteLine(responseText);
                    //sw.Close();
                    //fs.Close();

                }

                //Console.WriteLine(responseText);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }

        }
        private void ChartControl_OnSelectedRange(object sender, SelectedRangeEventArgs e)
        {
            ImportParamControl me = sender as ImportParamControl;
            minValue = e.MinValue;
            maxValue = e.MaxValue;

            Debug.Print(string.Format("-----> MinValue : {0}, MaxValue : {1}", minValue, maxValue));

            foreach (ImportParamControl ctrl in paramControlList)
            {
                if (ctrl == me)
                    continue;

                ctrl.SelectRegion(minValue, maxValue);
            }
        }

        private void btnAddSplittedInterval_ButtonClick(object sender, EventArgs e)
        {
            AddSplittedInterval();

            lblSplitCount.Text = string.Format(Properties.Resources.StringSplitCount, intervalList.Count);
        }

        const int START_SPLIT_INDEX = 0;
        const int SPLIT_HEIGHT = 24;
        const int MAX_SPLIT_CNT = 10;
        int intervalIndex = START_SPLIT_INDEX;

        private void AddSplittedInterval()
        {
            if (minValue == null || maxValue == null)
            {
                if (paramControlList.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.StringAddParameter);
                    return;
                }
                else
                {
                    ImportParamControl paramCtrl = paramControlList[0] as ImportParamControl;
                    paramCtrl.Sync();
                }
            }

            if (minValue == null || maxValue == null)
            {
                MessageBox.Show(Properties.Resources.StringNoSelectedRegion);
                return;
            }

            DataTable dt = null;
            int recordCnt = 0;

            if (paramControlList != null && paramControlList.Count > 0)
            {
                //DateTime sTime = Convert.ToDateTime(minValue.ToString());
                //DateTime eTime = Convert.ToDateTime(maxValue.ToString());

                DateTime sTime = (DateTime)minValue;
                DateTime eTime = (DateTime)maxValue;

                dt = GetIntervalData(paramControlList[0].Dt, sTime, eTime);
                recordCnt = dt.Rows.Count;
            }

            /*ImportIntervalControl ctrl = new ImportIntervalControl(minValue, maxValue, recordCnt);
            //ctrl.Title = "flight#" + (paramIndex + intervalIndex).ToString();
            ctrl.DeleteBtnClicked += new EventHandler(Interval_DeleteBtnClicked);
            flowLayoutPanel4.Controls.Add(ctrl);
            flowLayoutPanel4.Controls.SetChildIndex(ctrl, intervalIndex);
            splitList.Add(ctrl);

            intervalIndex++;
            if (intervalIndex <= MAX_SPLIT_CNT)
            {
                flowLayoutPanel4.Height += SPLIT_HEIGHT;
            }*/
            if (intervalList == null)
            {
                intervalList = new List<ImportIntervalData>();
            }
            DateTime min = (DateTime)minValue;
            DateTime max = (DateTime)maxValue;

            intervalList.Add(new ImportIntervalData("", "", min.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), max.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), recordCnt.ToString(), 1, 1, 1 ,1 ,1 ,1));
            this.gridControl2.DataSource = intervalList;
            gridView2.RefreshData();

        }

        private DataTable GetIntervalData(DataTable curDataTable, DateTime sTime, DateTime eTime)
        {
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

        void Interval_DeleteBtnClicked(object sender, EventArgs e)
        {
            /*ImportIntervalControl ctrl = sender as ImportIntervalControl;
            flowLayoutPanel1.Controls.Remove(ctrl);
            splitList.Remove(ctrl);
            ctrl.Dispose();

            intervalIndex--;
            if (intervalIndex <= MAX_SPLIT_CNT)
            {
                flowLayoutPanel4.Height -= SPLIT_HEIGHT;
            }

            lblSplitCount.Text = string.Format(Properties.Resources.StringSplitCount, splitList.Count);*/
        }

        private void btnSaveSplittedInterval_ButtonClick(object sender, EventArgs e)
        {
            if (cboImportType.SelectedIndex < 0)
            {
                MessageBox.Show("입력타입을 선택하세요.");
                return;
            }

            if (luePresetList.GetColumnValue("PresetPack") == null)
            {
                MessageBox.Show("매칭테이블을 선택하세요.");
                return;
            }

            if (gridView2.RowCount <= 0)
            {
                MessageBox.Show("분할 구간이 없습니다.");
                return;
            }

            for (int i = 0; i < gridView2.RowCount; i++)
            {
                string startTime = gridView2.GetRowCellValue(i, "StartTime") == null ? "" : gridView2.GetRowCellValue(i, "StartTime").ToString();
                string endTime = gridView2.GetRowCellValue(i, "EndTime") == null ? "" : gridView2.GetRowCellValue(i, "EndTime").ToString();
                DateTime startDateTime = DateTime.Parse(startTime);
                DateTime endDateTime = DateTime.Parse(endTime);
                for (int j = 0; j < gridView2.RowCount; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    string nextStartTime = gridView2.GetRowCellValue(j, "StartTime") == null ? "" : gridView2.GetRowCellValue(j, "StartTime").ToString();
                    string nextEndTime = gridView2.GetRowCellValue(j, "EndTime") == null ? "" : gridView2.GetRowCellValue(j, "EndTime").ToString();
                    DateTime nextStartDateTime = DateTime.Parse(nextStartTime);
                    DateTime nextEndDateTime = DateTime.Parse(nextEndTime);
                    if((startDateTime <= nextStartDateTime && endDateTime > nextStartDateTime) || (startDateTime > nextEndDateTime && endDateTime <= nextEndDateTime))
                    {
                        MessageBox.Show("분할 구간이 겹쳐져 있는 구간이 있습니다.");
                        return;
                    }
                }
            }

            bool bResult = Import();

            if (bResult)
            {
                //MessageBox.Show(Properties.Resources.StringSuccessImport, Properties.Resources.StringSuccess, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                AddDataProp();
            }
        }

        private bool Import()
        {
            try
            {
                ImportRequest import = new ImportRequest();
                import.command = "upload";
                import.sourcePath = csvFilePath;
                import.flightAt = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                import.dataType = cboImportType.Text.ToLower();
                import.forcedImport = chkForcedImport.Checked;
                import.qEquation = edtEquation.Text;
                import.lpfOption = new LpfOption();
                import.hpfOption = new HpfOption();
                import.tempMappingParams = new Dictionary<string, string>();
                import.parts = new List<Part>();

                string presetPack = String.Empty;
                if (luePresetList.GetColumnValue("PresetPack") != null)
                    presetPack = luePresetList.GetColumnValue("PresetPack").ToString();

                import.presetPack = presetPack;

                import.lpfOption.n = edtLPFn.Text;
                import.lpfOption.cutoff = edtLPFcutoff.Text;
                import.lpfOption.btype = cboLPFbtype.Text;

                import.hpfOption.n = edtHPFn.Text;
                import.hpfOption.cutoff = edtHPFcutoff.Text;
                import.hpfOption.btype = cboHPFbtype.Text;

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    string paramName = gridView1.GetRowCellValue(i, "UnmappedParamName") == null ? "" : gridView1.GetRowCellValue(i, "UnmappedParamName").ToString();
                    string paramKey = gridView1.GetRowCellValue(i, "ParamKey") == null ? "skip" : gridView1.GetRowCellValue(i, "ParamKey").ToString();
                    if (string.IsNullOrEmpty(paramKey))
                    {
                        paramKey = "skip";
                    }
                    if (import.tempMappingParams.ContainsKey(paramName) == false)
                        import.tempMappingParams.Add(paramName, paramKey);
                }

                for (int i = 0; i < gridView2.RowCount; i++)
                {
                    string splitName = gridView2.GetRowCellValue(i, "SplitName") == null ? "" : gridView2.GetRowCellValue(i, "SplitName").ToString();
                    string startTime = gridView2.GetRowCellValue(i, "StartTime") == null ? "" : gridView2.GetRowCellValue(i, "StartTime").ToString();
                    string endTime = gridView2.GetRowCellValue(i, "EndTime") == null ? "" : gridView2.GetRowCellValue(i, "EndTime").ToString();
                    //Encoding
                    byte[] basebyte = System.Text.Encoding.UTF8.GetBytes(splitName);
                    string partName = Convert.ToBase64String(basebyte);

                    string t1 = string.Empty;
                    string t2 = string.Empty;
                    string t3 = string.Empty;
                    string t4 = string.Empty;
                    if (importType == ImportType.FLYING)
                    {
                        t1 = Utils.GetJulianFromDate(startTime);
                        t2 = Utils.GetJulianFromDate(endTime);
                    }
                    else
                    {
                        t3 = Utils.GetJulianFromDate(startTime);
                        t4 = Utils.GetJulianFromDate(endTime);

                        t3 = t3.Substring(t3.Length - 9, 9);
                        t4 = t4.Substring(t4.Length - 9, 9);
                    }

                    import.parts.Add(new Part(partName, t1, t2, t3, t4));
                }

                string url = ConfigurationManager.AppSettings["UrlImport"];

                var json = JsonConvert.SerializeObject(import);

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
                ImportResponse result = JsonConvert.DeserializeObject<ImportResponse>(responseText);

                if (result != null)
                {
                    if (result.code != 200)
                    {
                        uploadSeq = String.Empty;
                        log.Error(result.message);
                        MessageBox.Show(result.message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        // progress 확인
                        //ImportProgressForm form = new ImportProgressForm("116a1460354a7065cb1393aa94a529e14221be82a5bae3bbccc8b1a5b6b59680"); // test

                        uploadSeq = result.response.seq;

                        ImportProgressForm form = new ImportProgressForm(uploadSeq);
                        if (form.ShowDialog() == DialogResult.Cancel)
                        {
                            List<UnmappedParamData> unmappedList = new List<UnmappedParamData>();
                            foreach (string type in form.NotMappedParams)
                            {
                                unmappedList.Add(new UnmappedParamData(type, "skip", "Unmapped"));
                            }
                            this.gridControl1.DataSource = unmappedList;
                            gridView1.RefreshData();
                        }
                        ChangeGridContorl2();
                    }
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

        private void ChangeGridContorl2()
        {
            List<ResponsePart> partLst = GetPartList(uploadSeq);
            foreach (var part in partLst)
            {
                string partName = Utils.base64StringDecoding(part.partName);
                intervalList.Find(x => x.SplitName == partName).Seq = part.seq;
            }
            this.gridControl2.DataSource = intervalList;


            GridColumn colDel = gridView2.Columns["Del"];
            colDel.VisibleIndex = -1;

            GridColumn colDownloadAll = gridView2.Columns["Download_ALL"];
            colDownloadAll.Visible = true;

            GridColumn colDownloadRaw = gridView2.Columns["Download_RAW"];
            colDownloadRaw.Visible = true;

            GridColumn colDownloadLPF = gridView2.Columns["Download_LPF"];
            colDownloadLPF.Visible = true;

            GridColumn colDownloadHPF = gridView2.Columns["Download_HPF"];
            colDownloadHPF.Visible = true;

            GridColumn colDownloadBPF = gridView2.Columns["Download_BPF"];
            colDownloadBPF.Visible = true;

            GridColumn colType = gridView2.Columns["ImportType"];
            colType.Width = 100;

            //btn_downloadAll.Visible = true;
            btnAddSplittedInterval.Visible = false;
        }
        private List<ResponsePart> GetPartList(string uploadSeq)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlPart"];


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

        private void AddDataProp()
        {
            try
            {

                if (string.IsNullOrEmpty(this.uploadSeq))
                {
                    return;
                }

                //Encoding
                byte[] basebyte = System.Text.Encoding.UTF8.GetBytes("tags");
                string encName = Convert.ToBase64String(basebyte);

                string tagValues = string.Empty;
                foreach (ButtonEdit btn in this.panelTag.Controls)
                {
                    tagValues += btn.Text + ",";
                }
                if (tagValues != null && tagValues != "" && tagValues.LastIndexOf(",") != -1)
                {
                    tagValues = tagValues.Substring(0, tagValues.LastIndexOf(","));
                }
                //Encoding
                byte[] basebyte2 = System.Text.Encoding.UTF8.GetBytes(tagValues);
                string encValue = Convert.ToBase64String(basebyte2);

                string url = ConfigurationManager.AppSettings["UrlDataProp"];
                string sendData = string.Format(@"
                {{
                ""command"":""add"",
                ""referenceType"": ""upload"",
                ""referenceKey"": ""{0}"",
                ""propName"": ""{1}"",
                ""propValue"": ""{2}""
                }}"
                    , this.uploadSeq, encName, encValue);

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
                        log.Error(result.message);
                        MessageBox.Show(result.message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
                return;
            }

            return;
        }

        private void lblFlyingData_Click(object sender, EventArgs e)
        {
            MainForm mainForm = this.ParentForm as MainForm;
            btnAddParameter.Enabled = false;
            headerRow = string.Empty;

            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.InitialDirectory = "C:\\";
            //dlg.Filter = "Excel files (*.xls, *.xlsx)|*.xls; *.xlsx|Comma Separated Value files (CSV)|*.csv|모든 파일 (*.*)|*.*";
            //dlg.Filter = "Comma Separated Value files (CSV)|*.csv";

#if !DEBUG
            if (dlg.ShowDialog() == DialogResult.OK)
#endif
            {
                mainForm.ShowSplashScreenManager("선택한 데이터 파일을 불러오는 중 입니다.. 잠시만 기다려주십시오.");
#if DEBUG
                // screen init
                luePresetList.EditValue = "";

                gridControl1.DataSource = null;
                gridView1.RefreshData();

                gridControl2.DataSource = null;
                gridView2.RefreshData();
                intervalList.Clear();
                lblSplitCount.Text = string.Format(Properties.Resources.StringSplitCount, intervalList.Count);

                paramIndex = START_PARAM_INDEX;
                flowLayoutPanel3.Height -= PARAM_HEIGHT * flowLayoutPanel3.Controls.Count;
                flowLayoutPanel3.Controls.Clear();

                edtTag.Text = "";
                panelTag.Controls.Clear();

                // screen init

                if (importType == ImportType.FLYING)
                {
                   
                    //csvFilePath = @"C:\temp\a.xls";
                    csvFilePath = @"C:\temp\xfa2_0003_test1208.csv";
                    //csvFilePath = @"C:\temp\XFA1_0001_1(원본).csv";
                    //csvFilePath = @"C:\temp\XFA1_1107.csv";
                    lblFlyingData.Text = csvFilePath;
                }
                else
                {
                    //csvFilePath = @"C:\temp\ANAYSIS_ZAERO_LOADMOD_WING_RH_220816.dat";
                    //csvFilePath = @"C:\temp\anaysis_zaero_loadmod_wing_rh_220816.dat";
                    csvFilePath = @"C:\temp\ANAYSIS_ZAERO_LD_LI212A5R2_M06_00k_abc001_AtoA_MD.dat";
                    
                    lblFlyingData.Text = csvFilePath;
                }
                StreamReader sr = new StreamReader(csvFilePath);
#else
                csvFilePath = dlg.FileName;
                lblFlyingData.Text = csvFilePath;
                StreamReader sr = new StreamReader(dlg.FileName);
#endif

                if (importType == ImportType.FLYING) // 비행데이터 import
                {
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
                            this.headerRow = line;
                            //headerRow = headerRow.Substring(0, headerRow.LastIndexOf(','));
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
                else // 해석데이터 import
                {
                    dicData.Clear();

                    Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

                    // 스트림의 끝까지 읽기
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        line = line.Trim();
                        string[] data = line.Split(' ');

                        if (string.IsNullOrEmpty(data[0]))
                            continue;

                        double dVal;
                        bool isNumber = double.TryParse(data[0], out dVal);
                        int i = 0;

                        if (isNumber == false)
                        {
                            foreach (string key in tempData.Keys)
                            {
                                if (dicData.ContainsKey(key) == false)
                                {
                                    dicData.Add(key, tempData[key]);
                                }
                            }

                            if (data[0].Equals("UNITS"))
                            {
                                tempData.Clear();

                                if (tempData.ContainsKey("DATE") == false)
                                {
                                    tempData.Add("DATE", new List<string>());
                                }
                                for (i = 1; i < data.Length; i++)
                                {
                                    if (tempData.ContainsKey(data[i]) == false)
                                    {
                                        if (string.IsNullOrEmpty(data[i]) == false)
                                        {
                                            tempData.Add(data[i], new List<string>());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            data = data.Where((source, index) => string.IsNullOrEmpty(source) == false).ToArray();

                            if (data[0].StartsWith("-"))
                            {
                                continue;
                            }

                            i = 0;
                            foreach (string key in tempData.Keys)
                            {
                                if (tempData.ContainsKey(key))
                                {
                                    if (string.IsNullOrEmpty(data[i]) == false)
                                        tempData[key].Add(data[i++]);
                                }
                            }
                        }
                    }
                }

                //foreach (KeyValuePair<string, List<string>> kv in dicData)
                //{
                //    Console.Write("{0} : ", kv.Key);
                //    foreach (string val in kv.Value)
                //    {
                //        Console.Write("{0} ", val);
                //    }
                //    Console.WriteLine();
                //}

                btnAddParameter.Enabled = true;

                CheckParam();
                mainForm.HideSplashScreenManager();
            }
        }

        private bool CheckParam()
        {
            try
            {
                string presetPack = String.Empty;
                string dataType = cboImportType.Text.ToLower();

                if (luePresetList.GetColumnValue("PresetPack") != null)
                    presetPack = luePresetList.GetColumnValue("PresetPack").ToString();

                if (string.IsNullOrEmpty(presetPack)
                    || string.IsNullOrEmpty(dataType)
                    || (importType == ImportType.FLYING && string.IsNullOrEmpty(this.headerRow))
                    )
                {
                    return false;
                }

                string url = ConfigurationManager.AppSettings["UrlImport"];
                //csvFilePath = csvFilePath.Replace("\\", "\\\\");

                CheckParamRequest checkParamRequest = new CheckParamRequest();
                checkParamRequest.command = "check-param";
                checkParamRequest.presetPack = presetPack;
                checkParamRequest.presetSeq = null;
                checkParamRequest.dataType = dataType;

                if (importType == ImportType.FLYING) // 비행데이터 import
                {
                    checkParamRequest.headerRow = this.headerRow;
                }
                else // 해석데이터 import
                {
                    checkParamRequest.importFilePath = this.csvFilePath;
                }

                var json = JsonConvert.SerializeObject(checkParamRequest);

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
                ImportResponse result = JsonConvert.DeserializeObject<ImportResponse>(responseText);

                if (result != null)
                {
                    if (result.code != 200)
                    {
                        log.Error(result.message);
                        MessageBox.Show(result.message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        List<UnmappedParamData> unmappedList = new List<UnmappedParamData>();
                        foreach (string type in result.response.notMappedParams)
                        {
                            unmappedList.Add(new UnmappedParamData(type, "skip", "Unmapped"));
                        }
                        foreach (var type in result.response.mappedParams)
                        {
                            switch (dataType)
                            {
                                case "gtp":
                                    unmappedList.Add(new UnmappedParamData(type.paramKey, type.grtKey, "Mapped"));
                                    break;
                                case "fltp": 
                                    unmappedList.Add(new UnmappedParamData(type.paramKey, type.fltpKey, "Mapped"));
                                    break;
                                case "flts": 
                                    unmappedList.Add(new UnmappedParamData(type.paramKey, type.fltsKey, "Mapped"));
                                    break;
                                case "adams": 
                                    unmappedList.Add(new UnmappedParamData(type.paramKey, type.adamsKey, "Mapped"));
                                    break;
                                case "zaero": 
                                    unmappedList.Add(new UnmappedParamData(type.paramKey, type.zaeroKey, "Mapped"));
                                    break;
                            }
                        }
                        this.gridControl1.DataSource = unmappedList;
                        gridView1.RefreshData();
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

        private void luePresetList_EditValueChanged(object sender, EventArgs e)
        {
            CheckParam();
        }

        private void edtTag_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit me = sender as ButtonEdit;
            if (me != null)
            {
                addTag(me.Text);
                me.Text = String.Empty;
            }
        }

        private void edtTag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            ButtonEdit me = sender as ButtonEdit;
            if (me != null)
            {
                addTag(me.Text);
                me.Text = String.Empty;
            }
        }

        private void addTag(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            ButtonEdit btn = new ButtonEdit();
            btn.Properties.Buttons[0].Kind = ButtonPredefines.Close;
            btn.BorderStyle = BorderStyles.Simple;
            btn.ForeColor = Color.White;
            btn.Properties.Appearance.BorderColor = Color.White;
            btn.Font = new Font(btn.Font, FontStyle.Bold);
            btn.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            //btn.ReadOnly = true;
            btn.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            btn.Properties.AllowFocused = false;
            btn.ButtonClick += removeTag_ButtonClick;
            btn.Text = name;
            panelTag.Controls.Add(btn);
        }

        private void removeTag_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit btn = sender as ButtonEdit;
            panelTag.Controls.Remove(btn);

        }

        private void cboImportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckParam();
        }

        private void chkCustomFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCustomFilter.Checked)
            {
                chkHPF.Checked = false;
                chkLPF.Checked = false;
                chkHPF.Enabled = false;
                chkLPF.Enabled = false;
            }
            else
            {
                chkHPF.Enabled = true;
                chkLPF.Enabled = true;
            }
        }

        private void gridView2_KeyUp(object sender, KeyEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.KeyCode == Keys.Enter && (view.FocusedColumn.FieldName == "StartTime" || view.FocusedColumn.FieldName == "EndTime"))
            {
                ImportIntervalData importIntervalData = (ImportIntervalData)gridView2.GetFocusedRow();
                DateTime startTime = DateTime.Now;
                DateTime endTime = DateTime.Now;

                bool startParseYN = DateTime.TryParse(importIntervalData.StartTime,out startTime);
                bool endParseYN = DateTime.TryParse(importIntervalData.EndTime, out endTime);

                if(importIntervalData.StartTime == null || importIntervalData.EndTime == null)
                {
                    return;
                }
                if (!(startParseYN && endParseYN))
                {
                    MessageBox.Show(startParseYN ? "종료시간이 시간형식이 아닙니다. \n다시 확인해주세요." : "시작시간이 시간형식이 아닙니다. \n다시 확인해주세요.");
                    if(view.FocusedColumn.FieldName == "StartTime")
                    {
                        importIntervalData.StartTime = null;
                    }
                    else
                    {
                        importIntervalData.EndTime = null;
                    }
                        return;
                }
                DataTable dataTable = paramControlList[0].Dt;

                var dataStartTime = (DateTime)dataTable.Rows[0][0];
                var dataEndTime = (DateTime)dataTable.Rows[(dataTable.Rows.Count - 1)][0];

                if(endTime < startTime)
                {
                    MessageBox.Show("선택된 시작시간이 종료 시간보다 빠릅니다. \n다시 확인해주세요.");
                    if (view.FocusedColumn.FieldName == "StartTime")
                    {
                        importIntervalData.StartTime = null;
                    }
                    else
                    {
                        importIntervalData.EndTime = null;
                    }
                    return;
                }

                if (importIntervalData.StartTime != null && startTime < dataStartTime)
                {
                    MessageBox.Show("선택된 시작시간이 전체 시간보다 빠릅니다. \n다시 확인해주세요.");
                    importIntervalData.StartTime = null;
                    return;
                }

                if (importIntervalData.EndTime != null && endTime > dataEndTime)
                {
                    MessageBox.Show("선택된 종료시간이 전체 시간보다 느립니다. \n다시 확인해주세요.");
                    importIntervalData.EndTime = null;
                    return;
                }
                DataTable dt = GetIntervalData(paramControlList[0].Dt, startTime, endTime);
                int recordCnt = dt.Rows.Count;
                importIntervalData.DataCount = recordCnt.ToString();
                minValue = Convert.ToDateTime(importIntervalData.StartTime);
                maxValue = Convert.ToDateTime(importIntervalData.EndTime);

                foreach (ImportParamControl ctrl in paramControlList)
                {
                    ctrl.SelectRegion(minValue, maxValue);
                }
                gridView2.RefreshData();

            }
        }

        private void btn_downloadAll_Click(object sender, EventArgs e)
        {
            int row = 0;
            string importType = gridView2.GetRowCellValue(row, "ImportType") == null ? "" : gridView2.GetRowCellValue(row, "ImportType").ToString();
            string partSeq = gridView2.GetRowCellValue(row, "Seq") == null ? "" : gridView2.GetRowCellValue(row, "Seq").ToString();
            string partName = gridView2.GetRowCellValue(row, "SplitName") == null ? "" : gridView2.GetRowCellValue(row, "SplitName").ToString();

            GetPartData(partName, importType, partSeq, "Z", true);
        }
    }

    
}
