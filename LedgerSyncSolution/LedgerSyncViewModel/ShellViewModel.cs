using Binance.Spot;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using LedgerSyncModel;
using LedgerSyncModel.Entity;
using LedgerSyncViewModel.Helper;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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


            ShellModels.ObservableCollectionYear.Add("2017");
            ShellModels.ObservableCollectionYear.Add("2018");
            ShellModels.ObservableCollectionYear.Add("2019");
            ShellModels.ObservableCollectionYear.Add("2020");
            ShellModels.ObservableCollectionYear.Add("2021");
            ShellModels.ObservableCollectionYear.Add("2022");
            ShellModels.ObservableCollectionYear.Add("2023");
            ShellModels.ObservableCollectionYear.Add("2024");
            ShellModels.ObservableCollectionYear.Add("2025");

            ShellModels.ItemYear = ShellModels.ObservableCollectionYear[ShellModels.ObservableCollectionYear.Count-1];
            GlobalTradeListEntities = new ObservableCollection<TradeListEntity>();
        }
        public SpotAccountTrade tradingAccountTrade;
        public Wallet wallet;

       public ObservableCollection<TradeListEntity> GlobalTradeListEntities;

        public int pageNumber=1;

        Window window;
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
Year VARCHAR(255) NOT NULL, 
Month VARCHAR(255) NOT NULL, 
    Time VARCHAR(255) NOT NULL );";


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

        public string selectCoin = "";

        [ObservableProperty]
        private ShellModel shellModels;

        [RelayCommand]
        public async void ShellViewLoad(FrameworkElement frameworkElement)
        {
            window = (Window)frameworkElement;
            window.MouseLeftButtonDown += delegate { window.DragMove(); };
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
        public void SelectCoin(string content)
        {
            selectCoin = content;
            Ioc.Default.GetService<TradeDataViewModel>().GetTradeListData(content);
        }

        [RelayCommand]
        public void QueryTradeListSymbol(string Symbol)
        {
            string isHave = "";
            selectCoin = Symbol;
            //var newSymbol = Symbol + "USDT";
            //Symbol = newSymbol;
            Ioc.Default.GetService<TradeDataViewModel>().TradeDataModels.ObservableCollectionTradeListEntity.Clear();
            GlobalTradeListEntities.Clear();
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={SQLiteDBPath};Version=3;"))
            {
                conn.Open();
                string query = "SELECT * FROM TradeList WHERE Symbol=@Symbol ";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    //cmd.Parameters.AddWithValue("@Year", Year); // 查询 BTCUSDT 交易记录
                    //cmd.Parameters.AddWithValue("@Month", Month); // 查询 BTCUSDT 交易记录
                    cmd.Parameters.AddWithValue("@Symbol", Symbol + "USDT");
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())  // 逐行读取数据
                        {
                            Debug.WriteLine($"ID: {reader["ID"]}, TradeListID: {reader["TradeListID"]}, Symbol: {reader["Symbol"]}, " + $"IsBuyers: {reader["IsBuyers"]}, Price: {reader["Price"]}, QTY: {reader["QTY"]}, Time: {reader["Time"]}");
                            //isHave = reader["ID"].ToString();

                            TradeListEntity tradeListEntity = new TradeListEntity();
                            tradeListEntity.TradeListID = reader["TradeListID"].ToString();
                            tradeListEntity.Symbol = reader["Symbol"].ToString();
                            tradeListEntity.IsBuyers = reader["IsBuyers"].ToString();
                            tradeListEntity.Price = reader["Price"].ToString();
                            tradeListEntity.QTY = reader["QTY"].ToString();
                            tradeListEntity.Year = reader["Year"].ToString();
                            tradeListEntity.Month = reader["Month"].ToString();
                            tradeListEntity.Time = reader["Time"].ToString();
                            GlobalTradeListEntities.Add(tradeListEntity);
                            //Ioc.Default.GetService<TradeDataViewModel>().TradeDataModels.ObservableCollectionTradeListEntity.Add(tradeListEntity);

                        }
                    }
                }
            }

            //获取前20条记录。
            Ioc.Default.GetService<TradeDataViewModel>().TradeDataModels.ObservableCollectionTradeListEntity = new ObservableCollection<TradeListEntity>(GlobalTradeListEntities.Take(20));

            //return isHave;
        }

        [RelayCommand]
        public void QueryTradeListMonth(string month)
        {
            if (!string.IsNullOrEmpty(selectCoin)) 
            {
                QueryTradeListYearMonth(ShellModels.ItemYear, month, selectCoin);
            }

        }

        [RelayCommand]
        public void SyncDataLocal()
        {
            //Ioc.Default.GetService<TradeDataViewModel>().Print();
            //Ioc.Default.GetService<ShellViewModel>().ShellModels.ObservableCollectionCoinEntity = TradeDataModels.ObservableCollectionCoinEntity;
            Ioc.Default.GetService<TradeDataViewModel>().SyncData();
        }
        //Analyze
        [RelayCommand]
        public void AnalyzeTradeList()
        {

        }

        [RelayCommand]
        public void Print()
        {
            Ioc.Default.GetService<TradeDataViewModel>().Print();
        }

        #region ExitSystem

        [RelayCommand]
        public void PreviousContent()
        {

            var pagedData = GlobalTradeListEntities.Skip((pageNumber - 1) * 20).Take(20).ToList();
            pageNumber--;
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

        }

        #endregion

        #region ExitSystem

        [RelayCommand]
        public void NextContent()
        {
            var pagedData = GlobalTradeListEntities.Skip((pageNumber + 1) * 20).Take(20).ToList();
            pageNumber++;
            if (pagedData.Count ==0) 
            {
                pageNumber--;
            }
        }

        #endregion

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

                        Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiKey = apiKey;// "your_api_key";
                        Ioc.Default.GetService<SecretKeyViewModel>().SecretKeyModels.ApiSecret = apiSecret;// "your_api_secret";
                    }
                    else
                    {
                        Debug.WriteLine("数据库中没有记录！");
                    }
                }
            }
        }

        public void InsertTradeList(string TradeListID, string Symbol, string IsBuyers, string Price, string QTY, string Year, string Month, string Time)
        {
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={SQLiteDBPath};Version=3;"))
            {
                conn.Open();
                string insertQuery = @"INSERT INTO TradeList (TradeListID, Symbol, IsBuyers, Price, QTY,Year,Month,Time) 
                                   VALUES (@TradeListID, @Symbol, @IsBuyers, @Price, @QTY,@Year,@Month,@Time);";

                using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TradeListID", TradeListID);
                    cmd.Parameters.AddWithValue("@Symbol", Symbol);
                    cmd.Parameters.AddWithValue("@IsBuyers", IsBuyers);
                    cmd.Parameters.AddWithValue("@Price", Price);
                    cmd.Parameters.AddWithValue("@QTY", QTY);
                    cmd.Parameters.AddWithValue("@Year", Year);
                    cmd.Parameters.AddWithValue("@Month", Month);
                    cmd.Parameters.AddWithValue("@Time", Time);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Debug.WriteLine($"插入TradeList成功，影响行数：{rowsAffected}");
                }
            }
        }

        public string QueryTradeList(string TradeListID)
        {
            string isHave = "";
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={SQLiteDBPath};Version=3;"))
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
                            Debug.WriteLine($"ID: {reader["ID"]}, TradeListID: {reader["TradeListID"]}, Symbol: {reader["Symbol"]}, " + $"IsBuyers: {reader["IsBuyers"]}, Price: {reader["Price"]}, QTY: {reader["QTY"]}, Time: {reader["Time"]}");
                            isHave = reader["ID"].ToString();
                        }
                    }
                }
            }
            return isHave;
        }

        public void QueryTradeListYearMonth(string Year,string Month,string Symbol)
        {
            string isHave = "";
            Ioc.Default.GetService<TradeDataViewModel>().TradeDataModels.ObservableCollectionTradeListEntity.Clear();
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={SQLiteDBPath};Version=3;"))
            {
                conn.Open();
                string query = "SELECT * FROM TradeList WHERE Year = @Year AND Month = @Month AND Symbol=@Symbol ";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", Year); // 查询 BTCUSDT 交易记录
                    cmd.Parameters.AddWithValue("@Month", Month); // 查询 BTCUSDT 交易记录
                    cmd.Parameters.AddWithValue("@Symbol", Symbol+"USDT");
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())  // 逐行读取数据
                        {
                            Debug.WriteLine($"ID: {reader["ID"]}, TradeListID: {reader["TradeListID"]}, Symbol: {reader["Symbol"]}, " + $"IsBuyers: {reader["IsBuyers"]}, Price: {reader["Price"]}, QTY: {reader["QTY"]}, Time: {reader["Time"]}");
                            //isHave = reader["ID"].ToString();

                            TradeListEntity tradeListEntity = new TradeListEntity();
                            tradeListEntity.TradeListID = reader["TradeListID"].ToString();
                            tradeListEntity.Symbol = reader["Symbol"].ToString();
                            tradeListEntity.IsBuyers = reader["IsBuyers"].ToString();
                            tradeListEntity.Price = reader["Price"].ToString();
                            tradeListEntity.QTY = reader["QTY"].ToString();
                            tradeListEntity.Year = reader["Year"].ToString();
                            tradeListEntity.Month = reader["Month"].ToString();
                            tradeListEntity.Time = reader["Time"].ToString();
                            Ioc.Default.GetService<TradeDataViewModel>().TradeDataModels.ObservableCollectionTradeListEntity.Add(tradeListEntity);

                        }
                    }
                }
            }
            //return isHave;
        }



        public void InsertCoin(string Free, string Asset)
        {
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={SQLiteDBPath};Version=3;"))
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
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={SQLiteDBPath};Version=3;"))
            {
                conn.Open();
                string query = "SELECT * FROM Coin WHERE Asset = @Asset";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@Asset", Asset); // 查询 BTCUSDT 交易记录

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
