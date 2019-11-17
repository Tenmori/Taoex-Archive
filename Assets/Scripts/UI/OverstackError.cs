using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverstackError : MonoBehaviour
{
    /// <summary>
    /// The fade duration in seconds.
    /// </summary>
    public float fadeDuration = 2f;

    /// <summary>
    /// How long to wait before starting the fade in seconds.
    /// </summary>
    public float fadeDelay = 1f;

    /// <summary>
    /// Initialization.
    /// </summary>
    private void Start()
    {
        // Hide this text
        Color oldColor = gameObject.GetComponent<Text>().color;
        gameObject.GetComponent<Text>().color = new Color(oldColor.r, oldColor.g, oldColor.b, 0f);
    }

    /// <summary>
    /// Shows an error message to the screen for invalid overstack operations.
    /// </summary>
    /// <param name="errorText">The error text to show</param>
    public void ShowError(string errorText)
    {
        // Set the text
        gameObject.GetComponent<Text>().text = errorText;

        // Show the text
        Color oldColor = gameObject.GetComponent<Text>().color;
        gameObject.GetComponent<Text>().color = new Color(oldColor.r, oldColor.g, oldColor.b, 1f);

        // Fade away
        StartCoroutine(Fade());
    }

    /// <summary>
    /// Fades the text away.
    /// </summary>
    public IEnumerator Fade()
    {
        yield return new WaitForSeconds(fadeDelay);

        while (gameObject.GetComponent<Text>().color.a > 0.0f)
        {
            Color oldColor = gameObject.GetComponent<Text>().color;
            gameObject.GetComponent<Text>().color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a - (Time.deltaTime / fadeDuration));
            yield return null;
        }
    }
}
