using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//TODO Tab少实现一个功能，目前实现方法是
//首次从TabContainer.OPENED_EDITORS读默认打开的编辑器,关闭时保存打开编辑器数据到配置文件
//非首次的化就从配置文件读取(已经打开的编辑器)
//对于默认没打开的tab没有处理，以后再处理，先记录下
//扩展时涉及到几个位置注意下,
//1GEditorRoot.InitGEditor()中注意一点，向TabContainer中Add后，还需要处理后续添加tree相关的操作，这跟整体构架有关，因为tab不包括tree所以tree的初始化不能由tab完成
//2MenuPanel.OnSkillClick等开启隐藏tab的回调都是空实现
//3目前来讲，可以直接在GEidotrEnum.OPENED_EDITORS扩展默认打开的编辑器，除非编辑器特别多时，再扩展隐藏编辑器相关功能

/// <summary>
/// 负责管理多个TabItem
/// 控制横向排布还是竖向排布
/// 管理选项卡移动后重新排布事件
/// TabContainer与MenuList一样,只存储根节点，不挂载到具体节点上(其实是否挂载到根节点没什么区别，反正已经存储了根节点)
/// </summary>
public class TabContainer{

	public enum TabLayoutType
	{
		vertical,
		horizontal,
		other
	}

	private RectTransform rectTrm;

	public Dictionary<string,TabItem> TabItemDic;

	private TabLayoutType type;

	/// <summary>
	/// Tab改变选中时对外发送事件
	/// </summary>
	public Action<string> TabSelectEvent;
	public Action<string> TabDeSelectEvent;

	private TabItem _CurTabItem;
	public TabItem CurTabItem{
		set
		{
			//如果多次设置同一个值，不做任何处理
			if (value.Equals (_CurTabItem))
				return;

			//处理之前的item丢失选择的事件
			if (null != _CurTabItem) {
				_CurTabItem.BeSelecte = false;
				if (null != TabDeSelectEvent)TabDeSelectEvent(_CurTabItem._TabData.RefName);
			}

			_CurTabItem = value;

			//处理新item被选中事件
			if (null != TabSelectEvent)TabSelectEvent(_CurTabItem._TabData.RefName);
				
		}
		get{
			return _CurTabItem;
		}
	}

	/// <summary>
	/// 初始化就需要觉得TabContainer的类型
	/// 这里涉及到后面TabItem的具体排序规则
	/// </summary>
	/// <param name="type">Type.</param>
	public TabContainer(TabLayoutType type,RectTransform tabContainerTrm)
	{
		this.type = type;
		rectTrm = tabContainerTrm;
		if (null == TabItemDic)TabItemDic = new Dictionary<string, TabItem> ();
	}

	//TODO 这个方法并没有使用和实现,目前实现方法是
	//首次从TabContainer.OPENED_EDITORS读默认打开的编辑器,关闭时保存打开编辑器数据到配置文件
	//非首次的化就从配置文件读取(已经打开的编辑器)
	/// <summary>
	/// 使用(已存在)数据构建Tab结构
	/// 由外部负责找到数据，ui框架本身不负责查找数据的具体业务
	/// </summary>
	/// <param name="bs">Bs.</param>
	public void BuildByData(byte[] bs)
	{
		IoBuffer ib = new IoBuffer();
		ib.PutBytes(bs);

	}

	public void Add(TabItem item)
	{
		//排序
		var rt = item.GetComponent<RectTransform>();
		int offx = TabItemDic.Count * 100;
		rt.offsetMin = new Vector2(offx,rt.offsetMin.y);
		rt.sizeDelta = new Vector2(UIEnum.TabW,rt.sizeDelta.y);//这里如果不设置w会变为0，暂时不明白原因
		TabItemDic.Add (item._TabData.Name,item);

	}

	public void Del(string itemName)
	{
		if (!TabItemDic.ContainsKey (itemName)) 
		{
			Debug.LogError("TabContainer.Del--->TabItemDic dont contain key:" + itemName);
			return;
		}

		TabItemDic.Remove(itemName);
	}



}
