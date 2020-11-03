using System;

namespace Core
{
    [Serializable, Flags]
    public enum IntMask : int
    {
        None   = 0,
        Mask01 = 1,
        Mask02 = 1 << 1,
        Mask03 = 1 << 2,
        Mask04 = 1 << 3,
        Mask05 = 1 << 4,
        Mask06 = 1 << 5,
        Mask07 = 1 << 6,
        Mask08 = 1 << 7,
        Mask09 = 1 << 8,
        Mask10 = 1 << 9,
        Mask11 = 1 << 10,
        Mask12 = 1 << 11,
        Mask13 = 1 << 12,
        Mask14 = 1 << 13,
        Mask15 = 1 << 14,
        Mask16 = 1 << 15,
        Mask17 = 1 << 16,
        Mask18 = 1 << 17,
        Mask19 = 1 << 18,
        Mask20 = 1 << 19,
        Mask21 = 1 << 20,
        Mask22 = 1 << 21,
        Mask23 = 1 << 22,
        Mask24 = 1 << 23,
        Mask25 = 1 << 24,
        Mask26 = 1 << 25,
        Mask27 = 1 << 26,
        Mask28 = 1 << 27,
        Mask29 = 1 << 28,
        Mask30 = 1 << 29,
        Mask31 = 1 << 30,
        Mask32 = 1 << 31
    }
}