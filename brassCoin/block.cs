using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class block
    {
        private long index;
        private long timestamp;
        private List<transaction> transactions;
        private proofOfWork nonce;
        private Sha256Hash prevHash;  

        public block(long indexIn, long timestampIn, List<transaction> transactionsIn, proofOfWork nonceIn, Sha256Hash prevHashIn)
        {
            index = indexIn;
            timestamp = timestampIn;
            transactions = transactionsIn;
            nonce = nonceIn;
            prevHash = prevHashIn;
        }
        public long Index => index;
        public long Timestamp => timestamp;
    }

}
