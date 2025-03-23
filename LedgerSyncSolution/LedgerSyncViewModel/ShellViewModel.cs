using Binance.Spot;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using LedgerSyncModel;
using LedgerSyncViewModel.Helper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LedgerSyncViewModel
{
    public partial class ShellViewModel : ObservableObject
    {
        public ShellViewModel()
        {
            shellModels = new ShellModel();
       

    


        }
        public SpotAccountTrade tradingAccountTrade;
        public Wallet wallet;
       public string SQLiteDBPath = "LedgerSync.db";

        public string SQLiteDBCreateSecretKeySQL = @"CREATE TABLE IF NOT EXISTS SecretKey (
    ID INTEGER PRIMARY KEY AUTOINCREMENT, 
    ApiKey TEXT NOT NULL, 
    ApiSecret TEXT NOT NULL);";

        public string SQLiteDBInsertSecretKeySQL = "INSERT INTO SecretKey (ApiKey, ApiSecret) VALUES (@ApiKey, @ApiSecret);";

        public string query = @"
                SELECT ID, ApiKey, ApiSecret 
                FROM SecretKey 
                ORDER BY ID DESC 
                LIMIT 1;";

        [ObservableProperty]
        private ShellModel shellModels;

        [RelayCommand]
        public async void ShellViewLoad()
        {
            bool isNewDatabase = !File.Exists(SQLiteDBPath);
            if (isNewDatabase)
            {
                //SQLiteConnection.CreateFile(SQLiteDBPath);
                Debug.WriteLine("数据库文件不存在，已创建 LedgerSync.db！");
                CreateSecretKey();
            }
            if (!isNewDatabase)
            {

                QuerySecretKey();
            }

            tradingAccountTrade = new SpotAccountTrade(apiKey: Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiKey, apiSecret: Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiSecret);
            wallet = new Wallet(apiKey: Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiKey, apiSecret: Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiSecret);
        }


        [RelayCommand]
        public async void SecretKey()
        {
            ShellModels.NavigationContent = "/UI/SecretKeyView.xaml";
        }


        [RelayCommand]
        public async void TradeData()
        {
            ShellModels.NavigationContent = "/UI/TradeDataView.xaml";
        }

        [RelayCommand]
        public  void SelectCoin(string content)
        {
            Ioc.Default.GetService<TradeDataViewModel>().GetTradeListData(content);
        }

        [RelayCommand]
        public void Print()
        {
            Ioc.Default.GetService<TradeDataViewModel>().Print();
        }

        #region MiniSystem

        [RelayCommand]
        public void MiniSystem()
        {
            ShellModels.SystemState = WindowState.Minimized;
        }

        #endregion

        #region MaxSystem

        [RelayCommand]
        public void MaxSystem()
        {
            // \uE002
            // <!--&#xEF2F; 默认状态-->
            //<!--&#xEF2E; 最大状态-->
            if (ShellModels.SystemState != WindowState.Maximized)
            {

                ShellModels.SystemState = WindowState.Maximized;
                //ShellModels.MaxOrNormal = "\uEF2F";
            }
            else
            {
                ShellModels.SystemState = WindowState.Normal;
                //ShellModels.MaxOrNormal = "\uEF2E";
            }
        }

        #endregion

        #region ExitSystem

        [RelayCommand]
        public void ExitSystem()
        {
            Environment.Exit(0);
        }

        #endregion

        #region Action

        public void CreateSecretKey()
        {
            using (var db = new SQLiteHelper(Ioc.Default.GetService<ShellViewModel>().SQLiteDBPath))
            {
                // 创建表
                db.ExecuteNonQuery(SQLiteDBCreateSecretKeySQL);

                //// 插入数据
                //db.ExecuteNonQuery("INSERT INTO Users (Name, Age) VALUES (@name, @age);",
                //    new Dictionary<string, object> { { "@name", "Alice" }, { "@age", 25 } });

                // 查询数据
                //var result = db.ExecuteQuery("SELECT * FROM Users;");
                //foreach (DataRow row in result.Rows)
                //{
                //    Console.WriteLine($"ID: {row["Id"]}, Name: {row["Name"]}, Age: {row["Age"]}");
                //}
            }
        }

        public void InsertSecretKey() 
        {
            string apiKey = Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiKey;// "your_api_key";
            string apiSecret = Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiSecret;// "your_api_secret";

            using (var connection = new SQLiteConnection($"Data Source={SQLiteDBPath};Version=3;"))
            {
                connection.Open();

                using (var cmd = new SQLiteCommand(SQLiteDBInsertSecretKeySQL, connection))
                {
                    cmd.Parameters.AddWithValue("@ApiKey", apiKey);
                    cmd.Parameters.AddWithValue("@ApiSecret", apiSecret);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    Debug.WriteLine($"成功插入 {rowsAffected} 条数据！");
                }
            }
        }

        public void QuerySecretKey() 
        {
            using (var connection = new SQLiteConnection($"Data Source={SQLiteDBPath};Version=3;"))
            {
                connection.Open();

                using (var cmd = new SQLiteCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())  // 读取一行数据
                    {
                        int id = reader.GetInt32(0);
                        string apiKey = reader.GetString(1);
                        string apiSecret = reader.GetString(2);

                        Debug.WriteLine($"最大 ID 记录 -> ID: {id}, ApiKey: {apiKey}, ApiSecret: {apiSecret}");

                        Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiKey= apiKey;// "your_api_key";
                         Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiSecret= apiSecret;// "your_api_secret";
                    }
                    else
                    {
                        Debug.WriteLine("数据库中没有记录！");
                    }
                }
            }
        }

        #endregion
    }
}
