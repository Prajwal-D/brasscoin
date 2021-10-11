using System;
using System.Text;
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
        public account(string base64RSA, bool priv)
        {
            byte[] RSAkey = Convert.FromBase64String(base64RSA);
            int temp;

            if (priv)
            {
                theAccount.ImportRSAPrivateKey(RSAkey, out temp);
            }
            else
            {
                theAccount.ImportRSAPublicKey(RSAkey, out temp);
            }
        }
        public string getAccountPubKey()
        {
            string base64OfRSA = Convert.ToBase64String(theAccount.ExportRSAPublicKey());

            return base64OfRSA;
        }

        public string getAccountPrivKey()
        {
            string base64OfRSA = Convert.ToBase64String(theAccount.ExportRSAPrivateKey());

            return base64OfRSA;
        }

        public string sign(string stringToSign)
        {
            byte[] bytesOfString = Encoding.ASCII.GetBytes(stringToSign);
            byte[] signedBytes = theAccount.SignData(bytesOfString, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signedBytes);
        }
        public Boolean verify(string stringToVerify, string signature)
        {
            byte[] bytesOfString = Encoding.ASCII.GetBytes(stringToVerify);
            byte[] bytesOfSig = Convert.FromBase64String(signature);
            return theAccount.VerifyData(bytesOfString, bytesOfSig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
