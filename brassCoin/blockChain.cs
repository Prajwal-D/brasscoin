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
        private List<node> nodes;

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
            try
            {
                //ok ensuring account imported is valid and has priv key
                userAccount = new account(base64String, true);
            }
            catch (Exception)
            {
                return false;    
            }

            chain.Clear();
            genesis();
            return true;
        }

        public account getCurAccount()
        {
            return userAccount;
        }

        public IReadOnlyCollection<transaction> CurrentTransactions => currentTransactions.AsReadOnly();
        public IReadOnlyCollection<block> Chain => chain.AsReadOnly();

        public block newBlock(proofOfWork nonce, Sha256Hash prevHash)
        {
            List<transaction> tempLoT = new List<transaction>(currentTransactions);
            block tempBlock = new block(chain.Count, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), tempLoT, nonce, prevHash);

            chain.Add(tempBlock);
            
            return tempBlock;
        }
        //lol this function is redundant but i used it while figuring out an annoying bug and i would like to keep it as a memento
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
        public void registerNode(string uri)
        {
            node nodeToAdd = new node(uri);
            if(!nodes.Contains(nodeToAdd))
            {
                nodes.Add(nodeToAdd);
            }
        }

        public Boolean validateNewChain(List<block> chainToCompare)
        {
            int currentBlock = 1;
            block lastBlock = chainToCompare[0];

            while (currentBlock < chainToCompare.Count)
            {
                //validating that the nonce in the block currently being compared and the previous block hashed is equal to the hash in this block
                block blockToCompare = chainToCompare[currentBlock];
                proofOfWork nonceToConfirm = blockToCompare.Nonce;

                if (!(blockToCompare.PrevHash == nonceToConfirm.getHashOf(lastBlock)))
                    return false;

                //validating all the transactions in this block
                List<transaction> transactions = blockToCompare.getListOfTrans();
                Boolean miningRewardRecievied = false;
                int currentTrans = 0;

                transaction transToVerify = transactions[currentTrans];
                string stringToVerify = $"{transToVerify.Sender}{transToVerify.Recipient}{transToVerify.Amount}";
                while (currentTrans < transactions.Count)
                {
                    //ignore reward for mining a block
                    if (transactions[currentTrans].Sender == "0" && !miningRewardRecievied)
                    {
                        miningRewardRecievied = true;
                        currentTrans += 1;
                    }
                    //ensuring no extra rewards are allowed
                    else if (transactions[currentTrans].Sender == "0" && miningRewardRecievied)
                        return false;
                    //for making sure no impersonation can occur
                    else if (!new account(transToVerify.Sender, false).verify(stringToVerify, transToVerify.Signature))
                        return false;
                    //progress to next transaction
                    else
                        currentTrans += 1;
                }
            }
            return true;
        }
        public block last_block()
        {
            return chain.Last();
        }
    }
}
