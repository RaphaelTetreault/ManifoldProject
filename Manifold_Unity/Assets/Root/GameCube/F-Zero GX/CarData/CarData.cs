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
    public class CarData : IBinarySerializable, IBinaryAddressable, IFile
    {
        public const int MachineCount = 41;
        public const int BodyCount = 25;
        public const int CockpitCount = 25;
        public const int BoosterCount = 25;

        public const int kPaddingSize = 12;
        public const bool kBigEndian = true;
        public const bool kLittleEndian = false;
        public const int kMachineNameTable = 43;
        public const int kUnknownTable = 32;

        [SerializeField] string fileName;
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        #region MEMBERS

        [Header("String Table")]
        public byte[] padding; // 12 bytes
        public string[] machineNames;
        public string[] unknownNames;

        [Header("Vehicles")]
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

        [Header("Body")]
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

        [Header("Cockpit")]
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

        [Header("Booster")]
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

        #endregion

        #region PROPERTIES

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        public VehicleParameters[] Machines
            => new VehicleParameters[]
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

        public VehicleParameters[] BodyParts
            => new VehicleParameters[]
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

        public VehicleParameters[] CockpitParts
            => new VehicleParameters[]
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

        public VehicleParameters[] BoosterParts
            => new VehicleParameters[]
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


        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianess(kBigEndian);

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
            reader.ReadX(ref DarkSchneider, true);
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

        #endregion
    }
}