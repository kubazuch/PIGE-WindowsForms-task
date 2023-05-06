using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;


namespace DiskSpaceAnalyzer
{
    public partial class SelectDialog : Form
    {

        public enum Mode
        {
            ALL,
            INDIVIDUAL,
            FOLDER
        }

        public Mode DriveMode
        {
            get
            {
                if (individualRadio.Checked) return Mode.INDIVIDUAL;
                if (folderRadio.Checked) return Mode.FOLDER;
                return Mode.ALL;
            }
        }

        public string FolderName => folderChosen.Text;

        public DriveInfo[] Drives => driveView.SelectedItems.OfType<ListViewItem>().Select(item => item.Tag as DriveInfo).ToArray()!;

        public SelectDialog()
        {
            InitializeComponent();
            Initialize();
        }

        private ProgressListView driveView;
        private ColumnHeader columnName;
        private ColumnHeader columnTotal;
        private ColumnHeader columnFree;
        private ColumnHeader columnUsed;
        private ColumnHeader columnProgress;
        private void Initialize()
        {
            driveView = new ProgressListView(3);
            columnName = new ColumnHeader();
            columnTotal = new ColumnHeader();
            columnFree = new ColumnHeader();
            columnProgress = new ColumnHeader();
            columnUsed = new ColumnHeader();
            driveView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            driveView.Columns.AddRange(new[] {columnName, columnTotal, columnFree, columnProgress, columnUsed});
            driveView.FullRowSelect = true;
            driveView.Location = new Point(12, 62);
            driveView.Name = "driveView";
            driveView.Size = new Size(460, 104);
            driveView.TabIndex = 2;
            driveView.UseCompatibleStateImageBehavior = false;
            driveView.View = View.Details;
            driveView.SelectedIndexChanged += driveView_SelectedIndexChanged;
            columnName.Text = "Name";
            columnTotal.Text = "Total";
            columnFree.Text = "Free";
            columnProgress.Text = "Used/Total";
            columnUsed.Text = "Used/Total";

            Controls.Add(driveView);

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                double fill = 1.0 - 1.0 * drive.AvailableFreeSpace / drive.TotalSize;
                ListViewItem newItem = new ListViewItem(new[] { drive.Name,
                    Utils.FormatFileSize(drive.TotalSize),
                    Utils.FormatFileSize(drive.AvailableFreeSpace),
                    "",
                    fill.ToString("0.00%")});
                newItem.Tag = drive;
                driveView.Items.Add(newItem);
                ProgressBar pb = new ProgressBar();
                pb.Value = (int) (fill * pb.Maximum);
                driveView.AddEmbeddedControl(pb, newItem);
            }

            for (int i = 0; i < 5; ++i)
                driveView.Columns[i].Width = -2;
        }

        private void driveView_SelectedIndexChanged(object sender, EventArgs e)
        {
            individualRadio.Checked = true;
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            // https://stackoverflow.com/questions/11624298/how-do-i-use-openfiledialog-to-select-a-folder
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    folderChosen.Text = fbd.SelectedPath;
                    folderRadio.Checked = true;
                }
            }
        }

        private void folderChosen_TextChanged(object sender, EventArgs e)
        {
            folderChosen.ForeColor = !Directory.Exists(folderChosen.Text) ? Color.Red : Color.Black;
        }

        private void modalOK_Click(object sender, EventArgs e)
        {
            if (folderRadio.Checked && !Directory.Exists(folderChosen.Text))
            {
                MessageBox.Show("Invalid path!", "Error", MessageBoxButtons.OK);
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}
