using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskSpaceAnalyzer
{
    internal class DefaultDict<TKey, TValue> : Dictionary<TKey, TValue> where TValue : new() where TKey : notnull
    {
        public new TValue? this[TKey key]
        {
            get
            {
                TValue? val;
                if (!TryGetValue(key, out val))
                {
                    val = new TValue();
                    Add(key, val);
                }

                return val;
            }
            set => base[key] = value!;
        }
    }
}
