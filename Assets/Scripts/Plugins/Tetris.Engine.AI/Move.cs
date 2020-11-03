namespace Tetris.Engine.AI
{
    using System;
    using System.Collections.Generic;

    public class Move
    {
        private Tetris.Engine.Move[]    moves;

        internal int                    Rotation { get; set; }
        internal int                    ColumnOffSet { get; set; }
        public float                    Fitness { get; set; }
        public bool                     IsValid { get; set; }

        public Tetris.Engine.Move[] Moves
        {
            get
            {
                if (moves == null)
                {
                    var list = new List<Tetris.Engine.Move>(Rotation + Math.Abs(ColumnOffSet) + 1);
                    list.Add(Tetris.Engine.Move.None);

                    for (var i = 0; i < Rotation; i++) 
                        list.Add(Tetris.Engine.Move.RotateRight);

                    var dir = ColumnOffSet < 0 ? Tetris.Engine.Move.Left : Tetris.Engine.Move.Right;
                    for (var i = 0; i < Math.Abs(ColumnOffSet); i++) 
                        list.Add(dir);

                    moves = list.ToArray();
                }

                return moves;
            }
        }
    }
}
