using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
/// <summary>
/// 收集ui相关工具类
/// </summary>
public class UITools{

	/// <summary>
	/// 输入点击事件，返回点击事件在指定ui容器(这里是MenuRoot的RectTrm  就是代码里的ParentTrm)内的相对位置
	/// </summary>
	/// <returns>The mouse position in parent rect transform.</returns>
	/// <param name="eventData">Event data.</param>
	public Vector2 GetMousePosInParentRectTransform(Transform ParentTrm,PointerEventData eventData)
	{
		Vector2 pos = new Vector2 (0,0);
		var rt = ParentTrm.GetComponent<RectTransform> ();
		RectTransformUtility.ScreenPointToLocalPointInRectangle (rt, eventData.position, eventData.pressEventCamera, out pos);  
		return pos;
	}
}
