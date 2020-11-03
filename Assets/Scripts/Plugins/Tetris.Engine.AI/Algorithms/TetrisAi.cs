namespace Tetris.Engine.AI.Algorithms
{
    using Weights;

    public class TetrisAi : IAlgorithm
    {
        private readonly float landingHeight;
        private readonly float rowTransitions;
        private readonly float columnTransitions;
        private readonly float numberOfHoles;
        private readonly float wellSums;
        private readonly float rowsCleared;

        //////////////////////////////////////////////////////////////////////////
        public TetrisAi(TetrisAiWeights tetrisAiWeights)
        {
            landingHeight = tetrisAiWeights.LandingHeight;
            rowTransitions = tetrisAiWeights.RowTransitions;
            columnTransitions = tetrisAiWeights.ColumnTransitions;
            rowsCleared = tetrisAiWeights.RowsCleared;
            numberOfHoles = tetrisAiWeights.NumberOfHoles;
            wellSums = tetrisAiWeights.WellSums;
        }

        public float CalculateFitness(bool[][] gameBoard, int height, int rowsCleared)
        {
            var features = new Features(gameBoard);
            var fitness = 0f;

            fitness += rowsCleared * this.rowsCleared;
            fitness += features.ColumnTransitions() * columnTransitions;
            fitness += features.LandingHeight(height) * landingHeight;
            fitness += features.NumberOfHoles() * numberOfHoles;
            fitness += features.RowTransitions() * rowTransitions;
            fitness += features.WellSums() * wellSums;

            return fitness;
        }
    }
}
