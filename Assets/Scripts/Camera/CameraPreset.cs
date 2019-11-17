using UnityEngine;
using UnityEngine.UI;

public class CameraPreset : MonoBehaviour {

    // The starting point
    public Vector2 startTile;

    private Button button;
    private CameraControl cameraControl;

	void Start () {

        // Reference to the button
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonOnClick);

        // Reference to the camera
        cameraControl = GameObject.Find("Main Camera").GetComponent<CameraControl>();
	}

    /**
     * Button's on click event
     */
    void ButtonOnClick()
    {
        // Reference to the board
        TaoexBoard tb = GameObject.Find("Taoex").GetComponent<TaoexBoard>();

        // Position of the camera is based on a tile on the board
        Vector3 start = tb.GetTiles()[(int)startTile.x, (int)startTile.y].gameObject.transform.position;

        // Focus of the camera is the center tile
        Vector3 end = tb.GetTiles()[13, 11].gameObject.transform.position;

        // Set the new position and focus of the camera
        cameraControl.SetPosition(start);
        cameraControl.SetFocus(end);

        // Vector from start to end
        Vector3 direction = end - start;
        direction.y = 0f;
        direction.Normalize();

        // Move the camera back
        cameraControl.Move(direction, -400f);


    }
}
