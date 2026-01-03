using ROMIdentifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;


namespace ROMGetInfo
{
    public partial class MainForm : Form
    {
        public string CurrentFile;

        public MainForm()
        {
            InitializeComponent();
        }

        private void OpenFile(string path)
        {
            CurrentFile = path;

            var results = RomFile.Identify(path, StatusBarCallback);
            var best = results.GetBestResult();

            if (best is null)
            {
                best = new RomScanningResult()
                {
                    Success = false,
                    Confidence = 0,
                    Message = "No valid scanners found for input file"
                };
            }

            // Setting values
            filePath.Text = Path.GetFileName(results.Path);
            filetypeBox.Text = $"{results.Filetype.Description} ({results.Filetype.Mime})";
            
            gameIdBox.Text = best.Details.TitleId;
            titleBox.Text = best.Details.Title;
            textBox1.Text = best.Scanner?.FullName ?? "Unknown";

            if (!best.Success)
            {
                gameIdBox.Text = "IT DID NOT WORK";
                titleBox.Text = best.Message;
            }

            listView1.Items.Clear();
            foreach (var result in results.Results)
            {
                ListViewItem item = new ListViewItem(result.Scanner.ToString());
                item.Group = result.Success ? listView1.Groups[1] : listView1.Groups[0];
                item.SubItems.Add(result.Success.ToString());
                item.SubItems.Add(result.Message);
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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

            OpenFile(file);
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

        private Task<bool> DownloadWii()
        {
            return ROMIdentifier.Utils.GameTDB.DownloadAsync(ROMIdentifier.Utils.GameTDBPlatform.WiiGameCube, false);
        }

        private void wiiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new DownloadDialog("wiitdb.zip", DownloadWii);
            if (dlg.ShowDialog() != DialogResult.Cancel)
            {
                if (CurrentFile != null) OpenFile(CurrentFile);
            }
        }
    }
}
