using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class node
    {
        private readonly Uri address;

        public node(Uri addressIn)
        {
            address = addressIn;
        }

        public node(string addressIn)
        {
            address = new Uri(addressIn);
        }

        public Uri Address => address;
    }
}
