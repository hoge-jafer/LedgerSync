using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncModel.ResponseModel
{
    public class AccountInformationResponseModel
    {
        public int makerCommission { get; set; }
        public int takerCommission { get; set; }
        public int buyerCommission { get; set; }
        public int sellerCommission { get; set; }
        public Commissionrates commissionRates { get; set; }
        public bool canTrade { get; set; }
        public bool canWithdraw { get; set; }
        public bool canDeposit { get; set; }
        public bool brokered { get; set; }
        public bool requireSelfTradePrevention { get; set; }
        public bool preventSor { get; set; }
        public long updateTime { get; set; }
        public string accountType { get; set; }
        public Balance[] balances { get; set; }
        public string[] permissions { get; set; }
        public int uid { get; set; }
    }
}
