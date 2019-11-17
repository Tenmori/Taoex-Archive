using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaoexBoard : MonoBehaviour {

    private static readonly int boardX = 27;
    private static readonly int boardZ = 22;

    private static int tilePerSec = 500;
    private static float secPerTile = 1f / (float)tilePerSec;

    // loading flag
    bool doneSpecialTiles;
    bool doneNormalTiles;

    // for overall board shape (edges)
    int[] shapeBottom = { 9, 8, 6, 5, 3, 2, 1, 1 };
    int[] shapeTop = { 13, 15, 16, 18, 19, 21, 21, 22 };

    // locations for block tiles
    int[,] blockPos = { { 7, 20 }, { 19, 20 }, { 25, 11 }, { 19, 2 }, { 7, 2 }, { 1, 11 } };

    // x position of edge/starting tiles
    int[] edgePos = { 0, 1, 2, 3, 4, 5, 8, 10, 12, 14, 16, 18, 21, 22, 23, 24, 25, 26 };
    public TileNode[,] edgeTiles;

    /// <summary>
    /// Array of way cross tiles.
    /// </summary>
    private TileNode[] wayCrossTiles;

    /// <summary>
    /// Reference to the player used for hook pieces.
    /// </summary>
    private Player hookPlayer;

    TileNode[,] tiles;
    GameObject baseTileObj, blockTileObj, wayTileObj, edgeTileObj, baseHookObj;

    /// <summary>
    /// Accessor for wayCrossTiles.
    /// </summary>
    public TileNode[] WayCrossTiles { get { return wayCrossTiles; } }

    /// <summary>
    /// Accessor for hookPlayer.
    /// </summary>
    public Player HookPlayer { get { return hookPlayer; } }

    // Use this for initialization
    void Start() {

        baseTileObj = Resources.Load("Prefabs/Tile/NormalTile") as GameObject;
        blockTileObj = Resources.Load("Prefabs/Tile/BlockTile") as GameObject;
        wayTileObj = Resources.Load("Prefabs/Tile/WayTile") as GameObject;
        edgeTileObj = Resources.Load("Prefabs/Tile/EdgeTile") as GameObject;
        baseHookObj = Resources.Load("Prefabs/GameObject/HexagonPiece") as GameObject;

        StartCoroutine(GenerateBoard());
    }

    /// <summary>
    ///  Creates the board with special tiles 
    /// </summary>
    private IEnumerator GenerateBoard() {
        // 29 x 21, 
        // center = (15, 12)
        tiles = new TileNode[boardX, boardZ];
        edgeTiles = new TileNode[6, 6];

        // Initialize way cross tiles array
        wayCrossTiles = new TileNode[6];

        // create special tiles
        //StartCoroutine(CreateSpecialTile());
        CreateSpecialTile();

        yield return new WaitUntil(() => doneSpecialTiles == true);

        // fills in the remaining tiles with normal tiles
        //StartCoroutine(fillBoardShape());
        fillBoardShape();

        yield return new WaitUntil(() => doneNormalTiles == true);

        // link edge tiles
        linkEdgeTiles();

        // unload assets
        baseTileObj = null;
        blockTileObj = null;
        wayTileObj = null;
        edgeTileObj = null;
        baseHookObj = null;
        Resources.UnloadUnusedAssets();

        GetComponent<TurnHandler>().Initalize();

        TileClick.clickable = true;
    }

    /// <summary>
    /// Creates special tiles nodes for the taoex board
    /// </summary>
    void CreateSpecialTile() {
        int hookDir = new System.Random().Next(6);
        hookPlayer = new LocalHumanPlayer();
        hookPlayer.colour = Player.PlayerColour.Hook;

        // block tiles and way tiles
        for (int i = 0; i < 6; i++) {
            int x = blockPos[i, 0];
            int z = blockPos[i, 1];

            // block tile
            GameObject tileObject = Instantiate(blockTileObj);
            tiles[x, z] = tileObject.GetComponent<TileNode>();
            tiles[x, z].Init(x, z, TileNode.Type.Block);

            // disable block tile
            tiles[x, z].gameObject.SetActive(false);

            #region generate way tiles
            for (int wayNum = 1; wayNum <= 17; wayNum++) {
                //yield return new WaitForSeconds(secPerTile);
                if (x % 2 == 0) {
                    x += HexDir.even[(i + 2) % 6].x;
                    z += HexDir.even[(i + 2) % 6].z;
                } else {
                    x += HexDir.odd[(i + 2) % 6].x;
                    z += HexDir.odd[(i + 2) % 6].z;
                }

                if (tiles[x, z] == null && wayNum != 12) {
                    if (wayNum == 6) { // way cross location
                        //tiles[x, z] = new TileNode(x, z, Instantiate(wayTileObj), TileNode.Type.WayCross);
                        GameObject wayTileObject = Instantiate(wayTileObj);
                        tiles[x, z] = wayTileObject.GetComponent<TileNode>();
                        tiles[x, z].Init(x, z, TileNode.Type.WayCross);

                        wayCrossTiles[i] = tiles[x, z];

                        PieceData hook = new HookPieceData(hookDir, Instantiate(baseHookObj));
                        new PieceTower(hookPlayer, hook, tiles[x, z]);
                        hookDir = (hookDir + 1) % 6;

                    } else {
                        //tiles[x, z] = new TileNode(x, z, Instantiate(wayTileObj), TileNode.Type.Way);
                        GameObject wayTileObject = Instantiate(wayTileObj);
                        tiles[x, z] = wayTileObject.GetComponent<TileNode>();
                        tiles[x, z].Init(x, z, TileNode.Type.Way);

                    }

                    // set wayline ID
                    tiles[x, z].wayLine = i;
                }
            }
            #endregion
        }

        // edge tiles
        #region generate edge tiles
        for (int i = 0; i < edgePos.Length; i++) {
            int x = edgePos[i];

            if (i < 6) {
//                TileNode topNode = new TileNode(x, shapeTop[i] - 1, Instantiate(edgeTileObj), TileNode.Type.Edge);
//                tiles[x, shapeTop[i] - 1] = topNode;
//                topNode.obj.GetComponent<TileNodeHolder>().node = topNode;
//                edgeTiles[5, i] = topNode;
//                topNode.edgeId = 5;

                int z = shapeTop[i] - 1;
                GameObject topTileObject = Instantiate(edgeTileObj);
                tiles[x, z] = topTileObject.GetComponent<TileNode>();
                tiles[x, z].Init(x, z, TileNode.Type.Edge);
                edgeTiles[5, i] = tiles[x, z];
                tiles[x, z].edgeId = 5;

//                TileNode bottomNode = new TileNode(x, shapeBottom[i], Instantiate(edgeTileObj), TileNode.Type.Edge);
//                tiles[x, shapeBottom[i]] = bottomNode;
//                bottomNode.obj.GetComponent<TileNodeHolder>().node = bottomNode;
//                edgeTiles[4, 5 - i] = bottomNode;
//                bottomNode.edgeId = 4;

                z = shapeBottom[i];
                GameObject bottomTileObject = Instantiate(edgeTileObj);
                tiles[x, z] = bottomTileObject.GetComponent<TileNode>();
                tiles[x, z].Init(x, z, TileNode.Type.Edge);
                edgeTiles[4, 5 - i] = tiles[x, z];
                tiles[x, z].edgeId = 4;

            } else if (i > 11) {
//                TileNode topNode = new TileNode(x, shapeTop[edgePos.Length - i - 1] - 1, Instantiate(edgeTileObj), TileNode.Type.Edge);
//                tiles[x, shapeTop[edgePos.Length - i - 1] - 1] = topNode;
//                topNode.obj.GetComponent<TileNodeHolder>().node = topNode;
//                edgeTiles[1, i - 12] = topNode;
//                topNode.edgeId = 1;

                int z = shapeTop[edgePos.Length - i - 1] - 1;
                GameObject topTileObject = Instantiate(edgeTileObj);
                tiles[x, z] = topTileObject.GetComponent<TileNode>();
                tiles[x, z].Init(x, z, TileNode.Type.Edge);
                edgeTiles[1, i - 12] = tiles[x, z];
                tiles[x, z].edgeId = 1;

//                TileNode bottomNode = new TileNode(x, shapeBottom[edgePos.Length - i - 1], Instantiate(edgeTileObj), TileNode.Type.Edge);
//                tiles[x, shapeBottom[edgePos.Length - i - 1]] = bottomNode;
//                bottomNode.obj.GetComponent<TileNodeHolder>().node = bottomNode;
//                edgeTiles[2, 5 - (i - 12)] = bottomNode;
//                bottomNode.edgeId = 2;

                z = shapeBottom[edgePos.Length - i - 1];
                GameObject bottomTileObject = Instantiate(edgeTileObj);
                tiles[x, z] = bottomTileObject.GetComponent<TileNode>();
                tiles[x, z].Init(x, z, TileNode.Type.Edge);
                edgeTiles[2, 5 - (i - 12)] = tiles[x, z];
                tiles[x, z].edgeId = 2;
            } else {
//                TileNode topNode = new TileNode(x, boardZ - 1, Instantiate(edgeTileObj), TileNode.Type.Edge);
//                tiles[x, boardZ - 1] = topNode;
//                topNode.obj.GetComponent<TileNodeHolder>().node = topNode;
//                edgeTiles[0, i - 6] = topNode;
//                topNode.edgeId = 0;

                int z = boardZ - 1;
                GameObject topTileObject = Instantiate(edgeTileObj);
                tiles[x, z] = topTileObject.GetComponent<TileNode>();
                tiles[x, z].Init(x, z, TileNode.Type.Edge);
                edgeTiles[0, i - 6] = tiles[x, z];
                tiles[x, z].edgeId = 0;

//                TileNode bottomNode = new TileNode(x, 0, Instantiate(edgeTileObj), TileNode.Type.Edge);
//                tiles[x, 0] = bottomNode;
//                bottomNode.obj.GetComponent<TileNodeHolder>().node = bottomNode;
//                edgeTiles[3, 5 - (i - 6)] = bottomNode;
//                bottomNode.edgeId = 3;

                z = 0;
                GameObject bottomTileObject = Instantiate(edgeTileObj);
                tiles[x, z] = bottomTileObject.GetComponent<TileNode>();
                tiles[x, z].Init(x, z, TileNode.Type.Edge);
                edgeTiles[3, 5 - (i - 6)] = tiles[x, z];
                tiles[x, z].edgeId = 3;
            }
        }
        #endregion

        doneSpecialTiles = true;
    }

    /// <summary>
    /// Fills the board with normal tile nodes on the board based on the shape
    /// </summary>
    void fillBoardShape() {
        // creating normal tiles (fill)
        for (int x = 0; x < boardX; x++) {
            int zStart = 0;
            int zEnd = boardZ;

            // figuring out where to start on this column
            if (x < 8) {
                zStart = shapeBottom[x];
                zEnd = shapeTop[x];
            } else if (x > boardX - 8) {
                zStart = shapeBottom[boardX - x - 1];
                zEnd = shapeTop[boardX - x - 1];
            } else {
                if (x % 2 != 0) {
                    zStart++;
                }
            }

            for (int z = zStart; z < zEnd; z++) {
                // create if special tile wasn't already created
                //yield return new WaitForSeconds(secPerTile);

                if (tiles[x, z] == null) {
//                    tiles[x, z] = new TileNode(x, z, Instantiate(baseTileObj), TileNode.Type.Normal);
                    GameObject normalTileObject = Instantiate(baseTileObj);
                    tiles[x, z] = normalTileObject.GetComponent<TileNode>();
                    tiles[x, z].Init(x, z, TileNode.Type.Normal);
                }

                // link tiles if not a block
                #region tile linking
                if (tiles[x, z].type != TileNode.Type.Block) {
                    for (int adj = 0; adj < 6; adj++) {
                        TileNode adjTile;
                        int adjX, adjZ;

                        // even and odd offset
                        if (x % 2 == 0) {
                            adjX = HexDir.even[adj].x + x;
                            adjZ = HexDir.even[adj].z + z;
                        } else {
                            adjX = HexDir.odd[adj].x + x;
                            adjZ = HexDir.odd[adj].z + z;
                        }

                        // boundary check
                        if (adjX < 0 || adjX >= boardX || adjZ < 0 || adjZ >= boardZ) {
                            continue;
                        }

                        adjTile = tiles[adjX, adjZ];

                        // link both ways
                        if (adjTile != null && adjTile.type != TileNode.Type.Block) {
                            tiles[x, z].adjacent[adj] = adjTile;
                            adjTile.adjacent[(adj + 3) % 6] = tiles[x, z];
                        }
                    }
                }
                #endregion
            }
        }

        doneNormalTiles = true;
    }

    /// <summary>
    /// Links the edge tiles (wrap around) to each other
    /// </summary>
    void linkEdgeTiles() {
        for (int side = 0; side < 6; side++) {
            for (int i = 0; i < 6; i++) {
                edgeTiles[side, i].adjacent[side] = edgeTiles[(side + 3) % 6, 5 - i];
                edgeTiles[side, i].adjacent[(side + 1) % 6] = edgeTiles[(side + 5) % 6, 5 - i];
                edgeTiles[side, i].adjacent[(side + 5) % 6] = edgeTiles[(side + 1) % 6, 5 - i];
            }
        }

        //edgeTiles[0, 0].adjacent[0] = edgeTiles[3, 0];
    }

    /// <summary>
    /// Places pieces ranndomly for 3 players
    /// </summary>
    private void placePieces() {

    }

    public TileNode[,] GetTiles()
    {
        return tiles;
    }
}
