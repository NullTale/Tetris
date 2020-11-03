namespace Tetris.Engine.AI.Algorithms
{
    using Weights;

    public class Tsitsiklis : IAlgorithm
    {
        private readonly float m_Height;
        private readonly float m_Holes;

        //////////////////////////////////////////////////////////////////////////
        public Tsitsiklis(TsitsiklisWeights tsitsiklisWeights)
        {
            m_Height = tsitsiklisWeights.Height;
            m_Holes = tsitsiklisWeights.Holes;
        }

        public float CalculateFitness(bool[][] gameBoard, int height, int rowsCleared)
        {
            var fitness = 0f;
            var maxHeight = 0;
            for (var column = 0; column < gameBoard[0].Length; column++)
            {
                var reachedTopColumn = false;

                for (var row = gameBoard.GetLength(0) - 1; row >= 0; row--)
                {
                    var field = gameBoard[row][column];
                    if (reachedTopColumn && !field)
                    {
                        fitness += m_Holes;
                    }

                    if (field)
                    {
                        reachedTopColumn = true;

                        if (row + 1 > maxHeight)
                        {
                            maxHeight = row + 1;
                        }
                    }
                }
            }

            fitness += maxHeight * m_Height;

            return fitness;
        }
    }
}
