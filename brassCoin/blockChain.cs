﻿ using System;
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

        public static long New_transaction(string sender, string recipient, double amount)
        {
            transaction tempTransaction = new transaction();
            
            tempTransaction.sender = sender;
            tempTransaction.recipient = recipient;
            tempTransaction.amount = amount;

            currentTransactions.Add(tempTransaction);

            return last_block().index + 1;
                
        }
        public static block last_block()
        {
            return chain[-1];
        }
    }
}
