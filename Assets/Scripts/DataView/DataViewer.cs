using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
/// <summary>
/// 数据查看器
/// 把二进制数据
/// </summary>
public class DataViewer : MonoBehaviour
{
    bool BeTest = false;
    /// <summary>
    /// 输入二进制文件路径
    /// </summary>
    /// 
    static string InputURL
    {
        get
        {
            return @"C:\root\work\gitspace\de\data\game\skill.gdata";
        }
    }

    /// <summary>
    /// 输出铭文.txt文件路径
    /// </summary>
    string OutputURL = @"d:\test.txt";

    void Start()
    {

        if (FileHelper.BeFileExists(OutputURL))
        {
            File.Delete(OutputURL);
        }

        byte[] bs = FileHelper.Get(InputURL);

        Log.i("bs.len:" + bs.Length);

        IoBuffer buffer = new IoBuffer(1000000);

        buffer.PutBytes(bs);

        int dataNum = buffer.GetInt();

        FileHelper.WriteMessage(OutputURL, "数据条数:" + dataNum);

        do
        {
            if (BeTest)
            {
                string key = buffer.GetString();//key

                //这句代表一个treeItem的开头
                if (key.Equals("ID"))
                {
                    FileHelper.WriteMessage(OutputURL, "----------------分割线---------------");
                }
                string value = buffer.GetString();//value
                string type = buffer.GetString();//type
                string s = "[" + key + "][" + value + "][" + type + "]";
                FileHelper.WriteMessage(OutputURL, s);
            }
            else
            {
                string value1 = buffer.GetString();
                FileHelper.WriteMessage(OutputURL, value1);
            }


        }
        while (buffer.HasData());

        Log.i("数据查看器生成数据完毕");

    }


}
