using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 说明
/// 
/// 禁止使用folder与item混排，folder下面只允许时folder或者item，混排没有任何好处
/// 目前功能上是支持混排的
/// </summary>
public class GEditorRoot : MonoBehaviour {

	#region panel节点
	public Canvas _Canvas;
	[SerializeField]private RectTransform TreePanel;
	[SerializeField]private RectTransform KeyValuePanel;
	[SerializeField]private RectTransform KeyPanel;
	[SerializeField]private RectTransform ValuePanel;
	[SerializeField]private RectTransform TabPanel;
	public RectTransform DialogPanel;
	#endregion

	#region 主要控件引用
	public TabContainer TabContainer;
	//public KVContainer _KVManager;
	//public TreeContainer CurTreeContainer;
	public Dictionary<string,TreeContainer> TreeContainerDic;
	public Dictionary<string,KVContainer> KVContainerDic;
	#endregion

	#region 单例
	private static GEditorRoot Ins;
	public static  GEditorRoot GetIns(){return Ins;}
	void Awake(){Ins = this;}
	void OnDestroy(){
		GEditorDataMgr.SaveEditorData();
		GEditorDataMgr.CloseSql();
		Ins = null;
	}
	#endregion 

	#region mono
	void Start ()
	{
		InitGEditor();
	}

	void Update ()
	{

		//点击任意空白位置，弹出菜单消失
		if (Input.GetMouseButtonDown(0)) 
		{
			if (null == Menu.CurMenu)
				return;
			bool b = RectTransformUtility.RectangleContainsScreenPoint (Menu.CurMenu._RectTransform, new Vector2 (Input.mousePosition.x, Input.mousePosition.y));

			if(!b)
			{
				Menu.CurMenu.Hide();	
			}
		}
			
        //正常运行的情况下这段不起作用
		if (Input.GetKeyUp (KeyCode.LeftAlt) /*&& Input.GetKeyUp (KeyCode.F4)*/)
		{
			
			GEditorDataMgr.SaveEditorData();

			GEditorDataMgr.CloseSql();

			Application.Quit();

			Log.i("编辑器正常关闭");
		}
	}
		

	/// <summary>
	/// 编辑器初始化，重要流程
	/// </summary>
	private void InitGEditor()
	{
		//创建Tab管理器
		TabContainer = new TabContainer(TabContainer.TabLayoutType.horizontal,TabPanel);
		TabContainer.TabSelectEvent = OnTabSelect;
		TabContainer.TabDeSelectEvent = OnTabDeSelect;

		//创建Tree管理器
		if (null == TreeContainerDic)TreeContainerDic = new Dictionary<string, TreeContainer> ();

		//创建列表管理器
		if (null == KVContainerDic)KVContainerDic = new Dictionary<string, KVContainer>();

		//加载tab数据，根据数据创建打开tab
		//检测是否存在tab.config配置文件，如果没有，就使用默认打开项目(GEditorEnum.OPENED_EDITORS)
		string tabCfgPath = GEditorEnum.EDITOR_DATA_ROOTURL+GEditorEnum.EDITOR_TAB_CONFIG_NAME;
		bool beTabCfgExists = FileHelper.BeFileExists(tabCfgPath);
		//VLog.I("tabCfgPaht:"+tabCfgPath+" beExis:"+beTabCfgExists);
		string[,] _CurOpenedTabs = null;
		if (beTabCfgExists) _CurOpenedTabs = GEditorDataMgr.LoadTabConfig(tabCfgPath);//非首次，使用tab.config中记录的数据
		else _CurOpenedTabs = GEditorEnum.OPENED_EDITORS;//首次打开，不存tab.config相关记录，使用默认打开tab页

		for(int i=0;i<_CurOpenedTabs.GetLength(0);i++)
		{

			//根据数创建tab项
			TabData tabData = new TabData();
			tabData.Name = _CurOpenedTabs[i,0];
			tabData.RefName = _CurOpenedTabs[i,1];
			tabData.Index = i;

			var tabItem = TabItem.Create (tabData, TabPanel, TabContainer);
			TabContainer.Add(tabItem);

			//根据现有打开的tab，创建对应的tree项
			string treeCfgPath = GEditorEnum.EDITOR_DATA_ROOTURL+GEditorEnum.EDITOR_TREE_CONFIG_NAME+tabData.RefName+".config";
			bool beTreeConfigExists = FileHelper.BeFileExists(treeCfgPath);
			TreeContainer treeContainer = new TreeContainer();
			if (beTreeConfigExists)//treeConfig配表存在，从配表载入TreeContainer
			{
				byte[] bs = GEditorDataMgr.LoadOneTreeConfig(treeCfgPath);
				treeContainer.CreateByData(tabData.RefName,TreePanel,TreePanel,0,0,bs);
				TreeContainerDic.Add(tabData.RefName,treeContainer);

			}
			else//treeConfig配表不存在存在，创建新的根节点
			{
				treeContainer.Create(tabData.RefName,TreePanel,TreePanel,0,0);
				TreeContainerDic.Add(tabData.RefName,treeContainer);

			}
			treeContainer.Hide();//创建完先隐藏，等等具体tab被选中时才显示


			//设置tabitem初始选中状态,这句需要放在TreeContainer被创建之后，
			//因为Beselect一旦被执行，就会执行切换TreeContainer的操作,如果TreeContainer还没被创建，就会出错
			if (i == 0)tabItem.BeSelecte = true;
			else tabItem.BeSelecte = false;


			//创建KVContainer，打开几个tab创建几个，跟tree类似
			KVContainerDic.Add (tabData.RefName,new KVContainer(KeyValuePanel,KeyPanel,ValuePanel,tabData.RefName));

		}
	
		//打开数据库,保持长连接
		GEditorDataMgr.ConnectSql();

		//判定是否是首次打开editor，如果是创建sql table，并创建首次打开标示文件app.config
		GEditorDataMgr.CheckSQLTableExist();

	}

	#endregion

	#region 3个字容器相关操作
	//NINFO 这里解释下为什么tab相关的事件要写到GEditorRoot中，因为目前的编辑器思路是tab，tree，kvcontainer是平级关系，而非相互包含
	public void OnTabSelect(string tabRefName){
		
		var treeContainer = TreeContainerDic[tabRefName];
		treeContainer.Show();

        //初始时KV数据比tab数据后创建，所以这里如果KVContainerDic还不包括相应数据的话，就掠过
        if (!KVContainerDic.ContainsKey(tabRefName)) return;

        //切换tab时刷新kv相关ui，如果新切换到的tab页中，treeContainer的CurSelItem被设置过，就把kv相关ui切换成功这个CurSelItem的kv数据
        if (TreeContainerDic[tabRefName].CurSelItem != null && (!TreeContainerDic[tabRefName].CurSelItem.BeFolder))
        {
            KVContainerDic[tabRefName].SwitchTreeItem(TreeContainerDic[tabRefName].CurSelItem.TreeItemID);
        }
        else//如果新切换到的tab页中，treeContainer的CurSelItem没被设置过，就把kv相关清空
        {
            KVContainerDic[tabRefName].ClearKVListUI();
        }
    }
	public void OnTabDeSelect(string tabRefName){
		var treeContainer = TreeContainerDic[tabRefName];
		treeContainer.Hide();
	}

	/// <summary>
	/// 返回所有TreeContainer根节点文件夹
	/// 目前用于存储tree数据
	/// 函数写在GEditorRoot中原因跟OnTabSelect相同
	/// </summary>
	/// <returns>The all tree container.</returns>
	public TreeFolder[] GetAllTreeRootFolder()
	{
		List<TreeFolder> _treeFolerList = new List<TreeFolder>();
		foreach(KeyValuePair<string,TreeContainer> p in TreeContainerDic)
		{
			_treeFolerList.Add (p.Value.RootFolder);
		}
		return _treeFolerList.ToArray();
	}

	/// <summary>
	/// 返回所有当前打开状态的Tab数据
	/// 这些数据目前用于存储tab数据
	/// 函数写在GEditorRoot中原因跟OnTabSelect相同
	/// </summary>
	/// <returns>The tab datas.</returns>
	public TabData[] GetOpenedTabDatas()
	{
		List<TabData> _tabDataList = new List<TabData>();
		foreach(KeyValuePair<string,TabItem> p in TabContainer.TabItemDic)
		{
			_tabDataList.Add (p.Value._TabData);
		}

		return _tabDataList.ToArray();
	}
	#endregion
}
