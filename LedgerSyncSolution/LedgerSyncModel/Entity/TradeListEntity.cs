using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncModel.Entity
{
    public class TradeListEntity
    {
        public string TradeListID { get; set; }
        public string Symbol { get; set; }
        public string IsBuyers { get; set; }
        public string Price { get; set; }
        public string QTY { get; set; }//数量
        public string Time { get; set; }
    }
}
