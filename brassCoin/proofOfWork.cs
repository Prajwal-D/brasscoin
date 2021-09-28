﻿using System;
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

        //transaction problem solved hopefully
        public virtual bool verify(block proofToVerify)
        {
            string stringOfHashes = "";
            List<transaction> toHash = blockChain.getCurrentTransactions();
            if (toHash.Count > 0)
            {
                for (int i = 0; i <= toHash.Count - 1 ; i++)
                {
                    string stringToHash = $"{toHash[i].sender}{toHash[i].recipient}{toHash[i].amount}";

                    stringOfHashes = $"{stringOfHashes}{Sha256Hash.Of(stringToHash).ToString()}";
                    
                }
            }

            return Sha256Hash.Of($"{nonce}{stringOfHashes}{proofToVerify.timestamp}{proofToVerify.prevHash}").StartsWith("0000");
        }

    }
}
