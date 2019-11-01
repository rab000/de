using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System;
using System.Text;

/// <summary>
/// 这个类只跟sql相关，不要做其他跟非sql逻辑相关的操作
/// </summary>
public class SQLiteHelper
{
	private bool BeShowLog = true;

    #region base

    static string _connectionString;

    private static StringBuilder SB = new StringBuilder();

    /// <summary>
    /// 数据库连接定义
    /// </summary>
    private SqliteConnection dbConnection;

    /// <summary>
    /// SQL命令定义
    /// </summary>
    private SqliteCommand dbCommand;

    /// <summary>
    /// 数据读取定义
    /// </summary>
    private SqliteDataReader dataReader;

    private static SQLiteHelper Ins;
    public static SQLiteHelper GetIns()
    {
        if (null == Ins) Ins = new SQLiteHelper();
        return Ins;
    }

    private SQLiteHelper() { }

    /// <summary>
    /// 打开连接
    /// 这里注意下打开连接格式和位置
    /// "data source= Assets/sqlite4unity.db"   在Assets根目录下创建名为sqlite4unity.db的库
    /// 
    /// 
    /// </summary>
    /// <param name="connectionString">Connection string.</param>
    public void OpenConnection(string connectionString)
    {
        try
        {
            //构造数据库连接
            dbConnection = new SqliteConnection(connectionString);
            //打开数据库
            dbConnection.Open();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public void CloseConnection()
    {
        //销毁Command
        if (dbCommand != null)
        {
            dbCommand.Cancel();
        }
        dbCommand = null;

        //销毁Reader
        if (dataReader != null)
        {
            dataReader.Close();
        }
        dataReader = null;

        //销毁Connection
        if (dbConnection != null)
        {
            dbConnection.Close();
            dbConnection.Dispose();
        }
        dbConnection = null;

    }
    
    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    public SqliteDataReader ExecuteQuery(string queryString)
    {
        dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = queryString;
        dataReader = dbCommand.ExecuteReader();
        return dataReader;
    }

    #endregion

    #region table operate

    /// <summary>
    /// 创建数据表
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">数据表名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
    public SqliteDataReader CreateTable(string tableName, string[] colNames, string[] colTypes)
    {
        string queryString = "CREATE TABLE IF NOT EXISTS " + tableName + "( " + colNames[0] + " " + colTypes[0];
        for (int i = 1; i < colNames.Length; i++)
        {
            //Debug.Log("-->i"+i+"clen:"+ colNames.Length+" tlen:"+ colTypes.Length);
            queryString += ", " + colNames[i] + " " + colTypes[i];
        }
        queryString += "  ) ";

        //Debug.Log ("-->"+queryString);
        return ExecuteQuery(queryString);
    }

    /// <summary>
	/// nafio info 暂时找不到好的方法查询表是否存在
	/// 用下面的方法查表里所有数据，如果表不存在会抛出异常
	/// </summary>
	/// <returns><c>true</c>, if table exist was been, <c>false</c> otherwise.</returns>
	/// <param name="tableName">Table name.</param>
	public bool BeTableExist(string tableName)
    {
        //string queryString = "SELECT * FROM sqlite_master where type = 'table' and 'name = " + tableName+"'";
        string queryString = "SELECT * FROM " + tableName;

        SqliteDataReader reader = null;
        try
        {
            reader = ExecuteQuery(queryString);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    /// <summary>
	/// 读取整张数据表
	/// </summary>
	/// <returns>The full table.</returns>
	/// <param name="tableName">数据表名称</param>
	public SqliteDataReader ReadFullTable(string tableName)
    {
        string queryString = "SELECT * FROM " + tableName;
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// Reads the table.
    /// </summary>
    /// <returns>The table.</returns>
    /// <param name="tableName">Table name.</param>
    /// <param name="items">Items.</param>
    /// <param name="colNames">Col names.</param>
    /// <param name="operations">Operations.</param>
    /// <param name="colValues">Col values.</param>
    public SqliteDataReader ReadTable(string tableName, string[] items, string[] colNames, string[] operations, string[] colValues)
    {
        string queryString = "SELECT " + items[0];
        for (int i = 1; i < items.Length; i++)
        {
            queryString += ", " + items[i];
        }
        queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " + operations[0] + " " + colValues[0];
        for (int i = 0; i < colNames.Length; i++)
        {
            queryString += " AND " + colNames[i] + " " + operations[i] + " " + colValues[0] + " ";
        }
        return ExecuteQuery(queryString);
    }

    #endregion

    #region add modify del

    /// <summary>
    /// 查询最后一个自增长的数据的id
    /// </summary>
    /// <returns>The last increse I.</returns>
    /// <param name="tableName">Table name.</param>
    public SqliteDataReader ReadLastIncreseID(string tableName)
    {
        string s = "select last_insert_rowid() from " + tableName;
        return ExecuteQuery(s);
    }

    /// <summary>
    /// 向指定数据表中插入数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="values">插入的数值</param>
    public SqliteDataReader InsertValues(string tableName, string[] values)
    {
        //获取数据表中字段数目
        int fieldCount = ReadFullTable(tableName).FieldCount;
        //当插入的数据长度不等于字段数目时引发异常
        if (values.Length != fieldCount)
        {
            throw new SqliteException("values.Length!=fieldCount len:" + values.Length + " fieldCount:" + fieldCount);
        }
        //"INSERT OR REPLACE INTO"
        string queryString = "INSERT INTO " + tableName + " VALUES (" + values[0];
        for (int i = 1; i < values.Length; i++)
        {
            queryString += ", " + values[i];
        }
        queryString += " )";

        Log.i("SQLiteHelper", "InsertValues", "sql str:" + queryString, BeShowLog);

        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 向指定列插入数据
    /// 比如这里跳过主键直接插入数据，让主键自增长
    /// 测试过不起作用
    /// </summary>
    /// <returns>The into specific.</returns>
    /// <param name="tableName">Table name.</param>
    /// <param name="colnames">Colnames.</param>
    /// <param name="values">Values.</param>
    //	public SqliteDataReader InsertIntoSpecific(string tableName, string[] colnames, string[] values)  
    //	{  
    //		string commPath = "INSERT INTO " + tableName + "(" + colnames[0];  
    //		for (int i = 1; i < colnames.Length; i++)  
    //		{  
    //			commPath += "," + colnames[i];  
    //		}  
    //		commPath += ") VALUES (" + values[0];  
    //		for (int i = 1; i < values.Length; i++)  
    //		{  
    //			commPath += "," + values[i];  
    //		}  
    //
    //		return ExecuteQuery(commPath);  
    //
    //	}  

    /// <summary>
    /// 更新指定数据表内的某一行或某几行数据
    /// eg:
    /// SQLiteHelper.GetIns().UpdateValues(tableName, new string[]{"Name"}, 	new string[]{"'Zhang3'"}, 	"Name", "=", "'张三'");
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    /// <param name="key">关键字</param>
    /// <param name="value">关键字对应的值</param>
    public SqliteDataReader UpdateValues(string tableName, string[] colNames, string[] colValues, string key, string operation, string value)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length");
        }

        string queryString = "UPDATE " + tableName + " SET " + colNames[0] + "=" + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += ", " + colNames[i] + "=" + colValues[i];
        }
        queryString += " WHERE " + key + operation + value;
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public SqliteDataReader DeleteValuesOR(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += "OR " + colNames[i] + operations[0] + colValues[i];
        }
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public SqliteDataReader DeleteValuesAND(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += " AND " + colNames[i] + operations[i] + colValues[i];
        }
        return ExecuteQuery(queryString);
    }

    #endregion

    #region serch

    /// <summary>
    /// 表中是否存在id
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsExistId(string tableName, int id)
    {
        //if (!connected_database)
        //{
        //    Debug.LogError("还没有初始化数据库");
        //    return false;
        //}

        var table_name = tableName;
        SB.Append("SELECT COUNT(*) FROM ");
        SB.Append(table_name);
        SB.Append(" WHERE Id = ");
        SB.Append(id);
        SqliteConnection conn;
        SqliteCommand cmd;
        SqliteDataReader reader = ExecuteQuery(SB.ToString());
        var count = 0;
        while (reader.Read())
        {
            count = reader.GetInt32(0);
        }
        SB.Clear();        
        return count != 0;
    }

    #endregion

    #region 不保持连接，用完即关闭

    /// <summary>
    /// 连接sql,执行SQL命令,返回查询结果,然后立刻关闭连接
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    public SqliteDataReader ExecuteQueryOnce(string queryString, out SqliteConnection conn, out SqliteCommand cmd)
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
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
    /// 连接sql,执行SQL命令,不返回查询结果,然后立刻关闭连接
    /// 事务概念
    /// 事务（Transaction）是指一个或多个更改数据库的扩展。
    /// 例如，如果您正在创建一个记录或者更新一个记录或者从表中删除一个记录，
    /// 那么您正在该表上执行事务。
    /// 重要的是要控制事务以确保数据的完整性和处理数据库错误。
    /// 实际上，可以把许多的 SQLite查询联合成一组，把所有这些放在一起作为事务的一部分进行执行
    /// </summary>
    /// <param name="queryString"></param>
    public void ExecuteNonQueryOnce(string queryString)
    {
        using (SqliteConnection conn = new SqliteConnection(_connectionString))
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

    #endregion

    /// <summary>
    /// 将DB从持久化目录拷贝到沙盒
    /// </summary>
    public void CopyDb()
    {
        var src_path = Application.streamingAssetsPath + "/const.db";
        var target_path = Application.persistentDataPath + "/const.db";
        FileHelper.CopyFile(src_path, target_path, true);
    }

}