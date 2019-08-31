using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UIAccount
{
	public class GameRulesPanel : MonoBehaviour
	{
		#region PUBLIC_VARIABLES

		public Button btnPayouts;
		public Button btnRealMoneyRake;
		public Button btnSnGRules;
		public Button btnTournamentRules;

		public Sprite spSelectedButton;
		public Sprite spDefaultButton;

		public GameObject payoutsObj;
		public GameObject realMoneyRake;
		public GameObject sngRulesObj;
		public GameObject tournamentRules;

		#endregion

		#region PRIVATE_VARIABLES

		#endregion

		#region UNITY_CALLBACKS

		// Use this for initialization
		void Start ()
		{

		}

		#endregion

		#region DELEGATE_CALLBACKS

		#endregion

		#region PUBLIC_METHODS

		public void OnPayoutsButtonTap ()
		{
			btnPayouts.image.sprite = spSelectedButton;
			btnRealMoneyRake.image.sprite = spDefaultButton;
			btnSnGRules.image.sprite = spDefaultButton;
			btnTournamentRules.image.sprite = spDefaultButton;

			Color goldenColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_GOLDEN_HEADER, out goldenColor);
			btnPayouts.GetComponentInChildren<Text> ().color = goldenColor;

			Color redColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_RED_HEADER, out redColor);
			btnRealMoneyRake.GetComponentInChildren<Text> ().color = redColor;
			btnSnGRules.GetComponentInChildren<Text> ().color = redColor;
			btnTournamentRules.GetComponentInChildren<Text> ().color = redColor;

			payoutsObj.SetActive (true);
			realMoneyRake.SetActive (false);
			sngRulesObj.SetActive (false);
			tournamentRules.SetActive (false);
		}

		public void OnRealMoneyRakeButtonTap ()
		{
			btnPayouts.image.sprite = spDefaultButton;
			btnRealMoneyRake.image.sprite = spSelectedButton;
			btnSnGRules.image.sprite = spDefaultButton;
			btnTournamentRules.image.sprite = spDefaultButton;

			Color goldenColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_GOLDEN_HEADER, out goldenColor);
			btnRealMoneyRake.GetComponentInChildren<Text> ().color = goldenColor;

			Color redColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_RED_HEADER, out redColor);
			btnPayouts.GetComponentInChildren<Text> ().color = redColor;
			btnSnGRules.GetComponentInChildren<Text> ().color = redColor;
			btnTournamentRules.GetComponentInChildren<Text> ().color = redColor;

			payoutsObj.SetActive (false);
			realMoneyRake.SetActive (true);
			sngRulesObj.SetActive (false);
			tournamentRules.SetActive (false);
		}

		public void OnSnGRulesButtonTap ()
		{
			btnPayouts.image.sprite = spDefaultButton;
			btnRealMoneyRake.image.sprite = spDefaultButton;
			btnSnGRules.image.sprite = spSelectedButton;
			btnTournamentRules.image.sprite = spDefaultButton;

			Color goldenColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_GOLDEN_HEADER, out goldenColor);
			btnSnGRules.GetComponentInChildren<Text> ().color = goldenColor;

			Color redColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_RED_HEADER, out redColor);
			btnPayouts.GetComponentInChildren<Text> ().color = redColor;
			btnRealMoneyRake.GetComponentInChildren<Text> ().color = redColor;
			btnTournamentRules.GetComponentInChildren<Text> ().color = redColor;

			payoutsObj.SetActive (false);
			realMoneyRake.SetActive (false);
			sngRulesObj.SetActive (true);
			tournamentRules.SetActive (false);
		}

		public void OnTournamentRulesButtonTap ()
		{
			btnPayouts.image.sprite = spDefaultButton;
			btnRealMoneyRake.image.sprite = spDefaultButton;
			btnSnGRules.image.sprite = spDefaultButton;
			btnTournamentRules.image.sprite = spSelectedButton;

			Color goldenColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_GOLDEN_HEADER, out goldenColor);
			btnTournamentRules.GetComponentInChildren<Text> ().color = goldenColor;

			Color redColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_RED_HEADER, out redColor);
			btnPayouts.GetComponentInChildren<Text> ().color = redColor;
			btnRealMoneyRake.GetComponentInChildren<Text> ().color = redColor;
			btnSnGRules.GetComponentInChildren<Text> ().color = redColor;

			payoutsObj.SetActive (false);
			realMoneyRake.SetActive (false);
			sngRulesObj.SetActive (false);
			tournamentRules.SetActive (true);
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		#region COROUTINES

		#endregion
	}
}