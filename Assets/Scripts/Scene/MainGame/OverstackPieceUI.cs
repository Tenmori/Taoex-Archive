using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverstackPieceUI : MonoBehaviour
{
    /// <summary>
    /// Starting position of this overstack piece.
    /// </summary>
    private Vector3 startPos;
        
    /// <summary>
    /// Whether this piece is considered.
    /// </summary>
    private bool considered;

    /// <summary>
    /// Reference to the overstack interface.
    /// </summary>
    private OverstackUI overstackUI;

    /// <summary>
    /// Reference to the piece.
    /// </summary>
    private PieceData piece;

    /// <summary>
    /// WHether this piece is flipped.
    /// </summary>
    private bool flipped;

    /// <summary>
    /// Accessor for startPos.
    /// </summary>
    public Vector3 StartPos { get { return startPos; } }

    /// <summary>
    /// Accessor for flipped.
    /// </summary>
    public bool Flipped { get { return flipped; } set { flipped = value; } }

    /// <summary>
    /// Accessor for piece.
    /// </summary>
    public PieceData Piece { get { return piece; } set { piece = value; } }

    /// <summary>
    /// Accessor for considered.
    /// </summary>
    public bool Considered { get { return considered; } set { considered = value; } }

    private void Start()
    {
        // Reference to the overstack gameobject
        overstackUI = GameObject.Find("UICanvas").transform.Find("Overstack").gameObject.GetComponent<OverstackUI>();

        // Starting position on the non-considered stack
        startPos = transform.localPosition;

        // Piece visuals
        gameObject.AddComponent<MeshCollider>();
        gameObject.GetComponent<MeshCollider>().convex = true;
        //meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!Input.GetMouseButton(1))
        {
            // Rotate the piece on the y axis
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
                transform.localEulerAngles.y + Time.deltaTime * 10f, transform.localEulerAngles.z);
        }
    }

    /// <summary>
    /// On-click effect of the piece.
    /// </summary>
    public void OnMouseUpAsButton()
    {
        // Check if this piece is in the considered stack
        if (!considered)
        {
            // Consider the piece
            overstackUI.ConsiderPiece(this);
        }
        else
        {
            // Remove the piece from the consideration stack
            overstackUI.DeconsiderPiece(this);

            // Return to the starting position;
            transform.localPosition = startPos;
        }
    }
}
