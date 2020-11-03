namespace Tetris.Engine.AI.Algorithms.Weights
{
    public class TsitsiklisWeights
    {
        public TsitsiklisWeights()
        {
        }

        public TsitsiklisWeights(int height, int holes)
        {
            this.Height = height;
            this.Holes = holes;
        }

        public int Height { get; set; }
        public int Holes { get; set; }
    }
}
