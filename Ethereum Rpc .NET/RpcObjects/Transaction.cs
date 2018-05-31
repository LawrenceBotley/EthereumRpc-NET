using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthereumRpc.Ethereum;
using Newtonsoft.Json;

namespace EthereumRpc.RpcObjects
{
    public class Transaction
    {
        [JsonProperty(PropertyName = "to", Order = 0)]
        public string To { get; set; }

        [JsonProperty(PropertyName = "from", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string From { get; set; }

        [JsonProperty(PropertyName = "gas", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Gas { get; set; }

        [JsonProperty(PropertyName = "data", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "gasPrice", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GasPrice { get; set; }

        [JsonProperty(PropertyName = "value", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "nonce", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Nonce { get; set; }

        [JsonProperty(PropertyName = "hash", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "blockHash", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BlockHash { get; set; }

        [JsonProperty(PropertyName = "blockNumber", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BlockNumber { get; set; }

        [JsonProperty(PropertyName = "input", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Input { get; set; }

        /// <summary>
        /// The total amount of gas used when this transaction was executed in the block.
        /// </summary>
        [JsonProperty(PropertyName = "cumulativeGasUsed", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CumulativeGasUsed { get; set; }

        /// <summary>
        ///  The contract address created, if the transaction was a contract creation, otherwise null.
        /// </summary>
        [JsonProperty(PropertyName = "contractAddress", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ContractAddress { get; set; }

        /// <summary>
        /// Array of log objects, which this transaction generated.
        /// </summary>
        [JsonProperty(PropertyName = "logs", Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Log> Logs { get; set; }
    }
}
