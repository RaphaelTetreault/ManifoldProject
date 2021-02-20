﻿using GameCube.GFZX01;

namespace GameCube.GFZX01
{
    public static class CourseUtility
    {
        /// <summary>
        /// Returns tuple of (CourseVenue, CourseVenueID, CourseName) from both F-Zero GX
        /// and F-Zero AX for any existing course indices.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static (CourseVenue venue, CourseVenueID venueID, CourseName name) GetCourseInfo(int index)
        {
            switch (index)
            {
                #region GX STAGES

                case 1: return (CourseVenue.MuteCity, CourseVenueID.MUT, CourseName.TwistRoad);
                ////
                case 3: return (CourseVenue.MuteCity, CourseVenueID.MUT, CourseName.SerialGaps);
                ////
                case 5: return (CourseVenue.Aeropolis, CourseVenueID.TOW, CourseName.Multiplex);
                ////
                case 7: return (CourseVenue.PortTown, CourseVenueID.POR, CourseName.AeroDive);
                case 8: return (CourseVenue.Lightning, CourseVenueID.LIG, CourseName.LoopCross);
                case 9: return (CourseVenue.Lightning, CourseVenueID.LIG, CourseName.HalfPipe);
                case 10: return (CourseVenue.GreenPlant, CourseVenueID.FOR, CourseName.Intersection);
                case 11: return (CourseVenue.GreenPlant, CourseVenueID.FOR, CourseName.MobiusRing);
                ////
                case 13: return (CourseVenue.PortTown, CourseVenueID.POR, CourseName.LongPipe);
                case 14: return (CourseVenue.BigBlue, CourseVenueID.BIG, CourseName.DriftHighway);
                case 15: return (CourseVenue.FireField, CourseVenueID.FIR, CourseName.CylinderKnot);
                case 16: return (CourseVenue.CasinoPalace, CourseVenueID.CAS, CourseName.SplitOval);
                case 17: return (CourseVenue.FireField, CourseVenueID.FIR, CourseName.Undulation);
                ////
                case 21: return (CourseVenue.Aeropolis, CourseVenueID.TOW, CourseName.DragonSlope);
                ////
                case 24: return (CourseVenue.CosmoTerminal, CourseVenueID.ELE, CourseName.Trident);
                case 25: return (CourseVenue.SandOcean, CourseVenueID.SAN, CourseName.LateralShift);
                case 26: return (CourseVenue.SandOcean, CourseVenueID.SAN, CourseName.SurfaceSlide);
                case 27: return (CourseVenue.BigBlue, CourseVenueID.BIG, CourseName.Ordeal);
                case 28: return (CourseVenue.PhantomRoad, CourseVenueID.RAI, CourseName.SlimLineSlits);
                case 29: return (CourseVenue.CasinoPalace, CourseVenueID.CAS, CourseName.DoubleBranches);

                #endregion

                #region AX STAGES

                case 31: return (CourseVenue.Aeropolis, CourseVenueID.TOW, CourseName.ScrewDrive);
                case 32: return (CourseVenue.OuterSpace, CourseVenueID.MET, CourseName.MeteorStream);
                case 33: return (CourseVenue.PortTown, CourseVenueID.POR, CourseName.CylinderWave);
                case 34: return (CourseVenue.Lightning, CourseVenueID.LIG, CourseName.ThunderRoad);
                case 35: return (CourseVenue.GreenPlant, CourseVenueID.FOR, CourseName.Spiral);
                case 36: return (CourseVenue.MuteCity, CourseVenueID.COM, CourseName.SonicOval);

                #endregion

                #region STORY

                case 37: return (CourseVenue.MuteCity, CourseVenueID.COM_S, CourseName.Story1);
                case 38: return (CourseVenue.SandOcean, CourseVenueID.SAN_S, CourseName.Story2);
                case 39: return (CourseVenue.CasinoPalace, CourseVenueID.CAS, CourseName.Story3);
                case 40: return (CourseVenue.BigBlue, CourseVenueID.BIG_S, CourseName.Story4);
                case 41: return (CourseVenue.Lightning, CourseVenueID.LIG, CourseName.Story5);
                case 42: return (CourseVenue.PortTown, CourseVenueID.POR_S, CourseName.Story6);
                case 43: return (CourseVenue.MuteCity, CourseVenueID.MUT, CourseName.Story7);
                case 44: return (CourseVenue.FireField, CourseVenueID.FIR_S, CourseName.Story8);
                case 45: return (CourseVenue.PhantomRoad, CourseVenueID.RAI, CourseName.Story9);

                #endregion

                #region GRAND PRIX / VS WIN

                case 49: return (CourseVenue.MuteCity, CourseVenueID.WIN_GX, CourseName.GrandPrixPodium);
                case 50: return (CourseVenue.MuteCity, CourseVenueID.WIN, CourseName.VictoryLap);

                #endregion

                #region UNUSED GX STAGES

                case 0: return (CourseVenue.SandOcean, CourseVenueID.SAN, CourseName.ScrewDrive);

                #endregion

                #region UNUSED AX STAGES

                case 72: return (CourseVenue.None, CourseVenueID.None, CourseName.SurfaceSlide);
                ////
                case 77: return (CourseVenue.None, CourseVenueID.None, CourseName.LoopCross);
                ////
                case 86: return (CourseVenue.None, CourseVenueID.None, CourseName.MeteorStream);
                case 87: return (CourseVenue.None, CourseVenueID.None, CourseName.CylinderWave);
                ////
                case 90: return (CourseVenue.None, CourseVenueID.None, CourseName.LongPipe);
                case 91: return (CourseVenue.None, CourseVenueID.None, CourseName.Story2);
                case 92: return (CourseVenue.None, CourseVenueID.None, CourseName.Story3);
                case 93: return (CourseVenue.None, CourseVenueID.None, CourseName.Story4);
                case 94: return (CourseVenue.None, CourseVenueID.None, CourseName.Story5);
                case 95: return (CourseVenue.None, CourseVenueID.None, CourseName.Story6);
                case 96: return (CourseVenue.None, CourseVenueID.None, CourseName.Story7);
                case 97: return (CourseVenue.None, CourseVenueID.None, CourseName.Story8);
                case 98: return (CourseVenue.None, CourseVenueID.None, CourseName.Story9);
                ////
                case 101: return (CourseVenue.None, CourseVenueID.None, CourseName.TwistRoad);
                case 102: return (CourseVenue.None, CourseVenueID.None, CourseName.TwistRoad);
                case 103: return (CourseVenue.None, CourseVenueID.None, CourseName.Multiplex);
                case 104: return (CourseVenue.None, CourseVenueID.None, CourseName.Intersection);
                case 105: return (CourseVenue.None, CourseVenueID.None, CourseName.Undulation);
                ////
                case 107: return (CourseVenue.None, CourseVenueID.None, CourseName.DriftHighway);
                case 108: return (CourseVenue.None, CourseVenueID.None, CourseName.AeroDive);
                case 109: return (CourseVenue.None, CourseVenueID.None, CourseName.LateralShift);

                #endregion

                #region UNUSED VENUE DEFINED

                // Unused Mute City
                case 2:
                case 4:
                    return (CourseVenue.MuteCity, CourseVenueID.MUT, CourseName.None);

                // Unused Port Town
                case 6:
                    return (CourseVenue.PortTown, CourseVenueID.POR, CourseName.None);

                // Unused Lighting
                case 12:
                case 23:
                    return (CourseVenue.Lightning, CourseVenueID.LIG, CourseName.None);

                // Unused Fire Field
                case 18:
                    return (CourseVenue.FireField, CourseVenueID.FIR, CourseName.None);

                // Unused Outer Space
                case 19:
                case 20:
                    return (CourseVenue.OuterSpace, CourseVenueID.MET, CourseName.None);

                // Unused Cosmo Terminal
                case 22:
                    return (CourseVenue.CosmoTerminal, CourseVenueID.ELE, CourseName.None);

                // Unused Sand Ocean
                case 30:
                    return (CourseVenue.SandOcean, CourseVenueID.SAN, CourseName.None);

                #endregion

                default:
                    return (CourseVenue.None, CourseVenueID.None, CourseName.None);
            }
        }

        public static CourseVenueID GetVenueID(int index)
        {
            switch (index)
            {
                // MUT == Mute City
                case 1:
                case 2:
                case 3:
                case 4:
                    return CourseVenueID.MUT;

                // SAN == Sand Ocean
                case 0:
                case 25:
                case 26:
                case 30:
                    return CourseVenueID.SAN;

                // TOW = Aeropolis
                case 5:
                case 21:
                case 31:
                    return CourseVenueID.TOW;

                // POR == Port Town
                case 6:
                case 7:
                case 13:
                case 33:
                    return CourseVenueID.POR;

                // LIG == Lightning
                case 8:
                case 9:
                case 12:
                case 23:
                case 34:
                    return CourseVenueID.LIG;

                // FOR == Green Plant
                case 10:
                case 11:
                case 35:
                    return CourseVenueID.FOR;

                // BIG == Big Blue
                case 14:
                case 27:
                    return CourseVenueID.BIG;

                // FIR == Fire Field
                case 15:
                case 17:
                case 18:
                    return CourseVenueID.FIR;

                // CAS == Casino Palace
                case 16:
                case 29:
                    return CourseVenueID.CAS;

                // MET == Outer Space
                case 19:
                case 20:
                case 32:
                    return CourseVenueID.MET;

                // ELE == Cosmo Terminal
                case 22:
                case 24:
                    return CourseVenueID.ELE;

                // RAI == Phantom Road
                case 28:
                    return CourseVenueID.RAI;

                // COM == Mute City / Casino Palace hybrid
                case 36:
                    return CourseVenueID.COM;

                // Story venues
                case 37: return CourseVenueID.COM_S;
                case 38: return CourseVenueID.SAN_S;
                case 39: return CourseVenueID.CAS;
                case 40: return CourseVenueID.BIG_S;
                case 41: return CourseVenueID.LIG;
                case 42: return CourseVenueID.POR_S;
                case 43: return CourseVenueID.MUT;
                case 44: return CourseVenueID.FIR_S;
                case 45: return CourseVenueID.RAI;

                // Victory screens
                case 49: return CourseVenueID.WIN;
                case 50: return CourseVenueID.WIN_GX;

                default:
                    return CourseVenueID.None;
            }
        }

        public static CourseID GetCourseID(int index)
        {
            var courseID = (CourseID)index;
            var notDefined = !System.Enum.IsDefined(typeof(CourseID), courseID);
            if (notDefined)
            {
                courseID = CourseID.None;
            }
            return courseID;
        }

    }
}
