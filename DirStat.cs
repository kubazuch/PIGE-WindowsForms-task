using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskSpaceAnalyzer
{
    public class DirStatDict : Dictionary<string, DirStat>
    {
        public new DirStat this[string key]
        {
            get
            {
                DirStat? val;
                if (!TryGetValue(key, out val))
                {
                    val = new DirStat(key);
                    Add(key, val);
                }

                return val;
            }
            set => base[key] = value;
        }
    }

    public class DirStat
    {
        private readonly DefaultDict<string, long> _counts;
        private readonly DefaultDict<string, long> _sizes;

        public DirStat(string dir, bool isFile = false)
        {
            Path = dir;
            _counts = new DefaultDict<string, long>();
            _sizes = new DefaultDict<string, long>();
            IsFile = isFile;
        }

        public void Add(string file)
        {
            FileInfo fi = new FileInfo(file);
            _counts[fi.Extension]++;
            _sizes[fi.Extension] += fi.Length;
        }

        public bool IsFile { get; }

        public string Path { get; }

        public long Size => IsFile ? new FileInfo(Path).Length : _sizes.Select(x => x.Value).Sum();

        public Dictionary<string, long> Counts
        {
            get
            {
                if (_counts.Count <= 10) 
                    return new Dictionary<string, long>(_counts.OrderByDescending(x => x.Value));

                var top10 = _counts.OrderByDescending(x => x.Value).Take(9);
                long sumOthers = _counts.Values.Except(top10.Select(x => x.Value)).Sum();
                var others = new KeyValuePair<string, long>("Others", sumOthers);
                return new Dictionary<string, long>(top10.Concat(new[] { others }));
            }
        }
        
        public Dictionary<string, long> Sizes
        {
            get
            {
                if (_sizes.Count <= 10)
                    return new Dictionary<string, long>(_sizes.OrderByDescending(x => x.Value));

                var top10 = _sizes.OrderByDescending(x => x.Value).Take(9);
                long sumOthers = _sizes.Values.Except(top10.Select(x => x.Value)).Sum();
                var others = new KeyValuePair<string, long>("Others", sumOthers);
                return new Dictionary<string, long>(top10.Concat(new[] { others }));
            }
        }

        public int Files
        {
            get
            {
                if (IsFile) return 1;

                try
                {
                    return Directory.GetFiles(Path).Length;
                }
                catch (UnauthorizedAccessException)
                {
                    return 0;
                }
            }
        }

        public int Subdirs
        {
            get
            {
                if (IsFile) return 0;

                try
                {
                    return Directory.GetDirectories(Path).Length;
                }
                catch (UnauthorizedAccessException)
                {
                    return 0;
                }
            }
        }

        public DateTime LastChange => File.GetLastWriteTime(Path);
    }
}
