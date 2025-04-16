using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace LedgerSyncViewModel.Helper
{
    public class LocExtension : MarkupExtension, INotifyPropertyChanged
    {
        public string Key { get; set; }

        public LocExtension(string key) => Key = key;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            LocalizationManager.OnLanguageChanged += (_, __) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            return Value;
        }

        public object Value => LocalizationManager.Get(Key);

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
