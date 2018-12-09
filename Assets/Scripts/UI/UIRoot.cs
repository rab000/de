using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// 绑定在根部Canvas上
/// 菜单弹出时，点击空白区域要关闭菜单
/// 需要有个区域接受点击，收到点击并且没有在当前菜单区域，则隐藏当前菜单
/// 
/// 
/// 加上这个所有上层点击失效，暂时没绑这段代码
/// </summary>
public class UIRoot : MonoBehaviour ,IPointerDownHandler{

	public void OnPointerDown (PointerEventData data)
	{
		if (null == Menu.CurMenu)return;
		Vector2 v = new Vector2 (0,0);
		bool b = RectTransformUtility.ScreenPointToLocalPointInRectangle (Menu.CurMenu._RectTransform, data.position, data.pressEventCamera, out v); 
		if (!b) 
		{
			Menu.CurMenu.Hide ();
		}
		Debug.Log("Canvas Click!!!");
	}
}
