//using System.Text;
//using UnityEngine;
//using Mono.Data.Sqlite;
//using System.Collections.Generic;

//public class DataMangaer// : MonoSingleton<DataMangaer>
//{

//    public static SQLiteHelper db;



//    static void InitDb()
//    {
//        var db_path = Application.persistentDataPath + "/const.db";

//        db = new SQLiteHelper1("Data Source=" + db_path + "; Pooling=False;Cache Size=2000");

//    }

//    /// <summary>
//    /// 将DB从持久化目录拷贝到沙盒
//    /// </summary>
//    public void CopyDb()
//    {
//        var src_path = Application.streamingAssetsPath + "/const.db";
//        var target_path = Application.persistentDataPath + "/const.db";
//        FileHelper.CopyFile(src_path, target_path, true);
//    }

//    public void UpdateDb()
//    {
//        ConnectDatabase();
//        Dictionary<string, string> temp_dict = new Dictionary<string, string>();
//        sql_sb.Append("SELECT * FROM LOCALDATA");
//        SqliteConnection conn;
//        SqliteCommand cmd;
//        var sql = sql_sb.ToString();
//        var reader = db.ExecuteQuery(sql, out conn, out cmd);
//        while (reader.Read())
//        {
//            temp_dict.Add(reader.GetString(0), reader.GetString(1));
//        }
//        reader.Close();
//        conn.Close();
//        cmd.Dispose();
//        conn.Dispose();
//        sql_sb.Clear();
//        CopyDb();
//        InitLocalData();
//        GameDataHelper.CreateGameDataTable();

//        foreach (var item in temp_dict)
//        {
//            SaveLocalData(item.Key, item.Value);
//        }
//        temp_dict.Clear();
//        DailyPuzzleDataHelper.ClearDailyPuzzleCompletionData(DailyPuzzleDataHelper.CurDailyPuzzleLevel + 28010001);
//    }

//    public bool connected_database { get; private set; }

//    /// <summary>
//    /// 连接数据库
//    /// </summary>
//    public void ConnectDatabase()
//    {

//        InitDb();
//        connected_database = true;
//    }
//    StringBuilder sql_sb = new StringBuilder();
//    public T GetConfigById<T>(int _Id) where T : BaseConfig, new()
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return null;
//        }
//        T config = new T();
//        var table_name = config.GetType().Name;

//        sql_sb.Append("SELECT * FROM ");
//        sql_sb.Append(table_name);
//        sql_sb.Append(" WHERE Id = ");
//        sql_sb.Append(_Id);
//        sql_sb.Append(" LIMIT 1;");
//        db.ReadConfig(sql_sb.ToString(), config);
//        sql_sb.Clear();
//        return config;
//    }

//    public List<T> GetConfigsByCondition<T>(string _Condition, object _Value) where T : BaseConfig, new()
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return null;
//        }
//        var table_name = typeof(T).Name; ;
//        sql_sb.Append("SELECT * FROM ");
//        sql_sb.Append(table_name);
//        sql_sb.Append(" WHERE ");
//        sql_sb.Append(_Condition);
//        sql_sb.Append(" = ");
//        sql_sb.Append(_Value);
//        List<T> r = new List<T>();
//        db.ReadConfigs(sql_sb.ToString(), r);
//        sql_sb.Clear();
//        return r;
//    }

//    public List<T> GetConfigsAll<T>() where T : BaseConfig, new()
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return null;
//        }
//        var table_name = typeof(T).Name; ;
//        sql_sb.Append("SELECT * FROM ");
//        sql_sb.Append(table_name);
//        List<T> r = new List<T>();
//        db.ReadConfigs<T>(sql_sb.ToString(), r);
//        sql_sb.Clear();
//        return r;
//    }
//    public int getConfigCount<T>() where T : BaseConfig, new()
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return 0;
//        }
//        var table_name = typeof(T).Name; ;
//        sql_sb.Append("SELECT COUNT(*) FROM ");
//        sql_sb.Append(table_name);
//        SqliteConnection conn;
//        SqliteCommand cmd;
//        SqliteDataReader reader = db.ExecuteQuery(sql_sb.ToString(), out conn, out cmd);
//        var count = 0;
//        while (reader.Read())
//        {
//            count = reader.GetInt32(0);
//        }
//        sql_sb.Clear();
//        reader.Close();
//        cmd.Dispose();
//        conn.Close();
//        conn.Dispose();
//        return count;
//    }
//    public void InitLocalData()
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return;
//        }
//        string sql = " CREATE TABLE IF NOT EXISTS LOCALDATA (DATA_KEY TEXT PRIMARY KEY,DATA_VALUE TEXT); ";
//        SqliteConnection conn;
//        SqliteCommand cmd;
//        db.ExecuteQuery(sql, out conn, out cmd);
//        cmd.Dispose();
//        conn.Close();
//        conn.Dispose();
//    }

//    public int GetLocalDataInt(string _Key)
//    {
//        if (_localdata_cache.ContainsKey(_Key))
//        {
//            return int.Parse(_localdata_cache[_Key].ToString());
//        }
//        else
//        {
//            SqliteConnection conn;
//            SqliteCommand cmd;
//            var reader = _GetLocalData(_Key, out conn, out cmd);
//            if (reader == null)
//            {
//                return 0;
//            }
//            if (!reader.HasRows)
//            {
//                reader.Close();
//                return 0;
//            }
//            object data = reader.GetValue(0);
//            reader.Close();
//            cmd.Dispose();
//            conn.Close();
//            conn.Dispose();
//            _localdata_cache.Add(_Key, data);
//            return int.Parse(data.ToString());
//        }
        
//    }
//    public float GetLocalDatafloat(string _Key)
//    {
//        if (_localdata_cache.ContainsKey(_Key))
//        {
//            return float.Parse(_localdata_cache[_Key].ToString());
//        }
//        else
//        {
//            SqliteConnection conn;
//            SqliteCommand cmd;
//            var reader = _GetLocalData(_Key, out conn, out cmd);
//            if (!reader.HasRows)
//            {
//                reader.Close();
//                return 0;
//            }
//            var data = reader.GetValue(0);
//            reader.Close();
//            cmd.Dispose();
//            conn.Close();
//            conn.Dispose();
//            _localdata_cache.Add(_Key, data);
//            return float.Parse(data.ToString());
//        }
//    }
//    public string GetLocalDataString(string _Key)
//    {
//        if (_localdata_cache.ContainsKey(_Key))
//        {
//            return _localdata_cache[_Key].ToString();
//        }
//        else
//        {


//            SqliteConnection conn;
//            SqliteCommand cmd;
//            var reader = _GetLocalData(_Key, out conn, out cmd);
//            if (reader == null) return "";
//            if (!reader.HasRows)
//            {
//                reader.Close();
//                return "";
//            }
//            var data = reader.GetValue(0);
//            reader.Close();
//            cmd.Dispose();
//            conn.Close();
//            conn.Dispose();
//            _localdata_cache.Add(_Key, data);
//            return data.ToString();
//        }
//    }
//    private Dictionary<string, object> _localdata_cache = new Dictionary<string, object>();
//    public void SaveLocalData(string _Key, object _Value)
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return;
//        }
//        if (_Value == null)
//        {
//            return;
//        }
//        sql_sb.Append("INSERT OR REPLACE INTO LOCALDATA (DATA_KEY,DATA_VALUE) VALUES (\"");
//        sql_sb.Append(_Key);
//        sql_sb.Append("\",\"");
//        sql_sb.Append(_Value.ToString());
//        sql_sb.Append("\");");
//        db.ExecuteNonQuery(sql_sb.ToString());
//        sql_sb.Clear();
//        if (_localdata_cache.ContainsKey(_Key))
//        {
//            _localdata_cache[_Key] = _Value;
//        }
//        else
//        {
//            _localdata_cache.Add(_Key, _Value);
//        }
//    }

//    private SqliteDataReader _GetLocalData(string _Key, out SqliteConnection conn, out SqliteCommand cmd)
//    {


//        conn = null;
//        cmd = null;
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return null;
//        }

//        sql_sb.Append("SELECT DATA_VALUE FROM LOCALDATA WHERE DATA_KEY = \"");
//        sql_sb.Append(_Key);
//        sql_sb.Append("\" LIMIT 1");
//        var sql = sql_sb.ToString();
//        var reader = db.ExecuteQuery(sql, out conn, out cmd);
//        sql_sb.Clear();

//        return reader;
//    }
//    /// <summary>
//    /// 是否有这张表
//    /// </summary>
//    /// <param name="_TableName"></param>
//    /// <returns></returns>
//    public bool IsExistTable(string _TableName)
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return false;
//        }
//        else
//        {
//            string sql = "select count(*)  from sqlite_master where type = 'table' and name = '" + _TableName + "';";
//            SqliteConnection conn;
//            SqliteCommand cmd;
//            SqliteDataReader reader = db.ExecuteQuery(sql, out conn, out cmd);
//            int count = 0;
//            while (reader.Read())
//            {
//                count = reader.GetInt32(0);
//            }
//            reader.Close();
//            cmd.Dispose();
//            conn.Close();
//            conn.Dispose();
//            return count != 0;
//        }
//    }

//    public bool IsExistId(string _TableName, int _Id)
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return false;
//        }
//        var table_name = _TableName;
//        sql_sb.Append("SELECT COUNT(*) FROM ");
//        sql_sb.Append(table_name);
//        sql_sb.Append(" WHERE Id = ");
//        sql_sb.Append(_Id);
//        SqliteConnection conn;
//        SqliteCommand cmd;
//        SqliteDataReader reader = db.ExecuteQuery(sql_sb.ToString(), out conn, out cmd);
//        var count = 0;
//        while (reader.Read())
//        {
//            count = reader.GetInt32(0);
//        }
//        sql_sb.Clear();
//        cmd.Dispose();
//        conn.Close();
//        conn.Dispose();
//        reader.Close();
//        return count != 0;
//    }
//    /// <summary>
//    /// 执行一条sql,不会返回数据
//    /// </summary>
//    /// <param name="sql"></param>
//    public void ExecuteSql(string sql)
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return;
//        }
//        db.ExecuteNonQuery(sql);
//    }

//    /// <summary>
//    /// 执行一条sql,不会返回数据
//    /// </summary>
//    /// <param name="sql"></param>
//    public void UpdateSql(string sql)
//    {
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return;
//        }
//        db.ExecuteNonQuery(sql);
//    }

//    /// <summary>
//    /// 注意用完关闭reader!!
//    /// 注意用完关闭reader!!
//    /// 注意用完关闭reader!!
//    /// </summary>
//    /// <param name="sql"></param>
//    /// <returns></returns>
//    public SqliteDataReader GetReaderBySql(string sql, out SqliteConnection conn, out SqliteCommand cmd)
//    {
//        conn = null;
//        cmd = null;
//        if (!connected_database)
//        {
//            Debug.LogError("还没有初始化数据库");
//            return null;
//        }
//        var reader = db.ExecuteQuery(sql, out conn, out cmd);
//        return reader;
//    }

//}
