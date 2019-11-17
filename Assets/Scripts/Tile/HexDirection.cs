using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Hexagon directions
/// </summary>
public class HexDir {
    /// <summary>
    /// Hexagon direction for even columns.
    /// </summary>
    public static readonly HexDifference[] even =
        { new HexDifference(0, 1, "N", "North")
        , new HexDifference(1, 1, "NE", "North-West")
        , new HexDifference(1, 0, "SE", "South-West")
        , new HexDifference(0, -1, "S", "South")
        , new HexDifference(-1, 0, "SW", "South-East")
        , new HexDifference(-1, 1, "NW", "North-East")};

    /// <summary>
    /// Hexagon direction for odd columns
    /// </summary>
    public static readonly HexDifference[] odd =
        { new HexDifference(0, 1, "N", "North")
        , new HexDifference(1, 0, "NE", "North-West")
        , new HexDifference(1, -1, "SE", "South-West")
        , new HexDifference(0, -1, "S", "South")
        , new HexDifference(-1, -1, "SW", "South-East")
        , new HexDifference(-1, 0, "SW", "North-East")};

    public static string GetShortName(int dir) {
        return even[dir].shortName;
    }
}

/// <summary>
/// Hexagon position differences class
/// </summary>
public class HexDifference {
    public readonly int x, z;
    public readonly string shortName, fullName;

    internal HexDifference(int x, int z, string shortName, string fullName) {
        this.x = x;
        this.z = z;
        this.shortName = shortName;
        this.fullName = fullName;
    }
}

