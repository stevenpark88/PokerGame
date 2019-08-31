using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class GamePlayersManager : MonoBehaviour
//{
public class PlayersManager
{
	List<PlayerBean> listPlayers ;
		
	public PlayerBean loggedPlayer;

	public PlayersManager ()
	{
		listPlayers = new List<PlayerBean> ();
	}

	public void addPlayer (PlayerBean playerBean)
	{
		listPlayers.Add (playerBean);
		if (playerBean.getPlayerName ().Equals (appwarp.username)) {
			loggedPlayer = playerBean;
		}
	}

	public PlayerBean getLoggedPlayer(){
		return loggedPlayer;
	}
	public void removePlayer (PlayerBean playerBean)
	{
		playerBean.goneFromTable ();
		listPlayers.Remove (playerBean);
	}

	public void addBalanceToPlayer(string playerName,int balance){
		foreach (PlayerBean player in listPlayers){
			if(player.getPlayerName().Equals(playerName)){
				player.setPlayerBalance (player.getBalance () + balance);
//				DEBUG.Log ("ReBuy : "+playerName+" >> "+balance);
				return;
			}
		}
	}
	public int totalPlayerOnTable ()
	{
		return listPlayers.Count;
	}
	public List<PlayerBean> getAllPlayers(){
		return listPlayers;
	}
	public bool isPlayerAlreadyAdded(string name){
		foreach (PlayerBean player in listPlayers){
			if(player.getPlayerName().Equals(name)){
				return true;
			}
		}
		return false;
	}

	public PlayerBean getDealerPlayer(){
		foreach (PlayerBean player in listPlayers){
			if(player.isDealerPlayer()){
				return player;
			}
		}
		return null;
	}
	public PlayerBean getSmallBlindPlayer(){
		foreach (PlayerBean player in listPlayers){
			if(player.isSmallBlindPlayer()){
				return player;
			}
		}
		return null;
	}
	public PlayerBean getBigBlindPlayer(){
		foreach (PlayerBean player in listPlayers){
			if(player.isBigBlindPlayer()){
				return player;
			}
		}
		return null;
	}
	public PlayerBean getPlayerFromName(string name){
		foreach (PlayerBean player in listPlayers){
			if(player.getPlayerName().Equals(name)){
				return player;
			}
		}
		return null;
	}

	public PlayerBean findWinnerPlayerFromName(string name){
		PlayerBean winnerPlayer = null;
		foreach (PlayerBean player in listPlayers){
			if(player.getPlayerName().Equals(name)){
				winnerPlayer = player;
				winnerPlayer.setWinnerUser();

			}

			player.setPlayerLastAction(GameConstant.ACTION_DEALER,player.getBalance());
			player.stopAnimation();
		}
		return winnerPlayer;
	}
	public void setLastPlayerAction(string name,int action,int betAmount,int totalBetAmountInRound,int totalPlayerBalance){
		foreach (PlayerBean player in listPlayers){
			
			if(player.getPlayerName().Equals(name)){
				player.setPlayerLastAction(action,betAmount);

			}else{
				player.setPlayerLastAction(GameConstant.ACTION_DEALER,betAmount);
			}

			//player.setPlayerBalance(totalPlayerBalance);
		}
	}
	public void setBigBlindPlayer(string name){
		if (!name.Equals (GameConstant.RESPONSE_DATA_SEPRATOR)) {
		
		foreach (PlayerBean player in listPlayers){
			if(player.getPlayerName().Equals(name)){
				player.setBigBlindPlayer(true);
			}else{
					if(!player.isSmallBlindPlayer() && !player.isDealerPlayer()){
					player.setBigBlindPlayer(false);
				}
			}
			}
		}
	}

	public void setSmallBlindPlayer(string name){
		if (!name.Equals (GameConstant.RESPONSE_DATA_SEPRATOR)) {
			foreach (PlayerBean player in listPlayers) {
				if (player.getPlayerName ().Equals (name)) {
					player.setSmallBlindPlayer (true);
				} else {
					if (!player.isBigBlindPlayer () && !player.isDealerPlayer()) {
						player.setSmallBlindPlayer (false);
					}
				}
			}
		}
	}
	public void setDealerPlayer(string name){
		if (!name.Equals (GameConstant.RESPONSE_DATA_SEPRATOR)) {
			foreach (PlayerBean player in listPlayers) {
				if (player.getPlayerName ().Equals (name)) {
					player.setDealerPlayer (true);
				} else {
					if (!player.isBigBlindPlayer () && !player.isSmallBlindPlayer()) {
						player.setSmallBlindPlayer (false);
					}
				}
			}
		}
	}
	public void setPlayerBetAmount(PlayerBean player,int betamount,int totalBetAmountInRound,int totalPlayerBalance){
		player.setBetAmount(betamount,totalBetAmountInRound);
		player.setPlayerBalance(totalPlayerBalance);
	}
	public void resetPlayersBetAmount(){
		foreach (PlayerBean player in listPlayers) {
			player.setBetAmount(0,0);
		}
	}

	public PlayerBean setCurrentTurnPlayer(string name){
		PlayerBean currentPlayer=null;
		foreach (PlayerBean player in listPlayers) {
			if (player.getPlayerName ().Equals (name)) {
				player.setMyTurn(true);
				currentPlayer = player;
			} else {
				player.setMyTurn(false);
			}
		}
		return currentPlayer;
	}

	public void openAllPlayerCards(){
		foreach (PlayerBean player in listPlayers) {
			player.stopAnimation();
			if(!player.isWaitingForGame())
				player.setOpenCardsOnTable();
		}
	}

	public void closeActivePlayersCards(){
		
		foreach (PlayerBean player in listPlayers) {
			if(!player.isWaitingForGame())
				player.setCloseCardsOnTable();
		}
	}
	public void openWAUpcardsIfBuyed(){
		foreach (PlayerBean player in listPlayers) {
			player.upWACardOpen();
		}
	}

	public bool isAllPlayersAreFolded(){
		int foldedCntr = 0;
		foreach (PlayerBean playerBean in listPlayers) {
			if(playerBean.isFoldedPlayer()){
				foldedCntr++;
			}
		}
//		System.out.println("Number Of players : "+playersManager.getAllAvailablePlayers().size()+" >>Total folded : "+foldedCntr);
		if(listPlayers.Count-1 == foldedCntr){
			return true;
		}
		return false;
	}
}

//}

