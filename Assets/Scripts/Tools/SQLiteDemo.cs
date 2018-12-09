using UnityEngine;
using System.Collections;
using System.IO;
using Mono.Data.Sqlite;

public class SQLiteDemo : MonoBehaviour 
{
	/// <summary>
	/// SQLite数据库辅助类
	/// </summary>
	private SQLiteHelper sql;

	void Start () 
	{
		//创建名为sqlite4unity的数据库
		//sql = new SQLiteHelper("data source= data/sqlite4unity.db");
		if(true)return;

		sql = SQLiteHelper.GetIns ();

		sql.OpenConnection("data source= data/sqlite4unity.db");

		//创建名为table1的数据表
		sql.CreateTable("table1",new string[]{"ID","Name","Age","Email"},new string[]{"INTEGER PRIMARY KEY","TEXT","INTEGER","TEXT"});

		//插入两条数据
		sql.InsertValues("table1",new string[]{"null","'张三'","'22'","'Zhang3@163.com'"});
		sql.InsertValues("table1",new string[]{"null","'李四'","'25'","'Li4@163.com'"});

		//更新数据，将Name="张三"的记录中的Name改为"Zhang3"
		//sql.UpdateValues("table1", new string[]{"Name"}, new string[]{"'Zhang3'"}, "Name", "=", "'张三'");

		//插入3条数据
		sql.InsertValues("table1",new string[]{"null","'王五1'","25","'Wang5@163.com'"});
		sql.InsertValues("table1",new string[]{"null","'王五2'","26","'Wang5@163.com'"});


		sql.InsertValues("table1",new string[]{"null","'王五3'","27","'Wang5@163.com'"});


		//sql.InsertIntoSpecific("table1",new string[]{"Name","Age","Email"},new string[]{"'王八'","25","'WangBa'"});

		//删除Name="王五"且Age=26的记录,DeleteValuesOR方法类似
		sql.DeleteValuesAND("table1", new string[]{"Name","Age"}, new string[]{"=","="}, new string[]{"'王五2'","'26'"});





		sql.InsertValues("table1",new string[]{"null","'999'","21","'Wang5@163.com'"});

		string s = "select last_insert_rowid() from table1";
		var r = sql.ExecuteQuery (s);
		while (r.Read ()) {
			Debug.Log ("nafio--->"+r.GetInt32(0));
			//Debug.Log (r.GetOrdinal("ID"));
			//Debug.Log(r.GetInt32(r.GetOrdinal("ID")));
		}

		//读取整张表
		SqliteDataReader reader = sql.ReadFullTable ("table1");
		while(reader.Read()) 
		{
			//读取ID
			Debug.Log(reader.GetInt32(reader.GetOrdinal("ID")));
			//读取Name
			Debug.Log(reader.GetString(reader.GetOrdinal("Name")));
			//读取Age
			Debug.Log(reader.GetInt32(reader.GetOrdinal("Age")));
			//读取Email
			Debug.Log(reader.GetString(reader.GetOrdinal("Email")));
		}

		//读取数据表中Age>=25的所有记录的ID和Name
		reader = sql.ReadTable ("table1", new string[]{"ID","Name"}, new string[]{"Age"}, new string[]{">="}, new string[]{"'25'"});

		while(reader.Read()) 
		{
			//读取ID
			Debug.Log(reader.GetInt32(reader.GetOrdinal("ID")));
			//读取Name
			Debug.Log(reader.GetString(reader.GetOrdinal("Name")));
		}

		//自定义SQL,删除数据表中所有Name="王五"的记录
		sql.ExecuteQuery("DELETE FROM table1 WHERE NAME='王五'");



		//关闭数据库连接
		sql.CloseConnection();
	}

	void Update()
	{
//		if (Input.GetKeyUp (KeyCode.B)) {
//			sql.OpenConnection("data source= data/sqlite4unity.db");
//			sql.BeTableExist ("table1");
//			sql.CloseConnection();
//		}

		if (Input.GetKeyUp (KeyCode.S))
		{
			Debug.Log ("Start");
			sql = SQLiteHelper.GetIns ();
			sql.OpenConnection("data source= data/test.db");
		}

		if (Input.GetKeyUp (KeyCode.T)) {
			//创建名为table1的数据表
			sql.CreateTable("table1",new string[]{"ID","Name","Email"},new string[]{"INTEGER PRIMARY KEY","TEXT","TEXT"});
		}


		if (Input.GetKeyUp (KeyCode.N))
		{
			Debug.Log ("End");
			sql.CloseConnection ();
		}


		//测试新增一条数据
		if (Input.GetKeyUp (KeyCode.C)) 
		{
			Debug.Log ("Create");

			string s = "name3333333";

			GEditorDataMgr.CreateKVSqlData("table1",new string[]{"null",s,"a1@126.com"});

		}

		//测试查询
		if (Input.GetKeyUp (KeyCode.Q)) 
		{
			Debug.Log ("Query");
			//GEditorDataMgr.QueryKVSqlData("table1",new string[]{"ID","Name","Email"},1);

		}

		//测试修改
		if (Input.GetKeyUp (KeyCode.M)) 
		{
			Debug.Log ("Motify");
			GEditorDataMgr.ModifyKVSqlData("table1",new string[]{"Name"},new string[]{"'fuck'"},1);
		}

		//测试删除
		if (Input.GetKeyUp (KeyCode.D)) 
		{
			Debug.Log ("Del");
			GEditorDataMgr.DelKVSqlData ("table1",1);
		}

		//测试copy
//		if(Input.GetKeyUp(KeyCode.P))
//		{
//			Debug.Log ("Copy");
//			string s = "INSERT INTO table1 SELECT * FROM table1";
//			SQLiteHelper.GetIns ().ExecuteQuery (s);
//		}
	}

}