using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System;

/// <summary>
/// 相对于SQLiteHelper
/// SQLiteHelper4DataEditor用来处理跟编辑器使用sql相关的逻辑，是sql特殊化使用处理
/// 而SQLiteHelper只处理sql操作相关，保证通用型
/// 
/// 
/// 数据库设计说明
/// 结构如下
/// line0 line1 line2  line3  line4  line5 line6 line7 line8 ........line447 line448 line449
/// key   value type   key    value  type  key   value type  ........key     value   type
/// key   value type   key    value  type  key   value type  ........key     value   type
/// key   value type   key    value  type  key   value type  ........key     value   type
/// 特别注意一点line0 的key永远是ID type是不可更改的自增长int类型
/// 除了这个其他都是任意组合即可，并且内容都用字符串
/// 
/// 设置设计的原因
/// 从游戏数据需求上来说，以技能数据为例，每个技能数据(比如主动技能，被动技能)并不完全是一样的，这就要求生成长短不一，数据项代表意义不同的数据
/// 对于sql来说，不方便做长短不一的存储，所以这里sql取最大长度，段的数据也存到这里，允许有空数据，sql是在pc端，不用过分考虑性能问题
/// 
/// 设计的几个重点要素说明
/// sql:结构上面有描述
/// ui显示:显示部分保证跟sql一模一样，没数据的地方就添默认数据
/// 导出的最终数据:要求剔除冗余数据，生成干净数据，跟game中的模型一一对应 
/// </summary>
public class SQLiteHelper4DataEditor{

	static bool BeShowLog = true;

    //nsql
    /// <summary>
    /// sql里面一行最大数据
    /// (key+value+type)*150 = 450
    /// 注意这个数必须是3的倍数，否则会出问题比如KVContainer.CheckAndLoadTreeItemData2Local中
    /// </summary>
    //public const int MAX_NUM = 450;
    public static int GetMaxSqlDataNum(string editorType) {
        int MAX_NUM = 0;
        switch (editorType) {
            case "skill":
                MAX_NUM = ActiveSkillDataTemplate.GetMaxSqlDataNum();
                break;
            case "item":
                MAX_NUM = ItemDataTemplate.GetMaxSqlDataNum();
                break;
            case "task":
                MAX_NUM = TaskDataTemplate.GetMaxSqlDataNum();
                break;
            default:
                Log.e("SQLiteHelper4DataEditor", "GetMaxSqlDataNum","未知的编辑器类型editorType:"+editorType,BeShowLog);
                break;
        }

        return MAX_NUM;
    }
	/// <summary>
	/// sql固定(自增长int型)主键名称
	/// </summary>
	public const string PRIMARY_NAME = "line1";

	/// <summary>
	/// sql中的字段名统一使用line0~line449
	/// </summary>
	//private static string[] _Col_Names_InSql;

    public static string[] Get_Col_Names_InSql(string editorType)
    {
        
         int MAX_NUM = GetMaxSqlDataNum(editorType);
         string[] _Col_Names_InSql  = new string[MAX_NUM];
         for (int i = 0; i < MAX_NUM; i++){
             _Col_Names_InSql[i] = "line" + i;
         }
       
		return _Col_Names_InSql;
    }

 //   public static string[] Col_Names_InSql{
	//	get{
	//		if (null == _Col_Names_InSql) {

 //               _Col_Names_InSql = new string[MAX_NUM];
	//			for (int i = 0; i < MAX_NUM; i++) {
	//				_Col_Names_InSql[i] = "line" + i;
	//			}
	//		}
	//		return _Col_Names_InSql;
	//	}
	//}

	/// <summary>
	/// 插入数据时外部只需要插入有效部分
	/// SQLiteHelper4DataEditor的这个部分负责补全sql需要的无效数据部分
	/// </summary>
	/// <returns>The values.</returns>
	/// <param name="tableName">表名称和编辑器名称是一个</param>
	/// <param name="values">Values.</param>
	public static SqliteDataReader InsertValues(string tableName,string[] values){


        //NINFO 说明下为什么去掉在这里补全sql占位数据，如果是在这里补，那么只是sql里有占位数据，本地数据的占位数据就没被刷新，所以要同时补全sql和本地两处，在上一级补
        //		int orignalLength = values.Length;
        //
        //		string[] tempValue = new string[MAX_NUM];
        //
        //		//Debug.Log ("vlen:"+values.Length+" tLen:"+tempValue.Length);
        //		Array.Copy (values, tempValue,orignalLength);
        //
        //		//改版sql要求，每个数据在sql都占用300个固定位置，如果没有数据的部分，就填充空串
        //		int n = MAX_NUM - orignalLength;
        //
        //		if (n > 0) {
        //			for(int i = orignalLength; i < MAX_NUM; i++){
        //				
        //				tempValue [i] =  "'d"+i+"'";//NINFO 这里注意空串放sql要加''
        //			}
        //		}

        //NINFO 这里要即使不补全数据如果数据补全也要报警
        int MAX_NUM = GetMaxSqlDataNum(tableName);
		int orignalLength = values.Length;
		if (orignalLength != MAX_NUM) {
			Log.e ("SQLitHelper4DataEditor", "InsertValues", "插入数据条数没达到sql对等条数 插入条数:" + orignalLength + " 需要sql数据条数:" + MAX_NUM + " 插入数据失败",BeShowLog);
			return null;
		}

		SqliteDataReader sdr = SQLiteHelper.GetIns ().InsertValues (tableName,values);

		return sdr;
	}

	/// <summary>
	/// 创建的数据表字段数固定，字段类型主键是int，其他都是字符串
	/// </summary>
	/// <returns>The table.</returns>
	/// <param name="tableName">Table name.</param>
	/// <param name="colNames">Col names.</param>
	/// <param name="colTypes">Col types.</param>
	public static SqliteDataReader CreateTable(string tableName)
	{

        int MAX_NUM = GetMaxSqlDataNum(tableName);
        Debug.Log("============>M:"+MAX_NUM+" tableName:"+tableName);
        //开始生成数据类型数组
        //设置sql列类型，第一个为int型自增长主键，其他都是字符串TEXT
        string[] colTypes = new string[MAX_NUM];
		for (int j = 0; j < colTypes.Length; j++) {
			//NINFO ID
			if (j == 1)//第0个是key:ID 第一个是value:id int数值
				colTypes [j] = "INTEGER PRIMARY KEY";//把第一项ID设置为int类型的主键，主键自动自增长
			else {
				colTypes [j] = "TEXT";
			}
		}

        string[] col_Names_InSql = Get_Col_Names_InSql(tableName);

        SqliteDataReader sdr = SQLiteHelper.GetIns ().CreateTable(tableName, col_Names_InSql, colTypes);
		return sdr;
	}
}
