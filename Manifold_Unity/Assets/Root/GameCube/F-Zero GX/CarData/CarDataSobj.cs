using StarkTools.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.FZeroGX.CarData
{
    [Serializable]
    public class CarDataSobj : ScriptableObject, IBinarySerializable, IFile
    {
        public const int MachineCount = CarData.MachineCount;
        public const int BodyCount = CarData.BodyCount;
        public const int CockpitCount = CarData.CockpitCount;
        public const int BoosterCount = CarData.BoosterCount;
        public const int kPaddingSize = CarData.kPaddingSize;
        public const bool kBigEndian = CarData.kBigEndian;
        public const bool kLittleEndian = CarData.kLittleEndian;
        public const int kMachineNameTable = CarData.kMachineNameTable;
        public const int kUnknownTable = CarData.kUnknownTable;


        [SerializeField] string fileName;

        #region MEMBERS

        [Header("String Table")]
        public byte[] padding; // 12 bytes
        public string[] machineNames;
        public string[] unknownNames;


        [Header("Vehicles")]
        [LabelPrefix("00")]
        public VehicleParametersSobj DarkSchneider;
        [LabelPrefix("01")]
        public VehicleParametersSobj RedGazelle;
        [LabelPrefix("02")]
        public VehicleParametersSobj WhiteCat;
        [LabelPrefix("03")]
        public VehicleParametersSobj GoldenFox;
        [LabelPrefix("04")]
        public VehicleParametersSobj IronTiger;
        [LabelPrefix("05")]
        public VehicleParametersSobj FireStingray;
        [LabelPrefix("06")]
        public VehicleParametersSobj WildGoose;
        [LabelPrefix("07")]
        public VehicleParametersSobj BlueFalcon;
        [LabelPrefix("08")]
        public VehicleParametersSobj DeepClaw;
        [LabelPrefix("09")]
        public VehicleParametersSobj GreatStar;
        [LabelPrefix("10")]
        public VehicleParametersSobj LittleWyvern;
        [LabelPrefix("11")]
        public VehicleParametersSobj MadWolf;
        [LabelPrefix("12")]
        public VehicleParametersSobj SuperPiranha;
        [LabelPrefix("13")]
        public VehicleParametersSobj DeathAnchor;
        [LabelPrefix("14")]
        public VehicleParametersSobj AstroRobin;
        [LabelPrefix("15")]
        public VehicleParametersSobj BigFang;
        [LabelPrefix("16")]
        public VehicleParametersSobj SonicPhantom;
        [LabelPrefix("17")]
        public VehicleParametersSobj GreenPanther;
        [LabelPrefix("18")]
        public VehicleParametersSobj HyperSpeeder;
        [LabelPrefix("19")]
        public VehicleParametersSobj SpaceAngler;
        [LabelPrefix("20")]
        public VehicleParametersSobj KingMeteor;
        [LabelPrefix("21")]
        public VehicleParametersSobj QueenMeteor;
        [LabelPrefix("22")]
        public VehicleParametersSobj TwinNoritta;
        [LabelPrefix("23")]
        public VehicleParametersSobj NightThunder;
        [LabelPrefix("24")]
        public VehicleParametersSobj WildBoar;
        [LabelPrefix("25")]
        public VehicleParametersSobj BloodHawk;
        [LabelPrefix("26")]
        public VehicleParametersSobj WonderWasp;
        [LabelPrefix("27")]
        public VehicleParametersSobj MightyTyphoon;
        [LabelPrefix("28")]
        public VehicleParametersSobj MightyHurricane;
        [LabelPrefix("29")]
        public VehicleParametersSobj CrazyBear;
        [LabelPrefix("30")]
        public VehicleParametersSobj BlackBull;
        [LabelPrefix("31")]
        public VehicleParametersSobj FatShark;
        [LabelPrefix("32")]
        public VehicleParametersSobj CosmicDolphin;
        [LabelPrefix("33")]
        public VehicleParametersSobj PinkSpider;
        [LabelPrefix("34")]
        public VehicleParametersSobj MagicSeagull;
        [LabelPrefix("35")]
        public VehicleParametersSobj SilverRat;
        [LabelPrefix("36")]
        public VehicleParametersSobj SparkMoon;
        [LabelPrefix("37")]
        public VehicleParametersSobj BunnyFlash;
        [LabelPrefix("38")]
        public VehicleParametersSobj GroovyTaxi;
        [LabelPrefix("39")]
        public VehicleParametersSobj RollingTurtle;
        [LabelPrefix("40")]
        public VehicleParametersSobj RainbowPhoenix;

        [Header("Body")]
        [LabelPrefix("01")]
        public VehicleParametersSobj BraveEagle;
        [LabelPrefix("02")]
        public VehicleParametersSobj GalaxyFalcon;
        [LabelPrefix("03")]
        public VehicleParametersSobj GiantPlanet;
        [LabelPrefix("04")]
        public VehicleParametersSobj MegaloCruiser;
        [LabelPrefix("05")]
        public VehicleParametersSobj SplashWhale;
        [LabelPrefix("06")]
        public VehicleParametersSobj WildChariot;
        [LabelPrefix("07")]
        public VehicleParametersSobj ValiantJaguar;
        [LabelPrefix("08")]
        public VehicleParametersSobj HolySpider;
        [LabelPrefix("09")]
        public VehicleParametersSobj BloodRaven;
        [LabelPrefix("10")]
        public VehicleParametersSobj FunnySwallow;
        [LabelPrefix("11")]
        public VehicleParametersSobj OpticalWing;
        [LabelPrefix("12")]
        public VehicleParametersSobj MadBull;
        [LabelPrefix("13")]
        public VehicleParametersSobj BigTyrant;
        [LabelPrefix("14")]
        public VehicleParametersSobj GrandBase;
        [LabelPrefix("15")]
        public VehicleParametersSobj FireWolf;
        [LabelPrefix("16")]
        public VehicleParametersSobj DreadHammer;
        [LabelPrefix("17")]
        public VehicleParametersSobj SilverSword;
        [LabelPrefix("18")]
        public VehicleParametersSobj RageKnight;
        [LabelPrefix("19")]
        public VehicleParametersSobj RapidBarrel;
        [LabelPrefix("20")]
        public VehicleParametersSobj SkyHorse;
        [LabelPrefix("21")]
        public VehicleParametersSobj AquaGoose;
        [LabelPrefix("22")]
        public VehicleParametersSobj SpaceCancer;
        [LabelPrefix("23")]
        public VehicleParametersSobj MetalShell;
        [LabelPrefix("24")]
        public VehicleParametersSobj SpeedyDragon;
        [LabelPrefix("25")]
        public VehicleParametersSobj LibertyManta;

        [Header("Cockpit")]
        [LabelPrefix("01")]
        public VehicleParametersSobj WonderWorm;
        [LabelPrefix("02")]
        public VehicleParametersSobj RushCyclone;
        [LabelPrefix("03")]
        public VehicleParametersSobj CombatCannon;
        [LabelPrefix("04")]
        public VehicleParametersSobj MuscleGorilla;
        [LabelPrefix("05")]
        public VehicleParametersSobj CyberFox;
        [LabelPrefix("06")]
        public VehicleParametersSobj HeatSnake;
        [LabelPrefix("07")]
        public VehicleParametersSobj RaveDrifter;
        [LabelPrefix("08")]
        public VehicleParametersSobj AerialBullet;
        [LabelPrefix("09")]
        public VehicleParametersSobj SparkBird;
        [LabelPrefix("10")]
        public VehicleParametersSobj BlastCamel;
        [LabelPrefix("11")]
        public VehicleParametersSobj DarkChaser;
        [LabelPrefix("12")]
        public VehicleParametersSobj GarnetPhantom;
        [LabelPrefix("13")]
        public VehicleParametersSobj BrightSpear;
        [LabelPrefix("14")]
        public VehicleParametersSobj HyperStream;
        [LabelPrefix("15")]
        public VehicleParametersSobj SuperLynx;
        [LabelPrefix("16")]
        public VehicleParametersSobj CrystalEgg;
        [LabelPrefix("17")]
        public VehicleParametersSobj WindyShark;
        [LabelPrefix("18")]
        public VehicleParametersSobj RedRex;
        [LabelPrefix("19")]
        public VehicleParametersSobj SonicSoldier;
        [LabelPrefix("20")]
        public VehicleParametersSobj MaximumStar;
        [LabelPrefix("21")]
        public VehicleParametersSobj MoonSnail;
        [LabelPrefix("22")]
        public VehicleParametersSobj CrazyBuffalo;
        [LabelPrefix("23")]
        public VehicleParametersSobj ScudViper;
        [LabelPrefix("24")]
        public VehicleParametersSobj RoundDisk;
        [LabelPrefix("25")]
        public VehicleParametersSobj EnergyCrest;

        [Header("Booster")]
        [LabelPrefix("01")]
        public VehicleParametersSobj Euros_01;
        [LabelPrefix("02")]
        public VehicleParametersSobj Triangle_GT;
        [LabelPrefix("03")]
        public VehicleParametersSobj Velocity_J;
        [LabelPrefix("04")]
        public VehicleParametersSobj Sunrise140;
        [LabelPrefix("05")]
        public VehicleParametersSobj Saturn_SG;
        [LabelPrefix("06")]
        public VehicleParametersSobj Bluster_X;
        [LabelPrefix("07")]
        public VehicleParametersSobj Devilfish_RX;
        [LabelPrefix("08")]
        public VehicleParametersSobj Mars_EX;
        [LabelPrefix("09")]
        public VehicleParametersSobj Titan_G4;
        [LabelPrefix("10")]
        public VehicleParametersSobj Extreme_ZZ;
        [LabelPrefix("11")]
        public VehicleParametersSobj Thunderbolt_V2;
        [LabelPrefix("12")]
        public VehicleParametersSobj Boxer_2C;
        [LabelPrefix("13")]
        public VehicleParametersSobj Shuttle_M2;
        [LabelPrefix("14")]
        public VehicleParametersSobj Punisher_4X;
        [LabelPrefix("15")]
        public VehicleParametersSobj Scorpion_R;
        [LabelPrefix("16")]
        public VehicleParametersSobj Raiden_88;
        [LabelPrefix("17")]
        public VehicleParametersSobj Impulse220;
        [LabelPrefix("18")]
        public VehicleParametersSobj Bazooka_YS;
        [LabelPrefix("19")]
        public VehicleParametersSobj Meteor_RR;
        [LabelPrefix("20")]
        public VehicleParametersSobj Tiger_RZ;
        [LabelPrefix("21")]
        public VehicleParametersSobj Hornet_FX;
        [LabelPrefix("22")]
        public VehicleParametersSobj Jupiter_Q;
        [LabelPrefix("23")]
        public VehicleParametersSobj Comet_V;
        [LabelPrefix("24")]
        public VehicleParametersSobj Crown_77;
        [LabelPrefix("25")]
        public VehicleParametersSobj Triple_Z;

        #endregion

        #region PROPERTIES

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }


        public VehicleParametersSobj[] Vehicles
            => new VehicleParametersSobj[]
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

        public VehicleParametersSobj[] BodyParts
            => new VehicleParametersSobj[]
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

        public VehicleParametersSobj[] CockpitParts
            => new VehicleParametersSobj[]
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

        public VehicleParametersSobj[] BoosterParts
            => new VehicleParametersSobj[]
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

        public readonly int[] MachineIndex = new int[]
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16,
            17,
            18,
            19,
            20,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            0,
            31,
            32,
            33,
            34,
            35,
            36,
            37,
            38,
            39,
            40,
        };

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianess(kBigEndian);

            reader.ReadX(ref DarkSchneider, true);
            reader.ReadX(ref RedGazelle, true);
            reader.ReadX(ref WhiteCat, true);
            reader.ReadX(ref GoldenFox, true);
            reader.ReadX(ref IronTiger, true);
            reader.ReadX(ref FireStingray, true);
            reader.ReadX(ref WildGoose, true);
            reader.ReadX(ref BlueFalcon, true);
            reader.ReadX(ref DeepClaw, true);
            reader.ReadX(ref GreatStar, true);
            reader.ReadX(ref LittleWyvern, true);
            reader.ReadX(ref MadWolf, true);
            reader.ReadX(ref SuperPiranha, true);
            reader.ReadX(ref DeathAnchor, true);
            reader.ReadX(ref AstroRobin, true);
            reader.ReadX(ref BigFang, true);
            reader.ReadX(ref SonicPhantom, true);
            reader.ReadX(ref GreenPanther, true);
            reader.ReadX(ref HyperSpeeder, true);
            reader.ReadX(ref SpaceAngler, true);
            reader.ReadX(ref KingMeteor, true);
            reader.ReadX(ref QueenMeteor, true);
            reader.ReadX(ref TwinNoritta, true);
            reader.ReadX(ref NightThunder, true);
            reader.ReadX(ref WildBoar, true);
            reader.ReadX(ref BloodHawk, true);
            reader.ReadX(ref WonderWasp, true);
            reader.ReadX(ref MightyTyphoon, true);
            reader.ReadX(ref MightyHurricane, true);
            reader.ReadX(ref CrazyBear, true);
            reader.ReadX(ref BlackBull, true);
            reader.ReadX(ref FatShark, true);
            reader.ReadX(ref CosmicDolphin, true);
            reader.ReadX(ref PinkSpider, true);
            reader.ReadX(ref MagicSeagull, true);
            reader.ReadX(ref SilverRat, true);
            reader.ReadX(ref SparkMoon, true);
            reader.ReadX(ref BunnyFlash, true);
            reader.ReadX(ref GroovyTaxi, true);
            reader.ReadX(ref RollingTurtle, true);
            reader.ReadX(ref RainbowPhoenix, true);

            // Read some padding
            reader.ReadX(ref padding, kPaddingSize);
            foreach (var pad in padding)
                Assert.IsTrue(pad == 0);

            BinaryIoUtility.PushEndianess(kLittleEndian);
            machineNames = new string[kMachineNameTable];
            for (int i = 0; i < machineNames.Length; i++)
            {
                reader.ReadXCString(ref machineNames[i], System.Text.Encoding.ASCII);
            }
            BinaryIoUtility.PopEndianess();

            reader.ReadX(ref BraveEagle, true);
            reader.ReadX(ref GalaxyFalcon, true);
            reader.ReadX(ref GiantPlanet, true);
            reader.ReadX(ref MegaloCruiser, true);
            reader.ReadX(ref SplashWhale, true);
            reader.ReadX(ref WildChariot, true);
            reader.ReadX(ref ValiantJaguar, true);
            reader.ReadX(ref HolySpider, true);
            reader.ReadX(ref BloodRaven, true);
            reader.ReadX(ref FunnySwallow, true);
            reader.ReadX(ref OpticalWing, true);
            reader.ReadX(ref MadBull, true);
            reader.ReadX(ref BigTyrant, true);
            reader.ReadX(ref GrandBase, true);
            reader.ReadX(ref FireWolf, true);
            reader.ReadX(ref DreadHammer, true);
            reader.ReadX(ref SilverSword, true);
            reader.ReadX(ref RageKnight, true);
            reader.ReadX(ref RapidBarrel, true);
            reader.ReadX(ref SkyHorse, true);
            reader.ReadX(ref AquaGoose, true);
            reader.ReadX(ref SpaceCancer, true);
            reader.ReadX(ref MetalShell, true);
            reader.ReadX(ref SpeedyDragon, true);
            reader.ReadX(ref LibertyManta, true);

            reader.ReadX(ref WonderWorm, true);
            reader.ReadX(ref RushCyclone, true);
            reader.ReadX(ref CombatCannon, true);
            reader.ReadX(ref MuscleGorilla, true);
            reader.ReadX(ref CyberFox, true);
            reader.ReadX(ref HeatSnake, true);
            reader.ReadX(ref RaveDrifter, true);
            reader.ReadX(ref AerialBullet, true);
            reader.ReadX(ref SparkBird, true);
            reader.ReadX(ref BlastCamel, true);
            reader.ReadX(ref DarkChaser, true);
            reader.ReadX(ref GarnetPhantom, true);
            reader.ReadX(ref BrightSpear, true);
            reader.ReadX(ref HyperStream, true);
            reader.ReadX(ref SuperLynx, true);
            reader.ReadX(ref CrystalEgg, true);
            reader.ReadX(ref WindyShark, true);
            reader.ReadX(ref RedRex, true);
            reader.ReadX(ref SonicSoldier, true);
            reader.ReadX(ref MaximumStar, true);
            reader.ReadX(ref MoonSnail, true);
            reader.ReadX(ref CrazyBuffalo, true);
            reader.ReadX(ref ScudViper, true);
            reader.ReadX(ref RoundDisk, true);
            reader.ReadX(ref EnergyCrest, true);

            reader.ReadX(ref Euros_01, true);
            reader.ReadX(ref Triangle_GT, true);
            reader.ReadX(ref Velocity_J, true);
            reader.ReadX(ref Sunrise140, true);
            reader.ReadX(ref Saturn_SG, true);
            reader.ReadX(ref Bluster_X, true);
            reader.ReadX(ref Devilfish_RX, true);
            reader.ReadX(ref Mars_EX, true);
            reader.ReadX(ref Titan_G4, true);
            reader.ReadX(ref Extreme_ZZ, true);
            reader.ReadX(ref Thunderbolt_V2, true);
            reader.ReadX(ref Boxer_2C, true);
            reader.ReadX(ref Shuttle_M2, true);
            reader.ReadX(ref Punisher_4X, true);
            reader.ReadX(ref Scorpion_R, true);
            reader.ReadX(ref Raiden_88, true);
            reader.ReadX(ref Impulse220, true);
            reader.ReadX(ref Bazooka_YS, true);
            reader.ReadX(ref Meteor_RR, true);
            reader.ReadX(ref Tiger_RZ, true);
            reader.ReadX(ref Hornet_FX, true);
            reader.ReadX(ref Jupiter_Q, true);
            reader.ReadX(ref Comet_V, true);
            reader.ReadX(ref Crown_77, true);
            reader.ReadX(ref Triple_Z, true);

            BinaryIoUtility.PushEndianess(kLittleEndian);
            unknownNames = new string[kUnknownTable];
            for (int i = 0; i < unknownNames.Length; i++)
            {
                reader.ReadXCString(ref unknownNames[i], System.Text.Encoding.ASCII);
            }
            BinaryIoUtility.PopEndianess();

            BinaryIoUtility.PopEndianess();
        }

        public void Serialize(BinaryWriter writer)
        {
            BinaryIoUtility.PushEndianess(kBigEndian);

            writer.WriteX(DarkSchneider);
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

            BinaryIoUtility.PushEncoding(System.Text.Encoding.ASCII);
            BinaryIoUtility.PushEndianess(kLittleEndian);
            foreach (var name in machineNames)
            {
                writer.WriteXCString(name);
            }
            BinaryIoUtility.PopEndianess();
            BinaryIoUtility.PopEncoding();

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


            BinaryIoUtility.PushEncoding(System.Text.Encoding.ASCII);
            BinaryIoUtility.PushEndianess(kLittleEndian);
            foreach (var name in unknownNames)
            {
                writer.WriteXCString(name);
            }
            BinaryIoUtility.PopEndianess();
            BinaryIoUtility.PopEncoding();

            BinaryIoUtility.PopEndianess();
        }

        internal void SetMachine(int index, VehicleParametersSobj value)
        {
            switch (index)
            {
                case 0: DarkSchneider = value; return;
                case 1: RedGazelle = value; return;
                case 2: WhiteCat = value; return;
                case 3: GoldenFox = value; return;
                case 4: IronTiger = value; return;
                case 5: FireStingray = value; return;
                case 6: WildGoose = value; return;
                case 7: BlueFalcon = value; return;
                case 8: DeepClaw = value; return;
                case 9: GreatStar = value; return;
                case 10: LittleWyvern = value; return;
                case 11: MadWolf = value; return;
                case 12: SuperPiranha = value; return;
                case 13: DeathAnchor = value; return;
                case 14: AstroRobin = value; return;
                case 15: BigFang = value; return;
                case 16: SonicPhantom = value; return;
                case 17: GreenPanther = value; return;
                case 18: HyperSpeeder = value; return;
                case 19: SpaceAngler = value; return;
                case 20: KingMeteor = value; return;
                case 21: QueenMeteor = value; return;
                case 22: TwinNoritta = value; return;
                case 23: NightThunder = value; return;
                case 24: WildBoar = value; return;
                case 25: BloodHawk = value; return;
                case 26: WonderWasp = value; return;
                case 27: MightyTyphoon = value; return;
                case 28: MightyHurricane = value; return;
                case 29: CrazyBear = value; return;
                case 30: BlackBull = value; return;
                case 31: FatShark = value; return;
                case 32: CosmicDolphin = value; return;
                case 33: PinkSpider = value; return;
                case 34: MagicSeagull = value; return;
                case 35: SilverRat = value; return;
                case 36: SparkMoon = value; return;
                case 37: BunnyFlash = value; return;
                case 38: GroovyTaxi = value; return;
                case 39: RollingTurtle = value; return;
                case 40: RainbowPhoenix = value; return;

                default:
                    return;
            }
        }

        internal void SetBody(int index, VehicleParametersSobj value)
        {
            switch (index)
            {
                case 0: BraveEagle = value; return;
                case 1: GalaxyFalcon = value; return;
                case 2: GiantPlanet = value; return;
                case 3: MegaloCruiser = value; return;
                case 4: SplashWhale = value; return;
                case 5: WildChariot = value; return;
                case 6: ValiantJaguar = value; return;
                case 7: HolySpider = value; return;
                case 8: BloodRaven = value; return;
                case 9: FunnySwallow = value; return;
                case 10: OpticalWing = value; return;
                case 11: MadBull = value; return;
                case 12: BigTyrant = value; return;
                case 13: GrandBase = value; return;
                case 14: FireWolf = value; return;
                case 15: DreadHammer = value; return;
                case 16: SilverSword = value; return;
                case 17: RageKnight = value; return;
                case 18: RapidBarrel = value; return;
                case 19: SkyHorse = value; return;
                case 20: AquaGoose = value; return;
                case 21: SpaceCancer = value; return;
                case 22: MetalShell = value; return;
                case 23: SpeedyDragon = value; return;
                case 24: LibertyManta = value; return;

                default:
                    return;
            }
        }

        internal void SetCockpit(int index, VehicleParametersSobj value)
        {
            switch (index)
            {
                case 0: WonderWorm = value; return;
                case 1: RushCyclone = value; return;
                case 2: CombatCannon = value; return;
                case 3: MuscleGorilla = value; return;
                case 4: CyberFox = value; return;
                case 5: HeatSnake = value; return;
                case 6: RaveDrifter = value; return;
                case 7: AerialBullet = value; return;
                case 8: SparkBird = value; return;
                case 9: BlastCamel = value; return;
                case 10: DarkChaser = value; return;
                case 11: GarnetPhantom = value; return;
                case 12: BrightSpear = value; return;
                case 13: HyperStream = value; return;
                case 14: SuperLynx = value; return;
                case 15: CrystalEgg = value; return;
                case 16: WindyShark = value; return;
                case 17: RedRex = value; return;
                case 18: SonicSoldier = value; return;
                case 19: MaximumStar = value; return;
                case 20: MoonSnail = value; return;
                case 21: CrazyBuffalo = value; return;
                case 22: ScudViper = value; return;
                case 23: RoundDisk = value; return;
                case 24: EnergyCrest = value; return;

                default:
                    return;
            }
        }

        internal void SetBooster(int index, VehicleParametersSobj value)
        {
            switch (index)
            {
                case 0: Euros_01 = value; return;
                case 1: Triangle_GT = value; return;
                case 2: Velocity_J = value; return;
                case 3: Sunrise140 = value; return;
                case 4: Saturn_SG = value; return;
                case 5: Bluster_X = value; return;
                case 6: Devilfish_RX = value; return;
                case 7: Mars_EX = value; return;
                case 8: Titan_G4 = value; return;
                case 9: Extreme_ZZ = value; return;
                case 10: Thunderbolt_V2 = value; return;
                case 11: Boxer_2C = value; return;
                case 12: Shuttle_M2 = value; return;
                case 13: Punisher_4X = value; return;
                case 14: Scorpion_R = value; return;
                case 15: Raiden_88 = value; return;
                case 16: Impulse220 = value; return;
                case 17: Bazooka_YS = value; return;
                case 18: Meteor_RR = value; return;
                case 19: Tiger_RZ = value; return;
                case 20: Hornet_FX = value; return;
                case 21: Jupiter_Q = value; return;
                case 22: Comet_V = value; return;
                case 23: Crown_77 = value; return;
                case 24: Triple_Z = value; return;

                default:
                    return;
            }
        }

        #endregion
    }
}