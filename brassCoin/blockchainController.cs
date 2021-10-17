using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


//NEEED TO ADD VALIDATION OH GOD
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
        public OkObjectResult createNewBlock(proofOfWork nonce)
        {
            primaryBlockChain.newTransaction("0", primaryBlockChain.getCurAccount().getAccountPubKey(), 1, "ignorable signature"); //REMEMEBER IN VERIFICATION ONLY ALLOW ONE OF THESE TRANSACTIONS U CAN'T HAVE 200 REWARDS FOR ONE BLOCK MINED

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

        // GET blockchain/api/mine
        [HttpGet("mine")]
        public dynamic GetMining()
        {
            proofOfWork nonce = solver.Solve(primaryBlockChain.last_block(), 0);

            return createNewBlock(nonce);

        }

        // GET blockchain/api/mine/{valuetostartfrom}
        [HttpGet("mine/{startPoint}")]
        public dynamic GetMiningWithPosition(int startPoint)
        {
            proofOfWork nonce = solver.Solve(primaryBlockChain.last_block(), startPoint);

            return createNewBlock(nonce);
        }

        // GET blockchain/api/transactions
        [HttpGet("transactions")]
        public dynamic GetCurrentTransactions()
        {
            return new
            {
                transactions = primaryBlockChain.CurrentTransactions
            };
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

        // GET blockchain/api/account
        [HttpGet("account")]
        public dynamic GetAccount()
        {
            return new
            {
                account = primaryBlockChain.getCurAccount().getAccountPrivKey()
            };
        }

        // POST blockchain/api/account/set
        [HttpPost("account/set")]
        public dynamic PostSetAccount([FromBody] accountAPI base64OfprivKey)
        {
            Boolean success = primaryBlockChain.changeAccount(base64OfprivKey.Account);

            if (success)
            {
                return Ok(new
                {
                    message = $"Account changed successfully! Chain refreshed!"
                });
            }
            else
            {
                return BadRequest(new
                {
                    message = $"Error! Invalid Key!"
                });
            }

        }
        // POST blockchain/api/nodes/register
        [HttpPost("nodes/register")]
        public dynamic PostNewNode([FromBody] nodeAPI value)
        {
            if (value.Address == null || !Uri.TryCreate(value.Address, UriKind.Absolute, out var uri))
            {
                return BadRequest(new
                {
                    message = $"Invalid url!"
                });
            }

            primaryBlockChain.registerNode(value.Address);

            return Ok(new
            {
                message = $"New node registered at {value.Address}!"
            });

        }

        // GET blockchain/api/nodes/resolveconflicts

       
        // POST blockchain/api/transactions/new
        [HttpPost("transactions/new")]
        public dynamic PostNewTrans([FromBody] transactionAPI value)
        {
            long indexToAddTo;
            if (value.Sender == primaryBlockChain.getCurAccount().getAccountPubKey() || value.Sender == "me")
            {
                account userAccount = primaryBlockChain.getCurAccount();
                indexToAddTo = primaryBlockChain.newTransaction(userAccount.getAccountPubKey(), value.Recipient, value.Amount, userAccount.sign($"{userAccount.getAccountPubKey()}{value.Recipient}{value.Amount}"));
            }
            else
            {
                indexToAddTo = primaryBlockChain.newTransaction(value.Sender, value.Recipient, value.Amount, value.Signature);
            }
            

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
