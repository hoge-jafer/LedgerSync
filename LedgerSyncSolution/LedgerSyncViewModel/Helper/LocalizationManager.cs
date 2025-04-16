using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncViewModel.Helper
{
    public static class LocalizationManager
    {
        public static ResourceManager ResMgr = new ResourceManager("LedgerSync.Resources.Strings", typeof(LocalizationManager).Assembly);

        public static CultureInfo CurrentCulture { get; set; } = new CultureInfo("zh-CN");

        public static string Get(string key) => ResMgr.GetString(key, CurrentCulture);

        public static event EventHandler OnLanguageChanged;

        public static void ChangeCulture(string cultureName)
        {
            CurrentCulture = new CultureInfo(cultureName);
            OnLanguageChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
