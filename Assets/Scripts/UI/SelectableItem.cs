using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SelectableItem : MonoBehaviour ,IPointerClickHandler{

	[HideInInspector]
	public RectTransform _RectTransform;

	protected virtual void  Awake()
	{
		_RectTransform = gameObject.GetComponent<RectTransform> ();
	}
		
	public virtual void OnSelect()
	{
	
	}

	public virtual void DeSelect()
	{
	
	}

	protected virtual void OnMouseLeftSingleClick(PointerEventData eventData)
	{
		//显示刷新keyValue列表
	}
		
	protected virtual void OnMouseRightClick(PointerEventData eventData)
	{
		//弹出右键菜单
	}

	protected virtual void OnMouseLeftDoubleClick(PointerEventData eventData)
	{
		//显示刷新keyValue列表
	}



	public void OnPointerClick (PointerEventData eventData)
	{
		switch (eventData.button) 
		{
		case PointerEventData.InputButton.Left:
			if (eventData.clickCount == 2) 
			{
				OnMouseLeftDoubleClick(eventData);
			} 
			else 
			{
				OnMouseLeftSingleClick(eventData);
			}
			break;
		case PointerEventData.InputButton.Right:
			OnMouseRightClick( eventData);
			break;
		}
	}
		
}
