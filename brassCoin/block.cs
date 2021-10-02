using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        [JsonConverter(typeof(ObjectToPropertyConverter), typeof(proofOfWork), "Value")]
        public proofOfWork Nonce => nonce;

        [JsonConverter(typeof(ObjectToPropertyConverter), typeof(Sha256Hash), "Value")]
        public Sha256Hash PrevHash => prevHash;

        public IReadOnlyCollection<transaction> Transactions => transactions.AsReadOnly();
            

    }

}
