using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class StageUtility
{
    public static Tuple<StageVenue, StageVenueID, StageName> GetStageInfo(int index)
    {
        switch (index)
        {
            #region GX STAGES

            case 1:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.MuteCity,
                StageVenueID.MUT,
                StageName.TwistRoad);

            case 3:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.MuteCity,
                StageVenueID.MUT,
                StageName.SerialGaps);


            case 5:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.Aeropolis,
                StageVenueID.TOW,
                StageName.Multiplex);

            case 7:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.PortTown,
                StageVenueID.POR,
                StageName.AeroDive);

            case 8:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.Lightning,
                StageVenueID.LIG,
                StageName.LoopCross);

            case 9:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.Lightning,
                StageVenueID.LIG,
                StageName.HalfPipe);

            case 10:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.GreenPlant,
                StageVenueID.FOR,
                StageName.Intersection);

            case 11:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.GreenPlant,
                StageVenueID.FOR,
                StageName.MobiusRing);

            case 13:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.PortTown,
                StageVenueID.POR,
                StageName.LongPipe);

            case 14:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.BigBlue,
                StageVenueID.BIG,
                StageName.DriftHighway);

            case 15:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.FireField,
                StageVenueID.FIR,
                StageName.CylinderKnot);

            case 16:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.CasinoPalace,
                StageVenueID.CAS,
                StageName.SplitOval);

            case 17:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.FireField,
                StageVenueID.FIR,
                StageName.Undulation);

            case 21:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.Aeropolis,
                StageVenueID.TOW,
                StageName.DragonSlope);

            case 24:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.CosmoTerminal,
                StageVenueID.ELE,
                StageName.Trident);

            case 25:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.SandOcean,
                StageVenueID.SAN,
                StageName.LateralShift);

            case 26:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.SandOcean,
                StageVenueID.SAN,
                StageName.SurfaceSlide);

            case 27:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.BigBlue,
                StageVenueID.BIG,
                StageName.Ordeal);

            case 28:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.PhantomRoad,
                StageVenueID.RAI,
                StageName.SlimLineSlits);

            case 29:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.CasinoPalace,
                StageVenueID.CAS,
                StageName.DoubleBranches);

            #endregion

            #region AX STAGES

            case 31:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.Aeropolis,
                StageVenueID.TOW,
                StageName.ScrewDrive);

            case 32:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.OuterSpace,
                StageVenueID.MET,
                StageName.MeteorStream);

            case 33:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.PortTown,
                StageVenueID.POR,
                StageName.CylinderWave);

            case 34:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.Lightning,
                StageVenueID.LIG,
                StageName.ThunderRoad);

            case 35:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.GreenPlant,
                StageVenueID.FOR,
                StageName.Spiral);

            case 36:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.MuteCity,
                StageVenueID.COM,
                StageName.SonicOval);

            #endregion

            #region STORY

            case 37:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.MuteCity,
                StageVenueID.COM_S,
                StageName.Story1);

            case 38:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.SandOcean,
                StageVenueID.SAN_S,
                StageName.Story2);

            case 39:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.CasinoPalace,
                StageVenueID.CAS,
                StageName.Story3);

            case 40:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.BigBlue,
                StageVenueID.BIG_S,
                StageName.Story4);

            case 41:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.Lightning,
                StageVenueID.LIG,
                StageName.Story5);

            case 42:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.PortTown,
                StageVenueID.POR_S,
                StageName.Story6);

            case 43:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.MuteCity,
                StageVenueID.MUT,
                StageName.Story7);

            case 44:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.FireField,
                StageVenueID.FIR_S,
                StageName.Story8);

            case 45:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.PhantomRoad,
                StageVenueID.RAI,
                StageName.Story9);

            #endregion

            #region GRAND PRIX / VS WIN

            case 49:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.MuteCity,
                StageVenueID.WIN_GX,
                StageName.GrandPrixPodium);

            case 50:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.MuteCity,
                StageVenueID.WIN,
                StageName.VictoryLap);

            #endregion

            #region UNUSED GX STAGES

            case 0:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                    StageVenue.SandOcean,
                    StageVenueID.SAN,
                    StageName.ScrewDrive);

            #endregion

            #region UNUSED AX STAGES

            case 72:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.SurfaceSlide);

            case 77:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.LoopCross);

            case 86:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.MeteorStream);

            case 87:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.CylinderWave);

            case 90:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.LongPipe);

            case 91:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Story2);

            case 92:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Story3);

            case 93:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Story4);

            case 94:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Story5);

            case 95:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Story6);

            case 96:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Story7);

            case 97:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Story8);

            case 98:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Story9);

            case 101:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.TwistRoad);


            case 102:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.TwistRoad);

            case 103:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Multiplex);

            case 104:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Intersection);

            case 105:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.Undulation);

            case 107:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.DriftHighway);

            case 108:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.AeroDive);

            case 109:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.LateralShift);

            #endregion

            #region UNUSED VENUE DEFINED

            // Unused Mute City
            case 2:
            case 4:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.MuteCity,
                StageVenueID.MUT,
                StageName.None);

            // Unused Port Town
            case 6:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.PortTown,
                StageVenueID.POR,
                StageName.None);

            //Unused Lighting
            case 12:
            case 23:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.Lightning,
                StageVenueID.LIG,
                StageName.None);

            // Unused Fire Field
            case 18:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.FireField,
                StageVenueID.FIR,
                StageName.None);

            // Unused Outer Space
            case 19:
            case 20:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.OuterSpace,
                StageVenueID.MET,
                StageName.None);

            // Unused Cosmo Terminal
            case 22:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.CosmoTerminal,
                StageVenueID.ELE,
                StageName.None);

            // Unused Sand Ocean
            case 30:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.SandOcean,
                StageVenueID.SAN,
                StageName.None);

            #endregion

            default:
                return new Tuple<StageVenue, StageVenueID, StageName>(
                StageVenue.None,
                StageVenueID.None,
                StageName.None);
        }
    }


}
