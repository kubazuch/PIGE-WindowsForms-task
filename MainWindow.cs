using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DiskSpaceAnalyzer
{
    public partial class MainWindow : Form
    {
        private DirStatDict? _counts;
        private Chart chart;

        public MainWindow()
        {
            InitializeComponent();

            chart = new Chart();

            chart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chart.Location = new Point(20, 50);
            chart.Name = "chartControl";
            chart.Size = new Size(482, 306);

            chartPage.Controls.Add(chart);
            chartType.SelectedIndex = 0;
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectDialog dialog = new SelectDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileView.Nodes.Clear();

                _counts?.Clear();
                _counts = null;

                countingWorker.CancelAsync();
                while (countingWorker.IsBusy)
                    Application.DoEvents();

                switch (dialog.DriveMode)
                {
                    case SelectDialog.Mode.ALL:
                        ShowDrives(DriveInfo.GetDrives());
                        break;
                    case SelectDialog.Mode.INDIVIDUAL:
                        ShowDrives(dialog.Drives);
                        break;
                    case SelectDialog.Mode.FOLDER:
                        TreeNode node = new TreeNode(new DirectoryInfo(dialog.FolderName).Name);
                        node.Tag = dialog.FolderName;
                        node.Nodes.Add("...");
                        fileView.Nodes.Add(node);

                        countingWorker.RunWorkerAsync(new[] {dialog.FolderName});
                        break;
                }
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            ShowDrives(DriveInfo.GetDrives());
        }

        private void ShowDrives(DriveInfo[] drives)
        {
            List<string> paths = new List<string>();
            foreach (DriveInfo di in drives)
            {
                TreeNode node = new TreeNode(di.Name);
                node.Tag = di.Name;

                if (di.IsReady)
                    node.Nodes.Add("...");

                fileView.Nodes.Add(node);
                paths.Add(di.Name);
            }

            countingWorker.RunWorkerAsync(paths.ToArray());
        }

        private void fileView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // https://codehill.com/2013/06/list-drives-and-folders-in-a-treeview-using-c/
            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
                {
                    e.Node.Nodes.Clear();

                    string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString()!);
                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        TreeNode node = new TreeNode(di.Name);

                        try
                        {
                            node.Tag = dir;
                            if (di.GetDirectories().Length > 0 || di.GetFiles().Length > 0)
                                node.Nodes.Add(null, "...");
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }
                    }

                    string[] files = Directory.GetFiles(e.Node.Tag.ToString()!);
                    List<TreeNode> nodes = new List<TreeNode>();
                    foreach (string file in files)
                    {
                        TreeNode node = new TreeNode(Path.GetFileName(file));
                        node.Tag = file;
                        nodes.Add(node);
                    }

                    if (files.Length > 3)
                    {
                        TreeNode node = new TreeNode("<File>");
                        node.Tag = e.Node.Tag.ToString();
                        node.Nodes.AddRange(nodes.ToArray());
                        e.Node.Nodes.Add(node);
                    }
                    else
                    {
                        e.Node.Nodes.AddRange(nodes.ToArray());
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            countingWorker.CancelAsync();
            while(countingWorker.IsBusy)
                Application.DoEvents();
            Close();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            countingWorker.CancelAsync();
        }

        private void countingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] paths = (string[]) e.Argument;
            string searchPattern = "*.*";

            DirStatDict fileCounts = new();

            countingWorker.ReportProgress(0);

            bool HandleFile(string file)
            {
                if (countingWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return true;
                }

                string currentDir = Path.GetDirectoryName(file);
                while (!string.IsNullOrEmpty(currentDir))
                {
                    fileCounts[currentDir].Add(file);
                    currentDir = Path.GetDirectoryName(currentDir);
                }

                fileCounts[file] = new DirStat(file, true);

                return false;
            }

            IEnumerable<string> dirs = paths.SelectMany(Directory.GetDirectories);
            IEnumerable<string> files = paths.SelectMany(path =>
                Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly));
            foreach (var file in files)
            {
                if (HandleFile(file)) return;
            }

            int i = 0;
            int count = dirs.Count();
            foreach (var dir in dirs)
            {
                foreach (var file in Utils.GetFiles(dir, searchPattern))
                {
                    if (HandleFile(file)) return;
                }

                i++;
                countingWorker.ReportProgress(i * 100 / count);
            }

            e.Result = fileCounts;
        }

        private void countingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            statusProgressBar.Value = e.ProgressPercentage;
        }

        private void countingWorker_RunWorkerCompleted(object sender,
            System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                statusProgressBar.Value = 0;
                return;
            }

            statusProgressBar.Value = 100;
            _counts = (DirStatDict) e.Result;
            if(fileView.SelectedNode != null)
                handleSelect(fileView.SelectedNode);
        }

        private void fileView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            handleSelect(e.Node);
        }

        private void handleSelect(TreeNode node)
        {
            pathLabel.Text = (string) node.Tag;
            sizeLabel.Text = "Calculating...";
            itemsLabel.Text = "Calculating...";
            filesLabel.Text = "Calculating...";
            subdirsLabel.Text = "Calculating...";
            lastChangeLabel.Text = File.GetLastWriteTime(pathLabel.Text).ToString();

            chart.Stat = null;

            if (_counts == null) return;
            DirStat stat = _counts[pathLabel.Text];
            chart.Stat = stat;

            sizeLabel.Text = Utils.FormatFileSize(stat.Size);
            int files = stat.Files;
            int subdirs = stat.Subdirs;
            itemsLabel.Text = (files + subdirs).ToString();
            filesLabel.Text = files.ToString();
            subdirsLabel.Text = subdirs.ToString();
        }

        private void chartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart.Type = (Chart.ChartType) chartType.SelectedIndex;
        }
    }
}