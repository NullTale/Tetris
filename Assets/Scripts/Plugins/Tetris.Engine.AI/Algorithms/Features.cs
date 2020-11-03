namespace Tetris.Engine.AI.Algorithms
{
    public class Features
    {
        private readonly bool[][] gameBoard;

        public Features(bool[][] gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        /// <summary>
        /// The total number of row transitions.
        /// A row transition occurs when an empty cell is adjacent to a filled cell
        /// on the same row and vice versa.
        /// </summary>
        /// <returns></returns>
        public int RowTransitions()
        {
            var transitions = 0;
            for (var column = 0; column < this.gameBoard[0].Length; column++)
            {
                var prevValue = this.gameBoard[0][column];
                for (var row = this.gameBoard.GetLength(0) - 1; row >= 0; row--)
                {
                    if (this.gameBoard[row][column] != prevValue)
                    {
                        transitions++;
                        prevValue = !prevValue;
                    }
                }
            }

            return transitions;
        }

        /// <summary>
        /// The total number of column transitions.
        /// A column transition occurs when an empty cell is adjacent to a filled cell
        /// on the same row and vice versa.
        /// </summary>
        /// <returns></returns>
        public int ColumnTransitions()
        {
            var transitions = 0;

            for (var row = this.gameBoard.GetLength(0) - 1; row >= 0; row--)
            {
                var prevValue = this.gameBoard[row][0];
                for (var column = 0; column < this.gameBoard[0].Length; column++)
                {
                    if (this.gameBoard[row][column] != prevValue)
                    {
                        transitions++;
                        prevValue = !prevValue;
                    }
                }
            }

            return transitions;
        }

        /// <summary>
        /// Number of Holes.
        /// A hole is an empty cell that has at least one filled cell above it in the same column.
        /// </summary>
        /// <returns></returns>
        public int NumberOfHoles()
        {
            var holes = 0;

            for (var column = 0; column < this.gameBoard[0].Length; column++)
            {
                var reachedTopColumn = false;

                for (var row = this.gameBoard.GetLength(0) - 1; row >= 0; row--)
                {
                    var field = this.gameBoard[row][column];
                    if (reachedTopColumn && !field)
                    {
                        holes++;
                    }

                    if (field)
                    {
                        reachedTopColumn = true;
                    }
                }
            }

            return holes;
        }

        /// <summary>
        /// Well sums
        /// A well is a sequence of empty cells above the top piece in a column such
        /// that the top cell in the sequence is surrounded (left and right) by occupied
        /// cells or a boundary of the board.
        ///
        ///
        /// Args:
        ///   board - The game board (an array of integers)
        ///   num_columns - Number of columns in the board
        ///
        /// Return:
        ///    The well sums. For a well of length n, we define the well sums as
        ///    1 + 2 + 3 + ... + n. This gives more significance to deeper holes.
        /// </summary>
        /// <returns></returns>
        public int WellSums()
        {
            var wellSum = 0;
            for (var column = 1; column < this.gameBoard[0].Length - 1; column++)
            {
                for (var row = this.gameBoard.GetLength(0) - 1; row >= 0; row--)
                {
                    if (!(!this.gameBoard[row][column] && this.gameBoard[row][column - 1] && this.gameBoard[row][column + 1]))
                    {
                        continue;
                    }

                    // Found well count the depth
                    wellSum++;
                    for (var k = row - 1; k >= 0; --k)
                    {
                        if (!this.gameBoard[k][column])
                        {
                            wellSum++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Check for wells io most left columns
            for (var row = this.gameBoard.GetLength(0) - 1; row >= 0; row--)
            {
                if (!(!this.gameBoard[row][0] && this.gameBoard[row][0 + 1]))
                {
                    continue;
                }

                // Found well count the depth
                wellSum++;
                for (var k = row - 1; k >= 0; --k)
                {
                    if (!this.gameBoard[k][0])
                    {
                        wellSum++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Check for wells io most right columns
            for (var row = this.gameBoard.GetLength(0) - 1; row >= 0; row--)
            {
                if (!(!this.gameBoard[row][this.gameBoard[0].Length - 1] && this.gameBoard[row][this.gameBoard[0].Length - 2]))
                {
                    continue;
                }

                // Found well count the depth
                wellSum++;
                for (var k = row - 1; k >= 0; --k)
                {
                    if (!this.gameBoard[k][0])
                    {
                        wellSum++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return wellSum;
        }

        /// <summary>
        /// Height of the last block placed
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public int LandingHeight(int row)
        {
            return row / 2;
        }
    }
}
