using System;

namespace Tetris.Engine.AI.Algorithms.Weights
{
    [Serializable]
    public class TetrisAiWeights
    {
        public float LandingHeight;
        public float RowTransitions;
        public float ColumnTransitions;
        public float NumberOfHoles;
        public float WellSums;
        public float RowsCleared;
    }
}
