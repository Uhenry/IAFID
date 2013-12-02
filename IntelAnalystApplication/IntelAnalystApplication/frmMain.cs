using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using QuickXmlReader.Forms.DataUpload;
using QuickXmlReader.Forms.Fintran;
using QuickXmlReader.Functions;
using Telerik.WinControls.UI;

namespace QuickXmlReader
{
    public partial class frmMain : Telerik.WinControls.UI.RadForm
    {
        private dbConnections dbcon = new dbConnections();
        private List<int> UserMenuList = new List<int>(); //User Menu List
        private XML_Uploader XML_data; // XML/PDF DATA UPLOADER MENU
        private Fintran_Main fintran;
        private Fintran_Review fintranReview;
        private Frm_DataEntryReport dataEntry;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.timer.Start();
            this.lblStatusLogin.Text = "Username: " + UserInfo.userName + "/" + "Login Time: " + UserInfo.UserLoginTime;
            loadTabs();
            btnsOff();
        }

        private void dataUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XML_data = Application.OpenForms["XML_Uploader"] as XML_Uploader; //XML / PDF DATA UPLOADER MENU
            if (XML_data != null)
            {
                XML_data.WindowState = FormWindowState.Maximized;
                XML_data.BringToFront();
                XML_data.Focus();
            }
            else
            {
                XML_data = new XML_Uploader(this);
                XML_data.WindowState = FormWindowState.Normal;
                XML_data.MdiParent = this;
                XML_data.Show();
                XML_data.WindowState = FormWindowState.Maximized;
            }

            //dbcon.LoguserInter(UserInfo.userId, false, DateTime.Now, "POCA Query", "System");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblSystemTimer.Text = DateTime.Now.ToLongTimeString();
        }

        public void loadTabs()
        {
            UserMenuList = dbcon.userMenu(UserInfo.userId);
            UserMenuList.Sort();
            foreach (int menu in UserMenuList)
            {
                switch (menu)
                {
                    //Data Upload Menu
                    case 1:
                        this.dataUploadToolStripMenuItem.Visible = true;
                        this.XMLToolStripButton.Visible = true;
                        this.PDFToolStripButton.Visible = true;
                        this.XML_PDF_toolStripSeparator.Visible = true;
                        this.btnUpload.Enabled = false;
                        this.btnReject.Enabled = false;
                        break;

                    //Data Entry Menu
                    case 2:
                        break;
                }
            }
        }

        private void newBatchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fintran = Application.OpenForms["Fintran_Main"] as Fintran_Main; //XML / PDF DATA UPLOADER MENU
            if (fintran != null)
            {
                fintran.WindowState = FormWindowState.Maximized;
                fintran.BringToFront();
                fintran.Focus();
            }
            else
            {
                fintran = new Fintran_Main();
                fintran.WindowState = FormWindowState.Normal;
                fintran.MdiParent = this;
                fintran.Show();
                fintran.WindowState = FormWindowState.Maximized;
            }

            //dbcon.LoguserInter(UserInfo.userId, false, DateTime.Now, "POCA Query", "System");
        }

        //Fintran Report Creation
        private void createReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.newReportToolStripMenuItem.PerformClick();
        }

        //Fintran Report Creation
        private void FintranToolStripButton1_Click(object sender, EventArgs e)
        {
            this.newReportToolStripMenuItem.PerformClick();
        }

        //Fintran Report and Batch Review
        private void batchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fintranReview = Application.OpenForms["Fintran_Review"] as Fintran_Review;
            if (fintranReview != null)
            {
                fintranReview.WindowState = FormWindowState.Maximized;
                fintranReview.BringToFront();
                fintranReview.Focus();
            }
            else
            {
                fintranReview = new Fintran_Review();
                fintranReview.WindowState = FormWindowState.Normal;
                fintranReview.MdiParent = this;
                fintranReview.Show();
                fintranReview.WindowState = FormWindowState.Maximized;
            }
        }

        private void dataEntryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataEntry = Application.OpenForms["Frm_DataEntryReport"] as Frm_DataEntryReport;
            if (dataEntry != null)
            {
                dataEntry.WindowState = FormWindowState.Maximized;
                dataEntry.BringToFront();
                dataEntry.Focus();
            }
            else
            {
                dataEntry = new Frm_DataEntryReport();
                dataEntry.WindowState = FormWindowState.Normal;
                dataEntry.MdiParent = this;
                dataEntry.Show();
                dataEntry.WindowState = FormWindowState.Maximized;
            }
        }

        private void XMLToolStripButton_Click(object sender, EventArgs e)
        {
            this.dataUploadToolStripMenuItem.PerformClick();
            XML_data.loadXMLToolStripMenuItem.PerformClick();
            btnsEnabled();

            // btnsOn();
        }

        private void PDFToolStripButton_Click(object sender, EventArgs e)
        {
            this.dataUploadToolStripMenuItem.PerformClick();
            XML_data.loadPDFToolStripMenuItem.PerformClick();

            //btnsOn();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            this.dataUploadToolStripMenuItem.PerformClick();
            XML_data.uploadPDFDataToolStripMenuItem.PerformClick();
            btnsOff();
        }

        private void uploadPDFDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataUploadToolStripMenuItem.PerformClick();
            XML_data.uploadPDFDataToolStripMenuItem.PerformClick();
            btnsOff();
        }

        internal void btnsOff()
        {
            btnUpload.Visible = false;
            btnReject.Visible = false;
            macTrackBar1.Visible = false;
        }

        internal void btnsOn()
        {
            btnUpload.Visible = true;
            btnReject.Visible = true;
            macTrackBar1.Visible = false;
        }

        internal void btnsEnabled()
        {
            btnUpload.Enabled = true;
            btnReject.Enabled = false;
            macTrackBar1.Enabled = false;
        }

        internal void btnsDisabled()
        {
            btnUpload.Enabled = false;
            btnReject.Enabled = false;
            macTrackBar1.Enabled = false;
        }

        private void radProgressBar2_ValueChanged(object sender, ProgressBarEventArgs e)
        {
            radProgressBar2.Text = Convert.ToInt32((((float)radProgressBar2.Value1 / (float)radProgressBar2.Maximum) * 100)) + " %";
        }

        private void rdWaitingBar_WaitingStarted(object sender, EventArgs e)
        {
            rdWaitingBar.Text = Convert.ToInt32((((float)radProgressBar2.Value1 / (float)radProgressBar2.Maximum) * 100)) + " %";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show("The application version number is " + appVersion.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}