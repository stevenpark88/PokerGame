
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum SUITS
{
	NONE,
	heart,
	spade,
	diamond, 
	club
}

public enum RANKS
{
	NONE,
	two=2,
	three,
	four,
	five,
	six,
	seven,
	eight,
	nine,
	ten,
	jack,
	queen,
	king,
	ace
}

public class Card
{
	SUITS suit;
	RANKS ranks;
	Sprite cardSprite;
	string cardName="";

	public Card(String cardName){
		this.cardName = cardName;
		this.cardSprite = Resources.Load<Sprite>(GameConstant.RES_PATH_CARDS+cardName);
//		Debug.Log (GameConstant.RES_PATH_CARDS+cardName);
		SetRankAndSuit ();
	}

	public string getCardName(){
		return cardName;
	}
	public void SetRankAndSuit ()
	{
		if (cardSprite != null) {

			string[] Property = cardSprite.name.Split ('_');
			SetRank (Property [0]);
			SetSuit (Property [1]);
		}
	}

	public void SetSuit (string suit)
	{
		switch (suit) {
		case "heart": 
			this.suit = SUITS.heart;
			break;
		case "spade": 
			this.suit = SUITS.spade;
			break;
		case "diamond": 
			this.suit = SUITS.diamond;
			break;
		case "club": 
			this.suit = SUITS.club;
			break;

		}
	}

	public void SetRank (string rank)
	{
		switch (rank) {
		case "two": 
			this.ranks = RANKS.two;
			break;
		case "three": 
			this.ranks = RANKS.three;
			break;
		case "four": 
			this.ranks = RANKS.four;
			break;
		case "five": 
			this.ranks = RANKS.five;
			break;
		case "six": 
			this.ranks = RANKS.six;
			break;
		case "seven": 
			this.ranks = RANKS.seven;
			break;
		case "eight": 
			this.ranks = RANKS.eight;
			break;
		case "nine": 
			this.ranks = RANKS.nine;
			break;
		case "ten": 
			this.ranks = RANKS.ten;
			break;
		case "jack": 
			this.ranks = RANKS.jack;
			break;
		case "queen": 
			this.ranks = RANKS.queen;
			break;
		case "king": 
			this.ranks = RANKS.king;
			break;
		case "ace": 
			this.ranks = RANKS.ace;
			break;
			
		}
	}

	public RANKS getRank ()
	{
		return ranks;
	}

	public SUITS getSuit ()
	{
		return suit;
	}

	public int GetValue ()
	{
		return (int)this.ranks;
	}

	public Sprite getCardSprite(){
		return cardSprite;
	}
}





