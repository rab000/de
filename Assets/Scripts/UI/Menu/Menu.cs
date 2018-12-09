using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
/// <summary>
/// 菜单
/// 使用方法
/// Menu绑定在名为Menu的Prefab(就是个普通panel)上
/// 使用静态方法Generate产生菜单
/// 创建后会绑定在Canvas下名为MenuRoot的节点上
/// </summary>
public class Menu:MonoBehaviour
{
	/// <summary>
	/// item高
	/// </summary>
	public const int MenuItemH = 25;
	public const int MenuItemW = 100;

	//创建的菜单会绑定在这个名字的节点下面
	public const string MenuRootName = "MenuRoot";

	/// <summary>
	/// 同一时间只需要一个menu
	/// 当一个新menu打开时，旧的menu需要关掉
	/// 这个CurMenu保存当前使用的Menu
	/// 当新menu打开时先关闭当前menu
	/// 然后在新打开的menu赋值给CurMenu
	/// </summary>
	public static Menu CurMenu;

	public Dictionary<string,Button> ItemDic;

	private RectTransform _rectTransform;
	public RectTransform _RectTransform{
		get{ 
			if (null == _rectTransform)
				_rectTransform = gameObject.GetComponent<RectTransform> ();

			return _rectTransform;
		}
	}

	[HideInInspector]public Transform ParentTrm;

	/// <summary>
	/// 生成一个菜单
	/// Dic存储每个item的名称和回调函数 
	/// </summary>
	/// <param name="InitDic">Init dic.</param>
	public static Menu Generate(Dictionary<string,Listener> InitDic,string menuName)
	{
		GameObject go = GameObject.Instantiate(Resources.Load ("Prefabs/Menu")) as GameObject;
		go.name = menuName;
		Menu menu = go.GetComponent<Menu>();
		go.transform.SetParent(menu.ParentTrm);

		var rt = go.GetComponent<RectTransform>();
		//menu的这个设置跟RectTransformUtility.ScreenPointToLocalPointInRectangle的返回结果正好匹配
		rt.pivot = new Vector2(0,1);
		rt.anchorMax = new Vector2(0.5f,0.5f);
		rt.anchorMin = new Vector2(0.5f,0.5f);
		rt.anchoredPosition = Vector3.zero;
		int menuH = InitDic.Count * Menu.MenuItemH;
		rt.sizeDelta = new Vector2 (Menu.MenuItemW, menuH);

		foreach(KeyValuePair<string,Listener> p in InitDic)
		{
			menu.AddMenuItem(p.Key,p.Value);
		}

		menu.Hide();
		return menu;
	}

	/// <summary>
	/// 刷新菜单属性及回调，比如多个文件夹共有一个右键菜单
	/// 这时每次打开菜单前要刷新回调
	/// 把回调刷新成准备打开右键菜单那个文件夹的回调
	/// </summary>
	public void RefreshMenu(Dictionary<string,Listener> InitDic)
	{
		foreach(KeyValuePair<string,Listener> p in InitDic)
		{
			ItemDic [p.Key].onClick.RemoveAllListeners();
			ItemDic [p.Key].onClick.AddListener (delegate(){
				p.Value();
				Hide();//点击后要关闭菜单
			});
		}
	}

	void Awake()
	{
		ItemDic = new Dictionary<string,Button> ();
		ParentTrm = GameObject.Find(MenuRootName).transform;
	}
		
	/// <summary>
	/// 相对于父RectTrm的位置
	/// </summary>
	/// <param name="pos">Position.</param>
	public void Open(Vector2 pos)
	{
		//新menu打开，关闭前一个menu
		if (null != Menu.CurMenu)Menu.CurMenu.Hide();

		gameObject.SetActive(true);
		var rt = gameObject.GetComponent<RectTransform>();
		rt.anchoredPosition = pos;

		//记录当前打开的menu
		Menu.CurMenu = this;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	/// <summary>
	/// 删除
	/// </summary>
	public void OnClear()
	{
		Destroy(gameObject);
	}

	private void AddMenuItem(string name,Listener callback)
	{
		GameObject go = GameObject.Instantiate (Resources.Load("Prefabs/MenuItem")) as GameObject;
		go.name = name;
		go.transform.SetParent(transform);
		var btn = go.GetComponent<Button>();
		var btnText = btn.transform.GetChild (0).GetComponent<Text> ();
		btnText.text = name;
		btnText.fontSize = UIEnum.FontSize;
		if (null != callback) {
			btn.onClick.AddListener (
				delegate(){
					callback();
					Hide();//点击后要关闭菜单
				}
			);
		}

		//统一设置位置
		var rt = go.GetComponent<RectTransform> ();
		rt.pivot = new Vector2(0,1);
		rt.anchorMax = new Vector2(0,1);
		rt.anchorMin = new Vector2(0,1);
		int _h = ItemDic.Count * -MenuItemH;
		rt.anchoredPosition = new Vector2(0,_h);
		rt.sizeDelta = new Vector2(MenuItemW, MenuItemH);
		ItemDic.Add(name,btn);

	}

	/// <summary>
	/// 输入点击事件，返回点击事件在指定ui容器(这里是MenuRoot的RectTrm  就是代码里的ParentTrm)内的相对位置
	/// </summary>
	/// <returns>The mouse position in parent rect transform.</returns>
	/// <param name="eventData">Event data.</param>
	public Vector2 GetMousePosInParentRectTransform(PointerEventData eventData)
	{
		Vector2 pos = new Vector2 (0,0);
		var rt = ParentTrm.GetComponent<RectTransform> ();
		RectTransformUtility.ScreenPointToLocalPointInRectangle (rt, eventData.position, eventData.pressEventCamera, out pos);  
		return pos;
	}
		

	public Vector2 GetMousePosInParentRectTransform(Vector2 mousePos)
	{
		Vector2 pos = new Vector2 (0,0);
		var rt = ParentTrm.GetComponent<RectTransform> ();
		//注意这里要注意下是否会可能出现多个相机
		RectTransformUtility.ScreenPointToLocalPointInRectangle (rt, mousePos, GEditorRoot.GetIns()._Canvas.worldCamera, out pos);  
		return pos;
	}
}
