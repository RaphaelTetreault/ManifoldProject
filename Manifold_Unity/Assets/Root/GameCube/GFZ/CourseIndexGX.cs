using System.ComponentModel;

namespace GameCube.GFZ
{
    /// <summary>
    /// 
    /// </summary>
    public enum CourseIndexGX
    {
        [Description("---")]
        None = -1,

        [Description("Screw Drive (test)")]
        TestScrewDrive = 0,

        [Description("Twist Road")]
        TwistRoad = 1,
        [Description("Serial Gaps")]
        SerialGaps = 3,
        [Description("Multiplex")]
        Multiplex = 5,
        [Description("Aero Dive")]
        AeroDive = 7,
        [Description("Loop Cross")]
        LoopCross = 8,
        [Description("Half Pipe")]
        HalfPipe = 9,
        [Description("Intersection")]
        Intersection = 10,
        [Description("Mobius Ring")]
        MobiusRing = 11,
        [Description("Long Pipe")]
        LongPipe = 13,
        [Description("Drift Highway")]
        DriftHighway = 14,
        [Description("Cylinder Knot")]
        CylinderKnot = 15,
        [Description("Split Oval")]
        SplitOval = 16,
        [Description("Undulation")]
        Undulation = 17,
        [Description("Dragon Slope")]
        DragonSlope = 21,
        [Description("Trident")]
        Trident = 24,
        [Description("Lateral Shift")]
        LateralShift = 25,
        [Description("Surface Slide")]
        SurfaceSlide = 26,
        [Description("Ordeal")]
        Ordeal = 27,
        [Description("Slim-Line Slits")]
        SlimLineSlits = 28,
        [Description("Double Branches")]
        DoubleBranches = 29,
        [Description("Screw Drive")]
        ScrewDrive = 31,
        [Description("Meteor Stream")]
        MeteorStream = 32,
        [Description("Cylinder Wave")]
        CylinderWave = 33,
        [Description("Thunder Road")]
        ThunderRoad = 34,
        [Description("Spiral")]
        Spiral = 35,
        [Description("Sonic Oval")]
        SonicOval = 36,
        [Description("Story 1 - Captain Falcon Trains")]
        Story1 = 37,
        [Description("Story 2 - Goroh: The Vengeful Samurai")]
        Story2 = 38,
        [Description("Story 3 - High Stakes in Mute City")]
        Story3 = 39,
        [Description("Story 4 - Challenge of the Bloody Chain")]
        Story4 = 40,
        [Description("Story 5 - Save Jody!")]
        Story5 = 41,
        [Description("Story 6 - Black Shadow's Trap")]
        Story6 = 42,
        [Description("Story 7 - The F-Zero Grand Prix")]
        Story7 = 43,
        [Description("Story 8 - Secrets of the Champion Belt")]
        Story8 = 44,
        [Description("Story 9 - Finale: Enter the Creators")]
        Story9 = 45,
        [Description("Grand Prix Podium")]
        GrandPrixPodium = 49,
        [Description("Victory Lap")]
        VictoryLap = 50,
    }
}