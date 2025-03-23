using CommunityToolkit.Mvvm.ComponentModel;
using LedgerSyncModel.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncModel
{
    public partial class TradeDataModel : ObservableObject
    {

        [ObservableProperty]// 最高价
        private ObservableCollection<TradeListEntity> observableCollectionTradeListEntity = new ObservableCollection<TradeListEntity>();

        [ObservableProperty]// 最高价
        private ObservableCollection<CoinEntity> observableCollectionCoinEntity = new ObservableCollection<CoinEntity>();
    }
}
