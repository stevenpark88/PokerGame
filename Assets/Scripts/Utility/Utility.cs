using UnityEngine;
using System.Collections;
using System;

public class Utility
{
	public static string GetAmount (double num)
	{
		if (num < 0)
			num = 0;

		num = Math.Round (num, 2);
		if (UIManager.Instance.isRealMoney)
			return Utility.GetCurrencyPrefix () + num.ToString ("#,##0.00");

		if (num >= 1000) {
			if (num >= 100000000) {
				return Utility.GetCurrencyPrefix () + (num / 1000000D).ToString ("0.#M");
			} else if (num >= 1000000) {
				return Utility.GetCurrencyPrefix () + (num / 1000000D).ToString ("0.##M");
			} else if (num >= 100000) {
				return Utility.GetCurrencyPrefix () + (num / 1000D).ToString ("0.#K");
			} else if (num >= 10000) {
				return Utility.GetCurrencyPrefix () + (num / 1000D).ToString ("0.##K");
			} else {
				return Utility.GetCurrencyPrefix () + (num / 1000D).ToString ("0.#K");
			}
		} else
			return Utility.GetCurrencyPrefix () + num.ToString ();
	}

	public static string GetCommaSeperatedAmount (double amount, bool isRealMoney = false)
	{
		if (amount < 0)
			amount = 0;

		if (UIManager.Instance.isRealMoney || isRealMoney)
			return Utility.GetCurrencyPrefix (isRealMoney) + Math.Round (amount, 2).ToString ("#,##0.00");

		return Utility.GetCurrencyPrefix (isRealMoney) + amount.ToString ("#,##0");
	}

	public static string GetCommaSeperatedPlayMoneyAmount (double amount)
	{
		if (amount < 0)
			amount = 0;

		return amount.ToString ("#,##0");
	}

	public static string GetCurrencyPrefix (bool isRealMoney = false)
	{
		if (UIManager.Instance.isRealMoney || isRealMoney) {
            return "€ ";
            //return "$ ";
		} else
			return "";
	}

	public static string GetRealMoneyAmount (double amount)
	{
		if (amount < 0)
			amount = 0;

		return Utility.GetCurrencyPrefix (true) + amount.ToString ("#,##0.00");
	}
}

public static class MyExtension
{
	public static string ToCamelCase (this string str)
	{
		string[] words = str.Split (' ');
		string newString = "";

		foreach (string s in words) {
			newString += s.ToCharArray () [0].ToString ().ToUpper () + s.Substring (1) + " ";
		}

		return newString;
	}

	/// <summary>
	/// Rounds to ten.
	/// </summary>
	/// <returns>The to ten.</returns>
	/// <param name="value">Value.</param>
	public static float RoundToTen (this float value)
	{
		return (float)Math.Round ((double)value / 10) * 10;
	}

	/// <summary>
	/// Rounds the value to 2 digit floating point.
	/// </summary>
	/// <returns>2 digit floating point value.</returns>
	/// <param name="value">Value.</param>
	public static float RoundTo2DigitFloatingPoint (this float value)
	{
		return (float)Math.Round (value, 2);
	}

	/// <summary>
	/// Rounds the value to 2 digit floating point.
	/// </summary>
	/// <returns>2 digit floating point value.</returns>
	/// <param name="value">Value.</param>
	public static double RoundTo2DigitFloatingPoint (this double value)
	{
		return (double)Math.Round (value, 2);
	}
}