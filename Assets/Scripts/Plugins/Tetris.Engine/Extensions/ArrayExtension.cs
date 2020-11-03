namespace Tetris.Engine.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class ArrayExtension
    {
        public static bool[][] Split(this bool[] array, int size)
        {
            if (array.Length % size != 0)
            {
                throw new ArgumentOutOfRangeException("array");
            }

            var matrix = new List<bool[]>();
            var length = array.Length / size;

            for (var i = 0; i < size; i++)
            {
                matrix.Add(array.Skip(i * length).Take(length).ToArray());
            }

            return matrix.ToArray();
        }

        public static bool[][] DeepClone(this bool[][] array)
        {
            var list = new List<bool[]>();
            for (var i = 0; i < array.GetLength(0); i++)
            {
                list.Add(array[i].ToArray());
            }

            return list.ToArray();
        }

        public static string MatrixToString(this bool[][] gameBoard, Block active)
        {
            var fieldChars = new StringBuilder();
            for (int row = gameBoard.GetLength(0) - 1; row >= 0; row--)
            {
                fieldChars.Append("|");
                for (var column = 0; column < gameBoard[row].Length; column++)
                {
                    if (active != null && row - active.Position.Row >= 0 && row - active.Position.Row < active.BlockMatrixSize && column - active.Position.Column >= 0 && column - active.Position.Column < active.BlockMatrixSize && active.BlockMatrix[row - active.Position.Row][column - active.Position.Column])
                    {
                        fieldChars.Append("O");
                    }
                    else
                    {
                        fieldChars.Append(gameBoard[row][column] ? "1" : " ");
                    }
                    if (column + 1 < gameBoard[row].Length)
                    {
                        fieldChars.Append(" ");
                    }

                }
                fieldChars.Append("|");
                fieldChars.AppendLine();
            }

            return fieldChars.ToString();
        }
    }
}
