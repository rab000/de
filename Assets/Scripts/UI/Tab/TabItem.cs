using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TabItem : SelectableItem {

	public TabData _TabData;
	public Text _Text;
	public Image BG;
	private TabContainer tabContainer;
	private bool _BeSelect = false;
	public bool BeSelecte{
		set
		{ 
			_BeSelect = value;

			//Debug.Log("FUCK---->NAME:"+gameObject.name+" sel:"+_BeSelect);
			if (_BeSelect)
			{
				tabContainer.CurTabItem = this;
				BG.color = Color.white;
			} else {
				BG.color = Color.gray;
			}
		}
		get
		{ 
			return _BeSelect;
		}
	}

	public static TabItem Create(TabData tabData,RectTransform parentRectTrm,TabContainer tabContainer)
	{

		GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/TabItem"));
		go.name = tabData.Name;

		go.transform.SetParent(parentRectTrm as Transform);

		var rt = go.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(UIEnum.TabW,rt.sizeDelta.y);

		//做一些偏移
		rt.offsetMax = new Vector2(rt.offsetMax.x,0);
		rt.offsetMin = new Vector2(rt.offsetMin.x,0);

		TabItem item = go.GetComponent<TabItem>();
		item.tabContainer = tabContainer;
		item._TabData = tabData;
		item._Text.text = tabData.Name;
		return item;
	}

	public override void OnSelect()
	{
		BeSelecte = !BeSelecte;
	}

	protected override void OnMouseLeftSingleClick(PointerEventData eventData)
	{
		OnSelect();
	}
}
