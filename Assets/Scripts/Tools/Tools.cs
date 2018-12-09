using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Tools :MonoBehaviour{

	public Font _font;

	private static Tools Ins;
	void Awake()
	{
		Ins = this;
	}

	void OnDestroy()
	{
		Ins = null;
	}

	public static Tools GetIns()
	{
		return Ins;
	}
	/// <summary>
	/// 获取ugui字符串宽度
	/// </summary>
	/// <returns>The string w.</returns>
	/// <param name="s">S.</param>
	/// <param name="fontsize">Fontsize.</param>
	/// <param name="fontstyle">Fontstyle.</param>
	public static float GetStringW(string s,int fontsize,FontStyle fontstyle=FontStyle.Normal)
	{
		Font font1 = Tools.GetIns ()._font;
		font1.RequestCharactersInTexture (s,fontsize,fontstyle);
		CharacterInfo characterInfo;
		float w = 0f;
		for (int i = 0; i < s.Length; i++) {
			font1.GetCharacterInfo (s[i],out characterInfo,fontsize);
			w += characterInfo.advance;
		}

		return w;
	}


}
