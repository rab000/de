using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System;
/// <summary>
/// 菜单列表文件夹
/// 比菜单列表项多了可折叠功能
/// </summary>
public class TreeFolder : TreeItem 
{
	private bool BeShowLog = true;
	/// <summary>
	/// 打开关闭文件夹Button
	/// </summary>
	public Button FolderBtn;

	/// <summary>
	/// 加减号按钮文字
	/// </summary>
	[SerializeField]private Text ButtonText;

	/// <summary>
	/// 文件夹小图标
	/// </summary>
	[SerializeField]private Image FolderCloseImage;
	[SerializeField]private Image FolderOpenImage;

	/// <summary>
	/// 文件夹下的文件夹或item绑定在文件夹的这个节点下
	/// </summary>
	public RectTransform SubRootRectTransform;

	/// <summary>
	/// 是否是根文件夹
	/// </summary>
	[HideInInspector]public bool BeRootFolder;

	/// <summary>
	/// 文件夹是否打开
	/// </summary>
	private bool _BeFolderOpen;
	[HideInInspector]public bool BeFolderOpen{
		get
		{ 
			return _BeFolderOpen;
		}
		set
		{ 
			_BeFolderOpen = value;

			if (_BeFolderOpen) {
				FolderOpenImage.gameObject.SetActive(true);
				FolderCloseImage.gameObject.SetActive(false);
				ButtonText.text = "-";
			} 
			else 
			{
				FolderOpenImage.gameObject.SetActive(false);
				FolderCloseImage.gameObject.SetActive(true);
				ButtonText.text = "+";
			}
		}
	}

	/// <summary>
	/// 包含MenuListItem数目
	/// 注意这里只计算item，不计算任何folder
	/// 所有子节点的item也要计数
	/// </summary>
	public int SubAllItemNums{
		get
		{ 
			int num = 0;

			for (int i = 0; i < ItemList.Count; i++) {
				var item = ItemList[i];

				if (item.BeFolder) 
				{
					var folder = item as TreeFolder;
					num += folder.SubItemNums;
				} 
				else 
				{	
					num++;	
				}

			}
			return num;
		}	
	}

	/// <summary>
	/// 当前文件夹节点下面的item数，不计算子节点，包括文件夹
	/// </summary>
	/// <value>The sub item nums.</value>
	public int SubItemNums{
		get{ 
			return ItemList.Count;
		}
	}
	/// <summary>
	/// 返回当前文件夹占用(包括文件夹本身)
	/// 依次计算当前文件夹内部每个文件夹的H(里面无论打开多少级都没问题，文件夹和文件混排也可以支持)
	/// </summary>
	/// <value>The current h.</value>
	public float CurFolderH{
		get
		{
			float h = 0;
			if (BeFolderOpen) 
			{
				h += UIEnum.FolderH;//把自己的高度算进去

				h += CurSubItemH;
//				for(int i=0;i<SubItemNums;i++)
//				{
//					if (ItemList [i].BeFolder) 
//					{
//						MenuListFolder folder = ItemList [i] as MenuListFolder;
//						h += folder.CurFolderH;
//					} 
//					else 
//					{
//						h += MenuListItem.ItemH;
//					}
//				}
			} 
			else 
			{
				h = UIEnum.FolderH;
			}
			return h;
		}
	}

	/// <summary>
	/// 返回文件夹下所有子item长度占用
	/// </summary>
	/// <value>The current sub item h.</value>
	public float CurSubItemH{
		get
		{
			float h = 0;

			//依次计算当前文件夹内部每个文件夹的H(里面无论打开多少级都没问题，文件夹和文件混排也可以支持)
			for(int i=0;i<SubItemNums;i++)
			{
				if (ItemList [i].BeFolder) 
				{
					TreeFolder folder = ItemList [i] as TreeFolder;
					h += folder.CurFolderH;
				} 
				else 
				{
					h += UIEnum.ItemH;
				}
			}

			return h;
		}
	}



	[HideInInspector]public List<TreeItem> ItemList;




	protected override void  Awake()
	{
		base.Awake();
		ItemList = new List<TreeItem>();

	}

	public void RefreshItemNum()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append (Name);
		sb.Append ('(');
		sb.Append (SubAllItemNums);
		sb.Append (')');
		ItemNameText.text = sb.ToString();
		ItemNameText.fontSize = UIEnum.FontSize;
		var textRt = ItemNameText.gameObject.GetComponent<RectTransform>();
		float w = Tools.GetStringW (ItemNameText.text,UIEnum.FontSize);
		textRt.sizeDelta = new Vector2(w,UIEnum.ItmeH);

		//重新设置下Folder的边框宽度
		float w1 = UIEnum.FolderBtnW+UIEnum.FolderImageW+w;
		_RectTransform.sizeDelta = new Vector2(w1,UIEnum.FolderH);

		if (!BeRootFolder)
			ParentFolder.RefreshItemNum ();

	}

	#region 创建文件夹
	/// <summary>
	/// 创建根文件夹
	/// </summary>
	/// <returns>The root.</returns>
	/// <param name="name">Name.</param>
	/// <param name="parentTrm">Parent trm.</param>
	/// <param name="offx">Offx.</param>
	/// <param name="offy">Offy.</param>
	public static TreeFolder CreateRootFolder(string rootFolderName,RectTransform parentTrm,TreeContainer menuList,float offx,float offy)
	{
		TreeFolder folder = TreeFolder.Create (rootFolderName, parentTrm,menuList);
		folder.ParentFolder = null;
		folder.BeRootFolder = true;
		//设置PosX,PosY
		folder._RectTransform.anchoredPosition = new Vector2 (offx-UIEnum.FolderBtnW,offy);
		//根目录去掉+-号显示
		folder.FolderBtn.gameObject.SetActive(false);
		//创建菜单
		folder.CreateMenu(folder);
		return folder;
	}

	/// <summary>
	/// 创建子文件夹
	/// 不需要加入xy偏移量，因为这个偏移量是固定的
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="parentTrm">Parent trm.</param>
	public static TreeFolder CreateSubFolder(string folderName,TreeFolder parentFolder,TreeContainer menuList)
	{
		//添加文件夹时，如果父文件夹没有打开，先打开再添加
		if (!parentFolder.BeFolderOpen)
			parentFolder.BeFolderOpen = true;

		var folder = TreeFolder.Create(folderName,parentFolder.SubRootRectTransform,menuList);
		folder.ParentFolder = parentFolder;
		folder.BeRootFolder = false;

		//这里计算的是相对父folder的位置
		//float h = (parentFolder.SubItemNums + 1) * MenuListFolder.FolderH;
		float h = parentFolder.CurFolderH;
		folder._RectTransform.anchoredPosition = new Vector2 (UIEnum.FolderBtnW,-h);
		folder.CreateMenu(folder);
		parentFolder.ItemList.Add(folder);

		//新建文件夹后通知上级进行重新排序，比如rootFolder下有10个subFolder，在第二个subFolder下新建，那么后面所有subFolder都要重排
		//这里注意下ui坐标系是左下是0,0所以负值是向下移动
		parentFolder.ParentYOrder(-UIEnum.FolderH,folder.IndexInParentFolder);


		//刷新下MenuList的边框宽，用于刷新MenuList滚动区域,item距离Menulist左边距离+两个图标间隔+4(文字和图标间隔)+w1(文字长度)
		if(!folder.BeRootFolder){
			var textRt = folder.ItemNameText.gameObject.GetComponent<RectTransform>();
			float w = Tools.GetStringW (folder.ItemNameText.text,UIEnum.FontSize);
			float w1 = UIEnum.FolderBtnW+UIEnum.FolderImageW+w;
			float _w = folder.CurItemLeftOffset + UIEnum.ItemImageW*2 + 4 +w1; 
			menuList.SetTreeW(_w);
			menuList.RefreshTreeArea ();
		}

		return folder;
	}

	public static TreeFolder Create(string folderName,RectTransform parentRectTrm,TreeContainer menuList)
	{
		//实例化文件夹
		GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/TreeFolder")) as GameObject;
		go.name = folderName;
		TreeFolder folder = go.GetComponent<TreeFolder>();
		folder.MyTreeContainer = menuList;
		folder.BeFolder = true;
		folder.Name = folderName;
		go.transform.SetParent(parentRectTrm as Transform);
		folder._RectTransform = go.GetComponent<RectTransform>();
		folder._RectTransform.pivot = new Vector2(0,1);
		folder._RectTransform.anchorMax = new Vector2 (0,1);
		folder._RectTransform.anchorMin = new Vector2 (0,1);

		//设置下文件夹文字及大小
		StringBuilder sb = new StringBuilder();
		sb.Append (folderName);
		sb.Append ('(');
		sb.Append (folder.SubItemNums);
		sb.Append (')');
		folder.ItemNameText.text = sb.ToString();
		folder.ItemNameText.fontSize = UIEnum.FontSize;
		var textRt = folder.ItemNameText.gameObject.GetComponent<RectTransform>();
		float w = Tools.GetStringW (folder.ItemNameText.text,UIEnum.FontSize);
		textRt.sizeDelta = new Vector2(w,UIEnum.ItmeH);

		//重新设置下Folder的边框宽度
		float w1 = UIEnum.FolderBtnW+UIEnum.FolderImageW+w;
		folder._RectTransform.sizeDelta = new Vector2(w1,UIEnum.FolderH);

		//设置+-打开关闭文件夹button事件
		folder.FolderBtn.onClick.AddListener(folder.OnButtonClick);



			
		return folder;
	}
	#endregion



	#region 折叠排序逻辑



	private void OpenFolder()
	{
		BeFolderOpen = !BeFolderOpen;

		int flag = BeFolderOpen ? -1 : 1;

		TreeItem item = null;
		for (int i = 0; i < SubItemNums; i++) {
			item = ItemList [i];
			if (BeFolderOpen)
				item.gameObject.SetActive (true);
			else
				item.gameObject.SetActive (false);
			//显示(隐藏)子项,这里没必要计算，显示和隐藏不会改变子Item相对于父Folder的相对位置
			//float _h = ((i + 1) * FolderH)*flag;
			//item.ChangeY(_h);
		}
			
		//计算增长缩小高度
		float h = this.CurSubItemH*flag;
		//通知父文件夹排序
		if (!BeRootFolder) 
		{
			ParentFolder.ParentYOrder (h,IndexInParentFolder);
		}

	}
		
	/// <summary>
	/// 重要方法，用于向上一级文件夹汇报H变换
	/// h为正式变大，h为负为变小
	/// 
	/// 描述下具体逻辑
	/// 当一个item(folder)要发生变化(被删除，折叠，打开折叠，剪切)
	/// 要通知上一级folder，把自己在上一级folder中的序号和要改变的h的大小通知上一级MenuListItem.ChildYReport
	/// 
	/// 上一级folder中的进一步处理MenuListFolder.ParentYOrder
	/// 拿到子item传来的消息后，先判断子Item的Index是不是自己folder中所有Item中的最后一个
	/// 如果不是，那么从这个index往后的所有控件(Item)都要执行一次位置移动
	/// 
	/// </summary>
	public void ParentYOrder(float h,int index)
	{
		for (int i = index + 1; i < SubItemNums; i++) 
		{
			ItemList [i].ChangeY (h);
		}

		if (BeRootFolder)//如果已经执行到根部，就不要再向上传递了
			return;
		else 
		{
			ParentFolder.ParentYOrder(h,IndexInParentFolder);
		}

	}

	/// <summary>
	/// 计算子Item相对与父folder的相对高度H
	/// 
	/// TODO 使用情景需要补充说明
	/// 
	/// 传入folder在父folder中的序号index
	/// 返回从父folder到这个序号index之间实际的h
	/// 需要计算内部每个文件夹是否打开，总共暂用了多少位置
	/// </summary>
	/// <returns>The H from current h.</returns>
	/// <param name="index">item在文件夹中的index</param>
	public float GetFrontH(int subItemIndexInParentFolder)
	{
		float h = 0;

		for (int i = 0; i < subItemIndexInParentFolder; i++) {

			bool bFolder = ItemList [i].BeFolder;
			if (bFolder) 
			{
				TreeFolder folder = ItemList [i] as TreeFolder;
				h += folder.CurFolderH;
			}
			else
			{
				h += UIEnum.ItemH;
			}

		}
		return h;
	}



	/// <summary>
	/// 查询子item在父folder中的index
	/// </summary>
	/// <returns>The item index in folder.</returns>
	public int GetItemIndexInFolder(TreeItem item)
	{
		int index = -1;
		for (int i = 0; i < SubItemNums; i++) 
		{
			if (item.Equals (ItemList [i]))
				index = i;
		}
		return index;
	}

	#endregion

	#region 菜单功能区,说明，这里应该把菜单相关都提出去，Tree作为一个ui框架，只可以包含一个菜单接口（可以foler和item各一个menu），具体菜单实现应该由外部实现，这样才能保证Tree的独立性和复用性

	public void CreateMenu(TreeFolder menuListFolder){
		if (null != FolderRightMenu)return;
		Dictionary<string,Listener> dic = new Dictionary<string, Listener> ();
		dic.Add ("创建文件夹", menuListFolder.AddFolder);
		if (!BeRootFolder) 
		{
			dic.Add ("删除文件夹", menuListFolder.DelFolder);
		}
		else 
		{
			Listener callback =()=>{
				GEditorDataMgr.GenerateGameData(Name);
			};

			dic.Add ("导出"+Name+"数据", callback);
		}
		dic.Add ("创建文件", menuListFolder.AddItem);
		dic.Add ("重命名", menuListFolder.ReName);
        dic.Add ("黏贴", menuListFolder.Paste);
        FolderRightMenu = Menu.Generate(dic,"FolderMenu");
        FolderRightMenu.ItemDic["黏贴"].interactable = false;//默认禁用黏贴
    }

	/// <summary>
	/// 新增文件夹
	/// </summary>
	public void AddFolder()
	{
		InputTextDialog.Open(
			delegate(byte[] bts) 
			{
				//创建文件夹
				IoBuffer ib = new IoBuffer();
				ib.PutBytes(bts);
				string inputFieldText = ib.GetString();
				if(!BeFolderOpen)OpenFolder();//没打开的先打开再创建，否则位置错乱
				TreeFolder.CreateSubFolder(inputFieldText,this,MyTreeContainer);

			}
		);
	}

	/// <summary>
	/// 使用外部数据填充增加folder
	/// </summary>
	/// <param name="ib">Ib.</param>
	public void AddFolderByData(IoBuffer ib)
	{
		bool bFolder = false;
		string name = ib.GetString();
		int listSize = ib.GetInt();

		if(!BeFolderOpen)OpenFolder();
		TreeFolder curTreeFolder = TreeFolder.CreateSubFolder(name,this,MyTreeContainer);


		for (int i = 0; i < listSize; i++)
		{
			//CreateOneItem(ib);
			bFolder = ib.GetBool();

			if (bFolder) {
				curTreeFolder.AddFolderByData (ib);
			} else {
				curTreeFolder.AddItemByData (ib);
			}
		}
	}


	/// <summary>
	/// 插入文件夹
	/// </summary>
	private void InsertFolder()
	{

	}

	private void DelFolder()
	{
		//TODO 这里还要Folder中所有的item
		OKDialog.Open (
			delegate() {
				//先做删除文件夹后的排序
				ParentFolder.ParentYOrder(CurFolderH,IndexInParentFolder);
				//然后把folder从父类ListItem中去掉
				ParentFolder.ItemList.Remove(this);
				//再删除folder节点
				Destroy(gameObject);
			},
			"确定删除文件夹？"
		);
	}

	public void AddItem()
	{
		InputTextDialog.Open(
			delegate(byte[] bts) 
			{
				//创建文件夹
				IoBuffer ib = new IoBuffer();
				ib.PutBytes(bts);
				string inputFieldText = ib.GetString();
				if(!BeFolderOpen)OpenFolder();//没打开的先打开再创建，否则位置错乱
				var ti = TreeItem.Create(inputFieldText,this,MyTreeContainer);


				//nafio add 170623 这段代码有点冗余应该赋值一次就可以，类似重复的代码也需要整理下
//				if (null != MyMenuList.CurSelItem && 
//					!MyMenuList.CurSelItem.Equals (ti)) 
//				{
//					MyMenuList.CurSelItem.DeSelect();
//				}

				ti.TreeItemID = GEditorRoot.GetIns().KVContainerDic[MyTreeContainer.Name].CreateKVList(MyTreeContainer.Name);


				MyTreeContainer.CurSelItem = ti;
				//ti.OnSelect ();
		
				Log.i("TreeFolder","AddItem","新建item name:"+inputFieldText+" id:"+ti.TreeItemID,BeShowLog);
			}
		);
	}

    /// <summary>
    /// 这里的黏贴是treeItme的黏贴，不存在文件夹整体黏贴的说法
    /// treefolder的黏贴跟TreeItem有些不同(除了这些不同其他都相同，跟treeitem paste的操作类似)
    /// treeItem是把黏贴项放到自己之前
    /// treefoler是把黏贴项放到自己内部所有元素之后
    /// </summary>
    public void Paste()
    {
        //这里要判定黏贴板上是否有数据
        if (MyTreeContainer.CurCopyItem == null)
        {
            Log.i("TreeItem", "Paste", "黏贴的数据为null，不该出现的状态", BeShowLog);
            return;
        }

        if (!BeFolderOpen) OpenFolder();//没打开的先打开再创建，否则位置错乱

        //创建新位置的item的ui，ui节点名称与黏贴板上的一致
        var ti = TreeItem.Create(MyTreeContainer.CurCopyItem.Name, this, MyTreeContainer);
       
        //如果是剪切，要删除前一个状态
        if (MyTreeContainer.CurCopyItem.BeCut)
        {
            Log.i("TreeItem", "Paste", "开始黏贴", BeShowLog);
            MyTreeContainer.CurCopyItem.BeCut = false;
            MyTreeContainer.CurCopyItem.DelItem(false);
            ti.TreeItemID = MyTreeContainer.CurCopyItem.TreeItemID;//黏贴的话id保持不变
        }
        else//如果是copy，要新生成sql数据，拿到ID，然后再插入数据
        {
            //step1 创建一个空的默认数据  新建sql并拿回新id
            string kvType = this.MyTreeContainer.Name;
            string[] _defaultDate = GEditorConfig.GetKVDefaultValueData(kvType);
            string[] _fullDefaultDate = GEditorConfig.GetFullKVDefaultValueData(kvType, _defaultDate);
            int newID = GEditorDataMgr.CreateKVSqlData(kvType, _fullDefaultDate);

            ti.TreeItemID = newID.ToString();
            Log.i("TreeFolder", "Paste", "新建数据完毕返回 newID:" + newID, BeShowLog);
            //step2 获取旧数据 根据treeItem.ID（被copy的那个item的sql数据id）查询copy的数据
            //string[] rowNames = GEditorConfig.GetKVRowNames(kvType);
            string[] copyDate = GEditorDataMgr.QueryKVSqlData(kvType/*,rowNames*/, Convert.ToInt32(MyTreeContainer.CurCopyItem.TreeItemID));

            //step3 向新建的空位置添加旧数据，id使用新id
            copyDate[1] = newID.ToString();//特别注意这里是第一个元素，不是第0个，可以参考SQLiteHelper4DataEditor的说明

            string[] Col_Names_InSql = SQLiteHelper4DataEditor.Get_Col_Names_InSql(kvType);

            GEditorDataMgr.ModifyKVSqlData(kvType, Col_Names_InSql, copyDate, newID);

        }
        //黏贴要要清理黏贴版，这里暂时逻辑为无论剪切还是复制，每到黏贴时都要请客剪贴板
        MyTreeContainer.CurCopyItem = null;

        Log.i("TreeItem", "Paste", "黏贴的item ID:" + ti.TreeItemID, BeShowLog);

    }
	/// <summary>
	/// 使用数据填充具体item项
	/// 这个是app开始从配置中读数据
	/// 不需要向sql写入
	/// </summary>
	/// <param name="ib">Ib.</param>
	public void AddItemByData(IoBuffer ib)
	{
		string name = ib.GetString ();
		string id = ib.GetString();
		var treeItem = TreeItem.Create(name,this,MyTreeContainer);
		treeItem.TreeItemID = id;
	}

	protected override void ResizeTextRect(string newName)
	{
		//重新设置文件夹名称，并重置text和text父控件大小
		StringBuilder sb = new StringBuilder();
		sb.Append (newName);
		sb.Append ('(');
		sb.Append (this.SubItemNums);
		sb.Append (')');
		this.ItemNameText.text = sb.ToString();
		this.ItemNameText.fontSize = UIEnum.FontSize;
		var textRt = this.ItemNameText.gameObject.GetComponent<RectTransform>();
		float w = Tools.GetStringW (this.ItemNameText.text,UIEnum.FontSize);
		textRt.sizeDelta = new Vector2(w,UIEnum.ItmeH);

		//重新设置下Folder的边框宽度
		float w1 = UIEnum.FolderBtnW+UIEnum.FolderImageW+w;
		this._RectTransform.sizeDelta = new Vector2(w1,UIEnum.FolderH);
	}

	#endregion

	#region 鼠标及按钮点击事件
	protected override void OnMouseLeftDoubleClick(PointerEventData eventData)
	{
		OnButtonClick ();
	}

	/// <summary>
	/// 加减号按钮按下事件
	/// </summary>
	public void OnButtonClick()
	{
		
		OpenFolder ();

	}
	#endregion
		
}
