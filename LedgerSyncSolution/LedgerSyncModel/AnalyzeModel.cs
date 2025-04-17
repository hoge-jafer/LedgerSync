using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncModel
{
    public partial class AnalyzeModel : ObservableObject
    {
        /// <summary>
        /// 交易频率
        /// </summary>
        [ObservableProperty]
        private PlotModel tradingFrequencyPlotModel;

        /// <summary>
        /// 交易量
        /// </summary>
        [ObservableProperty]
        private PlotModel tradingVolumePlotModel;
    }
}
