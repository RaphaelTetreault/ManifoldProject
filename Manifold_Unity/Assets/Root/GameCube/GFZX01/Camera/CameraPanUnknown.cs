namespace GameCube.GFZX01.Camera
{
    [System.Flags]
    public enum CameraPanUnknown : byte
    {
        _1 = 1 << 0,
        _2 = 1 << 1,
        _3 = 1 << 2,
        _4 = 1 << 3,
        _5 = 1 << 4,
        _6 = 1 << 5,
        _7 = 1 << 6,
        _8 = 1 << 7,
        //_9 = 1 << 8,
        //_10 = 1 << 9,
        //_11 = 1 << 10,

        //Mask = 0b00000_11111111111,
    }


    ///// <summary>
    ///// % bits that control rotation. These are flags, but duw to 
    ///// how they are stored and would be represented in Unity, this
    ///// is a good compromise between being explicit and concise.
    ///// </summary>
    //public enum CameraPanRotation : sbyte
    //{
    //    CCW_Rotate0 = 0b00000_00000000000, // 0
    //    CCW_Rotate10 = 0b00001_00000000000, // 1
    //    CCW_Rotate25 = 0b00010_00000000000, // 2
    //    CCW_Rotate35 = 0b00011_00000000000, // 3
    //    CCW_Rotate45 = 0b00100_00000000000, // 4
    //    CCW_Rotate55 = 0b00101_00000000000, // 5
    //    CCW_Rotate70 = 0b00110_00000000000, // 6
    //    CCW_Rotate80 = 0b00111_00000000000, // 7
    //    CCW_Rotate90 = 0b01000_00000000000, // 8
    //    CCW_Rotate100 = 0b01001_00000000000, // 9
    //    CCW_Rotate115 = 0b01010_00000000000, // 10
    //    CCW_Rotate125 = 0b01011_00000000000, // 11
    //    CCW_Rotate135 = 0b01100_00000000000, // 12
    //    CCW_Rotate145 = 0b01101_00000000000, // 13
    //    CCW_Rotate160 = 0b01110_00000000000, // 14
    //    CCW_Rotate170 = 0b01111_00000000000, // 15
    //    CW_Rotate180 = 0b10000_00000000000, // -16
    //    CW_Rotate170 = 0b10001_00000000000, // -15
    //    CW_Rotate155 = 0b10010_00000000000, // -14
    //    CW_Rotate145 = 0b10011_00000000000, // -13
    //    CW_Rotate135 = 0b10100_00000000000, // -12
    //    CW_Rotate125 = 0b10101_00000000000, // -11
    //    CW_Rotate110 = 0b10110_00000000000, // -10
    //    CW_Rotate100 = 0b10111_00000000000, // -9
    //    CW_Rotate90 = 0b11000_00000000000, // -8
    //    CW_Rotate80 = 0b11001_00000000000, // -7
    //    CW_Rotate65 = 0b11010_00000000000, // -6
    //    CW_Rotate55 = 0b11011_00000000000, // -5
    //    CW_Rotate45 = 0b11100_00000000000, // -4
    //    CW_Rotate35 = 0b11101_00000000000, // -3
    //    CW_Rotate20 = 0b11110_00000000000, // -2
    //    CW_Rotate10 = 0b11111_00000000000, // -1

    //    Mask = 0b11111_00000000000,
    //}
}
