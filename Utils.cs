using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DiskSpaceAnalyzer
{
    public static class Utils
    {
        // https://stackoverflow.com/a/62698159/21552398
        public static string FormatFileSize(long bytes)
        {
            var unit = 1024;
            if (bytes < unit) { return $"{bytes} B"; }

            var exp = (int)(Math.Log(bytes) / Math.Log(unit));
            return $"{bytes / Math.Pow(unit, exp):F1} {("KMGTPE")[exp - 1]}B";
        }

        // https://stackoverflow.com/questions/57390865/how-can-i-get-all-files-on-disk-with-a-specific-extension-using-directory-getfi
        public static IEnumerable<string> GetFiles(string root, string spec)
        {
            var pending = new Stack<string>(new[] { root });

            while (pending.Count > 0)
            {
                var path = pending.Pop();
                IEnumerator<string> fileIterator = null;

                try
                {
                    fileIterator = Directory.EnumerateFiles(path, spec).GetEnumerator();
                }
                catch (UnauthorizedAccessException) { }

                if (fileIterator != null)
                {
                    using (fileIterator)
                    {
                        while (true)
                        {
                            try
                            {
                                if (!fileIterator.MoveNext()) // Throws if file is not accessible.
                                    break;
                            }

                            catch { break; }

                            yield return fileIterator.Current;
                        }
                    }
                }

                IEnumerator<string> dirIterator = null;

                try
                {
                    dirIterator = Directory.EnumerateDirectories(path).GetEnumerator();
                }

                catch { }

                if (dirIterator != null)
                {
                    using (dirIterator)
                    {
                        while (true)
                        {
                            try
                            {
                                if (!dirIterator.MoveNext()) // Throws if directory is not accessible.
                                    break;
                            }

                            catch { break; }

                            pending.Push(dirIterator.Current);
                        }
                    }
                }
            }
        }

        // https://stackoverflow.com/questions/374316/round-a-double-to-x-significant-figures
        public static double RoundToSignificantDigits(this double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);
        }

        // https://stackoverflow.com/questions/7075201/rounding-up-to-2-decimal-places-in-c-sharp
        public static double RoundUpToSignificantDigits(this double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1 - digits);
            return scale * Math.Ceiling(d / scale);
        }

        // https://stackoverflow.com/questions/1335426/is-there-a-built-in-c-net-system-api-for-hsv-to-rgb
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue * 6)) % 6;
            double f = hue * 6 - Math.Floor(hue * 6);

            value *= 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public static string ToScientific(this long d, int digits)
        {
            if (d < 1e6) return d.ToString();

            int exponent = (int) Math.Floor(Math.Log10(Math.Abs(d)));
            double scale = Math.Pow(10, exponent);
            double mantissa = d / scale;
            return $"{mantissa.ToString($"F{digits}")}*10^{exponent}";
        }

        public static double GetNextLogPoint(this double d)
        {
            double log = Math.Log10(d);
            //if (log >= 8.0) 
            //    return  Math.Pow(10, Math.Ceiling(log));

            double min = Math.Pow(10, Math.Floor(log));
            if (d <=  min)
                return  min;
            else if (d <= min * 2)
                return min * 2;
            else if (d <= min * 5)
                return min * 5;
            else
                return min * 10;
        }
    }
}
