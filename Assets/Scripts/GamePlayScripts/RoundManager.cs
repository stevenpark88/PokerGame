using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class RoundManager
{

	/**Current status of round. e.g active, finish, pending*/
	int status;
	/** Track all player action in round */
	List<TurnManager> listTurn = new List<TurnManager> ();
	int currentRound;

	public RoundManager (int currentRound)
	{
		this.currentRound = currentRound;
		listTurn = new List<TurnManager> ();
	}

	/**
	 * Add turn record when player performed his action
	 */
	public void addTurnRecord (TurnManager turnManager)
	{
		this.listTurn.Add (turnManager);
	}

	/**
	 * Fetch all turn's data in current round
	 * @return ArrayList<TurnManager>
	 */
	public List<TurnManager> getAllTurnRecords ()
	{
		return listTurn;
	}

	/**
	 * Get current status of round it may be 
	ROUND_STATUS_ACTIVE, ROUND_STATUS_PENDING, ROUND_STATUS_FINISH
	 * @return index
	 */
	public int getStatus ()
	{
		return status;
	}

	public int getRound ()
	{
		return this.currentRound;
	}

	/**
	 * Update current status of round it may be 
	 * ROUND_STATUS_ACTIVE, ROUND_STATUS_PENDING, ROUND_STATUS_FINISH
	 * @param status
	 */
	public void setStatus (int status)
	{
		this.status = status;
	}

	public TurnManager getLastActivePlayerTurn ()
	{
		TurnManager lastPlayer = null;
		TurnManager allInPlayer = null;
		for (int i = listTurn.Count - 1; i >= 0; i--) {
			lastPlayer = listTurn [i];
			if (!lastPlayer.getPlayer ().isFoldedPlayer () &&
//				!lastPlayer.getPlayer ().isAllInPlayer () && 
				lastPlayer.getPlayerAction () != GameConstant.ACTION_CHECK) {
				if (allInPlayer == null ) {
					allInPlayer = lastPlayer;
				} else {
					int totalBetOfLastPlayer = getTotalPlayerBetAmount (lastPlayer.getPlayer());
					int totalBetOfAllInPlayer = getTotalPlayerBetAmount (allInPlayer.getPlayer());
					if (totalBetOfLastPlayer > totalBetOfAllInPlayer) {
//						DEBUG.Log ("Crony :1 >> "+lastPlayer.getBetAmount()+" >> "+ allInPlayer.getBetAmount() +" << "+totalBetOfLastPlayer +" << "+ totalBetOfAllInPlayer);
						return lastPlayer;	
					} else {
//						DEBUG.Log ("Crony :2");
						return allInPlayer;
					}
				}
			}
		}
		if (allInPlayer != null && allInPlayer.getBetAmount () != 0) {
//			DEBUG.Log ("Crony :3");
			return allInPlayer;
		}
//		DEBUG.Log ("Crony :4");
		return null;
	}

	public int getPlayerLastAction (PlayerBean player)
	{
		int action = 0;
		foreach (TurnManager turnManager in listTurn) {
			if (turnManager.getPlayer ().getPlayerName ().Equals (player.getPlayerName ())) {
				action = turnManager.getPlayerAction ();
			}
		}
		return action;
	}

	public int getTotalPlayerBetAmount (PlayerBean player)
	{
		int totalBet = 0;
		foreach (TurnManager turnManager in listTurn) {
			if (turnManager.getPlayer ().getPlayerName ().Equals (player.getPlayerName ())) {
				totalBet += turnManager.getBetAmount (); // This will add every turn bet amount
//				totalBet = turnManager.getBetAmount (); // total round bet
			}
		}
		return totalBet;
	}

	public int getTotalRoundBetAmount ()
	{
		int totalBet = 0;
		foreach (TurnManager turnManager in listTurn) {
			totalBet += turnManager.getBetAmount ();
		}
		return totalBet;
	}

	public int getTotalRaiseCounter ()
	{
		int total = 0;
		foreach (TurnManager turnManager in listTurn) {
			if (turnManager.getPlayerAction () == GameConstant.ACTION_RAISE) {
				total += 1;
			}
		}
		return total;
	}

	public int getWACardAmount ()
	{
		int waCardAmt = 0;
		int preBetAmount = 0;
		foreach (TurnManager turnManager in listTurn) {
			 
			if (turnManager.getPlayerAction () == GameConstant.ACTION_BET) {
				waCardAmt += turnManager.getBetAmount ();	
			} else if (turnManager.getPlayerAction () == GameConstant.ACTION_RAISE) {
				waCardAmt += turnManager.getBetAmount () - preBetAmount;
			}
			preBetAmount = turnManager.getBetAmount ();
		}
//		if (waCardAmt == 0) {
//			waCardAmt = 10;//static BB amount
//		}
		return waCardAmt;
	}
}

