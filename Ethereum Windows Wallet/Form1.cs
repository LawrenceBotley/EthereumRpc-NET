using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EthereumRpc;
using EthereumRpc.Ethereum;
using EthereumRpc.RpcObjects;
using EthereumRpc.Service;

namespace Ethereum.Wallet
{
    public partial class Form1 : Form
    {
        public EthereumService EthereumService { get; set; }
        public List<SendTxHistory> SendTxHistoryList { get; set; }
        public List<Account> Accounts { get; set; }
        public int AccountIndex = -1;
        public string CurrentAccount;

        public Form1()
        {
            InitializeComponent();
            SendTxHistoryList = new List<SendTxHistory>();


            var privateConnection = new ConnectionOptions()
            {
                Port = "8545",
                Url = "http://localhost"
            };

            EthereumService = new EthereumService(privateConnection);

            LoadAccounts();

            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerAsync();

            cmbSendUnit.DataSource = Enum.GetValues(typeof(EtherUnit));
            cmbSendUnit.SelectedItem = "Ether";
        }

        private void LoadAccounts()
        {
            Accounts = new List<Account>();
            
            var accounts = EthereumService.GetAccounts();
            cmdAccounts.Items.Clear();

            foreach (var account in accounts)
            {
                var a = new Account();
                a.Address = account;

                var filter = new Filter(fromBlock: "0x01")
                {
                    Address = a.Address

                };

                a.FilterId = EthereumService.NewFilter(filter);
                Accounts.Add(a);

                var sign = EthereumService.Sign(a.Address, "hello");

                a.Unlocked = sign != null;

                cmdAccounts.Items.Add(a);
                //EthereumService.UnlockAccount(a.Address, "lawrence");
            }

            if (accounts.Any())
            {
                cmdAccounts.SelectedIndex = 0;
            }
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(2000);

            while (true)
            {
                RefreshAccount(GetCurrentAccount().Address);
                Thread.Sleep(500);
            }
        }

        private Account GetCurrentAccount()
        {
            return Accounts[AccountIndex];
        }

        private void cmdAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            AccountIndex = cmdAccounts.SelectedIndex;
            var account = Accounts[AccountIndex];
            if (!account.Unlocked)
            {
                var frm = new FrmUnlock(account.Address, EthereumService);
                frm.ShowDialog();
                LoadAccounts();
            }

            CurrentAccount = account.Address;

        }

        private void RefreshAccount(string account)
        {
            var balance = EthereumService.GetBalance(account, BlockTag.Latest);
            var pending = EthereumService.GetBalance(account, BlockTag.Pending);
            var txCount = EthereumService.GetTransactionCount(account, BlockTag.Latest);
            var changes = EthereumService.GetFilterChanges(GetCurrentAccount().FilterId);

            BeginInvoke((MethodInvoker)delegate
            {
                lblBalance.Text = string.Format("{0} Eth", balance.WeiToEther());
                lblPending.Text = string.Format("{0} Eth", pending.WeiToEther());
                lblTxCount.Text = txCount.ToString();
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var value = txtAmount.Text.ToBigInteger(NumberStyles.Integer);
            value = value*1000000000000000000;

            var transaction = new Transaction()
            {
                To = txtSendTo.Text,
                From = cmdAccounts.SelectedItem.ToString(),
                Value = value.ToHexString()
            };


            var txHash = EthereumService.SendTransaction(transaction);

            var sendTxHistory = new SendTxHistory()
            {
                DateTime = DateTime.Now.ToString(),
                Amount =  transaction.Value.ToBigInteger().WeiToEther(),
                Hash = txHash
            };

            SendTxHistoryList.Add(sendTxHistory);

            dgSendTransactions.DataSource = null;
            dgSendTransactions.DataSource = SendTxHistoryList;
            SetGridWidth();

            txtTransactionHash.Text = txHash;
            // MessageBox.Show(txHash, "Transaction");
        }

        private void SetGridWidth()
        {
            var section = (dgSendTransactions.Width-40)/10;

            //dgSendTransactions.Columns[0].Width = section*3;
            //dgSendTransactions.Columns[1].Width = section*2;
            //dgSendTransactions.Columns[2].Width = section*5;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tx = EthereumService.GetTransactionByHash(txtTransactionHash.Text);

            int i = 100;
        }

        private void dgSendTransactions_Resize(object sender, EventArgs e)
        {
            SetGridWidth();
        }
    }

    public class SendTxHistory
    {
        public string DateTime { get; set; }
        public string Amount { get; set; }
        public string Hash { get; set; }
    }

    public class Account
    {
        public string Address { get; set; }
        public string FilterId { get; set; }
        public bool Unlocked { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Address, Unlocked ? "Unlocked" : "Locked");
        }
    }
}
