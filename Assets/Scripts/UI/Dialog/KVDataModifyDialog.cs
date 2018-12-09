using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// nsql 这个dialog用于KVItem数据修改
/// 包括如下几个选项
/// key	 : 	key内容		对应ui是inputField
/// value: 	value内容    对应ui是inputField
/// type :  这里是value值通过什么方式填充  	对应ui是Dropdown  eg: string就是自己填写value的内容，listA就是从A列表选择value内容，ListB就是从B列表选择value内容
/// content: 用于填充value的具体内容列表  	对应ui是Dropdown  eg: 当type选项选择不同类型的时候，这个content中的dropdown被填充上不同的内容，选择一项具体内容，就会填充到value上
/// content这个参数只在这个dialog中有效，其他3个参数会以string方式发送给打开窗口的调用者
/// </summary>
public class KVDataModifyDialog : DragableDialog {
    bool BeShowLog = true;
	[SerializeField]private InputField KeyInputField;
	[SerializeField]private InputField ValueInputField;
    //[SerializeField]private Dropdown TypeDropdown
    [SerializeField]private Text TypeInfo;

    [SerializeField]private Dropdown ContentDropdown;
	[SerializeField]private Button OKBtn;
	[SerializeField]private Button UndoBtn;
	[SerializeField]private Text Title;
	[SerializeField]private Text Warning;
	private Listener<byte[]> DlgCallback;

    /// <summary>
    /// 用于区分打开DeaultDialog的是哪个编辑器
    /// 这个信息目前用于区分不同编辑器数据模板当中的dropdown类型
    /// 
    /// </summary>
    private string EditorType;

    /// <summary>
    /// 这里临时记录下打开窗口时的dataType，实际这个时荣誉的量
    /// </summary>
    private string DataType;

	/// <summary>
	/// 用来存储内容dropdown的数据
	/// 两个string分别代表显示内容(显示到ContentDropdown)，实际数据(这个数据会赋值给valueInputField，也是最后sql中要存储的数据)
	/// 这里存储的数据目前包括两种
	/// 
	/// 1 contentDropdown中的数据来自本编辑器,此情况 显示内容就是TreeItemName，实际数据就是TreeItemID
	/// 2 contentDropdown中的数据来自美术资源,此情况 显示内容就是美术资源文件名(含后缀)，实际数据就是美术资源的 相对路径
	/// </summary>
	Dictionary<string,string> ContentDropdownDataDic;

	protected void Awake()
	{
		OKBtn.onClick.AddListener(OkClick);
		UndoBtn.onClick.AddListener(UndoClick);
		//TypeDropdown.onValueChanged.AddListener(OnTypeDropdownChange);
		ContentDropdown.onValueChanged.AddListener(OnContentDropdownChange);
	}

    /// <summary>
    /// 打开窗口操作
    /// </summary>
    /// <param name="callback">用于返回窗口操作的结果</param>
    /// <param name="editorType">当前打开dlg的kvitem属于于哪个editor，有了这个信息，就可以去特定editor数据模板中查找一些数据项的dropdown选项</param>
    /// <param name="title">打开窗口的名称</param>
    /// <param name="key">kvitem中 KVData的 key值</param>
    /// <param name="value">kvitem中 KVData的 value值</param>
    /// <param name="type">kvitem中 KVData的 type值</param>
	public static void Open(Listener<byte[]> callback, string editorType,string title="title",string key = "key",string value = "value",string type = "string"){
		var go = GameObject.Instantiate (Resources.Load("Prefabs/DefaultDialog"))as GameObject;
		go.transform.SetParent(GEditorRoot.GetIns().DialogPanel);
		var rt = go.GetComponent<RectTransform> ();
		rt.anchoredPosition = Vector2.zero;

		KVDataModifyDialog dlg = go.GetComponent<KVDataModifyDialog>();
		//dialog.FilterRegularStr = regular;
		//dialog.WarningStr = info;
		//dialog.Info.text = info;
		dlg.Title.text = title;
		dlg.KeyInputField.text = key;
		dlg.ValueInputField.text = value;
        dlg.EditorType = editorType;
        dlg.DataType = type;
        dlg.TypeInfo.text = type;
        dlg.InitContentDropdown(type);

        dlg.DlgCallback = callback;

	}

	private void OkClick()
	{
		//输入内容有效性判定
		string keyStr = KeyInputField.text.Trim();

		if (keyStr.Equals (null) || keyStr.Equals ("")){
			Warning.text = "key字符串为空！";
			return;
		}

		string valueStr = ValueInputField.text.Trim ();
		if (valueStr.Equals (null) || valueStr.Equals ("")){
			Warning.text = "value字符串为空！";
			return;
		}

		//回调
		if (null != DlgCallback)
		{
			IoBuffer ib = new IoBuffer();
			ib.PutString(keyStr);
			ib.PutString(valueStr);
            //Debug.Log("----------->valueStr:" + valueStr);
			ib.PutString(DataType);
			DlgCallback (ib.ToArray());
		}

		Close();
	}

	private void UndoClick(){
		Close();
	}
   
	

    public void InitContentDropdown(string type)
    {
        //清理掉原来存储的数据
        if (null != ContentDropdownDataDic)
        {
            ContentDropdownDataDic.Clear();
            ContentDropdownDataDic = null;
        }

        //如果类型是自定义类型，说明需要手动输入，不需要从列表选择，把列表选择项隐藏即可
        if (type.Equals(GEditorConfig.DROPDOWN_TYPE_custom))
        {
            ContentDropdown.gameObject.SetActive(false);
            return;
        }
        else
        {
            ContentDropdown.gameObject.SetActive(true);
        }


        //重新获取dropdown数据
        ContentDropdownDataDic = GEditorConfig.GetContentDropdownDataList(EditorType, type);

        if (null != ContentDropdownDataDic && ContentDropdownDataDic.Count > 0)
        {

            //先清空
            ContentDropdown.ClearOptions();

            //填充数据
            foreach (KeyValuePair<string, string> p in ContentDropdownDataDic)
            {
                Dropdown.OptionData op = new Dropdown.OptionData();
                op.text = p.Key;
                ContentDropdown.options.Add(op);
            }
        }
        else
        {
            Log.i("KVDataModifyDialog", "InitContentDropdown","初始化contentDropdown时数据为null或数据条数小于0",BeShowLog);
        }
        
    }

	private void OnContentDropdownChange(int dropDownIndex){
		
		//NINFO nsql这里改变value数据，这里注意一点，只需要在CoententDropdown中看到显示数据，value中只需要写最终的引用数据即可

		string _contentShowName = ContentDropdown.options[dropDownIndex].text;

		string _realData = ContentDropdownDataDic [_contentShowName];//真实数据

		ValueInputField.text = _realData;

	}

	public void Close()
	{
		Destroy(gameObject);
	}

}
