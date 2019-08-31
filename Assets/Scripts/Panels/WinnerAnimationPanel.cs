using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinnerAnimationPanel : MonoBehaviour
{
    public Text txtWinner;

    // Use this for initialization
    void OnEnable()
    {
        txtWinner.text = "You Win!";

        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        while (true)
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            txtWinner.color = new Color(r, g, b, 1);

            yield return new WaitForSeconds(.2f) ;
        }
    }
}