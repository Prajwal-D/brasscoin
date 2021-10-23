using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class chainAPI
    {
        public class transMadeOf
        {
            public string sender;
            public string recipient;
            public double amount;
            public string signature;
        }
        public class chainMadeOf
        {
            public long index;
            public long timestamp;
            public List<transMadeOf> transactions;
            public long nonce;
            public string prevHash;
        }
        public List<chainMadeOf> Chain { get; set; }
        public long Length {get; set;}
    }
}
