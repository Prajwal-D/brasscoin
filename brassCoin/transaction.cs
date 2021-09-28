using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class transaction
    {
        public string sender;
        public string recipient;
        public double amount;

        public transaction(string senderIn, string recipientIn, double amountIn)
        {
            sender = senderIn;
            recipient = recipientIn;
            amount = amountIn;

        }
    }
}
