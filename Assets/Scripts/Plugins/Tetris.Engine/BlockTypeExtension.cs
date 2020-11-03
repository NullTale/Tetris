using System;
using System.Linq;

namespace Tetris.Engine
{
    using Extensions;

    public static class BlockTypeExtension
    {
        public class BlockData
        {
            public int              Dimention       { get; private set; }
            private string[]        MatrixData      { get; set; }

            public bool[][][]       Matrix          { get; private set; }
            public int              Length          { get; private set; }

            public bool[][]         this[int n] => Matrix[n];

            //////////////////////////////////////////////////////////////////////////
            public BlockData(int dimention, string[] matrix)
            {
                Dimention = dimention;
                MatrixData = matrix;
                Length = MatrixData.Length;
                Matrix = MatrixData.Select(n => n.StringToBoolMatrix(dimention)).ToArray();
            }

        }

        private static BlockData O = new BlockData(2, new []{
            @"11
              11" });

        private static BlockData I = new BlockData(4, new []{
            @"0000
              1111
              0000
              0000", 
            @"0010
              0010
              0010
              0010",
            @"0000
              0000
              1111
              0000",
            @"0100
              0100
              0100
              0100", });

        private static BlockData J = new BlockData(3, new []{
            @"100
              111
              000",
            @"011
              010
              010",
            @"000
              111
              001",
            @"010
              010
              110",
        });

        private static BlockData Z = new BlockData(3, new []{
            @"110
              011
              000",
            @"001
              011
              010",
            @"000
              110
              011",
            @"010
              110
              100", });

        private static BlockData S = new BlockData(3, new []{
            @"011
              110
              000",
            @"010
              011
              001",
            @"000
              011
              110",
            @"100
              110
              010" });
        
        private static BlockData L = new BlockData(3, new []{
            @"001
              111
              000",
            @"010
              010
              011",
            @"000
              111
              100",
            @"110
              010
              010",});
        
        private static BlockData T = new BlockData(3, new []{
            @"010
              111
              000",
            @"010
              011
              010",
            @"000
              111
              010",
            @"010
              110
              010",});

        //////////////////////////////////////////////////////////////////////////
        public static bool[][] Rotation(this BlockType type, int rotationIndex)
        {
            switch (type)
            {
                case BlockType.O: return O[0];
                case BlockType.I: return I[Math.Abs(rotationIndex) % I.Length];
                case BlockType.J: return J[Math.Abs(rotationIndex) % J.Length];
                case BlockType.Z: return Z[Math.Abs(rotationIndex) % Z.Length];
                case BlockType.S: return S[Math.Abs(rotationIndex) % S.Length];
                case BlockType.L: return L[Math.Abs(rotationIndex) % L.Length];
                case BlockType.T: return T[Math.Abs(rotationIndex) % T.Length];
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        public static int BlockDimension(this BlockType type)
        {
            switch (type)
            {
                case BlockType.O: return O.Dimention;
                case BlockType.I: return I.Dimention;
                case BlockType.J: return J.Dimention;
                case BlockType.Z: return Z.Dimention;
                case BlockType.S: return S.Dimention;
                case BlockType.L: return L.Dimention;
                case BlockType.T: return T.Dimention;
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        public static int BlockRotations(this BlockType type)
        {
            switch (type)
            {
                case BlockType.O: return O.Length;
                case BlockType.I: return I.Length;
                case BlockType.J: return J.Length;
                case BlockType.Z: return Z.Length;
                case BlockType.S: return S.Length;
                case BlockType.L: return L.Length;
                case BlockType.T: return T.Length;
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
