using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HeroesPowerPlant.LayoutEditor
{
    public static class LayoutEditorFunctions
    {

        public static List<SetObjectShadow> GetShadowLayout(string fileName)
        {
            BinaryReader LayoutFileReader = new BinaryReader(new FileStream(fileName, FileMode.Open));
            LayoutFileReader.BaseStream.Position = 0;

            string FileMagic = new string(LayoutFileReader.ReadChars(4));
            if (FileMagic != "sky2")
            {
                Debug.LogError("This is not a valid Shadow the Hedgehog layout file.");
                return null;
            }

            int AmountOfObjects = LayoutFileReader.ReadInt32();
            List<SetObjectShadow> list = new List<SetObjectShadow>(AmountOfObjects);
            List<int> miscCountL = new List<int>(AmountOfObjects);
            uint TotalMiscLength = LayoutFileReader.ReadUInt32();

            for (int i = 0; i < AmountOfObjects; i++)
            {
                LayoutFileReader.BaseStream.Position = 12 + i * 0x2C;

                if (LayoutFileReader.BaseStream.Position >= LayoutFileReader.BaseStream.Length)
                    break;

                Vector3 Position = new Vector3(LayoutFileReader.ReadSingle(), LayoutFileReader.ReadSingle(), LayoutFileReader.ReadSingle());
                Vector3 Rotation = new Vector3(LayoutFileReader.ReadSingle(), LayoutFileReader.ReadSingle(), LayoutFileReader.ReadSingle());

                byte[] UnkBytes = LayoutFileReader.ReadBytes(8);

                byte Type = LayoutFileReader.ReadByte();
                byte List = LayoutFileReader.ReadByte();
                byte Link = LayoutFileReader.ReadByte();
                byte Rend = LayoutFileReader.ReadByte();
                miscCountL.Add(LayoutFileReader.ReadInt32());

                SetObjectShadow TempObject = CreateShadowObject(List, Type, Position, Rotation, Link, Rend, UnkBytes);

                list.Add(TempObject);
            }

            LayoutFileReader.BaseStream.Position = 12 + AmountOfObjects * 0x2C;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].MiscSettings = LayoutFileReader.ReadBytes(miscCountL[i]);
            }

            LayoutFileReader.Close();

            return list;
        }

        public static Dictionary<(byte, byte), ObjectEntry> ReadObjectListData(string FileName)
        {
            var list = new Dictionary<(byte, byte), ObjectEntry>();

            byte List = 0;
            byte Type = 0;
            string Name = "";
            bool HasMiscSettings = true;
            int MiscSettingCount = -1;
            int ModelMiscSetting = -1;
            string DebugName = "";
            List<string[]> Models = new List<string[]>();

            foreach (string i in File.ReadAllLines(FileName))
            {
                if (i.StartsWith("["))
                {
                    List = Convert.ToByte(i.Substring(1, 2), 16);
                    Type = Convert.ToByte(i.Substring(5, 2), 16);
                }
                else if (i.StartsWith("Object="))
                    Name = i.Split('=')[1];
                else if (i.StartsWith("NoMiscSettings="))
                    HasMiscSettings = !Convert.ToBoolean(i.Split('=')[1]);
                else if (i.StartsWith("MiscSettingCount="))
                    MiscSettingCount = Convert.ToInt32(i.Split('=')[1]);
                else if (i.StartsWith("Debug="))
                    DebugName = i.Split('=')[1];
                else if (i.StartsWith("Model="))
                    Models.Add(i.Split('=')[1].Split(','));
                else if (i.StartsWith("ModelMiscSetting="))
                    ModelMiscSetting = Convert.ToInt32(i.Split('=')[1]);
                else if (i.StartsWith("EndOfFile"))
                {
                    list.Add((List, Type), new ObjectEntry()
                    {
                        List = List,
                        Type = Type,
                        Name = Name,
                        HasMiscSettings = HasMiscSettings,
                        DebugName = DebugName,
                        ModelNames = Models.ToArray(),
                        MiscSettingCount = MiscSettingCount,
                        ModelMiscSetting = ModelMiscSetting
                    });
                    break;
                }
                else if (i.Length == 0)
                {
                    list.Add((List, Type), new ObjectEntry()
                    {
                        List = List,
                        Type = Type,
                        Name = Name,
                        HasMiscSettings = HasMiscSettings,
                        DebugName = DebugName,
                        ModelNames = Models.ToArray(),
                        MiscSettingCount = MiscSettingCount,
                        ModelMiscSetting = ModelMiscSetting
                    });
                    List = 0;
                    Type = 0;
                    Name = "";
                    HasMiscSettings = true;
                    DebugName = "";
                    Models = new List<string[]>();
                    MiscSettingCount = -1;
                    ModelMiscSetting = -1;
                }
            }

            return list;
        }
        
        public static SetObjectShadow CreateShadowObject
            (byte List, byte Type, Vector3 Position, Vector3 Rotation, byte Link, byte Rend, byte[] UnkBytes = null)
        {
            SetObjectShadow shadowObj = FindObjectClassShadow(List, Type);
            shadowObj.Position = Position;
            shadowObj.Rotation = Rotation;
            shadowObj.List = List;
            shadowObj.Type = Type;
            shadowObj.Link = Link;
            shadowObj.Rend = Rend;
            shadowObj.UnkBytes = UnkBytes ?? new byte[8];
            shadowObj.SetObjectEntry(LayoutEditorSystem.shadowObjectEntry(List, Type));

            return shadowObj;
        }

        public static SetObjectShadow FindObjectClassShadow(byte List, byte Type)
        {
            switch (List) {
                case 0x00:
                    switch (Type) {
                        case 0x01: case 0x02: case 0x03: case 0x06: return new Object00_SpringShadow();
                        case 0x04: return new Object0004_DashRamp();
                        case 0x05: return new Object0005_Checkpoint();
                        case 0x07: return new Object0007_Case();
                        case 0x08: return new Object0008_Pulley();
                        case 0x09: return new Object0009_WoodBox();
                        case 0x0A: return new Object000A_MetalBox();
                        case 0x0B: return new Object000B_UnbreakableBox();
                        case 0x0C: return new Object000C_WeaponBox();
                        case 0x0E: return new Object000E_Rocket();
                        case 0x0F: return new Object000F_Platform();
                        case 0x10: return new Object0010_Ring();
                        case 0x11: return new Object0011_HintBall();
                        case 0x12: return new Object0012_ItemCapsule();
                        case 0x13: return new Object0013_Balloon();
                        case 0x14: return new Object0014_GoalRing();
                        case 0x15: return new Object0015_BallSwitch();
                        case 0x16: return new Object0016_TargetSwitch();
                        case 0x19: return new Object0019_Weight();
                        case 0x1A: return new Object001A_Wind();
                        case 0x1B: return new Object001B_Roadblock();
                        case 0x20: return new Object0020_Weapon();
                        case 0x23: return new Object0023_OverturnableObject();
                        case 0x3A: return new Object003A_SpecialWeaponBox();
                        case 0x33: return new Object0033_EnergyCore();
                        case 0x34: return new Object003X_UnusedMiscByteScaleType(); //Fire
                        case 0x35: return new Object003X_UnusedMiscByteScaleType(); //PoisonGas
                        case 0x37: return new Object003X_UnusedMiscByteScaleType(); //CaptureCage
                        case 0x4F: return new Object004F_Vehicle();
                        case 0x50: return new Object0050_Trigger();
                        case 0x51: return new Object0051_TriggerTalking();
                        case 0x59: return new Object0059_TriggerSkybox();
                        case 0x5A: return new Object005A_Pole();
                        case 0x61: return new Object0061_DarkSpinEntrance();
                        case 0x62: return new Object0062_LightColli();
                        case 0x64: return new Object0064_GUNSoldier();
                        case 0x65: return new Object0065_GUNBeetle();
                        case 0x66: return new Object0066_GUNBigfoot();
                        case 0x68: return new Object0068_GUNRobot();
                        case 0x78: return new Object0078_EggPierrot();
                        case 0x79: return new Object0079_EggPawn();
                        case 0x7A: return new Object007A_EggShadowAndroid();
                        case 0x8C: return new Object008C_BkGiant();
                        case 0x8D: return new Object008D_BkSoldier();
                        case 0x8E: return new Object008E_BkWingLarge();
                        case 0x8F: return new Object008F_BkWingSmall();
                        case 0x90: return new Object0090_BkWorm();
                        case 0x91: return new Object0091_BkLarva();
                        case 0x92: return new Object0092_BkChaos();
                        case 0x93: return new Object0093_BkNinja();
                        case 0x1F: default: return new Object_ShadowDefault(); // warp hole
                    }
                case 0x01:
                    switch (Type) {
                        case 0x2C: return new Object012C_EnvironmentalWeapon();
                        case 0x90: return new Object0190_Partner();
                        default: return new Object_ShadowDefault();
                    }
                case 0x03:
                    switch (Type) {
                        case 0xE9: return new Object03E9_FallingBuilding();
                        case 0xEA: return new Object03EA_GiantSkyLaser();
                        default: return new Object_ShadowDefault();
                    }
                case 0x07:
                    switch (Type) {
                        case 0xD1: return new Object07D1_Searchlight();
                        case 0xD2: return new Object07D2_ColorSwitch();
                        case 0xD3: return new Object07D3_RisingLaserBar();
                        case 0xD4: return new Object07D4_ElecSecurity();
                        case 0xD5: return new Object07D5_LightspeedRisingBlock();
                        case 0xD7: return new Object07D7_DigitalBreakableTile();
                        case 0xD8: return new Object07D8_LaserWallBarrier();
                        case 0xDA: return new Object07DA_MatrixTerminalElecFan();
                        case 0xDE: return new Object00_SpringShadow();
                        case 0xDF: return new Object07DF_LightspeedFirewall();
                        case 0xE1: return new Object07E1_TriggerDigitalBreakableTile();
                        case 0xE2: return new Object07E2_ElecCube();
                        case 0xE8: return new Object07E8_ElecRollHexa();
                        case 0xEB: return new Object07EB_CubePlatformCircle();
                        default: return new Object_ShadowDefault();
                    }
                case 0x08:
                    switch (Type) {
                        case 0x34: return new Object0834_Tornado();
                        case 0x35: return new Object0835_TornadoCollision();
                        case 0x36: return new Object0836_RollCircle();
                        case 0x37: return new Object0837_CollapsingPillar();
                        case 0x38: return new Object0838_RuinsStoneGuardian();
                        case 0x39: return new Object106C_SkyRuinsJewel(); //RuinsJewel / PowerDeviceCage
                        case 0x99: return new Object1451_CommandCollision(); //BlackTankCommandCollision
                        case 0x9A: return new Object089A_BreakingRoad();
                        case 0x9C: return new Object089C_FallingRoad();
                        default: return new Object_ShadowDefault();
                    }
                case 0x0B:
                    switch (Type) {
                        case 0xBB: return new Object0BBB_SmallLantern();
                        case 0xBC: return new Object0BBC_PopupDummyGhost();
                        case 0xBE: return new Object0BBE_Chao();
                        case 0xC7: return new Object0BC7_CastleMonster();
                        case 0xC8: return new Object0BC8_CastleMonsterControl();
                        default: return new Object_ShadowDefault();
                    }
                case 0x0C:
                    switch (Type) {
                        case 0x80: return new Object0C80_BounceBall();
                        case 0x81: return new Object0C81_CircusGong();
                        case 0x82: return new Object0C82_GameBalloonsGhosts();
                        case 0x83: return new Object0C83_CircusGameTarget();
                        case 0x87: return new Object005A_Pole(); //CircusPole
                        case 0x88: return new Object0C88_Zipline();
                        case 0x89: return new Object1133_ProximityDoor(); //TentCurtain
                        default: return new Object_ShadowDefault();
                    }
                case 0x0F:
                    switch (Type) {
                        case 0xA1: return new Object0FA1_BAMiniBomb();
                        case 0xA2: return new Object0FA2_Helicopter();
                        case 0xA4: return new Object0FA4_BuildingChunk();
                        default: return new Object_ShadowDefault();
                    }
                case 0x10:
                    switch (Type) {
                        case 0x04: return new Object1004_ArkCrackedWall();
                        case 0x05: return new Object1005_Researcher();
                        case 0x06: return new Object1006_HealUnitServer();
                        case 0x69: return new Object1069_FleetHolderEggmanBattleship();
                        case 0x6C: return new Object106C_SkyRuinsJewel();
                        case 0x6D: return new Object106D_RainEffect();
                        default: return new Object_ShadowDefault();
                    }
                case 0x11:
                    switch (Type) {
                        case 0x30: return new Object1130_FenceWall();
                        case 0x31: return new Object1131_Vine();
                        case 0x32: return new Object1132_ElevatorPlatformColumn();
                        case 0x33: return new Object1133_ProximityDoor();
                        case 0x34: return new Object1134_DamageBlock();
                        case 0x35: return new Object1130_FenceWall(); //ShatterTrijumpPanel (BreakWall)
                        case 0x37: return new Object1137_StretchGrass();
                        case 0x38: return new Object1138_JumpPanel();
                        default: return new Object_ShadowDefault();
                    }
                case 0x13:
                    switch (Type) {
                        case 0x8A: return new Object138B_MeteorsHolder(); //Meteor
                        case 0x8B: return new Object138B_MeteorsHolder();
                        case 0x8E: return new Object138E_ArkCannon();
                        case 0x92: return new Object1392_SpaceDebris();
                        case 0xED: return new Object13ED_EscapePodPathSwitch();
                        case 0xEF: return new Object13EF_SecurityLaser();
                        case 0xF0: return new Object13F0_SlideoutPlatform();
                        case 0xF1: return new Object1133_ProximityDoor(); //HeavyBaseDoor
                        case 0xF2: return new Object1451_CommandCollision(); //EscapePodCommandCollision
                        case 0xF3: return new Object13F3_EscapePodDownRail();
                        default: return new Object_ShadowDefault();
                    }
                case 0x14:
                    switch (Type) {
                        case 0x51: return new Object1451_CommandCollision();
                        case 0xB4: return new Object14B5_GravityChangeZone(); //GravityChangeSwitch
                        case 0xB5: return new Object14B5_GravityChangeZone();
                        case 0xB6: return new Object14B6_GravityChangeCollision();
                        case 0xBE: return new Object14BE_ArkGreenLaser();
                        case 0xED: return new Object13ED_EscapePodPathSwitch();
                        default: return new Object_ShadowDefault();
                    }
                case 0x17:
                    switch (Type) {
                        case 0x70: return new Object1770_GUNCamera();
                        case 0x72: return new Object1772_ConcreteDoor();
                        case 0x73: return new Object1773_CrushingWalls();
                        case 0xD4: return new Object11D4_BAGunShip();
                        case 0xD5: return new Object17D5_BlackArmsMine();
                        default: return new Object_ShadowDefault();
                    }
                case 0x18:
                    switch (Type) {
                        case 0x39: return new Object1839_RisingLava();
                        case 0x9E: return new Object189E_ARKDriftingPlat1();
                        case 0x9F: return new Object189F_ArkRollPlatform();
                        default: return new Object_ShadowDefault();
                    }
                case 0x19:
                    switch (Type) {
                        case 0x01: return new Object1901_CometBarrier();
                        case 0x03: return new Object1903_BlackDoomHologram();
                        default: return new Object_ShadowDefault();
                    }
                case 0x25:
                    switch (Type) {
                        case 0x86: return new Object2586_Sample();
                        case 0x88: return new Object2588_Decoration1();
                        case 0x89: return new Object2589_Destructable1();
                        case 0x8A: return new Object258A_Effect1();
                        case 0x90: return new Object2588_Decoration1();
                        case 0x91: return new Object2589_Destructable1();
                        case 0x92: return new Object2592_DebugMissionClearCollision();
                        case 0x93: return new Object2593_SetGenerator();
                        case 0x94: return new Object2594_Fan();
                        case 0x95: return new Object2595_MissionClearCollision();
                        case 0x97: return new Object2597_SetSeLoop();
                        case 0x98: return new Object2598_SetSeOneShot();
                        default: return new Object_ShadowDefault();
                    }
                default: return new Object_ShadowDefault();
            }
        }
    }
}
