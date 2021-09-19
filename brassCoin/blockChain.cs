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

        public class transaction
        {
            public string sender;
            public string recipient;
            public double amount;

        }

        public class block
        {
            public int index;
            public double timestamp;
            public List<transaction> transactions;
            public int nonce;
            public Sha256Hash prev_hash;
            public string genesis;

        }

        public blockChain()
        {
            List<block> chain = new List<block>();
            List<transaction> currentTransactions = new List<transaction>();
            // i guess we can put neon genesis evanglion in here
            
            bool isEmpty = !chain.Any();
            if(isEmpty)
                {
                block genesisBlock = new block();
                genesisBlock.index = 0;
                genesisBlock.timestamp = 0;
                genesisBlock.transactions = null;
                genesisBlock.nonce = 42;
                genesisBlock.prev_hash = Sha256Hash.Of("placeholder");
                genesisBlock.genesis = "placeholder";
                chain.Add(genesisBlock);
                }
        }

        public static int New_transaction(string sender, string recipient, double amount)
        {
            transaction tempTransaction = new transaction();
            
            tempTransaction.sender = sender;
            tempTransaction.recipient = recipient;
            tempTransaction.amount = amount;

            currentTransactions.Add(tempTransaction);

            return last_block().index;
                
        }
        public static block last_block()
        {
            return chain[-1];
        }
    }
}
