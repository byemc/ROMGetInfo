using ROMIdentifier;
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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void StatusBarCallback(string status, int? percent)
        {
            statusText.Text = status;
            toolStripProgressBar1.Value = percent ?? 1;
            if (percent is null)
                toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            else
                toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string file = ((string[])e.Data.GetData(DataFormats.FileDrop)).FirstOrDefault();
            if (file is null)
                return;

            UseWaitCursor = true;

            var ident = RomFile.Identify(file, StatusBarCallback);

            UseWaitCursor = false;

            infoDump.Text = ident.TitleId;
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }
    }
}
