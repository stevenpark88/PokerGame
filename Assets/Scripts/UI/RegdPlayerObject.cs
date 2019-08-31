using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RegdPlayerObject : MonoBehaviour
{
	public Image imgProfile;
	public Text txtName;
	public Text txtTotalChips;
	public Text txtBuyin;
	public Image imgCountryFlag;

	public Image imgBackground;

	// Use this for initialization
	void Start ()
	{
	
	}

	public void DownloadCountryFlag (string countryFlagUrl)
	{
		StartCoroutine (DownloadFlag (countryFlagUrl));
	}

	private IEnumerator DownloadFlag (string countyFlagUrl)
	{
		WWW www = new WWW (countyFlagUrl);

		yield return www;

		Sprite spFlag = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), Vector2.zero);
		imgCountryFlag.sprite = spFlag;
	}
}