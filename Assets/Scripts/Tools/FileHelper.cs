using UnityEngine;
using System.Collections;
using System.IO;

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

}
