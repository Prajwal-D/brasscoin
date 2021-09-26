using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public class solver
    {
        public proofOfWork Solve(block lastBlock)
        {
            long nonceToTest = 0;
            while(!new proofOfWork(nonceToTest).verify(lastBlock)) // for now it shall go on forever until it finds the nonce
            {
                nonceToTest += 1;
            }

            proofOfWork powToReturn = new proofOfWork(nonceToTest);
            return powToReturn;
        }
    }
}
