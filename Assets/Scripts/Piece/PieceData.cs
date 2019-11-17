using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PieceData : IComparable<PieceData> {

    public enum Type { Normal, Hook };

    public int direction, range;
    public int value;
    public readonly Player.PlayerColour colour;
    public Type type;

    private int normalDir, altDir, normalRan, altRan;
    public bool flipped;
    private GameObject obj;

    /// <summary>
    /// The sorting value.
    /// </summary>
    public int sortingValue;

    /// <summary>
    /// Accessor for normalDir.
    /// </summary>
    public int NormalDir { get { return normalDir; } }

    /// <summary>
    /// Constructor for a normal pieceData
    /// </summary>
    /// <param name="player">The Player who is the original owner of the piece</param>
    /// <param name="normalDir">Normal direction that this peice can go</param>
    /// <param name="normalRan">Normal range that this piece can go</param>
    /// <param name="altDir">The alternative direction when this piece is captured</param>
    /// <param name="altRan">The alternative range when this piece is captured</param>
    /// <param name="obj">Game object associated with this pieceData</param>
    public PieceData(Player player, int normalDir, int normalRan, int altDir, int altRan, GameObject obj) {
        Debug.Assert(normalDir < 6 && normalDir >= 0, "PieceData's normal direction must be between 0 and 6");
        Debug.Assert(normalRan > 0, "PieceData's normal range must be greater than 0");
        Debug.Assert(altDir < 6 && altDir >= 0, "PieceData's alt direction must be between 0 and 6");
        Debug.Assert(altRan > 0, "PieceData's alt range must be greater than 0");
        Debug.Assert(obj != null, "PieceData's obj is null on creation");
        this.normalDir = normalDir;
        this.normalRan = normalRan;
        this.altDir = altDir;
        this.altRan = altRan;
        this.obj = obj;
        colour = player.colour;
        value = 1;
        type = Type.Normal;
        direction = normalDir;
        range = normalRan;

        obj.transform.SetParent(GameObject.Find("Pieces").transform);

        // Move to center of the board
        obj.transform.position = new Vector3(975, 1000, 925);
    }

    /// <summary>
    /// Constructor for a normal pieceData
    /// </summary>
    /// <param name="colour">The colour of the piece</param>
    /// <param name="normalDir">Normal direction that this peice can go</param>
    /// <param name="normalRan">Normal range that this piece can go</param>
    /// <param name="altDir">The alternative direction when this piece is captured</param>
    /// <param name="altRan">The alternative range when this piece is captured</param>
    /// <param name="obj">Game object associated with this pieceData</param>
    public PieceData(Player.PlayerColour colour, int normalDir, int normalRan, int altDir, int altRan, GameObject obj) {
        Debug.Assert(normalDir < 6 && normalDir >= 0, "PieceData's normal direction must be between 0 and 6");
        Debug.Assert(normalRan > 0, "PieceData's normal range must be greater than 0");
        Debug.Assert(altDir < 6 && altDir >= 0, "PieceData's alt direction must be between 0 and 6");
        Debug.Assert(altRan > 0, "PieceData's alt range must be greater than 0");
        Debug.Assert(obj != null, "PieceData's obj is null on creation");
        this.normalDir = normalDir;
        this.normalRan = normalRan;
        this.altDir = altDir;
        this.altRan = altRan;
        this.colour = colour;
        this.obj = obj;
        value = 1;
        type = Type.Normal;
        direction = normalDir;
        range = normalRan;

        obj.transform.SetParent(GameObject.Find("Pieces").transform);

        // Move to center of the board
        obj.transform.position = new Vector3(975, 1000, 925);
    }

    /// <summary>
    /// applies the correct textures, should only call this once
    /// </summary>
    public void SetupTextures() {
        string textureBase = "Images/Texture/Piece/" + colour.ToString().ToLower() + "-";
        Vector2 textureScale = new Vector2(6f, 10f);
        MeshRenderer meshRender = obj.GetComponent<MeshRenderer>();

        // normal
        Material matNormColour = meshRender.materials[0];
        matNormColour.mainTexture = Resources.Load<Texture>(textureBase + HexDir.GetShortName(normalDir) + normalRan);
        matNormColour.mainTextureScale = textureScale;

        // alt
        Material matAltColour = meshRender.materials[1];
        if (type != Type.Hook) {
            matAltColour.mainTexture = Resources.Load<Texture>(textureBase + HexDir.GetShortName(normalDir) + normalRan + "f");
        } else {
            matAltColour.mainTexture = Resources.Load<Texture>(textureBase + HexDir.GetShortName(normalDir) + normalRan);
        }
        matAltColour.mainTextureScale = textureScale;

        // piece colour
        meshRender.materials[2].color = GetMaterialColor();
    }

    private Color GetMaterialColor() {
        return Player.ConvertToColor32(colour);
    }

    /// <summary>
    /// Flips the piece over when captured
    /// </summary>
    /// <param name="flip"></param>
    public void SetFlipped(bool flip) {
        if (type != Type.Hook) {
            flipped = flip;

            if (flipped) {
                obj.transform.eulerAngles = new Vector3(-270f, 90f, 0f);
                direction = altDir;
                range = altRan;
                //obj.transform.Rotate(new Vector3(180f, 60f, 0f));
            } else {
                obj.transform.eulerAngles = new Vector3(-90f, 30f, 0f);
                direction = normalDir;
                range = normalRan;
                //obj.transform.Rotate(new Vector3(-180f, -60f, 0f));
            }
        }
    }

    /// <summary>
    /// Moves the gameobject to the location
    /// </summary>
    /// <param name="location"></param>
    public void MoveTo(Vector3 location) {
        if (flipped) {
            location.y += 50;
        }

        // Smooth movement
        obj.GetComponent<SmoothMovementQueue>().ClearQueue();
        obj.GetComponent<SmoothMovementQueue>().ImmediateMove(location);
        
        //obj.transform.position = location;
    }

    /// <summary>
    /// Moves to multiple positions, one at the time.
    /// </summary>
    /// <param name="locations">Locations.</param>
    public void MultiMoveTo(params Vector3[] locations) {
        obj.GetComponent<SmoothMovementQueue>().ClearQueue();
        foreach (Vector3 v in locations) {
            obj.GetComponent<SmoothMovementQueue>().QueueMove(v);
        }
    }

    public GameObject getObj()
    {
        return obj;
    }

    public int CompareTo(PieceData comp) {
        return sortingValue - comp.sortingValue;
    }

    public bool isHook()
    {
        if (type.Equals(Type.Hook))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
