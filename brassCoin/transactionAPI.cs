using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class transactionAPI
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public double Amount { get; set; }
        public string Signature { get; set; }
    }
}
