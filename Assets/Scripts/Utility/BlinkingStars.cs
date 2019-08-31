using UnityEngine;
using System.Collections;

public class BlinkingStars : MonoBehaviour
{
	// Use this for initialization
	void OnEnable ()
	{
		StartCoroutine (AnimateStar ());
		StartCoroutine (BlinkStar ());
	}

	private IEnumerator AnimateStar()
	{
		while (true)
		{
			transform.Rotate (Vector3.forward * 2f);

			yield return 0;
		}
	}

	private IEnumerator BlinkStar()
	{
		float i = 0;
		while (true) {
			yield return new WaitForSeconds (Random.Range (1, 5));

			i = 0;
			while (i < 1) {
				i += Time.deltaTime;
				transform.localScale = Vector3.Lerp (Vector3.zero, Vector3.one, i);

				yield return 0;
			}

			i = 0;
			while (i < 1) {
				i += Time.deltaTime;
				transform.localScale = Vector3.Lerp (Vector3.one, Vector3.zero, i);

				yield return 0;
			}
		}
	}
}