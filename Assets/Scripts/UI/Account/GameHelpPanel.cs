using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UIAccount
{
	public class GameHelpPanel : MonoBehaviour
	{
		#region PUBLIC_VARIABLES
		public Button btnTableGame;
		public Button btnTexassGame;
		public Button btnWhoopAssGame;

		public Sprite spSelectedButton;
		public Sprite spDefaultButton;

		public GameObject tableGameObj;
		public GameObject texassGameObj;
		public GameObject whoopAssGameObj;
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

		public void OnTableGameButtonTap()
		{
			btnTableGame.image.sprite = spSelectedButton;
			btnTexassGame.image.sprite = spDefaultButton;
			btnWhoopAssGame.image.sprite = spDefaultButton;

			Color goldenColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_GOLDEN_HEADER, out goldenColor);
			btnTableGame.GetComponentInChildren<Text> ().color = goldenColor;

			Color redColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_RED_HEADER, out redColor);
			btnTexassGame.GetComponentInChildren<Text> ().color = redColor;
			btnWhoopAssGame.GetComponentInChildren<Text> ().color = redColor;

			tableGameObj.SetActive (true);
			texassGameObj.SetActive (false);
			whoopAssGameObj.SetActive (false);
		}

		public void OnTexassGameButtonTap()
		{
			btnTableGame.image.sprite = spDefaultButton;
			btnTexassGame.image.sprite = spSelectedButton;
			btnWhoopAssGame.image.sprite = spDefaultButton;

			Color goldenColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_GOLDEN_HEADER, out goldenColor);
			btnTexassGame.GetComponentInChildren<Text> ().color = goldenColor;

			Color redColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_RED_HEADER, out redColor);
			btnTableGame.GetComponentInChildren<Text> ().color = redColor;
			btnWhoopAssGame.GetComponentInChildren<Text> ().color = redColor;

			tableGameObj.SetActive (false);
			texassGameObj.SetActive (true);
			whoopAssGameObj.SetActive (false);
		}

		public void OnWhoopAssGameButtonTap()
		{
			btnTableGame.image.sprite = spDefaultButton;
			btnTexassGame.image.sprite = spDefaultButton;
			btnWhoopAssGame.image.sprite = spSelectedButton;

			Color goldenColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_GOLDEN_HEADER, out goldenColor);
			btnWhoopAssGame.GetComponentInChildren<Text> ().color = goldenColor;

			Color redColor;
			ColorUtility.TryParseHtmlString (Constants.HEX_RED_HEADER, out redColor);
			btnTableGame.GetComponentInChildren<Text> ().color = redColor;
			btnTexassGame.GetComponentInChildren<Text> ().color = redColor;

			tableGameObj.SetActive (false);
			texassGameObj.SetActive (false);
			whoopAssGameObj.SetActive (true);
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		#region COROUTINES

		#endregion
	}
}