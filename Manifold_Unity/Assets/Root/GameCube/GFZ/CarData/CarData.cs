using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CarData
{
    [Serializable]
    public class CarData :
        IBinarySerializable,
        IFileType
    {
        // CONSTANTS
        // Numbers of things
        public const int MachineCount = 41;
        public const int BodyCount = 25;
        public const int CockpitCount = 25;
        public const int BoosterCount = 25;
        // Sizes of things
        public const int kPaddingSize = 12;
        public const int kMachineNameTable = 43;
        public const int kPartsInternalTable = 32;


        // FIELDS
        // String table
        private byte[] padding; // 12 bytes
        public ShiftJisCString[] machineNames;
        public ShiftJisCString[] partsInternalNames;
        // Vehicles
        public VehicleParameters DarkSchneider;
        public VehicleParameters RedGazelle;
        public VehicleParameters WhiteCat;
        public VehicleParameters GoldenFox;
        public VehicleParameters IronTiger;
        public VehicleParameters FireStingray;
        public VehicleParameters WildGoose;
        public VehicleParameters BlueFalcon;
        public VehicleParameters DeepClaw;
        public VehicleParameters GreatStar;
        public VehicleParameters LittleWyvern;
        public VehicleParameters MadWolf;
        public VehicleParameters SuperPiranha;
        public VehicleParameters DeathAnchor;
        public VehicleParameters AstroRobin;
        public VehicleParameters BigFang;
        public VehicleParameters SonicPhantom;
        public VehicleParameters GreenPanther;
        public VehicleParameters HyperSpeeder;
        public VehicleParameters SpaceAngler;
        public VehicleParameters KingMeteor;
        public VehicleParameters QueenMeteor;
        public VehicleParameters TwinNoritta;
        public VehicleParameters NightThunder;
        public VehicleParameters WildBoar;
        public VehicleParameters BloodHawk;
        public VehicleParameters WonderWasp;
        public VehicleParameters MightyTyphoon;
        public VehicleParameters MightyHurricane;
        public VehicleParameters CrazyBear;
        public VehicleParameters BlackBull;
        public VehicleParameters FatShark;
        public VehicleParameters CosmicDolphin;
        public VehicleParameters PinkSpider;
        public VehicleParameters MagicSeagull;
        public VehicleParameters SilverRat;
        public VehicleParameters SparkMoon;
        public VehicleParameters BunnyFlash;
        public VehicleParameters GroovyTaxi;
        public VehicleParameters RollingTurtle;
        public VehicleParameters RainbowPhoenix;
        // Body Parts
        public VehicleParameters BraveEagle;
        public VehicleParameters GalaxyFalcon;
        public VehicleParameters GiantPlanet;
        public VehicleParameters MegaloCruiser;
        public VehicleParameters SplashWhale;
        public VehicleParameters WildChariot;
        public VehicleParameters ValiantJaguar;
        public VehicleParameters HolySpider;
        public VehicleParameters BloodRaven;
        public VehicleParameters FunnySwallow;
        public VehicleParameters OpticalWing;
        public VehicleParameters MadBull;
        public VehicleParameters BigTyrant;
        public VehicleParameters GrandBase;
        public VehicleParameters FireWolf;
        public VehicleParameters DreadHammer;
        public VehicleParameters SilverSword;
        public VehicleParameters RageKnight;
        public VehicleParameters RapidBarrel;
        public VehicleParameters SkyHorse;
        public VehicleParameters AquaGoose;
        public VehicleParameters SpaceCancer;
        public VehicleParameters MetalShell;
        public VehicleParameters SpeedyDragon;
        public VehicleParameters LibertyManta;
        // Cockpit parts
        public VehicleParameters WonderWorm;
        public VehicleParameters RushCyclone;
        public VehicleParameters CombatCannon;
        public VehicleParameters MuscleGorilla;
        public VehicleParameters CyberFox;
        public VehicleParameters HeatSnake;
        public VehicleParameters RaveDrifter;
        public VehicleParameters AerialBullet;
        public VehicleParameters SparkBird;
        public VehicleParameters BlastCamel;
        public VehicleParameters DarkChaser;
        public VehicleParameters GarnetPhantom;
        public VehicleParameters BrightSpear;
        public VehicleParameters HyperStream;
        public VehicleParameters SuperLynx;
        public VehicleParameters CrystalEgg;
        public VehicleParameters WindyShark;
        public VehicleParameters RedRex;
        public VehicleParameters SonicSoldier;
        public VehicleParameters MaximumStar;
        public VehicleParameters MoonSnail;
        public VehicleParameters CrazyBuffalo;
        public VehicleParameters ScudViper;
        public VehicleParameters RoundDisk;
        public VehicleParameters EnergyCrest;
        // Booster parts
        public VehicleParameters Euros_01;
        public VehicleParameters Triangle_GT;
        public VehicleParameters Velocity_J;
        public VehicleParameters Sunrise140;
        public VehicleParameters Saturn_SG;
        public VehicleParameters Bluster_X;
        public VehicleParameters Devilfish_RX;
        public VehicleParameters Mars_EX;
        public VehicleParameters Titan_G4;
        public VehicleParameters Extreme_ZZ;
        public VehicleParameters Thunderbolt_V2;
        public VehicleParameters Boxer_2C;
        public VehicleParameters Shuttle_M2;
        public VehicleParameters Punisher_4X;
        public VehicleParameters Scorpion_R;
        public VehicleParameters Raiden_88;
        public VehicleParameters Impulse220;
        public VehicleParameters Bazooka_YS;
        public VehicleParameters Meteor_RR;
        public VehicleParameters Tiger_RZ;
        public VehicleParameters Hornet_FX;
        public VehicleParameters Jupiter_Q;
        public VehicleParameters Comet_V;
        public VehicleParameters Crown_77;
        public VehicleParameters Triple_Z;


        // Properties
        public string FileName { get; set; }

        public string FileExtension => "";

        /// <summary>
        /// Returns all machines in internal strucuture order (X, GX, AX)
        /// </summary>
        public VehicleParameters[] MachinesInternalOrder => new VehicleParameters[]
        {
            RedGazelle,
            WhiteCat,
            GoldenFox,
            IronTiger,
            FireStingray,
            WildGoose,
            BlueFalcon,
            DeepClaw,
            GreatStar,
            LittleWyvern,
            MadWolf,
            SuperPiranha,
            DeathAnchor,
            AstroRobin,
            BigFang,
            SonicPhantom,
            GreenPanther,
            HyperSpeeder,
            SpaceAngler,
            KingMeteor,
            QueenMeteor,
            TwinNoritta,
            NightThunder,
            WildBoar,
            BloodHawk,
            WonderWasp,
            MightyTyphoon,
            MightyHurricane,
            CrazyBear,
            BlackBull,
            DarkSchneider,
            FatShark,
            CosmicDolphin,
            PinkSpider,
            MagicSeagull,
            SilverRat,
            SparkMoon,
            BunnyFlash,
            GroovyTaxi,
            RollingTurtle,
            RainbowPhoenix,
        };

        /// <summary>
        /// Returns all body parts in internal order.
        /// </summary>
        public VehicleParameters[] BodyParts => new VehicleParameters[]
        {
            BraveEagle,
            GalaxyFalcon,
            GiantPlanet,
            MegaloCruiser,
            SplashWhale,
            WildChariot,
            ValiantJaguar,
            HolySpider,
            BloodRaven,
            FunnySwallow,
            OpticalWing,
            MadBull,
            BigTyrant,
            GrandBase,
            FireWolf,
            DreadHammer,
            SilverSword,
            RageKnight,
            RapidBarrel,
            SkyHorse,
            AquaGoose,
            SpaceCancer,
            MetalShell,
            SpeedyDragon,
            LibertyManta,
        };

        /// <summary>
        /// Returns all cockpoit parts in internal order.
        /// </summary>
        public VehicleParameters[] CockpitParts => new VehicleParameters[]
        {
            WonderWorm,
            RushCyclone,
            CombatCannon,
            MuscleGorilla,
            CyberFox,
            HeatSnake,
            RaveDrifter,
            AerialBullet,
            SparkBird,
            BlastCamel,
            DarkChaser,
            GarnetPhantom,
            BrightSpear,
            HyperStream,
            SuperLynx,
            CrystalEgg,
            WindyShark,
            RedRex,
            SonicSoldier,
            MaximumStar,
            MoonSnail,
            CrazyBuffalo,
            ScudViper,
            RoundDisk,
            EnergyCrest,
        };

        /// <summary>
        /// 
        /// </summary>
        public VehicleParameters[] BoosterParts => new VehicleParameters[]
        {
            Euros_01,
            Triangle_GT,
            Velocity_J,
            Sunrise140,
            Saturn_SG,
            Bluster_X,
            Devilfish_RX,
            Mars_EX,
            Titan_G4,
            Extreme_ZZ,
            Thunderbolt_V2,
            Boxer_2C,
            Shuttle_M2,
            Punisher_4X,
            Scorpion_R,
            Raiden_88,
            Impulse220,
            Bazooka_YS,
            Meteor_RR,
            Tiger_RZ,
            Hornet_FX,
            Jupiter_Q,
            Comet_V,
            Crown_77,
            Triple_Z,
        };

        /// <summary>
        /// 
        /// </summary>
        public static readonly ShiftJisCString[] InternalPartNamesTable = new ShiftJisCString[]
        {
            "GC-D3",
            "GC-E2",
            "AC-B2",
            "AC-C1",
            "GC-C1",
            "GC-C2",
            "GC-C4",
            "GC-D2",
            "AC-C2",
            "AC-A2",
            "GC-B2",
            "GC-A2",
            "GC-A1",
            "AC-B3",
            "AC-D2",
            "GC-D1",
            "GC-C3",
            "AC-C3",
            "GC-B1",
            "AC-B4",
            "AC-E1",
            "AC-A1",
            "AC-B1",
            "GC-E1",
            "AC-D1",
            "AC-C4",
            "GC-B3",
            "AC-E2",
            "GC-A3",
            "AC-D3",
        };

        /// <summary>
        /// Ordered as they are in the file
        /// </summary>
        public static ShiftJisCString[] MachineNamesTable = new ShiftJisCString[]
        {
            "Rainbow Phoenix",
            "Rolling Turtle",
            "Groovy Taxi",
            "Bunny Flash",
            "Spark Moon",
            "Silver Rat",
            "Magic Seagull",
            "Pink Spider",
            "Cosmic Dolphin",
            "Fat Shark",
            "Dark Schneider",
            "Black Bull",
            "Crazy Bear",
            "Mighty Hurricane",
            "Mighty Typhoon",
            "Wonder Wasp",
            "Blood Hawk",
            "Wild Boar",
            "Night Thunder",
            "Twin Noritta",
            "Queen Meteor",
            "King Meteor",
            "Space Angler",
            "Hyper Speeder",
            "Green Panther",
            "Sonic Phantom",
            "Big Fang",
            "Astro Robin",
            "Death Anchor",
            "Super Piranha",
            "Mad Wolf",
            "Little Wyvern",
            "Great Star",
            "Deep Claw",
            "Blue Falcon",
            "Wild Goose",
            "Fire Stingray",
            "Iron Tiger",
            "Golden Fox",
            "White Cat",
            "Red Gazelle",
        };


        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianness(Endianness.LittleEndian);

            reader.ReadX(ref RedGazelle);
            reader.ReadX(ref WhiteCat);
            reader.ReadX(ref GoldenFox);
            reader.ReadX(ref IronTiger);
            reader.ReadX(ref FireStingray);
            reader.ReadX(ref WildGoose);
            reader.ReadX(ref BlueFalcon);
            reader.ReadX(ref DeepClaw);
            reader.ReadX(ref GreatStar);
            reader.ReadX(ref LittleWyvern);
            reader.ReadX(ref MadWolf);
            reader.ReadX(ref SuperPiranha);
            reader.ReadX(ref DeathAnchor);
            reader.ReadX(ref AstroRobin);
            reader.ReadX(ref BigFang);
            reader.ReadX(ref SonicPhantom);
            reader.ReadX(ref GreenPanther);
            reader.ReadX(ref HyperSpeeder);
            reader.ReadX(ref SpaceAngler);
            reader.ReadX(ref KingMeteor);
            reader.ReadX(ref QueenMeteor);
            reader.ReadX(ref TwinNoritta);
            reader.ReadX(ref NightThunder);
            reader.ReadX(ref WildBoar);
            reader.ReadX(ref BloodHawk);
            reader.ReadX(ref WonderWasp);
            reader.ReadX(ref MightyTyphoon);
            reader.ReadX(ref MightyHurricane);
            reader.ReadX(ref CrazyBear);
            reader.ReadX(ref BlackBull);
            reader.ReadX(ref DarkSchneider);
            reader.ReadX(ref FatShark);
            reader.ReadX(ref CosmicDolphin);
            reader.ReadX(ref PinkSpider);
            reader.ReadX(ref MagicSeagull);
            reader.ReadX(ref SilverRat);
            reader.ReadX(ref SparkMoon);
            reader.ReadX(ref BunnyFlash);
            reader.ReadX(ref GroovyTaxi);
            reader.ReadX(ref RollingTurtle);
            reader.ReadX(ref RainbowPhoenix);

            // Read some padding
            reader.ReadX(ref padding, kPaddingSize);
            foreach (var @byte in padding)
                Assert.IsTrue(@byte == 0);

            // 2022/01/28: I may have broken this. Used to be for-loop
            // manually assigning each value from length-init array.
            reader.ReadX(ref machineNames, kMachineNameTable);

            // Body parts
            reader.ReadX(ref BraveEagle);
            reader.ReadX(ref GalaxyFalcon);
            reader.ReadX(ref GiantPlanet);
            reader.ReadX(ref MegaloCruiser);
            reader.ReadX(ref SplashWhale);
            reader.ReadX(ref WildChariot);
            reader.ReadX(ref ValiantJaguar);
            reader.ReadX(ref HolySpider);
            reader.ReadX(ref BloodRaven);
            reader.ReadX(ref FunnySwallow);
            reader.ReadX(ref OpticalWing);
            reader.ReadX(ref MadBull);
            reader.ReadX(ref BigTyrant);
            reader.ReadX(ref GrandBase);
            reader.ReadX(ref FireWolf);
            reader.ReadX(ref DreadHammer);
            reader.ReadX(ref SilverSword);
            reader.ReadX(ref RageKnight);
            reader.ReadX(ref RapidBarrel);
            reader.ReadX(ref SkyHorse);
            reader.ReadX(ref AquaGoose);
            reader.ReadX(ref SpaceCancer);
            reader.ReadX(ref MetalShell);
            reader.ReadX(ref SpeedyDragon);
            reader.ReadX(ref LibertyManta);

            // Cockpit parts
            reader.ReadX(ref WonderWorm);
            reader.ReadX(ref RushCyclone);
            reader.ReadX(ref CombatCannon);
            reader.ReadX(ref MuscleGorilla);
            reader.ReadX(ref CyberFox);
            reader.ReadX(ref HeatSnake);
            reader.ReadX(ref RaveDrifter);
            reader.ReadX(ref AerialBullet);
            reader.ReadX(ref SparkBird);
            reader.ReadX(ref BlastCamel);
            reader.ReadX(ref DarkChaser);
            reader.ReadX(ref GarnetPhantom);
            reader.ReadX(ref BrightSpear);
            reader.ReadX(ref HyperStream);
            reader.ReadX(ref SuperLynx);
            reader.ReadX(ref CrystalEgg);
            reader.ReadX(ref WindyShark);
            reader.ReadX(ref RedRex);
            reader.ReadX(ref SonicSoldier);
            reader.ReadX(ref MaximumStar);
            reader.ReadX(ref MoonSnail);
            reader.ReadX(ref CrazyBuffalo);
            reader.ReadX(ref ScudViper);
            reader.ReadX(ref RoundDisk);
            reader.ReadX(ref EnergyCrest);

            // Booster parts
            reader.ReadX(ref Euros_01);
            reader.ReadX(ref Triangle_GT);
            reader.ReadX(ref Velocity_J);
            reader.ReadX(ref Sunrise140);
            reader.ReadX(ref Saturn_SG);
            reader.ReadX(ref Bluster_X);
            reader.ReadX(ref Devilfish_RX);
            reader.ReadX(ref Mars_EX);
            reader.ReadX(ref Titan_G4);
            reader.ReadX(ref Extreme_ZZ);
            reader.ReadX(ref Thunderbolt_V2);
            reader.ReadX(ref Boxer_2C);
            reader.ReadX(ref Shuttle_M2);
            reader.ReadX(ref Punisher_4X);
            reader.ReadX(ref Scorpion_R);
            reader.ReadX(ref Raiden_88);
            reader.ReadX(ref Impulse220);
            reader.ReadX(ref Bazooka_YS);
            reader.ReadX(ref Meteor_RR);
            reader.ReadX(ref Tiger_RZ);
            reader.ReadX(ref Hornet_FX);
            reader.ReadX(ref Jupiter_Q);
            reader.ReadX(ref Comet_V);
            reader.ReadX(ref Crown_77);
            reader.ReadX(ref Triple_Z);

            BinaryIoUtility.PushEndianness(Endianness.LittleEndian);
            {
                // 2022/01/28: I may have broken this. Used to be for-loop
                // manually assigning each value from length-init array.
                reader.ReadX(ref partsInternalNames, kPartsInternalTable);
            }
            BinaryIoUtility.PopEndianness();

            BinaryIoUtility.PopEndianness();
        }

        public void Serialize(BinaryWriter writer)
        {
            BinaryIoUtility.PushEndianness(Endianness.BigEndian);

            // Machines
            writer.WriteX(RedGazelle);
            writer.WriteX(WhiteCat);
            writer.WriteX(GoldenFox);
            writer.WriteX(IronTiger);
            writer.WriteX(FireStingray);
            writer.WriteX(WildGoose);
            writer.WriteX(BlueFalcon);
            writer.WriteX(DeepClaw);
            writer.WriteX(GreatStar);
            writer.WriteX(LittleWyvern);
            writer.WriteX(MadWolf);
            writer.WriteX(SuperPiranha);
            writer.WriteX(DeathAnchor);
            writer.WriteX(AstroRobin);
            writer.WriteX(BigFang);
            writer.WriteX(SonicPhantom);
            writer.WriteX(GreenPanther);
            writer.WriteX(HyperSpeeder);
            writer.WriteX(SpaceAngler);
            writer.WriteX(KingMeteor);
            writer.WriteX(QueenMeteor);
            writer.WriteX(TwinNoritta);
            writer.WriteX(NightThunder);
            writer.WriteX(WildBoar);
            writer.WriteX(BloodHawk);
            writer.WriteX(WonderWasp);
            writer.WriteX(MightyTyphoon);
            writer.WriteX(MightyHurricane);
            writer.WriteX(CrazyBear);
            writer.WriteX(BlackBull);
            writer.WriteX(DarkSchneider);
            writer.WriteX(FatShark);
            writer.WriteX(CosmicDolphin);
            writer.WriteX(PinkSpider);
            writer.WriteX(MagicSeagull);
            writer.WriteX(SilverRat);
            writer.WriteX(SparkMoon);
            writer.WriteX(BunnyFlash);
            writer.WriteX(GroovyTaxi);
            writer.WriteX(RollingTurtle);
            writer.WriteX(RainbowPhoenix);

            for (int i = 0; i < kPaddingSize; i++)
                writer.WriteX((byte)0);

            // Machine names
            writer.WriteX(machineNames);

            // Body parts
            writer.WriteX(BraveEagle);
            writer.WriteX(GalaxyFalcon);
            writer.WriteX(GiantPlanet);
            writer.WriteX(MegaloCruiser);
            writer.WriteX(SplashWhale);
            writer.WriteX(WildChariot);
            writer.WriteX(ValiantJaguar);
            writer.WriteX(HolySpider);
            writer.WriteX(BloodRaven);
            writer.WriteX(FunnySwallow);
            writer.WriteX(OpticalWing);
            writer.WriteX(MadBull);
            writer.WriteX(BigTyrant);
            writer.WriteX(GrandBase);
            writer.WriteX(FireWolf);
            writer.WriteX(DreadHammer);
            writer.WriteX(SilverSword);
            writer.WriteX(RageKnight);
            writer.WriteX(RapidBarrel);
            writer.WriteX(SkyHorse);
            writer.WriteX(AquaGoose);
            writer.WriteX(SpaceCancer);
            writer.WriteX(MetalShell);
            writer.WriteX(SpeedyDragon);
            writer.WriteX(LibertyManta);

            // Cockpoit parts
            writer.WriteX(WonderWorm);
            writer.WriteX(RushCyclone);
            writer.WriteX(CombatCannon);
            writer.WriteX(MuscleGorilla);
            writer.WriteX(CyberFox);
            writer.WriteX(HeatSnake);
            writer.WriteX(RaveDrifter);
            writer.WriteX(AerialBullet);
            writer.WriteX(SparkBird);
            writer.WriteX(BlastCamel);
            writer.WriteX(DarkChaser);
            writer.WriteX(GarnetPhantom);
            writer.WriteX(BrightSpear);
            writer.WriteX(HyperStream);
            writer.WriteX(SuperLynx);
            writer.WriteX(CrystalEgg);
            writer.WriteX(WindyShark);
            writer.WriteX(RedRex);
            writer.WriteX(SonicSoldier);
            writer.WriteX(MaximumStar);
            writer.WriteX(MoonSnail);
            writer.WriteX(CrazyBuffalo);
            writer.WriteX(ScudViper);
            writer.WriteX(RoundDisk);
            writer.WriteX(EnergyCrest);

            // Booster parts
            writer.WriteX(Euros_01);
            writer.WriteX(Triangle_GT);
            writer.WriteX(Velocity_J);
            writer.WriteX(Sunrise140);
            writer.WriteX(Saturn_SG);
            writer.WriteX(Bluster_X);
            writer.WriteX(Devilfish_RX);
            writer.WriteX(Mars_EX);
            writer.WriteX(Titan_G4);
            writer.WriteX(Extreme_ZZ);
            writer.WriteX(Thunderbolt_V2);
            writer.WriteX(Boxer_2C);
            writer.WriteX(Shuttle_M2);
            writer.WriteX(Punisher_4X);
            writer.WriteX(Scorpion_R);
            writer.WriteX(Raiden_88);
            writer.WriteX(Impulse220);
            writer.WriteX(Bazooka_YS);
            writer.WriteX(Meteor_RR);
            writer.WriteX(Tiger_RZ);
            writer.WriteX(Hornet_FX);
            writer.WriteX(Jupiter_Q);
            writer.WriteX(Comet_V);
            writer.WriteX(Crown_77);
            writer.WriteX(Triple_Z);

            // Custom parts names
            writer.WriteX(partsInternalNames);

            BinaryIoUtility.PopEndianness();
        }

    }
}