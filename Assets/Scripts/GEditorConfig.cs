using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// 这里写配置相关参数,主要是扩展新编辑器用
/// config这个类主要用来统一管理各个数据模板
/// 编辑器想使用数据模板必须通过config这个类来执行
/// </summary>
public class GEditorConfig{

	static bool BeShowLog = true;

	#region 编辑器模板相关
	public static Dictionary<int,KVData> GetKVTemplateData(string editorType)
	{
		switch(editorType)
		{
		case "skill":
			return ActiveSkillDataTemplate.GetTemplateData ();
			break;
		case "item":
			return ItemDataTemplate.GetTemplateData ();
			break;
		case "task":
			return TaskDataTemplate.GetTemplateData ();
			break;
		}
		return null;
	}

	/// <summary>
	/// 获取列名
	/// </summary>
	/// <returns>The KV rows data.</returns>
	/// <param name="editorType">Editor type.</param>
//	public static string[] GetKVRowNames(string editorType)
//	{
//		VLog.I ("GEditorConfig", "GetKVRowNames", "获取kvRowNames editorType:" + editorType);
//		Dictionary<int,KVData>  d = GetKVTemplateData (editorType);
//		string[] rows = new string[d.Count];
//		int i = 0;
//		foreach (KeyValuePair<int,KVData> p in d)
//		{
//			rows[i] = p.Value.key;	
//			i++;
//		}
//		return rows;
//
//	}

	/// <summary>
	/// 获取默认值
	/// </summary>
	/// <returns>The KV rows data.</returns>
	/// <param name="editorType">Editor type.</param>
	public static string[] GetKVDefaultValueData(string editorType)
	{
		Dictionary<int,KVData>  d = GetKVTemplateData (editorType);
		string[] values = new string[d.Count*3];
		int i = 0;
		foreach (KeyValuePair<int,KVData> p in d)
		{
			values[i] = p.Value.key;	
			values[i+1] = p.Value.value;	
			values[i+2] = p.Value.type;	
			i+=3;
		}
		return values;

	}
		
	/// <summary>
	/// 补全占位数据
	/// 比如sql talbe有6数据，模板数据只有3项，那么经过这个函数处理，就把不足6项的部分填空串，保证存入sql都是6项
	/// </summary>
	/// <returns>输出填充好空串的字符串数组</returns>
	/// <param name="editorType">编辑器类型</param>
	/// <param name="values">Values.</param>
	public static string[] GetFullKVDefaultValueData(string editorType,string[] orignalData){
		int orignalLength = orignalData.Length;

        int MAX_NUM = SQLiteHelper4DataEditor.GetMaxSqlDataNum(editorType);

        string[] tempValue = new string[MAX_NUM];

		//Debug.Log ("vlen:"+values.Length+" tLen:"+tempValue.Length);
		Array.Copy (orignalData, tempValue,orignalLength);

		//改版sql要求，每个数据在sql都占用300个固定位置，如果没有数据的部分，就填充空串
		int n = MAX_NUM - orignalLength;

		if (n > 0) {
			for(int i = orignalLength; i < MAX_NUM; i++){

				//tempValue [i] =  "'d"+i+"'";//NINFO 这里注意空串放sql要加''
				//string defaultValue = "default"+i;
				//tempValue [i] =  "'"+defaultValue+"'";
				tempValue [i] =  "default"+i;
			}
		}

		return tempValue;
	}

	public static byte[] FilterData(string editorType,Dictionary<string,Dictionary<int,KVData>> orignalData){
		Debug.Log ("GEditorConfig.FilterData-->editorType:"+editorType);
		switch(editorType)
		{
		case "skill":
			return ActiveSkillDataTemplate.FilterExportData(orignalData);
			break;
		case "item":
			return ItemDataTemplate.FilterExportData(orignalData);
			break;
		case "task":
			return TaskDataTemplate.FilterExportData(orignalData);
			break;
		}
		return null;
	}

	/// <summary>
	/// 在这里注册所有需要扩展的数据编辑器
	/// 这里是所有的子编辑器的类型
	/// </summary>
	public static string[] ALL_EDITORS={
		"skill",
		"item",
		"task"
	};

    #endregion


    #region 编辑器dropdown列表类型

    /// <summary>
    /// 这里说下dropdown类型的分类规则
    /// 目前有3种类型的数据
    /// custom      代表自定义，就是自己填充字符串型的value值
    /// default     代表dropdown列表数据来自本地写死的一些固定数据
    /// editorRef   代表dropdown列表的数据来自本地编辑器数据
    /// artRes      代表dropdown列表的数据来自美术资源
    /// 
    /// 另外字符串local_xxx后面的xxx部分代表具体是哪部分的编辑器说
    /// 字符串res_xxx后面的xxx代表是哪个文件夹下的资源
    /// </summary>
    public const string DROPDOWN_TYPE_custom = "custom";
    //public const string DROPDOWN_TYPE_default = "default";
    //public const string DROPDOWN_TYPE_editorRef = "editorRef";
    //public const string DROPDOWN_TYPE_artRes = "artRes";


    /// <summary>
    /// 获取(DefaultDiglog中)ContentDropdown数据
    /// 通过传入dropdown列表类型，得到具体要填充到dropdown的数据
    /// Dictionary<string,string>两个string分别是显示数据，真实数据，比如对于一个装备来说，显示数据就是玄武甲，真实数据时sql中的ID:1001
    /// 
    /// 对于参数type分3个大类型
    /// string 就是手动填写类型
    /// local_开头就是引用编辑器本身资源
    /// res_开头就是引用美术资源
    /// </summary>
    public static Dictionary<string,string> GetContentDropdownDataList(string editorType,string dataType){

		//自定义类型，不需要dropdown数据
		if (dataType.Equals (DROPDOWN_TYPE_custom)) {
			Log.w("GEditorConfig","GetDropdownDataList","type为string，不需要填充dropdown数据，返回null数据",BeShowLog);
			return null;
		}

        switch (editorType)
        {
            case "skill":
                return ActiveSkillDataTemplate.GetContentDropdownDataList(dataType);
                //break;
            case "item":
                //return ItemDataTemplate.GetTemplateData();
                break;
            case "task":
                //return TaskDataTemplate.GetTemplateData();
                break;
        }


        Log.e("GEditorConfig", "GetDropdownDataList","找不到传入类型dataType:"+dataType+"对应的模板，返回null数据！",BeShowLog);

        return null;
	}

    #endregion


}
