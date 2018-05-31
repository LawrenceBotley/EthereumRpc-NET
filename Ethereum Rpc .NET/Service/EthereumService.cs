using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Numerics;
using EthereumRpc.Ethereum;
using EthereumRpc.RpcObjects;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

namespace EthereumRpc
{
    public class EthereumService
    {
        public EthereumService(string url, string port)
        {
            RpcConnector.ConnectionOptions = new ConnectionOptions();
            RpcConnector.ConnectionOptions.Url = url;
            RpcConnector.ConnectionOptions.Port = port;
        }

        public EthereumService(ConnectionOptions connectionOptions)
        {
            RpcConnector.ConnectionOptions = connectionOptions;
        }

        public EthereumService()
        {
            if (RpcConnector.ConnectionOptions == null)
            {
                RpcConnector.ConnectionOptions = new ConnectionOptions();
                var urlString = ConfigurationManager.AppSettings["EthereumRpcUrl"];
                var portString = ConfigurationManager.AppSettings["EthereumRpcPort"];

                if (string.IsNullOrEmpty(urlString) || string.IsNullOrEmpty(portString))
                {
                    //throw new Exception("invalid app.config keys - please provide - EthereumRpcUrl and EthereumRpcPort");
                }

                RpcConnector.ConnectionOptions.Url = urlString;
                RpcConnector.ConnectionOptions.Port = portString;
            }
        }

        /// <summary>
        /// Returns the current client version
        /// </summary>
        /// <returns>The current client version</returns>
        public string GetWeb3ClientVersion()
        {
            var rpcRequest = new RpcRequest(RpcMethod.web3_clientVersion);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        /// <summary>
        ///  Returns Keccak-256 (not the standardized SHA3-256) of the given data.
        /// </summary>
        /// <param name="value"> the data to convert into a SHA3 hash</param>
        /// <returns>The SHA3 result of the given string.</returns>
        public string GetWeb3Sha3(string value)
        {
            var rpcRequest = new RpcRequest(RpcMethod.web3_sha3);
            rpcRequest.AddParam(value);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        /// <summary>
        /// Returns the current network protocol version.
        /// </summary>
        /// <returns> The current network protocol version</returns>
        public string GetNetVersion()
        {
            var rpcRequest = new RpcRequest(RpcMethod.net_version);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        /// <summary>
        /// Returns true if client is actively listening for network connections.
        /// </summary>
        /// <returns>true when listening, otherwise false.</returns>
        public bool GetNetListening()
        {
            var rpcRequest = new RpcRequest(RpcMethod.net_version);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var isListening = Convert.ToBoolean(Int32.Parse(rpcResult.Result));
            return isListening;
        }

        /// <summary>
        /// Returns number of peers currenly connected to the client.
        /// </summary>
        /// <returns>integer of the number of connected peers.</returns>
        public int GetNetPeerCount()
        {
            var rpcRequest = new RpcRequest(RpcMethod.net_peerCount);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var peerCount = Convert.ToInt32(rpcResult.Result, 16);
            return peerCount;
        }

        /// <summary>
        /// Returns the current ethereum protocol version.
        /// </summary>
        /// <returns>The current ethereum protocol version</returns>
        public string GetProtocolVersion()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_protocolVersion);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        /// <summary>
        /// Returns an object object with data about the sync status or FALSE.
        /// </summary>
        /// <returns></returns>
        public SyncStatus GetSyncing()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_syncing);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            
            if (rpcResult.Result.GetType().FullName != "System.Boolean")
            {
                var json = JsonConvert.SerializeObject(rpcResult.Result);
                SyncStatus syncStatus = JsonConvert.DeserializeObject<SyncStatus>(json);
                syncStatus.IsSyncing = true;

                return syncStatus;
            }

            return new SyncStatus() {IsSyncing = false};
        }

        /// <summary>
        /// Returns the client coinbase address.
        /// </summary>
        /// <returns>the current coinbase address.</returns>
        public Address GetCoinbase()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_coinbase);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var address = new Address(rpcResult.Result);
            return address;
        }

        /// <summary>
        /// Returns true if client is actively mining new blocks.
        /// </summary>
        /// <returns>returns true of the client is mining, otherwise false.</returns>
        public bool GetMining()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_mining);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        /// <summary>
        /// Returns the number of hashes per second that the node is mining with.
        /// </summary>
        /// <returns>number of hashes per second.</returns>
        public long GetHashrate()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_hashrate);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var hashRate = Convert.ToInt64(rpcResult.Result, 16);
            return hashRate;
        }

        /// <summary>
        /// Returns the current price per gas in wei.
        /// </summary>
        /// <returns>integer of the current gas price in wei.</returns>
        public long GetGasPrice()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_gasPrice);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var gasPrice = Convert.ToInt64(rpcResult.Result, 16);
            return gasPrice;
        }

        /// <summary>
        /// Returns a list of addresses owned by client.
        /// </summary>
        /// <returns>addresses owned by the client.</returns>
        public string[] GetAccounts()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_accounts);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var accountList = new List<string>();
            foreach (var account in rpcResult.Result)
            {
                accountList.Add(account.Value);
            }
            return accountList.ToArray();
        }

        /// <summary>
        /// Returns the number of most recent block.
        /// </summary>
        /// <returns> integer of the current block number the client is on.</returns>
        public long GetBlockNumber()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_blockNumber);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var ethBlockNumber = Convert.ToInt64(rpcResult.Result, 16);
            return ethBlockNumber;
        }

        /// <summary>
        /// Returns the balance of the account of given address.
        /// </summary>
        /// <param name="address">address to check for balance.</param>
        /// <param name="blockTag"> integer block number</param>
        /// <param name="blockNumber">Block param</param>
        /// <returns></returns>
        public BigInteger GetBalance(string address, BlockTag blockTag = BlockTag.Quantity, int blockNumber = -1)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getBalance);
            rpcRequest.AddParam(address);
            if (blockTag != BlockTag.Quantity && blockNumber > -1)
            {
                throw new Exception("Balance tag and block number cannot both be provided, chose either");
            }

            if (blockTag != BlockTag.Quantity)
            {
                rpcRequest.AddParam(blockTag.ToJsonMethodName());
            }
            else
            {
                rpcRequest.AddParam(blockNumber.ToString());
            }

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            if (rpcResult.Result == null)
            {
                return 0;
            }

            string value = rpcResult.Result.ToString();
            var balance = value.ToBigInteger();

            return balance;
        }

        /// <summary>
        /// Returns the value from a storage position at a given address.
        /// </summary>
        /// <param name="address"> address of the storage.</param>
        /// <param name="storagePosition">integer of the position in the storage.</param>
        /// <param name="blockTag">Block Param</param>
        /// <param name="blockNumber">integer block number</param>
        /// <returns>the value at this storage position.</returns>
        public long GetStorageAt(string address,int storagePosition, BlockTag blockTag = BlockTag.Quantity, int blockNumber = -1)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getStorageAt);
            rpcRequest.AddParam(address);
            rpcRequest.AddParam(storagePosition.ToString());
            if (blockTag != BlockTag.Quantity && blockNumber > -1)
            {
                throw new Exception("Balance tag and block number cannot both be provided, chose either");
            }

            if (blockTag != BlockTag.Quantity)
            {
                rpcRequest.AddParam(blockTag.ToJsonMethodName());
            }
            else
            {
                rpcRequest.AddParam(blockNumber.ToString());
            }

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var balance = Convert.ToInt64(rpcResult.Result, 16);
            return balance;
        }

        /// <summary>
        /// Returns the number of transactions sent from an address.
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="blockTag">Block Param</param>
        /// <param name="blockNumber">integer block number,</param>
        /// <returns>integer of the number of transactions send from this address.</returns>
        public long GetTransactionCount(string address, BlockTag blockTag = BlockTag.Quantity, int blockNumber = -1)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getTransactionCount);
            rpcRequest.AddParam(address);

            if (blockTag != BlockTag.Quantity && blockNumber > -1)
            {
                throw new Exception("Balance tag and block number cannot both be provided, chose either");
            }

            if (blockTag != BlockTag.Quantity)
            {
                rpcRequest.AddParam(blockTag.ToJsonMethodName());
            }
            else
            {
                rpcRequest.AddParam(blockNumber.ToString());
            }

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var balance = Convert.ToInt64(rpcResult.Result, 16);
            return balance;
        }

        /// <summary>
        /// Returns the number of transactions in a block from a block matching the given block hash.
        /// </summary>
        /// <param name="blockHash"> hash of a block</param>
        /// <returns> integer of the number of transactions in this block.</returns>
        public long GetBlockTransactionCountByHash(string blockHash)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getBlockTransactionCountByHash);
            //todo validate the 20 byte block hash
            rpcRequest.AddParam(blockHash);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            if (rpcResult.Result == null)
                return 0;
            var count = Convert.ToInt64(rpcResult.Result, 16);

            return count;
        }

        /// <summary>
        /// Returns the number of transactions in a block from a block matching the given block number.
        /// </summary>
        /// <param name="blockTag">Block Param</param>
        /// <param name="blockNumber">integer of a block number,</param>
        /// <returns></returns>
        public long GetBlockTransactionCountByNumber(BlockTag blockTag = BlockTag.Quantity, int blockNumber = -1)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getBlockTransactionCountByNumber);
            if (blockTag != BlockTag.Quantity && blockNumber > -1)
            {
                throw new Exception("Balance tag and block number cannot both be provided, chose either");
            }

            if (blockTag != BlockTag.Quantity)
            {
                rpcRequest.AddParam(blockTag.ToJsonMethodName());
            }
            else
            {
                rpcRequest.AddParam(blockNumber.ToString());
            };
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var count = Convert.ToInt64(rpcResult.Result, 16);
            return count;
        }

        /// <summary>
        /// Returns the number of uncles in a block from a block matching the given block hash.
        /// </summary>
        /// <param name="hashBlock">hash of a block</param>
        /// <returns> integer of the number of uncles in this block.</returns>
        public long GetUncleCountByBlockHash(string hashBlock)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getUncleCountByBlockHash);
            rpcRequest.AddParam(hashBlock);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            if (rpcResult.Result == null)
                return 0;
            var ethBlockNumber = Convert.ToInt64(rpcResult.Result, 16);
            return ethBlockNumber;
        }

        /// <summary>
        /// Returns the number of uncles in a block from a block matching the given block number.
        /// </summary>
        /// <param name="blockNumber">integer of a block number</param>
        /// <returns>integer of the number of uncles in this block.</returns>
        public long GetUncleCountByBlockNumber(int blockNumber)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getUncleCountByBlockNumber);
            rpcRequest.AddParam(blockNumber.ToHexString());
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var ethBlockNumber = Convert.ToInt64(rpcResult.Result, 16);

            return ethBlockNumber;
        }

        /// <summary>
        /// Returns code at a given address.
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="blockTag"></param>
        /// <param name="blockNumber">integer block number,</param>
        /// <returns></returns>
        public string GetCode(string address, BlockTag blockTag = BlockTag.Quantity, int blockNumber = -1)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getCode);
            rpcRequest.AddParam(address);

            if (blockTag != BlockTag.Quantity && blockNumber > -1)
            {
                throw new Exception("Balance tag and block number cannot both be provided, chose either");
            }

            if (blockTag != BlockTag.Quantity)
            {
                rpcRequest.AddParam(blockTag.ToJsonMethodName());
            }
            else
            {
                rpcRequest.AddParam(blockNumber.ToHexString());
            };
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            return rpcResult.Result;
        }

        /// <summary>
        /// Signs data with a given address.
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="data">Data to sign</param>
        /// <returns>Signed data</returns>
        public string Sign(string address, string data)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_sign);
            rpcRequest.AddParam(address);
            rpcRequest.AddParam(data);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        /// <summary>
        /// Creates new message call transaction or a contract creation, if the data field contains code.
        /// </summary>
        /// <param name="from">The address the transaction is send from.</param>
        /// <param name="to">(optional when creating new contract) The address the transaction is directed to.</param>
        /// <param name="gas">(optional, default: 90000) Integer of the gas provided for the transaction execution. It will return unused gas.</param>
        /// <param name="data">(optional) The compiled code of a contract</param>
        /// <param name="gasPrice">(optional, default: To-Be-Determined) Integer of the gasPrice used for each paid gas</param>
        /// <param name="value">(optional) Integer of the value send with this transaction</param>
        /// <param name="nonce">(optional) Integer of a nonce. This allows to overwrite your own pending transactions that use the same nonce.</param>
        /// <returns>the transaction hash, or the zero hash if the transaction is not yet available.</returns>
        public string SendTransaction(string from, string to, int gas, string data, int gasPrice = -1, int value = -1, int nonce = -1)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_sendTransaction);
            var transactionParams = new Transaction();
            transactionParams.To = to;

            if (from != null)
                transactionParams.From = from;

            if (data != null)
                transactionParams.Data = data;

            if (gas > -1)
                transactionParams.Gas = gas.ToHexString();

            if (gasPrice > -1)
                transactionParams.GasPrice = gas.ToHexString();

            if (value > -1)
                transactionParams.Value = value.ToHexString();

            if (nonce > -1)
                transactionParams.Nonce = nonce.ToHexString();

            rpcRequest.AddParam(transactionParams);

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            return rpcResult.Result;
        }

        public string SendTransaction(Transaction transaction)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_sendTransaction);
        
            rpcRequest.AddParam(transaction);

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            return rpcResult.Result;
        }

        /// <summary>
        /// Creates new message call transaction or a contract creation for signed transactions.
        /// </summary>
        /// <param name="data">The signed transaction data.</param>
        /// <returns> the transaction hash, or the zero hash if the transaction is not yet available.</returns>
        public string SendRawTransaction(string data)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_sendRawTransaction);
            rpcRequest.AddParam(new { data = data });
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        public string Call(Transaction transaction)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_call);
            rpcRequest.AddParam(transaction);
            rpcRequest.AddParam(BlockTag.Latest.ToJsonMethodName());

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        public string EstimateGas(Transaction transaction)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_estimateGas);
            rpcRequest.AddParam(transaction);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            string gas = rpcResult.Result.ToString();
            //var estimatedGas = gas.HexToInt();

            return gas;
        }

        public Block GetBlockByHash(string hash,bool returnFullBlock)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getBlockByHash);
            rpcRequest.AddParam(hash);
            rpcRequest.AddParam(returnFullBlock);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var json = JsonConvert.SerializeObject(rpcResult.Result);

            var block = JsonConvert.DeserializeObject<Block>(json);
            var jsonTransactions = JsonConvert.SerializeObject(rpcResult.Result.transactions);

            if (returnFullBlock)
            {
                block.TransactionsFull = JsonConvert.DeserializeObject<List<Transaction>>(jsonTransactions);

                if (block.TransactionsFull.Count > 0)
                {
                    int i = 100;
                }
            }
            else
            {
                block.TransactionHashes = JsonConvert.DeserializeObject<string>(jsonTransactions);
            }

            
            return block;
        }

        public Block GetBlockByNumber(int blockNumber, BlockTag blockTag, bool returnFullObject)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getBlockByNumber);
            if (blockTag != BlockTag.Quantity && blockNumber > -1)
            {
                throw new Exception("Balance tag and block number cannot both be provided, chose either");
            }

            if (blockTag != BlockTag.Quantity)
            {
                rpcRequest.AddParam(blockTag.ToJsonMethodName());
            }
            else
            {
                rpcRequest.AddParam(blockNumber.ToHexString());
            }

            rpcRequest.AddParam(returnFullObject);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var json = JsonConvert.SerializeObject(rpcResult.Result);
            var block = JsonConvert.DeserializeObject<Block>(json);
            return block;
        }

        public Transaction GetTransactionByHash(string hash)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getTransactionByHash);
            rpcRequest.AddParam(hash);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var json = JsonConvert.SerializeObject(rpcResult.Result);
            var tx = JsonConvert.DeserializeObject<Transaction>(json);
            return tx;
        }

        public Transaction GetTransactionByBlockHashAndIndex(string hash, int index)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getTransactionByBlockHashAndIndex);
            rpcRequest.AddParam(hash);
            rpcRequest.AddParam(index.ToHexString());
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var json = JsonConvert.SerializeObject(rpcResult.Result);
            var tx = JsonConvert.DeserializeObject<Transaction>(json);
            return tx;
        }

        public Transaction GetTransactionByBlockNumberAndIndex(int blockNumber, int index)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getTransactionByBlockNumberAndIndex);
            rpcRequest.AddParam(blockNumber.ToHexString());
            rpcRequest.AddParam(index.ToHexString());

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var json = JsonConvert.SerializeObject(rpcResult.Result);
            var tx = JsonConvert.DeserializeObject<Transaction>(json);

            return tx;
        }

        public Transaction GetTransactionReceipt(string transactionHash)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getTransactionReceipt);
            rpcRequest.AddParam(transactionHash);

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var json = JsonConvert.SerializeObject(rpcResult.Result);
            var tx = JsonConvert.DeserializeObject<Transaction>(json);

            return tx;
        }

        public Block GetUncleByBlockHashAndIndex(string hash, int index)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getUncleByBlockHashAndIndex);
            rpcRequest.AddParam(hash);
            rpcRequest.AddParam(index.ToHexString());

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var json = JsonConvert.SerializeObject(rpcResult.Result);
            var block = JsonConvert.DeserializeObject<Block>(json);

            return block;
        }

        public Block GetUncleByBlockNumberAndIndex(int blockNumber, int index)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getUncleByBlockNumberAndIndex);
            rpcRequest.AddParam(blockNumber.ToHexString());
            rpcRequest.AddParam(index.ToHexString());

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var json = JsonConvert.SerializeObject(rpcResult.Result);
            var block = JsonConvert.DeserializeObject<Block>(json);

            return block;
        }

        public string[] GetCompilers()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getCompilers);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            var compilerList = new List<string>();
            foreach (var account in rpcResult.Result)
            {
                compilerList.Add(account.Value);
            }

            return compilerList.ToArray();
        }

        public RpcResult CompileSolidity(string contract)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_compileSolidity);

            rpcRequest.AddParam(contract);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            return rpcResult;
        }

        public string CompileLLL()
        {
            throw new Exception("not yet implemented");
        }

        public string CompileSerpent()
        {
            throw new Exception("not yet implemented");
        }

        /// <summary>
        /// Creates a filter object, based on filter options, to notify when the state changes (logs). To check if the state has changed, call GetFilterChanges.
        /// </summary>
        /// <param name="fromBlock">(optional, default: "latest") Integer block number, </param>
        /// <param name="toBlock"></param>
        /// <param name="address"></param>
        /// <param name="topics"></param>
        /// <returns></returns>
        public string NewFilter(Filter filter)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_newFilter);
            
            rpcRequest.AddParam(filter);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            return rpcResult.Result;
        }

        public string NewBlockFilter()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_newBlockFilter);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        public string NewPendingTransactionFilter()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_newPendingTransactionFilter);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        public bool UninstallFilter(string filterId)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_uninstallFilter);
            rpcRequest.AddParam(filterId);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        public Log[] GetFilterChanges(string filterId)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getFilterChanges);
            rpcRequest.AddParam(filterId);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var list = new List<Log>();

            foreach (var account in rpcResult.Result)
            {
                var log = new Log(account);

                list.Add(log);
            }
            return list.ToArray();
        }

        public List<string> GetBlockFilterChanges(string filterId)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getFilterChanges);
            rpcRequest.AddParam(filterId);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var list = new List<string>();

            foreach (var blockHash in rpcResult.Result)
            {
                list.Add(blockHash.ToString());
            }
            return list;
        }


        public string[] GetFilterLogs(string filterId)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getFilterLogs);
            rpcRequest.AddParam(filterId);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var list = new List<string>();
            foreach (var account in rpcResult.Result)
            {
                list.Add(account.Value);
            }
            return list.ToArray();
        }


        public string[] GetLogs(Log log)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getLogs);
            rpcRequest.AddParam(log);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            var list = new List<string>();
            foreach (var account in rpcResult.Result)
            {
                list.Add(account.Value);
            }
            return list.ToArray();
        }

        public Work GetWork()
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_getWork);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            var list = new List<string>();
            foreach (var account in rpcResult.Result)
            {
                list.Add(account.ToString());
            }


            var work = new Work()
            {
                BlockHeaderPowHash = list[0],
                SeedHash = list[1],
                Target = list[2]
            };

            return work;
        }

        /// <summary>
        /// Used for submitting a proof-of-work solution.
        /// </summary>
        /// <param name="nonce">The nonce found(64 bits)</param>
        /// <param name="powHash">The header's pow-hash (256 bits)</param>
        /// <param name="mix">The mix digest(256 bits)</param>
        /// <returns> returns true if the provided solution is valid, otherwise false.</returns>
        public bool SubmitWork(string nonce, string powHash, string mix)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_submitWork);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            rpcRequest.AddParam(nonce);
            rpcRequest.AddParam(powHash);
            rpcRequest.AddParam(mix);

            return rpcResult.Result;
        }




        /// <summary>
        /// Used for submitting mining hashrate.
        /// </summary>
        /// <param name="hashRate">a hexadecimal string representation (32 bytes) of the hash rate</param>
        /// <param name="clientId">A random hexadecimal(32 bytes) ID identifying the client</param>
        /// <returns></returns>
        public bool SubmitHashrate(string hashRate, string clientId)
        {
            var rpcRequest = new RpcRequest(RpcMethod.eth_submitHashrate);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            rpcRequest.AddParam(hashRate);
            rpcRequest.AddParam(clientId);

            return rpcResult.Result;
        }

        /// <summary>
        /// Returns the current whisper protocol version.
        /// </summary>
        /// <returns>The current whisper protocol version</returns>
        public string ShhVersion()
        {
            var rpcRequest = new RpcRequest(RpcMethod.shh_version);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        public string NewAccount(string password)
        {
            var rpcRequest = new RpcRequest(RpcMethod.personal_newAccount);
            rpcRequest.AddParam(password);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }

        /// <summary>
        /// Unlocks account using password
        /// </summary>
        /// <param name="account">The account address</param>
        /// <param name="password">The password for the account</param>
        /// <returns></returns>
        public bool UnlockAccount(string account, string password)
        {
            var rpcRequest = new RpcRequest(RpcMethod.personal_unlockAccount);
            rpcRequest.AddParam(account);
            rpcRequest.AddParam(password);
            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);

            if (rpcResult.Error != null)
            {
                return false;
            }

            return rpcResult.Result;
        }



        public string ShhPost(string from, string to, string[] topics, string payload, string priority, string ttl)
        {
            var rpcRequest = new RpcRequest(RpcMethod.shh_post);

            var whisper = new Whisper()
            {
                From = "0x04f96a5e25610293e42a73908e93ccc8c4d4dc0edcfa9fa872f50cb214e08ebf61a03e245533f97284d442460f2998cd41858798ddfd4d661997d3940272b717b1",
                To = "0x3e245533f97284d442460f2998cd41858798ddf04f96a5e25610293e42a73908e93ccc8c4d4dc0edcfa9fa872f50cb214e08ebf61a0d4d661997d3940272b717b1",
                Payload =  "0x7b2274797065223a226d6",
                Priority = "0x64",
                Ttl = "0x64",
            };

            rpcRequest.AddParam(whisper);

            var rpcResult = new RpcConnector().MakeRequest(rpcRequest);
            return rpcResult.Result;
        }
    }
}
