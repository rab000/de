using UnityEngine;  
using UnityEngine.UI;  
using UnityEngine.EventSystems;  
using System.Collections;  

public class DragableDialog : MonoBehaviour , IPointerDownHandler, IDragHandler {

	// 鼠标起点  
	private Vector2 originalLocalPointerPosition;     
	// 面板起点  
	private Vector3 originalPanelLocalPosition;  
	// 当前要移动的面板  
	public RectTransform _PanelRectTransform; 
	public RectTransform PanelRectTransform{
		set
		{ 
			_PanelRectTransform = value;
		}
		get{
			if (null == _PanelRectTransform)
				_PanelRectTransform = transform as RectTransform;
			return _PanelRectTransform; 
		}

	}

	// 父节点,这个最好是UI父节点，因为它的矩形大小刚好是屏幕大小  
	public RectTransform _ParentRectTransform;  
	public RectTransform ParentRectTransform{
		set
		{
			_ParentRectTransform = value;
		}
		get
		{ 
			if (null == _ParentRectTransform)
				_ParentRectTransform = transform.parent as RectTransform;
			return _ParentRectTransform; 
		}
	}

	//private static int siblingIndex = 0;  

	// 鼠标按下  
	public void OnPointerDown (PointerEventData data) {  
		//siblingIndex++;  
		//panelRectTransform.transform.SetSiblingIndex(siblingIndex);  
		// 记录当前面板起点  
		originalPanelLocalPosition = PanelRectTransform.localPosition;  
		// 通过屏幕中的鼠标点，获取在父节点中的鼠标点  
		// parentRectTransform:父节点  
		// data.position:当前鼠标位置  
		// data.pressEventCamera:当前事件的摄像机  
		// originalLocalPointerPosition:获取当前鼠标起点  
		RectTransformUtility.ScreenPointToLocalPointInRectangle (ParentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);  
	}  
	// 拖动  
	public void OnDrag (PointerEventData data) {  
		if (PanelRectTransform == null || ParentRectTransform == null)  
			return;  
		Vector2 localPointerPosition;  
		// 获取本地鼠标位置  
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (ParentRectTransform, data.position, data.pressEventCamera, out localPointerPosition)) {  
			// 移动位置 = 本地鼠标当前位置 - 本地鼠标起点位置  
			Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;  
			// 当前面板位置 = 面板起点 + 移动位置  
			PanelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;  
		}  
		ClampToWindow ();  
	}  

	// 限制当前面板在父节点中的区域位置  
	void ClampToWindow () {  
		// 面板位置  
		Vector3 pos = PanelRectTransform.localPosition;  

		// 如果是UI父节点，设置面板大小为0，那么最大最小位置为正负屏幕的一半  
		Vector3 minPosition = ParentRectTransform.rect.min - PanelRectTransform.rect.min;  
		Vector3 maxPosition = ParentRectTransform.rect.max - PanelRectTransform.rect.max;  

		pos.x = Mathf.Clamp (PanelRectTransform.localPosition.x, minPosition.x, maxPosition.x);  
		pos.y = Mathf.Clamp (PanelRectTransform.localPosition.y, minPosition.y, maxPosition.y);  
		//VLog.I ("miny:"+minPosition.y+" maxy:"+maxPosition.y);
		PanelRectTransform.localPosition = pos;  
	}  

}
