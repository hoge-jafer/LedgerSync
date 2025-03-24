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

        public string SQLiteDBCreateTradeListSQL = @"CREATE TABLE IF NOT EXISTS TradeList (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,  
    TradeListID VARCHAR(255) NOT NULL,    
    Symbol VARCHAR(255) NOT NULL,          
    IsBuyers VARCHAR(255) NOT NULL,        
    Price VARCHAR(255) NOT NULL,           
    QTY VARCHAR(255) NOT NULL,             
    Time VARCHAR(255) NOT NULL );";

        public string sql = @"";

        public string SQLiteDBCreateCoinSQL = @"CREATE TABLE IF NOT EXISTS Coin (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,  
    Free VARCHAR(255) NOT NULL,            
    Asset VARCHAR(255) NOT NULL );";

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
            ShellModels.CoinVisibility = Visibility.Collapsed;
            ShellModels.SecretKeyVisibility = Visibility.Collapsed;
            ShellModels.NavigationContent = "/UI/MenuView.xaml";
            ShellModels.NavigationSecretKey = "/UI/MenuView.xaml";
            bool isNewDatabase = !File.Exists(SQLiteDBPath);
            if (isNewDatabase)
            {
                //SQLiteConnection.CreateFile(SQLiteDBPath);
                Debug.WriteLine("数据库文件不存在，已创建 LedgerSync.db！");
                CreateSecretKey();
                CreateTradeList();
                CreateCoin();
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
            ShellModels.CoinVisibility = Visibility.Collapsed;
            ShellModels.SecretKeyVisibility = Visibility.Visible;
            ShellModels.NavigationContent = "/UI/MenuView.xaml";
            ShellModels.NavigationSecretKey = "/UI/SecretKeyView.xaml";
        }


        [RelayCommand]
        public async void TradeData()
        {
            ShellModels.CoinVisibility = Visibility.Visible;
            ShellModels.SecretKeyVisibility = Visibility.Collapsed;
            ShellModels.NavigationContent = "/UI/TradeDataView.xaml";
            ShellModels.NavigationSecretKey = "/UI/MenuView.xaml";
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

        public void CreateTradeList()
        {
            using (var db = new SQLiteHelper(Ioc.Default.GetService<ShellViewModel>().SQLiteDBPath))
            {
                // 创建表
                db.ExecuteNonQuery(SQLiteDBCreateTradeListSQL);

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

        public void CreateCoin()
        {
            using (var db = new SQLiteHelper(Ioc.Default.GetService<ShellViewModel>().SQLiteDBPath))
            {
                // 创建表
                db.ExecuteNonQuery(SQLiteDBCreateCoinSQL);

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

        public void InsertTradeList(string TradeListID,string Symbol,string IsBuyers,string Price,string QTY,string Time)
        {
            using (SQLiteConnection conn = new SQLiteConnection(SQLiteDBPath))
            {
                conn.Open();
                string insertQuery = @"INSERT INTO TradeList (TradeListID, Symbol, IsBuyers, Price, QTY, Time) 
                                   VALUES (@TradeListID, @Symbol, @IsBuyers, @Price, @QTY, @Time);";

                using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TradeListID", TradeListID);
                    cmd.Parameters.AddWithValue("@Symbol", Symbol);
                    cmd.Parameters.AddWithValue("@IsBuyers", IsBuyers);
                    cmd.Parameters.AddWithValue("@Price", Price);
                    cmd.Parameters.AddWithValue("@QTY", QTY);
                    cmd.Parameters.AddWithValue("@Time", Time);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Debug.WriteLine($"插入TradeList成功，影响行数：{rowsAffected}");
                }
            }
        }

        public string QueryTradeList(string TradeListID)
        {
            string isHave = "";
            using (SQLiteConnection conn = new SQLiteConnection(SQLiteDBPath))
            {
                conn.Open();
                string query = "SELECT * FROM TradeList WHERE TradeListID = @TradeListID";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TradeListID", TradeListID); // 查询 BTCUSDT 交易记录

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())  // 逐行读取数据
                        {                          
                            Debug.WriteLine($"ID: {reader["ID"]}, TradeListID: {reader["TradeListID"]}, Symbol: {reader["Symbol"]}, " +$"IsBuyers: {reader["IsBuyers"]}, Price: {reader["Price"]}, QTY: {reader["QTY"]}, Time: {reader["Time"]}");
                             isHave=reader["ID"].ToString();
                        }
                    }
                }
            }
            return isHave;
        }

        public void InsertCoin(string Free,string Asset)
        {
            using (SQLiteConnection conn = new SQLiteConnection(SQLiteDBPath))
            {
                conn.Open();
                string insertQuery = @"INSERT INTO Coin (Free, Asset) VALUES (@Free, @Asset);";

                using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Free", Free);  // 资产数量
                    cmd.Parameters.AddWithValue("@Asset", Asset); // 资产名称

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Debug.WriteLine($"插入Coin成功，影响行数：{rowsAffected}");
                }
            }
        }

        public string QueryCoin(string Asset)
        {
            string isHave = "";
            using (SQLiteConnection conn = new SQLiteConnection(SQLiteDBPath))
            {
                conn.Open();
                string query = "SELECT * FROM Coin WHERE Asset = @Asset";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())  // 逐行读取数据
                        {
                            Debug.WriteLine($"ID: {reader["ID"]}, Asset: {reader["Asset"]}, Free: {reader["Free"]}");
                            isHave = reader["ID"].ToString();
                        }
                    }
                }
            }
            return isHave;
        }

        #endregion
    }
}
