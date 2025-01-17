﻿using DevExpress.XtraBars.Docking;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DynaRAP.Common;
using DynaRAP.Forms;
using DynaRAP.TEST;
using DynaRAP.UControl;
using IronPython.Hosting;
using log4net.Config;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynaRAP
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string defaultLayoutFile = "Default.xml";
        string defaultWorkspaceName = "Default";
        string projectLayoutFile = "Project.xml";
        string projectWorkspaceName = "Project";

        DockPanel panelBottom = null;

        StartScreenControl startControl = null;
        ImportModuleControl flyingImportModuleControl = null;
        ImportModuleControl analysisImportModuleControl = null;
        ImportDLLControl dllImportModuleControl = null;
        SBModuleControl sbModuleControl = null;
        BinModuleControl binModuleControl = null;
        MgmtLRPControl mgmtLRPControl = null;
        MgmtMatchingTabletControl mgmtMatchingTableControl = null;
        //MgmtPresetGroupControl mgmtPresetGroupControl = null;
        PlotModuleControl plotModuleControl = null;
        ParameterModuleControl parameterModuleControl = null;
        FlyingTypeControl flyingTypeControl = null;
        ImportDeleteControl importDeleteControl = null;
        CsvTableControl csvTableControl = null;
        SelectCsvTableControl selectCsvTableControl = null;
        SelectPointTableControl selectPointTableControl = null;
        //DockPanel panelChart = null;
        //ChartControl chartControl = null;
        
        public DevExpress.XtraBars.Docking.DockManager DockManager1
        {
            get
            {
                return this.dockManager1;
            }
        }
       
        public DockPanel PanelSelectPointTable
        {
            get { return this.panelSelectPointTable; }
        }

        public DockPanel PanelSelectPointList
        {
            get { return this.panelSelectPointList; }
        }

        public DockPanel PanelImportViewCsv
        {
            get { return this.panelImportViewCsv; }
        }

        public DockPanel PanelScenario
        {
            get { return this.panelScenario; }
        }

        public DockPanel PanelOther
        {
            get { return this.panelOther; }
        }

        public DockPanel PanelProperties
        {
            get { return this.panelProperties; }
        }

        public CsvTableControl CsvTableControl
        {
            get => csvTableControl;
            set => csvTableControl = value;
        }
        public SelectCsvTableControl SelectCsvTableControl
        {
            get => selectCsvTableControl;
            set => selectCsvTableControl = value;
        }
        public SelectPointTableControl SelectPointTableControl
        {
            get => selectPointTableControl;
            set => selectPointTableControl = value;
        }


        public StartScreenControl StartControl { get => startControl; set => startControl = value; }
        public ImportModuleControl FlyingImportModuleControl { get => flyingImportModuleControl; set => flyingImportModuleControl = value; }
        public ImportModuleControl AnalysisImportModuleControl { get => analysisImportModuleControl; set => analysisImportModuleControl = value; }
        public ImportDLLControl DllImportModuleControl { get => dllImportModuleControl; set => dllImportModuleControl = value; }
        public SBModuleControl SbModuleControl { get => sbModuleControl; set => sbModuleControl = value; }
        public BinModuleControl BinModuleControl { get => binModuleControl; set => binModuleControl = value; }
        public MgmtLRPControl MgmtLRPControl { get => mgmtLRPControl; set => mgmtLRPControl = value; }
        public MgmtMatchingTabletControl MgmtMatchingTabletControl { get => mgmtMatchingTableControl; set => mgmtMatchingTableControl = value; }
        public PlotModuleControl PlotModuleControl { get => plotModuleControl; set => plotModuleControl = value; }

        //public MgmtPresetGroupControl MgmtPresetGroupControl { get => mgmtPresetGroupControl; set => mgmtPresetGroupControl = value; }

        public ParameterModuleControl ParamModuleControl { get => parameterModuleControl; set => parameterModuleControl = value; }

        public void ShowSplashScreenManager(string message) {
            if (!splashScreenManager1.IsSplashFormVisible)
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption(message);
            }
        }
        public void HideSplashScreenManager()
        {
            if (splashScreenManager1.IsSplashFormVisible)
                splashScreenManager1.CloseWaitForm();
        }


public MainForm()
        {
            InitializeComponent();

            XmlConfigurator.Configure(new FileInfo("log4net.xml"));

            // Handling the QueryControl event that will populate all automatically generated Documents
            this.tabbedView1.QueryControl += tabbedView1_QueryControl;
            this.tabbedView1.DocumentRemoved += TabbedView1_DocumentRemoved;

            LoadWorkspaces();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeWorkspace();

#if DEBUG
#else
            bar2.Visible = false;
            Workspace.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barSubItemTest.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnStart.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
#endif

            if (startControl == null)
            {
                startControl = new StartScreenControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(startControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "시작화면";
                tabbedView1.ActivateDocument(startControl);
            }
            else
            {
                tabbedView1.ActivateDocument(startControl);
            }
            /*
            if (flyingImportModuleControl == null)
            {
                flyingImportModuleControl = new ImportModuleControl(ImportType.FLYING);
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(flyingImportModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "Flying";
                doc.Caption = "비행데이터 Import";
                tabbedView1.ActivateDocument(flyingImportModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(flyingImportModuleControl);
            }

            if (analysisImportModuleControl == null)
            {
                analysisImportModuleControl = new ImportModuleControl(ImportType.ANALYSIS);
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(analysisImportModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "Analysis";
                doc.Caption = "해석데이터 Import";
                tabbedView1.ActivateDocument(analysisImportModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(analysisImportModuleControl);
            }

            if (dllImportModuleControl == null)
            {
                dllImportModuleControl = new ImportDLLControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(dllImportModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "DLL";
                doc.Caption = "DLL Import";
                tabbedView1.ActivateDocument(dllImportModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(dllImportModuleControl);
            }

            if (sbModuleControl == null)
            {
                sbModuleControl = new SBModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(sbModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "Short Block";
                tabbedView1.ActivateDocument(sbModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(sbModuleControl);
            }

            if (binModuleControl == null)
            {
                binModuleControl = new BinModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(binModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "BIN 구성";
                tabbedView1.ActivateDocument(binModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(binModuleControl);
            }

            if (mgmtLRPControl == null)
            {
                mgmtLRPControl = new MgmtLRPControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(mgmtLRPControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "LRP 관리";
                tabbedView1.ActivateDocument(mgmtLRPControl);
            }
            else
            {
                tabbedView1.ActivateDocument(mgmtLRPControl);
            }

            if (mgmtMatchingTableControl == null)
            {
                mgmtMatchingTableControl = new MgmtMatchingTabletControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(mgmtMatchingTableControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "매칭테이블 관리";
                tabbedView1.ActivateDocument(mgmtMatchingTableControl);
            }
            else
            {
                tabbedView1.ActivateDocument(mgmtMatchingTableControl);
            }

            if (plotModuleControl == null)
            {
                plotModuleControl = new PlotModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(plotModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "PLOT";
                tabbedView1.ActivateDocument(plotModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(plotModuleControl);
            }

            //if (mgmtPresetGroupControl == null)
            //{
            //    mgmtPresetGroupControl = new MgmtPresetGroupControl();
            //    DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(mgmtPresetGroupControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
            //    doc.Caption = "프리셋그룹 관리";
            //    tabbedView1.ActivateDocument(mgmtPresetGroupControl);
            //}
            //else
            //{
            //    tabbedView1.ActivateDocument(mgmtPresetGroupControl);
            //}

            if (parameterModuleControl == null)
            {
                parameterModuleControl = new ParameterModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(parameterModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "Parameter Module";
                doc.Caption = "Parameter Module";
                tabbedView1.ActivateDocument(parameterModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(parameterModuleControl);
            }
            
            */

#if DEBUG
            tabbedView1.ActivateDocument(startControl);
#else
            tabbedView1.RemoveDocument(flyingImportModuleControl);
            tabbedView1.RemoveDocument(analysisImportModuleControl);
            tabbedView1.RemoveDocument(dllImportModuleControl);
            tabbedView1.RemoveDocument(mgmtMatchingTableControl);
            tabbedView1.RemoveDocument(sbModuleControl);
            tabbedView1.RemoveDocument(binModuleControl);
            tabbedView1.RemoveDocument(mgmtLRPControl);
            tabbedView1.RemoveDocument(plotModuleControl);
            tabbedView1.RemoveDocument(parameterModuleControl);
            tabbedView1.ActivateDocument(startControl);
#endif

            //if (panelBinTable == null)
            //{
            //    panelBinTable = dockManager1.AddPanel(DockingStyle.Bottom);
            //}
            panelSelectPointTable.Name = "panelBinTable";
            panelSelectPointTable.Text = "선택 Point TABLE";
            selectCsvTableControl = new SelectCsvTableControl();
            selectCsvTableControl.Dock = DockStyle.Fill;
            panelSelectPointTable.Controls.Add(selectCsvTableControl);
            //panelBinTable.Text = "BIN TABLE";
            //BinTableControl binTableCtrl = new BinTableControl();
            //binTableCtrl.Dock = DockStyle.Fill;
            //panelBinTable.Controls.Add(binTableCtrl);
            panelSelectPointTable.Hide();

            //panelBinSbList.Name = "panelBinSbList";
            //panelBinSbList.Text = "ShortBlockList";
            //BinParameterSelectControl binSbListCtrl = new BinParameterSelectControl();
            //binSbListCtrl.Dock = DockStyle.Fill;
            //panelBinSbList.Controls.Add(binSbListCtrl);
            //panelBinSbList.Hide();

            panelImportViewCsv.Name = "panelImportViewCsv";
            panelImportViewCsv.Text = "CSV TABLE";
            csvTableControl = new CsvTableControl();
            csvTableControl.Dock = DockStyle.Fill;
            panelImportViewCsv.Controls.Add(csvTableControl);
            panelImportViewCsv.Hide();


            panelSelectPointList.Name = "선택 Plot Point";
            panelSelectPointList.Text = "선택 Plot Point";
            selectPointTableControl = new SelectPointTableControl();
            selectPointTableControl.Dock = DockStyle.Fill;
            panelSelectPointList.Controls.Add(selectPointTableControl);
        
            panelScenario.Hide();
            panelOther.Hide();
            panelProperties.Hide();
            panelSelectPointList.Hide();
        }

        void LoadWorkspaces()
        {
            workspaceManager1.LoadWorkspace(defaultWorkspaceName, defaultLayoutFile);
            workspaceManager1.LoadWorkspace(projectWorkspaceName, projectLayoutFile);
        }

        void tabbedView1_QueryControl(object sender, DevExpress.XtraBars.Docking2010.Views.QueryControlEventArgs e)
        {
            //if (e.Document == startScreenControlDocument)
            //{
            //    e.Control = startControl;
            //}
            //else if (e.Document == importModuleControlDocument)
            //{
            //    e.Control = importModuleControl;
            //}
            //else if (e.Document == sBModuleControlDocument)
            //{
            //    e.Control = sbModuleControl;
            //}
            //else if (e.Document == binModuleControlDocument)
            //{
            //    e.Control = binModuleControl;
            //}
            //else if (e.Document == mgmtLRPControl)
            //{
            //    e.Control = mgmtLRPControl;
            //}
            //else if (e.Document == mgmtMatchingTableControl)
            //{
            //    e.Control = mgmtMatchingTableControl;
            //}
            //else if (e.Document == mgmtPresetGroupControl)
            //{
            //    e.Control = mgmtPresetGroupControl;
            //else if (e.Document == plotModuleControl)
            //{
            //    e.Control = plotModuleControl;
            //}
            if (e.Control == null)
            {
                e.Control = new System.Windows.Forms.Control();
            }
        }

        private void TabbedView1_DocumentRemoved(object sender, DevExpress.XtraBars.Docking2010.Views.DocumentEventArgs e)
        {
            if (e.Document.Control is StartScreenControl)
            {
                startControl = null;
            }
            else if (e.Document.Control is ImportModuleControl)
            {
                if(e.Document.Tag.ToString().Equals("Flying"))
                    flyingImportModuleControl = null;
                else if(e.Document.Tag.ToString().Equals("Analysis"))
                    analysisImportModuleControl = null;
            }
            else if (e.Document.Control is ImportDLLControl)
            {
                dllImportModuleControl = null;
            }
            else if (e.Document.Control is SBModuleControl)
            {
                sbModuleControl = null;
            }
            else if (e.Document.Control is BinModuleControl)
            {
                binModuleControl = null;
            }
            else if (e.Document.Control is MgmtLRPControl)
            {
                mgmtLRPControl = null;
            }
            else if (e.Document.Control is MgmtMatchingTabletControl)
            {
                mgmtMatchingTableControl = null;
            }
            else if (e.Document.Control is PlotModuleControl)
            {
                if (MessageBox.Show("저장하지 않은 PLOT은 삭제됩니다. 저장하시겠습니까?", "PLOT 종료", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    plotModuleControl = (PlotModuleControl)e.Document.Control;
                    plotModuleControl.SavePlotData("inSide");
                }
                plotModuleControl = null;
                panelSelectPointList.Hide();
                panelSelectPointTable.Hide();
            }
            else if (e.Document.Control is ParameterModuleControl)
            {
                parameterModuleControl = null;
            }
            else if (e.Document.Control is FlyingTypeControl)
            {
                flyingTypeControl = null;
            }
            else if (e.Document.Control is ImportDeleteControl)
            {
                importDeleteControl = null;
            }
            
            //else if (e.Document.Control is MgmtPresetGroupControl)
            //{
            //    mgmtPresetGroupControl = null;
            //}

        }

        private void InitializeWorkspace()
        {
            //Use the WorkspaceManager to handle the layout of DevExpress controls that reside within the current form.
            workspaceManager1.TargetControl = this;

            // Save & restore the form's size, position and state along with DevExpress controls' layouts.
            workspaceManager1.SaveTargetControlSettings = true;

            // Disable layout load animation effects 
            workspaceManager1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;

            // Disable (de)serialization for the following controls (if required):
            //WorkspaceManager.SetSerializationEnabled(gaugeControl1, false);
            //WorkspaceManager.SetSerializationEnabled(accordionControl1, false);

            // When restoring layouts of controls in a Form.Load event handler,
            // you may need to call the controls' ForceInitialize methods to finish their initialization before restoring their layouts.
            //gridControl1.ForceInitialize();
            //dockManager1.ForceInitialize();
            //barManager1.ForceInitialize();
            //...

            //Load DevExpress controls' layouts from a file
            //if (workspaceManager1.LoadWorkspace(projectWorkspaceName, projectLayoutFile, true))
            //    workspaceManager1.ApplyWorkspace(projectWorkspaceName);

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Save DevExpress controls' layouts to a file
            workspaceManager1.CaptureWorkspace(projectWorkspaceName, true);
            workspaceManager1.SaveWorkspace(projectWorkspaceName, projectLayoutFile, true);
            
        }

        public void Server_LoginSucceed(object sender, EventArgs e)
        {

        }
        private void btnPythonTest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //pythonTest1();
            pythonTest2();
            
        }

        private void pythonTest1()
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptSource source = engine.CreateScriptSourceFromFile("Python/Calculator.py");
            ScriptScope scope = engine.CreateScope();
            source.Execute(scope);

            dynamic Calculator = scope.GetVariable("Calculator");
            dynamic calc = Calculator();
            int result = calc.add(4, 5);
            Console.WriteLine("result = {0}", result);
        }

        private void pythonTest2()
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            var paths = engine.GetSearchPaths();

            paths.Add(@"C:\Python37");
            paths.Add(@"C:\Python37\DLLs");
            paths.Add(@"C:\Python37\Lib");
            paths.Add(@"C:\Python37\Lib\site-packages");

            engine.SetSearchPaths(paths);

            ScriptSource source = engine.CreateScriptSourceFromFile("Python/filter.py");
            source.Execute(scope);
        }

        private void btnChartTest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TestMsChartForm form = new TestMsChartForm();
            form.Show();
        }

        private void btnLogin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoginForm form = new LoginForm();
            form.Show();
        }

        private void PanelBottom_ClosedPanel(object sender, DockPanelEventArgs e)
        {
            //panelBottom.ClosedPanel -= PanelBottom_ClosedPanel;
            //panelBottom = null;
        }

        private void Panel_ClosedPanel(object sender, DockPanelEventArgs e)
        {
            DockPanel panel = sender as DockPanel;
            DockPanel container = panel.ParentPanel;
            if (container == null)
                panelBottom = null;
        }

        private void toolTreeTest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeTestForm form = new TreeTestForm();
            form.Show();
        }

        private void btnStart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (startControl == null)
            {
                startControl = new StartScreenControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(startControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "시작화면";
                tabbedView1.ActivateDocument(startControl);
            }
            else
            {
                tabbedView1.ActivateDocument(startControl);
            }

        }

        private void btnImportModule_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (flyingImportModuleControl == null)
            {
                flyingImportModuleControl = new ImportModuleControl(ImportType.FLYING);
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(flyingImportModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "Flying";
                doc.Caption = "비행데이터 Import";
                tabbedView1.ActivateDocument(flyingImportModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(flyingImportModuleControl);
            }

        }

        private void btnSBModule_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (sbModuleControl == null)
            {
                sbModuleControl = new SBModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(sbModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "Short Block";
                tabbedView1.ActivateDocument(sbModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(sbModuleControl);
            }
        }

        private void btnBinModule_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (binModuleControl == null)
            {
                binModuleControl = new BinModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(binModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "BIN 구성";
                tabbedView1.ActivateDocument(binModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(binModuleControl);
            }

        }

        private void btnImportAnalysis_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (analysisImportModuleControl == null)
            {
                analysisImportModuleControl = new ImportModuleControl(ImportType.ANALYSIS);
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(analysisImportModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "Analysis";
                doc.Caption = "해석데이터 Import";
                tabbedView1.ActivateDocument(analysisImportModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(analysisImportModuleControl);
            }
        }

        private void btnDLLImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dllImportModuleControl == null)
            {
                dllImportModuleControl = new ImportDLLControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(dllImportModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "DLL";
                doc.Caption = "기준데이터 Import";
                tabbedView1.ActivateDocument(dllImportModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(dllImportModuleControl);
            }
        }


        private void btnLPF_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TestFilterForm form = new TestFilterForm();
            form.Show();
        }

        private void btnWebTest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TestWebForm form = new TestWebForm();
            form.Show();
        }

        private void btnMgmtParameter_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (mgmtLRPControl == null)
            {
                mgmtLRPControl = new MgmtLRPControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(mgmtLRPControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "LRP 관리";
                tabbedView1.ActivateDocument(mgmtLRPControl);
            }
            else
            {
                tabbedView1.ActivateDocument(mgmtLRPControl);
            }
        }

        private void btnMgmtPreset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (mgmtMatchingTableControl == null)
            {
                mgmtMatchingTableControl = new MgmtMatchingTabletControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(mgmtMatchingTableControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "매칭테이블 관리";
                tabbedView1.ActivateDocument(mgmtMatchingTableControl);
            }
            else
            {
                tabbedView1.ActivateDocument(mgmtMatchingTableControl);
            }
        }

        private void btnMgmtPresetGroup_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (mgmtPresetGroupControl == null)
            //{
            //    mgmtPresetGroupControl = new MgmtPresetGroupControl();
            //    DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(mgmtPresetGroupControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
            //    doc.Caption = "프리셋그룹 관리";
            //    tabbedView1.ActivateDocument(mgmtPresetGroupControl);
            //}
            //else
            //{
            //    tabbedView1.ActivateDocument(mgmtPresetGroupControl);
            //}
        }

        private void btnTestChart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TestChartLine form = new TestChartLine();
            form.Show();
        }

        private void btnChartLine2d_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = new DataTable();

            if (dt != null)
            {
                //MainForm mainForm = this.ParentForm as MainForm;

                //if (chartControl != null)
                //{
                //    chartControl.Dispose();
                //    chartControl = null;
                //}

                DXChartControl chartControl = new DXChartControl();
                DockPanel panelChart = null;

                if (panelChart == null)
                {
                    panelChart = new DockPanel();
                    panelChart = this.DockManager1.AddPanel(DockingStyle.Float);
                    panelChart.FloatLocation = new Point(500, 100);
                    panelChart.FloatSize = new Size(1058, 528);
                    //panelChart.Name = this.sb.SbName;
                    //panelChart.Text = this.sb.SbName;
                    chartControl.Dock = DockStyle.Fill;
                    panelChart.Controls.Add(chartControl);
                    panelChart.ClosedPanel += PanelChart_ClosedPanel;
                }
                else
                {
                    //panelChart.Name = this.sb.SbName;
                    //panelChart.Text = this.sb.SbName;
                    //panelChart.Controls.Clear();
                    chartControl.Dock = DockStyle.Fill;
                    panelChart.Controls.Add(chartControl);
                    panelChart.Show();
                    panelChart.Focus();
                }
            }
            //return;
            
            /*
            if (plotModuleControl == null)
            {
                plotModuleControl = new PlotModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(plotModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "PLOT";
                tabbedView1.ActivateDocument(plotModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(plotModuleControl);
            }

            plotModuleControl.AddDocument(new DXChartControl());

            return;
            */

            //TestChartLine form = new TestChartLine();
            //form.Show();
            //return;

            //ChartControl chartControl = null;

            //chartControl = GetLine2DChartControl();
            //if (chartControl != null)
            //{
            //    XtraForm chartForm = GetChartForm(chartControl);
            //    chartForm.Text = "Line 2D";

            //    chartForm.Show();
            //}
        }

        private void PanelChart_ClosedPanel(object sender, DockPanelEventArgs e)
        {
        }

        private void btnChartMinMax_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (plotModuleControl == null)
            {
                plotModuleControl = new PlotModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(plotModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "PLOT";
                tabbedView1.ActivateDocument(plotModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(plotModuleControl);
            }

            plotModuleControl.AddDocument(new DXChartControl());

            //return;
            
            TestChartLine form = new TestChartLine();
            form.Show();
            return;
            
            /*
            ChartControl chartControl = null;

            chartControl = GetRangeArea2DChartControl();
            if (chartControl != null)
            {
                XtraForm chartForm = GetChartForm(chartControl);
                chartForm.Text = "Min Max";

                chartForm.Show();
            }
            */
        }

        private void btnChartPotato_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
                if (plotModuleControl == null)
            {
                plotModuleControl = new PlotModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(plotModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Caption = "PLOT";
                tabbedView1.ActivateDocument(plotModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(plotModuleControl);
            }

            plotModuleControl.AddDocument(new DXChartControl());

            //return;

            TestChartPotato form = new TestChartPotato();
            form.Show();
            return;

            ChartControl chartControl = null;

            chartControl = GetPolarArea2DChartControl();
            if (chartControl != null)
            {
                XtraForm chartForm = GetChartForm(chartControl);
                chartForm.Text = "Potato";

                chartForm.Show();
            }
        }

        private XtraForm GetChartForm(ChartControl chartControl)
        {
            XtraForm form = new XtraForm();

            form.Owner = this;
            form.Width = 1000;
            form.Height = 600;
            form.StartPosition = FormStartPosition.CenterParent;
            form.Text = chartControl.Titles.Count > 0 ? chartControl.Titles[0].Text : string.Empty;
            form.Padding = new System.Windows.Forms.Padding(10);

            Splitter splitter = new Splitter();

            splitter.Width = 10;
            splitter.Dock = DockStyle.Right;

            form.Controls.Add(splitter);

            PropertyGrid propertyGrid = new PropertyGrid();

            propertyGrid.SelectedObject = chartControl;
            propertyGrid.Dock = DockStyle.Right;
            propertyGrid.Width = 300;

            form.Controls.Add(propertyGrid);

            chartControl.Dock = DockStyle.Fill;

            form.Controls.Add(chartControl);

            chartControl.BringToFront();

            return form;
        }

        private ChartControl GetLine2DChartControl()
        {
            ChartControl chartControl = new ChartControl();

            Series series = new Series("SW901P_N", ViewType.Line);

            series.ArgumentScaleType = ScaleType.Numerical;

            ((LineSeriesView)series.View).LineMarkerOptions.Kind = MarkerKind.Triangle;
            ((LineSeriesView)series.View).LineStyle.DashStyle = DashStyle.Dash;

            Random rand = new Random();
            series.Points.Add(new SeriesPoint(1,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(2,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(3,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(4,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(5,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(6,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(7,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(8,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(9,  rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(10, rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(11, rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(12, rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(13, rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(14, rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(15, rand.Next(5, 15)));
            series.Points.Add(new SeriesPoint(16, rand.Next(5, 15)));

            chartControl.Series.Add(series);

            ((XYDiagram)chartControl.Diagram).EnableAxisXZooming = true;
            ((XYDiagram)chartControl.Diagram).EnableAxisYZooming = true;
            ((XYDiagram)chartControl.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartControl.Diagram).EnableAxisYScrolling = true;

            chartControl.Titles.Add(new ChartTitle());

            chartControl.Titles[0].Text = "";
            chartControl.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

            return chartControl;
        }

        private ChartControl GetRangeArea2DChartControl()
        {
            ChartControl chartControl = new ChartControl();

            Series series = new Series("SW901P_N", ViewType.RangeArea);

            series.ArgumentScaleType = ScaleType.Qualitative;

            ((RangeAreaSeriesView)series.View).Transparency = 128;

            series.Points.Add(new SeriesPoint("Jan", 21, 15));
            series.Points.Add(new SeriesPoint("Feb", 22, 14));
            series.Points.Add(new SeriesPoint("Mar", 24, 13));
            series.Points.Add(new SeriesPoint("Apr", 24, 11));
            series.Points.Add(new SeriesPoint("May", 25, 11));
            series.Points.Add(new SeriesPoint("Jun", 26, 10));
            series.Points.Add(new SeriesPoint("Jul", 26, 9));
            series.Points.Add(new SeriesPoint("Aug", 28, 9));
            series.Points.Add(new SeriesPoint("Sep", 29, 7));
            series.Points.Add(new SeriesPoint("Oct", 30, 6));
            series.Points.Add(new SeriesPoint("Nov", 31, 5));
            series.Points.Add(new SeriesPoint("Dec", 32, 4));

            chartControl.Series.AddRange(new Series[] { series });

            ((XYDiagram)chartControl.Diagram).EnableAxisXZooming = true;
            ((XYDiagram)chartControl.Diagram).EnableAxisYZooming = true;
            ((XYDiagram)chartControl.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartControl.Diagram).EnableAxisYScrolling = true;

            chartControl.Titles.Add(new ChartTitle());

            chartControl.Titles[0].Text = "";
            chartControl.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

            return chartControl;
        }

        private ChartControl GetPolarArea2DChartControl()
        {
            ChartControl chartControl = new ChartControl();

            Series series = new Series("SW901P_N", ViewType.PolarArea);

            series.ArgumentScaleType = ScaleType.Numerical;

            ((PolarAreaSeriesView)series.View).Transparency = 128;

            series.Points.Add(new SeriesPoint(0, 90));
            series.Points.Add(new SeriesPoint(90, 70));
            series.Points.Add(new SeriesPoint(180, 60));
            series.Points.Add(new SeriesPoint(270, 100));

            chartControl.Series.Add(series);

            ((PolarDiagram)chartControl.Diagram).StartAngleInDegrees = 180;
            ((PolarDiagram)chartControl.Diagram).RotationDirection = RadarDiagramRotationDirection.Counterclockwise;

            chartControl.Titles.Add(new ChartTitle());

            chartControl.Titles[0].Text = "";
            chartControl.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

            return chartControl;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //GridGroupingForm form = new GridGroupingForm();
            //form.Show();

            //TestGrid3Form form3 = new TestGrid3Form();
            //form3.Show();

            //TestGridForm form4 = new TestGridForm();
            //form4.Show();

            //TestGrid2Form form2 = new TestGrid2Form();
            //form2.Show();

            TestGrid4Form form4 = new TestGrid4Form();
            form4.Show();

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SBViewForm form = new SBViewForm();
            form.Show();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TestSplitContainerForm form = new TestSplitContainerForm();
            form.Show();
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TestTab1Form form = new TestTab1Form();
            form.Show();
        }

        private void btnSBInfoView_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SBListControl sbControl = new SBListControl(this);
            DockPanel panelSBList = new DockPanel();

            panelSBList = this.DockManager1.AddPanel(DockingStyle.Float);
            panelSBList.FloatLocation = new Point(500, 100);
            panelSBList.FloatSize = new Size(1058, 528);
            //panelSBList.Name = "";
            panelSBList.Text = "ShortBlock List";
            sbControl.Dock = DockStyle.Fill;
            panelSBList.Controls.Add(sbControl);
            panelSBList.ClosedPanel += PanelChart_ClosedPanel;
        }

        private void btnParamModule_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (parameterModuleControl == null)
            {
                parameterModuleControl = new ParameterModuleControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(parameterModuleControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "Parameter Module";
                doc.Caption = "Parameter Module";
                tabbedView1.ActivateDocument(parameterModuleControl);
            }
            else
            {
                tabbedView1.ActivateDocument(parameterModuleControl);
            }
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (importDeleteControl == null)
            {
                importDeleteControl = new ImportDeleteControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(importDeleteControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "분할데이터 삭제";
                doc.Caption = "분할데이터 삭제";
                tabbedView1.ActivateDocument(importDeleteControl);
            }
            else
            {
                tabbedView1.ActivateDocument(importDeleteControl);
            }
            
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (flyingTypeControl == null)
            {
                flyingTypeControl = new FlyingTypeControl();
                DevExpress.XtraBars.Docking2010.Views.Tabbed.Document doc = tabbedView1.AddDocument(flyingTypeControl) as DevExpress.XtraBars.Docking2010.Views.Tabbed.Document;
                doc.Tag = "분할데이터 기동이름 변경";
                doc.Caption = "분할데이터 기동이름 변경";
                tabbedView1.ActivateDocument(flyingTypeControl);
            }
            else
            {
                tabbedView1.ActivateDocument(flyingTypeControl);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MessageBox.Show("DynaRap 프로그램을 종료하시겠습니까?","종료",MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }

}