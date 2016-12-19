using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EthereumRpc;
using EthereumRpc.Ethereum;
using EthereumRpc.RpcObjects;
using EthereumRpc.Service;
using HashLib;


namespace Demo
{
    class Program
    {
        private static void Main(string[] args)
        {

            var privateConnection = new ConnectionOptions()
            {
                Port = "8545",
                Url = "http://localhost"
            };


            var ethereumService = new EthereumService(privateConnection);

            var accounts = ethereumService.GetAccounts();

            foreach (var account in accounts)
            {
                var balance = ethereumService.GetBalance(account, BlockTag.Latest);
                Console.WriteLine("account {0}: {1}", account, EtherCurrencyConverter.Convert(balance));

                //var sign = ethereumService.Sign(account, "School bus");

                //Console.WriteLine("sign : " + sign);
            }

            var blockNumber =  ethereumService.GetBlockNumber();

            Console.WriteLine("BlockNumber : {0}", blockNumber);

            for (var i=0; i<blockNumber; i++)
            {
                var block = ethereumService.GetBlockByNumber(i, BlockTag.Quantity, true);
                var txNum = ethereumService.GetBlockTransactionCountByNumber(BlockTag.Quantity, i);
                if (txNum > 0) {
                    Console.WriteLine("{0} - {1}", i, txNum);
                }
            }

            var res = ethereumService.UnlockAccount(accounts[0], "");
            Console.WriteLine(res);
            var tx = new Transaction
            {
                From = accounts[0],
                To = accounts[1],
                Value = "2"
            };
            var hash = ethereumService.SendTransaction(accounts[0], accounts[1], 90000, "", 1, 2);

            Console.WriteLine(hash);
        }
    }
}
