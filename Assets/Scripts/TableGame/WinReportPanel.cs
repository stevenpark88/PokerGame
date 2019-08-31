using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WinReportPanel : MonoBehaviour
{
	public Text txtTitle;

	public Text txtWhoopAssCard;
	public Text txtTotalBet;
	public Text txtStraightOrBetter;
	public Text txtBlindBet;
	public Text txtRakeAmount;
	public Text txtPayoutAmount;

	public Text txtDealerWon;

	public GameObject dealerWinParent;
	public GameObject tieParent;

	public GameObject winningCardsObject;

	public GameObject dealerHandRankParent;
	public List<Image> winningCardList;
	public List<Image> dealerWinningCardList;
	public Text txtDealerHandRank;
	public Text txtTieTitle;
	public Text txtTieDescription;

	public Image imgWABullet;
	public Image imgTotalBetBullet;
	public Image imgStraightBetBullet;
	public Image imgBlindBetBullet;
	public Image imgRakeAmountBullet;

	// Use this for initialization
	void Start ()
	{
		
	}

	public void DisplayWinReport (WinnerReport winnerReport)
	{
		GameManager.Instance.txtGameLog.text += "\n" + winnerReport.Winner.Winner_Name + " -> " + GetWinningRank (winnerReport.Winner.Winner_Rank) + " -> " + Utility.GetAmount (winnerReport.Winner.Winning_Amount);
		if (winnerReport.Winner.Winner_Name.Equals (Constants.FIELD_PLAYER_NAME_DEALER)) {
			txtDealerWon.text = "Dealer Won!\n\nDealer's Hand: <color=white>" + GetWinningRank (winnerReport.Winner.Winner_Rank) + "</color>" +
			"\nPlayer's Hand: <color=white>" + GetWinningRank (winnerReport.Loser.Loser_Rank) + "</color>";
			if (winnerReport.Loser.StraightAmount > 0)
				txtDealerWon.text += "\nStraight Bet: <color=white>" + winnerReport.Loser.StraightAmount + "</color>";
			if (winnerReport.Loser.BliendAmount > 0)
				txtDealerWon.text += "\nBlind Bet: <color=white>" + winnerReport.Loser.BliendAmount + "</color>";

			if (GameManager.Instance.ownTablePlayer != null) {
				GameManager.Instance.ownTablePlayer.buyinChips += (long)(winnerReport.Loser.StraightAmount + winnerReport.Loser.BliendAmount);
				if (UIManager.Instance.isRealMoney)
					GameManager.Instance.ownTablePlayer.totalRealMoney += (long)(winnerReport.Loser.StraightAmount + winnerReport.Loser.BliendAmount);
				else
					GameManager.Instance.ownTablePlayer.totalChips += (long)(winnerReport.Loser.StraightAmount + winnerReport.Loser.BliendAmount);
				GameManager.Instance.ownTablePlayer.DisplayTotalChips ();

				if (winnerReport.Loser.StraightAmount + winnerReport.Loser.BliendAmount > Constants.TABLE_GAME_PLAY_MIN_CHIPS)
					GameManager.Instance.rebuyPanel.gameObject.SetActive (false);
			}

			txtBlindBet.text = txtPayoutAmount.text = txtStraightOrBetter.text = txtTotalBet.text = txtWhoopAssCard.text = "";
			dealerWinParent.SetActive (true);
			tieParent.SetActive (false);
		} else {
			if (winnerReport.Winner.IsTie) {
				SetTieDescription (winnerReport);
				tieParent.SetActive (true);
			} else {
				SetPlayerWinReportDetails (winnerReport);
				tieParent.SetActive (false);
			}
		}

		if (GameManager.Instance.ownTablePlayer.playerInfo.Player_Status != (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME &&
		    (int)RoundController.GetInstance ().currentTableGameRound > (int)TABLE_GAME_ROUND.START) {

			SetWinningCards (winnerReport.Winner.Winner_Best_Cards);
			SetDealerBestCards (winnerReport.Loser.Loser_Best_Cards);

			if (winnerReport.Winner.Winner_Name.Equals (Constants.FIELD_PLAYER_NAME_DEALER)) {
				txtTitle.text = Constants.WIN_REPORT_TITLE;
				txtDealerHandRank.text = "Dealer's Hand: " + GetWinningRank (winnerReport.Winner.Winner_Rank);

				SetWinningCards (winnerReport.Loser.Loser_Best_Cards);
				SetDealerBestCards (winnerReport.Winner.Winner_Best_Cards);
			} else {
				txtTitle.text = Constants.WIN_REPORT_TITLE + " - " + GetWinningRank (winnerReport.Winner.Winner_Rank);
				txtDealerHandRank.text = "Dealer's Hand: " + GetWinningRank (winnerReport.Loser.Loser_Rank);

				SetWinningCards (winnerReport.Winner.Winner_Best_Cards);
				SetDealerBestCards (winnerReport.Loser.Loser_Best_Cards);
			}

			if (winnerReport.Winner.IsTie)
				txtTitle.text = Constants.WIN_REPORT_TITLE;
		} else {
			winningCardsObject.SetActive (false);
			txtTitle.text = Constants.WIN_REPORT_TITLE;
		}

		SoundManager.Instance.PlayGameCompleteSound (Camera.main.transform.position);
		gameObject.SetActive (true);
	}

	void SetPlayerWinReportDetails (WinnerReport winnerReport)
	{
		txtDealerWon.text = "";
		dealerWinParent.SetActive (false);
		tieParent.SetActive (false);
		txtWhoopAssCard.text = "WA Card money return: <color=white>" + Utility.GetAmount (winnerReport.Winner.WAAmount) + "</color>";
		txtTotalBet.text = "Total bet for:\n" + "Ante Bet, Bet-1, Bet-2, Bet-3, Play Bet: <color=white>" + Utility.GetAmount (winnerReport.Winner.BetAmount / 2) + "</color>\n" + "+ Equal Win Payout <color=white>" + Utility.GetAmount (winnerReport.Winner.BetAmount / 2) + "</color> = <color=white>" + Utility.GetAmount (winnerReport.Winner.BetAmount) + "</color>";
		if (winnerReport.Winner.StraightAmount > 0)
			txtStraightOrBetter.text = "\"Straight Bet\": " + "<color=white>" + Utility.GetAmount (winnerReport.Winner.StraightAmount) + "</color>";
		else {
			if (GameManager.Instance.straightAmount == 0)
				txtStraightOrBetter.text = "\"Straight Bet\": <color=white>Player didn't bet</color>";
			else
				txtStraightOrBetter.text = "\"Straight Bet\": <color=white>Player Loses</color> - <color=white>No Straight or Better</color>";
		}
		if (winnerReport.Winner.BliendAmount > 0)
			txtBlindBet.text = "\"Blind Bet\": " + GetWinningRank (winnerReport.Winner.Winner_Rank) + ": <color=white>" + Utility.GetAmount (winnerReport.Winner.BliendAmount) + "</color>";
		else
			txtBlindBet.text = "\"Blind Bet\": <color=white>Player Loses</color> - <color=white>No Flush or Better</color>";
		if (winnerReport.Winner.Rake_Amount > 0) {
			imgRakeAmountBullet.gameObject.SetActive (true);
			txtRakeAmount.text = "\"Rake Amount\": " + winnerReport.Winner.Rake_Percentage + "%  <color=white>" + Utility.GetAmount (winnerReport.Winner.Rake_Amount) + "</color>";
		} else {
			imgRakeAmountBullet.gameObject.SetActive (false);
			txtRakeAmount.text = "";
		}
		txtPayoutAmount.text = "Total Payout Amount: <color=white>" + Utility.GetAmount (winnerReport.Winner.Winning_Amount) + "</color>";

		if (GameManager.Instance.ownTablePlayer != null) {
			GameManager.Instance.ownTablePlayer.buyinChips += winnerReport.Winner.Winning_Amount;
			if (UIManager.Instance.isRealMoney)
				GameManager.Instance.ownTablePlayer.totalRealMoney += winnerReport.Winner.Winning_Amount;
			else
				GameManager.Instance.ownTablePlayer.totalChips += winnerReport.Winner.Winning_Amount;
			GameManager.Instance.ownTablePlayer.DisplayTotalChips ();

			if (winnerReport.Winner.Winning_Amount > Constants.TABLE_GAME_PLAY_MIN_CHIPS)
				GameManager.Instance.rebuyPanel.gameObject.SetActive (false);
		}
	}

	void SetTieDescription (WinnerReport winnerReport)
	{
		txtTieTitle.text = "Tie - " + GetWinningRank (winnerReport.Winner.Winner_Rank) + "!";
		txtTieDescription.text = "Return Bet:" + "\nAnte: <color=white>" + GameManager.Instance.anteAmount + "</color>" + "\nBet-1: <color=white>" + GameManager.Instance.bet1Amount + "</color>," + " Bet-2: <color=white>" + GameManager.Instance.bet2Amount + "</color>," + " Bet-3: <color=white>" + GameManager.Instance.bet3Amount + "</color>," + " Play: <color=white>" + GameManager.Instance.bet4Amount + "</color>" + " = <color=white>" + (GameManager.Instance.bet1Amount + GameManager.Instance.bet2Amount + GameManager.Instance.bet3Amount + GameManager.Instance.bet4Amount) + "</color>";
		txtTieDescription.text += "\nWhoopAss Card: <color=white>" + GameManager.Instance.waCardAmount + "</color>";
		if (winnerReport.Winner.StraightAmount > 0)
			txtTieDescription.text += "\nStraight Bet: <color=white>" + winnerReport.Winner.StraightAmount + GetStraightWinningRatio (winnerReport.Winner.Winner_Rank) + "</color>";
		if (winnerReport.Winner.BliendAmount > 0)
			txtTieDescription.text += "\nBlind Bet: <color=white>" + winnerReport.Winner.BliendAmount + "</color>";
		txtTieDescription.text += "\nTotal: <color=white>" + winnerReport.Winner.Winning_Amount + "</color>";
		if (winnerReport.Winner.StraightAmount == 0 || winnerReport.Winner.BliendAmount == 0) {
			txtTieDescription.text += "\n\nLost Bet:";
			double lostAmount = 0;
			if (winnerReport.Winner.StraightAmount == 0) {
				lostAmount += GameManager.Instance.straightAmount;
				txtTieDescription.text += "\nStraight Bet: <color=white>" + GameManager.Instance.straightAmount + "</color> (Lower than Straight)";
			}
			if (winnerReport.Winner.BliendAmount == 0) {
				lostAmount += GameManager.Instance.blindAmount;
				txtTieDescription.text += "\nBlind Bet: <color=white>" + GameManager.Instance.blindAmount + "</color> (Lower than Flush)";
			}
			txtTieDescription.text += "\nTotal: <color=white>" + lostAmount + "</color>";
		}

		if (GameManager.Instance.ownTablePlayer != null) {
			GameManager.Instance.ownTablePlayer.buyinChips += winnerReport.Winner.Winning_Amount;
			if (UIManager.Instance.isRealMoney)
				GameManager.Instance.ownTablePlayer.totalRealMoney += winnerReport.Winner.Winning_Amount;
			else
				GameManager.Instance.ownTablePlayer.totalChips += winnerReport.Winner.Winning_Amount;
			GameManager.Instance.ownTablePlayer.DisplayTotalChips ();

			if (winnerReport.Winner.Winning_Amount > Constants.TABLE_GAME_PLAY_MIN_CHIPS)
				GameManager.Instance.rebuyPanel.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Sets the winning rank
	/// </summary>
	private string GetWinningRank (int winningRank)
	{
		switch (winningRank) {
		case (int) WINNING_RANK.FLUSH:
			return "FLUSH";
		case (int) WINNING_RANK.FOUR_OF_A_KIND:
			return "FOUR OF A KIND";
		case (int) WINNING_RANK.FULL_HOUSE:
			return "FULL HOUSE";
		case (int) WINNING_RANK.HIGH_CARD:
			return "HIGH CARD";
		case (int) WINNING_RANK.ONE_PAIR:
			return "ONE PAIR";
		case (int) WINNING_RANK.ROYAL_FLUSH:
			return "ROYAL FLUSH";
		case (int) WINNING_RANK.STRAIGHT:
			return "STRAIGHT";
		case (int) WINNING_RANK.STRAIGHT_FLUSH:
			return "STRAIGHT FLUSH";
		case (int) WINNING_RANK.THREE_OF_A_KIND:
			return "THREE OF A KIND";
		case (int) WINNING_RANK.TWO_PAIR:
			return "TWO PAIR";
		}
		return "";
	}

	private void SetWinningCards (List<string> cardList)
	{
		for (int i = 0; i < winningCardList.Count; i++) {
			winningCardList [i].sprite = Resources.Load<Sprite> (Constants.RESOURCE_GAMECARDS + cardList [i]);
		}
	}

	private void SetDealerBestCards (List<string> cardList)
	{
		for (int i = 0; i < dealerWinningCardList.Count; i++) {
			dealerWinningCardList [i].sprite = Resources.Load<Sprite> (Constants.RESOURCE_GAMECARDS + cardList [i]);
		}
	}

	private string GetStraightWinningRatio (int winningRank)
	{
		switch (winningRank) {
		case (int) WINNING_RANK.FLUSH:
			return " (4-1)";
		case (int) WINNING_RANK.FOUR_OF_A_KIND:
			return " (27-1)";
		case (int) WINNING_RANK.FULL_HOUSE:
			return " (5-1)";
		case (int) WINNING_RANK.ROYAL_FLUSH:
			return " (64-1)";
		case (int) WINNING_RANK.STRAIGHT:
			return " (1-1)";
		case (int) WINNING_RANK.STRAIGHT_FLUSH:
			return " (36-1)";
		}

		return "";
	}

	private string GetBlindWinningRatio (int winningRank)
	{
		switch (winningRank) {
		case (int) WINNING_RANK.FLUSH:
			return " (1-1)";
		case (int) WINNING_RANK.FOUR_OF_A_KIND:
			return " (8-1)";
		case (int) WINNING_RANK.FULL_HOUSE:
			return " (2-1)";
		case (int) WINNING_RANK.ROYAL_FLUSH:
			return " (200-1)";
		case (int) WINNING_RANK.STRAIGHT_FLUSH:
			return " (40-1)";
		}

		return "";
	}
}