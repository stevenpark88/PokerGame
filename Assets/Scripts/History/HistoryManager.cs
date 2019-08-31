using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HistoryManager : MonoBehaviour
{
	private static HistoryManager Instance;

	[Header("Table Game")]
	#region TABLE_GAME
	public List<History> anteAndBlindTableRoundHistory;
	public List<History> firstTableRoundHistory;
	public List<History> secondTableRoundHistory;
	public List<History> thirdTableRoundHistory;
	public List<History> whoopAssCardTableRoundHistory;
	public List<History> playTableRoundHistory;
	#endregion

	[Header("Texass Game")]
	#region TEXASS_GAME
	public List<TexassGameHistory> preflopTexassRoundHistory;
	public List<TexassGameHistory> flopTexassRoundHistory;
	public List<TexassGameHistory> turnTexassRoundHistory;
	public List<TexassGameHistory> riverTexassRoundHistory;
	#endregion

	[Header("WhoopAss Game")]
	#region WHOOPASS_GAME
	public List<WhoopAssGameHistory> startWhoopAssRoundHistory;
	public List<WhoopAssGameHistory> firstWhoopAssRoundHistory;
	public List<WhoopAssGameHistory> secondWhoopAssRoundHistory;
	public List<WhoopAssGameHistory> whoopAssCardWhoopAssRoundHistory;
	public List<WhoopAssGameHistory> thirdWhoopAssRoundHistory;
	#endregion

	void Awake ()
	{
		anteAndBlindTableRoundHistory = new List<History> ();
		firstTableRoundHistory = new List<History> ();
		secondTableRoundHistory = new List<History> ();
		thirdTableRoundHistory = new List<History> ();
		whoopAssCardTableRoundHistory = new List<History> ();
		playTableRoundHistory = new List<History> ();

		preflopTexassRoundHistory = new List<TexassGameHistory> ();
		flopTexassRoundHistory = new List<TexassGameHistory> ();
		turnTexassRoundHistory = new List<TexassGameHistory> ();
		riverTexassRoundHistory = new List<TexassGameHistory> ();

		startWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
		firstWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
		secondWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
		whoopAssCardWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
		thirdWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
	}

	void OnEnable ()
	{
		GameManager.resetData += HandleOnResetData;
		TexassGame.resetData += HandleOnResetData;
		WhoopAssGame.resetData += HandleOnResetData;

		NetworkManager.onRoundComplete += HandleOnRoundComplete;
	}

	void OnDisable ()
	{
		GameManager.resetData -= HandleOnResetData;
		TexassGame.resetData -= HandleOnResetData;
		WhoopAssGame.resetData -= HandleOnResetData;

		NetworkManager.onRoundComplete -= HandleOnRoundComplete;
	}

	public static HistoryManager GetInstance ()
	{
		if (Instance == null) {
			Instance = new GameObject ("HistoryManager").AddComponent<HistoryManager> ();
		}

		return Instance;
	}

	/// <summary>
    /// Adds the Table Game History
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="playerName"></param>
    /// <param name="gameRound"></param>
    /// <param name="betAmount"></param>
    /// <param name="totalBetAmount"></param>
    /// <param name="playerAction"></param>
    /// <param name="isBetOnStraight"></param>
	public void AddHistory (string playerID, string playerName, TABLE_GAME_ROUND gameRound, double betAmount, double totalBetAmount, PLAYER_ACTION playerAction, bool isBetOnStraight)
	{
		History history = new History ();
		history.playerID = playerID;
		history.playerName = playerName;
		history.gameRound = gameRound;
		history.betAmount = betAmount;
		history.totalBetAmount = totalBetAmount;
		history.playerAction = playerAction;

		switch (gameRound) {
		case TABLE_GAME_ROUND.START:
			anteAndBlindTableRoundHistory.Add (history);
			break;
		case TABLE_GAME_ROUND.FIRST_BET:
			firstTableRoundHistory.Add (history);
			break;
		case TABLE_GAME_ROUND.SECOND_BET:
			secondTableRoundHistory.Add (history);
			break;
		case TABLE_GAME_ROUND.THIRD_BET:
			thirdTableRoundHistory.Add (history);
			break;
		case TABLE_GAME_ROUND.WHOOPASS:
			whoopAssCardTableRoundHistory.Add (history);
			break;
		case TABLE_GAME_ROUND.PLAY:
			whoopAssCardTableRoundHistory.Add (history);
			break;
		}

		if (betAmount > 0)
			GameManager.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + playerID + "</color> : " + GetPlayerAction (playerAction) + "->" + Utility.GetAmount (betAmount);
		else
			GameManager.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + playerID + "</color> : " + GetPlayerAction (playerAction);
        Canvas.ForceUpdateCanvases();
//		GameManager.Instance.scrollNote.verticalScrollbar.value = 0;
//		Debug.LogWarning (playerID + " has " + playerAction + " in " + gameRound + " round.\t\t--> bet amount  : " + betAmount + "\t\t--> total bet amount  : " + totalBetAmount);
	}


	/// <summary>
    /// Adds the Texass Game History
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="playerName"></param>
    /// <param name="gameRound"></param>
    /// <param name="betAmount"></param>
    /// <param name="totalBetAmount"></param>
    /// <param name="playerAction"></param>
	public void AddHistory (string playerID, string playerName, TEXASS_GAME_ROUND gameRound, double betAmount, double totalBetAmount, PLAYER_ACTION playerAction)
	{
//		if (playerAction == PLAYER_ACTION.SMALL_BLIND) {
//			if (preflopTexassRoundHistory.Count > 0)
//				return;
//		} else if (playerAction == PLAYER_ACTION.BIG_BLIND) {
//			if (preflopTexassRoundHistory.Count > 1)
//				return;
//		}

		TexassGameHistory history = new TexassGameHistory ();
		history.playerID = playerID;
		history.playerName = playerName;
		history.gameRound = gameRound;
		history.betAmount = betAmount;
		history.totalBetAmount = totalBetAmount;
		history.playerAction = playerAction;

		switch (gameRound) {
		case TEXASS_GAME_ROUND.PREFLOP:
			preflopTexassRoundHistory.Add (history);
			break;
		case TEXASS_GAME_ROUND.FLOP:
			flopTexassRoundHistory.Add (history);
			break;
		case TEXASS_GAME_ROUND.TURN:
			turnTexassRoundHistory.Add (history);
			break;
		case TEXASS_GAME_ROUND.RIVER:
			riverTexassRoundHistory.Add (history);
			break;
		}

		if (betAmount > 0)
			TexassGame.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + playerID + "</color> : " + GetPlayerAction (playerAction) + "->" + Utility.GetAmount (betAmount);
		else {
			if (playerAction != PLAYER_ACTION.SMALL_BLIND && playerAction != PLAYER_ACTION.BIG_BLIND)
				TexassGame.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + playerID + "</color> : " + GetPlayerAction (playerAction);
		}
		Canvas.ForceUpdateCanvases();
		TexassGame.Instance.scrollNote.verticalScrollbar.value = 0;
	}

    /// <summary>
    /// Adds the WhoopAss Game History
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="playerName"></param>
    /// <param name="gameRound"></param>
    /// <param name="betAmount"></param>
    /// <param name="totalBetAmount"></param>
    /// <param name="playerAction"></param>
	public void AddHistory(string playerID, string playerName, WHOOPASS_GAME_ROUND gameRound, double betAmount, double totalBetAmount, PLAYER_ACTION playerAction)
    {
        WhoopAssGameHistory history = new WhoopAssGameHistory();
        history.playerID = playerID;
        history.playerName = playerName;
        history.gameRound = gameRound;
        history.betAmount = betAmount;
        history.totalBetAmount = totalBetAmount;
        history.playerAction = playerAction;

        switch (gameRound)
        {
            case WHOOPASS_GAME_ROUND.START:
                startWhoopAssRoundHistory.Add(history);
                break;
            case WHOOPASS_GAME_ROUND.FIRST_FLOP:
                firstWhoopAssRoundHistory.Add(history);
                break;
            case WHOOPASS_GAME_ROUND.SECOND_FLOP:
                secondWhoopAssRoundHistory.Add(history);
                break;
            case WHOOPASS_GAME_ROUND.WHOOPASS_CARD:
                whoopAssCardWhoopAssRoundHistory.Add(history);
                break;
            case WHOOPASS_GAME_ROUND.THIRD_FLOP:
                thirdWhoopAssRoundHistory.Add(history);
                break;
        }

		if (betAmount > 0)
			WhoopAssGame.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + playerID + "</color> : " + GetPlayerAction (playerAction) + "->" + Utility.GetAmount (betAmount);
		else {
			if (playerAction != PLAYER_ACTION.SMALL_BLIND && playerAction != PLAYER_ACTION.BIG_BLIND)
				WhoopAssGame.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + playerID + "</color> : " + GetPlayerAction (playerAction);
		}
        Canvas.ForceUpdateCanvases();
        WhoopAssGame.Instance.scrollNote.verticalScrollbar.value = 0;
        //		Debug.LogWarning (playerID + " has " + playerAction + " in " + gameRound + " round.\t\t--> bet amount  : " + betAmount + "\t\t--> total bet amount  : " + totalBetAmount);
    }

    /// <summary>
    /// Gets the history.
    /// </summary>
    /// <returns>The history.</returns>
    /// <param name="round">Round.</param>
    public List<History> GetTableGameHistory (TABLE_GAME_ROUND round)
	{
		switch (round) {
		case TABLE_GAME_ROUND.START:
			return anteAndBlindTableRoundHistory;
		case TABLE_GAME_ROUND.FIRST_BET:
			return firstTableRoundHistory;
		case TABLE_GAME_ROUND.SECOND_BET:
			return secondTableRoundHistory;
		case TABLE_GAME_ROUND.THIRD_BET:
			return thirdTableRoundHistory;
		case TABLE_GAME_ROUND.WHOOPASS:
			return whoopAssCardTableRoundHistory;
		case TABLE_GAME_ROUND.PLAY:
			return playTableRoundHistory;
		}

		return null;
	}

	public List<TexassGameHistory> GetTexassGameHistory (TEXASS_GAME_ROUND round)
	{
		switch (round) {
		case TEXASS_GAME_ROUND.PREFLOP:
			return preflopTexassRoundHistory;
		case TEXASS_GAME_ROUND.FLOP:
			return flopTexassRoundHistory;
		case TEXASS_GAME_ROUND.TURN:
			return turnTexassRoundHistory;
		case TEXASS_GAME_ROUND.RIVER:
			return riverTexassRoundHistory;
		}

		return null;
	}

	public List<WhoopAssGameHistory> GetWhoopAssGameHistory(WHOOPASS_GAME_ROUND round)
	{
		switch (round) {
		case WHOOPASS_GAME_ROUND.START:
			return startWhoopAssRoundHistory;
		case WHOOPASS_GAME_ROUND.FIRST_FLOP:
			return firstWhoopAssRoundHistory;
		case WHOOPASS_GAME_ROUND.SECOND_FLOP:
			return secondWhoopAssRoundHistory;
		case WHOOPASS_GAME_ROUND.WHOOPASS_CARD:
			return whoopAssCardWhoopAssRoundHistory;
		case WHOOPASS_GAME_ROUND.THIRD_FLOP:
			return thirdWhoopAssRoundHistory;
		}

		return null;
	}

	private string GetPlayerAction(PLAYER_ACTION action)
	{
		switch (action) {
        case PLAYER_ACTION.REBUY:
            return "Rebuy";
		case PLAYER_ACTION.CHECK:
			return "Check";
		case PLAYER_ACTION.BET:
			return "Bet";
		case PLAYER_ACTION.ALLIN:
			return "All in";
		case PLAYER_ACTION.ACTION_WA_DOWN:
			return "WhoopAss Card Down";
		case PLAYER_ACTION.ACTION_WA_UP:
			return "WhoopAss Card Up";
		case PLAYER_ACTION.ACTION_WA_NO:
			return "No WhoopAss Card";
		case PLAYER_ACTION.SMALL_BLIND:
			return "Small Blind";
		case PLAYER_ACTION.BIG_BLIND:
			return "Big Blind";
		case PLAYER_ACTION.RAISE:
			return "Raise";
		case PLAYER_ACTION.CALL:
			return "Call";
		case PLAYER_ACTION.FOLD:
			return "Fold";
		case PLAYER_ACTION.TIMEOUT:
			return "Timeout";
		}

		return "";
	}

	#region DELEGATE_CALLBACKS

	private void HandleOnResetData ()
	{
		anteAndBlindTableRoundHistory = new List<History> ();
		firstTableRoundHistory = new List<History> ();
		secondTableRoundHistory = new List<History> ();
		thirdTableRoundHistory = new List<History> ();
		whoopAssCardTableRoundHistory = new List<History> ();
		playTableRoundHistory = new List<History> ();

		preflopTexassRoundHistory = new List<TexassGameHistory> ();
		flopTexassRoundHistory = new List<TexassGameHistory> ();
		turnTexassRoundHistory = new List<TexassGameHistory> ();
		riverTexassRoundHistory = new List<TexassGameHistory> ();

		startWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
		firstWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
		secondWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
		whoopAssCardWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
		thirdWhoopAssRoundHistory = new List<WhoopAssGameHistory> ();
	}

	private void HandleOnRoundComplete(string sender, string roundInfo)
	{
		GameRound round = JsonUtility.FromJson<GameRound>(roundInfo);

		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE) {
			GameManager.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + GetRoundInString(round.Round) + " Round Complete" + "</color>";
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
			TexassGame.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + GetRoundInString(round.Round) + " Round Complete" + "</color>";
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
			WhoopAssGame.Instance.txtGameLog.text += "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + GetRoundInString(round.Round) + " Round Complete" + "</color>";
		}
	}

	#endregion

	#region PRIVATE_METHODS

	private string GetRoundInString(int round)
	{
		round--;
		switch (round) {
		case (int)TEXASS_GAME_ROUND.FLOP:
			return "Flop";
		case (int)TEXASS_GAME_ROUND.PREFLOP:
			return "Preflop";
		case (int)TEXASS_GAME_ROUND.RIVER:
			return "River";
		case (int)TEXASS_GAME_ROUND.TURN:
			return "Turn";
		case (int)WHOOPASS_GAME_ROUND.FIRST_FLOP:
			return "First Flop";
		case (int)WHOOPASS_GAME_ROUND.SECOND_FLOP:
			return "Second Flop";
		case (int)WHOOPASS_GAME_ROUND.START:
			return "Start";
		case (int)WHOOPASS_GAME_ROUND.THIRD_FLOP:
			return "Third Flop";
		case (int)WHOOPASS_GAME_ROUND.WHOOPASS_CARD:
			return "WhoopAss Card";
		case (int)TABLE_GAME_ROUND.FIRST_BET:
			return "First Bet";
		case (int)TABLE_GAME_ROUND.PLAY:
			return "Play";
		case (int)TABLE_GAME_ROUND.SECOND_BET:
			return "Second Bet";
		case (int)TABLE_GAME_ROUND.START:
			return "Start";
		case (int)TABLE_GAME_ROUND.THIRD_BET:
			return "Third Bet";
		case (int)TABLE_GAME_ROUND.WHOOPASS:
			return "WhoopAss Card";
		}

		return "";
	}

	#endregion
}