using UnityEngine;
using System.IO;
/// <summary>
/// 用来保存编辑器(不可自定义)常量
/// </summary>
public class GEditorEnum{

	/// <summary>
	/// editor自身结构存储位置
	/// </summary>
	//public static string GEDITOR_DATA_PATH = "";

	/// <summary>
	/// 默认打开的子编辑器
	/// 首次进入Editor，找不到编辑器tab结构文件(tab.config)时，就用这里默认的
	/// 第一个数据是名称，第二个是refname，暂时用的是同一个
	/// </summary>
	public static string[,] OPENED_EDITORS ={
		{"skill","skill"},
		{"item","item"},
		{"task","task"}
	};

	/// <summary>
	/// 存储editor数据表的位置
	/// </summary>
	public static string EDITOR_DATA_ROOTURL
	{
		get{
			return  Path.GetDirectoryName(Application.dataPath)+"/data/config/";
		}
	}

	/// <summary>
	/// Editor导出数据位置
	/// </summary>
	public static string EDITOR_GAME_DATA_ROOTURL
	{
		get{
			return  Path.GetDirectoryName(Application.dataPath)+"/data/game/";
		}
	}

	/// <summary>
	/// Editor用于引用美术资源的位置根目录
	/// </summary>
	public static string EDITOR_ART_RES_ROOTURL
	{
		get{
			return  Path.GetDirectoryName(Application.dataPath)+"/res/";
		}
	}


	/// <summary>
	/// 用于记录app是否是首次启动的配表
	/// </summary>
	public const string EDITOR_APP_CONFIG_NAME = "app.config";

	/// <summary>
	/// tab配表名称，用于存储tab相关结构信息
	/// </summary>
	public const string EDITOR_TAB_CONFIG_NAME = "tab.config";

	/// <summary>
	/// tree配表名称，用于存储tree结构及tree上的item id
	/// </summary>
	public const string EDITOR_TREE_CONFIG_NAME = "tree_";

	/// <summary>
	/// 最终导出的游戏数据表的后缀
	/// </summary>
	public const string EDITOR_GAME_DATA_NAME = ".gdata";
	/// <summary>
	/// 数据库位置
	/// </summary>
	public const string SQL_URL = "data source= data/geditor.db";

}
