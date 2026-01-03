using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROMGetInfo
{
    public partial class DownloadDialog : Form
    {
        private Func<Task<bool>> DownloadFuncAsync;

        public DownloadDialog(string downloadName, Func<Task<bool>> downloadUnc)
        {
            InitializeComponent();
            label1.Text = String.Format(label1.Text, downloadName);
            DownloadFuncAsync = downloadUnc;
        }

        private async void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void downloadButton_Click(object sender, EventArgs e)
        {
            closeButton.Enabled = false;
            downloadButton.Enabled = false;

            UseWaitCursor = true;

            var downloaded = await DownloadFuncAsync();

            closeButton.Enabled = true;
            UseWaitCursor = false;

            if (downloaded)
            {
                label1.Text = "Successfully downloaded!!";
                return;
            }

            // Failed!!
            label1.ForeColor = Color.Red;
            label1.Text = "Downloading failed!!";
        }
    }
}
