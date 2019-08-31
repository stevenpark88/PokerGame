using UnityEngine;
using System.Collections;

namespace UIAccount
{
	public class GameTipsPanel : MonoBehaviour
	{
		#region PUBLIC_VARIABLES
		public GameObject makeGoodDicisionsObj;
		public GameObject newPokerPlayerObj;

		[Header ("Button Arrows")]
		public Transform dicisionArrow;
		public Transform playerArrow;
		#endregion

		#region PRIVATE_VARIABLES

		#endregion

		#region UNITY_CALLBACKS

		void OnEnable()
		{
			OnMakeGoodDicisionsButtonTap ();
		}

		void OnDisable ()
		{
			makeGoodDicisionsObj.SetActive (false);
			newPokerPlayerObj.SetActive (false);
		}

		#endregion

		#region DELEGATE_CALLBACKS

		#endregion

		#region PUBLIC_METHODS

		public void OnMakeGoodDicisionsButtonTap()
		{
			if (!makeGoodDicisionsObj.activeSelf) {
				makeGoodDicisionsObj.SetActive (true);
				newPokerPlayerObj.SetActive (false);

				dicisionArrow.rotation = new Quaternion (180, 0, 0, 0);
				playerArrow.rotation = new Quaternion (0, 0, 0, 0);
			} else {
				makeGoodDicisionsObj.SetActive (false);
				newPokerPlayerObj.SetActive (false);

				dicisionArrow.rotation = new Quaternion (0, 0, 0, 0);
				playerArrow.rotation = new Quaternion (0, 0, 0, 0);
			}
		}

		public void OnNewPokerPlayerButtonTap()
		{
			if (!newPokerPlayerObj.activeSelf) {
				newPokerPlayerObj.SetActive (true);
				makeGoodDicisionsObj.SetActive (false);

				playerArrow.rotation = new Quaternion (180, 0, 0, 0);
				dicisionArrow.rotation = new Quaternion (0, 0, 0, 0);
			} else {
				newPokerPlayerObj.SetActive (false);
				makeGoodDicisionsObj.SetActive (false);

				playerArrow.rotation = new Quaternion (0, 0, 0, 0);
				dicisionArrow.rotation = new Quaternion (0, 0, 0, 0);
			}
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		#region COROUTINES

		#endregion
	}
}