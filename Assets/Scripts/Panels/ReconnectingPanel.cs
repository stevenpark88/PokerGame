using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReconnectingPanel : MonoBehaviour
{
	public Image loaderImage;
	public Text txtMessage;

	void OnEnable()
	{
		txtMessage.text = Constants.MESSAGE_RECONNECTING;
		StartCoroutine ("StartRotating");
	}

	void OnDisable()
	{
		StopCoroutine ("StartRotating");
	}

	/// <summary>
	/// Starts the rotating.
	/// </summary>
	/// <returns>The rotating.</returns>
	private IEnumerator StartRotating()
	{
		while (true) {
			loaderImage.transform.RotateAround (loaderImage.transform.position, -Vector3.forward, 10);

			yield return 0;
		}
	}
}