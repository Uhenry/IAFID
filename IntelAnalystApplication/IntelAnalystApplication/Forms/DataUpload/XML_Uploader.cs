using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using QuickXmlReader.Db;
using Telerik.WinControls.UI;

namespace QuickXmlReader.Forms.DataUpload
{
    public partial class XML_Uploader : Form
    {
        private string filePath = " ";
        private string dirName = " ";
        private DataSet rptDataSet = new DataSet();
        private int count = 0;
        public int fileReviewPercent = 0;
        private static bool dtResults;
        private static double tempDbl = 0.0;
        private tempDBDataContext dbContext = new tempDBDataContext();
        private string batchType;
        public List<string> pdfFiles = new List<string>();
        private List<string> pdfFilesPercentage = new List<string>();
        private List<string> strPdfFiles = new List<string>();
        private frmMain _parent;
        private frmReview reviewPopupfrm;
        private BackgroundWorker worker = new BackgroundWorker();
        private FolderBrowserDialog foldersDialog = new FolderBrowserDialog();
        private List<int> randomFileIndx = new List<int>();

        public XML_Uploader(frmMain parent)
        {
            _parent = parent;
            InitializeComponent();
        }

        private void SetPreferences()
        {
            this.radGrid.TableElement.RowHeight = 20;
            this.radGrid.TableElement.TableHeaderHeight = 50;
            this.radGrid.MasterTemplate.ChildViewTabsPosition = TabPositions.Top;
            this.radGrid.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGrid.MasterTemplate.EnableAlternatingRowColor = true;
            
        }

        private void LoadXmlReportToList(XDocument xmlDoc)
        {
            XNamespace ns = "FID";

            #region Load Reports from Straight XML

            if (xmlDoc.Root.Name == "GKRS_POCAFORM")
            {
                rptDataSet = new DataSet();
                rptDataSet = CreateTablesAndAddToDataSet(rptDataSet);

                try
                {
                    #region Create Lists and Add Rows to Tables

                    //Create Lists for Each Part of the Report//
                    List<ReportEO.Institution> instList = Parser.GetInstitutionHeadersFromXml(xmlDoc);
                    List<ReportEO.Customer> custList = Parser.GetCustomersFromXml(xmlDoc);
                    List<ReportEO.Agent> agentList = Parser.GetAgentsFromXml(xmlDoc);
                    List<ReportEO.Transaction> transList = Parser.GetReportTransactionsFromXml(xmlDoc);
                    List<ReportEO.Beneficiary> benList = Parser.GetBeneficiariesFromXml(xmlDoc);
                    List<ReportEO.Reason> repReasonList = Parser.GetReportReasonsFromXml(xmlDoc);

                    //Add rows to tables in memory//
                    Parser.AddRowsToInstitutionTable(rptDataSet, instList);
                    Parser.AddRowsToCustomerTable(rptDataSet, custList);
                    Parser.AddRowsToAgentsTable(rptDataSet, agentList);
                    Parser.AddRowsToBeneficiaryTable(rptDataSet, benList);
                    Parser.AddRowsToTransactionTable(rptDataSet, transList);
                    Parser.AddRowstoReportReasonTable(rptDataSet, repReasonList);

                    #endregion Create Lists and Add Rows to Tables
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }
                try
                {
                    #region Create Review Grid Templates

                    this.radGrid.Columns.Clear();
                    this.radGrid.MasterTemplate.Templates.Clear();
                    this.radGrid.DataSource = rptDataSet.Tables[0];
                    this.radGrid.MasterTemplate.Columns["ID"].IsVisible = false;
                    GridViewRelation relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    SetPreferences();

                    for (int i = 1; i < rptDataSet.Tables.Count; i++)
                    {
                        AddChildViewTemplate(radGrid, rptDataSet.Tables[i]);
                    }

                    //this.radGrid.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                    this.radGrid.MasterTemplate.Columns["DateofSignature"].FormatString = "{0:dd/MM/yyyy}";
                    ShowSummaryInformation();

                    #endregion Create Review Grid Templates
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }
            }

            else if (xmlDoc.Root.Name == "POCAFORM")
            {
                rptDataSet = new DataSet();
                rptDataSet = CreateTablesAndAddToDataSet(rptDataSet);

                try
                {
                    #region Create Lists and Add Rows to Tables

                    //Create Lists for Each Part of the Report//
                    List<ReportEO.Institution> instList = Parser.GetInstitutionHeadersFromXml(xmlDoc);

                    //List<ReportEO.Institution> instList1 = instList;
                    //if (instList1.Count() < instList.Count())
                    //{
                    //    instList1.Clear();
                    //    instList1 = instList;
                    //    //Apply pecentage change to instList1
                    //}
                    //else
                    //{
                    //    //Apply percentage change to instList1
                    //}
                    List<ReportEO.Customer> custList = Parser.GetCustomersFromXml(xmlDoc);
                    List<ReportEO.Agent> agentList = Parser.GetAgentsFromXml(xmlDoc);
                    List<ReportEO.Transaction> transList = Parser.GetReportTransactionsFromXml(xmlDoc);
                    List<ReportEO.Beneficiary> benList = Parser.GetBeneficiariesFromXml(xmlDoc);
                    List<ReportEO.Reason> repReasonList = Parser.GetReportReasonsFromXml(xmlDoc);

                    

                    //Add rows to tables in memory//
                    Parser.AddRowsToInstitutionTable(rptDataSet, instList);
                    Parser.AddRowsToCustomerTable(rptDataSet, custList);
                    Parser.AddRowsToAgentsTable(rptDataSet, agentList);
                    Parser.AddRowsToBeneficiaryTable(rptDataSet, benList);
                    Parser.AddRowsToTransactionTable(rptDataSet, transList);

                    Parser.AddRowstoReportReasonTable(rptDataSet, repReasonList);

                    #endregion Create Lists and Add Rows to Tables
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }

                try
                {
                    #region Create Review Grid Templates

                    this.radGrid.Columns.Clear();
                    this.radGrid.MasterTemplate.Templates.Clear();
                    this.radGrid.DataSource = rptDataSet.Tables[0];
                    this.radGrid.MasterTemplate.Columns["ID"].IsVisible = false;
                    GridViewRelation relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    SetPreferences();

                    for (int i = 1; i < rptDataSet.Tables.Count; i++)
                    {
                        AddChildViewTemplate(radGrid, rptDataSet.Tables[i]);
                    }

                    //this.radGrid.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                    this.radGrid.MasterTemplate.Columns["DateofSignature"].FormatString = "{0:dd/MM/yyyy}";
                    ShowSummaryInformation();

                    #endregion Create Review Grid Templates
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }
            }

            #region Just for Alliance

            else if (xmlDoc.Root.Name == (ns + "POCA_FORM"))
            {
                rptDataSet = new DataSet();
                rptDataSet = CreateTablesAndAddToDataSet(rptDataSet);
                batchType = xmlDoc.Root.Attribute("ReportType").Value.ToString();

                try
                {
                    #region Create Lists and Add Rows to Tables

                    List<ReportEO.Reason> repReasonList = new List<ReportEO.Reason>();

                    //Create Lists for Each Part of the Report//
                    List<ReportEO.Institution> instList = Parser.GetInstitutionHeadersFromXml(xmlDoc);
                    List<ReportEO.Customer> custList = Parser.GetCustomersFromXml(xmlDoc);
                    List<ReportEO.Agent> agentList = Parser.GetAgentsFromXml(xmlDoc);
                    List<ReportEO.Transaction> transList = Parser.GetReportTransactionsFromXml(xmlDoc);
                    List<ReportEO.Beneficiary> benList = Parser.GetBeneficiariesFromXml(xmlDoc);
                    if (batchType == "STR")
                    { repReasonList = Parser.GetReportReasonsFromXml(xmlDoc); }

                    //Add rows to tables in memory//
                    Parser.AddRowsToInstitutionTable(rptDataSet, instList);
                    Parser.AddRowsToCustomerTable(rptDataSet, custList);
                    Parser.AddRowsToAgentsTable(rptDataSet, agentList);
                    Parser.AddRowsToBeneficiaryTable(rptDataSet, benList);
                    Parser.AddRowsToTransactionTable(rptDataSet, transList);

                    if (transList.Count > instList.Count)
                    {
                        //var transListDISTINCT = (from e in instList
                        //                         select e).GroupBy(x => x.refNum.First()).ToList();

                        var instTrans = (from i in instList
                                         join t in transList
                                         on i.refNum equals t.refNum
                                         select t).ToList();

                    }

                    if (repReasonList.Count != 0)
                    { Parser.AddRowstoReportReasonTable(rptDataSet, repReasonList); }

                    #endregion Create Lists and Add Rows to Tables
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }

                try
                {
                    #region Create Review Grid Templates

                    this.radGrid.Columns.Clear();
                    this.radGrid.MasterTemplate.Templates.Clear();
                    this.radGrid.DataSource = rptDataSet.Tables[0];
                    this.radGrid.MasterTemplate.Columns["ID"].IsVisible = false;
                    GridViewRelation relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    SetPreferences();

                    for (int i = 1; i < rptDataSet.Tables.Count; i++)
                    {
                        AddChildViewTemplate(radGrid, rptDataSet.Tables[i]);
                    }


                    this.radGrid.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                    this.radGrid.MasterTemplate.Columns["DateofSignature"].FormatString = "{0:dd/MM/yyyy}";
                   
                   //this was written to scan through the grid for duplicate rows in the master template 
                    //using the Referemce Number and if there are any hide them
                    for (int i = 0; i < this.radGrid.MasterTemplate.Rows.Count; i++)
                    {
                        if (this.radGrid.MasterTemplate.Rows[i].Cells["RefNum"].Value.ToString() == this.radGrid.MasterTemplate.Rows[++i].Cells["RefNum"].Value.ToString())
                        {
                            this.radGrid.MasterTemplate.Rows[i].IsVisible = false;
                        }

                    }
                    
                    ShowSummaryInformation();

                    #endregion Create Review Grid Templates
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }
            }

            #endregion Just for Alliance

            #endregion Load Reports from Straight XML
        }

        private void AddChildViewTemplate(RadGridView radGrid, DataTable dataTable)
        {
            GridViewTemplate cTemplate = new GridViewTemplate();
            cTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            cTemplate.ReadOnly = true;
            cTemplate.AllowAddNewRow = false;
            cTemplate.Caption = dataTable.TableName.ToString();
            this.radGrid.MasterTemplate.Templates.Add(cTemplate);
            cTemplate.DataSource = dataTable;
            cTemplate.Columns["ID"].IsVisible = false;

            GridViewRelation relation = new GridViewRelation(this.radGrid.MasterTemplate);

            switch (dataTable.TableName)
            {
                case "Customer":
                    relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    relation.ChildTemplate = cTemplate;
                    relation.RelationName = "Instituion_Customers";
                    relation.ParentColumnNames.Add("RefNum");
                    relation.ChildColumnNames.Add("RefNum");
                    radGrid.Relations.Add(relation);
                    break;

                case "Agent":
                    relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    relation.ChildTemplate = cTemplate;
                    relation.RelationName = "Instituion_Agents";
                    relation.ParentColumnNames.Add("RefNum");
                    relation.ChildColumnNames.Add("RefNum");
                    radGrid.Relations.Add(relation);
                    break;

                case "Beneficiary":
                    relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    relation.ChildTemplate = cTemplate;
                    relation.RelationName = "Institution_Beneficaries";
                    relation.ParentColumnNames.Add("RefNum");
                    relation.ChildColumnNames.Add("RefNum");
                    radGrid.Relations.Add(relation);
                    break;

                case "Reason":
                    if (batchType == "STR")
                    {
                        relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = cTemplate;
                        relation.RelationName = "Institution_Reason";
                        relation.ParentColumnNames.Add("RefNum");
                        relation.ChildColumnNames.Add("RefNum");
                        radGrid.Relations.Add(relation);
                    }
                    else
                    {
                        relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = cTemplate;
                        relation.RelationName = "Institution_Reason";
                        relation.ParentColumnNames.Add("RefNum");
                        relation.ChildColumnNames.Add("RefNum");
                        radGrid.Relations.Add(relation);
                        cTemplate.Caption = "TTR With Reason";
                    }
                    break;

                case "Transaction":
                    relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    relation.ChildTemplate = cTemplate;
                    relation.RelationName = "Institution_Trans";
                    relation.ParentColumnNames.Add("RefNum");
                    relation.ChildColumnNames.Add("RefNum");
                    radGrid.Relations.Add(relation);
                    break;

                default:
                    MessageBox.Show("No Table found!");
                    break;
            }
        }

        private DataSet CreateTablesAndAddToDataSet(DataSet repDataSet)
        {
            #region Create Institution Table

            //--Add Institution Table to DataSet and Add Columns for View
            repDataSet.Tables.Add("Institution");
            repDataSet.Tables["Institution"].Columns.Add("ID", typeof(int));
            repDataSet.Tables["Institution"].Columns.Add("RefNum", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("InstituionName", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("Address", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("TRN", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("TypeOfInst", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("BranchAddress", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("PreparerFullName", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("PreparerTitle", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("DateofSignature", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("ContactFullName", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("ContactTitle", typeof(string));
            repDataSet.Tables["Institution"].Columns.Add("ContactPhoneNum", typeof(string));

            #endregion Create Institution Table

            #region Create Customer Table

            //--Add Customer Table to DataSet and Add Columns
            repDataSet.Tables.Add("Customer");
            repDataSet.Tables["Customer"].Columns.Add("ID", typeof(int));
            repDataSet.Tables["Customer"].Columns.Add("RefNum", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("FullName", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("Address", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("DateofBirth", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("TRN", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("VerificationMethod", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("IDType", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("IssuedBy", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("ID#", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("AccNumType", typeof(string));
            repDataSet.Tables["Customer"].Columns.Add("Occupation", typeof(string));
            //xmlDataSet.Relations.Add("Instituion_Customers", xmlDataSet.Tables["Institution"].Columns[0], xmlDataSet.Tables["Customer"].Columns[0]);
            repDataSet.Relations.Add("Instituion_Customers", repDataSet.Tables["Institution"].Columns[1], repDataSet.Tables["Customer"].Columns[1]);

            #endregion Create Customer Table

            #region Create Agent Table

            //--Add Agent Table to DataSet and Add Columns
            repDataSet.Tables.Add("Agent");
            repDataSet.Tables["Agent"].Columns.Add("ID", typeof(int));
            repDataSet.Tables["Agent"].Columns.Add("RefNum", typeof(string));
            repDataSet.Tables["Agent"].Columns.Add("FullName", typeof(string));
            repDataSet.Tables["Agent"].Columns.Add("Address", typeof(string));
            repDataSet.Tables["Agent"].Columns.Add("DateofBirth", typeof(string));
            repDataSet.Tables["Agent"].Columns.Add("TRN", typeof(string));
            repDataSet.Tables["Agent"].Columns.Add("VerificationMethod", typeof(string));
            repDataSet.Tables["Agent"].Columns.Add("IDType", typeof(string));
            repDataSet.Tables["Agent"].Columns.Add("IssuedBy", typeof(string));
            repDataSet.Tables["Agent"].Columns.Add("ID#", typeof(string));
            repDataSet.Relations.Add("Institution_Agents", repDataSet.Tables["Institution"].Columns[1], repDataSet.Tables["Agent"].Columns[1]);

            #endregion Create Agent Table

            #region Create Beneficiary Table

            //--Add Beneficiary Table to DataSet and Add Columns
            repDataSet.Tables.Add("Beneficiary");
            repDataSet.Tables["Beneficiary"].Columns.Add("ID", typeof(int));
            repDataSet.Tables["Beneficiary"].Columns.Add("RefNum", typeof(string));
            repDataSet.Tables["Beneficiary"].Columns.Add("FullName", typeof(string));
            repDataSet.Tables["Beneficiary"].Columns.Add("Address", typeof(string));
            repDataSet.Relations.Add("Institution_Benficiaries", repDataSet.Tables["Institution"].Columns[1], repDataSet.Tables["Beneficiary"].Columns[1]);

            #endregion Create Beneficiary Table

            #region Create Reason Table

            repDataSet.Tables.Add("Reason");
            repDataSet.Tables["Reason"].Columns.Add("ID", typeof(int));
            repDataSet.Tables["Reason"].Columns.Add("RefNum", typeof(string));
            repDataSet.Tables["Reason"].Columns.Add("Reason", typeof(string));
            repDataSet.Relations.Add("Institution_Reason", repDataSet.Tables["Institution"].Columns[1], repDataSet.Tables["Reason"].Columns[1]);

            #endregion Create Reason Table

            #region Create Transaction Table

            //--Add Transaction Table to DataSet and Add Columns
            repDataSet.Tables.Add("Transaction");
            repDataSet.Tables["Transaction"].Columns.Add("ID", typeof(int));
            repDataSet.Tables["Transaction"].Columns.Add("RefNum", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("Type", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("Date", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("Time", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("Currency", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("Amount", typeof(double));
            repDataSet.Tables["Transaction"].Columns.Add("A/C Type", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("A/C Num.", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("J$ Equiv", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("J$ Ex.Rate", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("US$ Equiv", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("US$ Ex.Rate", typeof(string));
            repDataSet.Tables["Transaction"].Columns.Add("Funds Source", typeof(string));
            repDataSet.Relations.Add("Institution_Trans", repDataSet.Tables["Institution"].Columns[1], repDataSet.Tables["Transaction"].Columns[1]);

            #endregion Create Transaction Table

            return repDataSet;
        }

        //private DataSet CreateTablesAndAddToDataSet(DataSet xmlDataSet)
        //{
        //    #region Create Institution Table

        //    //--Add Institution Table to DataSet and Add Columns for View
        //    xmlDataSet.Tables.Add("Institution");
        //    xmlDataSet.Tables["Institution"].Columns.Add("ID", typeof(int));
        //    xmlDataSet.Tables["Institution"].Columns.Add("RefNum", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("InstituionName", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("Address", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("TRN", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("TypeOfInst", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("BranchAddress", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("PreparerFullName", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("PreparerTitle", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("DateofSignature", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("ContactFullName", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("ContactTitle", typeof(string));
        //    xmlDataSet.Tables["Institution"].Columns.Add("ContactPhoneNum", typeof(string));
        //    #endregion

        //    #region Create Customer Table

        //    //--Add Customer Table to DataSet and Add Columns
        //    xmlDataSet.Tables.Add("Customer");
        //    xmlDataSet.Tables["Customer"].Columns.Add("ID", typeof(int));
        //    xmlDataSet.Tables["Customer"].Columns.Add("RefNum", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("FullName", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("Address", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("DateofBirth", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("TRN", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("VerificationMethod", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("IDType", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("IssuedBy", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("ID#", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("AccNumType", typeof(string));
        //    xmlDataSet.Tables["Customer"].Columns.Add("Occupation", typeof(string));
        //    xmlDataSet.Relations.Add("Instituion_Customers", xmlDataSet.Tables["Institution"].Columns[0], xmlDataSet.Tables["Customer"].Columns[0]);
        //    #endregion

        //    #region Create Agent Table

        //    //--Add Agent Table to DataSet and Add Columns
        //    xmlDataSet.Tables.Add("Agent");
        //    xmlDataSet.Tables["Agent"].Columns.Add("ID", typeof(int));
        //    xmlDataSet.Tables["Agent"].Columns.Add("RefNum", typeof(string));
        //    xmlDataSet.Tables["Agent"].Columns.Add("FullName", typeof(string));
        //    xmlDataSet.Tables["Agent"].Columns.Add("Address", typeof(string));
        //    xmlDataSet.Tables["Agent"].Columns.Add("DateofBirth", typeof(string));
        //    xmlDataSet.Tables["Agent"].Columns.Add("TRN", typeof(string));
        //    xmlDataSet.Tables["Agent"].Columns.Add("VerificationMethod", typeof(string));
        //    xmlDataSet.Tables["Agent"].Columns.Add("IDType", typeof(string));
        //    xmlDataSet.Tables["Agent"].Columns.Add("IssuedBy", typeof(string));
        //    xmlDataSet.Tables["Agent"].Columns.Add("ID#", typeof(string));
        //    xmlDataSet.Relations.Add("Institution_Agents", xmlDataSet.Tables["Institution"].Columns[0], xmlDataSet.Tables["Agent"].Columns[0]);
        //    #endregion

        //    #region Create Beneficiary Table

        //    //--Add Beneficiary Table to DataSet and Add Columns
        //    xmlDataSet.Tables.Add("Beneficiary");
        //    xmlDataSet.Tables["Beneficiary"].Columns.Add("ID", typeof(int));
        //    xmlDataSet.Tables["Beneficiary"].Columns.Add("RefNum", typeof(string));
        //    xmlDataSet.Tables["Beneficiary"].Columns.Add("FullName", typeof(string));
        //    xmlDataSet.Tables["Beneficiary"].Columns.Add("Address", typeof(string));
        //    xmlDataSet.Relations.Add("Institution_Benficiaries", xmlDataSet.Tables["Institution"].Columns[0], xmlDataSet.Tables["Beneficiary"].Columns[0]);
        //    #endregion

        //    #region Create Reason Table
        //    xmlDataSet.Tables.Add("Reason");
        //    xmlDataSet.Tables["Reason"].Columns.Add("ID", typeof(int));
        //    xmlDataSet.Tables["Reason"].Columns.Add("RefNum", typeof(string));
        //    xmlDataSet.Tables["Reason"].Columns.Add("Reason", typeof(string));
        //    xmlDataSet.Relations.Add("Institution_Reason", xmlDataSet.Tables["Institution"].Columns[0], xmlDataSet.Tables["Reason"].Columns[0]);
        //    #endregion

        //    #region Create Transaction Table

        //    //--Add Transaction Table to DataSet and Add Columns
        //    xmlDataSet.Tables.Add("Transaction");
        //    xmlDataSet.Tables["Transaction"].Columns.Add("ID", typeof(int));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("RefNum", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("Type", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("Date", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("Time", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("Currency", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("Amount", typeof(double));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("A/C Type", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("A/C Num.", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("J$ Equiv", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("J$ Ex.Rate", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("US$ Equiv", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("US$ Ex.Rate", typeof(string));
        //    xmlDataSet.Tables["Transaction"].Columns.Add("Funds Source", typeof(string));
        //    xmlDataSet.Relations.Add("Institution_Trans", xmlDataSet.Tables["Institution"].Columns[0], xmlDataSet.Tables["Transaction"].Columns[0]);
        //    #endregion

        //    return xmlDataSet;

        //}

        //private void radMenuItem2_Click(object sender, EventArgs e)
        //{
        //    //create the open file dialog variable
        //    OpenFileDialog fDialog = new OpenFileDialog();

        //    //set the title of the window
        //    fDialog.Title = "Open File for Review";

        //    //Set a file type filter
        //    fDialog.Filter = "XML Files |*.xml";

        //    //Set the initial Directory
        //    fDialog.InitialDirectory = @"\\lambda\Intel_\INTEL\Electronic Reports\XML";

        //    if (fDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        filePath = fDialog.FileName.ToString();
        //        batchType = Utility.GetBatchTypeFromName(fDialog.SafeFileName);
        //        lblFileName.Text = Path.GetFileNameWithoutExtension(filePath);
        //        XDocument xmlDoc = XDocument.Load(filePath);
        //        LoadXmlReportToList(xmlDoc);
        //        count = 0;
        //        btnSavetoDB.Enabled = true;
        //    }
        //}

        private void ShowSummaryInformation()
        {
            lblFileName.Visible = true;
            lblReportCount.Visible = true;
            lblFileName.Visible = true;
            lblReportCount.Text = String.Format("# of Reports in File: {0}", this.radGrid.RowCount.ToString());
        }

        private void AddSummaryRows()
        {
            GridViewSummaryItem summaryItem = new GridViewSummaryItem("RefNum", "The Number of Reports = {0}", GridAggregateFunction.Count);
            GridViewSummaryRowItem summaryRowItem = new GridViewSummaryRowItem(new GridViewSummaryItem[] { summaryItem });
            this.radGrid.SummaryRowsBottom.Add(summaryRowItem);
        }

        private void radMIOpenXmlfrmPDF_Click(object sender, EventArgs e)
        {
            //create the open file dialog variable
            OpenFileDialog fDialog = new OpenFileDialog();

            //set the title of the window
            fDialog.Title = "Open File for Review";

            //Set a file type filter
            fDialog.Filter = "XML Files |*.xml";

            //Set the initial Directory
            fDialog.InitialDirectory = @"\\lambda\Intel_\INTEL\Electronic Reports\XML - from PDF";

            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = fDialog.FileName.ToString();
                batchType = Utility.GetBatchTypeFromName(fDialog.SafeFileName);
                lblFileName.Text = Path.GetFileNameWithoutExtension(filePath);
                XDocument xmlDoc = XDocument.Load(filePath);
                LoadPdfXmlReportToList(xmlDoc);
                count = 0;

                //btnSavetoDB.Enabled = true;
            }
        }

        private void LoadPdfXmlReportToList(XDocument xmlDoc)
        {
            if (batchType == "TTR")
            {
                try
                {
                    #region Load Reports from Xml Generated From PDF - TTR

                    var reports = (from report in xmlDoc.Descendants("ttr_Frm")
                                   select new
                                   {
                                       instName = report.Element("_1__Name_of_Financial_Institution").Value.Trim(),
                                       RepNo = report.Element("_ref_num_pg1").Value.Trim(),
                                       instAddress = report.Element("_2_Address_of_Financial_Institution").Value.Trim(),
                                       instTRN = report.Element("_3_TRN").Value.Trim(),
                                       instType = report.Element("_5__Type_of_Financial_Institution").Value.Trim(),
                                       instBranch = report.Element("_4__Branch_Address").Value.Trim(),
                                       pLastName = report.Element("_31__Last_Name").Value.Trim(),
                                       pFirstName = report.Element("_32__First_Name").Value.Trim(),
                                       pFullName = report.Element("_32__First_Name").Value.Trim() + " " + report.Element("_31__Last_Name").Value.Trim(),
                                       pMI = report.Element("_33__MI").Value.Trim(),
                                       pTitle = report.Element("_34__Title").Value.Trim(),
                                       pPhoneNo = report.Element("_35__Phone_No").Value.Trim(),
                                       pDateofSignature = report.Element("_37_Date_of_Signature_DDMMYYYY").Value.Trim(),
                                       nLastName = report.Element("_38__Last_Name").Value.Trim(),
                                       nFirstName = report.Element("_39_First_Name").Value.Trim(),
                                       nFullName = report.Element("_39_First_Name").Value.Trim() + " " + report.Element("_38__Last_Name").Value.Trim(),
                                       nMI = report.Element("_40__MI").Value.Trim(),
                                       nTitle = report.Element("_41__Title").Value.Trim(),
                                       nPhoneNo = report.Element("_42__Phone_No").Value.Trim(),
                                       t1Type = report.Element("_2_Transaction_Type").Value.Trim(),
                                       t1Date = report.Element("_3_Date_DDMMYYYY").Value.Trim(),
                                       t1Time = report.Element("_4_Time").Value.Trim(),
                                       t1Currency = report.Element("_5__Transaction_Type").Value.Trim(),
                                       t1Amount = report.Element("_6__Transaction_Amount").Value.Trim(),
                                       t1Accounts = report.Element("_7_Type1").Value.ToString().Trim() + " " + report.Element("_7_Type2").Value.ToString().Trim() + " " + report.Element("_7_Type3").Value.ToString().Trim(),
                                       t1JamEquiv = report.Element("_8__JA_Equivalent").Value.Trim(),
                                       t1JamExRate = report.Element("_9__JA_Exchange_Rate").Value.Trim(),
                                       t1UsEquiv = report.Element("_10__US_Equivalent").Value.Trim(),
                                       t1UsExRate = report.Element("_11__US_Exchange_Rate").Value.Trim(),
                                       t1SourceofFunds = report.Element("_12__Source_of_funds").Value.Trim(),
                                       t2Type = report.Element("_13__Transaction_Type").Value.Trim(),
                                       t2Date = report.Element("_14__Date_DDMMYYYY").Value.Trim(),
                                       t2Time = report.Element("_15_Time").Value.Trim(),
                                       t2Currency = report.Element("_16__Transaction_Currency").Value.Trim(),
                                       t2Amount = report.Element("_17__Transaction_Amount").Value.Trim(),
                                       t2Accounts = report.Element("_18_Type1").Value.ToString().Trim() + " " + report.Element("_18_Type2").Value.ToString().Trim() + " " + report.Element("_18_Type3").Value.ToString().Trim(),
                                       t2JamEquiv = report.Element("_19__JA_Equivalent").Value.Trim(),
                                       t2JamExRate = report.Element("_20__JA_Exchange_Rate").Value.Trim(),
                                       t2UsEquiv = report.Element("_21__US_Equivalent").Value.Trim(),
                                       t2UsExRate = report.Element("_22__US_Exchange_Rate").Value.Trim(),
                                       t2SourceofFunds = report.Element("_23__Source_of_funds").Value.Trim(),
                                       t3Type = report.Element("_24_Transaction_Type").Value.Trim(),
                                       t3Date = report.Element("_25_Date_DDMMYYYY").Value.Trim(),
                                       t3Time = report.Element("_26__Time").Value.Trim(),
                                       t3Currency = report.Element("_27__Transaction_Currency").Value.Trim(),
                                       t3Amount = report.Element("_28__Transaction_Amount").Value.Trim(),
                                       t3Accounts = report.Element("_29_Type1").Value.ToString().Trim() + " " + report.Element("_29_Type2").Value.ToString().Trim() + " " + report.Element("_29_Type3").Value.ToString().Trim(),
                                       t3JamEquiv = report.Element("_30__JA_Equivalent").Value.Trim(),
                                       t3JamExRate = report.Element("_31__JA_Exchange_Rate").Value.Trim(),
                                       t3UsEquiv = report.Element("_32__US_Equivalent").Value.Trim(),
                                       t3UsExRate = report.Element("_33__US_Exchange_Rate").Value.Trim(),
                                       t3SourceofFunds = report.Element("_34__Source_of_funds").Value.Trim(),
                                       t4Type = report.Element("_35__Transaction_Type").Value.Trim(),
                                       t4Date = report.Element("_36_Date_DDMMYYYY").Value.Trim(),
                                       t4Time = report.Element("_37__Time").Value.Trim(),
                                       t4Currency = report.Element("_38__Transaction_Currency").Value.Trim(),
                                       t4Amount = report.Element("_39__Transaction_Amount").Value.Trim(),
                                       t4Accounts = report.Element("_40_Type1").Value.ToString().Trim() + " " + report.Element("_40_Type2").Value.ToString().Trim() + " " + report.Element("_40_Type3").Value.ToString().Trim(),
                                       t4JamEquiv = report.Element("_41__JA_Equivalent").Value.Trim(),
                                       t4JamExRate = report.Element("_42__JA_Exchange_Rate").Value.Trim(),
                                       t4UsEquiv = report.Element("_43__US_Equivalent").Value.Trim(),
                                       t4UsExRate = report.Element("_44__US_Exchange_Rate").Value.Trim(),
                                       t4SourceofFunds = report.Element("_45__Source_of_funds").Value.Trim(),
                                       _13a = report.Element("CustAgenBenSub").Element("_13a_Checkbox").Value.Trim(),
                                       _13b = report.Element("CustAgenBenSub").Element("_13b_Checkbox").Value.Trim(),
                                       _14a = report.Element("CustAgenBenSub").Element("_14a_Checkbox").Value.Trim(),
                                       _14b = report.Element("CustAgenBenSub").Element("_14b_Checkbox").Value.Trim(),
                                       _14c = report.Element("CustAgenBenSub").Element("_14c_Checkbox").Value.Trim(),
                                       _14d = report.Element("CustAgenBenSub").Element("_14d_Checkbox").Value.Trim(),
                                       _14other = report.Element("CustAgenBenSub").Element("_14_Other").Value.Trim(),
                                       _14issuedBy = report.Element("CustAgenBenSub").Element("_14_Issued_by").Value.Trim(),
                                       _14Number = report.Element("CustAgenBenSub").Element("_14_Number").Value.Trim(),
                                       _24a = report.Element("CustAgenBenSub").Element("_24a_Checkbox").Value.Trim(),
                                       _24b = report.Element("CustAgenBenSub").Element("_24b_Checkbox").Value.Trim(),
                                       _25a = report.Element("CustAgenBenSub").Element("_25a_Checkbox").Value.Trim(),
                                       _25b = report.Element("CustAgenBenSub").Element("_25b_Checkbox").Value.Trim(),
                                       _25d = report.Element("CustAgenBenSub").Element("_25d_Checkbox").Value.Trim(),
                                       _25c = report.Element("CustAgenBenSub").Element("_25c_Checkbox").Value.Trim(),
                                       _25other = report.Element("CustAgenBenSub").Element("_25_Other").Value.Trim(),
                                       _25issuedBy = report.Element("CustAgenBenSub").Element("_25_Issued_by").Value.Trim(),
                                       _25Number = report.Element("CustAgenBenSub").Element("_25_Number").Value.Trim(),
                                       cLastName = report.Element("CustAgenBenSub").Element("_7__Individuals_last_name_or_organization_s_name").Value.Trim(),
                                       cFirstName = report.Element("CustAgenBenSub").Element("_8__First_Name").Value.Trim(),
                                       cMI = report.Element("CustAgenBenSub").Element("_9__MI").Value.Trim(),
                                       cAddress = report.Element("CustAgenBenSub").Element("_10__Permanent_Address").Value.Trim(),
                                       cDOB = report.Element("CustAgenBenSub").Element("_11_Date_of_Birth_DDMMYYYY").Value.Trim(),
                                       cTRN = report.Element("CustAgenBenSub").Element("_12__TRN").Value.Trim(),
                                       cAccNo = report.Element("CustAgenBenSub").Element("_15__Customers_Account_No_and_Type").Value.Trim(),
                                       cOccupation = report.Element("CustAgenBenSub").Element("_16__OccupationBusinessPrincipal_Activity").Value.Trim(),
                                       aLastName = report.Element("CustAgenBenSub").Element("_18__Individuals_last_name_or_organization_s_name").Value.Trim(),
                                       aFirstName = report.Element("CustAgenBenSub").Element("_19__First_Name").Value.Trim(),
                                       aMI = report.Element("CustAgenBenSub").Element("_20__MI").Value.Trim(),
                                       aAddress = report.Element("CustAgenBenSub").Element("_21__Permanent_Address").Value.Trim(),
                                       aDOB = report.Element("CustAgenBenSub").Element("_22_Date_of_Birth_DDMMYY").Value.Trim(),
                                       aTRN = report.Element("CustAgenBenSub").Element("_23__TRN").Value.Trim(),
                                       bLastName = report.Element("CustAgenBenSub").Element("_27__Individuals_last_name_or_organizations_name").Value.Trim(),
                                       bFirstName = report.Element("CustAgenBenSub").Element("_28_First_Name").Value.Trim(),
                                       bMI = report.Element("CustAgenBenSub").Element("_29__MI").Value.Trim(),
                                       bAddress = report.Element("CustAgenBenSub").Element("_30__Permanent_Address").Value.Trim(),
                                   }).ToList();

                    #endregion Load Reports from Xml Generated From PDF - TTR

                    rptDataSet = new DataSet();
                    rptDataSet = CreateTablesAndAddToDataSet(rptDataSet);

                    try
                    {
                        #region Break Into Parts

                        List<Customer> custs = new List<Customer> { };

                        foreach (var r in reports)
                        {
                            #region Add Rows to Institution Table

                            DataRow iRow = rptDataSet.Tables["Institution"].NewRow();
                            iRow["ID"] = count;
                            if (r.RepNo == "")
                            {
                                break;
                            }
                            else
                            {
                                iRow["RefNum"] = r.RepNo;
                            }
                            iRow["InstituionName"] = r.instName;
                            iRow["Address"] = r.instAddress;
                            iRow["TRN"] = r.instTRN;
                            iRow["TypeOfInst"] = r.instType;
                            iRow["BranchAddress"] = r.instBranch;
                            if (r.pFirstName != "")
                            {
                                iRow["PreparerFullName"] = r.pLastName + ", " + r.pMI + " " + r.pFirstName;
                            }
                            else
                            {
                                iRow["PreparerFullName"] = r.pLastName;
                            }
                            iRow["PreparerTitle"] = r.pTitle;
                            iRow["DateofSignature"] = r.pDateofSignature;
                            if (r.nFirstName != "")
                            {
                                iRow["ContactFullName"] = r.nLastName + ", " + r.nMI + " " + r.nFirstName;
                            }
                            else
                            {
                                iRow["ContactFullName"] = r.nLastName;
                            }
                            iRow["ContactTitle"] = r.nTitle;
                            iRow["ContactPhoneNum"] = r.nPhoneNo;
                            rptDataSet.Tables["Institution"].Rows.Add(iRow);

                            #endregion Add Rows to Institution Table

                            #region Add Rows to Customer Table

                            //Add Customer Rows
                            DataRow cRow = rptDataSet.Tables["Customer"].NewRow();
                            cRow["ID"] = count;
                            cRow["RefNum"] = r.RepNo;
                            if (r.cFirstName != String.Empty)
                            {
                                cRow["FullName"] = r.cLastName + ", " + r.cMI + " " + r.cFirstName;
                            }
                            else
                            {
                                cRow["FullName"] = r.cLastName;
                            }
                            cRow["Address"] = r.cAddress;
                            cRow["DateofBirth"] = r.cDOB;
                            cRow["TRN"] = r.cTRN;
                            if (r._13a == "Yes")
                            {
                                cRow["VerificationMethod"] = "Examined ID Credential/Document";
                            }
                            else if (r._13b == "Yes")
                            {
                                cRow["VerificationMethod"] = "Known Customer - Infomation on File";
                            }
                            else
                            {
                                cRow["VerificationMethod"] = "none selected";
                            }
                            if (r._14a == "Yes")
                            {
                                cRow["IDType"] = "Driver's License";
                            }
                            else if (r._14b == "Yes")
                            {
                                cRow["IDType"] = "Passport";
                            }
                            else if (r._14c == "Yes")
                            {
                                cRow["IDType"] = "National Id";
                            }
                            else if (r._14d == "Yes")
                            {
                                cRow["IDType"] = r._14other;
                            }
                            else
                            {
                                cRow["IDType"] = "unspecified";
                            }
                            cRow["IssuedBy"] = r._14issuedBy;
                            cRow["ID#"] = r._14Number;
                            cRow["AccNumType"] = r.cAccNo;
                            cRow["Occupation"] = r.cOccupation;
                            rptDataSet.Tables["Customer"].Rows.Add(cRow);

                            #endregion Add Rows to Customer Table

                            #region Add Rows to Agent Table

                            //Add Agent Rows
                            DataRow aRow = rptDataSet.Tables["Agent"].NewRow();
                            aRow["ID"] = count;
                            aRow["RefNum"] = r.RepNo;
                            if (r.aFirstName != String.Empty)
                            {
                                aRow["FullName"] = r.aLastName + ", " + r.aMI + " " + r.aFirstName;
                            }
                            else
                            {
                                aRow["FullName"] = r.aLastName;
                            }
                            aRow["Address"] = r.aAddress;
                            aRow["DateofBirth"] = r.aDOB;
                            aRow["TRN"] = r.aTRN;
                            if (r._24a.ToLower() == "yes")
                            {
                                aRow["VerificationMethod"] = "Examined ID Credential/Document";
                            }
                            else if (r._24b.ToLower() == "yes")
                            {
                                aRow["VerificationMethod"] = "Known Customer - Infomation on File";
                            }
                            else
                            {
                                aRow["VerificationMethod"] = "none selected";
                            }
                            if (r._25a.ToLower() == "yes")
                            {
                                aRow["IDType"] = "Driver's License"; ;
                            }
                            else if (r._25b.ToLower() == "yes")
                            {
                                aRow["IDType"] = "Passport";
                            }
                            else if (r._25c.ToLower() == "yes")
                            {
                                aRow["IDType"] = "National ID";
                            }
                            else if (r._25d.ToLower() == "yes")
                            {
                                aRow["IDType"] = r._25other;
                            }
                            else
                            {
                                aRow["IDType"] = "unspecified";
                            }
                            aRow["IssuedBy"] = r._25issuedBy;
                            aRow["ID#"] = r._25Number;
                            rptDataSet.Tables["Agent"].Rows.Add(aRow);
                            # endregion

                            #region Add Rows to Benficiary Table

                            //Add Beneficiary Rows
                            DataRow bRow = rptDataSet.Tables["Beneficiary"].NewRow();
                            bRow["ID"] = count;
                            bRow["RefNum"] = r.RepNo;
                            if (r.bFirstName != String.Empty)
                            {
                                bRow["FullName"] = r.bLastName + ", " + r.bMI + " " + r.bFirstName;
                            }
                            else
                            {
                                bRow["FullName"] = r.bLastName;
                            }
                            bRow["Address"] = r.bAddress;
                            rptDataSet.Tables["Beneficiary"].Rows.Add(bRow);

                            #endregion Add Rows to Benficiary Table

                            #region Add Rows to Transaction Table

                            DataRow tRow = rptDataSet.Tables["Transaction"].NewRow();
                            tRow["ID"] = count;
                            tRow["RefNum"] = r.RepNo;
                            tRow["Type"] = r.t1Type;
                            tRow["Date"] = r.t1Date;
                            tRow["Time"] = r.t1Time;
                            tRow["Currency"] = r.t1Currency;
                            tRow["Amount"] = Utility.CleanMoneyValue(r.t1Amount);
                            tRow["A/C Type"] = Utility.GetAccType(r.t1Accounts);
                            tRow["A/C Num."] = Utility.GetAccNum(r.t1Accounts);
                            tRow["J$ Equiv"] = Utility.CleanMoneyValue(r.t1JamEquiv);
                            tRow["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t1JamExRate);
                            tRow["US$ Equiv"] = Utility.CleanMoneyValue(r.t1UsEquiv);
                            tRow["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t1UsExRate);
                            tRow["Funds Source"] = r.t1SourceofFunds;
                            rptDataSet.Tables["Transaction"].Rows.Add(tRow);

                            if ((r.t2Amount != String.Empty) && (r.t2Amount != "0"))
                            {
                                DataRow t2Row = rptDataSet.Tables["Transaction"].NewRow();
                                t2Row["ID"] = count;
                                t2Row["RefNum"] = r.RepNo;
                                t2Row["Type"] = r.t2Type;
                                t2Row["Date"] = r.t2Date;
                                t2Row["Time"] = r.t2Time;
                                t2Row["Currency"] = r.t2Currency;
                                t2Row["Amount"] = Utility.CleanMoneyValue(r.t2Amount);
                                t2Row["A/C Type"] = Utility.GetAccType(r.t2Accounts);
                                t2Row["A/C Num."] = Utility.GetAccNum(r.t2Accounts);
                                t2Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t2JamEquiv);
                                t2Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2JamExRate);
                                t2Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t2UsEquiv);
                                t2Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                                t2Row["Funds Source"] = r.t2SourceofFunds;
                                rptDataSet.Tables["Transaction"].Rows.Add(t2Row);
                            }

                            if ((r.t3Amount != String.Empty) && (r.t3Amount != "0"))
                            {
                                DataRow t3Row = rptDataSet.Tables["Transaction"].NewRow();
                                t3Row["ID"] = count;
                                t3Row["RefNum"] = r.RepNo;
                                t3Row["Type"] = r.t3Type;
                                t3Row["Date"] = r.t3Date;
                                t3Row["Time"] = r.t3Time;
                                t3Row["Currency"] = r.t3Currency;
                                t3Row["Amount"] = Utility.CleanMoneyValue(r.t3Amount);
                                t3Row["A/C Type"] = Utility.GetAccType(r.t3Accounts);
                                t3Row["A/C Num."] = Utility.GetAccNum(r.t3Accounts);
                                t3Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t3JamEquiv);
                                t3Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t3JamExRate);
                                t3Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t3UsEquiv);
                                t3Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                                t3Row["Funds Source"] = r.t3SourceofFunds;
                                rptDataSet.Tables["Transaction"].Rows.Add(t3Row);
                            }

                            if ((r.t4Amount != String.Empty) && (r.t4Amount != "0"))
                            {
                                DataRow t4Row = rptDataSet.Tables["Transaction"].NewRow();
                                t4Row["ID"] = count;
                                t4Row["RefNum"] = r.RepNo;
                                t4Row["Type"] = r.t4Type;
                                t4Row["Date"] = r.t4Date;
                                t4Row["Time"] = r.t4Time;
                                t4Row["Currency"] = Utility.CleanMoneyValue(r.t4Currency);
                                t4Row["Amount"] = r.t4Amount;
                                t4Row["A/C Type"] = Utility.GetAccType(r.t4Accounts);
                                t4Row["A/C Num."] = Utility.GetAccNum(r.t4Accounts);
                                t4Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t4JamEquiv);
                                t4Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t4JamExRate);
                                t4Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t4UsEquiv);
                                t4Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                                t4Row["Funds Source"] = r.t4SourceofFunds;
                                rptDataSet.Tables["Transaction"].Rows.Add(t4Row);
                            }

                            #endregion Add Rows to Transaction Table

                            count++;
                        }

                        #region Create Review Grids Templates

                        this.radGrid.Columns.Clear();
                        this.radGrid.MasterTemplate.Templates.Clear();
                        this.radGrid.DataSource = rptDataSet.Tables[0];
                        this.radGrid.MasterTemplate.Columns["ID"].IsVisible = false;
                        if (batchType == "TTR")
                        {
                            this.radGrid.MasterTemplate.Columns["Reason"].IsVisible = false;
                        }

                        GridViewTemplate cusTemplate = new GridViewTemplate();
                        cusTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        cusTemplate.ReadOnly = true;
                        cusTemplate.AllowAddNewRow = false;
                        cusTemplate.Caption = "Customer(s)";
                        this.radGrid.MasterTemplate.Templates.Add(cusTemplate);
                        cusTemplate.DataSource = rptDataSet.Tables[1];
                        cusTemplate.Columns["ID"].IsVisible = false;

                        GridViewRelation relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = cusTemplate;
                        relation.RelationName = "Instituion_Customers";
                        relation.ParentColumnNames.Add("ID");
                        relation.ChildColumnNames.Add("ID");
                        radGrid.Relations.Add(relation);

                        GridViewTemplate agentTemplate = new GridViewTemplate();
                        agentTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        agentTemplate.AllowAddNewRow = false;
                        agentTemplate.ReadOnly = true;
                        agentTemplate.Caption = "Agent(s)";
                        this.radGrid.MasterTemplate.Templates.Add(agentTemplate);
                        agentTemplate.DataSource = rptDataSet.Tables[2];
                        agentTemplate.Columns["ID"].IsVisible = false;

                        relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = agentTemplate;
                        relation.RelationName = "Institution_Agents";
                        relation.ParentColumnNames.Add("ID");
                        relation.ChildColumnNames.Add("ID");
                        radGrid.Relations.Add(relation);

                        GridViewTemplate benfTemplate = new GridViewTemplate();
                        benfTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        benfTemplate.AllowAddNewRow = false;
                        benfTemplate.ReadOnly = true;
                        benfTemplate.Caption = "Beneficiarie(s)";
                        this.radGrid.MasterTemplate.Templates.Add(benfTemplate);
                        benfTemplate.DataSource = rptDataSet.Tables[3];
                        benfTemplate.Columns["ID"].IsVisible = false;

                        relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = benfTemplate;
                        relation.RelationName = "Institution_Beneficaries";
                        relation.ParentColumnNames.Add("ID");
                        relation.ChildColumnNames.Add("ID");
                        radGrid.Relations.Add(relation);

                        GridViewTemplate transTemplate = new GridViewTemplate();
                        transTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        transTemplate.AllowAddNewRow = false;
                        transTemplate.ReadOnly = true;
                        transTemplate.Caption = "Transaction(s)";
                        this.radGrid.MasterTemplate.Templates.Add(transTemplate);
                        transTemplate.DataSource = rptDataSet.Tables[4];
                        transTemplate.Columns["ID"].IsVisible = false;
                        transTemplate.Columns["Amount"].FormatString = "{0:$#,###0.00;($#,###0.00);0}";

                        relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = transTemplate;
                        relation.RelationName = "Institution_Trans";
                        relation.ParentColumnNames.Add("ID");
                        relation.ChildColumnNames.Add("ID");
                        radGrid.Relations.Add(relation);

                        //this.radGrid.MasterTemplate.BestFitColumns();
                        //this.radGrid.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        this.radGrid.MasterTemplate.Columns["DateofSignature"].FormatString = "{0:dd/MM/yyyy}";

                        ShowSummaryInformation();

                        #endregion Create Review Grids Templates

                        #endregion Add Rows to Agent Table
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                try
                {
                    #region Load Reports from Xml Generated From PDF - STR

                    var reports = (from report in xmlDoc.Descendants("ttr_Frm")
                                   select new
                                   {
                                       instName = report.Element("_1__Name_of_Financial_Institution").Value.Trim(),
                                       RepNo = report.Element("_ref_num_pg1").Value.Trim(),
                                       RepReason = report.Element("Reason_for_suspicion").Value.Trim(),
                                       instAddress = report.Element("_2_Address_of_Financial_Institution").Value.Trim(),
                                       instTRN = report.Element("_3_TRN").Value.Trim(),
                                       instType = report.Element("_5__Type_of_Financial_Institution").Value.Trim(),
                                       instBranch = report.Element("_4__Branch_Address").Value.Trim(),
                                       pLastName = report.Element("_31__Last_Name").Value.Trim(),
                                       pFirstName = report.Element("_32__First_Name").Value.Trim(),
                                       pFullName = report.Element("_32__First_Name").Value.Trim() + " " + report.Element("_31__Last_Name").Value.Trim(),
                                       pMI = report.Element("_33__MI").Value.Trim(),
                                       pTitle = report.Element("_34__Title").Value.Trim(),
                                       pPhoneNo = report.Element("_35__Phone_No").Value.Trim(),
                                       pDateofSignature = report.Element("_37_Date_of_Signature_DDMMYYYY").Value.Trim(),
                                       nLastName = report.Element("_38__Last_Name").Value.Trim(),
                                       nFirstName = report.Element("_39_First_Name").Value.Trim(),
                                       nFullName = report.Element("_39_First_Name").Value.Trim() + " " + report.Element("_38__Last_Name").Value.Trim(),
                                       nMI = report.Element("_40__MI").Value.Trim(),
                                       nTitle = report.Element("_41__Title").Value.Trim(),
                                       nPhoneNo = report.Element("_42__Phone_No").Value.Trim(),
                                       t1Type = report.Element("_2_Transaction_Type").Value.Trim(),
                                       t1Date = report.Element("_3_Date_DDMMYYYY").Value.Trim(),
                                       t1Time = report.Element("_4_Time").Value.Trim(),
                                       t1Currency = report.Element("_5__Transaction_Type").Value.Trim(),
                                       t1Amount = report.Element("_6__Transaction_Amount").Value.Trim(),
                                       t1Accounts = report.Element("_7_Type1").Value.ToString().Trim() + " " + report.Element("_7_Type2").Value.ToString().Trim() + " " + report.Element("_7_Type3").Value.ToString().Trim(),
                                       t1JamEquiv = report.Element("_8__JA_Equivalent").Value.Trim(),
                                       t1JamExRate = report.Element("_9__JA_Exchange_Rate").Value.Trim(),
                                       t1UsEquiv = report.Element("_10__US_Equivalent").Value.Trim(),
                                       t1UsExRate = report.Element("_11__US_Exchange_Rate").Value.Trim(),
                                       t1SourceofFunds = report.Element("_12__Source_of_funds").Value.Trim(),
                                       t2Type = report.Element("_13__Transaction_Type").Value.Trim(),
                                       t2Date = report.Element("_14__Date_DDMMYYYY").Value.Trim(),
                                       t2Time = report.Element("_15_Time").Value.Trim(),
                                       t2Currency = report.Element("_16__Transaction_Currency").Value.Trim(),
                                       t2Amount = report.Element("_17__Transaction_Amount").Value.Trim(),
                                       t2Accounts = report.Element("_18_Type1").Value.ToString().Trim() + " " + report.Element("_18_Type2").Value.ToString().Trim() + " " + report.Element("_18_Type3").Value.ToString().Trim(),
                                       t2JamEquiv = report.Element("_19__JA_Equivalent").Value.Trim(),
                                       t2JamExRate = report.Element("_20__JA_Exchange_Rate").Value.Trim(),
                                       t2UsEquiv = report.Element("_21__US_Equivalent").Value.Trim(),
                                       t2UsExRate = report.Element("_22__US_Exchange_Rate").Value.Trim(),
                                       t2SourceofFunds = report.Element("_23__Source_of_funds").Value.Trim(),
                                       t3Type = report.Element("_24_Transaction_Type").Value.Trim(),
                                       t3Date = report.Element("_25_Date_DDMMYYYY").Value.Trim(),
                                       t3Time = report.Element("_26__Time").Value.Trim(),
                                       t3Currency = report.Element("_27__Transaction_Currency").Value.Trim(),
                                       t3Amount = report.Element("_28__Transaction_Amount").Value.Trim(),
                                       t3Accounts = report.Element("_29_Type1").Value.ToString().Trim() + " " + report.Element("_29_Type2").Value.ToString().Trim() + " " + report.Element("_29_Type3").Value.ToString().Trim(),
                                       t3JamEquiv = report.Element("_30__JA_Equivalent").Value.Trim(),
                                       t3JamExRate = report.Element("_31__JA_Exchange_Rate").Value.Trim(),
                                       t3UsEquiv = report.Element("_32__US_Equivalent").Value.Trim(),
                                       t3UsExRate = report.Element("_33__US_Exchange_Rate").Value.Trim(),
                                       t3SourceofFunds = report.Element("_34__Source_of_funds").Value.Trim(),
                                       t4Type = report.Element("_35__Transaction_Type").Value.Trim(),
                                       t4Date = report.Element("_36_Date_DDMMYYYY").Value.Trim(),
                                       t4Time = report.Element("_37__Time").Value.Trim(),
                                       t4Currency = report.Element("_38__Transaction_Currency").Value.Trim(),
                                       t4Amount = report.Element("_39__Transaction_Amount").Value.Trim(),
                                       t4Accounts = report.Element("_40_Type1").Value.ToString().Trim() + " " + report.Element("_40_Type2").Value.ToString().Trim() + " " + report.Element("_40_Type3").Value.ToString().Trim(),
                                       t4JamEquiv = report.Element("_41__JA_Equivalent").Value.Trim(),
                                       t4JamExRate = report.Element("_42__JA_Exchange_Rate").Value.Trim(),
                                       t4UsEquiv = report.Element("_43__US_Equivalent").Value.Trim(),
                                       t4UsExRate = report.Element("_44__US_Exchange_Rate").Value.Trim(),
                                       t4SourceofFunds = report.Element("_45__Source_of_funds").Value.Trim(),
                                       _13a = report.Element("CustAgenBenSub").Element("_13a_Checkbox").Value.Trim(),
                                       _13b = report.Element("CustAgenBenSub").Element("_13b_Checkbox").Value.Trim(),
                                       _14a = report.Element("CustAgenBenSub").Element("_14a_Checkbox").Value.Trim(),
                                       _14b = report.Element("CustAgenBenSub").Element("_14b_Checkbox").Value.Trim(),
                                       _14c = report.Element("CustAgenBenSub").Element("_14c_Checkbox").Value.Trim(),
                                       _14d = report.Element("CustAgenBenSub").Element("_14d_Checkbox").Value.Trim(),
                                       _14other = report.Element("CustAgenBenSub").Element("_14_Other").Value.Trim(),
                                       _14issuedBy = report.Element("CustAgenBenSub").Element("_14_Issued_by").Value.Trim(),
                                       _14Number = report.Element("CustAgenBenSub").Element("_14_Number").Value.Trim(),
                                       _24a = report.Element("CustAgenBenSub").Element("_24a_Checkbox").Value.Trim(),
                                       _24b = report.Element("CustAgenBenSub").Element("_24b_Checkbox").Value.Trim(),
                                       _25a = report.Element("CustAgenBenSub").Element("_25a_Checkbox").Value.Trim(),
                                       _25b = report.Element("CustAgenBenSub").Element("_25b_Checkbox").Value.Trim(),
                                       _25d = report.Element("CustAgenBenSub").Element("_25d_Checkbox").Value.Trim(),
                                       _25c = report.Element("CustAgenBenSub").Element("_25c_Checkbox").Value.Trim(),
                                       _25other = report.Element("CustAgenBenSub").Element("_25_Other").Value.Trim(),
                                       _25issuedBy = report.Element("CustAgenBenSub").Element("_25_Issued_by").Value.Trim(),
                                       _25Number = report.Element("CustAgenBenSub").Element("_25_Number").Value.Trim(),
                                       cLastName = report.Element("CustAgenBenSub").Element("_7__Individuals_last_name_or_organization_s_name").Value.Trim(),
                                       cFirstName = report.Element("CustAgenBenSub").Element("_8__First_Name").Value.Trim(),
                                       cMI = report.Element("CustAgenBenSub").Element("_9__MI").Value.Trim(),
                                       cAddress = report.Element("CustAgenBenSub").Element("_10__Permanent_Address").Value.Trim(),
                                       cDOB = report.Element("CustAgenBenSub").Element("_11_Date_of_Birth_DDMMYYYY").Value.Trim(),
                                       cTRN = report.Element("CustAgenBenSub").Element("_12__TRN").Value.Trim(),
                                       cAccNo = report.Element("CustAgenBenSub").Element("_15__Customers_Account_No_and_Type").Value.Trim(),
                                       cOccupation = report.Element("CustAgenBenSub").Element("_16__OccupationBusinessPrincipal_Activity").Value.Trim(),
                                       aLastName = report.Element("CustAgenBenSub").Element("_18__Individuals_last_name_or_organization_s_name").Value.Trim(),
                                       aFirstName = report.Element("CustAgenBenSub").Element("_19__First_Name").Value.Trim(),
                                       aMI = report.Element("CustAgenBenSub").Element("_20__MI").Value.Trim(),
                                       aAddress = report.Element("CustAgenBenSub").Element("_21__Permanent_Address").Value.Trim(),
                                       aDOB = report.Element("CustAgenBenSub").Element("_22_Date_of_Birth_DDMMYY").Value.Trim(),
                                       aTRN = report.Element("CustAgenBenSub").Element("_23__TRN").Value.Trim(),
                                       bLastName = report.Element("CustAgenBenSub").Element("_27__Individuals_last_name_or_organizations_name").Value.Trim(),
                                       bFirstName = report.Element("CustAgenBenSub").Element("_28_First_Name").Value.Trim(),
                                       bMI = report.Element("CustAgenBenSub").Element("_29__MI").Value.Trim(),
                                       bAddress = report.Element("CustAgenBenSub").Element("_30__Permanent_Address").Value.Trim(),
                                   }).ToList();

                    #endregion Load Reports from Xml Generated From PDF - STR

                    rptDataSet = new DataSet();
                    rptDataSet = CreateTablesAndAddToDataSet(rptDataSet);

                    try
                    {
                        #region Break Into Parts

                        List<Customer> custs = new List<Customer> { };

                        foreach (var r in reports)
                        {
                            #region Add Rows to Institution Table

                            DataRow iRow = rptDataSet.Tables["Institution"].NewRow();
                            iRow["ID"] = count;
                            if (r.RepNo == "")
                            {
                                break;
                            }
                            else
                            {
                                iRow["RefNum"] = r.RepNo;
                            }
                            iRow["InstituionName"] = r.instName;
                            iRow["Address"] = r.instAddress;
                            iRow["TRN"] = r.instTRN;
                            iRow["TypeOfInst"] = r.instType;
                            iRow["BranchAddress"] = r.instBranch;
                            if (r.pFirstName != "")
                            {
                                iRow["PreparerFullName"] = r.pLastName + ", " + r.pMI + " " + r.pFirstName;
                            }
                            else
                            {
                                iRow["PreparerFullName"] = r.pLastName;
                            }
                            iRow["PreparerTitle"] = r.pTitle;
                            iRow["DateofSignature"] = r.pDateofSignature;
                            if (r.nFirstName != "")
                            {
                                iRow["ContactFullName"] = r.nLastName + ", " + r.nMI + " " + r.nFirstName;
                            }
                            else
                            {
                                iRow["ContactFullName"] = r.nLastName;
                            }
                            iRow["ContactTitle"] = r.nTitle;
                            iRow["ContactPhoneNum"] = r.nPhoneNo;
                            iRow["Reason"] = r.RepReason;
                            rptDataSet.Tables["Institution"].Rows.Add(iRow);

                            #endregion Add Rows to Institution Table

                            #region Add Rows to Customer Table

                            //Add Customer Rows
                            DataRow cRow = rptDataSet.Tables["Customer"].NewRow();
                            cRow["ID"] = count;
                            cRow["RefNum"] = r.RepNo;
                            if (r.cFirstName != String.Empty)
                            {
                                cRow["FullName"] = r.cLastName + ", " + r.cMI + " " + r.cFirstName;
                            }
                            else
                            {
                                cRow["FullName"] = r.cLastName;
                            }
                            cRow["Address"] = r.cAddress;
                            cRow["DateofBirth"] = r.cDOB;
                            cRow["TRN"] = r.cTRN;
                            if (r._13a == "Yes")
                            {
                                cRow["VerificationMethod"] = "Examined ID Credential/Document";
                            }
                            else if (r._13b == "Yes")
                            {
                                cRow["VerificationMethod"] = "Known Customer - Infomation on File";
                            }
                            else
                            {
                                cRow["VerificationMethod"] = "none selected";
                            }
                            if (r._14a == "Yes")
                            {
                                cRow["IDType"] = "Driver's License";
                            }
                            else if (r._14b == "Yes")
                            {
                                cRow["IDType"] = "Passport";
                            }
                            else if (r._14c == "Yes")
                            {
                                cRow["IDType"] = "National Id";
                            }
                            else if (r._14d == "Yes")
                            {
                                cRow["IDType"] = r._14other;
                            }
                            else
                            {
                                cRow["IDType"] = "unspecified";
                            }
                            cRow["IssuedBy"] = r._14issuedBy;
                            cRow["ID#"] = r._14Number;
                            cRow["AccNumType"] = r.cAccNo;
                            cRow["Occupation"] = r.cOccupation;
                            rptDataSet.Tables["Customer"].Rows.Add(cRow);

                            #endregion Add Rows to Customer Table

                            #region Add Rows to Agent Table

                            //Add Agent Rows
                            DataRow aRow = rptDataSet.Tables["Agent"].NewRow();
                            aRow["ID"] = count;
                            aRow["RefNum"] = r.RepNo;
                            if (r.aFirstName != String.Empty)
                            {
                                aRow["FullName"] = r.aLastName + ", " + r.aMI + " " + r.aFirstName;
                            }
                            else
                            {
                                aRow["FullName"] = r.aLastName;
                            }
                            aRow["Address"] = r.aAddress;
                            aRow["DateofBirth"] = r.aDOB;
                            aRow["TRN"] = r.aTRN;
                            if (r._24a.ToLower() == "yes")
                            {
                                aRow["VerificationMethod"] = "Examined ID Credential/Document";
                            }
                            else if (r._24b.ToLower() == "yes")
                            {
                                aRow["VerificationMethod"] = "Known Customer - Infomation on File";
                            }
                            else
                            {
                                aRow["VerificationMethod"] = "none selected";
                            }
                            if (r._25a.ToLower() == "yes")
                            {
                                aRow["IDType"] = "Driver's License"; ;
                            }
                            else if (r._25b.ToLower() == "yes")
                            {
                                aRow["IDType"] = "Passport";
                            }
                            else if (r._25c.ToLower() == "yes")
                            {
                                aRow["IDType"] = "National ID";
                            }
                            else if (r._25d.ToLower() == "yes")
                            {
                                aRow["IDType"] = r._25other;
                            }
                            else
                            {
                                aRow["IDType"] = "unspecified";
                            }
                            aRow["IssuedBy"] = r._25issuedBy;
                            aRow["ID#"] = r._25Number;
                            rptDataSet.Tables["Agent"].Rows.Add(aRow);
                            # endregion

                            #region Add Rows to Benficiary Table

                            //Add Beneficiary Rows
                            DataRow bRow = rptDataSet.Tables["Beneficiary"].NewRow();
                            bRow["ID"] = count;
                            bRow["RefNum"] = r.RepNo;
                            if (r.bFirstName != String.Empty)
                            {
                                bRow["FullName"] = r.bLastName + ", " + r.bMI + " " + r.bFirstName;
                            }
                            else
                            {
                                bRow["FullName"] = r.bLastName;
                            }
                            bRow["Address"] = r.bAddress;
                            rptDataSet.Tables["Beneficiary"].Rows.Add(bRow);

                            #endregion Add Rows to Benficiary Table

                            #region Add Rows to Transaction Table

                            DataRow tRow = rptDataSet.Tables["Transaction"].NewRow();
                            tRow["ID"] = count;
                            tRow["RefNum"] = r.RepNo;
                            tRow["Type"] = r.t1Type;
                            tRow["Date"] = r.t1Date;
                            tRow["Time"] = r.t1Time;
                            tRow["Currency"] = r.t1Currency;
                            tRow["Amount"] = Utility.CleanMoneyValue(r.t1Amount);
                            tRow["A/C Type"] = Utility.GetAccType(r.t1Accounts);
                            tRow["A/C Num."] = Utility.GetAccNum(r.t1Accounts);
                            tRow["J$ Equiv"] = Utility.CleanMoneyValue(r.t1JamEquiv);
                            tRow["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t1JamExRate);
                            tRow["US$ Equiv"] = Utility.CleanMoneyValue(r.t1UsEquiv);
                            tRow["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t1UsExRate);
                            tRow["Funds Source"] = r.t1SourceofFunds;
                            rptDataSet.Tables["Transaction"].Rows.Add(tRow);

                            if ((r.t2Amount != String.Empty) && (r.t2Amount != "0"))
                            {
                                DataRow t2Row = rptDataSet.Tables["Transaction"].NewRow();
                                t2Row["ID"] = count;
                                t2Row["RefNum"] = r.RepNo;
                                t2Row["Type"] = r.t2Type;
                                t2Row["Date"] = r.t2Date;
                                t2Row["Time"] = r.t2Time;
                                t2Row["Currency"] = r.t2Currency;
                                t2Row["Amount"] = Utility.CleanMoneyValue(r.t2Amount);
                                t2Row["A/C Type"] = Utility.GetAccType(r.t2Accounts);
                                t2Row["A/C Num."] = Utility.GetAccNum(r.t2Accounts);
                                t2Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t2JamEquiv);
                                t2Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2JamExRate);
                                t2Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t2UsEquiv);
                                t2Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                                t2Row["Funds Source"] = r.t2SourceofFunds;
                                rptDataSet.Tables["Transaction"].Rows.Add(t2Row);
                            }

                            if ((r.t3Amount != String.Empty) && (r.t3Amount != "0"))
                            {
                                DataRow t3Row = rptDataSet.Tables["Transaction"].NewRow();
                                t3Row["ID"] = count;
                                t3Row["RefNum"] = r.RepNo;
                                t3Row["Type"] = r.t3Type;
                                t3Row["Date"] = r.t3Date;
                                t3Row["Time"] = r.t3Time;
                                t3Row["Currency"] = r.t3Currency;
                                t3Row["Amount"] = Utility.CleanMoneyValue(r.t3Amount);
                                t3Row["A/C Type"] = Utility.GetAccType(r.t3Accounts);
                                t3Row["A/C Num."] = Utility.GetAccNum(r.t3Accounts);
                                t3Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t3JamEquiv);
                                t3Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t3JamExRate);
                                t3Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t3UsEquiv);
                                t3Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                                t3Row["Funds Source"] = r.t3SourceofFunds;
                                rptDataSet.Tables["Transaction"].Rows.Add(t3Row);
                            }

                            if ((r.t4Amount != String.Empty) && (r.t4Amount != "0"))
                            {
                                DataRow t4Row = rptDataSet.Tables["Transaction"].NewRow();
                                t4Row["ID"] = count;
                                t4Row["RefNum"] = r.RepNo;
                                t4Row["Type"] = r.t4Type;
                                t4Row["Date"] = r.t4Date;
                                t4Row["Time"] = r.t4Time;
                                t4Row["Currency"] = Utility.CleanMoneyValue(r.t4Currency);
                                t4Row["Amount"] = r.t4Amount;
                                t4Row["A/C Type"] = Utility.GetAccType(r.t4Accounts);
                                t4Row["A/C Num."] = Utility.GetAccNum(r.t4Accounts);
                                t4Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t4JamEquiv);
                                t4Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t4JamExRate);
                                t4Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t4UsEquiv);
                                t4Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                                t4Row["Funds Source"] = r.t4SourceofFunds;
                                rptDataSet.Tables["Transaction"].Rows.Add(t4Row);
                            }

                            #endregion Add Rows to Transaction Table

                            count++;
                        }

                        #region Create Review Grids Templates

                        this.radGrid.Columns.Clear();
                        this.radGrid.MasterTemplate.Templates.Clear();
                        this.radGrid.DataSource = rptDataSet.Tables[0];
                        this.radGrid.MasterTemplate.Columns["ID"].IsVisible = false;
                        SetPreferences();

                        if (batchType == "TTR")
                        {
                            this.radGrid.MasterTemplate.Columns["Reason"].IsVisible = false;
                        }

                        GridViewTemplate cusTemplate = new GridViewTemplate();
                        cusTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        cusTemplate.ReadOnly = true;
                        cusTemplate.AllowAddNewRow = false;
                        cusTemplate.Caption = "Customer(s)";
                        this.radGrid.MasterTemplate.Templates.Add(cusTemplate);
                        cusTemplate.DataSource = rptDataSet.Tables[1];
                        cusTemplate.Columns["ID"].IsVisible = false;

                        GridViewRelation relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = cusTemplate;
                        relation.RelationName = "Instituion_Customers";
                        relation.ParentColumnNames.Add("ID");
                        relation.ChildColumnNames.Add("ID");
                        radGrid.Relations.Add(relation);

                        GridViewTemplate agentTemplate = new GridViewTemplate();
                        agentTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        agentTemplate.AllowAddNewRow = false;
                        agentTemplate.ReadOnly = true;
                        agentTemplate.Caption = "Agent(s)";
                        this.radGrid.MasterTemplate.Templates.Add(agentTemplate);
                        agentTemplate.DataSource = rptDataSet.Tables[2];
                        agentTemplate.Columns["ID"].IsVisible = false;

                        relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = agentTemplate;
                        relation.RelationName = "Institution_Agents";
                        relation.ParentColumnNames.Add("ID");
                        relation.ChildColumnNames.Add("ID");
                        radGrid.Relations.Add(relation);

                        GridViewTemplate benfTemplate = new GridViewTemplate();
                        benfTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        benfTemplate.AllowAddNewRow = false;
                        benfTemplate.ReadOnly = true;
                        benfTemplate.Caption = "Beneficiarie(s)";
                        this.radGrid.MasterTemplate.Templates.Add(benfTemplate);
                        benfTemplate.DataSource = rptDataSet.Tables[3];
                        benfTemplate.Columns["ID"].IsVisible = false;

                        relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = benfTemplate;
                        relation.RelationName = "Institution_Beneficaries";
                        relation.ParentColumnNames.Add("ID");
                        relation.ChildColumnNames.Add("ID");
                        radGrid.Relations.Add(relation);

                        GridViewTemplate transTemplate = new GridViewTemplate();
                        transTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        transTemplate.AllowAddNewRow = false;
                        transTemplate.ReadOnly = true;
                        transTemplate.Caption = "Transaction(s)";
                        this.radGrid.MasterTemplate.Templates.Add(transTemplate);
                        transTemplate.DataSource = rptDataSet.Tables[4];
                        transTemplate.Columns["ID"].IsVisible = false;
                        transTemplate.Columns["Amount"].FormatString = "{0:$#,###0.00;($#,###0.00);0}";

                        relation = new GridViewRelation(this.radGrid.MasterTemplate);
                        relation.ChildTemplate = transTemplate;
                        relation.RelationName = "Institution_Trans";
                        relation.ParentColumnNames.Add("ID");
                        relation.ChildColumnNames.Add("ID");
                        radGrid.Relations.Add(relation);

                        //this.radGrid.MasterTemplate.BestFitColumns();
                        //this.radGrid.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        this.radGrid.MasterTemplate.Columns["DateofSignature"].FormatString = "{0:dd/MM/yyyy}";

                        ShowSummaryInformation();

                        #endregion Create Review Grids Templates

                        #endregion Add Rows to Agent Table
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void radGrid_GroupSummaryEvaluate(object sender, GroupSummaryEvaluationEventArgs e)
        {
            if ((e.SummaryItem.Name == "InstitutionName") || (e.SummaryItem.Name == "PreparerFullName") || (e.SummaryItem.Name == "ContactFullName"))
            {
                e.FormatString = String.Format("{0} has {1} reports in this batch", e.Value, e.Group.ItemCount);
            }
            if (e.SummaryItem.Name == "TypeOfInst")
            {
                e.FormatString = String.Format("{0} reports are of type: {1} ", e.Group.ItemCount, e.Value);
            }
            if (e.SummaryItem.Name == "BranchAddress")
            {
                e.FormatString = String.Format("{0} BRANCH has {1} report(s) in this batch", e.Value, e.Group.ItemCount);
            }
            if (e.SummaryItem.Name == "TRN")
            {
                e.FormatString = String.Format("{0} reports under #: {1} ", e.Group.ItemCount, e.Value);
            }
        }

        private void loadPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //create the open file dialog variable
            foldersDialog = new FolderBrowserDialog();

            //OpenFileDialog fDialog = new OpenFileDialog();

            //set the title of the window
            foldersDialog.Description = "Open Folders for PDF Review";
            foldersDialog.ShowNewFolderButton = false;

            //clear the filename viewer
            _parent.lblCurrFile.Text = String.Empty;

            //Set Virtual Drive Path
            foldersDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            foldersDialog.SelectedPath = @"P:\";

            if (foldersDialog.ShowDialog() == DialogResult.OK)
            {
                _parent.btnUpload.Enabled = false;
                List<ReportEO.Institution> instPdfList = new List<ReportEO.Institution>();
                List<ReportEO.Institution> filteredInstPdfList = new List<ReportEO.Institution>();
                List<ReportEO.Institution> shortInstPdfList = new List<ReportEO.Institution>();
                List<ReportEO.Customer> custPdfList = new List<ReportEO.Customer>();
                List<ReportEO.Agent> agentPdfList = new List<ReportEO.Agent>();
                List<ReportEO.Beneficiary> benPdfList = new List<ReportEO.Beneficiary>();
                List<ReportEO.Transaction> transPdfList = new List<ReportEO.Transaction>();
                List<ReportEO.Reason> repPdfReasonList = new List<ReportEO.Reason>();

                    string[] allFiles = Directory.GetFiles(foldersDialog.SelectedPath.ToString(), "*.pdf", SearchOption.AllDirectories);
                    pdfFiles = allFiles.ToList();
                    DialogResult result = MessageBox.Show("This directory contains " + pdfFiles.Count + ", do you wish to proceed?", "File Count", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    //this.loadPDFToolStripMenuItem.PerformClick();

                    reviewPopupfrm = new frmReview(this);
                    reviewPopupfrm.radTrkBar.Maximum = pdfFiles.Count;
                    reviewPopupfrm.radTextBox1.Text = pdfFiles.Count.ToString();
                    reviewPopupfrm.ShowDialog();

                    fileReviewPercent = (int)reviewPopupfrm.radTrkBar.Value;

                    if (fileReviewPercent < pdfFiles.Count)
                    {
                        randomFileIndx = Parser.RandomNumberFromAGivenSetOfNumbers(1, fileReviewPercent);
                        pdfFilesPercentage = Parser.RandomizePdfFilesForReview(pdfFiles, randomFileIndx);
                    }
                    else
                    {
                        pdfFilesPercentage = pdfFiles;
                    }
                    _parent.radProgressBar2.Maximum = (int)fileReviewPercent;
                    worker = new BackgroundWorker();
                    worker.RunWorkerAsync();

                    strPdfFiles = new List<string>();
                    filePath = new DirectoryInfo(foldersDialog.SelectedPath).ToString();
                    if ((Path.GetFileNameWithoutExtension(filePath.ToLower()).Contains("str")) || (Path.GetFileNameWithoutExtension(filePath.ToLower()).Contains("sar")))
                    {
                        batchType = "STR";
                    }
                    else
                    {
                        batchType = "TTR";
                    }

                    while (worker.IsBusy)
                    {
                        foreach (var pdf in pdfFilesPercentage)
                        {
                            DirectoryInfo info = new DirectoryInfo(pdf);
                            _parent.lblCurrFile.Text = Path.GetFileName(pdf);
                            _parent.lblCurrDir.Text = info.Parent.ToString();
                            
                            //This gives the full path and I only wanted the last subdirectory, hence the code above 
                            //lblCurrDir.Text = System.IO.Directory.GetParent(pdf).ToString();
                            
                            Parser.ReadPdfFile(pdf);
                            instPdfList.Add(Parser.ReadPdfInstitutionHeaders(pdf));
                            custPdfList.AddRange(Parser.ReadPdfCustomers(pdf));
                            agentPdfList.AddRange(Parser.ReadPdfAgents(pdf));
                            benPdfList.AddRange(Parser.ReadPdfBeneficiaries(pdf));
                            transPdfList.AddRange(Parser.ReadPdfTransactions(pdf));
                            //if (pdf.Contains("STR"))
                            if(batchType == "STR")
                            {
                                repPdfReasonList.AddRange(Parser.ReadPdfReason(pdf));
                                strPdfFiles.Add(pdf);
                            }

                            _parent.radProgressBar2.Value1 = count + 1;
                            Application.DoEvents();

                            if (_parent.radProgressBar2.Value1 == (int)_parent.radProgressBar2.Maximum)
                            {
                                //countOfAllFiles = 0;
                            }

                            count++;
                        }

                        for (int i = 0; i < instPdfList.Count; i++)
                        {
                            if (instPdfList[i].refNum.Length <= 18)
                            {
                                shortInstPdfList.Add(instPdfList[i]);
                                //System.Diagnostics.Debug.WriteLine(shortInstPdfList[i].refNum);
                                System.Diagnostics.Debug.WriteLine(instPdfList[i].refNum);
                            }


                        }
                    }

                    filteredInstPdfList = instPdfList.Where(item => item.refNum != null).ToList();

                    if (filteredInstPdfList.Count <= 0)
                    {
                        MessageBox.Show("There were no valid pdf files found in this directory!", _parent.lblCurrDir.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        MessageBox.Show("Please report to ITU and try another directory!", "Suggestion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        rptDataSet = new DataSet();
                        rptDataSet = CreateTablesAndAddToDataSet(rptDataSet);

                        //Add rows to tables in memory//
                        //Parser.AddRowsToInstitutionTable(rptDataSet, instPdfList);
                        //for (int i = 0; i < filteredInstPdfList.Count; i++)
                        //{
                        //    if (filteredInstPdfList[i].refNum.Length <= 18)
                        //    {
                        //        shortInstPdfList.Add(filteredInstPdfList[i]);

                        //    }
                        //}
                        Parser.AddRowsToInstitutionTable(rptDataSet, filteredInstPdfList);
                        Parser.AddRowsToCustomerTable(rptDataSet, custPdfList);
                        Parser.AddRowsToAgentsTable(rptDataSet, agentPdfList);
                        Parser.AddRowsToBeneficiaryTable(rptDataSet, benPdfList);
                        Parser.AddRowsToTransactionTable(rptDataSet, transPdfList);
                        if (strPdfFiles.Count > 0)
                        {
                            batchType = "STR";
                            Parser.AddRowstoReportReasonTable(rptDataSet, repPdfReasonList);
                        }

                        try
                        {
                            #region Create Review Grid Templates

                            this.radGrid.Columns.Clear();
                            this.radGrid.MasterTemplate.Templates.Clear();
                            this.radGrid.DataSource = rptDataSet.Tables[0];
                            this.radGrid.MasterTemplate.Columns["ID"].IsVisible = false;

                            GridViewRelation gridRelation = new GridViewRelation(this.radGrid.MasterTemplate);
                            SetPreferences();

                            for (int i = 1; i < rptDataSet.Tables.Count; i++)
                            {
                                AddChildViewTemplate(radGrid, rptDataSet.Tables[i]);
                                if (rptDataSet.Tables[i].TableName == "Reason")
                                {
                                    this.radGrid.MasterTemplate.Templates[3].Columns["Reason"].WrapText = true;
                                }
                            }
                            this.radGrid.MasterTemplate.Columns["DateofSignature"].FormatString = "{0:dd/MM/yyyy}";
                            ShowSummaryInformation();

                            #endregion Create Review Grid Templates
                        }
                        catch (Exception m)
                        {
                            MessageBox.Show(m.Message);
                        }

                        _parent.btnsOn();

                        #region redundant else

                        #endregion redundant else
                    }
                }
                else if (result == DialogResult.No)
                {
                    this.Close();
                }

                lblFileName.Text = Path.GetFileNameWithoutExtension(filePath);
                count = 0;

                if (!worker.IsBusy)
                {
                    //worker.RunWorkerAsync();
                    //_parent.rdWaitingBar.StartWaiting();

                    if (filteredInstPdfList.Count != 0)
                    {
                        _parent.btnsOn();
                        _parent.btnsEnabled();
                    }
                    else
                    {
                        _parent.lblCurrDir.Text = String.Empty;
                        _parent.lblCurrFile.Text = String.Empty;
                        _parent.radProgressBar2.Value1 = 0;
                    }
                }

                #region read extra 100%

                #endregion read extra 100%
            }
            else
            {
                if (radGrid.RowCount == 0)
                {
                    _parent.btnsOff();
                }
            }
        }

        private void LoadDynamicPdfXmlReportToList(XDocument xmlDoc)
        {
            #region Load Reports from Dynamic Xml Generated From PDF

            try
            {
                var reports = (from report in xmlDoc.Descendants("ttr_Frm")
                               select new
                               {
                                   instName = report.Element("_1__Name_of_Financial_Institution").Value.Trim(),
                                   RepNo = report.Element("_ref_num_pg1").Value.Trim(),
                                   instAddress = report.Element("_2_Address_of_Financial_Institution").Value.Trim(),
                                   instTRN = report.Element("_3_TRN").Value.Trim(),
                                   instType = report.Element("_5__Type_of_Financial_Institution").Value.Trim(),
                                   instBranch = report.Element("_4__Branch_Address").Value.Trim(),
                                   pLastName = report.Element("_31__Last_Name").Value.Trim(),
                                   pFirstName = report.Element("_32__First_Name").Value.Trim(),
                                   pFullName = report.Element("_32__First_Name").Value.Trim() + " " + report.Element("_31__Last_Name").Value.Trim(),
                                   pMI = report.Element("_33__MI").Value.Trim(),
                                   pTitle = report.Element("_34__Title").Value.Trim(),
                                   pPhoneNo = report.Element("_35__Phone_No").Value.Trim(),
                                   pDateofSignature = report.Element("_37_Date_of_Signature_DDMMYYYY").Value.Trim(),
                                   nLastName = report.Element("_38__Last_Name").Value.Trim(),
                                   nFirstName = report.Element("_39_First_Name").Value.Trim(),
                                   nFullName = report.Element("_39_First_Name").Value.Trim() + " " + report.Element("_38__Last_Name").Value.Trim(),
                                   nMI = report.Element("_40__MI").Value.Trim(),
                                   nTitle = report.Element("_41__Title").Value.Trim(),
                                   nPhoneNo = report.Element("_42__Phone_No").Value.Trim(),
                                   t1Type = report.Element("_2_Transaction_Type").Value.Trim(),
                                   t1Date = report.Element("_3_Date_DDMMYYYY").Value.Trim(),
                                   t1Time = report.Element("_4_Time").Value.Trim(),
                                   t1Currency = report.Element("_5__Transaction_Type").Value.Trim(),
                                   t1Amount = report.Element("_6__Transaction_Amount").Value.Trim(),
                                   t1Accounts = report.Element("_7_Type1").Value.ToString().Trim() + " " + report.Element("_7_Type2").Value.ToString().Trim() + " " + report.Element("_7_Type3").Value.ToString().Trim(),
                                   t1JamEquiv = report.Element("_8__JA_Equivalent").Value.Trim(),
                                   t1JamExRate = report.Element("_9__JA_Exchange_Rate").Value.Trim(),
                                   t1UsEquiv = report.Element("_10__US_Equivalent").Value.Trim(),
                                   t1UsExRate = report.Element("_11__US_Exchange_Rate").Value.Trim(),
                                   t1SourceofFunds = report.Element("_12__Source_of_funds").Value.Trim(),
                                   t2Type = report.Element("_13__Transaction_Type").Value.Trim(),
                                   t2Date = report.Element("_14__Date_DDMMYYYY").Value.Trim(),
                                   t2Time = report.Element("_15_Time").Value.Trim(),
                                   t2Currency = report.Element("_16__Transaction_Currency").Value.Trim(),
                                   t2Amount = report.Element("_17__Transaction_Amount").Value.Trim(),
                                   t2Accounts = report.Element("_18_Type1").Value.ToString().Trim() + " " + report.Element("_18_Type2").Value.ToString().Trim() + " " + report.Element("_18_Type3").Value.ToString().Trim(),
                                   t2JamEquiv = report.Element("_19__JA_Equivalent").Value.Trim(),
                                   t2JamExRate = report.Element("_20__JA_Exchange_Rate").Value.Trim(),
                                   t2UsEquiv = report.Element("_21__US_Equivalent").Value.Trim(),
                                   t2UsExRate = report.Element("_22__US_Exchange_Rate").Value.Trim(),
                                   t2SourceofFunds = report.Element("_23__Source_of_funds").Value.Trim(),
                                   t3Type = report.Element("_24_Transaction_Type").Value.Trim(),
                                   t3Date = report.Element("_25_Date_DDMMYYYY").Value.Trim(),
                                   t3Time = report.Element("_26__Time").Value.Trim(),
                                   t3Currency = report.Element("_27__Transaction_Currency").Value.Trim(),
                                   t3Amount = report.Element("_28__Transaction_Amount").Value.Trim(),
                                   t3Accounts = report.Element("_29_Type1").Value.ToString().Trim() + " " + report.Element("_29_Type2").Value.ToString().Trim() + " " + report.Element("_29_Type3").Value.ToString().Trim(),
                                   t3JamEquiv = report.Element("_30__JA_Equivalent").Value.Trim(),
                                   t3JamExRate = report.Element("_31__JA_Exchange_Rate").Value.Trim(),
                                   t3UsEquiv = report.Element("_32__US_Equivalent").Value.Trim(),
                                   t3UsExRate = report.Element("_33__US_Exchange_Rate").Value.Trim(),
                                   t3SourceofFunds = report.Element("_34__Source_of_funds").Value.Trim(),
                                   t4Type = report.Element("_35__Transaction_Type").Value.Trim(),
                                   t4Date = report.Element("_36_Date_DDMMYYYY").Value.Trim(),
                                   t4Time = report.Element("_37__Time").Value.Trim(),
                                   t4Currency = report.Element("_38__Transaction_Currency").Value.Trim(),
                                   t4Amount = report.Element("_39__Transaction_Amount").Value.Trim(),
                                   t4Accounts = report.Element("_40_Type1").Value.ToString().Trim() + " " + report.Element("_40_Type2").Value.ToString().Trim() + " " + report.Element("_40_Type3").Value.ToString().Trim(),
                                   t4JamEquiv = report.Element("_41__JA_Equivalent").Value.Trim(),
                                   t4JamExRate = report.Element("_42__JA_Exchange_Rate").Value.Trim(),
                                   t4UsEquiv = report.Element("_43__US_Equivalent").Value.Trim(),
                                   t4UsExRate = report.Element("_44__US_Exchange_Rate").Value.Trim(),
                                   t4SourceofFunds = report.Element("_45__Source_of_funds").Value.Trim(),
                                   _13a = report.Element("CustAgenBenSub").Element("_13a_Checkbox").Value.Trim(),
                                   _13b = report.Element("CustAgenBenSub").Element("_13b_Checkbox").Value.Trim(),
                                   _14a = report.Element("CustAgenBenSub").Element("_14a_Checkbox").Value.Trim(),
                                   _14b = report.Element("CustAgenBenSub").Element("_14b_Checkbox").Value.Trim(),
                                   _14c = report.Element("CustAgenBenSub").Element("_14c_Checkbox").Value.Trim(),
                                   _14d = report.Element("CustAgenBenSub").Element("_14d_Checkbox").Value.Trim(),
                                   _14other = report.Element("CustAgenBenSub").Element("_14_Other").Value.Trim(),
                                   _14issuedBy = report.Element("CustAgenBenSub").Element("_14_Issued_by").Value.Trim(),
                                   _14Number = report.Element("CustAgenBenSub").Element("_14_Number").Value.Trim(),
                                   _24a = report.Element("CustAgenBenSub").Element("_24a_Checkbox").Value.Trim(),
                                   _24b = report.Element("CustAgenBenSub").Element("_24b_Checkbox").Value.Trim(),
                                   _25a = report.Element("CustAgenBenSub").Element("_25a_Checkbox").Value.Trim(),
                                   _25b = report.Element("CustAgenBenSub").Element("_25b_Checkbox").Value.Trim(),
                                   _25d = report.Element("CustAgenBenSub").Element("_25d_Checkbox").Value.Trim(),
                                   _25c = report.Element("CustAgenBenSub").Element("_25c_Checkbox").Value.Trim(),
                                   _25other = report.Element("CustAgenBenSub").Element("_25_Other").Value.Trim(),
                                   _25issuedBy = report.Element("CustAgenBenSub").Element("_25_Issued_by").Value.Trim(),
                                   _25Number = report.Element("CustAgenBenSub").Element("_25_Number").Value.Trim(),
                                   cLastName = report.Element("CustAgenBenSub").Element("_7__Individuals_last_name_or_organization_s_name").Value.Trim(),
                                   cFirstName = report.Element("CustAgenBenSub").Element("_8__First_Name").Value.Trim(),
                                   cMI = report.Element("CustAgenBenSub").Element("_9__MI").Value.Trim(),
                                   cAddress = report.Element("CustAgenBenSub").Element("_10__Permanent_Address").Value.Trim(),
                                   cDOB = report.Element("CustAgenBenSub").Element("_11_Date_of_Birth_DDMMYYYY").Value.Trim(),
                                   cTRN = report.Element("CustAgenBenSub").Element("_12__TRN").Value.Trim(),
                                   cAccNo = report.Element("CustAgenBenSub").Element("_15__Customers_Account_No_and_Type").Value.Trim(),
                                   cOccupation = report.Element("CustAgenBenSub").Element("_16__OccupationBusinessPrincipal_Activity").Value.Trim(),
                                   aLastName = report.Element("CustAgenBenSub").Element("_18__Individuals_last_name_or_organization_s_name").Value.Trim(),
                                   aFirstName = report.Element("CustAgenBenSub").Element("_19__First_Name").Value.Trim(),
                                   aMI = report.Element("CustAgenBenSub").Element("_20__MI").Value.Trim(),
                                   aAddress = report.Element("CustAgenBenSub").Element("_21__Permanent_Address").Value.Trim(),
                                   aDOB = report.Element("CustAgenBenSub").Element("_22_Date_of_Birth_DDMMYY").Value.Trim(),
                                   aTRN = report.Element("CustAgenBenSub").Element("_23__TRN").Value.Trim(),
                                   bLastName = report.Element("CustAgenBenSub").Element("_27__Individuals_last_name_or_organizations_name").Value.Trim(),
                                   bFirstName = report.Element("CustAgenBenSub").Element("_28_First_Name").Value.Trim(),
                                   bMI = report.Element("CustAgenBenSub").Element("_29__MI").Value.Trim(),
                                   bAddress = report.Element("CustAgenBenSub").Element("_30__Permanent_Address").Value.Trim(),
                               }).ToList();

            #endregion Load Reports from Dynamic Xml Generated From PDF

                rptDataSet = new DataSet();
                rptDataSet = CreateTablesAndAddToDataSet(rptDataSet);

                try
                {
                    #region Break Into Parts

                    List<Customer> custs = new List<Customer> { };

                    foreach (var r in reports)
                    {
                        #region Add Rows to Institution Table

                        DataRow iRow = rptDataSet.Tables["Institution"].NewRow();
                        iRow["ID"] = count;
                        if (r.RepNo == "")
                        {
                            break;
                        }
                        else
                        {
                            iRow["RefNum"] = r.RepNo;
                        }
                        iRow["InstituionName"] = r.instName;
                        iRow["Address"] = r.instAddress;
                        iRow["TRN"] = r.instTRN;
                        iRow["TypeOfInst"] = r.instType;
                        iRow["BranchAddress"] = r.instBranch;
                        if (r.pFirstName != "")
                        {
                            iRow["PreparerFullName"] = r.pLastName + ", " + r.pMI + " " + r.pFirstName;
                        }
                        else
                        {
                            iRow["PreparerFullName"] = r.pLastName;
                        }
                        iRow["PreparerTitle"] = r.pTitle;
                        iRow["DateofSignature"] = r.pDateofSignature;
                        if (r.nFirstName != "")
                        {
                            iRow["ContactFullName"] = r.nLastName + ", " + r.nMI + " " + r.nFirstName;
                        }
                        else
                        {
                            iRow["ContactFullName"] = r.nLastName;
                        }
                        iRow["ContactTitle"] = r.nTitle;
                        iRow["ContactPhoneNum"] = r.nPhoneNo;
                        rptDataSet.Tables["Institution"].Rows.Add(iRow);

                        #endregion Add Rows to Institution Table

                        #region Add Rows to Customer Table

                        //Add Customer Rows
                        DataRow cRow = rptDataSet.Tables["Customer"].NewRow();
                        cRow["ID"] = count;
                        cRow["RefNum"] = r.RepNo;
                        if (r.cFirstName != String.Empty)
                        {
                            cRow["FullName"] = r.cLastName + ", " + r.cMI + " " + r.cFirstName;
                        }
                        else
                        {
                            cRow["FullName"] = r.cLastName;
                        }
                        cRow["Address"] = r.cAddress;
                        cRow["DateofBirth"] = r.cDOB;
                        cRow["TRN"] = r.cTRN;
                        if (r._13a != "")
                        {
                            cRow["VerificationMethod"] = r._13a;
                        }
                        else if (r._13b != "")
                        {
                            cRow["VerificationMethod"] = r._13b;
                        }
                        else
                        {
                            cRow["VerificationMethod"] = "unknown";
                        }
                        if (r._14a != String.Empty)
                        {
                            cRow["IDType"] = r._14a;
                        }
                        else if (r._14b != String.Empty)
                        {
                            cRow["IDType"] = r._14b;
                        }
                        else if (r._14c != String.Empty)
                        {
                            cRow["IDType"] = r._14c;
                        }
                        else if (r._14d != String.Empty)
                        {
                            cRow["IDType"] = r._14d;
                        }
                        else if (r._14other != String.Empty)
                        {
                            cRow["IDType"] = r._14other;
                        }
                        else
                        {
                            cRow["IDType"] = "unknown";
                        }
                        cRow["IssuedBy"] = r._14issuedBy;
                        cRow["ID#"] = r._14Number;
                        cRow["AccNumType"] = r.cAccNo;
                        cRow["Occupation"] = r.cOccupation;
                        rptDataSet.Tables["Customer"].Rows.Add(cRow);

                        #endregion Add Rows to Customer Table

                        #region Add Rows to Agent Table

                        //Add Agent Rows
                        DataRow aRow = rptDataSet.Tables["Agent"].NewRow();
                        aRow["ID"] = count;
                        aRow["RefNum"] = r.RepNo;
                        if (r.aFirstName != String.Empty)
                        {
                            aRow["FullName"] = r.aLastName + ", " + r.aMI + " " + r.aFirstName;
                        }
                        else
                        {
                            aRow["FullName"] = r.aLastName;
                        }
                        aRow["Address"] = r.aAddress;
                        aRow["DateofBirth"] = r.aDOB;
                        aRow["TRN"] = r.aTRN;
                        if (r._24a != "")
                        {
                            aRow["VerificationMethod"] = r._24a;
                        }
                        else if (r._24b != "")
                        {
                            aRow["VerificationMethod"] = r._24b;
                        }
                        else
                        {
                            aRow["VerificationMethod"] = "unknown";
                        }
                        if (r._25a != String.Empty)
                        {
                            aRow["IDType"] = r._25a;
                        }
                        else if (r._25b != String.Empty)
                        {
                            aRow["IDType"] = r._25b;
                        }
                        else if (r._25c != String.Empty)
                        {
                            aRow["IDType"] = r._25c;
                        }
                        else if (r._25d != String.Empty)
                        {
                            aRow["IDType"] = r._25d;
                        }
                        else if (r._25other != String.Empty)
                        {
                            aRow["IDType"] = r._25other;
                        }
                        else
                        {
                            aRow["IDType"] = "unknown";
                        }
                        aRow["IssuedBy"] = r._25issuedBy;
                        aRow["ID#"] = r._25Number;
                        rptDataSet.Tables["Agent"].Rows.Add(aRow);
                        # endregion

                        #region Add Rows to Benficiary Table

                        //Add Beneficiary Rows
                        DataRow bRow = rptDataSet.Tables["Beneficiary"].NewRow();
                        bRow["ID"] = count;
                        bRow["RefNum"] = r.RepNo;
                        if (r.bFirstName != String.Empty)
                        {
                            bRow["FullName"] = r.bLastName + ", " + r.bMI + " " + r.bFirstName;
                        }
                        else
                        {
                            bRow["FullName"] = r.bLastName;
                        }
                        bRow["Address"] = r.bAddress;
                        rptDataSet.Tables["Beneficiary"].Rows.Add(bRow);

                        #endregion Add Rows to Benficiary Table

                        #region Add Rows to Transaction Table

                        DataRow tRow = rptDataSet.Tables["Transaction"].NewRow();
                        tRow["ID"] = count;
                        tRow["Type"] = r.t1Type;
                        tRow["Date"] = r.t1Date;
                        tRow["Time"] = r.t1Time;
                        tRow["Currency"] = r.t1Currency;
                        tRow["Amount"] = Utility.CleanMoneyValue(r.t1Amount);
                        tRow["A/C Type"] = Utility.GetAccType(r.t1Accounts);
                        tRow["A/C Num."] = Utility.GetAccNum(r.t1Accounts);
                        tRow["J$ Equiv"] = Utility.CleanMoneyValue(r.t1JamEquiv);
                        tRow["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t1JamExRate);
                        tRow["US$ Equiv"] = Utility.CleanMoneyValue(r.t1UsEquiv);
                        tRow["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t1UsExRate);
                        tRow["Funds Source"] = r.t1SourceofFunds;
                        rptDataSet.Tables["Transaction"].Rows.Add(tRow);

                        if ((r.t2Amount != String.Empty) && (r.t2Amount != "0"))
                        {
                            DataRow t2Row = rptDataSet.Tables["Transaction"].NewRow();
                            t2Row["ID"] = count;
                            t2Row["Type"] = r.t2Type;
                            t2Row["Date"] = r.t2Date;
                            t2Row["Time"] = r.t2Time;
                            t2Row["Currency"] = r.t2Currency;
                            t2Row["Amount"] = Utility.CleanMoneyValue(r.t2Amount);
                            t2Row["A/C Type"] = Utility.GetAccType(r.t2Accounts);
                            t2Row["A/C Num."] = Utility.GetAccNum(r.t2Accounts);
                            t2Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t2JamEquiv);
                            t2Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2JamExRate);
                            t2Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t2UsEquiv);
                            t2Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                            t2Row["Funds Source"] = r.t2SourceofFunds;
                            rptDataSet.Tables["Transaction"].Rows.Add(t2Row);
                        }

                        if ((r.t3Amount != String.Empty) && (r.t3Amount != "0"))
                        {
                            DataRow t3Row = rptDataSet.Tables["Transaction"].NewRow();
                            t3Row["ID"] = count;
                            t3Row["Type"] = r.t3Type;
                            t3Row["Date"] = r.t3Date;
                            t3Row["Time"] = r.t3Time;
                            t3Row["Currency"] = r.t3Currency;
                            t3Row["Amount"] = Utility.CleanMoneyValue(r.t3Amount);
                            t3Row["A/C Type"] = Utility.GetAccType(r.t3Accounts);
                            t3Row["A/C Num."] = Utility.GetAccNum(r.t3Accounts);
                            t3Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t3JamEquiv);
                            t3Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t3JamExRate);
                            t3Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t3UsEquiv);
                            t3Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                            t3Row["Funds Source"] = r.t3SourceofFunds;
                            rptDataSet.Tables["Transaction"].Rows.Add(t3Row);
                        }

                        if ((r.t4Amount != String.Empty) && (r.t4Amount != "0"))
                        {
                            DataRow t4Row = rptDataSet.Tables["Transaction"].NewRow();
                            t4Row["ID"] = count;
                            t4Row["Type"] = r.t4Type;
                            t4Row["Date"] = r.t4Date;
                            t4Row["Time"] = r.t4Time;
                            t4Row["Currency"] = Utility.CleanMoneyValue(r.t4Currency);
                            t4Row["Amount"] = r.t4Amount;
                            t4Row["A/C Type"] = Utility.GetAccType(r.t4Accounts);
                            t4Row["A/C Num."] = Utility.GetAccNum(r.t4Accounts);
                            t4Row["J$ Equiv"] = Utility.CleanMoneyValue(r.t4JamEquiv);
                            t4Row["J$ Ex.Rate"] = Utility.CleanMoneyValue(r.t4JamExRate);
                            t4Row["US$ Equiv"] = Utility.CleanMoneyValue(r.t4UsEquiv);
                            t4Row["US$ Ex.Rate"] = Utility.CleanMoneyValue(r.t2UsExRate);
                            t4Row["Funds Source"] = r.t4SourceofFunds;
                            rptDataSet.Tables["Transaction"].Rows.Add(t4Row);
                        }

                        #endregion Add Rows to Transaction Table

                        count++;
                    }

                    #region Create Review Grids Templates

                    this.radGrid.Columns.Clear();
                    this.radGrid.MasterTemplate.Templates.Clear();
                    this.radGrid.DataSource = rptDataSet.Tables[0];
                    this.radGrid.MasterTemplate.Columns["ID"].IsVisible = false;
                    SetPreferences();

                    if (batchType == "TTR")
                    {
                        this.radGrid.MasterTemplate.Columns["Reason"].IsVisible = false;
                    }

                    GridViewTemplate cusTemplate = new GridViewTemplate();
                    cusTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                    cusTemplate.ReadOnly = true;
                    cusTemplate.AllowAddNewRow = false;
                    cusTemplate.Caption = "Customer(s)";
                    this.radGrid.MasterTemplate.Templates.Add(cusTemplate);
                    cusTemplate.DataSource = rptDataSet.Tables[1];
                    cusTemplate.Columns["ID"].IsVisible = false;

                    GridViewRelation relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    relation.ChildTemplate = cusTemplate;
                    relation.RelationName = "Instituion_Customers";
                    relation.ParentColumnNames.Add("ID");
                    relation.ChildColumnNames.Add("ID");
                    radGrid.Relations.Add(relation);

                    GridViewTemplate agentTemplate = new GridViewTemplate();
                    agentTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                    agentTemplate.AllowAddNewRow = false;
                    agentTemplate.ReadOnly = true;
                    agentTemplate.Caption = "Agent(s)";
                    this.radGrid.MasterTemplate.Templates.Add(agentTemplate);
                    agentTemplate.DataSource = rptDataSet.Tables[2];
                    agentTemplate.Columns["ID"].IsVisible = false;

                    relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    relation.ChildTemplate = agentTemplate;
                    relation.RelationName = "Institution_Agents";
                    relation.ParentColumnNames.Add("ID");
                    relation.ChildColumnNames.Add("ID");
                    radGrid.Relations.Add(relation);

                    GridViewTemplate benfTemplate = new GridViewTemplate();
                    benfTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                    benfTemplate.AllowAddNewRow = false;
                    benfTemplate.ReadOnly = true;
                    benfTemplate.Caption = "Beneficiarie(s)";
                    this.radGrid.MasterTemplate.Templates.Add(benfTemplate);
                    benfTemplate.DataSource = rptDataSet.Tables[3];
                    benfTemplate.Columns["ID"].IsVisible = false;

                    relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    relation.ChildTemplate = benfTemplate;
                    relation.RelationName = "Institution_Beneficaries";
                    relation.ParentColumnNames.Add("ID");
                    relation.ChildColumnNames.Add("ID");
                    radGrid.Relations.Add(relation);

                    GridViewTemplate transTemplate = new GridViewTemplate();
                    transTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                    transTemplate.AllowAddNewRow = false;
                    transTemplate.ReadOnly = true;
                    transTemplate.Caption = "Transaction(s)";
                    this.radGrid.MasterTemplate.Templates.Add(transTemplate);
                    transTemplate.DataSource = rptDataSet.Tables[4];
                    transTemplate.Columns["ID"].IsVisible = false;
                    transTemplate.Columns["Amount"].FormatString = "{0:$#,###0.00;($#,###0.00);0}";

                    relation = new GridViewRelation(this.radGrid.MasterTemplate);
                    relation.ChildTemplate = transTemplate;
                    relation.RelationName = "Institution_Trans";
                    relation.ParentColumnNames.Add("ID");
                    relation.ChildColumnNames.Add("ID");
                    radGrid.Relations.Add(relation);

                    //this.radGrid.MasterTemplate.BestFitColumns();
                    //this.radGrid.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                    this.radGrid.MasterTemplate.Columns["DateofSignature"].FormatString = "{0:dd/MM/yyyy}";

                    ShowSummaryInformation();

                    #endregion Create Review Grids Templates
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                    #endregion Add Rows to Agent Table
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void readPdfMenuBtn_Click(object sender, EventArgs e)
        {
            ////create the open file dialog variable
            //OpenFileDialog fDialog = new OpenFileDialog();

            ////set the title of the window
            //fDialog.Title = "Open File for Review";

            ////Set a file type filter
            //fDialog.Filter = "PDF Files |*.pdf";

            ////Set the initial Directory
            ////fDialog.InitialDirectory = @"\\lambda\Intel_\INTEL\Electronic Reports\XML";

            //if (fDialog.ShowDialog() == DialogResult.OK)
            //{
            //    List<string> pdfFiles = new List<string>();
            //    filePath = fDialog.FileName.ToString();
            //    batchType = Utility.GetBatchTypeFromName(fDialog.SafeFileName);
            //    lblFileName.Text = Path.GetFileNameWithoutExtension(filePath);
            //    //Utility.ReadPdfFile(filePath);
            //    MessageBox.Show(pdfFiles[0].ToString());
            //    count = 0;
            //    //btnSavetoDB.Enabled = true;
            //}
        }

        private void loadXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //create the open file dialog variable
            OpenFileDialog fDialog = new OpenFileDialog();

            //set the title of the window
            fDialog.Title = "Open File for Review";

            //Set a file type filter
            fDialog.Filter = "XML Files |*.xml";

            //Set the initial Directory
            //fDialog.InitialDirectory = @"\\lambda\Intel_\INTEL\Electronic Reports\XML";

            //Set the initial Directory
            fDialog.InitialDirectory = @"X:\";

            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                _parent.btnUpload.Enabled = false;
                filePath = fDialog.FileName.ToString();
                batchType = Utility.GetBatchTypeFromName(fDialog.SafeFileName);
                lblFileName.Text = Path.GetFileNameWithoutExtension(filePath);
                XDocument xmlDoc = XDocument.Load(filePath);

                LoadXmlReportToList(xmlDoc);
                count = 0;
                _parent.btnsOn();

                //btnsOn = true;
            }
            else
            {
                if (radGrid.RowCount == 0)
                {
                    _parent.btnsOff();
                }
            }
        }

        private void uploadPDFDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DateTime dtValue;
            string[] name;
            DateTime? tempDate;
            DateTime minimumDate = new DateTime(1753, 01, 01);

            List<DataRow> instList = rptDataSet.Tables[0].AsEnumerable().ToList();
            List<DataRow> custList = rptDataSet.Tables[1].AsEnumerable().ToList();
            List<DataRow> agentList = rptDataSet.Tables[2].AsEnumerable().ToList();
            List<DataRow> benList = rptDataSet.Tables[3].AsEnumerable().ToList();
            List<DataRow> reasonList = rptDataSet.Tables[4].AsEnumerable().ToList();
            List<DataRow> transList = rptDataSet.Tables[5].AsEnumerable().ToList();

            try
            {
                #region Add Institution Headers to DB

                try
                {
                    foreach (var i in instList)
                    {
                        InstitutionDetail inst = new InstitutionDetail();
                        inst.RefNum = i.ItemArray[1].ToString();
                        inst.InstitutionName = i.ItemArray[2].ToString();
                        inst.Address = i.ItemArray[3].ToString();
                        inst.TRN = i.ItemArray[4].ToString();
                        inst.TypeOfInstution = i.ItemArray[5].ToString();
                        inst.BranchAddress = i.ItemArray[6].ToString();
                        name = Utility.SplitName(i.ItemArray[7].ToString());
                        if (name.Length == 3)
                        {
                            inst.PrepareLastName = name[0].Trim(',');
                            inst.PrepareMI = name[1];
                            inst.PrepareFirstName = name[2];
                        }
                        else if (name.Length == 2)
                        {
                            inst.PrepareLastName = name[0].Trim(',');
                            inst.PrepareMI = String.Empty;
                            inst.PrepareFirstName = name[1];
                        }
                        else if (name.Length == 1)
                        {
                            inst.PrepareLastName = name[0].Trim(',');
                            inst.PrepareFirstName = String.Empty;
                            inst.PrepareMI = String.Empty;
                        }
                        inst.PrepareTitle = i.ItemArray[8].ToString().Trim();
                        tempDate = Utility.StringToDate(i.ItemArray[9].ToString());
                        if (tempDate == null)
                        {
                            inst.PrepareSignDate = null;
                        }
                        else if (tempDate < minimumDate)
                        {
                            if (File.Exists("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + "error.log"))
                            {
                                using (FileStream fs = new FileStream("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + "- fintran error.log", FileMode.Append))
                                {
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.Write("\nRef Number:" + inst.RefNum + ", " + "Date in File:" + tempDate + ", " + "File Location: " + foldersDialog.SelectedPath.ToString());
                                    sw.Close();
                                }
                            }
                            else
                            {
                                using (FileStream fs = new FileStream("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + " - fintran error.log", FileMode.OpenOrCreate))
                                {
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.Write("\nRef Number:" + inst.RefNum + ", " + "Date in File:" + tempDate + " File Location: " + foldersDialog.SelectedPath.ToString());
                                    sw.Close();
                                }
                            }
                        }
                        else
                        {
                            inst.PrepareSignDate = tempDate;
                        }
                        name = Utility.SplitName(i.ItemArray[10].ToString());
                        if (name.Length == 3)
                        {
                            inst.ContactLastName = name[0].Trim(',');
                            inst.ContactMI = name[1];
                            inst.ContactFirstName = name[2];
                        }
                        else if (name.Length == 2)
                        {
                            inst.ContactLastName = name[0].Trim(',');
                            inst.ContactMI = String.Empty;
                            inst.ContactFirstName = name[1];
                        }
                        else if (name.Length == 1)
                        {
                            inst.ContactLastName = name[0].Trim(',');
                            inst.ContactFirstName = String.Empty;
                            inst.ContactMI = String.Empty;
                        }
                        inst.ContactTitle = i.ItemArray[11].ToString();
                        inst.ContactPhone = i.ItemArray[12].ToString();

                        //inst.Reason = i.ItemArray[13].ToString();
                        if (batchType == "STR")
                        {
                            inst.Report_type = "STR";
                        }
                        if (batchType == "TTR")
                        {
                            inst.Report_type = "TTR";
                        }

                        dbContext.InstitutionDetails.InsertOnSubmit(inst);
                    }
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }

                #endregion Add Institution Headers to DB

                #region Add Customers to DB

                try
                {
                    foreach (var c in custList)
                    {
                        Customer cust = new Customer();
                        cust._ref_num_pg1 = c.ItemArray[1].ToString();
                        name = Utility.SplitName(c.ItemArray[2].ToString());
                        if (name.Length == 3)
                        {
                            cust._7__Individuals_last_name_or_organization_s_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            cust._8__First_Name = Utility.CleanExcessiveWhiteSpaces(name[2]);
                            cust._9__MI = Utility.CleanExcessiveWhiteSpaces(name[1]);
                        }
                        else if (name.Length == 2)
                        {
                            cust._7__Individuals_last_name_or_organization_s_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            cust._8__First_Name = Utility.CleanExcessiveWhiteSpaces(name[1]);
                            cust._9__MI = " ";
                        }
                        else if (name.Length == 1)
                        {
                            cust._7__Individuals_last_name_or_organization_s_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            cust._8__First_Name = "";
                            cust._9__MI = " ";
                        }
                        cust._10__Permanent_Address = c.ItemArray[3].ToString();
                        tempDate = Utility.StringToDate(c.ItemArray[4].ToString());
                        if (tempDate == null)
                        {
                            cust._11_Date_of_Birth_DDMMYYYY = null;
                        }
                        else if (tempDate < minimumDate)
                        {
                            if (File.Exists("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + "error.log"))
                            {
                                using (FileStream fs = new FileStream("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + "- fintran error.log", FileMode.Append))
                                {
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.Write("/nRef Number:" + cust._ref_num_pg1 + ", " + "Date in File:" + tempDate + ", " + "File Location: " + foldersDialog.SelectedPath.ToString());
                                    sw.Close();
                                }
                            }
                            else
                            {
                                using (FileStream fs = new FileStream("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + " - fintran error.log", FileMode.OpenOrCreate))
                                {
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.Write("/nRef Number:" + cust._ref_num_pg1 + ", " + "Date in File:" + tempDate + " File Location: " + foldersDialog.SelectedPath.ToString());
                                    sw.Close();
                                }
                            }

                            cust._11_Date_of_Birth_DDMMYYYY = null;
                        }
                        else
                        {
                            cust._11_Date_of_Birth_DDMMYYYY = tempDate;
                        }
                        cust._12__TRN = c.ItemArray[5].ToString();
                        cust._13_Verification_Method = c.ItemArray[6].ToString();
                        cust._14_ID_Type = c.ItemArray[7].ToString();
                        cust._14_Issued_by = c.ItemArray[8].ToString();
                        cust._14_ID_Number = c.ItemArray[9].ToString();
                        cust._15__Customers_Account_No_and_Type = c.ItemArray[10].ToString();
                        cust._16__OccupationBusinessPrincipal_Activity = c.ItemArray[11].ToString();
                        cust.InvolveTYP = "Customer";
                        dbContext.Customers.InsertOnSubmit(cust);
                    }
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }

                #endregion Add Customers to DB

                #region Add Agents to DB

                try
                {
                    foreach (var a in agentList)
                    {
                        Agent agent = new Agent();
                        agent._ref_num_pg1 = a.ItemArray[1].ToString();
                        name = Utility.SplitName(a.ItemArray[2].ToString());
                        if (name.Length == 3)
                        {
                            agent._18__Individuals_last_name_or_organization_s_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            agent._19__First_Name = Utility.CleanExcessiveWhiteSpaces(name[2]);
                            agent._20__MI = Utility.CleanExcessiveWhiteSpaces(name[1]);
                        }
                        else if (name.Length == 2)
                        {
                            agent._18__Individuals_last_name_or_organization_s_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            agent._19__First_Name = Utility.CleanExcessiveWhiteSpaces(name[1]);
                            agent._20__MI = " ";
                        }
                        else if (name.Length == 1)
                        {
                            agent._18__Individuals_last_name_or_organization_s_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            agent._19__First_Name = "";
                            agent._20__MI = " ";
                        }
                        agent._21__Permanent_Address = a.ItemArray[3].ToString();
                        tempDate = Utility.StringToDate(a.ItemArray[4].ToString());

                        //dtResults = DateTime.TryParseExact(tempDate, "ddMMyyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out dtValue);
                        if (tempDate == null)
                        {
                            agent._22_Date_of_Birth_DDMMYY = null;
                        }
                        else if (tempDate < minimumDate)
                        {
                            if (File.Exists("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + "error.log"))
                            {
                                using (FileStream fs = new FileStream("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + "- fintran error.log", FileMode.Append))
                                {
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.Write("/nRef Number:" + agent._ref_num_pg1 + ", " + "Date in File:" + tempDate + ", " + "File Location: " + foldersDialog.SelectedPath.ToString());
                                    sw.Close();
                                }
                            }
                            else
                            {
                                using (FileStream fs = new FileStream("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + " - fintran error.log", FileMode.OpenOrCreate))
                                {
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.Write("/nRef Number:" + agent._ref_num_pg1 + ", " + "Date in File:" + tempDate + " File Location: " + foldersDialog.SelectedPath.ToString());
                                    sw.Close();
                                }
                            }

                            agent._22_Date_of_Birth_DDMMYY = null;
                        }
                        else
                        {
                            agent._22_Date_of_Birth_DDMMYY = tempDate;
                        }
                        agent._23__TRN = a.ItemArray[5].ToString();
                        agent._24_Verification_Method = a.ItemArray[6].ToString();
                        agent._25_ID_Type = a.ItemArray[7].ToString();
                        agent._25_Issued_by = a.ItemArray[8].ToString();
                        agent._25_ID_Number = a.ItemArray[9].ToString();
                        agent.InvolveTYP = "Agent";
                        dbContext.Agents.InsertOnSubmit(agent);
                    }
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }

                #endregion Add Agents to DB

                #region Add Beneficiaries to DB

                try
                {
                    foreach (var b in benList)
                    {
                        Beneficiary ben = new Beneficiary();
                        ben._ref_num_pg1 = b.ItemArray[1].ToString();
                        name = Utility.SplitName(b.ItemArray[2].ToString());
                        if (name.Length == 3)
                        {
                            ben._27__Individuals_last_name_or_organizations_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            ben._28_First_Name = name[2];
                            ben._29__MI = name[1];
                        }
                        else if (name.Length == 2)
                        {
                            ben._27__Individuals_last_name_or_organizations_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            ben._28_First_Name = Utility.CleanExcessiveWhiteSpaces(name[1]);
                            ben._29__MI = " ";
                        }
                        else if (name.Length == 1)
                        {
                            ben._27__Individuals_last_name_or_organizations_name = Utility.CleanExcessiveWhiteSpaces(name[0].Trim(','));
                            ben._28_First_Name = "";
                            ben._29__MI = " ";
                        }
                        ben._30__Permanent_Address = b.ItemArray[3].ToString();
                        ben.InvolveTYP = "Beneficiary";
                        dbContext.Beneficiaries.InsertOnSubmit(ben);
                    }
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }

                #endregion Add Beneficiaries to DB

                #region Add Reasons to DB

                if (batchType == "STR")
                {
                    try
                    {
                        foreach (var r in reasonList)
                        {
                            ReportReason rReason = new ReportReason();
                            rReason.RefNumber = r.ItemArray[1].ToString();
                            rReason.Reason = r.ItemArray[2].ToString();
                            dbContext.ReportReasons.InsertOnSubmit(rReason);

                            //dbContext.SubmitChanges();
                        }
                    }
                    catch (Exception m)
                    {
                        MessageBox.Show(m.Message);
                    }
                }
                else
                {
                    ;
                }

                #endregion Add Reasons to DB

                #region Add Transaction to DB

                try
                {
                    foreach (var t in transList)
                    {
                        TransactionDetail tran = new TransactionDetail();
                        tran._ref_num_pg1 = t.ItemArray[1].ToString();
                        tran._2_Transaction_Type = t.ItemArray[2].ToString();
                        tempDate = Utility.StringToDate(t.ItemArray[3].ToString());

                        //dtResults = DateTime.TryParseExact(tempDate, "ddMMyyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out dtValue);
                        if (tempDate == null)
                        {
                            tran._3_Date_DDMMYYYY = null;
                        }
                        else if (tempDate < minimumDate)
                        {
                            if (File.Exists("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + "error.log"))
                            {
                                using (FileStream fs = new FileStream("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + "- fintran error.log", FileMode.Append))
                                {
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.Write("\nRef Number:" + tran._ref_num_pg1 + ", " + "Date in File:" + tempDate + ", " + "File Location: " + foldersDialog.SelectedPath.ToString());
                                    sw.Close();
                                }
                            }
                            else
                            {
                                using (FileStream fs = new FileStream("c:\\" + DateTime.Now.ToString("yyyy-MM-dd") + " - fintran error.log", FileMode.OpenOrCreate))
                                {
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.Write("\nRef Number:" + tran._ref_num_pg1 + ", " + "Date in File:" + tempDate + " File Location: " + foldersDialog.SelectedPath.ToString());
                                    sw.Close();
                                }
                            }

                            tran._3_Date_DDMMYYYY = null;
                        }
                        else
                        {
                            tran._3_Date_DDMMYYYY = tempDate;
                        }
                        tran._4_Time = t.ItemArray[4].ToString();
                        tran._5__Transaction_Currency = t.ItemArray[5].ToString();
                        tran._6__Transaction_Amount = Utility.CleanMoneyValue(t.ItemArray[6].ToString());
                        tran._7_ACType = t.ItemArray[7].ToString();
                        tran._7_ACNumber = t.ItemArray[8].ToString();
                        tran._8__JA_Equivalent = Utility.CleanMoneyValue(t.ItemArray[9].ToString());
                        tran._9__JA_Exchange_Rate = Utility.CleanMoneyValue(t.ItemArray[10].ToString());
                        tran._10__US_Equivalent = Utility.CleanMoneyValue(t.ItemArray[11].ToString());
                        tran._11__US_Exchange_Rate = Utility.CleanMoneyValue(t.ItemArray[12].ToString());
                        tran._12__Source_of_funds = t.ItemArray[13].ToString();
                        dbContext.TransactionDetails.InsertOnSubmit(tran);
                    }
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message);
                }

                #endregion Add Transaction to DB
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message + " Please contact the ITU.");

                //MessageBox.Show();0
            }

            try
            {
                dbContext.SubmitChanges();
                MessageBox.Show("All Transactions submitted successfully");
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message.ToString() + " Please contact ITU. [Code #DB1]");
                this.radGrid.Columns.Clear();
            }

            try
            {
                #region Moving File After Review

                //    if (filePath.EndsWith(".xml"))
                //    {
                //        this.radGrid.Columns.Clear();

                //        string newfilePath = @"\\lambda\Intel_\INTEL\Electronic Reports\Reviewed\XML\" + filePath.Substring(3);

                //        if (!Directory.Exists(newfilePath))
                //        {
                //            Directory.CreateDirectory(Path.GetDirectoryName(newfilePath));
                //        }
                //        File.Move(filePath, newfilePath);
                //        File.Delete(filePath);
                //        MessageBox.Show("XML file successfully moved to Review Folder");
                //    }
                //    else
                //    {
                //        foreach (var pdf in pdfFiles)
                //        {
                //            string newfilePath = @"\\lambda\Intel_\INTEL\Electronic Reports\Reviewed\" + filePath.Substring(3);

                //            if (!Directory.Exists(newfilePath))
                //            {
                //                Directory.CreateDirectory(newfilePath);
                //            }

                //            File.Move(pdf, newfilePath + @"\" + Path.GetFileName(pdf));
                //            File.Delete(pdf);
                //        }

                //        //List<string> FILESLIST = Directory.GetFiles(filePath, "*.*").ToList();
                //        Directory.Delete(filePath);

                //        //Directory.Delete(Path.GetDirectoryName(filePath));
                //        MessageBox.Show("PDF files successfully moved to Review Folder");
                //    }

                //    this.radGrid.Columns.Clear();
                //    _parent.radProgressBar2.Value1 = 0;
                //}

                #endregion Moving File After Review
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message.ToString() + " Could not move all files.");
                this.radGrid.Columns.Clear();
                _parent.radProgressBar2.Value1 = 0;
            }

            this.radGrid.Columns.Clear();
            _parent.radProgressBar2.Value1 = 0;
        }

        private void XML_Uploader_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.btnsOff();
        }

        private void radGrid_DataBindingComplete(object sender, GridViewBindingCompleteEventArgs e)
        {
            var grid = sender as RadGridView;
            foreach (var column in grid.Columns)
            {
                column.BestFit();
            }
        }
    }
}