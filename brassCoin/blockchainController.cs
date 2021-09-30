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

        private readonly blockChain primaryBlockChain;
        public blockchainController()
        {
            primaryBlockChain = new blockChain();
        }

        // GET: blockchain/api/mine
        [HttpGet("mine")]
        public dynamic Get()
        {
            return "we do a little mining";
        }

        // GET blockchain/api/chain
        [HttpGet("chain")]
        public dynamic Get(int id)
        {
            return new
            {
                chain = primaryBlockChain.chain, // so essentially, ive fucked something up and i cannot be arsed to unfuck it, so the chain and the new transaction list is shared between every blockchain
                length = primaryBlockChain.chain.Count // this is actually fairly easy to unfuck in the future if needs be
            };
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
