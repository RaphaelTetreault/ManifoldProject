namespace GameCube.GFZ
{
    public static class CourseUtility
    {
        /// <summary>
        /// Returns tuple of (CourseVenue, CourseVenueID, CourseNameAX) from both F-Zero GX
        /// and F-Zero AX for any existing course indices.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static (VenueName venue, VenueID venueID, CourseIndexAX name) GetCourseInfo(int index)
        {
            switch (index)
            {
                #region GX STAGES

                case 1: return (VenueName.MuteCity, VenueID.MUT, CourseIndexAX.TwistRoad);
                ////
                case 3: return (VenueName.MuteCity, VenueID.MUT, CourseIndexAX.SerialGaps);
                ////
                case 5: return (VenueName.Aeropolis, VenueID.TOW, CourseIndexAX.Multiplex);
                ////
                case 7: return (VenueName.PortTown, VenueID.POR, CourseIndexAX.AeroDive);
                case 8: return (VenueName.Lightning, VenueID.LIG, CourseIndexAX.LoopCross);
                case 9: return (VenueName.Lightning, VenueID.LIG, CourseIndexAX.HalfPipe);
                case 10: return (VenueName.GreenPlant, VenueID.FOR, CourseIndexAX.Intersection);
                case 11: return (VenueName.GreenPlant, VenueID.FOR, CourseIndexAX.MobiusRing);
                ////
                case 13: return (VenueName.PortTown, VenueID.POR, CourseIndexAX.LongPipe);
                case 14: return (VenueName.BigBlue, VenueID.BIG, CourseIndexAX.DriftHighway);
                case 15: return (VenueName.FireField, VenueID.FIR, CourseIndexAX.CylinderKnot);
                case 16: return (VenueName.CasinoPalace, VenueID.CAS, CourseIndexAX.SplitOval);
                case 17: return (VenueName.FireField, VenueID.FIR, CourseIndexAX.Undulation);
                ////
                case 21: return (VenueName.Aeropolis, VenueID.TOW, CourseIndexAX.DragonSlope);
                ////
                case 24: return (VenueName.CosmoTerminal, VenueID.ELE, CourseIndexAX.Trident);
                case 25: return (VenueName.SandOcean, VenueID.SAN, CourseIndexAX.LateralShift);
                case 26: return (VenueName.SandOcean, VenueID.SAN, CourseIndexAX.SurfaceSlide);
                case 27: return (VenueName.BigBlue, VenueID.BIG, CourseIndexAX.Ordeal);
                case 28: return (VenueName.PhantomRoad, VenueID.RAI, CourseIndexAX.SlimLineSlits);
                case 29: return (VenueName.CasinoPalace, VenueID.CAS, CourseIndexAX.DoubleBranches);

                #endregion

                #region AX STAGES

                case 31: return (VenueName.Aeropolis, VenueID.TOW, CourseIndexAX.ScrewDrive);
                case 32: return (VenueName.OuterSpace, VenueID.MET, CourseIndexAX.MeteorStream);
                case 33: return (VenueName.PortTown, VenueID.POR, CourseIndexAX.CylinderWave);
                case 34: return (VenueName.Lightning, VenueID.LIG, CourseIndexAX.ThunderRoad);
                case 35: return (VenueName.GreenPlant, VenueID.FOR, CourseIndexAX.Spiral);
                case 36: return (VenueName.MuteCity, VenueID.COM, CourseIndexAX.SonicOval);

                #endregion

                #region STORY

                case 37: return (VenueName.MuteCity, VenueID.COM_S, CourseIndexAX.Story1);
                case 38: return (VenueName.SandOcean, VenueID.SAN_S, CourseIndexAX.Story2);
                case 39: return (VenueName.CasinoPalace, VenueID.CAS, CourseIndexAX.Story3);
                case 40: return (VenueName.BigBlue, VenueID.BIG_S, CourseIndexAX.Story4);
                case 41: return (VenueName.Lightning, VenueID.LIG, CourseIndexAX.Story5);
                case 42: return (VenueName.PortTown, VenueID.POR_S, CourseIndexAX.Story6);
                case 43: return (VenueName.MuteCity, VenueID.MUT, CourseIndexAX.Story7);
                case 44: return (VenueName.FireField, VenueID.FIR_S, CourseIndexAX.Story8);
                case 45: return (VenueName.PhantomRoad, VenueID.RAI, CourseIndexAX.Story9);

                #endregion

                #region GRAND PRIX / VS WIN

                case 49: return (VenueName.MuteCity, VenueID.WIN_GX, CourseIndexAX.GrandPrixPodium);
                case 50: return (VenueName.MuteCity, VenueID.WIN, CourseIndexAX.VictoryLap);

                #endregion

                #region UNUSED GX STAGES

                case 0: return (VenueName.SandOcean, VenueID.SAN, CourseIndexAX.TestScrewDrive);

                #endregion

                #region UNUSED AX STAGES

                case 72: return (VenueName.None, VenueID.None, CourseIndexAX.TestSurfaceSlide);
                ////
                case 77: return (VenueName.None, VenueID.None, CourseIndexAX.TestLoopCross);
                ////
                case 86: return (VenueName.None, VenueID.None, CourseIndexAX.TestMeteorStream);
                case 87: return (VenueName.None, VenueID.None, CourseIndexAX.TestCylinderWave);
                ////
                case 90: return (VenueName.None, VenueID.None, CourseIndexAX.TestLongPipe);
                case 91: return (VenueName.None, VenueID.None, CourseIndexAX.TestStory2);
                case 92: return (VenueName.None, VenueID.None, CourseIndexAX.TestStory3);
                case 93: return (VenueName.None, VenueID.None, CourseIndexAX.TestStory4);
                case 94: return (VenueName.None, VenueID.None, CourseIndexAX.TestStory5);
                case 95: return (VenueName.None, VenueID.None, CourseIndexAX.TestStory6);
                case 96: return (VenueName.None, VenueID.None, CourseIndexAX.TestStory7);
                case 97: return (VenueName.None, VenueID.None, CourseIndexAX.TestStory8);
                case 98: return (VenueName.None, VenueID.None, CourseIndexAX.TestStory9);
                ////
                case 101: return (VenueName.None, VenueID.None, CourseIndexAX.TestTwistRoadOld);
                case 102: return (VenueName.None, VenueID.None, CourseIndexAX.TestTwistRoad);
                case 103: return (VenueName.None, VenueID.None, CourseIndexAX.TestMultiplex);
                case 104: return (VenueName.None, VenueID.None, CourseIndexAX.TestIntersection);
                case 105: return (VenueName.None, VenueID.None, CourseIndexAX.TestUndulation);
                ////
                case 107: return (VenueName.None, VenueID.None, CourseIndexAX.TestDriftHighway);
                case 108: return (VenueName.None, VenueID.None, CourseIndexAX.TestAeroDive);
                case 109: return (VenueName.None, VenueID.None, CourseIndexAX.TestLateralShift);

                #endregion

                #region UNUSED VENUE DEFINED

                // Unused Mute City
                case 2:
                case 4:
                    return (VenueName.MuteCity, VenueID.MUT, CourseIndexAX.None);

                // Unused Port Town
                case 6:
                    return (VenueName.PortTown, VenueID.POR, CourseIndexAX.None);

                // Unused Lighting
                case 12:
                case 23:
                    return (VenueName.Lightning, VenueID.LIG, CourseIndexAX.None);

                // Unused Fire Field
                case 18:
                    return (VenueName.FireField, VenueID.FIR, CourseIndexAX.None);

                // Unused Outer Space
                case 19:
                case 20:
                    return (VenueName.OuterSpace, VenueID.MET, CourseIndexAX.None);

                // Unused Cosmo Terminal
                case 22:
                    return (VenueName.CosmoTerminal, VenueID.ELE, CourseIndexAX.None);

                // Unused Sand Ocean
                case 30:
                    return (VenueName.SandOcean, VenueID.SAN, CourseIndexAX.None);

                #endregion

                default:
                    return (VenueName.None, VenueID.None, CourseIndexAX.None);
            }
        }

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

        public static Venue GetVenue(int index)
        {
            return index switch
            {
                00 => Venue.SandOcean,      // Sand Ocean [Screw Drive] (unused)
                01 => Venue.MuteCity,       // Mute City [Twist Road]
                02 => Venue.MuteCity,
                03 => Venue.MuteCity,       // Mute City [Serial Gaps]
                04 => Venue.MuteCity,
                05 => Venue.Aeropolis,      // Aeropolis [Multiplex]
                06 => Venue.PortTown,
                07 => Venue.PortTown,       // Port Town [Aero Dive]
                08 => Venue.Lightning,      // Lightning [Loop Cross]
                09 => Venue.Lightning,      // Lightning [Half-Pipe]
                10 => Venue.GreenPlant,     // Green Plant [Intersection]
                11 => Venue.GreenPlant,     // Green Plant [Mobius Ring]
                12 => Venue.Lightning,
                13 => Venue.PortTown,       // Port Town [Long Pipe]
                14 => Venue.BigBlue,        // Big Blue [Drift Highway]
                15 => Venue.FireField,      // Fire Field [Cylinder Knot]
                16 => Venue.CasinoPalace,   // Casino Palace [Split Oval]
                17 => Venue.FireField,      // Fire Field [Undulation]
                18 => Venue.FireField,
                19 => Venue.OuterSpace,
                20 => Venue.OuterSpace,
                21 => Venue.Aeropolis,      // Aeropolis [Dragon Slope]
                22 => Venue.CosmoTerminal,
                23 => Venue.Lightning,
                24 => Venue.CosmoTerminal,  // Cosmo Terminal [Trident]
                25 => Venue.SandOcean,      // Sand Ocean [Lateral Shift]
                26 => Venue.SandOcean,      // Sand Ocean [Surface Slide]
                27 => Venue.BigBlue,        // Big Blue [Ordeal]
                28 => Venue.PhantomRoad,    // Phantom Road [Slim-line Slits]
                29 => Venue.CasinoPalace,   // Casino Palace [Double Branches]
                30 => Venue.SandOcean,
                31 => Venue.Aeropolis,      // Aeropolis [Screw Drive]
                32 => Venue.OuterSpace,     // Outer Space [Meteor Stream]
                33 => Venue.PortTown,       // Port Town [Cylinder Wave]
                34 => Venue.Lightning,      // Lightning [Thunder Road]
                35 => Venue.GreenPlant,     // Green Plant [Sprial]
                36 => Venue.MuteCityCOM,    // Mute City [Sonic Oval]
                37 => Venue.StoryMuteCityCOM, // Story 1: Captain Falcon Trains
                38 => Venue.StorySandOcean, // Story 2: Goroh: The Vengeful Samurai
                39 => Venue.CasinoPalace,   // Story 3: High Stakes in Mute City
                40 => Venue.StoryBigBlue,   // Story 4: Challenge of the Bloody Chain
                41 => Venue.Lightning,      // Story 5: Save Jody Summer!
                42 => Venue.StoryPortTown,  // Story 6: Black Shadow's Trap
                43 => Venue.MuteCity,       // Story 7: The F-Zero Grand Prix
                44 => Venue.StoryFireField, // Story 8: Secrets of the Champion Belt
                45 => Venue.PhantomRoad,    // Story 9: Finale: Enter The Creators

                49 => Venue.VictoryLap,     // Grand Prix Podium
                50 => Venue.GrandPrixPodium, // Victory Lap

                _ => Venue.None, // For all other indices, there is no venue
            };
        }

        public static CourseIndexGX GetCourseNameGX(int index)
        {
            var courseID = (CourseIndexGX)index;
            var notDefined = !System.Enum.IsDefined(typeof(CourseIndexGX), courseID);
            if (notDefined)
            {
                courseID = CourseIndexGX.None;
            }
            return courseID;
        }

        public static CourseIndexAX GetCourseNameAX(int index)
        {
            var courseID = (CourseIndexAX)index;
            var notDefined = !System.Enum.IsDefined(typeof(CourseIndexAX), courseID);
            if (notDefined)
            {
                courseID = CourseIndexAX.None;
            }
            return courseID;
        }

        public static string GetCourseName(int index)
        {
            var value = GetCourseNameAX(index);
            var name = Manifold.EnumExtensions.GetDescription(value);
            return name;
        }

        public static string GetVenueName(int index)
        {
            var value = GetVenue(index);
            var name = Manifold.EnumExtensions.GetDescription(value);
            return name;
        }


    }
}
