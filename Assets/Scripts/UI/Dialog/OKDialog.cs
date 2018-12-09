using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 确认窗口
/// </summary>
public class OKDialog : DragableDialog {
	
	[SerializeField]private Button OKBtn;
	[SerializeField]private Button UndoBtn;
	[SerializeField]private Text ContentText;

	private Listener Callback;

	protected void Awake()
	{
		OKBtn.onClick.AddListener(OkClick);
		UndoBtn.onClick.AddListener(UndoClick);
	}

	public static void Open(Listener callback,string content)
	{
		var go = GameObject.Instantiate (Resources.Load("Prefabs/OKDialog"))as GameObject;
		go.transform.SetParent(GEditorRoot.GetIns().DialogPanel);
		var rt = go.GetComponent<RectTransform> ();
		rt.anchoredPosition = Vector2.zero;

		OKDialog dialog = go.GetComponent<OKDialog>();
		dialog.Callback = callback;
		dialog.ContentText.text = content;
	}

	public void Close()
	{
		Destroy(gameObject);
	}

	private void OkClick()
	{
		if (null != Callback)
		{
			Callback();
		}
		Close();
	}

	private void UndoClick()
	{
		Close();
	}
}
