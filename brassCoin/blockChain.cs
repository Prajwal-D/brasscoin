 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class blockChain
    {
        public List<block> chain;
        public List<transaction> currentTransactions;

        public blockChain(List<block> chainForSync)
        {
            chain = chainForSync;
        }
        public blockChain()
        {
            chain = new List<block>();

            List<transaction> currentTransactions = new List<transaction>();
            // i guess we can put neon genesis evanglion in here

            for (int i = 0; i < 6; i++)
            {
                proofOfWork tempPOW = new proofOfWork(42);
                block genesisBlock = new block(0, 0, null, tempPOW, Sha256Hash.Of("placeholder"));

                chain.Add(genesisBlock);
            }
        }

        public List<transaction> getCurrentTransactions()
        {
            return currentTransactions;
        }

        public block newBlock(proofOfWork nonce, Sha256Hash prevHash)
        {
            block tempBlock = new block(chain.Count, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), currentTransactions, nonce, prevHash);

            currentTransactions.Clear();

            chain.Add(tempBlock);

            return tempBlock;
        }
        public long newTransaction(string sender, string recipient, double amount)
        {
            transaction tempTransaction = new transaction(sender, recipient, amount);
            
            currentTransactions.Add(tempTransaction);

            return last_block().index + 1;
                
        }
        public block last_block()
        {
            return chain[-1];
        }
    }
}
