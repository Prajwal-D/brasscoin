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
        private Dictionary<string, double> ledger;

        //function made redundant because i decided to make genesis block consistent
        public void genesis()
        {
            //neon genesis evanglion moved to here
            proofOfWork tempPOW = new proofOfWork(42);
            List<transaction> tempLoT = new List<transaction>();
            transaction tempTrans = new transaction("brassbinn", "genesis", 0, "I've been resting in the basement. For about 495 years.");
            tempLoT.Add(tempTrans);

            block genesisBlock = new block(0, 0, tempLoT, tempPOW, Sha256Hash.Of("something pretentious"));

            chain.Add(genesisBlock);
        }
        public blockChain()
        {
            chain = new List<block>();
            currentTransactions = new List<transaction>();
            nodes = new List<node>();
            userAccount = new account();
            ledger = new Dictionary<string, double>();

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
            return true;
        }
        public void changeLedger(List<transaction> transactionsToUse)
        {
            //this method assumes that values sent to it are sanitised
            foreach (var trans in last_block().getListOfTrans())
            {
                if (!(trans.Recipient == "genesis"))
                {
                    try
                    {
                        ledger.Add(trans.Recipient, trans.Amount);
                    }
                    catch (Exception)
                    {
                        ledger[trans.Recipient] = ledger[trans.Recipient] + trans.Amount;
                    }
                }

                if (!(trans.Sender == "0") && !(trans.Sender == "brassbinn"))
                {
                    ledger[trans.Sender] = ledger[trans.Sender] - trans.Amount;
                }
            }
        }
        public void replaceChain(List<block> chainForReplacement)
        {
            chain = new List<block>(chainForReplacement);
            ledger.Clear();
            foreach(var block in chain)
            {
                changeLedger(block.getListOfTrans());
            }
        }
        public account getCurAccount()
        {
            return userAccount;
        }

        public IReadOnlyCollection<transaction> CurrentTransactions => currentTransactions.AsReadOnly();
        public IReadOnlyCollection<block> Chain => chain.AsReadOnly();
        public IReadOnlyCollection<node> Nodes => nodes.AsReadOnly();

        //this method assumes a block has been mined if ever called
        public block newBlock(proofOfWork nonce, Sha256Hash prevHash)
        {
            //adds values in block that was just mined to ledger
            changeLedger(last_block().getListOfTrans());

            transaction tempTrans = new transaction("0", userAccount.getAccountPubKey(), 1, "he mined this wowowwowo");

            currentTransactions.Add(tempTrans);

            List<transaction> tempLoT = new List<transaction>(currentTransactions);
            block tempBlock = new block(chain.Count, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), tempLoT, nonce, prevHash);

            chain.Add(tempBlock);

            currentTransactions.Clear();
            
            return tempBlock;
        }

        //lol this function is redundant but i used it while figuring out an annoying bug and i would like to keep it as a memento
        //actually nvm could probably assign this to an endpoint to allow dropping transactions
        public void dropTrans()
        {
            currentTransactions.Clear();
        }
        public long newTransaction(string sender, string recipient, double amount, string signature)
        {
            //ensuring sender and recipient are valid keys
            try
            {
                new account(sender, false);
                new account(recipient, false);
            }
            catch (Exception)
            {
                throw new Exception("Invalid sender/recipient!");
            }

            //ensuring signature is valid to prevent impersonation
            if(!new account(sender, false).verify($"{sender}{recipient}{amount}",signature))
                throw new Exception("Invalid signature! Transaction cannot be verified!");


            //ensuring enough money in wallet
            if (!(ledger.TryGetValue(sender, out double value) && ledger[sender] > amount))
                throw new Exception("Not enough money in wallet!");             

            transaction tempTrans = new transaction(sender, recipient, amount, signature);
            currentTransactions.Add(tempTrans);
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
        public static Boolean validateNewChain(List<block> chainToCompare)
        {
            //genesis block is ignored
            int currentBlock = 1;

            //creating ongoing tempLedger for easy verification
            Dictionary<string, double> tempLedger = new Dictionary<string, double>();

            while (currentBlock < chainToCompare.Count)
            {
                //validating that the nonce in the block currently being compared and the previous block hashed is equal to the hash in this block
                block blockToCompare = chainToCompare[currentBlock];
                block lastBlock = chainToCompare[currentBlock - 1];
                proofOfWork nonceToConfirm = blockToCompare.Nonce;


                Sha256Hash hash = nonceToConfirm.getHashOf(lastBlock);
                if (!(blockToCompare.PrevHash.Value == nonceToConfirm.getHashOf(lastBlock).Value))
                    return false;

                //validating all the transactions in this block
                List<transaction> transactions = blockToCompare.getListOfTrans();
                Boolean miningRewardRecievied = false;
                int currentTrans = 0;

                while (currentTrans < transactions.Count)
                {

                    transaction transToVerify = transactions[currentTrans];
                    string stringToVerify = $"{transToVerify.Sender}{transToVerify.Recipient}{transToVerify.Amount}";

                    //ensuring sender and recipient are valid keys
                    if (!(transactions[currentTrans].Sender == "0"))
                    {
                        try
                        {
                            new account(transToVerify.Sender, false);
                            new account(transToVerify.Recipient, false);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                        //for making sure no impersonation can occur
                        if (!new account(transToVerify.Sender, false).verify(stringToVerify, transToVerify.Signature))
                            return false;

                        //the sender must logically be in the tempLedger and have balance if they are to send any money
                        if (tempLedger.TryGetValue(transToVerify.Sender, out double value) && tempLedger[transToVerify.Sender] > transToVerify.Amount)
                        {
                            //transaction goes through
                            tempLedger[transToVerify.Sender] = value - transToVerify.Amount;
                        }
                        else
                            return false;
                    }
                    //ignore reward for mining a block
                    if (transactions[currentTrans].Sender == "0" && !miningRewardRecievied)
                    {
                        miningRewardRecievied = true;
                        currentTrans += 1;
                    }
                    //ensuring no extra rewards are allowed
                    else if (transactions[currentTrans].Sender == "0" && miningRewardRecievied)
                        return false;

                    //tempLedger has to be changed after all validation is done
                    try
                    {
                        tempLedger.Add(transToVerify.Recipient, transToVerify.Amount);
                    }
                    catch (ArgumentException)
                    {
                        tempLedger[transToVerify.Recipient] = tempLedger[transToVerify.Recipient] + transToVerify.Amount;
                    }

                    currentTrans += 1;
                }
                currentBlock += 1;
            }
            return true;
        }
        public long getChainLen()
        {
            return chain.Count();
        }
        public block last_block()
        {
            return chain.Last();
        }
    }
}
