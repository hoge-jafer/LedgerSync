using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncModel.ResponseModel
{
    public class FundingWalletResponseModel
    {
        public string asset { get; set; }
        public string free { get; set; }
        public string locked { get; set; }
        public string freeze { get; set; }
        public string withdrawing { get; set; }
        public string btcValuation { get; set; }
    }
}
