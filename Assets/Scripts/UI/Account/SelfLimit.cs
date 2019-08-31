using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UIAccount
{
	public class SelfLimit : MonoBehaviour
	{
		#region PUBLIC_VARIABLES
		public Text txtSelfLimitTitle;

		public List<Toggle> toggleButtonsList;

		public List<double> selfLimitAmountList = new List<double> () {
			0, 100, 500, 1000, 3000
		};

		public ToggleGroup selfLimitToggleGroup;
		#endregion

		#region PRIVATE_VARIABLES
		private double selectedSelfLimit;
		#endregion

		#region UNITY_CALLBACKS

		// Use this for initialization
		void OnEnable ()
		{
			txtSelfLimitTitle.text = Constants.MESSAGE_SELF_LIMIT_TITLE;

			SetToggleValues ();

			if (LoginScript.loggedInPlayer != null) {
				if (LoginScript.loggedInPlayer.daily_limit == selfLimitAmountList [0]) {
					toggleButtonsList [0].isOn = true;
				} else if (LoginScript.loggedInPlayer.daily_limit == selfLimitAmountList [1]) {
					toggleButtonsList [1].isOn = true;
				} else if (LoginScript.loggedInPlayer.daily_limit == selfLimitAmountList [2]) {
					toggleButtonsList [2].isOn = true;
				} else if (LoginScript.loggedInPlayer.daily_limit == selfLimitAmountList [3]) {
					toggleButtonsList [3].isOn = true;
				} else if (LoginScript.loggedInPlayer.daily_limit == selfLimitAmountList [4]) {
					toggleButtonsList [4].isOn = true;
				}
			}

			AccountAPIManager.onSelfLimitUpdated += OnSelfLimitUpdated;
		}

		void OnDisable()
		{
			AccountAPIManager.onSelfLimitUpdated -= OnSelfLimitUpdated;
		}

		#endregion

		#region DELEGATE_CALLBACKS
		private void OnSelfLimitUpdated (string info)
		{
			UIManager.Instance.HideLoader ();

			LoginScript.loggedInPlayer.daily_limit = selectedSelfLimit;
		}
		#endregion

		#region PUBLIC_METHODS

		public void OnSubmitButtonTap ()
		{
			for (int i = 0; i < toggleButtonsList.Count; i++) {
				if (toggleButtonsList [i].isOn)
					selectedSelfLimit = selfLimitAmountList [i];
			}

			Debug.Log ("Selected Self Limit Amount  : " + selectedSelfLimit);

			AccountAPIManager.GetInstance ().SetSelfLimit (selectedSelfLimit);
		}

		#endregion

		#region PRIVATE_METHODS

		private void SetToggleValues()
		{
			for (int i = 0; i < toggleButtonsList.Count; i++) {
				if (selfLimitAmountList [i] == 0) {
					toggleButtonsList [i].GetComponentInChildren<Text> ().text = "No Limit";
				} else {
					toggleButtonsList [i].GetComponentInChildren<Text> ().text = Utility.GetCommaSeperatedAmount (selfLimitAmountList [i], true);
				}
			}
		}

		#endregion

		#region COROUTINES

		#endregion
	}
}