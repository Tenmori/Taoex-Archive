using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for controling the required alpha in an image to register a "hit"
/// The image import settings must have:
///     1) Mesh Type = Full Rect
///     2) Read/Write = Enabled
/// </summary>
[RequireComponent(typeof(Image))]
public class AlphaHitThreshold : MonoBehaviour {

    /// <summary>
    /// The alpha threshold.
    /// </summary>
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start () {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;    
    }
}
