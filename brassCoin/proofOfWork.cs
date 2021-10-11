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

        //transaction problem solved hopefully
        public long Value => nonce;
        public virtual Sha256Hash getHashOf(block proofToVerify)
        {
            string stringOfHashes = "";
            List<transaction> toHash = proofToVerify.getListOfTrans();
            if (toHash.Count > 0)
            {
                for (int i = 0; i <= toHash.Count - 1; i++)
                {
                    string stringToHash = $"{toHash[i].Sender}{toHash[i].Recipient}{toHash[i].Amount}{toHash[i].Signature}";

                    stringOfHashes = $"{stringOfHashes}{Sha256Hash.Of(stringToHash).ToString()}";

                }
            }

            string hashOfBlock = Sha256Hash.Of($"{stringOfHashes}{proofToVerify.Index}{proofToVerify.Nonce}{proofToVerify.Timestamp}{proofToVerify.PrevHash}").ToString();
            return Sha256Hash.Of($"{nonce}{hashOfBlock}");
        }

        public virtual bool verify(block proofToVerify)
        {
            Sha256Hash hashOfBlock = getHashOf(proofToVerify);
            return hashOfBlock.StartsWith("0000");
        }
        
    }
}
