using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace brassCoin
{
    [Route("[controller]/api")]
    [ApiController]
    public class blockchainController : ControllerBase
    {
        //variables that need to be defined

        private readonly blockChain primaryBlockChain;
        public blockchainController(blockChain blockChain)
        {
            primaryBlockChain = blockChain;
        }

        //idk maybe this should be within the blockchain?? might be useful in there idk
        public OkObjectResult Mine(proofOfWork nonce)
        {
            primaryBlockChain.newTransaction("0", "thisAccount", 1); //have to implement accounts and public/private key encryption

            block blockToJsonify = primaryBlockChain.newBlock(nonce, new proofOfWork(nonce.Value).getHashOf(primaryBlockChain.last_block()));

            primaryBlockChain.dropTrans();

            return Ok(new
            {
                message = "Block forged",
                index = blockToJsonify.Index,
                timestamp = blockToJsonify.Timestamp,
                transactions = blockToJsonify.Transactions,
                nonce = blockToJsonify.Nonce,
                prevHash = blockToJsonify.PrevHash
            });
        }

        //the mapped endpoints

        // GET: blockchain/api/mine
        [HttpGet("mine")]
        public dynamic GetMining()
        {
            proofOfWork nonce = solver.Solve(primaryBlockChain.last_block(), 0);

            return Mine(nonce);
            
        }

        // GET: blockchain/api/mine/{valuetostartfrom}
        [HttpGet("mine/{startPoint}")]
        public dynamic GetMiningWithPosition(int startPoint)
        {
            proofOfWork nonce = solver.Solve(primaryBlockChain.last_block(), startPoint);

            return Mine(nonce);
        }

        // GET blockchain/api/chain
        [HttpGet("chain")]
        public dynamic GetChain()
        {
            return new
            {
                chain = primaryBlockChain.Chain, 
                length = primaryBlockChain.Chain.Count
            };
        }

        // POST blockchain/api/transactions/new
        [HttpPost("transactions/new")]
        public dynamic PostNewTrans([FromBody] transactionAPI value)
        {
            long indexToAddTo = primaryBlockChain.newTransaction(value.Sender, value.Recipient, value.Amount);

            return Ok(new
            {
                message = $"Transaction will be added to block {indexToAddTo}"
            });
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
