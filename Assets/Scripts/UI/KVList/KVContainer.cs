using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// keyValue区统一对外接口，用来管理一个KV区域，可以同时存在多个kv区域(用多个KVManager实例管理)
/// 数据生命周期说明
/// 每组kv数据创建时就使用模板中的原始数据写入sql，并得到sql中的id同时写入到kv数据Dic中
/// 每个kv中某个数据改动时也立刻写入sql，并且同时修改本地内存数据（KeyValuDateDic），方便切换页面时直接使用，切页面时就不需要二次从sql中读了
/// 
/// kvItem每次打开时，查找kvDic,查不到就去sql加载
/// 
/// ui列表prefab每次切页面要重新刷内容，这里可以有个空闲对象池，避免每次彻底删除重建
/// 
/// 
/// </summary>
public class KVContainer{

	private bool BeShowLog = true;
	/// <summary>
	/// KVContainer属于哪个Tab，暂时这个参数可有客户，记录下，如果有必要就用
	/// </summary>
	public string Name;

	/// <summary>
	/// 记录KVContainer当前显示的是哪个TreeItem的数据
	/// </summary>
	public string CurTreeItemID;

	//key，value prefab挂在的父节点
	private RectTransform KeyValueParentTrm;
	private RectTransform KeyParentTrm;
	private RectTransform ValueParentTrm;

	public ListItem CurSelKeyListItem;

	private ListItem _CurSelectedKVItem;
	public ListItem CurSelectedKVItem{
		set
		{
			ListItem NextItem = value;

			if (null == NextItem) {//选择了一个null值得处理，相当于清理选中
				if (null != _CurSelectedKVItem)_CurSelectedKVItem.DeSelect ();
				return;
			}
			else
			{
				if (NextItem.Equals (_CurSelectedKVItem))return;//重复点中了当前的选择项，什么都不做，直接返回
				if (null != _CurSelectedKVItem)	_CurSelectedKVItem.DeSelect();
				_CurSelectedKVItem = NextItem;
				_CurSelectedKVItem.OnSelect();
			}
		}
		get
		{
			return _CurSelectedKVItem;
		}
	}

	/// <summary>
	/// 这个Dic用于存储所有打开过的kv项数据
	/// 从sql加载回的数据，保存在内存，方便随时切换使用
	/// 第一个int是具体kv在sql中的唯一ID
	/// 第二个参数是某个具体的KVData的数据项,第二个dic中的int使用数据项KVData的key在sql中的index
	/// </summary>
	public Dictionary<string,Dictionary<int,KVData>> KVDateDic;

	/// <summary>
	/// Initializes a new instance of the <see cref="KVContainer"/> class.
	/// </summary>
	/// <param name="keyvalueTrm">Keyvalue trm.这个trm就是scrollView下的content用于调整滚动区域大小</param>
	/// <param name="keyTrm">Key trm.</param>
	/// <param name="valueTrm">Value trm.</param>
	/// <param name="TabType">Tab type.</param>
	public KVContainer(RectTransform keyvalueTrm,RectTransform keyTrm,RectTransform valueTrm,string TabType)
	{
		if(null==KVDateDic)KVDateDic = new Dictionary<string, Dictionary<int, KVData>>();
		KeyValueParentTrm = keyvalueTrm;
		KeyParentTrm = keyTrm;
		ValueParentTrm = valueTrm;
		Name = TabType;
//		for(int i=0; i<GEditorConfig.ALL_EDITORS.Length; i++)
//		{
//			KVDateDic.Add(GEditorConfig.ALL_EDITORS[i],new Dictionary<int, Dictionary<string, KVData>>());
//		}
	}
		
	/// <summary>
	/// 检测本地是否存在TreeItmeID的数据，
	/// 不存在就去sql中查找，并存到本地，返回找到的这个treeItem的数据
	/// 存在就直接返回
	public Dictionary<int ,KVData> CheckAndLoadTreeItemData2Local(string TreeItemID)
	{
		//根据TreeItem.ID判断本地是否有对应的Kvitem的数据,如果有就直接填充数据
		bool b = BeKVListDataCache(TreeItemID);

		Dictionary<int ,KVData> kvDic = null;

		if (b)//如果有缓存就从缓存刷新数据
		{
			Log.i("KVContainer","RefreshKVList","从本地缓存刷数据 ID:"+TreeItemID,BeShowLog);

			kvDic = GetLocaCacheKVListData (TreeItemID);
//			if (beRefreshUI) {
//				RefreshKVListUI(GetLocaCacheKVListData(TreeItemID));
//				CurTreeItemID = TreeItemID;
//			}
		}
		else//如果没有就使用TreeItem.ID从sql查询读取，并缓存到本地
		{
			Log.i("KVContainer","RefreshKVList","从sql查询获取数据 ID:"+TreeItemID,BeShowLog);
			//这里要把不同tab的row刷出来填进去
			int _id = Convert.ToInt32(TreeItemID);
			//string[] rowNames = GEditorConfig.GetKVRowNames(Name);
			//string[] openType = GEditorConfig.GetKVRowOpenType(Name);
			//这里查询结果要存到本地缓存,这里查询到的是某个id的数据
			string[] queryResult = GEditorDataMgr.QueryKVSqlData (Name/*, rowNames*/, _id);

			kvDic = new Dictionary<int, KVData>();
			for (int i = 0,j=0; i < queryResult.Length; i+=3,j++) //nafio nsql
			{
				KVData kvdata = new KVData();
				kvdata.key = queryResult [i];
				kvdata.value = queryResult[i+1];
				kvdata.type = queryResult[i+2];
				kvDic.Add(j,kvdata);
				//Log.i("KVContainer","RefreshKVList","从sql向本地填充queryResult["+i+"]->key:"+kvdata.key+" value:"+kvdata.value+" type:"+kvdata.type);
			}

			KVDateDic.Add (TreeItemID,kvDic);
		}

//		if (beRefreshUI){
//			RefreshKVListUI(kvDic);
//			CurTreeItemID = TreeItemID;
//		}

		return kvDic;

	}

	/// <summary>
	/// Creates the KV item.
	/// </summary>
	/// <param name="kvType">Kv type.数据表名，kvitem属于那个tree，哪个tab,就是tabType或editorType</param>
	public string CreateKVList(string kvType)
	{
		//使用模板数据填入sql,并取回id
		//Dictionary<int,KVData> dic = GEditorConfig.GetKVTemplateData(kvType);

		string[] _defaultDate = GEditorConfig.GetKVDefaultValueData(kvType);
		string[] _fullDefaultDate = GEditorConfig.GetFullKVDefaultValueData(kvType,_defaultDate);
		//_t[0]= "null";

		//从sql拿回id,并保存这个id
		int id = GEditorDataMgr.CreateKVSqlData(kvType,_fullDefaultDate);
		//NINFO nsql 这里3处"ID"，写法不是很好，但目前是正确的，后面如果感觉不合理要修改下

		//从sql取出id为id的数据dic，这个函数还会把数据存到本地


		Dictionary<int,KVData>  dic = CheckAndLoadTreeItemData2Local(id.ToString());
		dic[0].value = id.ToString();

		//Log.i("KVContainer","CreateKVItem","key:"+dic[0]);
		//KVDateDic.Add(dic[0].value,dic);

		//使用KvDic中的数据填充 itemUI prefab，刷新ui
		RefreshKVListUI(dic);
		CurTreeItemID = dic[0].value;

		Log.i("KVContainer","CreateKVItem","新增KVItem id:"+id,BeShowLog);

		return id.ToString();
	}

	/// <summary>
	/// 要删除的item的ID
	/// </summary>
	/// <param name="ID"></param>
	public void DelKVListData(string ID)
	{
		//删本地数据
		KVDateDic.Remove(ID);

		int id = Convert.ToInt32(ID);
		//删sql数据
		GEditorDataMgr.DelKVSqlData(Name,id);

		Log.i("KVContainer","DelKVItem","删除KVItem id:"+id,BeShowLog);
	}

	/// <summary>
	/// 修改本地的一条kv数据,包括本地数据，及sql中的存储数据
	/// </summary>
	/// <param name="treeItemID">Tree item I.</param>
	/// <param name="index">这个kvdata数据在sql同时也是在数据dic中的index(kvdata.key在sql中的顺序位置)</param>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	/// <param name="type">数据编辑类型，或打开窗口类型</param>
	public void ModifyKVItem(string treeItemID,int index,string key,string value,string type)
	{
		//修改本地数据-------------------------------------
		if (KVDateDic.ContainsKey (treeItemID)) {
			KVDateDic [treeItemID] [index].key = key;
			KVDateDic [treeItemID] [index].value = value;
			KVDateDic [treeItemID] [index].type = type;
		} else {
			//说明，这个函数OnDialogReturnValue是打开某一项TreeItem后再点击其中的KvItem才执行的，所以不可能出现在KVContainer中找不到当前TreeItem数据的情况
			Log.e("KVListItem","OnDialogReturnValue","KVContainer中不包含当前打开的dialog所处理的treeItem  TreeItemID:"+treeItemID,BeShowLog);
		}

		//修改sql数据-------------------------------------
		int key_offset_inSql = 0;
		int _index = (index * 3 + key_offset_inSql);
		string key_col_name_inSql = "line" + _index;//这里这么写的原因可以参考SQLitHelper中对数据库设计的说明

		int value_offset_inSql = 1;
		_index = (index * 3 + value_offset_inSql);
		string value_col_name_inSql = "line" + _index;//这里这么写的原因可以参考SQLitHelper中对数据库设计的说明

		int type_offset_inSql = 2;
		_index = (index * 3 + type_offset_inSql);
		string type_col_name_inSql = "line" + _index;//这里这么写的原因可以参考SQLitHelper中对数据库设计的说明

		GEditorDataMgr.ModifyKVSqlData (Name, new string[]{ key_col_name_inSql,value_col_name_inSql,type_col_name_inSql }, new string[]{ key,value,type }, Convert.ToInt32(treeItemID));

        //修改ui,这里稍微有点浪费效率，整体都重新刷新了一便，但是不用重新开发功能，就这样
        RefreshKVListUI(KVDateDic[treeItemID]);
    }

    /// <summary>
    /// 获取本地缓存好的某个ID的kvitem的KVData
    /// </summary>
    /// <returns><c>true</c>, if loca cache KV data was gotten, <c>false</c> otherwise.</returns>
    /// <param name="ID">I.</param>
    public Dictionary<int,KVData> GetLocaCacheKVListData(string ID)
	{
		if (!KVDateDic.ContainsKey (ID)) {
			Log.e("KVContainer","GetLocaCacheKVListData","本地缓存不包含ID:"+ID+"的数据！",BeShowLog);
			return null;
		}
		Dictionary<int,KVData> d = KVDateDic[ID];

		return  d;
	}

	/// <summary>
	/// 是否已经从sql中缓存了某个ID的本地数据
	/// </summary>
	/// <returns><c>true</c>, if KV data cache was been, <c>false</c> otherwise.</returns>
	/// <param name="ID">I.</param>
	public bool BeKVListDataCache(string ID)
	{
		if (KVDateDic.ContainsKey (ID))
			return true;

		return false;
	}

	/// <summary>
	/// 清理掉kvitem UI
	/// </summary>
	public void ClearKVListUI()
	{

		//每次清理ui时把当前选择项置null否则切TreeItem时，当前选择项会出现混乱
		CurSelectedKVItem = null;
		//清理旧节点
		//清理key节点
		int j = 0;
		KVListItem[] _tempItems = KeyParentTrm.GetComponentsInChildren<KVListItem>();
		for (j = 0; j < _tempItems.Length; j++)
		{
			_tempItems [j].Reset();
			FreeItemPool.PushFreeItem2Pool(_tempItems [j]);
		}
		_tempItems = null;
		//清理value节点
		_tempItems = ValueParentTrm.GetComponentsInChildren<KVListItem>();
		for (j = 0; j < _tempItems.Length; j++)
		{
			_tempItems [j].Reset();
			FreeItemPool.PushFreeItem2Pool(_tempItems [j]);
		}
	}

	/// <summary>
	/// 当选中某个treeItem时，切换当前treeItme相关显示
	/// </summary>
	/// <param name="treeItemID">Tree item I.</param>
	public void SwitchTreeItem(string treeItemID)
	{
		//获取当前treeItemID的数据
		Dictionary<int ,KVData> kvDic = CheckAndLoadTreeItemData2Local(treeItemID);

		//NINFO 这部要在RefreshKVListUI之前，因为RefreshKVListUI会用到最新的CurTreeItemID
		//把当前的item置为新的itemID
		CurTreeItemID = treeItemID;

		//刷新新kv列表的ui
		RefreshKVListUI(kvDic);


	}

	/// <summary>
	/// 使用kvData这个dic中的数据
	/// 刷新kv区域ui
	/// </summary>
	/// <param name="kvData">Kv data.</param>
	private void RefreshKVListUI(Dictionary<int,KVData> kvData){

		ClearKVListUI();

		//创建新节点
		int i = 0;
		//创建ui相关
		foreach(KeyValuePair<int,KVData> p in kvData){
			//Debug.Log("nafio--->k:"+p.Key+" v:"+p.Value);
			KVListItem keyItem = FreeItemPool.GetFreeItem();
			if (!keyItem.gameObject.activeSelf)keyItem.gameObject.SetActive (true);
			keyItem.gameObject.name = p.Value.key;
			keyItem.IsKey = true;
			keyItem.Index = i;
			keyItem.TreeItemID = Convert.ToInt32(CurTreeItemID);
			//Debug.Log("==========>i:"+i+"====>CurItemID:"+CurTreeItemID);
			keyItem.SetContent(p.Value.key);
			keyItem.SetKVMgr(this);
			RectTransform keyRT = keyItem.gameObject.GetComponent<RectTransform>();
			keyRT.SetParent(KeyParentTrm as Transform);
			keyRT.anchoredPosition = new Vector2(0,i*-1*UIEnum.KVItemH);
			keyRT.offsetMax = new Vector2(-1,keyRT.offsetMax.y);
			keyRT.offsetMin = new Vector2(1,keyRT.offsetMin.y);

			KVListItem valueItem = FreeItemPool.GetFreeItem();
			if (!valueItem.gameObject.activeSelf)valueItem.gameObject.SetActive (true);
			RectTransform valueRT = valueItem.gameObject.GetComponent<RectTransform>();
			valueItem.gameObject.name = p.Value.value;
			valueItem.IsKey = false;
			valueItem.Index = i;
			valueItem.TreeItemID = Convert.ToInt32(CurTreeItemID);
			valueItem.SetContent(p.Value.value);
			//valueItem.DialogOpenType = p.Value.type;
			valueItem.SetKVMgr(this);
			valueRT.SetParent(ValueParentTrm as Transform);
			valueRT.anchoredPosition = new Vector2(0,i*-1*UIEnum.KVItemH);
			valueRT.offsetMax = new Vector2(-1,valueRT.offsetMax.y);
			valueRT.offsetMin = new Vector2(1,valueRT.offsetMin.y);

			//keyItem.DialogOpenType = p.Value.type;

			keyItem.BrotherItem = valueItem;
			valueItem.BrotherItem = keyItem;

			i++;
		}

		float h = i * UIEnum.KVItemH;
		RefreshKVContainerArea(h);
	}

	/// <summary>
	/// 刷新整个KV区域的滚动区域大小
	/// </summary>
	public void RefreshKVContainerArea(float h)
	{
		KeyValueParentTrm.sizeDelta = new Vector2(KeyValueParentTrm.sizeDelta.x,h);
	}

}


class FreeItemPool
{
	static int size = 0;
	static KVListItem head = null;

	public static KVListItem GetFreeItem()
	{
		if(size == 0)
		{
			GameObject go = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/KVListItem"));
			KVListItem listItem = go.GetComponent<KVListItem>();
			return listItem;
		}
		else if(size == 1)
		{
			size = 0;
			return head;
		}
		else
		{
			KVListItem tempNode  =  head;
			head = head.Next as KVListItem;
			tempNode.Next  =  null;
			size--;
			return tempNode;
		}

	}

	public static void PushFreeItem2Pool(KVListItem node)
	{

		node.Reset();

		if(size==0){
			head=node;
		}else{
			node.Next = head;
			head = node;
		}

		size++;
	}
}