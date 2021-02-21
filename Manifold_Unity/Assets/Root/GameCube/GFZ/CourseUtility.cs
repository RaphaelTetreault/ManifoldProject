namespace GameCube.GFZ
{
    public static class CourseUtility
    {
        /// <summary>
        /// Returns tuple of (CourseVenue, CourseVenueID, CourseName) from both F-Zero GX
        /// and F-Zero AX for any existing course indices.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static (VenueName venue, VenueID venueID, CourseName name) GetCourseInfo(int index)
        {
            switch (index)
            {
                #region GX STAGES

                case 1: return (VenueName.MuteCity, VenueID.MUT, CourseName.TwistRoad);
                ////
                case 3: return (VenueName.MuteCity, VenueID.MUT, CourseName.SerialGaps);
                ////
                case 5: return (VenueName.Aeropolis, VenueID.TOW, CourseName.Multiplex);
                ////
                case 7: return (VenueName.PortTown, VenueID.POR, CourseName.AeroDive);
                case 8: return (VenueName.Lightning, VenueID.LIG, CourseName.LoopCross);
                case 9: return (VenueName.Lightning, VenueID.LIG, CourseName.HalfPipe);
                case 10: return (VenueName.GreenPlant, VenueID.FOR, CourseName.Intersection);
                case 11: return (VenueName.GreenPlant, VenueID.FOR, CourseName.MobiusRing);
                ////
                case 13: return (VenueName.PortTown, VenueID.POR, CourseName.LongPipe);
                case 14: return (VenueName.BigBlue, VenueID.BIG, CourseName.DriftHighway);
                case 15: return (VenueName.FireField, VenueID.FIR, CourseName.CylinderKnot);
                case 16: return (VenueName.CasinoPalace, VenueID.CAS, CourseName.SplitOval);
                case 17: return (VenueName.FireField, VenueID.FIR, CourseName.Undulation);
                ////
                case 21: return (VenueName.Aeropolis, VenueID.TOW, CourseName.DragonSlope);
                ////
                case 24: return (VenueName.CosmoTerminal, VenueID.ELE, CourseName.Trident);
                case 25: return (VenueName.SandOcean, VenueID.SAN, CourseName.LateralShift);
                case 26: return (VenueName.SandOcean, VenueID.SAN, CourseName.SurfaceSlide);
                case 27: return (VenueName.BigBlue, VenueID.BIG, CourseName.Ordeal);
                case 28: return (VenueName.PhantomRoad, VenueID.RAI, CourseName.SlimLineSlits);
                case 29: return (VenueName.CasinoPalace, VenueID.CAS, CourseName.DoubleBranches);

                #endregion

                #region AX STAGES

                case 31: return (VenueName.Aeropolis, VenueID.TOW, CourseName.ScrewDrive);
                case 32: return (VenueName.OuterSpace, VenueID.MET, CourseName.MeteorStream);
                case 33: return (VenueName.PortTown, VenueID.POR, CourseName.CylinderWave);
                case 34: return (VenueName.Lightning, VenueID.LIG, CourseName.ThunderRoad);
                case 35: return (VenueName.GreenPlant, VenueID.FOR, CourseName.Spiral);
                case 36: return (VenueName.MuteCity, VenueID.COM, CourseName.SonicOval);

                #endregion

                #region STORY

                case 37: return (VenueName.MuteCity, VenueID.COM_S, CourseName.Story1);
                case 38: return (VenueName.SandOcean, VenueID.SAN_S, CourseName.Story2);
                case 39: return (VenueName.CasinoPalace, VenueID.CAS, CourseName.Story3);
                case 40: return (VenueName.BigBlue, VenueID.BIG_S, CourseName.Story4);
                case 41: return (VenueName.Lightning, VenueID.LIG, CourseName.Story5);
                case 42: return (VenueName.PortTown, VenueID.POR_S, CourseName.Story6);
                case 43: return (VenueName.MuteCity, VenueID.MUT, CourseName.Story7);
                case 44: return (VenueName.FireField, VenueID.FIR_S, CourseName.Story8);
                case 45: return (VenueName.PhantomRoad, VenueID.RAI, CourseName.Story9);

                #endregion

                #region GRAND PRIX / VS WIN

                case 49: return (VenueName.MuteCity, VenueID.WIN_GX, CourseName.GrandPrixPodium);
                case 50: return (VenueName.MuteCity, VenueID.WIN, CourseName.VictoryLap);

                #endregion

                #region UNUSED GX STAGES

                case 0: return (VenueName.SandOcean, VenueID.SAN, CourseName.ScrewDrive);

                #endregion

                #region UNUSED AX STAGES

                case 72: return (VenueName.None, VenueID.None, CourseName.SurfaceSlide);
                ////
                case 77: return (VenueName.None, VenueID.None, CourseName.LoopCross);
                ////
                case 86: return (VenueName.None, VenueID.None, CourseName.MeteorStream);
                case 87: return (VenueName.None, VenueID.None, CourseName.CylinderWave);
                ////
                case 90: return (VenueName.None, VenueID.None, CourseName.LongPipe);
                case 91: return (VenueName.None, VenueID.None, CourseName.Story2);
                case 92: return (VenueName.None, VenueID.None, CourseName.Story3);
                case 93: return (VenueName.None, VenueID.None, CourseName.Story4);
                case 94: return (VenueName.None, VenueID.None, CourseName.Story5);
                case 95: return (VenueName.None, VenueID.None, CourseName.Story6);
                case 96: return (VenueName.None, VenueID.None, CourseName.Story7);
                case 97: return (VenueName.None, VenueID.None, CourseName.Story8);
                case 98: return (VenueName.None, VenueID.None, CourseName.Story9);
                ////
                case 101: return (VenueName.None, VenueID.None, CourseName.TwistRoad);
                case 102: return (VenueName.None, VenueID.None, CourseName.TwistRoad);
                case 103: return (VenueName.None, VenueID.None, CourseName.Multiplex);
                case 104: return (VenueName.None, VenueID.None, CourseName.Intersection);
                case 105: return (VenueName.None, VenueID.None, CourseName.Undulation);
                ////
                case 107: return (VenueName.None, VenueID.None, CourseName.DriftHighway);
                case 108: return (VenueName.None, VenueID.None, CourseName.AeroDive);
                case 109: return (VenueName.None, VenueID.None, CourseName.LateralShift);

                #endregion

                #region UNUSED VENUE DEFINED

                // Unused Mute City
                case 2:
                case 4:
                    return (VenueName.MuteCity, VenueID.MUT, CourseName.None);

                // Unused Port Town
                case 6:
                    return (VenueName.PortTown, VenueID.POR, CourseName.None);

                // Unused Lighting
                case 12:
                case 23:
                    return (VenueName.Lightning, VenueID.LIG, CourseName.None);

                // Unused Fire Field
                case 18:
                    return (VenueName.FireField, VenueID.FIR, CourseName.None);

                // Unused Outer Space
                case 19:
                case 20:
                    return (VenueName.OuterSpace, VenueID.MET, CourseName.None);

                // Unused Cosmo Terminal
                case 22:
                    return (VenueName.CosmoTerminal, VenueID.ELE, CourseName.None);

                // Unused Sand Ocean
                case 30:
                    return (VenueName.SandOcean, VenueID.SAN, CourseName.None);

                #endregion

                default:
                    return (VenueName.None, VenueID.None, CourseName.None);
            }
        }

        //public static VenueID GetVenueID(int index)
        //{
        //    switch (index)
        //    {
        //        // MUT == Mute City
        //        case 1:
        //        case 2:
        //        case 3:
        //        case 4:
        //            return VenueID.MUT;

        //        // SAN == Sand Ocean
        //        case 0:
        //        case 25:
        //        case 26:
        //        case 30:
        //            return VenueID.SAN;

        //        // TOW = Aeropolis
        //        case 5:
        //        case 21:
        //        case 31:
        //            return VenueID.TOW;

        //        // POR == Port Town
        //        case 6:
        //        case 7:
        //        case 13:
        //        case 33:
        //            return VenueID.POR;

        //        // LIG == Lightning
        //        case 8:
        //        case 9:
        //        case 12:
        //        case 23:
        //        case 34:
        //            return VenueID.LIG;

        //        // FOR == Green Plant
        //        case 10:
        //        case 11:
        //        case 35:
        //            return VenueID.FOR;

        //        // BIG == Big Blue
        //        case 14:
        //        case 27:
        //            return VenueID.BIG;

        //        // FIR == Fire Field
        //        case 15:
        //        case 17:
        //        case 18:
        //            return VenueID.FIR;

        //        // CAS == Casino Palace
        //        case 16:
        //        case 29:
        //            return VenueID.CAS;

        //        // MET == Outer Space
        //        case 19:
        //        case 20:
        //        case 32:
        //            return VenueID.MET;

        //        // ELE == Cosmo Terminal
        //        case 22:
        //        case 24:
        //            return VenueID.ELE;

        //        // RAI == Phantom Road
        //        case 28:
        //            return VenueID.RAI;

        //        // COM == Mute City / Casino Palace hybrid
        //        case 36:
        //            return VenueID.COM;

        //        // Story venues
        //        case 37: return VenueID.COM_S;
        //        case 38: return VenueID.SAN_S;
        //        case 39: return VenueID.CAS;
        //        case 40: return VenueID.BIG_S;
        //        case 41: return VenueID.LIG;
        //        case 42: return VenueID.POR_S;
        //        case 43: return VenueID.MUT;
        //        case 44: return VenueID.FIR_S;
        //        case 45: return VenueID.RAI;

        //        // Victory screens
        //        case 49: return VenueID.WIN;
        //        case 50: return VenueID.WIN_GX;

        //        default:
        //            return VenueID.None;
        //    }
        //}

        public static VenueID GetVenueID(int index)
        {
            return index switch
            {
                00 => VenueID.SAN, // Unused Twist Road leftovers
                01 => VenueID.MUT, // Mute City [Twist Road]
                02 => VenueID.MUT, 
                03 => VenueID.MUT, // Mute City [Serial Gaps]
                04 => VenueID.MUT,
                05 => VenueID.TOW, // Aeropolis [Multiplex]
                06 => VenueID.POR, 
                07 => VenueID.POR, // Port Town [Aero Dive]
                08 => VenueID.LIG, // Lightning [Loop Cross]
                09 => VenueID.LIG, // Lightning [Half-Pipe]
                10 => VenueID.FOR, // Green Plant [Intersection]
                11 => VenueID.FOR, // Green Plant [Mobius Ring]
                12 => VenueID.LIG,
                13 => VenueID.POR, // Port Town [Long Pipe]
                14 => VenueID.BIG, // Big Blue [Drift Highway]
                15 => VenueID.FIR, // Fire Field [Cylinder Knot]
                16 => VenueID.CAS, // Casino Palace [Split Oval]
                17 => VenueID.FIR, // Fire Field [Undulation]
                18 => VenueID.FIR,
                19 => VenueID.MET,
                20 => VenueID.MET,
                21 => VenueID.TOW, // Aeropolis [Dragon Slope]
                22 => VenueID.ELE,
                23 => VenueID.LIG,
                24 => VenueID.ELE, // Cosmo Terminal [Trident]
                25 => VenueID.SAN, // Sand Ocean [Lateral Shift]
                26 => VenueID.SAN, // Sand Ocean [Surface Slide]
                27 => VenueID.BIG, // Big Blue [Ordeal]
                28 => VenueID.RAI, // Phantom Road [Slim-line Slits]
                29 => VenueID.CAS, // Casino Palace [Double Branches]
                30 => VenueID.SAN, 
                31 => VenueID.TOW, // Aeropolis [Screw Drive]
                32 => VenueID.MET, // Outer Space [Meteor Stream]
                33 => VenueID.POR, // Port Town [Cylinder Wave]
                34 => VenueID.LIG, // Lightning [Thunder Road]
                35 => VenueID.FOR, // Green Plant [Sprial]
                36 => VenueID.COM, // Mute City [Sonic Oval]
                37 => VenueID.COM_S, // Story 1: Captain Falcon Trains
                38 => VenueID.SAN_S, // Story 2: Goroh: The Vengeful Samurai
                39 => VenueID.CAS,   // Story 3: High Stakes in Mute City
                40 => VenueID.BIG_S, // Story 4: Challenge of the Bloody Chain
                41 => VenueID.LIG,   // Story 5: Save Jody Summer!
                42 => VenueID.POR_S, // Story 6: Black Shadow's Trap
                43 => VenueID.MUT,   // Story 7: The F-Zero Grand Prix
                44 => VenueID.FIR_S, // Story 8: Secrets of the Champion Belt
                45 => VenueID.RAI,   // Story 9: Finale: Enter The Creators

                49 => VenueID.WIN,    // Grand Prix Podium
                50 => VenueID.WIN_GX, // Victory Lap

                _ => VenueID.None, // For all other indices, there is no venue
            };
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
