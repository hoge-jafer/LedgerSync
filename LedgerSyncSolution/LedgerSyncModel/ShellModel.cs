using CommunityToolkit.Mvvm.ComponentModel;
using LedgerSyncModel.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LedgerSyncModel
{
    public partial class ShellModel : ObservableObject
    {
        [ObservableProperty]
        private string navigationContent;

        [ObservableProperty]
        private Visibility coinVisibility;

        [ObservableProperty]
        private string navigationSecretKey;

        [ObservableProperty]
        private Visibility secretKeyVisibility;

        [ObservableProperty]// 最高价
        private ObservableCollection<CoinEntity> observableCollectionCoinEntity = new ObservableCollection<CoinEntity>();

        [ObservableProperty]
        private WindowState systemState;
    }
}
