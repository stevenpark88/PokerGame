using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinnerReport
{
	public WinnerReport(string winnerInfo)
	{
		JSON_Object obj = new JSON_Object (winnerInfo);
		Winner = JsonUtility.FromJson<WinnerReport_Winner> (obj.getString("Winner"));
		Loser = JsonUtility.FromJson<WinnerReport_Loser> (obj.getString("Loser"));
	}

	public WinnerReport_Winner Winner;
	public WinnerReport_Loser Loser;
}

public class WinnerReport_Winner
{
	public double winner_balance;
	public double StraightAmount;
	public List<string> Winner_Best_Cards;
	public double BetAmount;
	public double WAAmount;
	public string Winner_Name;
	public double Total_Table_Amount;
	public double BliendAmount;
	public double Winning_Amount;
	public double Round;
	public int Winner_Rank;
	public double Rake_Amount;
	public float Rake_Percentage;
	public bool IsTie;
}

public class WinnerReport_Loser
{
	public List<string> Loser_Best_Cards;
	public int Loser_Rank;
	public string Loser_Name;
	public double StraightAmount;
	public double BliendAmount;
}