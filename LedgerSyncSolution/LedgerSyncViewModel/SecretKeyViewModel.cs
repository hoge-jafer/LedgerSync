using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using LedgerSyncModel;
using LedgerSyncViewModel.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncViewModel
{
    public partial class SecretKeyViewModel : ObservableObject
    {
        public SecretKeyViewModel()
        {
            secretKeyModels = new SecretKeyModel();
        }

        [ObservableProperty]
        private SecretKeyModel secretKeyModels;

        [RelayCommand]
        public async void SecretKeyViewLoad()
        {

            //var priceresult = await tradingAccountTrade.AccountTradeList("BTCUSDT");

            //var result = await tradingAccountTrade.AccountInformation(8000);

            //var results = await wallet.FundingWallet();
            //Ioc.Default.GetService<ShellViewModel>()
            if (!string.IsNullOrEmpty(SecretKeyModels.ApiKey) && !string.IsNullOrEmpty(SecretKeyModels.ApiSecret))
            {
                Ioc.Default.GetService<ShellViewModel>().QuerySecretKey();
            }

        }

        [RelayCommand]
        public async void SaveSecretKey()
        {
            if (!string.IsNullOrEmpty( SecretKeyModels.ApiKey) && !string.IsNullOrEmpty(SecretKeyModels.ApiSecret)) 
            {
                Ioc.Default.GetService<ShellViewModel>().InsertSecretKey();
            }
   
        }
    }
}
