using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class FileHelper{

	#region for editor

	/// <summary>
	/// 存bytes到文件
	/// </summary>
	/// <param name="path">文件夹路径</param>
	/// <param name="fileName">文件名</param>
	/// <param name="bs">Bs.</param>
	public static void Save(string path,string fileName,byte[] bs)
	{
		string folderPath = Path.GetDirectoryName(path);
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string filePath = folderPath + "/" +fileName;

		if(File.Exists(filePath))File.Delete(filePath);

		FileStream fs = File.Create(filePath);
		//FileStream fs = new FileStream(filePath,FileMode.Create);//可替换
		fs.Write(bs,0,bs.Length);
		fs.Close();
	}

	/// <summary>
	/// 从文件读bytes
	/// </summary>
	/// <param name="path">文件夹路径</param>
	/// <param name="fileName">文件名</param>
	public static byte[] Get(string path,string fileName)
	{
		string folderPath = Path.GetDirectoryName(path);
		if (!Directory.Exists (folderPath)) 
		{
			Debug.LogError("GEditorDataManager.Get--->读取文件失败，找不到文件夹"+folderPath);
			return null;
		}
		string filePath = folderPath + "/" +fileName;
		return Get(filePath);
	}

	/// <summary>
	/// 从文件读bytes
	/// </summary>
	/// <param name="path">文件夹路径</param>
	/// <param name="fileName">文件名</param>
	public static byte[] Get(string url)
	{
		if (!File.Exists (url))
		{
			Debug.LogError("GEditorDataManager.Get--->读取文件失败，找不到文件"+url);
			return null;
		}
		FileStream fs = new FileStream (url, FileMode.Open);
		byte[] bs = new byte[fs.Length];
		fs.Read(bs,0,bs.Length);
		fs.Close();
		return bs;
	}

	public static bool BeFileExists(string filePath)
	{
		if (File.Exists (filePath))
			return true;
		else
			return false;
	}
    #endregion

    /// <summary>
    /// 输出指定信息到文本文件
    /// </summary>
    /// <param name="path">文本文件路径</param>
    /// <param name="msg">输出信息</param>
    public static void WriteMessage(string path, string msg)
    {
        //using (FileStream fs = new FileStream(@"d:\test.txt", FileMode.OpenOrCreate, FileAccess.Write))
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.BaseStream.Seek(0, SeekOrigin.End);
                //sw.WriteLine("{0}\n", msg, DateTime.Now);
                sw.WriteLine(msg);
                //sw.Write(msg);
                sw.Flush();
            }
        }
    }
		
	/// <summary>
	/// 查找目录下所有文件
	/// 输出目录eg:Assets/NEditor/RoleEditor/Res/Female/Materials\female_top-2_orange.mat
	/// </summary>
	/// <returns>The all files.</returns>
	/// <param name="folderPath">文件夹目录，eg:Assets/NEditor/RoleEditor/Res/Female/Materials</param>
	public static string[] FindAllFileURLs(string folderPath) {

		string[] fileURLs = Directory.GetFiles(folderPath,"*",SearchOption.AllDirectories);//注意这里要排除meta文件

		List<string> filsList = new List<string>();
		for (int i = 0; i < fileURLs.Length; i++) 
		{
			

			//避免找到无用的.meta文件
			if (fileURLs [i].Contains (".meta"))continue;

			string s = fileURLs [i].Replace ("\\","/");

			Debug.Log("查找目录"+folderPath+"下后文件"+i+"->"+fileURLs[i]+" 转换后文件:"+s);

			filsList.Add (s);

		}
		return filsList.ToArray();
	}

    /// <summary>
    /// file copy
    /// </summary>
    /// <param name="_Src_Path"></param>
    /// <param name="_Target_Path"></param>
    /// <param name="_Imperative"> 如果目标文件是否先删除再copy</param>
    public static void CopyFile(string _Src_Path, string _Target_Path, bool _Imperative = false)
    {
        if (_Imperative)
        {
            if (File.Exists(_Target_Path))
            {
                File.Delete(_Target_Path);
            }
        }
        if (File.Exists(_Src_Path))
            File.Copy(_Src_Path, _Target_Path);
        else
        {
            Debug.LogError("拷贝文件出错");
        }
    }

}
