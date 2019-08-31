using UnityEngine;
using System.Collections;

public class TurnManager 
{
	PlayerBean player;
	int playerAction;
	int betAmount;

	public TurnManager(PlayerBean player,int playerAction,int betAmount) {
	
		this.player=player;
		this.playerAction = playerAction;
		this.betAmount = betAmount;

	}
	
	
	public PlayerBean getPlayer() {
		return player;
	}
	
	public void setPlayer(PlayerBean player) {
		this.player = player;
	}
	
	public int getPlayerAction() {
		return playerAction;
	}
	
	public void setPlayerAction(int playerAction) {
		this.playerAction = playerAction;
	}
	
	public int getBetAmount() {
		return betAmount;
	}
	
	public void setBetAmount(int betAmount) {
		this.betAmount = betAmount;
	}
}

