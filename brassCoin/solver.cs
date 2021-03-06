using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brassCoin
{
    public static class solver
    {
        public static proofOfWork Solve(block lastBlock, long nonceToStartFrom)
        {
            if (nonceToStartFrom < 0)
                nonceToStartFrom = 0;

            while(!new proofOfWork(nonceToStartFrom).verify(lastBlock))
            {
                nonceToStartFrom += 1;
            }

            proofOfWork powToReturn = new proofOfWork(nonceToStartFrom);
            return powToReturn;
        }
    }
}
