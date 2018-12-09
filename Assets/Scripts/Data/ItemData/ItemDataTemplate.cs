using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDataTemplate{
	/// <summary>
	/// 表名称，默认值，(面板)打开类型(文字输入，资源选项表等)
	/// </summary>
	private static string[,] Content =new string[3,3]{
		{"ID",     "null"   ,"doNothing"},//默认值为null这里代表自增长
		{"NAME",   "name"  ,"stringValue"},
		{"Content","con"   ,"stringValue"},
		//{"end",		"end",  "custom"} //215一条数据结束标识
	};

    public static int GetMaxSqlDataNum()
    {
        return 90;
    }

    public static Dictionary<int,KVData> GetTemplateData()
	{
		Dictionary<int, KVData> KVDic = new Dictionary<int, KVData> ();
		int count = Content.GetLength (0);
		for(int i=0;i<count;i++)
		{
			KVData data = new KVData();
			data.key = Content [i, 0];
			data.value = Content [i, 1];
			data.type = Content [i, 2];

			KVDic.Add (i, data);
		}

		//模板数据比sql中最大数据少多少
//		int nullData =SQLiteHelper4DataEditor.MAX_NUM - count;
//
//		if (nullData>0){
//			if ((nullData%3)!=0) {//因为每条数据都是3项，所以这个数一定是3的倍数，如果不是，说明数据错误，报警
//				Log.e("SkillDataTemplate.GetTemplateData->为模板数据补全sql占位数据时发生错误 nulData="+nullData);
//				return null;
//			}
//
//			for(int i=0;i<nullData;i++){
//				KVData data = new KVData();
//				data.key = "";
//				data.value = "";
//				data.type = "";//
//
//				int tempIndex = count + i;
//				KVDic.Add (tempIndex, data);
//			}
//		}

		return KVDic;
	}

	public static byte[] FilterExportData(Dictionary<string,Dictionary<int,KVData>> orignalData)
	{
		return null;
	}


}
