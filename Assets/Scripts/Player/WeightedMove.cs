public class WeightedMove
{
    public PieceTower piece;
    public TileNode dest;
    public int weight; //Not yet implimented
    public int attackValue = 0;

    public WeightedMove (PieceTower piece, TileNode dest, int weight)
    {
        this.piece = piece;
        this.dest = dest;
        this.weight = weight;
    }

    public WeightedMove (PieceTower piece, TileNode dest, int weight, int attackValue)
    {
        this.piece = piece;
        this.dest = dest;
        this.weight = weight;
        this.attackValue = attackValue;
    }
}
