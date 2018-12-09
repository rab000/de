using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ListItem : SelectableItem {
	/// <summary>
	/// ListItem在容器中的顺序位置
	/// 对于KVListItem来说，一个KVListItem包含3个数据key，value，type，但是一个sql包含1个数据
	/// 那么通过这个Index就能找到key，value，type在sql分别对应的位置  
	/// Index就是key在sql中的顺序位置
	/// Index+1就是value在sql中的顺序位置
	/// Index+2就是type在sql中的顺序位置
	/// </summary>
	[HideInInspector]public int Index;
	[HideInInspector]public ListItem Next;
	[SerializeField]protected Text Content;
	[SerializeField]protected GameObject SelImgGo;
	protected bool BeSelected = false;
	protected KVContainer _MyKVContainer;
	public virtual void Reset()
	{
		Index = -1;
		SetContent ("");
		SetKVMgr(null);
		Next = null;
	}

	public void SetContent(string s){
		if (null == Content)
			return;
		Content.text = s;
	}

	public void SetKVMgr(KVContainer mgr){_MyKVContainer = mgr;}

	public override void OnSelect(){
		SelImgGo.SetActive (true);
		BeSelected = true;
	}

	public override void DeSelect(){
		SelImgGo.SetActive(false);
		BeSelected = false;
	}

	protected override void OnMouseLeftSingleClick(PointerEventData eventData)
	{
		_MyKVContainer.CurSelectedKVItem = this;
	}

	protected override void OnMouseRightClick(PointerEventData eventData)
	{
		_MyKVContainer.CurSelectedKVItem = this;
	}

	protected override void OnMouseLeftDoubleClick(PointerEventData eventData)
	{
		_MyKVContainer.CurSelectedKVItem = this;
	}



}
