using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SlideOnHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    /// <summary>
    /// factor to convert degrees to radians
    /// </summary>
    private static readonly float DEG_TO_RAD = Mathf.PI / 180f;

    /// <summary>
    /// The direction is in degrees.
    /// </summary>
    public bool directionInDegrees = true;

    /// <summary>
    /// Direction of sliding in.
    /// right (along +x) represents 0 degrees
    /// </summary>
    public float direction;

    /// <summary>
    /// The slide amount.
    /// </summary>
    public float slideAmount;

    /// <summary>
    /// The slide speed.
    /// </summary>
    [Range(0f, 1f)]
    public float slideSpeed = 0.05f;

    /// <summary>
    /// The vector for slide direction
    /// </summary>
    private Vector3 vecDir;

    /// <summary>
    /// The vec move.
    /// </summary>
    private Vector3 vecMove;

    /// <summary>
    /// Flag for when mouse is over the game object
    /// </summary>
    private bool hover;

    /// <summary>
    /// The start position.
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// The out position.
    /// </summary>
    private Vector3 outPosition;

    /// <summary>
    /// The last recorded Resolution.
    /// </summary>
    private Resolution res;

    private RectTransform trans;

	/// <summary>
    /// Start this instance.
    /// </summary>
	void Start() {
        // calculate direction of vector
        if (directionInDegrees) {
            direction = direction * DEG_TO_RAD;
        }

        vecDir = new Vector3(Mathf.Cos(direction), Mathf.Sin(direction));

        trans = GetComponent<RectTransform>();

        // calculate start and out positions
        startPosition = clone(trans.anchoredPosition3D);

        outPosition = clone(startPosition);
        Vector3 translate = clone(vecDir);
        translate *= slideAmount;
        outPosition += translate;

        vecMove = translate;

        res = Screen.currentResolution;
	}
	
    /// <summary>
    /// Fixed duration between updates
    /// </summary>
    void FixedUpdate() {
        #region slide update
        // slide in
        if (hover) {
            if (!equal(trans.anchoredPosition3D, startPosition)) {
                Vector3 move = clone(vecMove);
                move.Scale(new Vector3(slideSpeed, slideSpeed, slideSpeed));
                move *= -1f;
                trans.anchoredPosition3D += move;
            }
        }

        // slide out
        else {
            if (!equal(trans.anchoredPosition3D, outPosition)) {
                Vector3 move = clone(vecMove);
                move.Scale(new Vector3(slideSpeed, slideSpeed, slideSpeed));
                trans.anchoredPosition3D += move;
            }
        }
        #endregion
    }

    /// <summary>
    /// Raises the pointer enter event.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public void OnPointerEnter(PointerEventData eventData) {
        hover = true;
    }

    /// <summary>
    /// Raises the pointer exit event.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public void OnPointerExit(PointerEventData eventData) {
        hover = false;
    }

    /// <summary>
    /// Clone the specified v.
    /// </summary>
    /// <param name="v">V.</param>
    private Vector3 clone(Vector3 v) {
        return new Vector3(v.x, v.y, v.z);
    }

    /// <summary>
    /// Equal the specified a and b.
    /// </summary>
    /// <param name="a">first vector</param>
    /// <param name="b">second vector</param>
    private bool equal(Vector3 a, Vector3 b) {
        float epsilson = slideAmount * slideSpeed;
        Vector3 c = a - b;
        c.x = Mathf.Abs(c.x);
        c.y = Mathf.Abs(c.y);
        c.z = Mathf.Abs(c.z);

        return c.x < epsilson && c.y < epsilson && c.z < epsilson;
    }
}
