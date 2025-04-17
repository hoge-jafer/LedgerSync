using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncViewModel
{
    public partial class AnalyzeViewModel : ObservableObject
    {
        public AnalyzeViewModel()
        {
                
        }


        [RelayCommand]
        public async void AnalyzeViewLoad()
        {
            var model = new PlotModel { Title = "DateTimeAxis MVVM Example" };

            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                StringFormat = "MM-dd",
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsZoomEnabled = true,
                IsPanEnabled = true
            };

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Value",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };

            model.Axes.Add(dateAxis);
            model.Axes.Add(valueAxis);

            var series = new LineSeries
            {
                Title = "Random Data",
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.White
            };

            var now = DateTime.Now;
            var rand = new Random();

            for (int i = 0; i < 10; i++)
            {
                var date = now.AddDays(i);
                double value = rand.NextDouble() * 10;
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), value));
            }

            model.Series.Add(series);

            return model;
        }
    }
}
