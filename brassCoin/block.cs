using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class block
    {
        public int index;
        public double timestamp;
        public List<transaction> transactions;
        public int nonce;
        public Sha256Hash prev_hash;
        public string everythingConcatnated;
    }
}
