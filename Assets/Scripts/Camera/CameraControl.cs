using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Speed for keyboard camera movements
    private static float panSpeed = 1000f;

    // Speed for mouse camera movements
    private static float moveSpeed = 200f;

    // Speed for scrolling
    private static float scrollSpeed = 100000f;

    // Borders
    private float minY;
    private float maxY;
    private Vector2 panLimit;

    // Rotations
    private float rotX;

    // Mouse movements
    private float moveX;
    private float moveY;
    private float diffX;
    private float diffY;

    // Focus position
    private Vector3 focusPos;

    void Start()
    {
        rotX = 90f;

        // If these variables aren't declared, set them to some default values
        if (minY == 0f)
        {
            minY = 100f;
        }
        if (maxY == 0f)
        {
            maxY = 1400f;
        }
        if (panLimit.x == 0f)
        {
            panLimit.x = 2400f;
        }
        if (panLimit.y == 0f)
        {
            panLimit.y = 2400f;
        }

        focusPos = GameObject.Find("Compass").transform.position;
    }

    void Update()
    {

        // Current position of the camera
        Vector3 pos = transform.position;
        rotX = transform.eulerAngles.x;

        // Set the camera to move as if it were 90 degrees
        transform.eulerAngles = new Vector3(90f, transform.eulerAngles.y, transform.eulerAngles.z);

        // Get the local space so this works if the camera is rotated
        pos = transform.InverseTransformPoint(pos);

        // Keyboard camera movements
        if (Input.GetKey("w"))
        {
            pos.y += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s"))
        {
            pos.y -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // Zooming
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.z += scroll * scrollSpeed * Time.deltaTime;

        // Turn it back to world space
        pos = transform.TransformPoint(pos);

        // Get mouse position
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            moveX = Input.mousePosition.x;
            moveY = Input.mousePosition.y;
        }

        // Get mouse differences on either click
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
        {
            // Calculate the differences
            diffX = moveX - Input.mousePosition.x;
            diffY = moveY - Input.mousePosition.y;

            // Update the current mouse position
            moveX = Input.mousePosition.x;
            moveY = Input.mousePosition.y;
        }

        // Left click for camera movement
        if (Input.GetKey(KeyCode.Mouse0))
        {
            // Get the local space so this works if the camera is rotated
            pos = transform.InverseTransformPoint(pos);
            focusPos = transform.InverseTransformPoint(focusPos);

            // Move the camera
            pos.x += diffX * Time.deltaTime * moveSpeed;
            pos.y += diffY * Time.deltaTime * moveSpeed;

            // Move the focus point
            focusPos.x += diffX * Time.deltaTime * moveSpeed;
            focusPos.y += diffY * Time.deltaTime * moveSpeed;

            // Turn it back to world space
            pos = transform.TransformPoint(pos);
            focusPos = transform.TransformPoint(focusPos);
        }

        // Set the camera angle back
        transform.eulerAngles = new Vector3(rotX, transform.eulerAngles.y, transform.eulerAngles.z);

        // Right click for rotations
        if (Input.GetKey(KeyCode.Mouse1))
        {
            // Calculate x-z distnace from the focus point
            float distanceFromFocus = Vector2.Distance(new Vector2(pos.x, pos.z),
                new Vector2(focusPos.x, focusPos.z));

            // Get the local space so this works if the camera is rotated
            pos = transform.InverseTransformPoint(pos);

            // Left and right camera movement, speed adjusted by distance from focus
            pos.x += diffX * Time.deltaTime * moveSpeed * distanceFromFocus / 750f;

            // Only move up or down if the camera is not at capped angles
            if (rotX > 5f && rotX < 70f)
            {
                pos.y += diffY * Time.deltaTime * moveSpeed;
            }

            // Turn it back to world space
            pos = transform.TransformPoint(pos);

            // Move camera up and down in world space to prevent camera from getting stuck
            pos.y += diffY * Time.deltaTime * moveSpeed;
        }

        // Camera boundaries
        pos.x = Mathf.Clamp(pos.x, -400, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -400, panLimit.y);

        // Checking if the focus position was clamped
        Vector3 before = focusPos;

        focusPos.x = Mathf.Clamp(focusPos.x, -400, panLimit.x);
        focusPos.z = Mathf.Clamp(focusPos.z, -400, panLimit.y);

        Vector3 difference = focusPos - before;

        // If the focus position wasn't clamped, then move the camera
        if (difference.magnitude < 1f)
        {
            // Set new camera position
            transform.position = pos;
        }

        // Camera looks at the focus position
        transform.LookAt(focusPos, Vector3.up);

        // Clamp the angles
        transform.eulerAngles = new Vector3(Mathf.Clamp(transform.eulerAngles.x, 5f, 70f)
            , transform.eulerAngles.y, 0);

        Debug.DrawLine(transform.position, focusPos, Color.red);
    }

    /**
     * Setter for the position of the camera.
     * @param newPosition the position to set to
     */
    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    
    /**
     * Moves the camera in the specified direction by the specified distance.
     * @param direction the direction to move
     * @param distance the distance to move
     */
    public void Move(Vector3 direction, float distance)
    {
        transform.position += direction * distance;
    }

    /**
     * Setter for the position of the focus.
     * @param newFocusPos the focus position to set to
     */
    public void SetFocus(Vector3 newFocusPos)
    {
        focusPos = newFocusPos;
    }
}
