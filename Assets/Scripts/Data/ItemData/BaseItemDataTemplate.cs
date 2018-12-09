using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemDataTemplate{

	/// <summary>
	/// 过滤导出数据
	/// 
	/// 导出数据时存在冗余的，过滤导出数据就是为了过滤掉冗余数据
	/// 
	/// 不同类型的item因为数据结构不同，所以具体的过滤交给具体数据模板来完成
	/// 举例：技能数据，因为数据时2进制格式，所以无法直接从2进制中了解数据时什么格式，
	/// 必须要有对应的解析方法，这个解析方法除了在game中，还要放到dataEditor的模板中
	/// 这样才能有效删除冗余数据，想想技能段数据冗余的删除方式，只能在具体模板中进行
	/// 
	/// 这个方法中提供最基本的过滤，方法为，一旦遇到""字符串就抛弃掉后面所有的数据
	/// 
	/// 
	/// 传入数据data为，一个编辑器(或者说一个Tab，一个KVContainer)包含的所有数据
	/// 输出为过滤好的二进制，可以直接用来存储
	/// </summary>
//	public virtual byte[] FilterExportData(Dictionary<string,Dictionary<int,KVData>> data){
//
//		IoBuffer buffer = new IoBuffer();
//
//		byte[] bs = null;
//
//		foreach (KeyValuePair<string,Dictionary<int,KVData>> p in data)
//		{
//			Dictionary<int,KVData> _dic = p.Value;
//
//			foreach (KeyValuePair<int,KVData> k in _dic)
//			{
//				//这里准备开始记录，赛选数据
//				if(k.Value.key.Equals("")||k.Value.value.Equals("")||k.Value.type.Equals("")){
//					bs = buffer.ToArray();
//					return bs;
//				}
//
//				buffer.PutString(k.Value.key);
//				buffer.PutString(k.Value.value);
//				buffer.PutString(k.Value.type);
//
//			}
//		}
//
//		bs = buffer.ToArray ();
//		return bs;
//
//	}
}
