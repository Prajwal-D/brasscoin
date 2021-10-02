using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class proofOfWork
    {
        private long nonce;

        public proofOfWork(long nonceIn)
        {
            nonce = nonceIn;

        }

        public long Nonce => nonce;

        //transaction problem solved hopefully
        public virtual bool verify(block proofToVerify)
        {
            string stringOfHashes = "";
            List<transaction> toHash = (List<transaction>)proofToVerify.Transactions;
            if (toHash.Count > 0)
            {
                for (int i = 0; i <= toHash.Count - 1 ; i++)
                {
                    string stringToHash = $"{toHash[i].Sender}{toHash[i].Recipient}{toHash[i].Amount}";

                    stringOfHashes = $"{stringOfHashes}{Sha256Hash.Of(stringToHash).ToString()}";
                    
                }
            }

            string hashOfBlock = Sha256Hash.Of($"{stringOfHashes}{proofToVerify.Timestamp}{proofToVerify.PrevHash}").ToString();
            return Sha256Hash.Of($"{nonce}{hashOfBlock}").StartsWith("0000");
        }

    }
}
