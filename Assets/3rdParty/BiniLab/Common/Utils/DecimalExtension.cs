

using System;
using System.Globalization;
using UnityEngine;

public static class DecimalExtension
{
	private const string Zero = "0";
	static readonly string[] UnitsKR = new string[] { "", "만", "억", "조", "경", "해", "자", "양", "구", "간", "정", "재", "극", "항", "아", "나", "불", "무", "홍", "몽" };
	static readonly string[] Units = new string[] { "", "K", "M", "G", "T", "P", "F", "E", "Z", "Y", "KK", "MM", "GG", "TT", "PP", "FF", "EE", "ZZ", "YY", "KKK" };

	private static string ToCurrentStringExceptKr(this decimal value)
	{
		if (value <= 0) return Zero;

		string showString = string.Empty;
		string unit = string.Empty;
		string significant = (value < 0) ? "-" : string.Empty;

		string[] parts = value.ToString("E").Split('+');
		if (parts.Length < 2)
		{
			Debug.LogError($"[X] - ToCurrencyString({value})");
			return Zero;
		}

		// 지수
		if (!int.TryParse(parts[1], out int exp))
		{
			Debug.LogError($"[X] - ToCurrencyString({value}) : parts[1] = {parts[1]}");
			return Zero;
		}

		int quot = exp / 3;      // 몫
		int remainder = exp % 3; // 나머지

		if (exp < 3)
		{
			showString = System.Math.Truncate(value).ToString();
		}
		else
		{
			double temp = double.Parse(parts[0].Replace("E", "")) * System.Math.Pow(10, remainder);
			showString = $"{temp:.##}";//temp.ToString("F3").Replace(".000", ""));
		}

		unit = Units[quot];

		return $"{significant}{showString}{unit}";
	}


	public static string ToCurrencyString(this decimal value, int blankSize = 3)
	{
		if (LanguageManager.Instance.Language != Language.KR)
		{
			return value.ToCurrentStringExceptKr();
		}

		if (value <= 0) return Zero;

		string showString = string.Empty;
		string showDecimalString = string.Empty;
		decimal decima = 0;
		string unit = string.Empty;
		string prevUnit = string.Empty;
		string significant = (value < 0) ? "-" : string.Empty;

		// 1052.0329112756 ("E") -> 1.052033E+003
		string[] parts = value.ToString("E").Split('+');

		if (parts.Length < 2)
		{
			Debug.LogError($"[X] - ToCurrencyString({value})");
			return Zero;
		}

		// 지수
		if (!int.TryParse(parts[1], out int exp))
		{
			Debug.LogError($"[X] - ToCurrencyString({value}) : parts[1] = {parts[1]}");
			return Zero;
		}

		int quot = exp / 4;      // 몫
		int remainder = exp % 4; // 나머지

		if (exp < 4)
		{
			showString = $"{Math.Truncate(value)}";
		}
		else
		{
			decimal temp = decimal.Parse(parts[0].Replace("E", "")) * (decimal)System.Math.Pow(10, remainder);
			decimal leftValue = Math.Truncate(temp);

			showString = $"{leftValue}";

			decima = (temp - leftValue) * 10000;
		}

		unit = UnitsKR[quot];

		if (decima > 0)
		{
			showDecimalString = $"{Math.Truncate(decima)}";

			if (quot > 1)
			{
				prevUnit = UnitsKR[quot - 1];
			}
		}

		if (blankSize > 0)
		{
			return $"{significant}{showString}{unit}<size={blankSize}> </size>{showDecimalString}{prevUnit}";
		}
		else
		{
			return $"{significant}{showString}{unit} {showDecimalString}{prevUnit}";
		}
	}

	public static string ToPriceString(this decimal value)
	{
		var currentCulture = CultureInfo.CurrentCulture;
		return string.Format(currentCulture, "{0:##,##0}", value);
	}

	public static string ToN0String(this decimal value)
	{
		return value.ToString("n0");
	}

	public static string ToDamageString(this decimal value)
	{
		if (value < 10000)
			return value.ToString();
		return value.ToCurrencyString(0);
	}
}