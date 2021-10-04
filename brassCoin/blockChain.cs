 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class blockChain
    {
        private List<block> chain;
        private List<transaction> currentTransactions;

        public blockChain(List<block> chainForSync)
        {
            chain = chainForSync;
            currentTransactions = new List<transaction>();
        }
        public blockChain()
        {
            chain = new List<block>();
            currentTransactions = new List<transaction>();
            // i guess we can put neon genesis evanglion in here


           proofOfWork tempPOW = new proofOfWork(42);
           List<transaction> tempLoT = new List<transaction>();
           transaction tempTrans = new transaction("genesis", "brassbinn", 0);
           tempLoT.Add(tempTrans);

           block genesisBlock = new block(0, 0, tempLoT, tempPOW, Sha256Hash.Of("placeholder"));

           chain.Add(genesisBlock);

        }

        public IReadOnlyCollection<block> Chain => chain.AsReadOnly();

        public block newBlock(proofOfWork nonce, Sha256Hash prevHash)
        {
            
            block tempBlock = new block(chain.Count, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), currentTransactions, nonce, prevHash);

            chain.Add(tempBlock);
            
            return tempBlock;
        }
        public void dropTrans()
        {
            currentTransactions.Clear();
        }
        public long newTransaction(string sender, string recipient, double amount)
        {
            transaction tempTransaction = new transaction(sender, recipient, amount);
            
            currentTransactions.Add(tempTransaction);

            return last_block().Index + 1;
                
        }
        public block last_block()
        {
            return chain.Last();
        }
    }
}
