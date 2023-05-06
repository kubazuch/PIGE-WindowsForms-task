using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskSpaceAnalyzer
{
    // https://www.codeproject.com/Articles/9188/Embedding-Controls-in-a-ListView
    public class ProgressListView : ListView
    {
        private struct EmbeddedProgress
        {
            public ProgressBar Bar;
            public ListViewItem Item;
        }

        public int Column { get; }

        private List<EmbeddedProgress> embeddedBars = new ();

        public ProgressListView(int column)
        {
            Column = column;
        }

        private Rectangle CalcProgressBounds(ListViewItem item)
        {
            Rectangle itemBounds = item.GetBounds(ItemBoundsPortion.Entire);
            int x = itemBounds.X;
            for (int i = 0; i < Column; ++i)
                x += this.Columns[i].Width;

            return new Rectangle(x, itemBounds.Top, this.Columns[Column].Width, itemBounds.Height);
        }

        public void AddEmbeddedControl(ProgressBar bar, ListViewItem item)
        {
            EmbeddedProgress eb;
            eb.Bar = bar;
            eb.Item = item ;

            embeddedBars.Add(eb);

            // Add a Click event handler to select the ListView row when an embedded control is clicked
            bar.Click += new EventHandler(_embeddedControl_Click);

            this.Controls.Add(bar);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0xF:
                    // Calculate the position of all embedded controls
                    foreach (EmbeddedProgress eb in embeddedBars)
                    {
                        Rectangle rc = this.CalcProgressBounds(eb.Item);

                        if ((this.HeaderStyle != ColumnHeaderStyle.None) &&
                            (rc.Top < this.Font.Height)) // Control overlaps ColumnHeader
                        {
                            eb.Bar.Visible = false;
                            continue;
                        }

                        eb.Bar.Visible = true;

                        // Set embedded control's bounds
                        eb.Bar.Bounds = rc;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

		private void _embeddedControl_Click(object sender, EventArgs e)
        {
            // When a control is clicked the ListViewItem holding it is selected
            foreach (EmbeddedProgress ec in embeddedBars)
            {
                if (ec.Bar == (Control)sender)
                {
                    this.SelectedItems.Clear();
                    ec.Item.Selected = true;
                }
            }
        }
    }
}
