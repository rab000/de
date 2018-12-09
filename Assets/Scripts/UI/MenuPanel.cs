using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class MenuPanel : MonoBehaviour {

	[SerializeField]private Button FileBtn;
	[SerializeField]private Button ViewBtn;

	private Menu ViewMenu;
	private Menu FileMenu;

	void Start () 
	{
		CreateFileMenu();
		CreateViewMenu();
		ViewBtn.onClick.AddListener(OnViewClick);
		FileBtn.onClick.AddListener(OnFileClick);

	}

	private void OnViewClick()
	{
		var rt = ViewBtn.GetComponent<RectTransform>();
		Vector2 pos = ViewMenu.GetMousePosInParentRectTransform(Input.mousePosition);
		ViewMenu.Open(pos);
	}

	private void OnFileClick()
	{
		var rt = FileBtn.GetComponent<RectTransform>();
		Vector2 pos = FileMenu.GetMousePosInParentRectTransform(Input.mousePosition);
		FileMenu.Open(pos);
	}

	#region view菜单

	public void CreateViewMenu(){
		if (null != ViewMenu)return;
		Dictionary<string,Listener> dic = new Dictionary<string, Listener> ();
		dic.Add ("skill", OnSkillClick);
		dic.Add ("item", OnItemClick);
		dic.Add ("task", OnItemClick);
		ViewMenu = Menu.Generate(dic,"ViewMenu");
	}

	public void CreateFileMenu(){
		if (null != FileMenu)return;
		Dictionary<string,Listener> dic = new Dictionary<string, Listener> ();
		dic.Add ("GenerateAllGameDate", OnGenerateAllClick);
		FileMenu = Menu.Generate(dic,"FileMenu");
	}

	private void OnSkillClick()
	{
		Debug.Log ("skillMenu Click");
	}

	private void OnItemClick()
	{
		Debug.Log ("itemMenu Click");
	}


	private void OnGenerateAllClick(){
		Debug.Log ("generateAll date Click");
		GEditorDataMgr.GenerateAllGameData();
	}
	#endregion

}
