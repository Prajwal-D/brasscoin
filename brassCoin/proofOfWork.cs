using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class proofOfWork
    {
        private readonly Double nonce;

        public proofOfWork(double proof)
        {
            if (proof <= 0)
                throw new ArgumentException("Can't have negative proof");

            nonce = proof;

        }

        public virtual bool verify(block proofToVerify)
        {
            if (proofToVerify == null)
                throw new InvalidOperationException("block cannot be empty");
            // OHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH it takes the last nonce and nonces it with the new nonce
            // cringe im gonna make it nonce EVERYTHING
            // because how else do you ensure uhhhhhhh something something timestamp i don't remember but i've thought about this

            return Sha256Hash.Of($"{proofToVerify.everythingConcatnated}{nonce}").StartsWith("0000");
        }

    }
}
