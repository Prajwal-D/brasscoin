using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class proofOfWork
    {
        public long nonce;

        public proofOfWork(long nonceIn)
        {
            nonce = nonceIn;

        }

        //so the proofer rn essentially takes the timestamp of the last block created, the hash of the block before that, and the nonce.
        //however, this can lead to the transactions in the block being removed and the block keeping the same hash, which is a HUGE secuurity flaw that i will fix in the next git commit
        //for now, bear with me
        public virtual bool verify(block proofToVerify)
        {
            return Sha256Hash.Of($"{nonce}{proofToVerify.timestamp}{proofToVerify.prevHash}").StartsWith("0000");
        }

    }
}
