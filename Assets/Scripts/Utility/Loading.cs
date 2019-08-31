using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
	public Image loaderImage;
	public Text message;

	// Use this for initialization
	void OnEnable()
	{
		transform.SetAsLastSibling ();
//		StartCoroutine (StartAnimating ());
	}

	void Update()
	{
		loaderImage.transform.Rotate (Vector3.forward * -10f);
	}

	/// <summary>
	/// Starts the fill animation.
	/// </summary>
	/// <returns>The fill animation.</returns>
	private IEnumerator StartAnimating()
	{
		while (true) {
			loaderImage.transform.RotateAround (loaderImage.transform.position, -Vector3.forward, 10);

			yield return 0;
		}
	}
}