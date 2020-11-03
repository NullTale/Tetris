using System;
using System.Collections.Generic;
using Core;

namespace Core
{
    [Serializable]
    public enum Dice : int
    {
        None	= 0,

        D4		= DiceType.D4 | DiceValue.D4,
        D6		= DiceType.D6 | DiceValue.D6,
        D8		= DiceType.D8 | DiceValue.D8,
        D10		= DiceType.D10 | DiceValue.D10,
        D12		= DiceType.D12 | DiceValue.D12,
        D20		= DiceType.D20 | DiceValue.D20,
    }

    [Serializable, Flags]
    public enum DiceType : int
    {
	    None	= 0,

	    D4		= 1 << 8,
	    D6		= 1 << 9,
	    D8		= 1 << 10,
	    D10		= 1 << 11,
	    D12		= 1 << 12,
	    D20		= 1 << 13,
    }

    [Serializable]
    public enum DiceValue : int
    {
	    None	= 0,

	    D4		= 4,
	    D6		= 6,
	    D8		= 8,
	    D10		= 10,
	    D12		= 12,
	    D20		= 20,
    }

    [Serializable]
    public enum RollValue : int
    {
	    None	= 0,

	    D1		= 1,
	    D2		= 2,
	    D3		= 3,
	    D4		= 4,
	    D5		= 5,
	    D6		= 6,
	    D7		= 7,
	    D8		= 7,
	    D9		= 9,
	    D10		= 10,
	    D11		= 11,
	    D12		= 12,
	    D13		= 13,
	    D14		= 14,
	    D15		= 15,
	    D16		= 16,
	    D17		= 17,
	    D18		= 18,
	    D19		= 19,
	    D20		= 20,
    }


    public static class DiceHelper
    {
	    public static int Roll(int dice)
	    {
		    return dice > 0 ? (UnityEngine.Random.Range(0, dice) + 1) : 0;
	    }

	    public static int Roll(DiceType dice)
	    {
		    return Roll((int)g_DiceTypeValue[dice]);
	    }

	    public static int Roll(DiceValue dice)
	    {
		    return Roll((int)dice);
	    }
	    
	    public static int Roll(Dice dice)
	    {
		    return Roll((int)DiceValueOf(dice));
	    }


	    public static int Roll(int count, Dice dice)
	    {
		    var result = 0;
		    for (var n = 0; n < count; n++)
			    result += Roll(dice);

		    return result;
	    }

	    public static int Roll(int count, Dice dice, out List<int> rollList)
	    {
		    rollList = new List<int>(count);
		    var result = 0;
		    for (var n = 0; n < count; n++)
		    {
			    var roll = Roll(dice);
			    rollList.Add(roll);
			    result += roll;
		    }

		    return result;
	    }
	    
	    public static readonly SortedDictionary<DiceType, DiceValue> g_DiceTypeValue =
		    new SortedDictionary<DiceType, DiceValue>()
		    {
			    {DiceType.None, DiceValue.None},
			    {DiceType.D4, DiceValue.D4},
			    {DiceType.D6, DiceValue.D6},
			    {DiceType.D8, DiceValue.D8},
			    {DiceType.D10, DiceValue.D10},
			    {DiceType.D12, DiceValue.D12},
			    {DiceType.D20, DiceValue.D20},
		    };
	    
	    public static readonly SortedDictionary<DiceValue, DiceType> g_DiceValueType =
		    new SortedDictionary<DiceValue, DiceType>()
		    {
			    {DiceValue.None,  DiceType.None},
			    {DiceValue.D4,  DiceType.D4},
			    {DiceValue.D6,  DiceType.D6},
			    {DiceValue.D8,  DiceType.D8},
			    {DiceValue.D10, DiceType.D10},
			    {DiceValue.D12, DiceType.D12},
			    {DiceValue.D20, DiceType.D20},
		    };
	    
	    
	    public static Dice DiceOf(DiceValue dice)
	    {
		    return (Dice)((int)dice | (int)DiceTypeOf(dice));
	    }

	    public static Dice DiceOf(DiceType dice)
	    {
		    return (Dice)((int)dice | (int)DiceValueOf(dice));
	    }

	    public static DiceType DiceTypeOf(DiceValue dice)
	    {
		    return g_DiceValueType[dice];
	    }

	    public static DiceType DiceTypeOf(Dice dice)
	    {
		    return (DiceType)((int)dice >> 8);
	    }

	    public static DiceValue DiceValueOf(Dice dice)
	    {
		    return (DiceValue)((int)dice & 0x0000ff);
	    }

	    public static DiceValue DiceValueOf(DiceType dice)
	    {
		    return g_DiceTypeValue[dice];
	    }
    }
}