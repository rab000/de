using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
public class KVListItem : ListItem {

	private bool BeShowLog = true;

	/// <summary>
	/// 如果是Key，则brother就是Value，相反一样
	/// </summary>
	[HideInInspector]public KVListItem BrotherItem;
	[HideInInspector]public bool IsKey; //不是key就是value
	[HideInInspector]public int TreeItemID; //记录下KVListItem属于哪个TreeItemID
	//[HideInInspector]public string DialogOpenType; //打开(Dialog)方式

	public override void Reset()
	{
		base.Reset ();
		BrotherItem = null;
		IsKey = true;
		//DialogOpenType = GEditorConfig.DLG_TYPE_stringValue;
		transform.SetParent(null);
		gameObject.SetActive(false);
	}

	public override void OnSelect()
	{
		base.OnSelect();
		if (null != BrotherItem) {
			if(!BrotherItem.BeSelected)BrotherItem.OnSelect();
		}
		else {
			Log.i("KVListItem","OnSelect","BrotherItem=null name:"+this.gameObject.name,BeShowLog);
		}
	}

	public override void DeSelect()
	{
		base.DeSelect();
		if (null != BrotherItem) {
			if(BrotherItem.BeSelected)BrotherItem.DeSelect();
		}
		else {
			Log.i("KVListItem","DeSelect","BrotherItem=null  name:"+this.gameObject.name,BeShowLog);
		}
	}

	protected override void OnMouseLeftDoubleClick(PointerEventData eventData)
	{
		base.OnMouseLeftDoubleClick (eventData);

		//if (_MyKVContainer.KVDateDic.ContainsKey (TreeItemID.ToString ())) {
		//	Debug.Log ("==========>包含:" + TreeItemID.ToString ());
		//}
		//else
		//{
		//	Debug.Log ("==========>不包含:" + TreeItemID.ToString ());
		//}

		//if (_MyKVContainer.KVDateDic[TreeItemID.ToString()].ContainsKey(Index)) {
		//	Debug.Log ("==========>包含Index:" + Index);
		//}
		//else
		//{
		//	Debug.Log ("==========>不包含Index:" + Index);
		//}

		KVData data = _MyKVContainer.KVDateDic[TreeItemID.ToString()][Index];
		string title = "title";
		string key = data.key;
		string value = data.value;
		string type = data.type;
        Log.i("KVListItem", "OnMouseLeftDoubleClick","Index:"+Index+" type:"+type+" key:"+key+" value:"+value);
		KVDataModifyDialog.Open(OnDialogReturnValue, _MyKVContainer.Name,title, data.key,value,type);


	}

	/// <summary>
	/// 接受dialog返回数据
	/// </summary>
	/// <param name="bs">Bs.</param>
	private void OnDialogReturnValue(byte[] bs)
	{
		IoBuffer _ib = new IoBuffer ();
		_ib.PutBytes(bs);

		string _key = _ib.GetString ();		//从输入框返回要修改的key值
		string _value = _ib.GetString ();	//从输入框返回的要修改的value值
		string _type = _ib.GetString ();    //从输入框返回的修改类型值，比如字符串类型，或者列表类型

		string _TreeItemIDStr = TreeItemID.ToString ();
        //Log.i("KVListItem", "OnDialogReturnValue", "=======>iNDEX:" + Index, BeShowLog);
       
		_MyKVContainer.ModifyKVItem (_TreeItemIDStr,Index,_key,_value,_type);

		_ib.Clear ();
		_ib = null;
	}
}
