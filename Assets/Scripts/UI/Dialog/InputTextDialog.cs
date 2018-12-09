using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
/// <summary>
/// 文字输入框
/// 需要时创建，用完就销毁，不需要保留
/// </summary>
public class InputTextDialog : DragableDialog  {
	
	[SerializeField]private InputField _InputField;
	[SerializeField]private Button OKBtn;
	[SerializeField]private Button UndoBtn;
	[SerializeField]private Text Info;
	[SerializeField]private Text Title;
	[SerializeField]private Text Warning;

	private Listener<byte[]> Callback;
	[HideInInspector]public string FilterRegularStr;
	[HideInInspector]public string WarningStr;
	protected void Awake()
	{
		OKBtn.onClick.AddListener(OkClick);
		UndoBtn.onClick.AddListener(UndoClick);
	}

	/// <summary>
	/// Open the specified callback, regular and info.
	/// </summary>
	/// <param name="callback">Callback.</param>
	/// <param name="regular">正则过滤字符串</param>
	/// <param name="info">如果输入不符合过滤，需要提示的信息</param>
	public static void Open(Listener<byte[]> callback,string title="title",string info="info",string regular = null,string warning = "")
	{
		var go = GameObject.Instantiate (Resources.Load("Prefabs/InputTextDialog"))as GameObject;
		go.transform.SetParent(GEditorRoot.GetIns().DialogPanel);
		var rt = go.GetComponent<RectTransform> ();
		rt.anchoredPosition = Vector2.zero;

		InputTextDialog dialog = go.GetComponent<InputTextDialog>();
		dialog.FilterRegularStr = regular;
		dialog.WarningStr = info;
		dialog.Info.text = info;
		dialog.Title.text = title;

		dialog.Callback = callback;
	}

	public void Close()
	{
		Destroy(gameObject);
	}

	private void OkClick()
	{

		string s = _InputField.text.Trim();

		if (s.Equals (null) || s.Equals (""))
		{
			Warning.text = "字符串为空！";
			return;
		}

		if (null != FilterRegularStr){
			bool b = StringFilter.StrMatchRegex(s,FilterRegularStr);

			if (!b) 
			{
				//不符合过滤规则，输出错误提示
				Warning.text = WarningStr;
				return;
			}
		}


		if (null != Callback)
		{
			IoBuffer ib = new IoBuffer();
			if (!s.Equals (null)) {
				ib.PutString(_InputField.text);
				Callback (ib.ToArray());
			}

		}
			
		Close();
	}

	private void UndoClick()
	{
		Close();
	}
}
