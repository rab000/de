using UnityEngine;
using System.Collections;

public struct TreeNodeData{

	public string Name;

	/// <summary>
	/// 是否是文件夹节点
	/// </summary>
	public bool BeFolder;

	/// <summary>
	/// 子节点数，只有是文件夹这个才有意义
	/// </summary>
	public int SubNodeNum;

	//属于哪个tab
	public string RefName;

	/// <summary>
	/// 持有的数据表的唯一ID
	/// </summary>
	public int SqlID;

}
