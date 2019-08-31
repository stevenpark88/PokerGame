using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WA_Game_Winner
{
	public List<string> Table_Pot;
	public List<string> WA_Pot;
}

public class WA_GW_Table_Pot
{
	public double Total_Table_Amount;
	public double Winning_Amount;
	public string Winner_Name;
	public double winner_balance;
	public int Round;
	public List<string> Winner_Best_Cards;
	public int Winner_Rank;
	public double Rake_Amount;
	public double AffiliateCommission;
}

public class WA_GW_WA_Pot
{
	public double Total_Table_Amount;
	public double Winning_Amount;
	public string Winner_Name;
	public double winner_balance;
	public int Round;
	public List<string> Winner_Best_Cards;
	public int Winner_Rank;
	public double Rake_Amount;
	public double AffiliateCommission;
}