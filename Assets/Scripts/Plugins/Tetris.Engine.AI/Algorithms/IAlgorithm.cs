namespace Tetris.Engine.AI.Algorithms
{
    public interface IAlgorithm
    {
        float CalculateFitness(bool[][] gameBoard, int height, int rowsCleared);
    }
}
