using System;
using System.Windows.Forms;
using QuickXmlReader.Functions;
using Telerik.WinControls.UI.Export;

namespace QuickXmlReader.Forms.Fintran
{
    public partial class Frm_DataEntryReport : Telerik.WinControls.UI.RadForm
    {
        private dbConnections dbcon = new dbConnections();
        private DateTime date;

        public Frm_DataEntryReport()
        {
            InitializeComponent();
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (DateTime.Parse((this.dtimeStart.Value.ToShortDateString())) < this.dtimeEnd.Value)
            {
                if (this.dtimeEnd.Value.ToShortDateString().Equals(DateTime.Now.ToShortDateString()))
                {
                    date = DateTime.Now;
                }
                else
                {
                    string dt = dtimeEnd.Value.ToShortDateString();
                    date = DateTime.Parse(dt).AddDays(1);
                }

                PleaseWait wait = new PleaseWait();
                wait.Show();
                Application.DoEvents();
                loadGrid(DateTime.Parse((this.dtimeStart.Value.ToShortDateString())), date);
                loadGridDetails(DateTime.Parse((this.dtimeStart.Value.ToShortDateString())), date);
                wait.Close();
                wait.Dispose();
                dbcon.AuditUser(UserInfo.userName, "Data Entry Report - Criteria = Between  " + this.dtimeStart.Value.ToShortDateString() + " and " + date.ToShortDateString(), 2, DateTime.Now);
            }
            else
            {
                MessageBox.Show("Start Date cannot be greater than End Date");
            }
        }

        public void loadGrid(DateTime StartDate, DateTime endDate)
        {
            this.chartDataEntry.DataSource = dbcon.getDataEntryReportBasic(StartDate, endDate);
            this.chartDataEntry.PlotArea.XAxis.DataLabelsColumn = "Create_User";
            this.chartDataEntry.ChartTitle.TextBlock.Text = "Data Entry Report - Between the period " + this.dtimeStart.Value.ToShortDateString() + " & " + this.dtimeEnd.Value.ToShortDateString();
            this.chartDataEntry.DataBind();
        }

        public void loadGridDetails(DateTime StartDate, DateTime endDate)
        {
            this.gridviewReport.Visible = true;
            this.gridviewReport.DataSource = null;
            this.gridviewReport.DataSource = dbcon.getDataEntryReport(StartDate, endDate);
            int reportSum = 0;
            int TransSum = 0;

            //Sum total Reports done
            for (int i = 0; i < this.gridviewReport.Rows.Count; i++)
            {
                reportSum += int.Parse(this.gridviewReport.Rows[i].Cells[1].Value.ToString());
            }

            //Sum total Transaction done
            for (int i = 0; i < this.gridviewReport.Rows.Count; i++)
            {
                TransSum += int.Parse(this.gridviewReport.Rows[i].Cells[2].Value.ToString());
            }

            this.lblReportCount.Text = reportSum.ToString();
            this.lblTransactionCount.Text = TransSum.ToString();
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel|*.xls";
            DialogResult result = saveFile.ShowDialog();
            string pathName = saveFile.FileName;
            if (result == DialogResult.OK)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(pathName))
                    {
                        ExportGridResults(pathName);
                    }
                    else
                    {
                        MessageBox.Show("No data to save", "Export Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        public void ExportGridResults(string fileName)
        {
            try
            {
                if (this.gridviewReport.RowCount > 0)
                {
                    PleaseWait wait = new PleaseWait();
                    wait.Show();
                    Application.DoEvents();
                    var excel = new Telerik.WinControls.UI.Export.ExportToExcelML(this.gridviewReport);
                    excel.ExportVisualSettings = false;
                    excel.HiddenColumnOption = HiddenOption.ExportAlways;
                    excel.SheetMaxRows = ExcelMaxRows._65536;
                    excel.RunExport(fileName);
                    wait.Close();
                    dbcon.AuditUser(UserInfo.userName, "Data Entry Report Exported - Criteria = Between  " + this.dtimeStart.Value.ToShortDateString() + " and " + date.ToShortDateString(), 2, DateTime.Now);
                    MessageBox.Show("Result successful exported", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(fileName);
                }
                else
                {
                    MessageBox.Show("No data to save", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}