using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace brassCoin
    //ensure only localuser can access certain webpages
{    public class RestrictToLocalhostAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            if (!IPAddress.IsLoopback(remoteIp))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            base.OnActionExecuting(context);
        }
    }
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
        [RestrictToLocalhost]
        public dynamic GetMining()
        {
            proofOfWork nonce = solver.Solve(primaryBlockChain.last_block(), 0);

            return createNewBlock(nonce);

        }

        // GET blockchain/api/mine/{valuetostartfrom}
        [HttpGet("mine/{startPoint}")]
        [RestrictToLocalhost]
        public dynamic GetMiningWithPosition(int startPoint)
        {
            proofOfWork nonce = solver.Solve(primaryBlockChain.last_block(), startPoint);

            return createNewBlock(nonce);
        }

        // GET blockchain/api/transactions
        [HttpGet("transactions")]
        public dynamic GetCurrentTransactions()
        {
            return Ok(new
            {
                transactions = primaryBlockChain.CurrentTransactions
            });
        }
        // GET blockchain/api/transactions/drop
        [HttpGet("transactions/drop")]
        [RestrictToLocalhost]
        public dynamic GetDropTrans()
        {
            primaryBlockChain.dropTrans();

            return Ok(new
            {
                message = "Transactions dropped!"
            });

        }
        // GET blockchain/api/ledger
        [HttpGet("ledger")]
        public dynamic GetLedger()
        {
            return Ok(new
            {
                ledger = primaryBlockChain.Ledger
            });
        }
        // GET blockchain/api/chain
        [HttpGet("chain")]
        public dynamic GetChain()
        {
            return Ok(new
            {
                chain = primaryBlockChain.Chain,
                length = primaryBlockChain.Chain.Count
            });
        }

        // GET blockchain/api/account
        [HttpGet("account")]
        [RestrictToLocalhost]
        public dynamic GetAccount()
        {
            return Ok(new
            {
                privateKey = primaryBlockChain.getCurAccount().getAccountPrivKey(),
                publicKey = primaryBlockChain.getCurAccount().getAccountPubKey()
            });
        }

        // GET blockchain/api/account/public
        [HttpGet("account/public")]
        public dynamic GetAccountPublic()
        {
            return Ok(new
            {
                publicKey = primaryBlockChain.getCurAccount().getAccountPubKey()
            });
        }

        // POST blockchain/api/account/set
        [HttpPost("account/set")]
        [RestrictToLocalhost]
        public dynamic PostSetAccount([FromBody] accountAPI base64OfprivKey)
        {
            Boolean success = primaryBlockChain.changeAccount(base64OfprivKey.Account);

            if (success)
            {
                return Ok(new
                {
                    message = $"Account changed successfully!"
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

        // GET blockchain/api/nodes
        [HttpGet("nodes")]
        public dynamic GetNodes()
        {
            return Ok(new
            {
                nodes = primaryBlockChain.Nodes
            });
        }

        // GET 
        [HttpGet("nodes/drop")]blockchain/api/nodes/drop
        [RestrictToLocalhost]
        public dynamic GetDropNodes()
        {
            primaryBlockChain.dropAllNodes();

            return Ok(new
            {
                message = "Nodes dropped!"
            });
        }

        // POST blockchain/api/nodes/drop
        [HttpPost("nodes/drop/{nodeID}")]
        [RestrictToLocalhost]
        public dynamic PostDropNode([FromBody] nodeAPI value)
        {

            if (value.Address == null || !Uri.TryCreate(value.Address, UriKind.Absolute, out var uri))
            {
                return BadRequest(new
                {
                    message = $"Invalid url!"
                });
            }

            node nodeToRem = new node(value.Address);

            if (primaryBlockChain.dropNode(nodeToRem))
                return Ok(new
                {
                    message = $"Node {value.Address} successfully removed!"
                });
            else
                return NotFound(new
                {
                    message = $"Node {value.Address} was not found!"
                });
        }

        // GET blockchain/api/nodes/consensus
        [HttpGet("nodes/consensus")]
        [RestrictToLocalhost]
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
                //this code ignores ssl certificates, which sounds unsafe, but fuck if I know how to fix it
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                
                using (var client = new HttpClient(clientHandler))
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
                            long.Parse(b.index),
                            long.Parse(b.timestamp),
                            b.transactions.Select(t => new transaction(t.sender, t.recipient, double.Parse(t.amount), t.signature)),
                            long.Parse(b.nonce.value),
                            b.prevHash.value
                        )).ToList());// This recreates a block, and adds it to a chain based on json data
                               
                    }
                }
                catch (Exception) { }//just to capture any failed connections 
            }

            Boolean chainReplaced = false;
            foreach (var chain in chains)
            {
                if (chain.Count > primaryBlockChain.getChainLen() && blockChain.validateNewChain(chain)) 
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

        // POST blockchain/api/nodes/register
        [HttpPost("nodes/register")]
        [RestrictToLocalhost]
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

        // POST blockchain/api/transactions/new
        [HttpPost("transactions/new")]
        [RestrictToLocalhost]
        //ENSURE VALUES AREN'T EMPTY IF SENT IN
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
                    message = ex.Message
                }); 
            }

            return Ok(new
            {
                message = $"Transaction will be added to block {indexToAddTo}"
            });
        }
    }
}
