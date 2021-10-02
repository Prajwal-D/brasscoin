using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class transaction
    {
        private string sender;
        private string recipient;
        private double amount;

        public transaction(string senderIn, string recipientIn, double amountIn)
        {
            sender = senderIn;
            recipient = recipientIn;
            amount = amountIn;

        }
        public string Sender => sender;
        public string Recipient => recipient;
        public double Amount => amount;
    }
}
