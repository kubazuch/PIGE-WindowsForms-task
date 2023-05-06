using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskSpaceAnalyzer
{
    public partial class Chart : Control
    {
        private static readonly int MARGIN = 10;

        StringFormat axisFormat = new StringFormat();
        StringFormat labelFormat = new StringFormat();
        StringFormat pieFormat = new StringFormat();

        public enum ChartType
        {
            Bar,
            Pie,
            LogBar
        };

        private ChartType _chartType;
        public ChartType Type
        {
            get => _chartType;
            set
            {
                _chartType = value;
                Invalidate();
            }
        }

        private DirStat? _dirStat;
        public DirStat? Stat
        {
            get => _dirStat;
            set
            {
                _dirStat = value;
                Invalidate();
            }
        }

        public Chart()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);

            axisFormat.Alignment = StringAlignment.Far;
            axisFormat.LineAlignment = StringAlignment.Center;
            labelFormat.Alignment = StringAlignment.Center;
            labelFormat.LineAlignment = StringAlignment.Center;
            pieFormat.Alignment = StringAlignment.Near;
            pieFormat.LineAlignment = StringAlignment.Center;

            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.Clear(Color.White);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int width = Size.Width / 2;
            Size chartSize = Size with {Width = width - MARGIN / 2};
            var chart1 = new Rectangle(new Point(0, 0), chartSize);
            var chart2 = new Rectangle(new Point(width + MARGIN / 2, 0), chartSize);
            switch (Type)
            {
                case ChartType.Bar:
                    DrawBarChart(e, chart1, true);
                    DrawBarChart(e, chart2, false);
                    break;
                case ChartType.LogBar:
                    DrawLogBarChart(e, chart1, true);
                    DrawLogBarChart(e, chart2, false);
                    break;
                case ChartType.Pie:
                    DrawPieChart(e, chart1, true);
                    DrawPieChart(e, chart2, false);
                    break;
            }

            base.OnPaint(e);
        }
        
        private void DrawBarChart(PaintEventArgs e, Rectangle drawArea, bool first)
        {
            Graphics g = e.Graphics;
            if (Stat == null || Stat.Counts.Count == 0)
                return;

            var data = first ? Stat.Counts : Stat.Sizes;

            float chartMargin = 75;

            float chartWidth = drawArea.Width - chartMargin;
            float chartHeight = drawArea.Height - chartMargin / 2;
            g.FillRectangle(Brushes.LightGray, drawArea.X + chartMargin, drawArea.Y, chartWidth, chartHeight);

            int barCount = Stat.Counts.Count;
            float barWidthMargin = chartWidth / barCount;
            float barMargin = barWidthMargin / 4;
            float barWidth = barWidthMargin - 2*barMargin;
            float maxBarHeight = 90*chartHeight/100;

            long maxValue = data.Max(x => x.Value);
            long maxLine = (long) ((double)maxValue).RoundUpToSignificantDigits(2);

            float scale = 1.0f * maxBarHeight / Math.Max(maxValue, maxLine);

            for (int line = 0; line <= 10; line++)
            {
                float lineHeight = scale * line * maxLine / 10;
                float lineY = drawArea.Y + chartHeight - lineHeight;
                g.DrawLine(Pens.Black, drawArea.X + chartMargin, lineY, drawArea.X + drawArea.Width, lineY);
                g.DrawString($"{(line * maxLine / 10).ToScientific(2)}", this.Font, Brushes.Black, new RectangleF(drawArea.X, lineY, chartMargin, 0), axisFormat);
            }

            int i = -1;
            foreach (var item in data)
            {
                float barHeight = item.Value * scale;
                using Brush brush = new SolidBrush(Utils.ColorFromHSV((double)++i / (double)barCount, 0.75, 1));
                float barX = drawArea.X + chartMargin + barMargin + i * barWidthMargin;
                RectangleF bar = new RectangleF(barX, drawArea.Y + chartHeight - barHeight, barWidth, barHeight);
                g.FillRectangle(brush, bar);
                g.DrawRectangle(Pens.Black, bar.X, bar.Y, bar.Width, bar.Height);
                g.DrawString(item.Key, Font, Brushes.Black, 
                    new RectangleF(barX - barMargin, chartHeight, barWidthMargin, Font.Height), labelFormat);
            }
        }

        private void DrawLogBarChart(PaintEventArgs e, Rectangle drawArea, bool first)
        {
            Graphics g = e.Graphics;
            if (Stat == null || Stat.Counts.Count == 0)
                return;

            var data = first ? Stat.Counts : Stat.Sizes;

            float chartMargin = 75;

            float chartWidth = drawArea.Width - chartMargin;
            float chartHeight = drawArea.Height - chartMargin / 2;
            g.FillRectangle(Brushes.LightGray, drawArea.X + chartMargin, drawArea.Y, chartWidth, chartHeight);

            int barCount = Stat.Counts.Count;
            float barWidthMargin = chartWidth / barCount;
            float barMargin = barWidthMargin / 4;
            float barWidth = barWidthMargin - 2 * barMargin;
            float maxBarHeight = 90 * chartHeight / 100;

            long maxValue = data.Max(x => x.Value);
            long maxLine = (long) ((double)maxValue).GetNextLogPoint();

            int end = (int)Math.Round(Math.Log10(maxLine) * 3, MidpointRounding.AwayFromZero) + 1;
            int start = Math.Max(0, end - 10);

            float scale = (float) (1.0 * maxBarHeight / (Math.Log10(Math.Max(maxValue, maxLine))*3 - start + 1));
            
            g.DrawLine(Pens.Black, drawArea.X + chartMargin, drawArea.Y + chartHeight, drawArea.X + drawArea.Width, drawArea.Y + chartHeight);
            g.DrawString($"0", this.Font, Brushes.Black, new RectangleF(drawArea.X, drawArea.Y + chartHeight, chartMargin, 0), axisFormat);


            for (int line = start; line < end; line++)
            {
                long value = (long) Math.Pow(10, line / 3.0).RoundToSignificantDigits(1);

                float lineHeight = (float)(scale * (Math.Log10(value)*3 - start + 1));
                float lineY = drawArea.Y + chartHeight - lineHeight;
                g.DrawLine(Pens.Black, drawArea.X + chartMargin, lineY, drawArea.X + drawArea.Width, lineY);
                g.DrawString($"{value.ToScientific(1)}", this.Font, Brushes.Black, new RectangleF(drawArea.X, lineY, chartMargin, 0), axisFormat);
            }

            int i = -1;
            foreach (var item in data)
            {
                float barHeight = (float) ((Math.Log10(item.Value)*3 -start + 1) * scale);
                using Brush brush = new SolidBrush(Utils.ColorFromHSV((double)++i / (double)barCount, 0.75, 1));
                float barX = drawArea.X + chartMargin + barMargin + i * barWidthMargin;
                RectangleF bar = new RectangleF(barX, drawArea.Y + chartHeight - barHeight, barWidth, barHeight);
                g.FillRectangle(brush, bar);
                g.DrawRectangle(Pens.Black, bar.X, bar.Y, bar.Width, bar.Height);
                g.DrawString(item.Key, Font, Brushes.Black,
                    new RectangleF(barX - barMargin, chartHeight, barWidthMargin, Font.Height), labelFormat);
            }
        }

        private void DrawPieChart(PaintEventArgs e, Rectangle drawArea, bool first)
        {
            Graphics g = e.Graphics;
            if (Stat == null || Stat.Counts.Count == 0)
                return;

            var data = first ? Stat.Counts : Stat.Sizes;

            float chartMargin = drawArea.Width / 3.0f;

            float chartWidth = drawArea.Width - chartMargin;
            float chartHeight = drawArea.Height;

            long pieTotal = data.Sum(x => x.Value);
            int pieCount = data.Count;
            float centerX = chartWidth / 2;
            float centerY = chartHeight / 2;
            float radius = Math.Min(chartWidth, chartHeight) / 2;
            float boxHeightMargin = chartHeight / pieCount;
            float boxMargin = boxHeightMargin / 4;
            float boxHeight = boxHeightMargin - 2 * boxMargin;
            float smallMargin = chartMargin / 8;
            float boxWidth = chartMargin / 3 - smallMargin;

            float startAngle = 0;
            int i = -1;
            foreach (var item in data)
            {
                float sweepAngle = item.Value * 360.0f / pieTotal;
                
                using Brush brush = new SolidBrush(Utils.ColorFromHSV((double)++i / (double)pieCount, 0.75, 1));
                g.FillPie(brush, drawArea.X + centerX - radius, drawArea.Y + centerY - radius, radius * 2, radius * 2, startAngle, sweepAngle);
                g.DrawPie(Pens.Black, drawArea.X + centerX - radius, drawArea.Y + centerY - radius, radius * 2, radius * 2, startAngle, sweepAngle);
                startAngle += sweepAngle;

                g.FillRectangle(brush, drawArea.X + chartWidth + smallMargin, drawArea.Y + boxMargin + i * boxHeightMargin, boxWidth, boxHeight);
                g.DrawRectangle(Pens.Black, drawArea.X + chartWidth + smallMargin, drawArea.Y + boxMargin + i * boxHeightMargin, boxWidth, boxHeight);

                g.DrawString($"{item.Key} - {(first ? item.Value.ToScientific(0) : Utils.FormatFileSize(item.Value))}", Font, Brushes.Black,
                    new RectangleF(drawArea.X + chartWidth + boxWidth + smallMargin + 20, drawArea.Y + boxMargin + i * boxHeightMargin, 2*boxWidth, boxHeight), pieFormat);
            }
        }
    }
}
