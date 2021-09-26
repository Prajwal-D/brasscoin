using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class block
    {
        public long index;
        public long timestamp;
        public List<transaction> transactions;
        public proofOfWork nonce;
        public Sha256Hash prevHash;  

        public block(long indexIn, long timestampIn, List<transaction> transactionsIn, proofOfWork nonceIn, Sha256Hash prevHashIn)
        {
            index = indexIn;
            timestamp = timestampIn;
            transactions = transactionsIn;
            nonce = nonceIn;
            prevHash = prevHashIn;
        }
    }

}
