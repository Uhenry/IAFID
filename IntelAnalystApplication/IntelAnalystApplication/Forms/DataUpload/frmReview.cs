using System;
using System.Windows.Forms;

namespace QuickXmlReader.Forms.DataUpload
{
    public partial class frmReview : Form
    {
        private XML_Uploader _parent;

        public frmReview(XML_Uploader parent)
        {
            _parent = parent;
            InitializeComponent();
            radTrkBar.Value = _parent.pdfFiles.Count;
        }

        private void radTrkBar_ValueChanged(object sender, EventArgs e)
        {
            this.radTextBox1.Text = radTrkBar.Value.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            _parent.fileReviewPercent = (int)radTrkBar.Value;
            this.Close();
        }
    }
}