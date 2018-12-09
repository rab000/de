using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System.Collections.Generic;
using System;
public class TreeItem : SelectableItem
{
	private bool BeShowLog = true;
	public Text ItemNameText;
	public Image ItemTextBGImg;
	/// <summary>
	/// 文件夹右键菜单
	/// </summary>
	[HideInInspector]public Menu FolderRightMenu;
	[HideInInspector]public bool BeFolder;
	[HideInInspector]public string Name;
	protected Image Icon;
	/// <summary>
	/// 物品，技能的ID
	/// </summary>
	public string TreeItemID;
		
	protected TreeFolder ParentFolder;

	/// <summary>
	/// 保留对menuLsit的引用，方便跟上级沟通
	/// </summary>
	public TreeContainer MyTreeContainer;

	/// <summary>
	/// 是否被剪切
	/// </summary>
	private bool _BeCut;
	public bool BeCut{
		set
		{ 
			_BeCut = value;

			if (_BeCut)
				ItemNameText.color = new Color (0, 1, 0, 1);
			else
				ItemNameText.color = Color.black;
		}
		get
		{ 
			return _BeCut;
		}
	}

	/// <summary>
	/// Item在Folder中的相对位置
	/// 随时去父folder查找，这样就算经过删除，插入依然能保证index正确
	/// </summary>
	public int IndexInParentFolder{
		get
		{
			int index = ParentFolder.GetItemIndexInFolder(this);
			return index;
		}
	}

	/// <summary>
	/// 当前item距离整个MenuList左边缘的相对位置
	/// </summary>
	/// <returns>The item left offset.</returns>
	public float CurItemLeftOffset
	{
		get
		{
			float dis = 0;
			if (ParentFolder != null)
				dis = ParentFolder.CurItemLeftOffset + UIEnum.FolderBtnW;
			else
				dis = 0;//根目录
			return dis;
		}
	}

	protected override void  Awake()
	{
		base.Awake ();
	}


	#region 创建Item

	/// <summary>
	/// 这个方法只有从folder右键创建item时调用
	/// 其他地方用InsertItem
	/// </summary>
	/// <param name="itemName">Item name.</param>
	/// <param name="parentFolder">Parent folder.</param>
	/// <param name="menuList">Menu list.</param>
	public static TreeItem Create(string itemName,TreeFolder parentFolder,TreeContainer treeContainer)
	{
		//添加文件夹时，如果父文件夹没有打开，先打开再添加
		if (!parentFolder.BeFolderOpen)
			parentFolder.BeFolderOpen = true;

		//实例化文件夹
		GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/TreeItem")) as GameObject;
		go.name = itemName;
		TreeItem item = go.GetComponent<TreeItem>();
		item.MyTreeContainer = treeContainer;
		item.BeFolder = false;
		item.Name = itemName;
		go.transform.SetParent(parentFolder.SubRootRectTransform as Transform);
		item._RectTransform = go.GetComponent<RectTransform>();
		item._RectTransform.pivot = new Vector2(0,1);
		item._RectTransform.anchorMax = new Vector2 (0,1);
		item._RectTransform.anchorMin = new Vector2 (0,1);

		item.ItemNameText.text = itemName;
		item.ItemNameText.fontSize = UIEnum.FontSize;
		var textRt = item.ItemNameText.gameObject.GetComponent<RectTransform>();
		float w = Tools.GetStringW (item.ItemNameText.text,UIEnum.FontSize);
		textRt.sizeDelta = new Vector2(w,UIEnum.ItmeH);

		//重新设置下Folder的边框宽度
		float w1 = UIEnum.ItemImageW+w;
		item._RectTransform.sizeDelta = new Vector2(w1,UIEnum.ItemH);

		item.ParentFolder = parentFolder;

		//float h = (parentFolder.SubItemNums + 1) * UIEnum.ItemH;
		float h = parentFolder.CurFolderH;

		//float h = parentFolder.CurFolderH;
		item._RectTransform.anchoredPosition = new Vector2 (UIEnum.FolderBtnW*2+4,-h);
		item.CreateMenu(item);

		parentFolder.ItemList.Add(item);
		parentFolder.RefreshItemNum();


		//新建文件夹后通知上级进行重新排序，比如rootFolder下有10个subFolder，在第二个subFolder下新建，那么后面所有subFolder都要重排
		//这里注意下ui坐标系是左下是0,0所以负值是向下移动
		parentFolder.ParentYOrder(-UIEnum.ItemH,item.IndexInParentFolder);


		//刷新下MenuList的边框宽，用于刷新MenuList滚动区域,item距离Menulist左边距离+一个图标间隔+4(文字和图标间隔)+w1(文字长度)
		float _w = item.CurItemLeftOffset + UIEnum.ItemImageW + 4 +w1; 
		treeContainer.SetTreeW(_w);
		treeContainer.RefreshTreeArea();

		return item;
	}
	#endregion



	#region 菜单功能区
	public void CreateMenu(TreeItem menuListItem){
		if (null != FolderRightMenu)return;
		Dictionary<string,Listener> dic = new Dictionary<string, Listener> ();
		dic.Add ("新建", menuListItem.AddItem);
		dic.Add ("重命名", menuListItem.ReName);
		dic.Add ("复制", menuListItem.Copy);
		dic.Add ("剪切", menuListItem.Cut);
		dic.Add ("黏贴", menuListItem.Paste);
		dic.Add ("删除", menuListItem.DelItemWithDlg);
		FolderRightMenu = Menu.Generate(dic,"FolderMenu");
		FolderRightMenu.ItemDic ["黏贴"].interactable = false;//默认禁用黏贴
	}

	public void ReName ()
	{
		//base.ReName ();
		InputTextDialog.Open(
			delegate(byte[] bts) 
			{
				//创建文件夹
				IoBuffer ib = new IoBuffer();
				ib.PutBytes(bts);
				string newName = ib.GetString();
                Name = newName;//这个名称不设置的化，当重命名后，再复制黏贴就出问题，显示的是未改名前的名称
                ResizeTextRect(newName);

				//刷TreeContainer的W.这里w的算法，跟上面Create中一致
				float _w = this.CurItemLeftOffset + UIEnum.ItemImageW + 4 +UIEnum.ItemImageW+Tools.GetStringW (this.ItemNameText.text,UIEnum.FontSize); 
				MyTreeContainer.SetTreeW(_w);
				MyTreeContainer.RefreshTreeArea();
			}
		);
	}

	protected virtual void ResizeTextRect(string s)
	{
		//重新设置文件夹名称，并重置text和text父控件大小
		this.ItemNameText.text = s;
		this.ItemNameText.fontSize = UIEnum.FontSize;
		var textRt = this.ItemNameText.gameObject.GetComponent<RectTransform>();
		float w = Tools.GetStringW (this.ItemNameText.text,UIEnum.FontSize);
		textRt.sizeDelta = new Vector2(w,UIEnum.ItmeH);

		//重新设置下Folder的边框宽度
		float w1 = UIEnum.ItemImageW+w;
		this._RectTransform.sizeDelta = new Vector2(w1,UIEnum.ItemH);
	}

	/// <summary>
	/// 删除item，先填出确认删除窗口，确认后才删除
	/// </summary>
	private void DelItemWithDlg()
	{
		OKDialog.Open (
			delegate() {
				DelItem(true);
			},
			"确定删除文件?"
		);
	}

	/// <summary>
	/// 删除item
	/// </summary>
	/// <param name="bDelData">true删除数据+treeitem节点，false只删节点，不删除本地及sql数据，剪切时用后者</param>
	public void DelItem(bool bDelData=false)
	{

        if (bDelData) {
            GEditorRoot.GetIns().KVContainerDic[MyTreeContainer.Name].DelKVListData(TreeItemID);
            GEditorRoot.GetIns().KVContainerDic[MyTreeContainer.Name].ClearKVListUI();//删除时清空当前kvcontainer的显示，否则item已经删除，但是kv上还显示数据就很奇怪了
        } 

		Log.i ("TreeItem", "DelItem", "删除TreeItem id:" + TreeItemID,BeShowLog);

		//先做删除文件夹后的排序
		ParentFolder.ParentYOrder (UIEnum.ItemH, IndexInParentFolder);
		//然后把folder从父类ListItem中去掉
		ParentFolder.ItemList.Remove (this);
		//再删除folder节点
		Destroy (gameObject);
		ParentFolder.RefreshItemNum ();
	}

	private void AddItem()
	{
		InputTextDialog.Open(
			delegate(byte[] bts) 
			{
				//创建文件夹
				IoBuffer ib = new IoBuffer();
				ib.PutBytes(bts);
				string name = ib.GetString();
				string _itemID = GEditorRoot.GetIns().KVContainerDic[MyTreeContainer.Name].CreateKVList(MyTreeContainer.Name);
				var treeItem = InsertItem(IndexInParentFolder,name,ParentFolder,MyTreeContainer);
				treeItem.TreeItemID = _itemID;
				Log.i("TreeItem","AddItem","新建item name:"+name+" id:"+_itemID,BeShowLog);
			}
		);
	}
		
	/// <summary>
	/// 新建,黏贴插入操作
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="itemName">Item name.</param>
	/// <param name="parentFolder">Parent folder.</param>
	/// <param name="menuList">Menu list.</param>
	private TreeItem InsertItem(int index,string itemName,TreeFolder parentFolder,TreeContainer menuList)
	{
		//实例化文件夹
		GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/TreeItem")) as GameObject;
		go.name = itemName;
		TreeItem item = go.GetComponent<TreeItem>();
		item.MyTreeContainer = menuList;
		item.BeFolder = false;
		item.Name = itemName;
		go.transform.SetParent(parentFolder.SubRootRectTransform as Transform);
		item._RectTransform = go.GetComponent<RectTransform>();
		item._RectTransform.pivot = new Vector2(0,1);
		item._RectTransform.anchorMax = new Vector2 (0,1);
		item._RectTransform.anchorMin = new Vector2 (0,1);

		item.ItemNameText.text = itemName;
		item.ItemNameText.fontSize = UIEnum.FontSize;
		var textRt = item.ItemNameText.gameObject.GetComponent<RectTransform>();
		float w = Tools.GetStringW (item.ItemNameText.text,UIEnum.FontSize);
		textRt.sizeDelta = new Vector2(w,UIEnum.ItmeH);

		//重新设置下Folder的边框宽度
		float w1 = UIEnum.ItemImageW+w;
		item._RectTransform.sizeDelta = new Vector2(w1,UIEnum.ItemH);

		item.ParentFolder = parentFolder;

		//这里这句还是要注意点，文件夹和item混合插入应该会出问题，不过目前没这种需求
		float h = (index + 1) * UIEnum.ItemH;

		item._RectTransform.anchoredPosition = new Vector2 (UIEnum.FolderBtnW*2+4,-h);
		item.CreateMenu(item);

		parentFolder.ItemList.Insert(index,item);
		parentFolder.RefreshItemNum();
		//新建文件夹后通知上级进行重新排序，比如rootFolder下有10个subFolder，在第二个subFolder下新建，那么后面所有subFolder都要重排
		//这里注意下ui坐标系是左下是0,0所以负值是向下移动
		parentFolder.ParentYOrder(-UIEnum.ItemH,index);

		return item;
	}

	/// <summary>
	/// 剪切
	/// 这里等于先复制数据，然后删除
	/// 注意下有哪些数据需要复制
	/// name,icon,dataID
	/// 
	/// 剪切逻辑如下
	/// 1 点击剪切，目标选项变色（剪切态），但并不消失，记录被剪切目标treeItem
	/// 2 当按下黏贴时，才删除原剪切目标
	/// 
	/// 复制逻辑
	/// 1 点击复制，记录被剪切目标treeItem
	/// 2 当复制执行时，新插入一个数据，跟原数据ID不同
	/// </summary>
	private  void Cut()
	{
		//如果之前已经copy或cut数据的处理
		if (null != MyTreeContainer.CurCopyItem)
		{
			//已经剪切了一个数据，然后又去剪切另一个数据的处理，前一个剪切的数据不变恢复状态
			if (MyTreeContainer.CurCopyItem.BeCut)
			{
				MyTreeContainer.CurCopyItem.BeCut = false;
			}
		}

		//删除并保存当前项
		MyTreeContainer.CurCopyItem = this;
		BeCut = true;

//		TreeContainer.CopyItemDataStruct data = new TreeContainer.CopyItemDataStruct();
//		data.Name = Name;
//		data.iconID = ID;//TODO 这是临时的
//		data.dataID = ID;
//		data.bCut = true;
//		MyTreeContainer.CurCopyItemData = data;

		//这里应该改变剪切颜色，而不是直接删除,这步放到MyTreeContainer.CurCopyItemData = data;执行，shit貌似还不行，因为这里cut时copy的不是item而是item的数据，所以无法操作item的BeCut
		//BeCut = true;

		Log.i("TreeItem","Cut","数据剪切，复制数据，并改变被剪切者颜色",BeShowLog);
		//删除
//		ParentFolder.ParentYOrder(UIEnum.ItemH,IndexInParentFolder);
//		ParentFolder.ItemList.Remove(this);
//		Destroy(gameObject);
//		ParentFolder.RefreshItemNum();

		//恢复被禁用的黏贴按钮
		//FolderRightMenu.ItemDic ["黏贴"].interactable = true;
	}

	/// <summary>
	/// 复制
	/// 等于，复制数据
	/// </summary>
	private void Copy()
	{

		//如果之前已经copy或cut数据的处理
		if (null != MyTreeContainer.CurCopyItem)
		{
			//已经剪切了一个数据，然后又去剪切另一个数据的处理，前一个剪切的数据不变恢复状态
			if (MyTreeContainer.CurCopyItem.BeCut)
			{
				MyTreeContainer.CurCopyItem.BeCut = false;
			}
		}

		//删除并保存当前项
		MyTreeContainer.CurCopyItem = this;
		BeCut = false;

//		TreeContainer.CopyItemDataStruct data = new TreeContainer.CopyItemDataStruct();
//		data.Name = Name;
//		data.iconID = 0;
//		data.dataID = 0;
//		MyTreeContainer.CurCopyItemData = data;

		//恢复被禁用的黏贴按钮
		//FolderRightMenu.ItemDic ["黏贴"].interactable = true;
	}

	/// <summary>
	/// 利用剪切版里的数据新建一个Item
	/// 新建一个item
	/// 这里要注意的是创建的位置
	/// 
	/// 当选择一个item并且黏贴时，是把剪切版中的item粘到这个选择item的前面
	/// 原因是如果是后面的化，那么folder下第一个位置永远不能被插入
	/// </summary>
	private void Paste()
	{
		//这里要判定黏贴板上是否有数据
		if(MyTreeContainer.CurCopyItem==null){
			Log.i ("TreeItem","Paste","黏贴的数据为null，不该出现的状态",BeShowLog);
			return;
		}

		//插入TreeItem项到指定位置
		var treeItem = InsertItem (IndexInParentFolder, MyTreeContainer.CurCopyItem.Name, ParentFolder, MyTreeContainer);
		treeItem.Icon = MyTreeContainer.CurCopyItem.Icon;
		treeItem.TreeItemID = MyTreeContainer.CurCopyItem.TreeItemID;
		string _curCopyItemID = MyTreeContainer.CurCopyItem.TreeItemID;
		//如果数据来自剪切，就要清理原数据，如果来自复制，就要复制原数据（数据id如果不变，复制是没有意义的）

		//如果是剪切，要删除前一个状态
		if (MyTreeContainer.CurCopyItem.BeCut) 
		{
			Log.i ("TreeItem","Paste","开始黏贴",BeShowLog);
			MyTreeContainer.CurCopyItem.BeCut = false;
			MyTreeContainer.CurCopyItem.DelItem(false);
		}
		else//如果是copy，要新生成sql数据，拿到ID，然后再插入数据
		{
			//NINFO nsql 这里是否也要向本地写数据
			//这里并不需要向本地Dic写入数据，只向sql写入数据即可，复制并没有把当前选中项置为 复制项，
			//当鼠标点击选中复制项时，选中逻辑自然会把sql数据导入本地
			Log.i ("TreeItem","Paste","开始复制",BeShowLog);
			//step1 创建一个空的默认数据  新建sql并拿回新id
			string kvType = this.MyTreeContainer.Name;
			string[] _defaultDate = GEditorConfig.GetKVDefaultValueData(kvType);
			string[] _fullDefaultDate = GEditorConfig.GetFullKVDefaultValueData(kvType,_defaultDate);

			int newID = GEditorDataMgr.CreateKVSqlData (kvType,_fullDefaultDate);
			treeItem.TreeItemID = newID.ToString();
			Log.i ("TreeItem","Paste","新建数据完毕返回 newID:"+newID,BeShowLog);
			//step2 获取旧数据 根据treeItem.ID（被copy的那个item的sql数据id）查询copy的数据
			//string[] rowNames = GEditorConfig.GetKVRowNames(kvType);
			string[] copyDate = GEditorDataMgr.QueryKVSqlData(kvType/*,rowNames*/,Convert.ToInt32(_curCopyItemID));

			//step3 向新建的空位置添加旧数据，id使用新id
			copyDate[1] = newID.ToString();//特别注意这里是第一个元素，不是第0个，可以参考SQLiteHelper4DataEditor的说明

            string[] Col_Names_InSql = SQLiteHelper4DataEditor.Get_Col_Names_InSql(kvType);
            GEditorDataMgr.ModifyKVSqlData (kvType, Col_Names_InSql, copyDate, newID);
		}

		//黏贴要要清理黏贴版，这里暂时逻辑为无论剪切还是复制，每到黏贴时都要清空剪贴板
		MyTreeContainer.CurCopyItem = null;

		Log.i ("TreeItem","Paste","黏贴的item ID:"+treeItem.TreeItemID,BeShowLog);

	}

	public override void OnSelect ()
	{

		Log.i("TreeItem","OnSelect","当前被选中TreeItem ID:" + TreeItemID,BeShowLog);
		//ItemNameText.color = Color.white;
		ItemTextBGImg.gameObject.SetActive(true);

		//通知KVContain刷新当前选中的treeItem数据
		if (!BeFolder)
		{
			GEditorRoot.GetIns ().KVContainerDic [MyTreeContainer.Name].SwitchTreeItem (TreeItemID);
		}
		else
		{
			//这里如果点中非item，清空KV区域,暂时没找到清理kvitemui的位置
			GEditorRoot.GetIns ().KVContainerDic [MyTreeContainer.Name].ClearKVListUI();

		}
	}

	public override void DeSelect()
	{
		Log.i("TreeItem","OnSelect","取消选中TreeItem ID:" + TreeItemID,BeShowLog);
		//ItemNameText.color = Color.black;
		ItemTextBGImg.gameObject.SetActive(false);
	}

	#endregion

	#region 折叠排序逻辑
	/// <summary>
	/// 重要方法，用于向上一级文件夹汇报H变换
	/// h为正式变大，h为负为变小
	/// 
	/// 描述下具体逻辑
	/// 当一个item(folder)要发生变化(被删除，折叠，打开折叠，剪切)
	/// 要通知上一级folder，把自己在上一级folder中的序号和要改变的h的大小通知上一级ChildYReport
	/// 
	/// 上一级folder中的进一步处理ParentYOrder
	/// 拿到子item传来的消息后，先判断子Item的Index是不是自己folder中所有Item中的最后一个
	/// 如果不是，那么从这个index往后的所有控件(Item)都要执行一次位置移动
	/// 
	/// </summary>
//	protected virtual void ChildYReport(float h,int index)
//	{
//		ParentFolder.ParentYOrder(h,index);
//	}

	public void ChangeY(float h)
	{
		_RectTransform.anchoredPosition = new Vector2 (_RectTransform.anchoredPosition.x,_RectTransform.anchoredPosition.y+h);
	}


	#endregion

	#region 按键处理
	protected override void OnMouseRightClick(PointerEventData eventData)
	{
		//处理下选中
//		if (null != MyMenuList.CurSelItem && 
//			!MyMenuList.CurSelItem.Equals (this)) 
//		{
//			MyMenuList.CurSelItem.DeSelect();
//		}

		MyTreeContainer.CurSelItem = this;
		//this.OnSelect ();

		//计算相对位置并弹出菜单
		Vector2 v = FolderRightMenu.GetMousePosInParentRectTransform(eventData);

        if (MyTreeContainer.CurCopyItem != null) {
            FolderRightMenu.ItemDic["黏贴"].interactable = true;
        }
			
		FolderRightMenu.Open(v);//弹出右键菜单
	}

	protected override void OnMouseLeftSingleClick(PointerEventData eventData)
	{
		
//		if (null != MyMenuList.CurSelItem && 
//			!MyMenuList.CurSelItem.Equals (this)) 
//		{
//			MyMenuList.CurSelItem.DeSelect();
//		}

		MyTreeContainer.CurSelItem = this;
		//this.OnSelect ();
	}
	#endregion
}
