using UnityEngine;
using System.Collections;

public class History
{
	public string playerID;
	public string playerName;

	public TABLE_GAME_ROUND gameRound;

	public double betAmount;
	public double totalBetAmount;
	public PLAYER_ACTION playerAction;
	public bool isBetOnStraight;
}

public class TexassGameHistory
{
	public string playerID;
	public string playerName;

	public TEXASS_GAME_ROUND gameRound;

	public double betAmount;
	public double totalBetAmount;
	public PLAYER_ACTION playerAction;
}

public class WhoopAssGameHistory
{
	public string playerID;
	public string playerName;

	public WHOOPASS_GAME_ROUND gameRound;

	public double betAmount;
	public double totalBetAmount;
	public PLAYER_ACTION playerAction;
}