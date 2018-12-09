using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// 基于ugui的可折叠菜单列表
/// 外界只需要跟MenuList这一个类交互就可以了
/// MenuList本身就是一个RootFolder
/// MenuList会决定整个菜单列表的最外层位置
/// </summary>
public class TreeContainer{

	/// <summary>
	/// 每个Tree都包含一个,方便Tab对其的引用
	/// 如果一个ui需要大量数据就不能单独这么写
	/// 要把具体数据按块封装
	/// 
	/// 这个名称也是对于TabItem中的RefName
	/// </summary>
	public string Name;

	public TreeFolder RootFolder;

	private RectTransform TreeRootTrm;

	/// <summary>
	/// 当前选中的那个Item
	/// </summary>
	private TreeItem _CurSelItem;
	public TreeItem CurSelItem{
		set
		{ 
			if (_CurSelItem!=null && _CurSelItem.Equals (value))return;
			if (null != _CurSelItem)_CurSelItem.DeSelect ();
			_CurSelItem = value;
			_CurSelItem.OnSelect ();
		}
		get
		{ 
			return _CurSelItem;
		}
	}
	/// <summary>
	/// 新建item，folder时每次都刷新W
	/// </summary>
	public float TreeW;

	private float TreeH{
		get
		{
			float h = RootFolder.CurFolderH;
			return h;
		}
	}

	public void Hide(){
		RootFolder.gameObject.SetActive(false);
	}

	public void Show(){
		RootFolder.gameObject.SetActive(true);
	}

	/// <summary>
	/// 当MenuList区域发生变化，刷新滚动区域范围
	/// </summary>
	public void RefreshTreeArea()
	{
		if (null == TreeRootTrm)
			Debug.Log ("FFFFFF");
		TreeRootTrm.sizeDelta = new Vector2(TreeW,TreeH);
	}

	public void SetTreeW(float w)
	{
		if (w > TreeW)TreeW = w;

		RefreshTreeArea();
			
	}


	/// <summary>
	/// 存储被剪切或copy的那个treeItem
	/// </summary>
	public TreeItem CurCopyItem;

	/// <summary>
	/// 创建可折叠菜单列表
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="parentTrm">Parent trm.</param>
	/// <param name="offx">Offx.</param>
	/// <param name="offy">Offy.</param>
	public void Create(string treeName,RectTransform parentTrm,RectTransform treeRootTrm,float offx,float offy)
	{
		Name = treeName;
		TreeRootTrm = treeRootTrm;
		RootFolder = TreeFolder.CreateRootFolder(treeName,parentTrm,this,offx,offy);
		//读取menu文件夹文件结构
		//设置位置
	}

	/// <summary>
	/// 使用数据创建Tree
	/// </summary>
	public void CreateByData(string treeName,RectTransform parentTrm,RectTransform treeRootTrm,float offx,float offy,byte[] data)
	{
		Name = treeName;
		IoBuffer ib = new IoBuffer();
		ib.PutBytes(data);

		bool bFolder = ib.GetBool();//这里第一个节点一定是根节点，但是还是按正常数据读下
		string name = ib.GetString();
		int listSize = ib.GetInt();

		//VLog.I("TreeContainer","CreateByData","bFolder:"+bFolder+" name:"+name+" listSize:"+listSize);

		//这里需要创建
		TreeRootTrm = treeRootTrm;
		RootFolder = TreeFolder.CreateRootFolder(treeName,parentTrm,this,offx,offy);


		for (int i = 0; i < listSize; i++)
		{
			//CreateOneItem(ib);
			bFolder = ib.GetBool();

			if (bFolder) {
				RootFolder.AddFolderByData (ib);
			} else {
				RootFolder.AddItemByData (ib);
			}
		}
			
	}

	/// <summary>
	/// 获取一个TreeContainer下的所有TreeItem的信息，不包括TreeFolder
	/// 目前的一个应用是，把TreeContianer中的数据提供给其他编辑器做dropdown引用
	/// </summary>
	/// <returns>The all tree item data.</returns>
	public TreeItemData[] GetAllTreeItemData(){
		List<TreeItemData> buffer = new List<TreeItemData> ();
		GetOneTreeItemData(RootFolder,buffer);
		return buffer.ToArray();
	}

	private void GetOneTreeItemData(TreeItem treeItem,List<TreeItemData> buffer)
	{
		if (treeItem.BeFolder) {
			TreeFolder _treeFolder = treeItem as TreeFolder;
			//buffer.PutBool (_treeFolder.BeFolder);//是否是folder
			//buffer.PutString (_treeFolder.Name);//节点名称
			List<TreeItem> _ItemList = _treeFolder.ItemList;
			//buffer.PutInt (_ItemList.Count);//直接子节点数

			for (int i = 0; i < _ItemList.Count; i++) {
				TreeItem item = _ItemList [i];
				GetOneTreeItemData (item,buffer);
			}
		} 
		else
		{
			TreeItemData tempData = new TreeItemData ();
			tempData.TreeItemID = treeItem.TreeItemID;
			tempData.TreeItemName = treeItem.Name;
			buffer.Add(tempData);

			//buffer.PutBool (treeItem.BeFolder);//是否是folder
			//buffer.PutString (treeItem.Name);//节点名称
			//buffer.PutString(treeItem.TreeItemID);//数据索引ID
		}

	}


}
