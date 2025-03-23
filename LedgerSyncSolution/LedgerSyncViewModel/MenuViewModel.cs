using Binance.Spot;
using CommunityToolkit.Mvvm.ComponentModel;
using LedgerSyncModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncViewModel
{
   public partial class MenuViewModel : ObservableObject
    {
        public  MenuViewModel()
        {
            menuModels = new MenuModel();


        }

        SpotAccountTrade tradingAccountTrade;

        [ObservableProperty]
        private MenuModel menuModels;



    }
}
