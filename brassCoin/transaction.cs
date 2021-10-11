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
        private string signature;

        public transaction(string senderIn, string recipientIn, double amountIn, string signatureIn)
        {
            sender = senderIn;
            recipient = recipientIn;
            amount = amountIn;
            signature = signatureIn;
        }
        public string Sender => sender;
        public string Recipient => recipient;
        public double Amount => amount;
        public string Signature => signature;
    }
}
