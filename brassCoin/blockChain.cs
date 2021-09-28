 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class blockChain
    {
        private static List<block> chain;
        private static List<transaction> currentTransactions;

        public blockChain(List<block> chainForSync)
        {
            if (!(chainForSync?.Any() ?? false))
            {
                List<block> chain = new List<block>();
            }
            else
            {
                List<block> chain = chainForSync;
            }

            
            List<transaction> currentTransactions = new List<transaction>();
            // i guess we can put neon genesis evanglion in here
            
            bool isEmpty = !chain.Any();
            if(isEmpty)
                {
                proofOfWork tempPOW = new proofOfWork(42);
                block genesisBlock = new block(0, 0, null, tempPOW, Sha256Hash.Of("placeholder"));

                chain.Add(genesisBlock);
                }
        }

        public static List<transaction> getCurrentTransactions()
        {
            return currentTransactions;
        }

        public static block newBlock(proofOfWork nonce, Sha256Hash prevHash)
        {
            block tempBlock = new block(chain.Count, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), currentTransactions, nonce, prevHash);

            currentTransactions.Clear();

            chain.Add(tempBlock);

            return tempBlock;
        }
        public static long newTransaction(string sender, string recipient, double amount)
        {
            transaction tempTransaction = new transaction(sender, recipient, amount);
            
            currentTransactions.Add(tempTransaction);

            return last_block().index + 1;
                
        }
        public static block last_block()
        {
            return chain[-1];
        }
    }
}
