using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
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
            block blockToJsonify = primaryBlockChain.newBlock(nonce, new proofOfWork(nonce.Value).getHashOf(primaryBlockChain.last_block()));

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
                privateKey = primaryBlockChain.getCurAccount().getAccountPrivKey(), //lol glaring security flaw is that if i expose endpoints to everyone then anyone can access this url and grab the key idk maybe i host a second thing??? fuck how do i fix this
                publicKey = primaryBlockChain.getCurAccount().getAccountPubKey()
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

        // GET blockchain/api/nodes/consensus
        [HttpGet("nodes/consensus")]
        public dynamic GetConsensus()
        {
            if (primaryBlockChain?.Nodes.Any() != true)
            {
                return NotFound(new
                {
                    message = $"No nodes found! Did you add any?"
                });
            }

            List<List<block>> chains = new List<List<block>>();
            foreach (var node in primaryBlockChain.Nodes)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = node.Address;
                        var responseTask = client.GetAsync("blockchain/api/chain");
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.StatusCode != HttpStatusCode.OK) //if fail to open webapi chain
                        {
                            throw new Exception();
                        }

                        var chainData = result.Content.ReadAsAsync<chainAPI>();
                        chainData.Wait();

                        var chainAndLen = chainData.Result;
                        chains.Add(chainAndLen.Chain.Select(b => block.recreate(
                            b.index,
                            b.timestamp,
                            b.transactions.Select(t => new transaction(t.sender, t.recipient, t.amount, t.signature)),
                            b.nonce,
                            b.prevHash
                        )).ToList());
                               
                    }
                }
                catch (Exception) { }//just to capture any failed connections 
            }

            Boolean chainReplaced = false;
            foreach (var chain in chains)
            {
                if (chain.Count < primaryBlockChain.getChainLen() && blockChain.validateNewChain(chain)) 
                {
                    primaryBlockChain.replaceChain(chain);
                    chainReplaced = true;
                }
            }

            if (chainReplaced)
            {
                return Ok(new
                {
                    message = "Our chain was replaced!"
                });
            }
            else
            {
                return Ok(new
                {
                    message = "Our chain remains... but for how long?"
                });
            }
         
        }

        // POST blockchain/api/transactions/new
        [HttpPost("transactions/new")]
        //HAVE TO ADD VALIDATION TO THIS
        public dynamic PostNewTrans([FromBody] transactionAPI value)
        {
            long indexToAddTo;
            try 
            { 
                if (value.Sender == primaryBlockChain.getCurAccount().getAccountPubKey() || value.Sender == "me")
                {
                    account userAccount = primaryBlockChain.getCurAccount();
                    indexToAddTo = primaryBlockChain.newTransaction(userAccount.getAccountPubKey(), value.Recipient, value.Amount, userAccount.sign($"{userAccount.getAccountPubKey()}{value.Recipient}{value.Amount}"));
                }
                else
                {
                    indexToAddTo = primaryBlockChain.newTransaction(value.Sender, value.Recipient, value.Amount, value.Signature);
                }
            }
            catch(Exception ex) { 
                return BadRequest(new 
                {
                    message = ex 
                }); 
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
