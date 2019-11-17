using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(RectTransform))]
public class SliderUpdateText : MonoBehaviour {
	
    /// <summary>
    /// The match X position.
    /// </summary>
    public bool matchXPosition;

    /// <summary>
    /// The slider.
    /// </summary>
    private Slider slider;

    /// <summary>
    /// The x increment.
    /// </summary>
    private float xIncrement;

    /// <summary>
    /// The text.
    /// </summary>
    private GameObject text;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start() {
        slider = GetComponent<Slider>();

        text = transform.Find("Text").gameObject;

        RectTransform rect = GetComponent<RectTransform>();

        xIncrement = (rect.sizeDelta.x - 20f) / (slider.maxValue - slider.minValue);
    }

	/// <summary>
    /// Update is called once per frame
    /// </summary>
	void Update () {
		
        text.GetComponent<Text>().text = "" + slider.value;

        if (matchXPosition) {
            RectTransform rect = text.GetComponent<RectTransform>();
            float xPos = xIncrement * (slider.value - slider.minValue);
            xPos += 10f;
            rect.anchoredPosition3D = new Vector3(xPos, rect.anchoredPosition3D.y);
        }
	}
}
