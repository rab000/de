using System.Text.RegularExpressions;

public class StringFilter{

	/// <summary>
	/// 字符串匹配正则规则返回true
	/// </summary>
	/// <returns><c>true</c>, if match regex was strung, <c>false</c> otherwise.</returns>
	/// <param name="content">Content.</param>
	/// <param name="regularStr">Regular string.</param>
	public static bool StrMatchRegex(string content,string regularStr)
	{
		Regex rg = new Regex(regularStr);
		Match match = rg.Match (content);

		if (match.Success)
			return true;

		return false;
	}
}
