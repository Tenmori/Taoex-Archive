using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandboxHandler : MonoBehaviour {

    public enum Mode {
        Add,
        Edit,
        Delete,
        None
    }

    private static Color defaultBtnColour = Color.white;
    private static Color selectBtnColour = new Color32(255, 190, 128, 255);

    private int[] dirWheel = { 0, 0, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1 };
    private int[,] dirWheelOffset = { { 1, 9 }, { 11, 7 }, { 9, 1 }, { 7, 3 }, { 5, 1 }, { 3, 11 } };

    /// <summary>
    /// Main containing panel for the UI
    /// </summary>
    private GameObject panel;

    /// <summary>
    /// if the sandbox menu is enabled
    /// </summary>
    public bool sandBoxEnabled;

    /// <summary>
    /// Current sandbox mode
    /// </summary>
    private Mode currentMode;

    /// <summary>
    /// The selected direction.
    /// </summary>
    private int selectedDirection;

    /// <summary>
    /// The selected range.
    /// </summary>
    private int selectedRange;

    /// <summary>
    /// The selected colour.
    /// </summary>
    private Player.PlayerColour selectedColour;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start() {
        currentMode = Mode.None;
        selectedRange = 2;
        panel = GameObject.Find("SandboxPanel") as GameObject;
        panel.SetActive(sandBoxEnabled);
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            sandBoxEnabled = !sandBoxEnabled;
            panel.SetActive(sandBoxEnabled);
            Debug.Log("sandbox mode= " + sandBoxEnabled);
        }
    }

    /// <summary>
    /// Changes the mode which converts the int into the enum representing 
    /// </summary>
    /// <param name="mode">Mode.</param>
    public void ChangeMode(int mode) {
        if (currentMode == (Mode)mode) {
            currentMode = Mode.None;
        } else {
            currentMode = (Mode)mode;
        }

        switch (currentMode) {
            case Mode.Add:
                GameObject.Find("AddBtn").GetComponent<Image>().color = selectBtnColour;
                GameObject.Find("DeleteBtn").GetComponent<Image>().color = defaultBtnColour;
                break;

            case Mode.Delete:
                GameObject.Find("AddBtn").GetComponent<Image>().color = defaultBtnColour;
                GameObject.Find("DeleteBtn").GetComponent<Image>().color = selectBtnColour;
                break;

            case Mode.None:
                GameObject.Find("AddBtn").GetComponent<Image>().color = defaultBtnColour;
                GameObject.Find("DeleteBtn").GetComponent<Image>().color = defaultBtnColour;
                break;
        }

        Debug.Log("Changed sandbox mode= " + currentMode);
    }

    /// <summary>
    /// Changes the colour which converts the int into the enum for representing player colour
    /// </summary>
    /// <param name="colour">Colour.</param>
    public void ChangeColour(int colour) {
        selectedColour = (Player.PlayerColour)colour;
        Debug.Log("Sandbox Colour= " + selectedColour);
    }

    /// <summary>
    /// Changes the range.
    /// </summary>
    /// <param name="range">Range.</param>
    public void ChangeRange(int range) {
        selectedRange = range + 2;
        Debug.Log("Sandbox Range= " + selectedRange);
    }

    /// <summary>
    /// Changes the direction.
    /// </summary>
    /// <param name="dir">Dir.</param>
    public void ChangeDirection(int dir) {
        selectedDirection = dir;
        Debug.Log("Sandbox dir= " + selectedDirection);
    }

    /// <summary>
    /// Selects the tile.
    /// </summary>
    /// <param name="node">Node.</param>
    public void SelectTile(TileNode node) {
        switch (currentMode) {
            case Mode.Add:

                // create new tower
                if (node.tower == null) {
                    CreateTower(selectedColour, selectedDirection, selectedRange, node);
                } else if (node.tower.pieces.Count < 6) {
                    // special case for hook
                    if (selectedColour == Player.PlayerColour.Hook) {

                        // one hook per tower
                        if (node.tower.GetHook() == null) {
                            PieceData piece = new HookPieceData(selectedDirection, Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/GameObject/HexagonPiece")));
                            piece.SetupTextures();
                            node.tower.AddPiece(piece);
                            node.tower.updatePosition();
                            node.tower.owningPlayer.updateScore();
                        }
                    
                    } else { // non hooks
                        PieceData piece = CreatePiece(selectedColour, selectedDirection, selectedRange);
                        piece.SetupTextures();
                        node.tower.AddPiece(piece);
                        node.tower.updatePosition();
                        node.tower.owningPlayer.updateScore();
                    }
                }
                break;

            case Mode.Delete:
                if (node.tower != null) {

                    // unhighlight
                    foreach (TileNode tn in node.tower.GetMoves().Destinations) {
                        tn.unhighlight();
                    }

                    foreach (PieceData pd in node.tower.pieces) {
                        Destroy(pd.getObj());
                    }

                    Player p = node.tower.owningPlayer;

                    node.tower.Die();

                    p.updateScore();
                }
                break;

            case Mode.Edit:
                break;
        }
    }

    /// <summary>
    /// Creates a piece.
    /// </summary>
    /// <returns>The piece.</returns>
    /// <param name="colour">Colour.</param>
    /// <param name="dir">Dir.</param>
    /// <param name="range">Range.</param>
    private PieceData CreatePiece(Player.PlayerColour colour, int dir, int range) {
        // only 6 directions
        Debug.Assert(0 <= dir && dir <= 5);

        // max range of 3
        Debug.Assert(1 <= range && range <= 3);

        // only hooks have range of 1
        if (colour == Player.PlayerColour.Hook) {
            Debug.Assert(range == 1);
        } else {
            Debug.Assert(range != 1);
        }

        // normal and flipped/alternate
        int normRan = range;
        int normDir = dir;
        int altRan = normRan == 2 ? 3 : 2;
        int wheelIndex = normDir * 2;

        if (normRan == 3) {
            wheelIndex++;
        }

        // array "wheel" dir offset
        wheelIndex += dirWheelOffset[(int)colour, normRan == 2 ? 0 : 1];
        wheelIndex = wheelIndex % 12;

        int altDir = dirWheel[wheelIndex];

        if (altDir == 1 || altDir == 4) {
            altDir = (altDir + 4) % 6;
        } else if (altDir == 5 || altDir == 2){
            altDir = (altDir + 2) % 6;
        }

        // create piece
        PieceData piece = new PieceData(colour, normDir, normRan, altDir, altRan, Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/GameObject/HexagonPiece")));
    
        return piece;
    }

    /// <summary>
    /// Creates the tower at the given location.
    /// </summary>
    /// <param name="colour">Colour of the piece</param>
    /// <param name="dir">Direction based on hexagon (0 to 5)</param>
    /// <param name="range">Range of the piece (1 for hook, otherwise 2 or 3)</param>
    /// <param name="node">TileNode location.</param>
    private void CreateTower(Player.PlayerColour colour, int dir, int range, TileNode node) {
        
        // node must be clear of tower
        Debug.Assert(node.tower == null);

        Player player = GetComponentInParent<TurnHandler>().getPlayerByColour(colour);

        // no player found with colour
        if (player == null) {
            return;
        }

        // create piece
        PieceData piece = CreatePiece(colour, dir, range);

        // create tower at node location owned by player
        PieceTower tower = new PieceTower(player, piece, node);

        // add to player
        player.AddTower(tower);
    }
}
