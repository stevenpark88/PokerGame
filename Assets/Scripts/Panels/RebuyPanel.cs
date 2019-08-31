using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RebuyPanel : MonoBehaviour
{
	public Text txtTitle;
	public Text txtSliderValue;
	public Slider rebuySlider;

	// Use this for initialization
	void OnEnable ()
	{
		txtTitle.text = Constants.MESSAGE_SELECT_REBUY_AMOUNT;
	}

	public void DisplayRebuyPanel (double maxAmount, double minAmount = 0)
	{
		maxAmount = maxAmount < 0 ? 0 : maxAmount;
		minAmount = minAmount > maxAmount ? maxAmount : minAmount;

		rebuySlider.minValue = (float)minAmount;
		rebuySlider.maxValue = (float)maxAmount;
		rebuySlider.value = (float)minAmount;
		OnRebuySliderValueChanged ();
		gameObject.SetActive (true);
	}

	public void OnRebuyButtonTap ()
	{
		if (rebuySlider.value > 0) {
			SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);
			gameObject.SetActive (false);

			NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_REBUY + rebuySlider.value);
		}
	}

	public void OnCancelButtonTap ()
	{
//		if (UIManager.Instance.texassGamePanel.gameObject.activeSelf &&
//		    UIManager.Instance.texassGamePanel.canRebuy)
//			UIManager.Instance.texassGamePanel.btnRebuy.gameObject.SetActive (true);
//		else if (UIManager.Instance.whoopAssGamePanel.gameObject.activeSelf &&
//		         UIManager.Instance.whoopAssGamePanel.canRebuy)
//			UIManager.Instance.whoopAssGamePanel.btnRebuy.gameObject.SetActive (true);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);
		gameObject.SetActive (false);
	}

	public void OnRebuySliderValueChanged ()
	{
//		txtSliderValue.text = Utility.GetCurrencyPrefix() + rebuySlider.value;
		txtSliderValue.text = Utility.GetCommaSeperatedAmount ((double)rebuySlider.value);
	}
}