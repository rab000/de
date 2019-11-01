using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
//using Tools;
using UnityEngine;

public class SQLiteHelper1
{
    static string db_path = Application.persistentDataPath + "/const.db";

    static string ConnectionString
    {      
        get
        {
            return "Data Source=" + db_path + "; Pooling=False;Cache Size=2000";

        }
    }
    /// <summary>
    /// 构造函数    
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    public SQLiteHelper1(string connectionString)
    {
        //ConnectionString = connectionString;
        //构造数据库连接
    }

    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    public SqliteDataReader ExecuteQuery(string queryString, out SqliteConnection conn, out SqliteCommand cmd)
    {
        SqliteConnection connection = new SqliteConnection(ConnectionString);
        conn = connection;
        using (cmd = new SqliteCommand(queryString, connection))
        {
            try
            {
                connection.Open();
                var r = cmd.ExecuteReader();
                return r;
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                return null;
            }
        }
    }

    /// <summary>
    /// 事务概念
    /// 事务（Transaction）是指一个或多个更改数据库的扩展。
    /// 例如，如果您正在创建一个记录或者更新一个记录或者从表中删除一个记录，
    /// 那么您正在该表上执行事务。
    /// 重要的是要控制事务以确保数据的完整性和处理数据库错误。
    /// 实际上，可以把许多的 SQLite查询联合成一组，把所有这些放在一起作为事务的一部分进行执行
    /// </summary>
    /// <param name="queryString"></param>
    public void ExecuteNonQuery(string queryString)
    {
        using (SqliteConnection conn = new SqliteConnection(ConnectionString))
        {
            conn.Open();
            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = conn;

            //开启事务
            //事务通常会持续执行下去，直到遇到下一个 COMMIT 或 ROLLBACK 命令。
            //不过在数据库关闭或发生错误时，事务处理也会回滚。
            SqliteTransaction tx = conn.BeginTransaction();
            cmd.Transaction = tx;
            try
            {
                cmd.CommandText = queryString;
                cmd.ExecuteNonQuery();
                //提交(事务执行成功)
                //COMMIT 命令是用于把事务调用的更改保存到数据库中的事务命令。
                //命令把自上次 COMMIT 或 ROLLBACK 命令以来的所有事务保存到数据库。
                tx.Commit();
                tx.Dispose();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();

            }
            catch
            {
                //回滚(事务执行失败，不提交)
                //ROLLBACK 命令是用于撤消尚未保存到数据库的事务的事务命令。
                //命令只能用于撤销自上次发出 COMMIT 或 ROLLBACK 命令以来的事务。
                tx.Rollback();
                
                throw;
            }
        }
    }


    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    //public void ReadConfig(string queryString, BaseConfig baseConfig)
    //{
    //    SqliteConnection connection = new SqliteConnection(_connectionString);

    //    SqliteCommand cmd = new SqliteCommand(queryString, connection);

    //    try
    //    {
    //        connection.Open();

    //        var r = cmd.ExecuteReader();
    //        baseConfig.InjectData(r);
    //        r.Close();
    //        cmd.Dispose();
    //        connection.Close();
    //        connection.Dispose();

    //    }
    //    catch
    //    {
    //        connection.Close();
    //        connection.Dispose();
    //        cmd.Dispose();
    //        throw;
    //    }

    //}

    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    //public void ReadConfigs<T>(string queryString, List<T> configs) where T : BaseConfig, new()
    //{
    //    using (SqliteConnection connection = new SqliteConnection(_connectionString))
    //    {
    //        using (SqliteCommand cmd = new SqliteCommand(queryString, connection))
    //        {
    //            try
    //            {
    //                connection.Open();

    //                var r = cmd.ExecuteReader();
    //                while (r.Read())
    //                {
    //                    T config = new T();
    //                    config.InjectData(r);
    //                    configs.Add(config);
    //                }
    //                r.Close();
    //                cmd.Dispose();
    //                connection.Close();
    //                connection.Dispose();
    //            }
    //            catch
    //            {
    //                connection.Close();
    //                connection.Dispose();
    //                cmd.Dispose();

    //                if (DebugLog.IsTestDebug)
    //                {
    //                    File.Delete(Application.persistentDataPath + "/const.db");
    //                    PlayerPrefs.DeleteAll();
    //                }
    //                throw;
    //            }
    //        }
    //    }

    //}

}
