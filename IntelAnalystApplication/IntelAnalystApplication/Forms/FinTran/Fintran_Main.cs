using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using QuickXmlReader.Functions;
using Telerik.WinControls.UI;

namespace QuickXmlReader.Forms.Fintran
{
    public partial class Fintran_Main : Form
    {
        private dbConnections dbcon = new dbConnections();
        private dbConnectionsInSql sqlDBCon = new dbConnectionsInSql();
        private Boolean reportNumState = false;
        private Boolean reportTypeState = false;
        private Boolean TransTypeState, TransDateState, TransAmountState, TransCurrencyState;
        private string idcred = string.Empty;
        private string issued = string.Empty;
        private string accountType = string.Empty;

        public Fintran_Main()
        {
            InitializeComponent();
        }

        private void Fintran_Main_Load(object sender, EventArgs e)
        {
            #region Load All DropLists Data

            if (FintranList.GetInstitutionList.Count == 0)
            {
                dbcon.getInstitutionNames();
                dbcon.getCurrencyNames();
                dbcon.getCountryNames();
                dbcon.getParishNames();
                dbcon.getAccountTypeNames();
                dbcon.getIDCredNames();
                dbcon.getIssuedByNames();
                dbcon.getTranscationNames();

                loadInsitutionList(); //Loads Insitution List
                loadSelectBatch(); //Loads Batch Selection
                loadCurrency(); //Loads Currency
                loadCountry(); //Loads Country
                loadParish(); //Loads Parish
                loadAccountTypeList(); //Loads Account Type
                loadIssuedByList(); //Loads Issued By
                loadIDCredList(); //Loads IDCredList
                loadTransactionTypeList(); //Loads Transaction Type
            }
            else
            {
                loadInsitutionList();
                loadSelectBatch();
                loadCurrency();
                loadCountry();
                loadParish();
                loadAccountTypeList();
                loadIssuedByList();
                loadIDCredList();
                loadTransactionTypeList();
            }

            #endregion Load All DropLists Data

            nullDatePicker(); //Null all Date Time Pickers
            PageControl(false);
            this.txtLNameStepTwo.AutoCompleteCustomSource = dbcon.getAutoCompleteInfo("LastName_");
            this.txtFNameStepTwo.AutoCompleteCustomSource = dbcon.getAutoCompleteInfo("First_Name");
            this.txtReportNumberStepOne.AutoCompleteCustomSource = dbcon.getAutoCompleteReportID();
        }

        public void PageControl(Boolean state)
        {
            this.StepTwo_PageViewOuter.Enabled = state; //Person Involved
            this.StepThree_PageViewOuter.Enabled = state; //Transaction
            this.StepFour_PageViewOuter.Enabled = state; //Report Reason
        }

        #region DropList Functions

        //Loads InstitutionList
        public void loadInsitutionList()
        {
            this.ListbatchInst.DataSource = FintranList.GetInstitutionList;
            this.ListbatchInst.ValueMember = "Expansion";
        }

        //Loads Step One Select Batch Droplist Data
        public void loadSelectBatch()
        {
            this.ListSelectBathStepOne.DataSource = dbcon.getBatchUserInfo();
            foreach (GridViewDataColumn column in this.ListSelectBathStepOne.MultiColumnComboBoxElement.Columns)
            {
                column.BestFit();
            }
        }

        //Loads Step Two Country Droplist Data
        public void loadCountry()
        {
            this.listReportCountryStepTwo.DataSource = FintranList.GetCountryList;
            this.listReportCountryStepTwo.ValueMember = "Expansion";
        }

        //Loads Step Two Parish Droplist Data
        public void loadParish()
        {
            this.listReportParishStepTwo.DataSource = FintranList.GetParishList;
            this.listReportParishStepTwo.ValueMember = "Expansion";
        }

        //Loads Step Three Currency Droplist Data
        public void loadCurrency()
        {
            this.listReportCurrencyStepThree.DataSource = FintranList.GetCurrencyList;
            this.listReportCurrencyStepThree.ValueMember = "Expansion";
        }

        //Loads Account Type
        public void loadAccountTypeList()
        {
            this.listAccountTypeStepTwo.DataSource = FintranList.GetAccountTypeList;
            this.listAccountTypeStepTwo.ValueMember = "Expansion";

            this.listAccountTypeStepThree.DataSource = FintranList.GetAccountTypeList;
            this.listAccountTypeStepThree.ValueMember = "Expansion";
        }

        //Loads IssuedBy Type
        public void loadIssuedByList()
        {
            this.listIssuedByStepTwo.DataSource = FintranList.GetIssuedByList;
            this.listIssuedByStepTwo.ValueMember = "Expansion";
        }

        //Loads IDCred Type
        public void loadIDCredList()
        {
            this.listIDCredStepTwo.DataSource = FintranList.GetIDCredList;
            this.listIDCredStepTwo.ValueMember = "Expansion";
        }

        //Loads Transcation Type
        public void loadTransactionTypeList()
        {
            this.listTransTypeStepThree.DataSource = FintranList.GetTransactionTypeList;
            this.listTransTypeStepThree.ValueMember = "Expansion";
        }

        //Loads Step One Branch  Data
        public void loadBranchNames(string InsitutionName)
        {
            //Loads All Branch Names for the selected Branch
            this.listBranchNameStepOne.DataSource = dbcon.getBranchNames(InsitutionName);
            this.listBranchNameStepOne.DisplayMember = "branchname_";
        }

        #endregion DropList Functions

        #region Nullable DatePicker

        public void nullDatePicker()
        {
            this.dtimeBatch.Value = null;
            this.dtimePrepSignedStepOne.Value = null;
            this.dtimeDOBStepTwo.Value = null;
            this.dtimeTransDateStepThree.Value = null;
        }

        #endregion Nullable DatePicker

        #region Batch Creation

        private void btnBatchSave_Click(object sender, EventArgs e)
        {
            if (this.dtimeBatch.Text.ToString() != "<Select Date>")
            {
                int bid = dbcon.getID();
                FintranList.BatchID = "FIDFTR-" + DateTime.Now.Year + "-" + bid;
                string uniqueId = dbcon.getSetID();

                Boolean result = dbcon.InsertBatch(uniqueId, FintranList.BatchID, this.dtimeBatch.Text.ToString(), this.ListbatchInst.SelectedItem.ToString(),
                   this.txtBatchTotal.Text.ToString().Trim(), this.txtBatchNote.Text.ToString().Trim());

                if (result)
                {
                    MessageBox.Show(FintranList.BatchID + " Created Sucessfully", "Batch Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //this.lblBatchMessage.Visible = true;
                    loadSelectBatch();
                    BatchReset();
                }
                else
                {
                    MessageBox.Show("Error Creating Batch Please Contact System Administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //this.lblBatchMessage.Visible = true;
                }
            }
            else
            {
                MessageBox.Show("Batch Date Not Selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Batch Creation

        #region Batch Fields Reset

        public void BatchReset()
        {
            this.dtimeBatch.Value = null;
            this.ListbatchInst.SelectedIndex = 0;
            this.txtBatchTotal.Text = string.Empty;
            this.txtBatchNote.Text = string.Empty;

            //this.btnBatchSave.Text = "Save";
            //this.lblBatchMessage.Visible = false;
        }

        #endregion Batch Fields Reset

        #region Step One - Insitution Information

        //Load Institution Name AND all it's Branch Names
        private void ListSelectBath_SelectedValueChanged(object sender, EventArgs e)
        {
            //Gets Reporting Insitution Name
            this.txtReportFIStepOne.Text = dbcon.getInsitutionName(this.ListSelectBathStepOne.SelectedValue.ToString());
            FintranList.BatchID = this.ListSelectBathStepOne.SelectedValue.ToString();

            //this.Text = "Insitution:  " + this.txtReportFIStepOne.Text.ToString().Trim() + " Batch Number: " + this.ListSelectBathStepOne.SelectedValue.ToString() + " Number of Reports Currently in Batch: "+dbcon.getReportCount(FintranList.BatchID).ToString();
            this.Report_Window.Text = "Report Main Menu - Batch Number:  " + this.ListSelectBathStepOne.SelectedValue.ToString();

            //Loads Branch Names
            ClearStepOneTwo();
            loadBranchNames(Utility.ReplaceSingleQuote(this.txtReportFIStepOne.Text.ToString().Trim()));
        }

        //Load All Branch Related Inf
        private void listBranchNameStepOne_SelectedValueChanged(object sender, EventArgs e)
        {
            //loadBranchInfo(this.txtReportFIStepOne.Text.ToString().Trim(), ((DataRowView)this.listBranchNameStepOne.SelectedItem)["branchname_"].ToString());
            ClearStepOneTwo();
            loadBranchInfo(this.txtReportFIStepOne.Text.ToString().Trim(), this.listBranchNameStepOne.Text.ToString());
        }

        //Report Number Validator
        private void txtReportNumberStepOne_Leave(object sender, EventArgs e)
        {
            if (this.btnSave_UpdateStepOne.Text != "Update")
            {
                if (!string.IsNullOrEmpty(this.txtReportNumberStepOne.Text))
                {
                    bool Isvalid = sqlDBCon.isValidReportID(this.txtReportNumberStepOne.Text.ToString().Trim());

                    if (!Isvalid) //New Report Number
                    {
                        this.reportNumState = true;
                        SteOnepSaveBtn(this.reportNumState, this.reportTypeState);
                        FintranList.ReportID = this.txtReportNumberStepOne.Text.Trim().ToString();
                    }
                    else
                    {
                        this.btnSave_UpdateStepOne.Enabled = false;
                        MessageBox.Show("Report Number already in use, please try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.txtReportNumberStepOne.Text = string.Empty;
                        this.txtReportNumberStepOne.Focus();
                    }
                }
                else
                {
                    this.btnSave_UpdateStepOne.Enabled = false;

                    //this.reportNumError.SetError(this.txtReportNumberStepOne, "Report Number is Required");
                }
            }
        }

        //Report Type Error Check
        private void listReportTypeStepOne_Validating(object sender, CancelEventArgs e)
        {
            if (this.Report_radPageView.SelectedPage == this.StepOne_PageViewOuter)
            {
                if (this.listReportTypeStepOne.SelectedItem.ToString() == "Select")
                {
                    // Cancel the event
                    e.Cancel = true;
                    this.ReportErrorControl.SetError(this.listReportTypeStepOne, "Report Type is Required");
                    this.reportTypeState = false;
                }
            }
        }

        //Report Type Error Check
        private void listReportTypeStepOne_Validated(object sender, EventArgs e)
        {
            this.ReportErrorControl.SetError(this.listReportTypeStepOne, "");
            this.reportTypeState = true;
            SteOnepSaveBtn(this.reportNumState, this.reportTypeState);
        }

        //Controls Step One Save Button Enabled State
        public void SteOnepSaveBtn(Boolean reptNum, Boolean reptType)
        {
            if (reptNum == true && reptType == true)
            {
                this.btnSave_UpdateStepOne.Enabled = true;
            }
            else
            {
                this.btnSave_UpdateStepOne.Enabled = false;
            }
        }

        //Loads Data Specific to the Insitution and It's Branch
        public void loadBranchInfo(string insitutionName, string BranchName)
        {
            SqlDataReader rd = dbcon.getBranchInformation(Utility.ReplaceSingleQuote(insitutionName), Utility.ReplaceSingleQuote(BranchName));
            while (rd.Read())
            {
                this.txtInsitutionTypeStepOne.Text = (object)rd["Inst_type"] == DBNull.Value ? string.Empty : (string)rd["Inst_type"];
                this.txtTRNStepOne.Text = (object)rd["Inst_trn"] == DBNull.Value ? string.Empty : (string)rd["Inst_trn"];
                this.txtAddressStepOne.Text = (object)rd["Inst_address"] == DBNull.Value ? string.Empty : (string)rd["Inst_address"]; ;
                this.txtBranchAddressStepOne.Text = (object)rd["branchaddr_"] == DBNull.Value ? string.Empty : (string)rd["branchaddr_"];
                this.txtPrepFNameStepOne.Text = (object)rd["preparerfn_"] == DBNull.Value ? string.Empty : (string)rd["preparerfn_"];
                this.txtPrepLNameStepOne.Text = (object)rd["preparerln_"] == DBNull.Value ? string.Empty : (string)rd["preparerln_"];
                this.txtPrepMIStepOne.Text = (object)rd["prep_mi"] == DBNull.Value ? string.Empty : (string)rd["prep_mi"];
                this.txtPrepTitleStepOne.Text = (object)rd["perp_title"] == DBNull.Value ? string.Empty : (string)rd["perp_title"];
                this.txtPrepPhoneStepOne.Text = (object)rd["prep_phone"] == DBNull.Value ? string.Empty : (string)rd["prep_phone"];

                if ((object)rd["perp_date"] != DBNull.Value)
                {
                    this.dtimePrepSignedStepOne.Value = (DateTime)rd["perp_date"];
                }
                else
                {
                    this.dtimePrepSignedStepOne.Value = null;
                }

                this.txtContLNameStepOne.Text = (object)rd["contact_ln"] == DBNull.Value ? string.Empty : (string)rd["contact_ln"];
                this.txtContFNameStepOne.Text = (object)rd["contact_fn"] == DBNull.Value ? string.Empty : (string)rd["contact_fn"];
                this.txtContMIStepOne.Text = (object)rd["contact_mi"] == DBNull.Value ? string.Empty : (string)rd["contact_mi"];
                this.txtContTitleStepOne.Text = (object)rd["cont_title"] == DBNull.Value ? string.Empty : (string)rd["cont_title"];
                this.txtContPhoneStepOne.Text = (object)rd["cont_phone"] == DBNull.Value ? string.Empty : (string)rd["cont_phone"];
            }
            rd.Close();
        }

        //Insert Step One Info Into DB
        private void btnSave_UpdateStepOne_Click(object sender, EventArgs e)
        {
            string prepdate = string.Empty;

            switch (this.btnSave_UpdateStepOne.Text)
            {
                case "Save":
                    if (this.dtimePrepSignedStepOne.Text.ToString() != "<Select Date>")
                    {
                        prepdate = this.dtimePrepSignedStepOne.Text.ToString();
                    }

                    Boolean result = dbcon.InsertTransactionReport(this.listReportTypeStepOne.SelectedItem.Text.ToString(),
                    this.txtReportNumberStepOne.Text.Trim().ToString(),
                    this.txtReportFIStepOne.Text.ToString(), this.txtAddressStepOne.Text.ToString(), this.txtTRNStepOne.Text.ToString(),
                    this.txtBranchAddressStepOne.Text.ToString(), this.txtPrepLNameStepOne.Text.ToString(), this.txtPrepFNameStepOne.Text.ToString(),
                    this.txtPrepMIStepOne.Text.ToString(), this.txtPrepTitleStepOne.Text.ToString(), this.txtPrepPhoneStepOne.Text.ToString(),
                    prepdate, this.txtContLNameStepOne.Text.ToString(), this.txtContFNameStepOne.Text.ToString(),
                    this.txtContMIStepOne.Text.ToString(), this.txtContTitleStepOne.Text.ToString(), this.txtContPhoneStepOne.Text.ToString(),
                    FintranList.BatchID, this.txtInsitutionTypeStepOne.Text.ToString());

                    if (result)
                    {
                        //this.ListSelectBathStepOne.Enabled = false;
                        //this.txtReportNumberStepOne.Enabled = false;

                        this.lblStatusMessageStepOne.Text = ("Report " + FintranList.ReportID + " Created Sucessfully");
                        this.lblStatusMessageStepOne.Visible = true;
                        this.StepTwo_PageViewOuter.Enabled = true; //Enabled Involved Person Menu
                        if (this.listReportTypeStepOne.SelectedItem.Text.ToString() == "TTR")
                        {
                            this.StepFour_PageViewOuter.Enabled = false;
                        }
                        this.Report_radPageView.SelectedPage = this.StepTwo_PageViewOuter;

                        //this.btnSave_UpdateStepOne.Text = "Update";
                        this.btnSave_UpdateStepOne.Enabled = false;
                        this.txtReportNumberStepOne.AutoCompleteCustomSource.Add(FintranList.ReportID);
                    }
                    else
                    {
                        MessageBox.Show("Error Creating Report Please Contact System Administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case "Update":
                    if (this.dtimePrepSignedStepOne.Text.ToString() != "<Select Date>")
                    {
                        prepdate = this.dtimePrepSignedStepOne.Text.ToString();
                    }

                    Boolean resultUpdate = dbcon.InsertTransactionReport(this.listReportTypeStepOne.SelectedItem.Text.ToString(),
                  this.txtReportNumberStepOne.Text.Trim().ToString(),
                  this.txtReportFIStepOne.Text.ToString(), this.txtAddressStepOne.Text.ToString(), this.txtTRNStepOne.Text.ToString(),
                  this.txtBranchAddressStepOne.Text.ToString(), this.txtPrepLNameStepOne.Text.ToString(), this.txtPrepFNameStepOne.Text.ToString(),
                  this.txtPrepMIStepOne.Text.ToString(), this.txtPrepTitleStepOne.Text.ToString(), this.txtPrepPhoneStepOne.Text.ToString(),
                  prepdate, this.txtContLNameStepOne.Text.ToString(), this.txtContFNameStepOne.Text.ToString(),
                  this.txtContMIStepOne.Text.ToString(), this.txtContTitleStepOne.Text.ToString(), this.txtContPhoneStepOne.Text.ToString(),
                  FintranList.BatchID, this.txtInsitutionTypeStepOne.Text.ToString());

                    if (resultUpdate)
                    {
                        this.lblStatusMessageStepOne.Text = ("Report " + FintranList.ReportID + " Updated Sucessfully");

                        //PageControl(true);
                        this.StepTwo_PageViewOuter.Enabled = true; //Enabled Involved Person Menu
                        if (this.listReportTypeStepOne.SelectedItem.Text.ToString() == "TTR")
                        {
                            this.StepFour_PageViewOuter.Enabled = false;
                        }
                        this.Report_radPageView.SelectedPage = this.StepTwo_PageViewOuter;
                        this.btnSave_UpdateStepOne.Text = "Update";
                    }
                    else
                    {
                        MessageBox.Show("Error Creating Report Please Contact System Administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
            }
        }

        //Clear Step One Fields
        public void ClearStepOne()
        {
            //this.ListSelectBathStepOne.SelectedIndex = 0;
            this.txtReportNumberStepOne.Text = string.Empty;
            this.listReportTypeStepOne.SelectedValue = "Select";
            this.dtimePrepSignedStepOne.Value = null;
            this.btnSave_UpdateStepOne.Enabled = false;
            this.btnSave_UpdateStepOne.Text = "Save";
            this.lblStatusMessageStepOne.Visible = false;
        }

        public void ClearStepOneTwo()
        {
            this.txtInsitutionTypeStepOne.Text = string.Empty;
            this.txtTRNStepOne.Text = string.Empty;
            this.txtAddressStepOne.Text = string.Empty;
            this.txtBranchAddressStepOne.Text = string.Empty;
            this.txtPrepFNameStepOne.Text = string.Empty;
            this.txtPrepLNameStepOne.Text = string.Empty;
            this.txtPrepMIStepOne.Text = string.Empty;
            this.txtPrepTitleStepOne.Text = string.Empty;
            this.txtPrepPhoneStepOne.Text = string.Empty;
            this.dtimePrepSignedStepOne.Value = null;
            this.txtContLNameStepOne.Text = string.Empty;
            this.txtContFNameStepOne.Text = string.Empty;
            this.txtContMIStepOne.Text = string.Empty;
            this.txtContTitleStepOne.Text = string.Empty;
            this.txtContPhoneStepOne.Text = string.Empty;
        }

        #endregion Step One - Insitution Information

        #region Step Two - Persons Involved Information

        private void btnSaveStepTwo_Click(object sender, EventArgs e)
        {
            string DOB = string.Empty;

            //FintranList.ReportID = "1";

            switch (this.btnSaveStepTwo.Text)
            {
                case "Save":

                    //Insert Step Two Info Into DB

                    if (this.dtimeDOBStepTwo.Text.ToString() != "<Select Date>")
                    {
                        DOB = this.dtimeDOBStepTwo.Text.ToString();
                    }

                    // this.listIDCredStepTwo.SelectedItem.Text.ToString()
                    //this.listIssuedByStepTwo.SelectedItem.Text.ToString()
                    // this.txtAccountStepTwo.Text.ToString().Trim()
                    Boolean result = dbcon.InsertPerson(FintranList.ReportID, this.ListInvolvementStepTwo.SelectedItem.Text.ToString(),
                        this.txtLNameStepTwo.Text.ToString().Trim(), this.txtFNameStepTwo.Text.ToString().Trim(),
                        this.txtMIStepTwo.Text.ToString().Trim(), this.txtAddressStepTwo.Text.ToString().Trim(),
                        this.listReportCountryStepTwo.SelectedItem.Text.ToString(), this.listReportParishStepTwo.Text.ToString(),
                        this.txtTRNStepTwo.Text.ToString().Trim(), this.listIDMethodStepTwo.SelectedItem.Text.ToString(),
                        idcred, issued, DOB,
                        this.txtIDNumStepTwo.Text.ToString().Trim(), this.txtAccountNumberStepTwo.Text.Trim(),
                        this.listAccountTypeStepTwo.SelectedItem.Text.ToString(), this.txtOccupationStepTwo.Text.ToString().Trim(), this.chkAgentStepTwo.Checked, this.chkBeneficiaryStepTwo.Checked);

                    if (result)
                    {
                        if (this.listReportTypeStepOne.SelectedItem.Text.ToString() == "STR")
                        {
                            this.StepFour_PageViewOuter.Enabled = true;
                        }
                        this.StepThree_PageViewOuter.Enabled = true; //Enabled Transcation Menu
                        this.btnSaveStepTwo.Enabled = false;
                        this.btnNewRecordStepTwo.Enabled = true;
                        this.lblStatusStepTwo.Text = "Report Number: " + FintranList.ReportID + " - Record Created Sucessfully";
                        this.lblStatusStepTwo.Visible = true;
                        loadGridViewStepTwo();
                    }
                    else
                    {
                        MessageBox.Show("Error Creating Report Please Contact System Administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;

                case "Update":

                    //Update Step Two Info Into DB

                    DOB = string.Empty;

                    if (this.dtimeDOBStepTwo.Text.ToString() != "<Select Date>")
                    {
                        DOB = this.dtimeDOBStepTwo.Text.ToString();
                    }

                    Boolean Updateresult = dbcon.UpdatePerson(FintranList.UniqueID, this.ListInvolvementStepTwo.Text.ToString(),
                        this.txtLNameStepTwo.Text.ToString().Trim(), this.txtFNameStepTwo.Text.ToString().Trim(),
                        this.txtMIStepTwo.Text.ToString().Trim(), this.txtAddressStepTwo.Text.ToString().Trim(),
                        this.listReportCountryStepTwo.Text.ToString(), this.listReportParishStepTwo.Text.ToString(),
                        this.txtTRNStepTwo.Text.ToString().Trim(), this.listIDMethodStepTwo.Text.ToString(),
                        idcred, issued, DOB,
                        this.txtIDNumStepTwo.Text.ToString().Trim(), this.txtAccountNumberStepTwo.Text.Trim(),
                        this.listAccountTypeStepTwo.Text.ToString(), this.txtOccupationStepTwo.Text.ToString().Trim());

                    if (Updateresult)
                    {
                        this.btnSaveStepTwo.Enabled = false;
                        this.btnNewRecordStepTwo.Enabled = true;
                        this.lblStatusStepTwo.Text = "Report Number: " + FintranList.ReportID + " - Record Updated Sucessfully";
                        this.lblStatusStepTwo.Visible = true;
                        loadGridViewStepTwo();
                    }
                    else
                    {
                        MessageBox.Show("Error Creating Report Please Contact System Administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
            }
        }

        private void btnNewRecordStepTwo_Click(object sender, EventArgs e)
        {
            ClearStepTwo();
            this.btnNewRecordStepTwo.Enabled = false;
            this.lblStatusStepTwo.Visible = false;
            this.btnDeleteStepTwo.Enabled = false;
            this.btnSaveStepTwo.Enabled = false;

            this.txtIDCredStepTwo.Visible = false;
            this.listIDCredStepTwo.Visible = true;
            this.txtIDCredStepTwo.Text = string.Empty;

            this.txtIssuedByStepTwo.Visible = false;
            this.listIssuedByStepTwo.Visible = true;
            this.txtIssuedByStepTwo.Text = string.Empty;
        }

        public void ClearStepTwo()
        {
            this.ListInvolvementStepTwo.SelectedValue = "Select";
            txtLNameStepTwo.Text = string.Empty;
            txtFNameStepTwo.Text = string.Empty;
            txtMIStepTwo.Text = string.Empty;
            txtAddressStepTwo.Text = string.Empty;
            listReportCountryStepTwo.SelectedValue = "Jamaica";
            listReportParishStepTwo.SelectedValue = "[Unknown]";
            txtTRNStepTwo.Text = string.Empty;
            listIDMethodStepTwo.SelectedValue = "Select";
            listIDCredStepTwo.SelectedValue = "[Other]";
            listIssuedByStepTwo.SelectedValue = "[Select]";
            this.dtimeDOBStepTwo.Value = null;
            txtIDNumStepTwo.Text = string.Empty;
            txtAccountNumberStepTwo.Text = string.Empty;
            listAccountTypeStepTwo.SelectedValue = "[None]";
            txtOccupationStepTwo.Text = string.Empty;
            this.lblStatusStepTwo.Visible = false;
            this.btnSaveStepTwo.Text = "Save";
            this.chkAgentStepTwo.Visible = false;
            this.chkBeneficiaryStepTwo.Visible = false;
            this.chkAgentStepTwo.Checked = false;
            this.chkBeneficiaryStepTwo.Checked = false;

            this.txtIDCredStepTwo.Visible = false;
            this.listIDCredStepTwo.Visible = true;
            this.txtIDCredStepTwo.Text = string.Empty;

            this.txtIssuedByStepTwo.Visible = false;
            this.listIssuedByStepTwo.Visible = true;
            this.txtIssuedByStepTwo.Text = string.Empty;

            this.txtAccountTypeStepTwo.Visible = false;
            this.listAccountTypeStepTwo.Visible = true;
            this.txtAccountTypeStepTwo.Text = string.Empty;
        }

        public void loadGridViewStepTwo()
        {
            this.gridViewStepTwo.Visible = true;
            this.gridViewStepTwo.DataSource = null;
            this.gridViewStepTwo.DataSource = sqlDBCon.getPersonInvolvedByReportID(FintranList.ReportID);
            this.gridViewStepTwo.Columns[0].IsVisible = false;
        }

        private void ListInvolvementStepOne_Validating(object sender, CancelEventArgs e)
        {
            if (this.Report_radPageView.SelectedPage == this.StepTwo_PageViewOuter)
            {
                if (this.ListInvolvementStepTwo.SelectedItem.ToString() == "Select")
                {
                    // Cancel the event
                    e.Cancel = true;
                    this.ReportErrorControl.SetError(this.ListInvolvementStepTwo, "Involvement Type is Required");
                    this.btnSaveStepTwo.Enabled = false;
                }
            }
        }

        private void ListInvolvementStepOne_Validated(object sender, EventArgs e)
        {
            this.ReportErrorControl.SetError(this.ListInvolvementStepTwo, "");
            this.btnSaveStepTwo.Enabled = true;
            this.lblStatusStepTwo.Visible = false;
        }

        //Controls Check box for agent and beneficiary once Customer is SELETED
        private void ListInvolvementStepTwo_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (this.ListInvolvementStepTwo.Text.ToString() == "Customer")
            {
                this.chkAgentStepTwo.Visible = true;
                this.chkBeneficiaryStepTwo.Visible = true;
                this.chkAgentStepTwo.Checked = false;
                this.chkBeneficiaryStepTwo.Checked = false;
            }
            else
            {
                this.chkAgentStepTwo.Visible = false;
                this.chkBeneficiaryStepTwo.Visible = false;
                this.chkAgentStepTwo.Checked = false;
                this.chkBeneficiaryStepTwo.Checked = false;
            }

            this.txtIDCredStepTwo.Visible = false;
            this.listIDCredStepTwo.Visible = true;
            this.txtIDCredStepTwo.Text = string.Empty;

            this.txtIssuedByStepTwo.Visible = false;
            this.listIssuedByStepTwo.Visible = true;
            this.txtIssuedByStepTwo.Text = string.Empty;

            this.txtAccountTypeStepTwo.Visible = false;
            this.listAccountTypeStepTwo.Visible = true;
            this.txtAccountTypeStepTwo.Text = string.Empty;
        }

        //Loads Person Involved for Update/Review
        public void loadPersonInvolved()
        {
            FintranList.UniqueID = Int32.Parse(this.gridViewStepTwo.SelectedRows[0].Cells[0].Value.ToString());

            SqlDataReader rd = dbcon.getPersonInvolvedInformation(FintranList.UniqueID, FintranList.ReportID);
            while (rd.Read())
            {
                this.ListInvolvementStepTwo.Text = (object)rd["Involvement__Type"] == DBNull.Value ? string.Empty : (string)rd["Involvement__Type"];
                this.txtLNameStepTwo.Text = (object)rd["LastName_"] == DBNull.Value ? string.Empty : (string)rd["LastName_"];
                this.txtFNameStepTwo.Text = (object)rd["First_Name"] == DBNull.Value ? string.Empty : (string)rd["First_Name"]; ;
                this.txtAddressStepTwo.Text = (object)rd["Address_"] == DBNull.Value ? string.Empty : (string)rd["Address_"];
                this.txtMIStepTwo.Text = (object)rd["Middle_In"] == DBNull.Value ? string.Empty : (string)rd["Middle_In"];
                this.listReportCountryStepTwo.Text = (object)rd["District_"] == DBNull.Value ? string.Empty : (string)rd["District_"];
                this.listReportParishStepTwo.Text = (object)rd["Parish_"] == DBNull.Value ? string.Empty : (string)rd["Parish_"];
                this.txtTRNStepTwo.Text = (object)rd["TRN_"] == DBNull.Value ? string.Empty : (string)rd["TRN_"];
                this.listIDMethodStepTwo.Text = (object)rd["ID__Verify_Method"] == DBNull.Value ? string.Empty : (string)rd["ID__Verify_Method"];
                this.listIDCredStepTwo.Text = (object)rd["ID_Type"] == DBNull.Value ? string.Empty : (string)rd["ID_Type"];

                if ((object)rd["DOB_"] != DBNull.Value)
                {
                    this.dtimeDOBStepTwo.Value = (DateTime)rd["DOB_"];
                }
                else
                {
                    this.dtimeDOBStepTwo.Value = null;
                }

                this.listIssuedByStepTwo.Text = (object)rd["ID_Issued_By"] == DBNull.Value ? string.Empty : (string)rd["ID_Issued_By"];
                this.txtIDNumStepTwo.Text = (object)rd["ID__Number"] == DBNull.Value ? string.Empty : (string)rd["ID__Number"];
                this.txtAccountNumberStepTwo.Text = (object)rd["Acc__Number"] == DBNull.Value ? string.Empty : (string)rd["Acc__Number"];
                this.listAccountTypeStepTwo.Text = (object)rd["Account_Type"] == DBNull.Value ? string.Empty : (string)rd["Account_Type"];
                this.txtOccupationStepTwo.Text = (object)rd["Occupation_"] == DBNull.Value ? string.Empty : (string)rd["Occupation_"];
            }
            rd.Close();
        }

        private void gridViewStepTwo_DoubleClick(object sender, EventArgs e)
        {
            loadPersonInvolved();
            this.btnSaveStepTwo.Text = "Update";
            this.btnSaveStepTwo.Enabled = true;
            this.btnDeleteStepTwo.Enabled = true;
            this.btnNewRecordStepTwo.Enabled = true;
        }

        //Delete a Person Record
        private void btnDeleteStepTwo_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this Record ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    Boolean result = dbcon.DeletePersonRecord(FintranList.UniqueID);
                    if (result)
                    {
                        this.lblStatusStepTwo.Text = "Report Number: " + FintranList.ReportID + " - Record Deleted Sucessfully";
                        this.lblStatusStepTwo.Visible = true;
                        loadGridViewStepTwo();
                        ClearStepTwo();
                        this.btnSaveStepTwo.Enabled = false;
                        this.btnNewRecordStepTwo.Enabled = false;
                        this.btnDeleteStepTwo.Enabled = false;
                        this.btnSaveStepTwo.Text = "Save";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
            }
        }

        #endregion Step Two - Persons Involved Information

        #region Step Three - Transcation Information

        private void btnSave_UpdateStepThree_Click(object sender, EventArgs e)
        {
            string transDate = string.Empty;

            //FintranList.ReportID = "1";

            switch (this.btnSave_UpdateStepThree.Text)
            {
                case "Save":

                    //Insert Step Three Info Into DB

                    if (this.dtimeTransDateStepThree.Text.ToString() != "<Select Date>")
                    {
                        transDate = this.dtimeTransDateStepThree.Text.ToString();
                    }

                    Boolean result = dbcon.InsertTransaction(FintranList.ReportID, listTransTypeStepThree.Text.ToString(),
                        this.listSelectAgentStepThree.Text.ToString(), transDate,
                        this.listReportCurrencyStepThree.Text.ToString(), this.listAccountTypeStepThree.Text.ToString(),
                        this.txtAccountStepThree.Text.ToString().Trim(), this.txtJAEqStepThree.Text.ToString().Trim(),
                        this.txtJAExRateStepThree.Text.ToString().Trim(), this.txtUSEqStepThree.Text.ToString().Trim(),
                        this.txtUSExRateStepThree.Text.ToString().Trim(), this.txtSourceFundsStepThree.Text.ToString().Trim(), this.txtTransTimeStepThree.Text.ToString().Trim(), this.txtTransAmountStepThree.Text.ToString());

                    if (result)
                    {
                        this.btnSave_UpdateStepThree.Enabled = false;
                        this.btnNewRecordStepThree.Enabled = true;
                        this.lblStatusStepThree.Text = "Report Number: " + FintranList.ReportID + " - Record Created Sucessfully";
                        this.lblStatusStepThree.Visible = true;
                        loadGridViewStepThree();

                        if (this.listReportTypeStepOne.Text.ToString() == "TTR")
                        {
                            this.btnCompletedStepThree.Visible = true;
                        }
                        else
                        {
                            this.btnCompletedStepThree.Visible = false;
                            this.StepFour_PageViewOuter.Enabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error Creating Report Please Contact System Administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;

                case "Update":

                    //Update Step Two Info Into DB

                    transDate = string.Empty;

                    if (this.dtimeTransDateStepThree.Text.ToString() != "<Select Date>")
                    {
                        transDate = this.dtimeTransDateStepThree.Text.ToString();
                    }

                    Boolean Updateresult = dbcon.UpdateTransaction(FintranList.UniqueID, listTransTypeStepThree.Text.ToString(),
                        this.listSelectAgentStepThree.Text.ToString(), transDate,
                        this.listReportCurrencyStepThree.Text.ToString(), this.listAccountTypeStepThree.Text.ToString(),
                        this.txtAccountStepThree.Text.ToString().Trim(), this.txtJAEqStepThree.Text.ToString().Trim(),
                        this.txtJAExRateStepThree.Text.ToString().Trim(), this.txtUSEqStepThree.Text.ToString().Trim(),
                        this.txtUSExRateStepThree.Text.ToString().Trim(), this.txtSourceFundsStepThree.Text.ToString().Trim(), this.txtTransTimeStepThree.Text.ToString().Trim(), this.txtTransAmountStepThree.Text.ToString());

                    if (Updateresult)
                    {
                        this.btnSave_UpdateStepThree.Enabled = false;
                        this.btnNewRecordStepThree.Enabled = true;
                        this.lblStatusStepThree.Text = "Report Number: " + FintranList.ReportID + " - Record Updated Sucessfully";
                        this.lblStatusStepThree.Visible = true;
                        loadGridViewStepThree();
                    }
                    else
                    {
                        MessageBox.Show("Error Creating Report Please Contact System Administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
            }
        }

        private void btnNewRecordStepThree_Click(object sender, EventArgs e)
        {
            ClearStepThree();
            this.btnNewRecordStepThree.Enabled = false;
            this.lblStatusStepThree.Visible = false;
            this.btnDeleteRecordStepThree.Enabled = false;
            this.btnSave_UpdateStepThree.Enabled = false;
        }

        public void ClearStepThree()
        {
            this.listTransTypeStepThree.SelectedIndex = 0;
            this.listSelectAgentStepThree.DataSource = null;
            this.listSelectAgentStepThree.DisplayMember = null;
            listReportCurrencyStepThree.SelectedValue = "[Unknown]";
            listAccountTypeStepThree.SelectedValue = "[None]";
            txtAccountStepThree.Text = string.Empty;
            txtJAEqStepThree.Value = string.Empty;
            txtJAExRateStepThree.Value = string.Empty;
            txtUSEqStepThree.Value = string.Empty;
            this.dtimeTransDateStepThree.Value = null;
            txtUSExRateStepThree.Value = string.Empty;
            txtSourceFundsStepThree.Text = string.Empty;
            this.txtTransAmountStepThree.Value = string.Empty;
            this.txtTransTimeStepThree.Text = string.Empty;
            TransTypeState = false;
            TransDateState = false;
            TransAmountState = false;
            TransCurrencyState = false;
            this.lblStatusStepThree.Visible = false;
            this.btnSave_UpdateStepThree.Text = "Save";
            this.btnNewRecordStepThree.Enabled = false;
        }

        public void loadGridViewStepThree()
        {
            this.gridViewStepThree.Visible = true;
            this.gridViewStepThree.DataSource = null;
            this.gridViewStepThree.DataSource = dbcon.getTransactionByReportID(FintranList.ReportID);
            this.gridViewStepThree.Columns[0].IsVisible = false;
        }

        #region Validation Controls

        private void listTransTypeStepThree_Validating(object sender, CancelEventArgs e)
        {
            if (this.Report_radPageView.SelectedPage == this.StepThree_PageViewOuter)
            {
                if (this.listTransTypeStepThree.SelectedItem.ToString() == "" || this.listTransTypeStepThree.SelectedItem.ToString() == "Other")
                {
                    // Cancel the event and select the text to be corrected by the user.
                    e.Cancel = true;
                    this.ReportErrorControl.SetError(this.listTransTypeStepThree, "Transaction Type is Required");
                    this.TransTypeState = false;
                }
            }
        }

        private void listTransTypeStepThree_Validated(object sender, EventArgs e)
        {
            this.ReportErrorControl.SetError(this.listTransTypeStepThree, "");
            this.TransTypeState = true;
            StepThreeSaveBtn(TransTypeState, TransDateState, TransAmountState, TransCurrencyState);
            this.lblStatusStepThree.Visible = false;
        }

        private void dtimeTransDateStepThree_Validating(object sender, CancelEventArgs e)
        {
            if (this.Report_radPageView.SelectedPage == this.StepThree_PageViewOuter)
            {
                if (this.dtimeTransDateStepThree.Text.ToString() == "<Select Date>")
                {
                    // Cancel the event and select the text to be corrected by the user.
                    e.Cancel = true;
                    this.ReportErrorControl.SetError(this.dtimeTransDateStepThree, "Transaction Date is Required");
                    this.TransDateState = false;
                }
            }
        }

        private void dtimeTransDateStepThree_Validated(object sender, EventArgs e)
        {
            this.ReportErrorControl.SetError(this.dtimeTransDateStepThree, "");
            this.TransDateState = true;
            StepThreeSaveBtn(TransTypeState, TransDateState, TransAmountState, TransCurrencyState);
            this.lblStatusStepThree.Visible = false;
        }

        private void listReportCurrencyStepThree_Validating(object sender, CancelEventArgs e)
        {
            if (this.Report_radPageView.SelectedPage == this.StepThree_PageViewOuter)
            {
                if (this.listReportCurrencyStepThree.SelectedItem.ToString() == "[Unknown]")
                {
                    // Cancel the event and select the text to be corrected by the user.
                    e.Cancel = true;
                    this.ReportErrorControl.SetError(this.listReportCurrencyStepThree, "Transaction Currency is Required");
                    this.TransCurrencyState = false;
                }
            }
        }

        private void listReportCurrencyStepThree_Validated(object sender, EventArgs e)
        {
            this.ReportErrorControl.SetError(this.listReportCurrencyStepThree, "");
            this.TransCurrencyState = true;
            StepThreeSaveBtn(TransTypeState, TransDateState, TransAmountState, TransCurrencyState);
            this.lblStatusStepThree.Visible = false;
        }

        private void txtTransAmountStepThree_Validating(object sender, CancelEventArgs e)
        {
            if (this.Report_radPageView.SelectedPage == this.StepThree_PageViewOuter)
            {
                if (this.txtTransAmountStepThree.Text == "0.00")
                {
                    // Cancel the event and select the text to be corrected by the user.
                    e.Cancel = true;
                    this.ReportErrorControl.SetError(this.txtTransAmountStepThree, "Transaction Amount is Required");
                    this.TransAmountState = false;
                }
            }
        }

        private void txtTransAmountStepThree_Validated(object sender, EventArgs e)
        {
            this.ReportErrorControl.SetError(this.txtTransAmountStepThree, "");
            this.TransAmountState = true;
            StepThreeSaveBtn(TransTypeState, TransDateState, TransAmountState, TransCurrencyState);
            this.lblStatusStepThree.Visible = false;
        }

        //Controls Step One Save Button Enabled State
        public void StepThreeSaveBtn(Boolean transType, Boolean transDate, Boolean transAmount, Boolean transCurr)
        {
            if (transType == true && transDate == true && transAmount == true && transCurr == true)
            {
                this.btnSave_UpdateStepThree.Enabled = true;
            }
            else
            {
                this.btnSave_UpdateStepThree.Enabled = false;
            }

            if (!string.IsNullOrWhiteSpace(this.listTransTypeStepThree.Text.ToString()) && this.dtimeTransDateStepThree.Text.ToString() != "<Select Date>" && !string.IsNullOrWhiteSpace(this.txtTransAmountStepThree.Text.ToString()) && !string.IsNullOrWhiteSpace(this.listReportCurrencyStepThree.Text.ToString()))
            {
                this.btnSave_UpdateStepThree.Enabled = true;
            }
        }

        #endregion Validation Controls

        //Loads Transaction Details for Update/Review
        public void loadTransactionDetails()
        {
            FintranList.UniqueID = Int32.Parse(this.gridViewStepThree.SelectedRows[0].Cells[0].Value.ToString());

            SqlDataReader rd = dbcon.getTransactionInformation(FintranList.UniqueID, FintranList.ReportID);
            while (rd.Read())
            {
                this.listTransTypeStepThree.Text = (object)rd["Tran_type"] == DBNull.Value ? string.Empty : (string)rd["Tran_type"];
                this.listSelectAgentStepThree.Text = (object)rd["JATS_Trading"] == DBNull.Value ? string.Empty : (string)rd["JATS_Trading"];
                this.listReportCurrencyStepThree.Text = (object)rd["Tran_currency"] == DBNull.Value ? string.Empty : (string)rd["Tran_currency"]; ;
                this.listAccountTypeStepThree.Text = (object)rd["Affect_Account_type"] == DBNull.Value ? string.Empty : (string)rd["Affect_Account_type"];
                this.txtAccountStepThree.Text = (object)rd["Affect_Account_no"] == DBNull.Value ? string.Empty : (string)rd["Affect_Account_no"];
                this.txtJAEqStepThree.Text = (object)rd["JA_equivalent"] == DBNull.Value ? string.Empty : rd["JA_equivalent"].ToString();
                this.txtJAExRateStepThree.Text = (object)rd["JA_rate"] == DBNull.Value ? string.Empty : rd["JA_rate"].ToString();
                this.txtUSEqStepThree.Text = (object)rd["US_equivalent"] == DBNull.Value ? string.Empty : rd["US_equivalent"].ToString();
                this.txtUSExRateStepThree.Text = (object)rd["US_rate"] == DBNull.Value ? string.Empty : rd["US_rate"].ToString();
                this.txtSourceFundsStepThree.Text = (object)rd["Fund_source"] == DBNull.Value ? string.Empty : (string)rd["Fund_source"];
                this.txtTransTimeStepThree.Text = (object)rd["tran_time"] == DBNull.Value ? string.Empty : (string)rd["tran_time"];
                this.txtTransAmountStepThree.Text = (object)rd["Tran_amount"] == DBNull.Value ? string.Empty : rd["Tran_amount"].ToString();

                if ((object)rd["tran_date"] != DBNull.Value)
                {
                    this.dtimeTransDateStepThree.Value = (DateTime)rd["tran_date"];
                }
                else
                {
                    this.dtimeTransDateStepThree.Value = null;
                }
            }
            rd.Close();
        }

        private void gridViewStepThree_DoubleClick(object sender, EventArgs e)
        {
            loadTransactionDetails();
            this.btnSave_UpdateStepThree.Text = "Update";
            this.btnSave_UpdateStepThree.Enabled = true;
            this.btnDeleteRecordStepThree.Enabled = true;
            this.btnNewRecordStepThree.Enabled = true;
        }

        //Delete Transcation Record
        private void btnDeleteRecordStepThree_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this Record ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    Boolean result = dbcon.DeleteTransactionRecord(FintranList.UniqueID);
                    if (result)
                    {
                        this.lblStatusStepThree.Text = "Report Number: " + FintranList.ReportID + " - Record Deleted Sucessfully";
                        this.lblStatusStepThree.Visible = true;
                        loadGridViewStepThree();
                        ClearStepThree();
                        this.btnSave_UpdateStepThree.Enabled = false;
                        this.btnNewRecordStepThree.Enabled = false;
                        this.btnDeleteRecordStepThree.Enabled = false;
                        this.btnSave_UpdateStepThree.Text = "Save";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
            }
        }

        //Load Agent's and Customer's Name
        private void Report_radPageView_SelectedPageChanged(object sender, EventArgs e)
        {
            if (this.Report_radPageView.SelectedPage == this.StepThree_PageViewOuter)
            {
                //this.listSelectAgentStepThree.DataSource = dbcon.getAgentReportID(FintranList.ReportID);

                //foreach (GridViewDataColumn column in this.listSelectAgentStepThree.MultiColumnComboBoxElement.Columns)
                //{
                //    column.BestFit();
                //}

                /////////////////Added this instead//////////////
                this.listSelectAgentStepThree.DataSource = null;
                loadGridViewStepThree();
            }
        }

        private void btnCompletedStepThree_Click(object sender, EventArgs e)
        {
            ReportCompleted();
            this.Report_radPageView.SelectedPage = this.StepOne_PageViewOuter;
        }

        //private void listReportCurrencyStepThree_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        //{
        //    //this.txtTransAmountStepThree.Text = string.Empty;
        //    this.txtJAEqStepThree.Value = string.Empty;
        //    this.txtJAExRateStepThree.Value = string.Empty;
        //    this.txtUSEqStepThree.Value = string.Empty;
        //    this.txtUSExRateStepThree.Value = string.Empty;

        //    if (this.listReportCurrencyStepThree.Text.ToString().Equals("JMD Jamaican Dollar"))
        //    {
        //        this.txtJAExRateStepThree.Text = "1.00";
        //    }

        //    if (this.listReportCurrencyStepThree.Text.ToString().Equals("US$ Dollars") || this.listReportCurrencyStepThree.Text.ToString().Equals("US DOLLARS"))
        //    {
        //        this.txtUSExRateStepThree.Text = "1.00";
        //    }

        //}

        //private void txtTransAmountStepThree_Leave(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(this.txtTransAmountStepThree.Text.ToString()))
        //    {
        //        //IF Jamaican Currency Selected
        //        if (this.listReportCurrencyStepThree.Text.ToString().Equals("JMD Jamaican Dollar"))
        //        {
        //            //SET JAMAICAN Equivalent
        //            this.txtJAEqStepThree.Text = this.txtTransAmountStepThree.Text.ToString();
        //        }

        //        //If US Currency Selected
        //        if (this.listReportCurrencyStepThree.Text.ToString().Equals("US$ Dollars") || this.listReportCurrencyStepThree.Text.ToString().Equals("US DOLLARS"))
        //        {
        //            //SET US Equivalent
        //            this.txtUSEqStepThree.Text = this.txtTransAmountStepThree.Text.ToString();
        //        }

        //    }
        //}

        //private void txtJAExRateStepThree_Leave(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(this.txtTransAmountStepThree.Text.ToString()) && !string.IsNullOrWhiteSpace(this.txtJAExRateStepThree.Text.ToString()))
        //    {
        //        //SET JAMAICAN Eqivalent: TransAmount * JA Exchange Rate
        //        this.txtJAEqStepThree.Value = double.Parse(this.txtTransAmountStepThree.Text.ToString()) * double.Parse(this.txtJAExRateStepThree.Text.ToString());
        //    }
        //}

        //private void txtUSExRateStepThree_Leave(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(this.txtTransAmountStepThree.Text.ToString()) && !string.IsNullOrWhiteSpace(this.txtUSExRateStepThree.Text.ToString()))
        //    {
        //        if (this.listReportCurrencyStepThree.Text.ToString().Equals("US$ Dollars") || this.listReportCurrencyStepThree.Text.ToString().Equals("US DOLLARS"))
        //        {
        //            //SET US Equivalent
        //            this.txtUSEqStepThree.Text = this.txtTransAmountStepThree.Text.ToString();
        //        }
        //        else
        //        {
        //            if (!this.txtJAEqStepThree.Text.ToString().Equals("0.00") && !this.txtUSExRateStepThree.Text.ToString().Equals("0.00"))
        //            {
        //                //SET US Eqivalent: US TransAmount / JA Exchange Rate
        //                this.txtUSEqStepThree.Value = double.Parse(this.txtJAEqStepThree.Text.ToString()) / double.Parse(this.txtUSExRateStepThree.Text.ToString());
        //            }

        //        }

        //    }
        //}

        #region Transaction Automated Calculation

        private void listReportCurrencyStepThree_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            //this.txtTransAmountStepThree.Value = string.Empty;
            this.txtJAEqStepThree.Value = string.Empty;
            this.txtJAExRateStepThree.Value = string.Empty;
            this.txtUSEqStepThree.Value = string.Empty;
            this.txtUSExRateStepThree.Value = string.Empty;

            if (this.listReportCurrencyStepThree.Text.ToString().Equals("JMD Jamaican Dollar"))
            {
                this.txtJAExRateStepThree.Text = "1.00";
                this.txtJAExRateStepThree.Value = "1.00";
            }

            if (this.listReportCurrencyStepThree.Text.ToString().Equals("US$ Dollars") || this.listReportCurrencyStepThree.Text.ToString().Equals("US DOLLARS"))
            {
                this.txtUSExRateStepThree.Text = "1.00";
                this.txtUSExRateStepThree.Value = "1.00";
            }

            if (!string.IsNullOrWhiteSpace(this.txtTransAmountStepThree.Text.ToString()) || this.txtTransAmountStepThree.Text.ToString().Equals("0.00"))
            {
                //ReCalculate if amount changed - JA Eq
                if (!string.IsNullOrWhiteSpace(this.txtJAExRateStepThree.Text.ToString()) || this.txtJAExRateStepThree.Text.ToString().Equals("0.00"))
                {
                    this.txtJAExRateStepThree_Leave(sender, e);
                }

                //ReCalculate if amount changed - US Eq
                if (!string.IsNullOrWhiteSpace(this.txtUSExRateStepThree.Text.ToString()) || this.txtUSExRateStepThree.Text.ToString().Equals("0.00"))
                {
                    this.txtUSExRateStepThree_Leave(sender, e);
                }
            }
        }

        private void txtTransAmountStepThree_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtTransAmountStepThree.Text.ToString()) || this.txtTransAmountStepThree.Text.ToString().Equals("0.00"))
            {
                //IF Jamaican Currency Selected
                if (this.listReportCurrencyStepThree.Text.ToString().Equals("JMD Jamaican Dollar"))
                {
                    //SET JAMAICAN Equivalent
                    this.txtJAEqStepThree.Text = this.txtTransAmountStepThree.Text.ToString();
                    this.txtJAEqStepThree.Value = this.txtTransAmountStepThree.Text.ToString();
                }

                //If US Currency Selected
                if (this.listReportCurrencyStepThree.Text.ToString().Equals("US$ Dollars") || this.listReportCurrencyStepThree.Text.ToString().Equals("US DOLLARS"))
                {
                    //SET US Equivalent
                    this.txtUSEqStepThree.Text = this.txtTransAmountStepThree.Text.ToString();
                    this.txtUSEqStepThree.Value = this.txtTransAmountStepThree.Text.ToString();
                }

                //ReCalculate if amount changed - JA Eq
                if (!string.IsNullOrWhiteSpace(this.txtJAExRateStepThree.Text.ToString()) || this.txtJAExRateStepThree.Text.ToString().Equals("0.00"))
                {
                    this.txtJAExRateStepThree_Leave(sender, e);
                }

                //ReCalculate if amount changed - US Eq
                if (!string.IsNullOrWhiteSpace(this.txtUSExRateStepThree.Text.ToString()) || this.txtUSExRateStepThree.Text.ToString().Equals("0.00"))
                {
                    this.txtUSExRateStepThree_Leave(sender, e);
                }
            }
        }

        private void txtJAExRateStepThree_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtTransAmountStepThree.Text.ToString()) && !string.IsNullOrWhiteSpace(this.txtJAExRateStepThree.Text.ToString()))
            {
                if (this.listReportCurrencyStepThree.Text.ToString().Equals("JMD Jamaican Dollar"))
                {
                    this.txtJAEqStepThree.Value = this.txtTransAmountStepThree.Text.ToString();
                    this.txtJAExRateStepThree.Text = "1.00";
                    this.txtJAExRateStepThree.Value = "1.00";
                }
                else
                {
                    //SET JAMAICAN Eqivalent: TransAmount * JA Exchange Rate
                    this.txtJAEqStepThree.Value = double.Parse(this.txtTransAmountStepThree.Text.ToString()) * double.Parse(this.txtJAExRateStepThree.Text.ToString());
                }
            }
        }

        private void txtUSExRateStepThree_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtTransAmountStepThree.Text.ToString()) && !string.IsNullOrWhiteSpace(this.txtUSExRateStepThree.Text.ToString()))
            {
                if (this.listReportCurrencyStepThree.Text.ToString().Equals("US$ Dollars") || this.listReportCurrencyStepThree.Text.ToString().Equals("US DOLLARS"))
                {
                    //SET US Equivalent
                    this.txtUSEqStepThree.Text = this.txtTransAmountStepThree.Text.ToString();
                    this.txtUSEqStepThree.Value = this.txtTransAmountStepThree.Text.ToString();
                }
                else
                {
                    if (!this.txtJAEqStepThree.Text.ToString().Equals("0.00") && !this.txtUSExRateStepThree.Text.ToString().Equals("0.00"))
                    {
                        //SET US Eqivalent: US TransAmount / JA Exchange Rate
                        this.txtUSEqStepThree.Value = double.Parse(this.txtJAEqStepThree.Text.ToString()) / double.Parse(this.txtUSExRateStepThree.Text.ToString());
                    }
                }
            }
        }

        #endregion Transaction Automated Calculation

        #endregion Step Three - Transcation Information

        #region Step Four - STR Reason

        private void btnSave_UpdateStepFour_Click(object sender, EventArgs e)
        {
            //FintranList.ReportID = "1";

            if (!String.IsNullOrWhiteSpace(this.txtSTRStepFour.Text.ToString()))
            {
                Boolean result = dbcon.InsertUpdateSTR(FintranList.ReportID, this.txtSTRStepFour.Text.ToString().Trim());
                if (result)
                {
                    this.btnCompletedStepFour.Visible = true;
                    this.lblStatusStepFour.Text = "Report Number: " + FintranList.ReportID + " - Record Created Sucessfully";
                    this.lblStatusStepFour.Visible = true;
                    this.btnSave_UpdateStepFour.Text = "Update";
                }
                else
                {
                    MessageBox.Show("Error Creating Report Please Contact System Administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("STR is Required");
            }
        }

        private void txtSTRStepFour_Validating(object sender, CancelEventArgs e)
        {
            if (this.Report_radPageView.SelectedPage == this.StepThree_PageViewOuter)
            {
                if (string.IsNullOrWhiteSpace(this.txtSTRStepFour.Text.ToString()))
                {
                    // Cancel the event and select the text to be corrected by the user.
                    e.Cancel = true;
                    this.ReportErrorControl.SetError(this.txtSTRStepFour, "STR is Required");
                    this.btnSave_UpdateStepFour.Enabled = false;
                }
            }
        }

        private void txtSTRStepFour_Validated(object sender, EventArgs e)
        {
            this.ReportErrorControl.SetError(this.txtSTRStepFour, "");
            this.btnSave_UpdateStepFour.Enabled = true;
        }

        private void btnCompletedStepFour_Click(object sender, EventArgs e)
        {
            ReportCompleted();
            this.Report_radPageView.SelectedPage = this.StepOne_PageViewOuter;
        }

        #endregion Step Four - STR Reason

        public void ReportCompleted()
        {
            ClearStepOne();
            ClearStepTwo();
            ClearStepThree();

            //More Step Two Reseting
            this.btnSaveStepTwo.Text = "Save";
            this.btnSaveStepTwo.Enabled = false;
            this.btnNewRecordStepTwo.Enabled = false;
            this.btnDeleteStepTwo.Enabled = false;
            this.gridViewStepTwo.DataSource = null;
            this.gridViewStepTwo.Visible = false;

            //Step Three
            this.gridViewStepThree.DataSource = null;
            this.gridViewStepThree.Visible = false;
            btnCompletedStepThree.Visible = false;

            //Step Four
            this.lblStatusStepFour.Visible = false;
            this.txtSTRStepFour.Text = string.Empty;
            this.btnSave_UpdateStepFour.Text = "Save";

            //Reset Validation
            reportNumState = false;
            reportTypeState = false;
            TransTypeState = false;
            TransDateState = false;
            TransAmountState = false;
            TransCurrencyState = false;

            PageControl(false);
        }

        private void listIDCredStepTwo_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            this.txtIDCredStepTwo.Text = string.Empty;

            if (this.listIDCredStepTwo.Text.ToString().Equals("[Other]"))
            {
                this.txtIDCredStepTwo.Location = new System.Drawing.Point(467, 71);
                this.txtIDCredStepTwo.Visible = true;
                this.listIDCredStepTwo.Visible = false;
                txtIDCredStepTwo.Focus();
            }
            else
            {
                idcred = this.listIDCredStepTwo.Text.ToString();
                this.txtIDCredStepTwo.Visible = false;
                this.listIDCredStepTwo.Visible = true;
                this.txtIDCredStepTwo.Text = string.Empty;
            }
        }

        private void txtIDCredStepTwo_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.txtIDCredStepTwo.Text))
            {
                this.listIDCredStepTwo.Visible = true;
                this.txtIDCredStepTwo.Visible = false;
                this.txtIDCredStepTwo.Text = string.Empty;
            }
            else
            {
                idcred = this.txtIDCredStepTwo.Text.ToString().Trim();
            }
        }

        private void listIssuedByStepTwo_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            this.txtIssuedByStepTwo.Text = string.Empty;

            if (this.listIssuedByStepTwo.Text.ToString().Equals("Other"))
            {
                this.txtIssuedByStepTwo.Location = new System.Drawing.Point(467, 97);
                this.txtIssuedByStepTwo.Visible = true;
                this.listIssuedByStepTwo.Visible = false;
                txtIssuedByStepTwo.Focus();
            }
            else
            {
                issued = this.listIssuedByStepTwo.Text.ToString();
                this.txtIssuedByStepTwo.Visible = false;
                this.listIssuedByStepTwo.Visible = true;
                this.txtIssuedByStepTwo.Text = string.Empty;
            }
        }

        private void txtIssuedByStepTwo_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.txtIssuedByStepTwo.Text))
            {
                this.listIssuedByStepTwo.Visible = true;
                this.txtIssuedByStepTwo.Visible = false;
                this.txtIssuedByStepTwo.Text = string.Empty;
            }
            else
            {
                issued = this.txtIssuedByStepTwo.Text.ToString().Trim();
            }
        }

        private void listAccountTypeStepTwo_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            this.txtAccountTypeStepTwo.Text = string.Empty;

            if (this.listAccountTypeStepTwo.Text.ToString().Equals("[Other]"))
            {
                this.txtAccountTypeStepTwo.Location = new System.Drawing.Point(467, 175);
                this.txtAccountTypeStepTwo.Visible = true;
                this.listAccountTypeStepTwo.Visible = false;
                txtAccountTypeStepTwo.Focus();
            }
            else
            {
                accountType = this.listAccountTypeStepTwo.Text.ToString();
                this.txtAccountTypeStepTwo.Visible = false;
                this.listAccountTypeStepTwo.Visible = true;
                this.txtAccountTypeStepTwo.Text = string.Empty;
            }
        }

        private void txtAccountTypeStepTwo_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.txtAccountTypeStepTwo.Text))
            {
                this.listAccountTypeStepTwo.Visible = true;
                this.txtAccountTypeStepTwo.Visible = false;
                this.txtAccountTypeStepTwo.Text = string.Empty;
            }
            else
            {
                accountType = this.txtAccountTypeStepTwo.Text.ToString().Trim();
            }
        }

        private void gridViewStepThree_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            if (e.CellElement.ColumnInfo.HeaderText == "Tran_amount")
            {
                double value = 0.0;
                if (double.TryParse(e.CellElement.RowInfo.Cells["Tran_amount"].Value.ToString(), out value))
                {
                    e.CellElement.Text = string.Format(CultureInfo.CreateSpecificCulture("en-us"), "{0:0,0.0}", value);
                }
            }
        }

        private void setBackGroundColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.StepOne_PageViewOuter.BackColor = colorDlg.Color;
                this.groupBoxPart1.BackColor = colorDlg.Color;
                this.groupBoxPart3.BackColor = colorDlg.Color;
                this.groupBoxBatch.BackColor = colorDlg.Color;
                this.groupBoxPart4.BackColor = colorDlg.Color;

                this.groupBoxStepTwo.BackColor = colorDlg.Color;
                this.groupBoxStepThree.BackColor = colorDlg.Color;
                this.groupBoxStepFour.BackColor = colorDlg.Color;
                this.Refresh();
            }
        }

        private void topStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.Report_radPageView.GetChildAt(0))).StripAlignment = Telerik.WinControls.UI.StripViewAlignment.Top;
            this.Refresh();
        }

        private void rightStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.Report_radPageView.GetChildAt(0))).StripAlignment = Telerik.WinControls.UI.StripViewAlignment.Right;
            this.Refresh();
        }

        private void bottomStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.Report_radPageView.GetChildAt(0))).StripAlignment = Telerik.WinControls.UI.StripViewAlignment.Bottom;
            this.Refresh();
        }

        private void leftStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Telerik.WinControls.UI.RadPageViewStripElement)(this.Report_radPageView.GetChildAt(0))).StripAlignment = Telerik.WinControls.UI.StripViewAlignment.Left;
            this.Refresh();
        }

        //CREATE THE LISTING OF CUSTOMERS/AGENTS
        private void listSelectAgentStepThree_Enter(object sender, EventArgs e)
        {
            DataTable personsTBL = dbcon.getAgentReportID(FintranList.ReportID);
            List<DataRow> pTBList = personsTBL.AsEnumerable().ToList();
            List<ReportEO.PersonStepThree> Ppersons = new List<ReportEO.PersonStepThree>();
            Ppersons.Clear();

            foreach (var person in pTBList)
            {
                ReportEO.PersonStepThree PP = new ReportEO.PersonStepThree();
                PP.name = person.ItemArray[0].ToString();
                PP.InvolvementType = person.ItemArray[1].ToString();
                Ppersons.Add(PP);
            }
            RadGridView gridViewControl = this.listSelectAgentStepThree.EditorControl;
            gridViewControl.Templates.Clear();
            this.listSelectAgentStepThree.DataSource = Ppersons;
            foreach (GridViewDataColumn column in this.listSelectAgentStepThree.MultiColumnComboBoxElement.Columns)
            {
                column.BestFit();
            }
        }

        //USE BACKSPACE TO DELETE UNWANTED ENTRIES
        private void listSelectAgentStepThree_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Back))
            {
                this.listSelectAgentStepThree.DataSource = null;
                this.listSelectAgentStepThree.DisplayMember = null;
            }
        }

        ////Load Agent's and Customer's Name
        //private void listSelectAgentStepThree_DropDownOpened(object sender, EventArgs e)
        //{
        //        this.listSelectAgentStepThree.DataSource = dbcon.getAgentReportID(FintranList.ReportID);

        //        foreach (GridViewDataColumn column in this.listSelectAgentStepThree.MultiColumnComboBoxElement.Columns)
        //        {
        //            column.BestFit();
        //        }
        //}

        //private void listSelectAgentStepThree_Click(object sender, EventArgs e)
        //{
        //    this.listSelectAgentStepThree.DataSource = dbcon.getAgentReportID(FintranList.ReportID);

        //    foreach (GridViewDataColumn column in this.listSelectAgentStepThree.MultiColumnComboBoxElement.Columns)
        //    {
        //        column.BestFit();
        //    }

        //}
    }
}