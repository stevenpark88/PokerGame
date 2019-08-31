using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChipsOnChips : MonoBehaviour
{
	public GameObject chipPrefab;
	public Text txtChips;

	private int totalChipObject = 0;

	private int totalRedChips = 0;
	private int totalGreenChips = 0;
	private int totalBlueChips = 0;

	public GameObject redChipPrefab;
	public GameObject greenChipPrefab;
	public GameObject blueChipPrefab;

	public Transform redInitialChip;
	public Transform greenInitialChip;
	public Transform blueInitialChip;

	private long totalChips = 10000;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
//		if (Input.GetKeyDown (KeyCode.Space)) {
//			if (totalChipObject < 10)
//				StartCoroutine (AddChips ());
//		}
//		if (Input.GetKeyDown (KeyCode.S))
//			StartCoroutine (GenerateChips ());
//		else if (Input.GetKeyDown (KeyCode.Escape))
//			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
//
//
//
//		if (Input.GetKeyDown (KeyCode.G)) {
//			totalChips = Random.Range (1000, 100000);
//			Debug.Log ("Total Chips  : " + totalChips);
//			Debug.Log ("Red Chips(" + Constants.RED_CHIP_VALUE + ")  : " + CalculateRedChips ());
//			Debug.Log ("Green Chips(" + Constants.GREEN_CHIP_VALUE + ")  : " + CalculateGreenChips ());
//			Debug.Log ("Blue Chips(" + Constants.BLUE_CHIP_VALUE + ")  : " + CalculateBlueChips ());
//		}
	}

	private IEnumerator AddChips ()
	{
		for (int i = 0; i < 10; i++) {
			GameObject chip = Instantiate (chipPrefab, new Vector2 (transform.position.x, GetYPos (++totalChipObject)), Quaternion.identity) as GameObject;
			chip.transform.SetParent (transform);
			chip.transform.localScale = Vector3.one;

			yield return new WaitForEndOfFrame ();
		}
	}

	private float GetYPos (int i)
	{
		return transform.position.y + (i * 6f);
	}

	private float GetRedChipYPos (int i)
	{
		return redInitialChip.position.y + (i * 5f);
	}

	private float GetGreenChipYPos (int i)
	{
		return greenInitialChip.position.y + (i * 5f);
	}

	private float GetBlueChipYPos (int i)
	{
		return blueInitialChip.position.y + (i * 5f);
	}

	private IEnumerator GenerateChips ()
	{
//		int redChips = CalculateRedChips ();
//		int greenChips = CalculateGreenChips ();
//		int blueChips = CalculateBlueChips ();
//
//		for (int r = 0; r < redChips; r++) {
//			GameObject chip = Instantiate (redChipPrefab, new Vector2 (redInitialChip.position.x, GetRedChipYPos (++totalRedChips)), Quaternion.identity) as GameObject;
//			chip.transform.SetParent (redInitialChip);
//			chip.transform.localScale = Vector3.one;
//
//			yield return 0;
//		}
//
//		for (int g = 0; g < greenChips; g++) {
//			GameObject chip = Instantiate (greenChipPrefab, new Vector2 (greenInitialChip.position.x, GetGreenChipYPos (++totalGreenChips)), Quaternion.identity) as GameObject;
//			chip.transform.SetParent (greenInitialChip);
//			chip.transform.localScale = Vector3.one;
//
//			yield return 0;
//		}
//
//		for (int b = 0; b < blueChips; b++) {
//			GameObject chip = Instantiate (blueChipPrefab, new Vector2 (blueInitialChip.position.x, GetBlueChipYPos (++totalBlueChips)), Quaternion.identity) as GameObject;
//			chip.transform.SetParent (blueInitialChip);
//			chip.transform.localScale = Vector3.one;
//
//			yield return 0;
//		}

		yield return 0;
	}

	private long CalculateRedChips ()
	{
		long t = totalChips / Constants.RED_CHIP_VALUE;

		return t;
	}

	private long CalculateGreenChips ()
	{
		long val = totalChips % Constants.RED_CHIP_VALUE;
		long t = val / Constants.GREEN_CHIP_VALUE;

		return t;
	}

	private long CalculateBlueChips ()
	{
		long val = totalChips % Constants.RED_CHIP_VALUE;
		long gval = val % Constants.GREEN_CHIP_VALUE;
		long t = gval / Constants.BLUE_CHIP_VALUE;

		return t;
	}
}