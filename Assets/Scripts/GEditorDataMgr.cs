using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using Mono.Data.Sqlite;
/// <summary>
/// 1 导入导出GEditor本身结构数据
/// 2 导入导出Editor生成数据到sql
/// 3 导出gameDate
/// </summary>
public class GEditorDataMgr{

	private static bool BeShowLog = true;

	/// <summary>
	/// 存储数据
	/// 主要是打开的tab和tree及节点id
	/// </summary>
	public static void SaveEditorData()
	{
		ExportTabConfig();
		ExportTreeConfig();
	}

	#region App

	/// <summary>
	/// app启动检测,检查sqlTable是否存在，不存在就创建，每次启动都检查
	/// 
	/// xxxx 1 保存app.config,先不保存这个了，每次都检查所有编辑器类型，如果sql中没这个table就创建，这么做方便新增编辑器时的扩展
	/// 2 根据现在有tab数量,检查并创建不存在的sql table表，保证应用启动后，所有sql table表都是已经存在的
	/// </summary>
	public static void CheckSQLTableExist()
	{
		//如果不存在，1先创建sql库 2创建sql table，3创建首次启动配置文件app.config

		//step1 创建sql库,这步可以忽略，原因是，sql库创建和打开目前是一个过程，每次启动editor都会打开sql连接，如果库不存在会自动创建而AppFirstStartup要在 sql连接后执行，保证已经连接了sql 

		//setp2 创建sql table,这里需要判断下table是否存在再创建,检查所有config中的tab项
		for (int i = 0; i < GEditorConfig.ALL_EDITORS.Length; i++)
		{
			string name = GEditorConfig.ALL_EDITORS [i];

			bool b = SQLiteHelper.GetIns ().BeTableExist(name);

			if (!b)
			{
				Log.i ("GEditorDataMgr","AppFirstStartup","sql 不存在名为"+name+"的table 开始创建",BeShowLog);

				//设置sql表列名数组
				//string[] sqlItemNames = GEditorConfig.GetKVRowNames(name);;

				//设置sql列类型，第一个为int型自增长主键，其他都是字符串TEXT
//				string[] types = new string[sqlItemNames.Length];
//				for (int j = 0; j < types.Length; j++) {
//					if (j == 0)
//						types [j] = "INTEGER PRIMARY KEY";//把第一项ID设置为int类型的主键，主键自动自增长
//					else {
//						types [j] = "TEXT";
//					}
//				}

				//sql类型统一用string，所以写Text
				SQLiteHelper4DataEditor.CreateTable (name);

				Log.i ("GEditorDataMgr","AppFirstStartup","创建"+name+"表完毕",BeShowLog);
			}

		}

		//step3 创建app.config,这里暂时存任何数据，以后如果有全局配置数据可以存在这里
//		IoBuffer ib = new IoBuffer();
//		ib.PutBool(true);
//		FileHelper.Save (GEditorEnum.EDITOR_DATA_ROOTURL,GEditorEnum.EDITOR_APP_CONFIG_NAME,ib.ToArray());
	}

	#endregion




	#region ItemSQLData

	//存储kvitem数据到sql

	public static void ConnectSql()
	{
		SQLiteHelper.GetIns().OpenConnection(GEditorEnum.SQL_URL);
	}

	public static void CloseSql()
	{
		SQLiteHelper.GetIns().CloseConnection();
        Debug.Log("sql close!");
	}


	/// <summary>
	/// 新建一条kvData(使用传入的values的值)，返回ID值
	/// 
	/// </summary>
	public static int CreateKVSqlData(string tableName,string[] values)
	{
		for (int i = 0; i < values.Length; i++) {
			//Debug.Log ("--->I:"+i+" v:"+values[i]);	
			if(values[i].Equals("null"))continue;//NINFO null的那个数据时int型的主键，不需要加'',这里要特别注意，不能删除这句

			values[i] = "'"+values[i]+"'";
		}

		SqliteDataReader _reader = SQLiteHelper4DataEditor.InsertValues(tableName,values);
		//SQLiteHelper.GetIns().InsertValues(tableName,new string[]{"null","'张三'","'22'","'Zhang3@163.com'"});
		while(_reader.Read()) 
		{
			//读取主键ID
			//Debug.Log(_reader.GetInt32(_reader.GetOrdinal(SQLiteHelper4DataEditor.PRIMARY_NAME)));
			Debug.Log(_reader.GetInt32(0));//猜测写0的原因是查找表中第一个int
		}
			
		//查询自增长存入的最后一个数据
		_reader = SQLiteHelper.GetIns().ReadLastIncreseID(tableName);
		int reslutID = 0;
		while(_reader.Read()) 
		{

			//NINFO ID,这里本应该写1才是取id的value，为什么只能写0呢，难道0取得是主键？？
			//猜测这里的意思应该是读取第一个int类型，所以写0，这样才说的通
			//读取ID
			reslutID = _reader.GetInt32(0);
			//reslutID = _reader.GetInt32(_reader.GetOrdinal("line1"));

			Debug.Log("创建KVData，返回ID:"+reslutID);
		}

		return reslutID;
	}
		
	/// <summary>
	/// 修改某一条kvData中的某个选项的值
	/// rows，values是要修改哪些列，修改成哪些数据，可以是修改1列，或者多列都可以
	/// TreeItemID就是一个物品，技能在sql中的唯一id
	/// </summary>
	public static void ModifyKVSqlData(string tableName,string[] rows,string[] values,int TreeItemID)
	{
		//把值转换为sql认识的格式，距离"content" 转为 "'content'"
		for (int i = 0; i < values.Length; i++)
		{
			if(values[i].Equals("null"))continue;
			values[i] = "'"+values[i]+"'";
		}

		SQLiteHelper.GetIns().UpdateValues(tableName, 	rows, 					values, 					SQLiteHelper4DataEditor.PRIMARY_NAME, 	"=", "'"+TreeItemID+"'");
		//SQLiteHelper.GetIns().UpdateValues(tableName, new string[]{"Name"}, 	new string[]{"'Zhang3'"}, 	"Name", "=", "'张三'");
	}

	/// <summary>
	/// 删除一条KVData
	/// </summary>
	public static void DelKVSqlData(string talbename,int ID)
	{
		SQLiteHelper.GetIns().DeleteValuesAND(talbename, new string[]{SQLiteHelper4DataEditor.PRIMARY_NAME}, new string[]{"="}, new string[]{"'"+ID+"'"});
	}
		
	/// <summary>
	/// 查找某一条KvData
	/// allRows所有列的名称
	/// </summary>
	/// <param name="ID">I.</param>
	public static string[] QueryKVSqlData(string tablename/*,string[] allRows*/,int ID)
	{
        string[] Col_Names_InSql = SQLiteHelper4DataEditor.Get_Col_Names_InSql(tablename);


        SqliteDataReader reader = SQLiteHelper.GetIns().ReadTable (tablename, Col_Names_InSql, new string[]{SQLiteHelper4DataEditor.PRIMARY_NAME}, new string[]{"="}, new string[]{"'"+ID+"'"});

        int MAX_NUM = SQLiteHelper4DataEditor.GetMaxSqlDataNum(tablename);

        string[] result = new string[MAX_NUM];

		while(reader.Read()) 
		{

			for (int i = 0; i < MAX_NUM; i++)
			{
				//NINFO ID
				if (i == 1) //第一个参数是ID是int型
				{
					result[i] = reader.GetInt32(reader.GetOrdinal(Col_Names_InSql[i])).ToString();
					//int fuck = reader.GetInt32(0);
					//Debug.Log("FUCK ----------------------=============-------------->"+fuck);
					//result[i] = (reader.GetInt32(0)).ToString();//这里猜测是sql中第一个int类型
				} 
				else 
				{
					result[i] = reader.GetString(reader.GetOrdinal(Col_Names_InSql[i]));
				}


				//Debug.Log("------------sql 查询id:"+ID+" 第"+i+"个数据为"+result[i]);
			}
		}

		return result;
	}



	#endregion


	#region TabData,TreeData

	//存储编辑器节点结构

	public struct TempTabData{
		public string Name;
		public string RefName;
		public int Index;
	};


	/// <summary>
	/// 导入tab.config
	/// tab表只有一个记录已经打开的tab及其位置
	/// </summary>
	public static string[,] LoadTabConfig(string tabConfigPath)
	{
		//VLog.I("test1");
		//这里要把二进制数据转成按顺序存储的string[,]然后返回去
		byte[] bs = FileHelper.Get (tabConfigPath);
		IoBuffer ib = new IoBuffer ();
		ib.PutBytes(bs);

		int len = ib.GetInt();
		//VLog.I("test2--->len:"+bs.Length);
		string[,] ss = new string[len,2];

		List<TempTabData> _TabList = new List<TempTabData>();
		for (int i = 0; i < len; i++) 
		{
			TempTabData data = new TempTabData();
			data.Name = ib.GetString();
			data.RefName = ib.GetString();
			data.Index = ib.GetInt();
			_TabList.Add(data);
		}
		_TabList.Sort (new TabDataCom());

		for (int j = 0; j < len; j++)
		{
			ss [j, 0] = _TabList [j].Name;
			ss [j, 1] = _TabList [j].RefName;
			//VLog.I("GEditorDataMgr","LoadTabConfig","j:"+j+" Name:"+ss[j, 0]+" RefName:"+ss [j, 1]);
		}
			
		return ss;		
	}

	/// <summary>
	/// 这里只用于排序
	/// </summary>
	private class TabDataCom:IComparer<TempTabData>
	{
		public int Compare(TempTabData a,TempTabData b)
		{
			if(a.Index<b.Index)
				return -1;
			else if(a.Index==b.Index)
				return 0;
			else {
				return 1;
			}
		}
	}

	/// <summary>
	/// 导入tree_xxxx.config
	/// tree表有多个，每个tab对应一个tree表
	/// </summary>
	public static byte[] LoadOneTreeConfig(string treeConfigPath)
	{
		//这里要把二进制数直接返回去交给TreeContainer自己解析并创建具体Tree
		return FileHelper.Get(treeConfigPath);
	}

	public static void ExportTabConfig()
	{
		TabData[] td = GEditorRoot.GetIns().GetOpenedTabDatas();
		IoBuffer ib = new IoBuffer();
		ib.PutInt (td.Length);//长度
		for(int i=0;i<td.Length;i++)
		{
			ib.PutString(td[i].Name);//name;
			ib.PutString(td[i].RefName);//refName;
			ib.PutInt(td[i].Index);//Index;
			//VLog.I("GEditorDataMgr","ExportTabConfig","i:"+i+" Name:"+td[i].Name+" RefName:"+td[i].RefName+" Index:"+td[i].Index);
		}

		FileHelper.Save (GEditorEnum.EDITOR_DATA_ROOTURL,GEditorEnum.EDITOR_TAB_CONFIG_NAME,ib.ToArray());

	}


	private static IoBuffer ioBuffer4TreeCfgExport;
	/// <summary>
	/// 导出所有当前打开tree的config
	/// </summary>
	public static void ExportTreeConfig()
	{
		//获取所有根Folder
		TreeFolder[] treeFolder = GEditorRoot.GetIns ().GetAllTreeRootFolder ();
		ioBuffer4TreeCfgExport = new IoBuffer();
		for (int i = 0; i < treeFolder.Length; i++)
		{
			ProcessOneTreeItem(treeFolder[i]);
			FileHelper.Save (GEditorEnum.EDITOR_DATA_ROOTURL,GEditorEnum.EDITOR_TREE_CONFIG_NAME+treeFolder[i].Name+".config",ioBuffer4TreeCfgExport.ToArray());
			ioBuffer4TreeCfgExport.Clear();
		}

		ioBuffer4TreeCfgExport = null;
	}

	/// <summary>
	/// 处理1个tree的导出
	/// 
	/// </summary>
	/// <param name="treeItem">Tree item.</param>
	/// <param name="ioBuffer">Io buffer.</param>
	private static void ProcessOneTreeItem(TreeItem treeItem)
	{

		if (treeItem.BeFolder) {
			TreeFolder _treeFolder = treeItem as TreeFolder;
			ioBuffer4TreeCfgExport.PutBool (_treeFolder.BeFolder);//是否是folder
			ioBuffer4TreeCfgExport.PutString (_treeFolder.Name);//节点名称
			List<TreeItem> _ItemList = _treeFolder.ItemList;
			ioBuffer4TreeCfgExport.PutInt (_ItemList.Count);//直接子节点数

			for (int i = 0; i < _ItemList.Count; i++) {
				TreeItem item = _ItemList [i];
				ProcessOneTreeItem (item);
			}
		} 
		else
		{
			ioBuffer4TreeCfgExport.PutBool (treeItem.BeFolder);//是否是folder
			ioBuffer4TreeCfgExport.PutString (treeItem.Name);//节点名称
			ioBuffer4TreeCfgExport.PutString(treeItem.TreeItemID);//数据索引ID
		}

	}

    //描述下导出数据的数据结构
    // |tabName|itemSize|
    //		|item id name dataKVSize|  
    //			|stringKey stringValue| 
    //			|stringKey stringValue| 
    //		|item id name dataKVSize|  
    //			|stringKey stringValue| 
    //			|stringKey stringValue| 

    #endregion


    #region GameData

    /// <summary>
    /// 用来控制是否导出测试数据
    /// 测试数据key value type
    /// 正式数据value
    /// </summary>
	public static bool BeTest = false;
    /// <summary>
    /// 临时存储准备导出的gamedata数据
    /// </summary>
    private static IoBuffer ioBuffer4GameData;

	/// <summary>
	/// 生成所有GameData
	/// </summary>
	public static void GenerateAllGameData()
	{
		for (int i = 0; i < GEditorConfig.ALL_EDITORS.Length; i++)
		{
			GenerateGameData(GEditorConfig.ALL_EDITORS[i]);
		}
	}

	/// <summary>
	/// 生成单个Tab页的GameData
	/// </summary>
	public static void GenerateGameData(string editorType)
	{
		//初始化缓存
		//ioBuffer4GameData = new IoBuffer();

		//获取某个tab的TreeContainer
		TreeContainer _treeContainer = GEditorRoot.GetIns().TreeContainerDic[editorType];
		//获取某个tab的KvContainer
		KVContainer _kvContainer = GEditorRoot.GetIns ().KVContainerDic [editorType];

		//得到一个treeContainer下所有的treeItem信息
		TreeItemData[] itemData = _treeContainer.GetAllTreeItemData();

		//确保所有的treeItem数据都被(从sql)拉取到本地
		for (int i = 0; i < itemData.Length; i++) {
			string _treeItemID = itemData[i].TreeItemID;
			_kvContainer.CheckAndLoadTreeItemData2Local(_treeItemID);
		}

		//开始过滤并,返回过滤后要存储的二进制数据
		byte[] bs = GEditorConfig.FilterData(editorType,_kvContainer.KVDateDic);

		if (null == bs) {
			Debug.Log ("未导出 editorType:"+editorType+" 原因数据为null");
			return;
		}
        //Debug.Log("TExportData------->bs.len:"+bs.Length);
		//ioBuffer4GameData.PutString(editorType);//存储tab名
		//ioBuffer4GameData.PutInt(_treeContainer.RootFolder.SubAllItemNums);//存储当前tab下数据总条数

		//递归存储一个treeItem下的所有gdata
		//SaveOneTreeItemData(_treeContainer.RootFolder,editorType);

		//NINFO 对存储到ioBuffer4GameData中的数据进行冗余过滤

		FileHelper.Save (GEditorEnum.EDITOR_GAME_DATA_ROOTURL,editorType+GEditorEnum.EDITOR_GAME_DATA_NAME,bs /*ioBuffer4GameData.ToArray()*/);

		//ioBuffer4GameData.Clear();

		//ioBuffer4GameData = null;

	}

	#endregion
}
