using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UIAccount
{
	public class SelfExclusion : MonoBehaviour
	{
		#region PUBLIC_VARIABLES
		public Text txtSelfExclusionTempTitle;
		public Text txtSelfExclusionPermaTitle;

		public List<Toggle> toggleButtonsList;

		public List<int> selfExclusionDayList = new List<int> () {
			7, 60
		};

		public Toggle togglePermaExlusion;

		public ToggleGroup selfExclusionToggleGroup;
		#endregion

		#region PRIVATE_VARIABLES

		#endregion

		#region UNITY_CALLBACKS

		// Use this for initialization
		void OnEnable ()
		{
			txtSelfExclusionTempTitle.text = Constants.MESSAGE_SELF_EXCLUSION_TEMP;
			txtSelfExclusionPermaTitle.text = Constants.MESSAGE_SELF_EXCLUSION_PERMA;

			SetToggleValues ();

			AccountAPIManager.onSelfExclusionUpdated += OnSelfExclusionUpdated;
		}

		void OnDisable()
		{
			AccountAPIManager.onSelfExclusionUpdated -= OnSelfExclusionUpdated;
		}

		#endregion

		#region DELEGATE_CALLBACKS

		private void OnSelfExclusionUpdated (string info)
		{
			UIManager.Instance.HideLoader ();
		}

		#endregion

		#region PUBLIC_METHODS

		public void OnTempExclusionSubmitButtonTap ()
		{
			int selectedSelfExclusionDays = 0;

			for (int i = 0; i < toggleButtonsList.Count; i++) {
				if (toggleButtonsList [i].isOn) {
					selectedSelfExclusionDays = selfExclusionDayList [i];
				}
			}

			Debug.Log ("Selected Self Exlusion Days  : " + selectedSelfExclusionDays);

			AccountAPIManager.GetInstance ().SetSelfExclusion ("" + selectedSelfExclusionDays);
		}

		public void OnPermaExclusionSubmitButtonTap()
		{
			Debug.Log ("Permanent Exclusion  : " + togglePermaExlusion.isOn);
		}

		#endregion

		#region PRIVATE_METHODS

		private void SetToggleValues()
		{
			for (int i = 0; i < toggleButtonsList.Count; i++) {
				toggleButtonsList [i].GetComponentInChildren<Text> ().text = selfExclusionDayList [i] + " Days";
			}
		}

		#endregion

		#region COROUTINES

		#endregion
	}
}