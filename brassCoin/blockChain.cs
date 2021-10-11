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
        private account userAccount;
        public blockChain(List<block> chainForSync)
        {
            chain = chainForSync;
            currentTransactions = new List<transaction>();
            userAccount = new account();
        }

        public void genesis()
        {
            //neon genesis evanglion moved to here
            proofOfWork tempPOW = new proofOfWork(42);
            List<transaction> tempLoT = new List<transaction>();
            transaction tempTrans = new transaction(userAccount.getAccountPubKey(), "genesis", 0, userAccount.sign($"{userAccount.getAccountPubKey()}genesis0"));
            tempLoT.Add(tempTrans);

            block genesisBlock = new block(0, 0, tempLoT, tempPOW, Sha256Hash.Of("placeholder"));

            chain.Add(genesisBlock);
        }
        public blockChain()
        {
            chain = new List<block>();
            currentTransactions = new List<transaction>();
            userAccount = new account();

            genesis();

        }
        public Boolean changeAccount(string base64String)
        {
            //ok ensuring account imported has priv key
            account tempAccount = new account(base64String, true);

            try
            {
                string tempString = tempAccount.getAccountPrivKey();
            }
            catch (Exception)
            {
                return false;
                
            }

            userAccount = new account(base64String, true);
            genesis();
            return true;

        }

        public account getCurAccount()
        {
            return userAccount;
        }

        public IReadOnlyCollection<block> Chain => chain.AsReadOnly();

        public block newBlock(proofOfWork nonce, Sha256Hash prevHash)
        {
            List<transaction> tempLoT = new List<transaction>(currentTransactions);
            block tempBlock = new block(chain.Count, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), tempLoT, nonce, prevHash);

            chain.Add(tempBlock);
            
            return tempBlock;
        }
        public void dropTrans()
        {
            currentTransactions.Clear();
        }
        public long newTransaction(string sender, string recipient, double amount, string signature)
        {
            transaction tempTransaction = new transaction(sender, recipient, amount, signature);
            
            currentTransactions.Add(tempTransaction);

            return last_block().Index + 1;
                
        }
        public block last_block()
        {
            return chain.Last();
        }
    }
}
