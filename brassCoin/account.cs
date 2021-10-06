using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace brassCoin
{
    public class account
    {
        private RSA theAccount;

        public account()
        {
            theAccount = RSA.Create();
        }
        public account(string XMLstring)
        {
            theAccount.FromXmlString(XMLstring);
        }
        public account(RSAParameters rsaParams)
        {
            theAccount.ImportParameters(rsaParams);
        }

        public string getAccountPubKey()
        {
            string base64OfRSA = Convert.ToBase64String(theAccount.ExportRSAPublicKey());

            return base64OfRSA;
        }
    }
}
