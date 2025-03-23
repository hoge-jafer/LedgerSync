using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncModel
{
    public partial class SecretKeyModel : ObservableObject
    {
        //apiKey
        [ObservableProperty]
        private string apiKey;
        //apiSecret
        [ObservableProperty]
        private string apiSecret;
    }
}
