
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerSyncViewModel.Helper
{
    public class SQLiteHelper : IDisposable
    {
        private readonly string _connectionString;
        private SQLiteConnection _connection;

        public SQLiteHelper(string databasePath)
        {
            _connectionString = $"Data Source={databasePath};Version=3;";
            _connection = new SQLiteConnection(_connectionString);
            _connection.Open();
        }

        // 执行非查询SQL语句（INSERT, UPDATE, DELETE）
        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            using (var cmd = new SQLiteCommand(query, _connection))
            {
                AddParameters(cmd, parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        // 执行查询SQL语句（返回单个值）
        public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            using (var cmd = new SQLiteCommand(query, _connection))
            {
                AddParameters(cmd, parameters);
                return cmd.ExecuteScalar();
            }
        }

        // 执行查询SQL语句（返回 DataTable）
        public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            using (var cmd = new SQLiteCommand(query, _connection))
            {
                AddParameters(cmd, parameters);
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // 添加参数到命令
        private void AddParameters(SQLiteCommand cmd, Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
        }

        // 事务处理
        public void ExecuteTransaction(List<string> queries)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                using (var cmd = new SQLiteCommand(_connection))
                {
                    try
                    {
                        foreach (var query in queries)
                        {
                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 释放资源
        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
