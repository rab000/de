

/// <summary>
/// 这个类的作用就是扩展kv相关数据模板
/// 原来模板只包括2个数据 key value
/// 以后也可能会增加多个数据，所以用KVData来做这个扩展
/// 
/// KVData用来存储KVItem的具体数据，但是只存储一项而不是一列，举例
/// sql中一列数据为 id值 name值 content值
/// 那么一个KVData中存储的数据可以使   key = "ID" value=id值
/// 如果要存一列数据就用Dictionary<string,KVData>
/// 
/// </summary>
public class KVData{

	public string key;

	public string value;

    /// <summary>
    /// 说明，这个type不再代表数据打开dlg的方式
    /// 而是代表数据本身的修改类型
    /// "custom" 手动修改
    /// "default_xxx" 从写死的固定数据中选择
    /// "editorRef_xxx" 引用其他编辑器数据
    /// "artRes_xxx" 从资源目录下所有文件路径中选择一个路径
    /// </summary>
	public string type;

}
