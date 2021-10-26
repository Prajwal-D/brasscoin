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
            public string amount;
            public string signature;
        }
        public class nonceOrHash
        {
            public string value;
        }
        public class blockMadeOf
        {
            public string index;
            public string timestamp;
            public nonceOrHash nonce;
            public nonceOrHash prevHash;
            public IList<transMadeOf> transactions;
        }
        public IList<blockMadeOf> Chain { get; set; }
        public long Length {get; set;}
    }
}
