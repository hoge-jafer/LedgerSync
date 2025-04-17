using CommunityToolkit.Mvvm.DependencyInjection;
using LedgerSyncViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace LedgerSync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            Ioc.Default.ConfigureServices(Services);

        }
        public EventWaitHandle ProgramStarted { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "LedgerSync", out createNew);

            if (!createNew)
            {
                //MessageBox.Show("已经退出");
                //App.Current.Shutdown();
                //Environment.Exit(0);
            }
            base.OnStartup(e);
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ShellViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddSingleton<SecretKeyViewModel>();
            services.AddSingleton<AnalyzeViewModel>();
            services.AddSingleton<TradeDataViewModel>();
            services.BuildServiceProvider();
            return services.BuildServiceProvider();
        }
    }

}
