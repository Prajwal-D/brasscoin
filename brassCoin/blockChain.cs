using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class blockChain
    {
        public class transaction
        {
            public string sender;
            public string recipient;
            public double amount;

        }

        public class block
        {
            public string index;
            public double timestamp;
            public List<transaction> transactions = new List<transaction>();
            public int nonce;
            public string prev_hash;

        }

        public blockChain()
        {
            List<block> chain = new List<block>();
            List<transaction> currentTransactions = new List<transaction>();
        }
    }
}
