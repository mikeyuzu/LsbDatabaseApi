using Google.Protobuf.WellKnownTypes;
using LsbDatabaseApi.Controllers;
using LsbDatabaseApi.mission;
using LsbDatabaseApi.@struct;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static LsbDatabaseApi.DatabaseApi;
using static LsbDatabaseApi.MessageParam;
using static Mysqlx.Notice.Warning.Types;

namespace LsbDatabaseApi
{
    // IDataReaderの拡張メソッド
    public static class DataReaderExtensions
    {
        public static T? Get<T>(this IDataReader reader, string columnName)
        {
            var value = reader[columnName];
            if (value == DBNull.Value)
            {
                return default;
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }

    public class DatabaseApi
    {
        public const int _MAX_ZONE_TABLE = 38;      // ゾーンテーブルの最大数
        public const int _MAX_KEYS_TABLE = 7;       // だいじなものテーブルの最大数
        public const int _KEYITEM_BIT_SIZE = 512;   // だいじなもののビットサイズ
        public const int _QUEST_BIT_SIZE = 32;      // クエストのビットサイズ
        public const int _MAX_MISSIONAREA = 15;     // ミッションのエリア数

        public const int _MAX_QUESTAREA = 11;       // クエストのエリア数
        public const int _MISSION_COP = 6;          // プロマシアの呪縛

        // 合成倉庫のキャラID
        private const int _synergyInventoryCharacterId = 1000000;

        // キャッシュ
        private static readonly ConcurrentDictionary<int, MissionInfo> CacheMissionInfo = new();                   // ミッション
        private static readonly ConcurrentDictionary<int, QuestInfo> CacheQuestInfo = new();                       // クエスト
        private static readonly ConcurrentDictionary<int, ZoneList> CacheZoneList = new();                         // ゾーン
        private static readonly ConcurrentDictionary<int, KeyItems> CacheKeyItems = new();                         // だいじなもの
        private static readonly ConcurrentDictionary<int, byte> CacheNation = new();                               // 所属国
        private static readonly ConcurrentDictionary<int, byte> CacheCampaignAllegiance = new();                   // アルタナ所属国
        private static readonly ConcurrentDictionary<int, CharacterVariableList> CacheVarList = new();             // 変数リスト
        private static readonly ConcurrentDictionary<int, TeleportInfo> CacheTeleportInfo = new();                 // テレポート情報
        private static readonly ConcurrentDictionary<int, CharacterProfile> CacheProfile = new();                  // プロフィール情報
        private static readonly ConcurrentDictionary<int, List<Inventory>> CacheInventory = new();                 // インベントリー情報
        private static readonly ConcurrentDictionary<int, List<CharaEffect>> CacheCharaEffect = new();             // キャラクター効果情報
        private static readonly ConcurrentDictionary<int, Dictionary<int, CharaSkill>> CacheCharaSkill = new();    // キャラクタースキル情報
        private static readonly ConcurrentDictionary<int, CharaStatus> CacheCharaStatus = new();                   // ステータス情報
        private static readonly ConcurrentDictionary<int, CharaJob> CacheCharaJob = new();                         // ジョブ情報
        private static readonly ConcurrentDictionary<int, CharaMagic> CacheCharaMagic = new();                     // 魔法情報
        private static readonly ConcurrentDictionary<int, SynthesisRecipe> CacheSynthesisRecipes = new();          // レシピ情報

        private MySqlConnection? _connection = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DatabaseApi()
        {
            _connection = null;
        }

        /// <summary>
        ﻿/// ミッション情報の構造体
        ﻿/// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MissionRecord
        {
            public ushort Current;
            public ushort StatusUpper;
            public ushort StatusLower;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
            public bool[] Complete;

            public MissionRecord()
            {
                Current = 0;
                StatusUpper = 0;
                StatusLower = 0;
                Complete = new bool[64];
            }
        }

        /// <summary>
        /// missionテーブル
        ﻿/// </summary>
        public struct MissionInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = _MAX_MISSIONAREA)]
            public MissionRecord[] Tables;

            public MissionInfo()
            {
                Tables = new MissionRecord[_MAX_MISSIONAREA];
                for (int i = 0; i < Tables.Length; i++)
                {
                    Tables[i] = new();
                }
                Tables[(int)MissionId.SANDORIA].Current = (int)MissionSandoria.NONE; // サンドリア
                Tables[(int)MissionId.BASTOK].Current = (int)MissionBastok.NONE; // バストゥーク
                Tables[(int)MissionId.WINDURST].Current = (int)MissionWindurst.NONE; // ウィンダス
            }

            /// <summary>
            /// ミッションの受託状況を取得
            ﻿/// </summary>
            ﻿/// <param name="missionId"></param>
            ﻿/// <param name="currentId"></param>
            ﻿/// <returns></returns>
            public readonly bool HasMissionCurrent(MissionId missionId, int currentId)
            {
                return Tables[(int)missionId].Current == currentId;
            }

            /// <summary>
            ﻿/// ミッションの進行状況を取得
            ﻿/// </summary>
            ﻿/// <param name="missionId"></param>
            ﻿/// <param name="currentId"></param>
            ﻿/// <returns></returns>
            public readonly bool HasMissionComplete(MissionId missionId, int currentId)
            {
                if (missionId == MissionId.COP)
                {
                    return currentId < Tables[(int)missionId].Current;
                }
                else
                {
                    return Tables[(int)missionId].Complete[currentId];
                }
            }
        }

        /// <summary>
        ﻿/// クエスト情報の構造体
        ﻿/// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct QuestRecord
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.U1)]
            public byte[] Current;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.U1)]
            public byte[] Complete;

            public QuestRecord()
            {
                Current = new byte[32];
                Complete = new byte[32];
            }
        }

        /// <summary>
        ﻿/// questテーブル
        ﻿/// </summary>
        public struct QuestInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = _MAX_QUESTAREA)]
            public QuestRecord[] Tables;

            public QuestInfo(int tableCount, int bitSize)
            {
                Tables = new QuestRecord[tableCount];
                for (int i = 0; i < tableCount; i++)
                {
                    Tables[i] = new QuestRecord
                    {
                        Current = new byte[bitSize],
                        Complete = new byte[bitSize]
                    };
                }
            }

            /// <summary>
            /// クエストを受託しているか
            ﻿/// </summary>
            ﻿/// <param name="questAreaId"></param>
            ﻿/// <param name="questId"></param>
            ﻿/// <returns></returns>
            public readonly bool HasQuestCurrent(QuestId questAreaId, int questId)
            {
                var current = Tables[(int)questAreaId].Current[questId / 8] & (1 << (questId % 8));
                return current != 0;
            }

            /// <summary>
            ﻿/// クエストをクリアしているか
            ﻿/// </summary>
            ﻿/// <param name="questAreaId"></param>
            ﻿/// <param name="questId"></param>
            ﻿/// <returns></returns>
            public readonly bool HasQuestComplete(QuestId questAreaId, int questId)
            {
                var complete = Tables[(int)questAreaId].Complete[questId / 8] & (1 << (questId % 8));
                return complete != 0;
            }
        }

        /// <summary>
        /// ゾーン情報の構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ZoneRecord
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38, ArraySubType = UnmanagedType.U1)]
            public byte[] Zone;

            public ZoneRecord()
            {
                Zone = new byte[_MAX_ZONE_TABLE];
            }
        }

        /// <summary>
        /// Zoneテーブル
        /// </summary>
        public struct ZoneList
        {
            public ZoneRecord Record;

            public ZoneList()
            {
                Record = new();
            }

            /// <summary>
            /// 指定のゾーンに入ったことあるか
            /// </summary>
            /// <param name="zoneId"></param>
            /// <returns></returns>
            public readonly bool HasZone(ZoneId zoneId)
            {
                var table = (int)zoneId / 8;
                var bit = 1 << (int)zoneId % 8;

                if (table >= _MAX_ZONE_TABLE)
                {
                    return false;
                }

                return (Record.Zone[table] & bit) > 0;
            }
        }

        /// <summary>
        /// だいじなものテーブルの構造体
        /// </summary>
        public struct DBKeyitems(int size)
        {
            public BitArray KeyList = new(size);
            public BitArray SeenList = new(size);
        }

        /// <summary>
        /// だいじなものテーブル
        /// </summary>
        public struct KeyItems
        {
            public DBKeyitems[] Tables;

            public KeyItems(int tableCount, int bitSize)
            {
                Tables = new DBKeyitems[tableCount];
                for (int i = 0; i < tableCount; i++)
                {
                    Tables[i] = new DBKeyitems(bitSize);
                }
            }

            /// <summary>
            /// だいじなものを所持しているか
            /// </summary>
            /// <param name="keyitems"></param>
            /// <param name="KeyItemID"></param>
            /// <returns></returns>
            public readonly bool HasKeyItem(KeyItemId KeyItemID)
            {
                var table = (int)KeyItemID / 512;
                var id = (int)KeyItemID % 512;

                if (table >= _MAX_KEYS_TABLE)
                {
                    return false;
                }

                return Tables[table].KeyList[id];
            }
        }

        /// <summary>
        ﻿/// テレポートポイントの構造体
        ﻿/// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TelePoint
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
            public uint[] Access;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I4)]
            public int[] Menu;

            public TelePoint()
            {
                Access = new uint[4];
                Menu = new int[10];
            }
        }

        /// <summary>
        ﻿/// WayPointの構造体 (Sample1: WayPoint)
        ﻿/// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WayPoint
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
            public uint[] Access;
            [MarshalAs(UnmanagedType.U1)]
            public bool Confirmation;

            public WayPoint()
            {
                Access = new uint[2];
                Confirmation = false;
            }
        }

        /// <summary>
        /// テレポートの情報
        ﻿/// </summary>
        public struct TeleportInfo
        {
            public uint OutpostSandy;
            public uint OutpostBastok;
            public uint OutpostWindy;
            public uint RunicPortal;
            public uint PastMaw;
            public uint CampaignSandy;
            public uint CampaignBastok;
            public uint CampaignWindy;
            public TelePoint Homepoint;
            public TelePoint Survival;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.U1)]
            public byte[] AbysseaConflux;
            public WayPoint Waypoints;
            public uint EschanPortal;

            public TeleportInfo()
            {
                OutpostSandy = 0;
                OutpostBastok = 0;
                OutpostWindy = 0;
                RunicPortal = 0;
                PastMaw = 0;
                CampaignSandy = 0;
                CampaignBastok = 0;
                CampaignWindy = 0;
                Homepoint = new();
                Survival = new();
                AbysseaConflux = new byte[9];
                Waypoints = new();
                EschanPortal = 0;
            }

            /// <summary>
            ﻿/// PastMawが開いているか確認する
            ﻿/// </summary>
            ﻿/// <param name="pastMaw"></param>
            ﻿/// <returns></returns>
            public readonly bool IsPastMawOpen(int pastMaw)
            {
                return (PastMaw & (1 << pastMaw)) != 0;
            }

            /// <summary>
            ﻿/// runicPortalが開いているか確認する
            ﻿/// </summary>
            ﻿/// <param name="portal"></param>
            ﻿/// <returns></returns>
            public readonly bool IsRunicPortalOpen(RunicPortalId portal)
            {
                return (RunicPortal & (1 << (int)portal)) != 0;
            }

            /// <summary>
            /// homepointが開いているか確認する
            ﻿/// </summary>
            ﻿/// <param name="hp"></param>
            ﻿/// <returns></returns>
            public readonly bool IsHomePointOpen(HomePointId hp)
            {
                return (Homepoint.Access[(int)hp / 32] & (1 << ((int)hp % 32))) != 0;
            }

            /// <summary>
            ﻿/// survivalが開いているか確認する
            ﻿/// </summary>
            ﻿/// <param name="book"></param>
            ﻿/// <returns></returns>
            public readonly bool IsSurvivalOpen(SurvivalId book)
            {
                return (Survival.Access[(int)book / 32] & (1 << ((int)book % 32))) != 0;
            }
        }

        /// <summary>
        ﻿/// varパラメータの構造体
        ﻿/// </summary>
        public struct CharacterVariable
        {
            public string Varname;
            public int Value;

            public CharacterVariable()
            {
                Varname = string.Empty;
                Value = 0;
            }
        }

        /// <summary>
        /// varテーブル
        /// </summary>
        public struct CharacterVariableList
        {
            public Dictionary<string, CharacterVariable> VarList;
            public CharacterVariableList()
            {
                VarList = [];
            }
        }


        /// <summary>
        ﻿/// プロフィールの構造体
        ﻿/// </summary>
        public struct CharacterProfile
        {
            public int RankPoints;              // ランクポイント
            public int RankSandoria;            // サンドリアランク
            public int RankBastok;              // バストゥークランク
            public int RankWindurst;            // ウィンダスランク
            public int FameSandoria;            // サンドリア名声
            public int FameBastok;              // バストゥーク名声
            public int FameWindurst;            // ウィンダス名声
            public int FameNorg;                // ノーグ名声
            public int FameJeuno;               // ジュノ名声
            public int FameAbyKonschtat;        // アビセアコンシュタット名声
            public int FameAbyTahrongi;         // アビセアタロンギ名声
            public int FameAbyLatheine;         // アビセアラテーヌ名声
            public int FameAbyMisareaux;        // アビセアミザレオ名声
            public int FameAbyVunkerl;          // アビセアブンカール名声
            public int FameAbyAttohwa;          // アビセアアットワ名声
            public int FameAbyAltepa;           // アビセアアルテパ名声
            public int FameAbyGrauberg;         // アビセアグロウベルグ名声
            public int FameAbyUleguerand;       // アビセアウルガラン名声
            public int FameAdoulin;              // アドゥリン名声

            public CharacterProfile()
            {
                RankPoints = 0;
                RankSandoria = 0;
                RankBastok = 0;
                RankWindurst = 0;
                FameSandoria = 0;
                FameBastok = 0;
                FameWindurst = 0;
                FameNorg = 0;
                FameJeuno = 0;
                FameAbyKonschtat = 0;
                FameAbyTahrongi = 0;
                FameAbyLatheine = 0;
                FameAbyMisareaux = 0;
                FameAbyVunkerl = 0;
                FameAbyAttohwa = 0;
                FameAbyAltepa = 0;
                FameAbyGrauberg = 0;
                FameAbyUleguerand = 0;
                FameAdoulin = 0;
            }

            /// <summary>
            ﻿/// 名声エリア
            ﻿/// </summary>
            public enum FameArea : int
            {
                SANDORIA = 0,
                BASTOK = 1,
                WINDURST = 2,   // Mhaura, Kazham
                JEUNO = 3,
                SELBINA_RABAO = 4,
                NORG = 5,
                ABYSSEA_KONSCHTAT = 6,
                ABYSSEA_TAHRONGI = 7,
                ABYSSEA_LATHEINE = 8,
                ABYSSEA_MISAREAUX = 9,
                ABYSSEA_VUNKERL = 10,
                ABYSSEA_ATTOHWA = 11,
                ABYSSEA_ALTEPA = 12,
                ABYSSEA_GRAUBERG = 13,
                ABYSSEA_ULEGUERAND = 14,
                ADOULIN = 15,
            }

            /// <summary>
            ﻿/// 名声ポイント取得
            ﻿/// </summary>
            ﻿/// <param name="fameArea"></param>
            ﻿/// <returns></returns>
            public readonly int GetFamePoint(int fameArea)
            {
                int fame = 0;
                switch ((FameArea)fameArea)
                {
                    case FameArea.SANDORIA:
                        fame = FameSandoria;
                        break;
                    case FameArea.BASTOK:
                        fame = FameBastok;
                        break;
                    case FameArea.WINDURST:
                        fame = FameWindurst;
                        break;
                    case FameArea.JEUNO:
                        fame = FameJeuno + ((FameSandoria + FameBastok + FameWindurst) / 3);
                        break;
                    case FameArea.SELBINA_RABAO:
                        fame = (FameSandoria + FameBastok) / 2;
                        break;
                    case FameArea.NORG:
                        fame = FameNorg;
                        break;
                    case FameArea.ABYSSEA_KONSCHTAT:
                        fame = FameAbyKonschtat;
                        break;
                    case FameArea.ABYSSEA_TAHRONGI:
                        fame = FameAbyTahrongi;
                        break;
                    case FameArea.ABYSSEA_LATHEINE:
                        fame = FameAbyLatheine;
                        break;
                    case FameArea.ABYSSEA_MISAREAUX:
                        fame = FameAbyMisareaux;
                        break;
                    case FameArea.ABYSSEA_VUNKERL:
                        fame = FameAbyVunkerl;
                        break;
                    case FameArea.ABYSSEA_ATTOHWA:
                        fame = FameAbyAttohwa;
                        break;
                    case FameArea.ABYSSEA_ALTEPA:
                        fame = FameAbyAltepa;
                        break;
                    case FameArea.ABYSSEA_GRAUBERG:
                        fame = FameAbyGrauberg;
                        break;
                    case FameArea.ABYSSEA_ULEGUERAND:
                        fame = FameAbyUleguerand;
                        break;
                    case FameArea.ADOULIN:
                        fame = FameAdoulin;
                        break;
                }

                return fame;
            }

            /// <summary>
            ﻿/// 名声ランク取得
            ﻿/// </summary>
            ﻿/// <param name="fameArea"></param>
            ﻿/// <returns></returns>
            public readonly int GetFameRank(int fameArea)
            {
                int fame = GetFamePoint(fameArea);

                int fameRank = 0;
                if (fame >= 613)
                {
                    fameRank = 9;
                }
                else if (fame >= 550)
                {
                    fameRank = 8;
                }
                else if (fame >= 488)
                {
                    fameRank = 7;
                }
                else if (fame >= 425)
                {
                    fameRank = 6;
                }
                else if (fame >= 325)
                {
                    fameRank = 5;
                }
                else if (fame >= 225)
                {
                    fameRank = 4;
                }
                else if (fame >= 125)
                {
                    fameRank = 3;
                }
                else if (fame >= 50)
                {
                    fameRank = 2;
                }

                if ((fameArea >= (int)FameArea.ABYSSEA_KONSCHTAT) && (fameArea <= (int)FameArea.ABYSSEA_ULEGUERAND) && (fameRank >= 6))
                {
                    // アビセアエリアは名声ランク6でキャップ
                    fameRank = 6;
                }

                return fameRank;
            }
        }

        /// <summary>
        ﻿/// 効果の構造体
        ﻿/// </summary>
        public struct CharacterEffect
        {
            public int EffectId;

            public CharacterEffect()
            {
                EffectId = 0;
            }
        }

        /// <summary>
        ﻿/// スキルの構造体
        ﻿/// </summary>
        public struct CharacterSkill
        {
            public int SkillId;
            public int Value;
            public int Rank;

            public CharacterSkill()
            {
                SkillId = 0;
                Value = 0;
                Rank = 0;
            }
        }

        /// <summary>
        ﻿/// ミッション情報を構造体に格納
        ﻿/// </summary>
        ﻿/// <param name="reader"></param>
        ﻿/// <param name="columnName"></param>
        ﻿/// <returns></returns>
        ﻿/// <exception cref="Exception"></exception>
        private static MissionInfo ExtractMissionsFromBlob(DbDataReader reader, string columnName)
        {
            if (reader == null || reader.IsClosed || !reader.HasRows)
            {
                return new();
            }

            if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                byte[] data = (byte[])reader[columnName];

                // BLOBデータが予期せぬサイズの場合はエラーとする
                int expectedSize = Marshal.SizeOf(typeof(MissionRecord)) * _MAX_MISSIONAREA;
                if (data.Length != expectedSize)
                {
                    throw new Exception($"ミッションのBLOBデータのサイズが一致しません。予期されるサイズ: {expectedSize}, 実際のサイズ: {data.Length}");
                }
                MissionInfo missions = new();

                int byteIndex = 0;
                for (int missionIndex = 0; missionIndex < _MAX_MISSIONAREA; missionIndex++)
                {
                    if (byteIndex + 6 <= data.Length) // 2 for Current, 2 for StatusUpper, 2 for StatusLower
                    {
                        missions.Tables[missionIndex].Current = BitConverter.ToUInt16(data, byteIndex);
                        byteIndex += 2;
                        missions.Tables[missionIndex].StatusUpper = BitConverter.ToUInt16(data, byteIndex);
                        byteIndex += 2;
                        missions.Tables[missionIndex].StatusLower = BitConverter.ToUInt16(data, byteIndex);
                        byteIndex += 2;
                    }
                    else
                    {
                        throw new Exception("MissionRecordのフィールドのデータが不足しています。");
                    }

                    for (int bitIndex = 0; bitIndex < 64; bitIndex++)
                    {
                        if (byteIndex < data.Length)
                        {
                            missions.Tables[missionIndex].Complete[bitIndex] = (data[byteIndex] != 0);
                            byteIndex++;
                        }
                        else
                        {
                            throw new Exception("MissionRecordのComplete配列のデータが不足しています。");
                        }
                    }
                }

                return missions;
            }
            else
            {
                return new(); // NULLの場合
            }
        }

        /// <summary>
        ﻿/// クエストを構造体に格納
        ﻿/// </summary>
        ﻿/// <param name="reader"></param>
        ﻿/// <param name="columnName"></param>
        ﻿/// <returns></returns>
        ﻿/// <exception cref="Exception"></exception>
        private static QuestInfo ExtractQuestsFromBlob(DbDataReader reader, string columnName)
        {
            if (reader == null || reader.IsClosed || !reader.HasRows)
            {
                return new(0, 0);
            }

            if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                byte[] data = (byte[])reader[columnName];

                // BLOBデータが予期せぬサイズの場合はエラーとする
                int expectedSize = (_QUEST_BIT_SIZE + _QUEST_BIT_SIZE) * _MAX_QUESTAREA;
                if (data.Length != expectedSize)
                {
                    throw new Exception($"クエストのBLOBデータのサイズが一致しません。予期されるサイズ: {expectedSize}, 実際のサイズ: {data.Length}");
                }
                QuestInfo quests = new(_MAX_QUESTAREA, _QUEST_BIT_SIZE);

                int byteIndex = 0;
                for (int questIndex = 0; questIndex < _MAX_QUESTAREA; questIndex++)
                {
                    for (int bitIndex = 0; bitIndex < _QUEST_BIT_SIZE; bitIndex++)
                    {
                        if (byteIndex < data.Length)
                        {
                            quests.Tables[questIndex].Current[bitIndex] = data[byteIndex];
                            byteIndex++;
                        }
                        else
                        {
                            throw new Exception("QuestRecordのCurrent配列のデータが不足しています。");
                        }
                    }
                    for (int bitIndex = 0; bitIndex < _QUEST_BIT_SIZE; bitIndex++)
                    {
                        if (byteIndex < data.Length)
                        {
                            quests.Tables[questIndex].Complete[bitIndex] = data[byteIndex];
                            byteIndex++;
                        }
                        else
                        {
                            throw new Exception("QuestRecordのComplete配列のデータが不足しています。");
                        }
                    }
                }
                return quests;
            }
            else
            {
                return new(0, 0); // NULLの場合
            }
        }

        /// <summary>
        /// ゾーン情報を構造体に格納
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static ZoneList ExtractZonesFromBlob(DbDataReader reader, string columnName)
        {
            if (reader == null || reader.IsClosed || !reader.HasRows)
            {
                return new();
            }

            if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                byte[] data = (byte[])reader[columnName];

                // BLOBデータが予期せぬサイズの場合はエラーとする
                if (data.Length != _MAX_ZONE_TABLE)
                {
                    throw new Exception($"ゾーンのBLOBデータのサイズが一致しません。予期されるサイズ: {_MAX_ZONE_TABLE}, 実際のサイズ: {data.Length}");
                }
                ZoneList zones = new();

                for (int zoneIndex = 0; zoneIndex < _MAX_ZONE_TABLE; zoneIndex++)
                {
                    if (zoneIndex < data.Length)
                    {
                        zones.Record.Zone[zoneIndex] = data[zoneIndex];
                    }
                    else
                    {
                        throw new Exception("ZoneRecordのZone配列のデータが不足しています。");
                    }
                }
                return zones;
            }
            else
            {
                return new(); // NULLの場合
            }
        }

        /// <summary>
        /// KeyItemsを構造体に格納
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static KeyItems ExtractKeyitemsFromBlob(DbDataReader reader, string columnName)
        {
            if (reader == null || reader.IsClosed || !reader.HasRows)
            {
                return new KeyItems(0, 0); // または例外をスロー
            }

            if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                byte[] blobData = (byte[])reader[columnName];

                // BLOBデータが予期せぬサイズの場合はエラーとする
                if (blobData.Length != _MAX_KEYS_TABLE * 512 / 8 * 2) // 7 tables * 512 bits / 8 bits/byte * 2 lists
                {
                    // エラーは一旦しない
                    // throw new Exception("BLOB data size mismatch.");
                }

                KeyItems keys = new(_MAX_KEYS_TABLE, 512);

                int byteIndex = 0;
                for (int tableIndex = 0; tableIndex < _MAX_KEYS_TABLE; tableIndex++)
                {
                    for (int listIndex = 0; listIndex < 2; listIndex++)
                    {
                        BitArray currentList = (listIndex == 0) ? keys.Tables[tableIndex].KeyList : keys.Tables[tableIndex].SeenList;
                        for (int bitIndex = 0; bitIndex < 512; bitIndex++)
                        {
                            int bytePos = byteIndex / 8;
                            int bitPos = byteIndex % 8;
                            currentList[bitIndex] = (blobData[bytePos] & (1 << bitPos)) != 0;
                            byteIndex++;
                        }
                    }
                }
                return keys;
            }
            else
            {
                return new KeyItems(0, 0); // NULLの場合
            }
        }

        /// <summary>
        /// エミネンス・レコードを構造体に格納
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static EminenceRecord ExtractEminenceRecordFromBlob(DbDataReader reader)
        {
            var list = new EminenceRecord();
            if (reader == null || reader.IsClosed || !reader.HasRows)
            {
                return list; // または例外をスロー
            }

            while (reader.Read())
            {
                // データの処理
                var category = reader.Get<string>("category");
                var item = reader.Get<string>("item");
                var status = reader.GetInt32("status");

                if (category == EminenceRecordCategory.MISSION.ToString() && item != null)
                {
                    var id = (EminenceRecordMission)System.Enum.Parse(typeof(EminenceRecordMission), item);
                    list.Mission[(int)id] = status;
                }
                else if (category == EminenceRecordCategory.AREA.ToString() && item != null)
                {
                    var id = (EminenceRecordArea)System.Enum.Parse(typeof(EminenceRecordArea), item);
                    list.Area[(int)id] = status;
                }
                else if (category == EminenceRecordCategory.FACE.ToString() && item != null)
                {
                    var id = (EminenceRecordFace)System.Enum.Parse(typeof(EminenceRecordFace), item);
                    list.Face[(int)id] = status;
                }
            }

            return list;
        }

        /// <summary>
        /// インベントリーの構造体
        /// </summary>
        public struct Inventory
        {
            // 鞄の種類
            public int location;
            // アイテムID
            public int itemId;
            // アイテムの個数
            public int quantity;
        }

        /// <summary>
        /// インベントリーの情報取得
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public List<Inventory> GetDBInventoryList(int charaId)
        {
            // キャッシュがあればキャッシュを返す
            if (CacheInventory.TryGetValue(charaId, out List<Inventory>? value))
            {
                return value;
            }

            var InventoryList = new List<Inventory>();

            string query = "SELECT location, itemId, quantity FROM char_inventory WHERE charid = @CharaId";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@charaId", charaId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dBInventory = new Inventory
                    {
                        // データの処理
                        location = reader.GetInt32("location"),
                        itemId = reader.GetInt32("itemId"),
                        quantity = reader.GetInt32("quantity")
                    };
                    InventoryList.Add(dBInventory);
                }
            }

            // キャッシュに追加
            CacheInventory.AddOrUpdate(charaId, InventoryList, (key, oldInfo) => InventoryList);

            return InventoryList;
        }

        /// <summary>
        ﻿/// バイト配列を構造体にデシリアライズする
        ﻿/// </summary>
        ﻿/// <typeparam name="T"></typeparam>
        ﻿/// <param name="data"></param>
        ﻿/// <returns></returns>
        private static T Deserialize<T>(byte[] data) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            if (data == null || data.Length == 0) return default;

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(data, 0, ptr, Math.Min(size, data.Length));
                return Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// DB接続の初期化
        /// </summary>
        /// <param name="connectionString"></param>
        public void DatabaseInitialize(string? connectionString)
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                return;
            }

            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        }

        /// <summary>
        /// CharsテーブルをDBから読み込み、キャッシュする。
        /// </summary>
        /// <param name="charaId"></param>
        public void LoadChars(int charaId)
        {
            try
            {
                string query = "SELECT missions, quests, zones, keyitems, nation, campaign_allegiance FROM chars WHERE charid = @CharaId";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    MissionInfo missionInfo = ExtractMissionsFromBlob(reader, "missions");
                    CacheMissionInfo.AddOrUpdate(charaId, missionInfo, (key, oldInfo) => missionInfo);
                    QuestInfo questInfo = ExtractQuestsFromBlob(reader, "quests");
                    CacheQuestInfo.AddOrUpdate(charaId, questInfo, (key, oldInfo) => questInfo);
                    ZoneList zoneList = ExtractZonesFromBlob(reader, "zones");
                    CacheZoneList.AddOrUpdate(charaId, zoneList, (key, oldInfo) => zoneList);
                    KeyItems keyitems = ExtractKeyitemsFromBlob(reader, "keyitems");
                    CacheKeyItems.AddOrUpdate(charaId, keyitems, (key, oldInfo) => keyitems);
                    CacheNation[charaId] = reader.Get<byte>("nation");
                    CacheCampaignAllegiance[charaId] = reader.Get<byte>("campaign_allegiance");
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// だいじなものをDBから読み込み、キャッシュする。
        /// </summary>
        /// <param name="charaId"></param>
        public void LoadKeyItems(int charaId)
        {
            try
            {
                string query = "SELECT keyitems FROM chars WHERE charid = @CharaId";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    KeyItems keyitems = ExtractKeyitemsFromBlob(reader, "keyitems");
                    CacheKeyItems.AddOrUpdate(charaId, keyitems, (key, oldInfo) => keyitems);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        // だいじなもののキャッシュのフラグを更新する
        public void UpdateKeyItemCache(int charaId, KeyItemId keyItemId, bool hasKeyItem)
        {
            if (!CacheKeyItems.ContainsKey(charaId))
            {
                LoadKeyItems(charaId);
            }

            var keyItems = CacheKeyItems[charaId];
            var table = (int)keyItemId / 512;
            var id = (int)keyItemId % 512;
            if (table < _MAX_KEYS_TABLE)
            {
                keyItems.Tables[table].KeyList[id] = hasKeyItem;
                CacheKeyItems[charaId] = keyItems; // キャッシュを更新
            }
        }

        /// <summary>
        /// だいじなものを更新
        /// </summary>
        public void UpdateKeyItems(int charaId)
        {
            if (!CacheKeyItems.ContainsKey(charaId))
            {
                LoadKeyItems(charaId);
            }

            // KeyItemsのキャッシュをBLOBデータに変換
            byte[] data = new byte[_MAX_KEYS_TABLE * 512 / 8 * 2];
            int byteIndex = 0;
            for (int tableIndex = 0; tableIndex < _MAX_KEYS_TABLE; tableIndex++)
            {
                for (int listIndex = 0; listIndex < 2; listIndex++)
                {
                    BitArray currentList = (listIndex == 0) ? CacheKeyItems[charaId].Tables[tableIndex].KeyList : CacheKeyItems[charaId].Tables[tableIndex].SeenList;
                    for (int bitIndex = 0; bitIndex < 512; bitIndex++)
                    {
                        int bytePos = byteIndex / 8;
                        int bitPos = byteIndex % 8;
                        if (currentList[bitIndex])
                        {
                            data[bytePos] |= (byte)(1 << bitPos);
                        }
                        byteIndex++;
                    }
                }
            }

            string query = "UPDATE chars SET keyitems = @keyItems WHERE charid = @CharaId";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@keyItems", data);
            command.Parameters.AddWithValue("@CharaId", charaId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 変数データをDBから読み込み、キャッシュする。
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public void LoadVariables(int charaId)
        {
            try
            {
                string query = "SELECT varname, value FROM char_vars WHERE charid = @CharaId";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);

                CharacterVariableList varList = new();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string? varname = reader.Get<string>("varname");
                    if (varname != null)
                    {
                        CharacterVariable variable = new()
                        {
                            Varname = varname,
                            Value = reader.GetInt32("value")
                        };
                        varList.VarList[varname] = variable;
                    }
                }
                // キャッシュに追加
                CacheVarList.AddOrUpdate(charaId, varList, (key, oldInfo) => varList);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// テレポート情報をDBから読み込み、キャッシュする。
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public void LoadTeleportInfo(int charaId)
        {
            TeleportInfo teleport = new();
            try
            {
                const string query = "SELECT outpost_sandy, outpost_bastok, outpost_windy, runic_portal, maw, " +
                                     "campaign_sandy, campaign_bastok, campaign_windy, homepoints, survivals, " +
                                     "abyssea_conflux, waypoints, eschan_portals " +
                                     "FROM char_unlocks WHERE charid = @Charid";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    teleport.OutpostSandy = reader.Get<uint>("outpost_sandy");
                    teleport.OutpostBastok = reader.Get<uint>("outpost_bastok");
                    teleport.OutpostWindy = reader.Get<uint>("outpost_windy");
                    teleport.RunicPortal = reader.Get<uint>("runic_portal");
                    teleport.PastMaw = reader.Get<uint>("maw");
                    teleport.CampaignSandy = reader.Get<uint>("campaign_sandy");
                    teleport.CampaignBastok = reader.Get<uint>("campaign_bastok");
                    teleport.CampaignWindy = reader.Get<uint>("campaign_windy");
                    teleport.EschanPortal = reader.Get<uint>("eschan_portals");

                    if (reader["homepoints"] != DBNull.Value)
                    {
                        teleport.Homepoint = Deserialize<TelePoint>((byte[])reader["homepoints"]);
                    }
                    if (reader["survivals"] != DBNull.Value)
                    {
                        teleport.Survival = Deserialize<TelePoint>((byte[])reader["survivals"]);
                    }
                    if (reader["abyssea_conflux"] != DBNull.Value)
                    {
                        teleport.AbysseaConflux = (byte[])reader["abyssea_conflux"];
                    }
                    if (reader["waypoints"] != DBNull.Value)
                    {
                        teleport.Waypoints = Deserialize<WayPoint>((byte[])reader["waypoints"]);
                    }
                }
                CacheTeleportInfo[charaId] = teleport;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// プロフィールデータをDBから読み込み、キャッシュする。
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public void LoadProfile(int charaId)
        {
            CharacterProfile profile = new();
            try
            {
                string query = "SELECT rank_points, rank_sandoria, rank_bastok, rank_windurst, fame_sandoria, fame_bastok, fame_windurst, fame_norg, fame_jeuno, fame_aby_konschtat, fame_aby_tahrongi, fame_aby_latheine, fame_aby_misareaux, fame_aby_vunkerl, fame_aby_attohwa, fame_aby_altepa, fame_aby_grauberg, fame_aby_uleguerand, fame_adoulin FROM char_profile WHERE charid = @CharaId";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);

                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    profile.RankPoints = reader.Get<int>("rank_points");
                    profile.RankSandoria = reader.Get<int>("rank_sandoria");
                    profile.RankBastok = reader.Get<int>("rank_bastok");
                    profile.RankWindurst = reader.Get<int>("rank_windurst");
                    profile.FameSandoria = reader.Get<int>("fame_sandoria");
                    profile.FameBastok = reader.Get<int>("fame_bastok");
                    profile.FameWindurst = reader.Get<int>("fame_windurst");
                    profile.FameNorg = reader.Get<int>("fame_norg");
                    profile.FameJeuno = reader.Get<int>("fame_jeuno");
                    profile.FameAbyKonschtat = reader.Get<int>("fame_aby_konschtat");
                    profile.FameAbyTahrongi = reader.Get<int>("fame_aby_tahrongi");
                    profile.FameAbyLatheine = reader.Get<int>("fame_aby_latheine");
                    profile.FameAbyMisareaux = reader.Get<int>("fame_aby_misareaux");
                    profile.FameAbyVunkerl = reader.Get<int>("fame_aby_vunkerl");
                    profile.FameAbyAttohwa = reader.Get<int>("fame_aby_attohwa");
                    profile.FameAbyAltepa = reader.Get<int>("fame_aby_altepa");
                    profile.FameAbyGrauberg = reader.Get<int>("fame_aby_grauberg");
                    profile.FameAbyUleguerand = reader.Get<int>("fame_aby_uleguerand");
                    profile.FameAdoulin = reader.Get<int>("fame_adoulin");
                }
                CacheProfile[charaId] = profile;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 効果の構造体
        /// </summary>
        public struct CharaEffect
        {
            // 効果ID
            public int effecctid;
        }

        /// <summary>
        /// 効果の情報取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public List<CharaEffect> GetCharaEffect(int charaId)
        {
            // キャッシュがあればキャッシュを返す
            if (CacheCharaEffect.TryGetValue(charaId, out List<CharaEffect>? value))
            {
                return value;
            }

            var effectlist = new List<CharaEffect>();

            string query = "SELECT effectid FROM char_effects WHERE charid = @CharaId";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@CharaId", charaId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var effect = new CharaEffect
                    {
                        effecctid = reader.GetInt32("effectid")
                    };
                    effectlist.Add(effect);
                }
            }

            // キャッシュに追加
            CacheCharaEffect.AddOrUpdate(charaId, effectlist, (key, oldInfo) => effectlist);

            return effectlist;
        }

        /// <summary>
        /// スキルの構造体
        /// </summary>
        public struct CharaSkill
        {
            // スキルID
            public int skillid;
            // スキル値
            public int value;
        }

        /// <summary>
        /// スキルの情報取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public Dictionary<int, CharaSkill> GetDBCharaSkill(int charaId)
        {
            // キャッシュがあればキャッシュを返す
            if (CacheCharaSkill.TryGetValue(charaId, out Dictionary<int, CharaSkill>? value))
            {
                return value;
            }

            var SkillList = new Dictionary<int, CharaSkill>();

            string query = "SELECT skillid, value FROM char_skills WHERE charid = @CharaId";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@CharaId", charaId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var skill = new CharaSkill
                    {
                        skillid = reader.GetInt32("skillid"),
                        value = reader.GetInt32("value")
                    };
                    SkillList.Add(skill.skillid, skill);
                }
            }

            // キャッシュに追加
            CacheCharaSkill.AddOrUpdate(charaId, SkillList, (key, oldInfo) => SkillList);

            return SkillList;
        }

        /// <summary>
        /// ステータスの構造体
        /// </summary>
        public struct CharaStatus
        {
            // メインジョブ
            public int mjob;
            // サポートジョブ
            public int sjob;
        }

        /// <summary>
        /// ステータスの情報取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public CharaStatus GetDBCharaStats(int charaId)
        {
            // キャッシュがあればキャッシュを返す
            if (CacheCharaStatus.TryGetValue(charaId, out CharaStatus value))
            {
                return value;
            }

            var stats = new CharaStatus();

            string query = "SELECT mjob, sjob FROM char_stats WHERE charid = @CharaId";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@CharaId", charaId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    stats.mjob = reader.GetInt32("mjob");
                    stats.sjob = reader.GetInt32("sjob");
                }
            }

            // キャッシュに追加
            CacheCharaStatus.AddOrUpdate(charaId, stats, (key, oldInfo) => stats);

            return stats;
        }

        /// <summary>
        /// ジョブの構造体
        /// </summary>
        public struct CharaJob
        {
            // ジョブレベル
            public List<int> level;
        }

        /// <summary>
        /// ジョブの情報取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public CharaJob GetDBCharaJob(int charaId)
        {
            // キャッシュがあればキャッシュを返す
            if (CacheCharaJob.TryGetValue(charaId, out CharaJob value))
            {
                return value;
            }

            var job = new CharaJob
            {
                level = []
            };

            string query = "SELECT war, mnk, whm, blm, rdm, thf, pld, drk, bst, brd, rng, sam, nin, drg, smn, blu, cor, pup, dnc, sch, geo, run FROM char_jobs WHERE charid = @CharaId";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@CharaId", charaId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    job.level.Add(reader.GetInt32("war"));
                    job.level.Add(reader.GetInt32("mnk"));
                    job.level.Add(reader.GetInt32("whm"));
                    job.level.Add(reader.GetInt32("blm"));
                    job.level.Add(reader.GetInt32("rdm"));
                    job.level.Add(reader.GetInt32("thf"));
                    job.level.Add(reader.GetInt32("pld"));
                    job.level.Add(reader.GetInt32("drk"));
                    job.level.Add(reader.GetInt32("bst"));
                    job.level.Add(reader.GetInt32("brd"));
                    job.level.Add(reader.GetInt32("rng"));
                    job.level.Add(reader.GetInt32("sam"));
                    job.level.Add(reader.GetInt32("nin"));
                    job.level.Add(reader.GetInt32("drg"));
                    job.level.Add(reader.GetInt32("smn"));
                    job.level.Add(reader.GetInt32("blu"));
                    job.level.Add(reader.GetInt32("cor"));
                    job.level.Add(reader.GetInt32("pup"));
                    job.level.Add(reader.GetInt32("dnc"));
                    job.level.Add(reader.GetInt32("sch"));
                    job.level.Add(reader.GetInt32("geo"));
                    job.level.Add(reader.GetInt32("run"));
                }
            }

            // キャッシュに追加
            CacheCharaJob.AddOrUpdate(charaId, job, (key, oldInfo) => job);

            return job;
        }

        /// <summary>
        /// 魔法の構造体
        /// </summary>
        public struct CharaMagic
        {
            // 魔法ID
            public List<int> magicIds;

            /// <summary>
            /// 魔法を所持しているか
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public readonly bool IsMagic(int id)
            {
                foreach (var magicId in magicIds)
                {
                    if (magicId == id)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 魔法の情報取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public CharaMagic GetCharaMagic(int charaId)
        {
            // キャッシュがあればキャッシュを返す
            if (CacheCharaMagic.TryGetValue(charaId, out CharaMagic value))
            {
                return value;
            }

            var magicList = new CharaMagic
            {
                magicIds = []
            };

            string query = "SELECT spellid FROM char_spells WHERE charid = @CharaId";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@CharaId", charaId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var magicId = new int();
                    magicId = reader.GetInt32("spellid");
                    magicList.magicIds.Add(magicId);
                }
            }

            // キャッシュに追加
            CacheCharaMagic.AddOrUpdate(charaId, magicList, (key, oldInfo) => magicList);

            return magicList;
        }

        /// <summary>
        /// 魔法を覚える
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="spellId"></param>
        public void LearnMagic(int charaId, MagicId spellId)
        {
            string query = "INSERT INTO char_spells (charid, spellid) VALUES (@CharaId, @SpellId)";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@CharaId", charaId);
            command.Parameters.AddWithValue("@SpellId", spellId);
            command.ExecuteNonQuery();
            // キャッシュを更新
            if (CacheCharaMagic.TryGetValue(charaId, out CharaMagic magic))
            {
                magic.magicIds.Add((int)spellId);
                CacheCharaMagic[charaId] = magic;
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Look
        {
            public uint face;
            public RaceId race;

            public Look()
            {
                face = 0;
                race = RaceId.NONE;
            }
        }

        /// <summary>
        /// キャラ容姿情報取得
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public Look GetLook(int charaId)
        {
            var look = new Look();
            const string query = "SELECT face, race FROM char_look WHERE charid = @Charid";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@Charid", charaId);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    look.face = reader.Get<uint>("face");
                    look.race = (RaceId)reader.Get<uint>("race");
                }
            }

            return look;
        }

        /// <summary>
        /// 合成倉庫情報
        /// </summary>
        public struct SynergyInventoryItem
        {
            public int Id { get; set; }              // アイテムID
            public int SubId { get; set; }           // サブID
            public int Quantity { get; set; }        // 数量
            public int AuctionHouseId { get; set; }  // 競売ID
            public int StackSize { get; set; }       // スタックサイズ
            public string Name { get; set; }         // アイテム名
        }

        /// <summary>
        /// 合成倉庫情報取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public List<SynergyInventoryItem> GetSynergyInventoryItems(int charaId)
        {
            var inventory = new List<SynergyInventoryItem>();

            string query = "SELECT" +
                "     ci.itemId, ib.subid, ci.quantity, ib.aH, ib.stackSize," +
                "     MAX(sr.Wood) AS Wood," +
                "     MAX(sr.Smith) AS Smith," +
                "     MAX(sr.Gold) AS Gold," +
                "     MAX(sr.Cloth) AS Cloth," +
                "     MAX(sr.Leather) AS Leather," +
                "     MAX(sr.Bone) AS Bone," +
                "     MAX(sr.Alchemy) AS Alchemy," +
                "     MAX(sr.Cook) AS Cook," +
                "     ji.name" +
                " FROM custom_inventory AS ci" +
                " INNER JOIN item_basic AS ib ON ib.itemid = ci.itemId" +
                " INNER JOIN japanese_item AS ji ON ji.itemid = ci.itemId" +
                " LEFT JOIN synth_recipes AS sr ON ib.aH = 0 AND (sr.Ingredient1 = ci.itemId OR sr.Ingredient2 = ci.itemId OR sr.Ingredient3 = ci.itemId OR sr.Ingredient4 = ci.itemId OR sr.Ingredient5 = ci.itemId OR sr.Ingredient6 = ci.itemId OR sr.Ingredient7 = ci.itemId OR sr.Ingredient8 = ci.itemId)" +
                " WHERE ci.charid = @characterId" +
                " GROUP BY ci.itemId";

            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@characterId", charaId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var inventoryItem = new SynergyInventoryItem
                    {
                        // データの処理
                        Id = Convert.ToInt32(reader["itemId"]),
                        SubId = Convert.ToInt32(reader["subid"]),
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        AuctionHouseId = Convert.ToInt32(reader["aH"]),
                        StackSize = Convert.ToInt32(reader["stackSize"])
                    };
                    if (inventoryItem.AuctionHouseId == 0)
                    {
                        var craftSkills = new Dictionary<string, int>
                        {
                            { "Wood", Convert.ToInt32(reader["Wood"]) },
                            { "Smith", Convert.ToInt32(reader["Smith"]) },
                            { "Gold", Convert.ToInt32(reader["Gold"]) },
                            { "Cloth", Convert.ToInt32(reader["Cloth"]) },
                            { "Leather", Convert.ToInt32(reader["Leather"]) },
                            { "Bone", Convert.ToInt32(reader["Bone"]) },
                            { "Alchemy", Convert.ToInt32(reader["Alchemy"]) },
                            { "Cook", Convert.ToInt32(reader["Cook"]) }
                        };

                        int maxSkillLevel = int.MinValue; // 最小のint値で初期化（確実に更新されるように）
                        string maxSkillName = "";

                        foreach (var pair in craftSkills)
                        {
                            if (pair.Value > maxSkillLevel)
                            {
                                maxSkillLevel = pair.Value;
                                maxSkillName = pair.Key;
                            }
                        }

                        switch (maxSkillName)
                        {
                            case "Wood": inventoryItem.AuctionHouseId = (int)AuctionHouseId.WOODWORKING; break;
                            case "Smith": inventoryItem.AuctionHouseId = (int)AuctionHouseId.SMITHING; break;
                            case "Gold": inventoryItem.AuctionHouseId = (int)AuctionHouseId.GOLDSMITHING; break;
                            case "Cloth": inventoryItem.AuctionHouseId = (int)AuctionHouseId.CLOTHCRAFT; break;
                            case "Leather": inventoryItem.AuctionHouseId = (int)AuctionHouseId.LEATHERCRAFT; break;
                            case "Bone": inventoryItem.AuctionHouseId = (int)AuctionHouseId.BONECRAFT; break;
                            case "Alchemy": inventoryItem.AuctionHouseId = (int)AuctionHouseId.ALCHEMY; break;
                            case "Cook": inventoryItem.AuctionHouseId = (int)AuctionHouseId.INGREDIENTS; break;
                            default:
                                break;
                        }
                    }

                    inventoryItem.Name = (string)reader["name"];
                    inventory.Add(inventoryItem);
                }
            }

            return inventory;
        }

        /// <summary>
        /// 合成倉庫からアイテム数を減らす
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
        public void UpdateCustomInventory(int charaId, ItemId itemId, int amount)
        {
            // 減算後に0より大きい場合はUPDATE、0以下の場合はDELETE
            string query = @"
                DELETE FROM custom_inventory 
                WHERE charid = @characterId 
                    AND itemId = @itemId 
                    AND quantity - @amount <= 0;
        
                UPDATE custom_inventory 
                SET quantity = quantity - @amount 
                WHERE charid = @characterId 
                    AND itemId = @itemId 
                    AND quantity - @amount > 0";

            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@characterId", charaId);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.Parameters.AddWithValue("@amount", amount);
            command.ExecuteNonQuery();
        }


        /// <summary>
        /// 合成倉庫からアイテムを削除する
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="itemId"></param>
        public void DeleteCustomInventory(int charaId, ItemId itemId)
        {
            string query = "DELETE FROM custom_inventory WHERE charid = @characterId AND itemId = @itemId";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@characterId", charaId);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 素材倉庫にいれる
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        public void InsertCustomInventory(int charaId, int itemId, int quantity)
        {
            string query = "INSERT INTO custom_inventory (charid, location, slot, itemId, quantity) " +
                "VALUES (@characterId, 0, 0, @itemId, @quantity) " +
                "ON DUPLICATE KEY UPDATE quantity = quantity + @quantity";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@characterId", charaId);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// CharaSkillをUPDATEする
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="skillid"></param>
        /// <param name="addValue">加算する値</param>
        public int UpdateCharaSkill(int charaId, SkillId skillid, int addValue)
        {
            // 既存の値を取得
            int currentValue = 0;
            string selectQuery = "SELECT value FROM char_skills WHERE charid = @characterId AND skillid = @skillId";
            using (MySqlCommand selectCommand = new(selectQuery, _connection))
            {
                selectCommand.Parameters.AddWithValue("@characterId", charaId);
                selectCommand.Parameters.AddWithValue("@skillId", (int)skillid);
                object result = selectCommand.ExecuteScalar();
                if (result != null)
                {
                    currentValue = Convert.ToInt32(result);
                }
            }

            // 新しい値を計算
            int newValue = currentValue + addValue;

            // 新しいランクを計算
            var rank = Math.Max(0, (newValue - 1) / 100);

            string query = "INSERT INTO char_skills (charid, skillid, value, rank) VALUES (@characterId, @skillId, @value, @rank) ON DUPLICATE KEY UPDATE value = value + @addValue, rank = @rank";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@characterId", charaId);
            command.Parameters.AddWithValue("@skillId", (int)skillid);
            command.Parameters.AddWithValue("@value", addValue);
            command.Parameters.AddWithValue("@addValue", addValue);
            command.Parameters.AddWithValue("@rank", rank);
            command.ExecuteNonQuery();

            // 新しいレベル
            var newLevel = Math.Max(0, newValue / 10);

            return newLevel;
        }

        /// <summary>
        /// 送信済みのアイテム情報を削除する
        /// </summary>
        /// <param name="charaId"></param>
        private void DeleteDeliveryBoxSentItem(int charaId)
        {
            string query = "DELETE FROM delivery_box WHERE charid = @characterId AND received = 1";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@characterId", _synergyInventoryCharacterId + charaId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// ポストに送信済みのスロットを得る
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        private List<int> GetDeliveryBoxSentSlots(int charaId)
        {
            var slots = new List<int>();
            string query = "SELECT slot FROM delivery_box WHERE charid = @characterId";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@characterId", _synergyInventoryCharacterId + charaId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    slots.Add(Convert.ToInt32(reader["slot"]));
                }
            }
            return slots;
        }

        /// <summary>
        /// デリバリーボックスにアイテムを追加する
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="itemId"></param>
        /// <param name="subId"></param>
        /// <param name="quantity"></param>
        public void InsertDeliveryBoxItem(int charaId, int itemId, int subId, int quantity, string extra)
        {
            // すでに送り済みのレコードを削除する
            DeleteDeliveryBoxSentItem(charaId);
            // 使われているスロットを取得
            var usedSlots = GetDeliveryBoxSentSlots(charaId);
            // 一番小さい使われていないスロットを探す
            int slot = 0;
            while (usedSlots.Contains(slot))
            {
                slot++;
            }

            // 送り主
            string query = "INSERT INTO delivery_box (charid, charname, box, slot, itemid, itemsubid, quantity, extra, senderid, sender, received, sent) SELECT @senderCharacterId, @senderCharacterName, 2, @slotId, @itemId, @subId, @quantity, UNHEX(@extra), c.charid, c.charname, 0, 1 FROM chars c WHERE c.charid = @characterId;";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@senderCharacterId", _synergyInventoryCharacterId + charaId);
                command.Parameters.AddWithValue("@senderCharacterName", "CustomBox1");
                command.Parameters.AddWithValue("@slotId", slot);
                command.Parameters.AddWithValue("@itemId", itemId);
                command.Parameters.AddWithValue("@subId", subId);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@extra", extra);
                command.Parameters.AddWithValue("@characterId", charaId);
                command.ExecuteNonQuery();
            }

            // 送り先
            query = "INSERT INTO delivery_box (charid, charname, box, slot, itemid, itemsubid, quantity, extra, senderid, sender, received, sent) SELECT c.charid, c.charname, 1, 8, @itemId, @subId, @quantity, UNHEX(@extra), @senderCharacterId, @senderCharacterName, 0, 0 FROM chars c WHERE c.charid = @characterId;";
            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@senderCharacterId", _synergyInventoryCharacterId + charaId);
                command.Parameters.AddWithValue("@senderCharacterName", "CustomBox1");
                command.Parameters.AddWithValue("@itemId", itemId);
                command.Parameters.AddWithValue("@subId", subId);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@extra", extra);
                command.Parameters.AddWithValue("@characterId", charaId);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// アイテム情報
        /// </summary>
        public struct Item
        {
            public int ItemId { get; set; }         // アイテムID
            public int SubId { get; set; }          // サブID
            public string Name { get; set; }        // 名前
            public string Description { get; set; } // 説明
            public int Quantity { get; set; }       // 個数
            public int StackSize { get; set; }      // スタックサイズ
            public int AuctionHouseId { get; set; } // 競売種別ID
            // equipment
            public int Level { get; set; }          // 装備可能なレベル
            public int ItemLevel { get; set; }      // アイテムレベル
            public int SuLevel { get; set; }        // SUレベル
            public int Jobs { get; set; }           // 装備可能なジョブ
        }

        /// <summary>
        /// 合成レシピ情報
        /// </summary>
        public struct SynthesisRecipe
        {
            public int Id { get; set; }                 // レシピID
            public int Desynth { get; set; }            // 分解フラグ
            public int KeyItem { get; set; }            // だいじなものID
            public List<int> CraftRank { get; set; }    // 合成ランク
            public Item Crystal { get; set; }           // クリスタル
            public Item HQCrystal { get; set; }         // 特殊クリスタル
            public List<Item> Ingredient { get; set; }  // 合成素材
            public Item Result { get; set; }            // 完成品
            public Item ResultHQ1 { get; set; }         // 完成品HQ1
            public Item ResultHQ2 { get; set; }         // 完成品HQ2
            public Item ResultHQ3 { get; set; }         // 完成品HQ3
            public int IsOpen { get; set; }             // レシピ開放フラグ
        }

        /// <summary>
        /// アイテム情報をマージする
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        private static Item GetMergeItem(Item src, Item dst)
        {
            var resultItem = dst;
            resultItem.Quantity += src.Quantity;
            return resultItem;
        }

        /// <summary>
        /// 合成レシピのSELECT句
        /// </summary>
        private static readonly string RecipeSelectClause =
            "SELECT" +
            "    sr.ID," +
            "    Desynth," +
            "    KeyItem," +
            "    Wood," +
            "    Smith," +
            "    Gold," +
            "    Cloth," +
            "    Leather," +
            "    Bone," +
            "    Alchemy," +
            "    Cook," +
            "    Crystal," +
            "    HQCrystal," +
            "    Ingredient1," +
            "    Ingredient2," +
            "    Ingredient3," +
            "    Ingredient4," +
            "    Ingredient5," +
            "    Ingredient6," +
            "    Ingredient7," +
            "    Ingredient8," +
            "    Result," +
            "    ResultHQ1," +
            "    ResultHQ2," +
            "    ResultHQ3," +
            "    ResultQty," +
            "    ResultHQ1Qty," +
            "    ResultHQ2Qty," +
            "    ResultHQ3Qty," +
            "    cor.ID AS flag ";

        /// <summary>
        /// アイテム情報のSELECT句
        /// </summary>
        private static readonly string ItemSelectQuery =
            "SELECT" +
            "    ji.itemid," +
            "    ib.subid," +
            "    ji.name," +
            "    ji.description," +
            "    ib.stackSize," +
            "    ib.aH," +
            "    ie.level," +
            "    ie.ilevel," +
            "    ie.su_level," +
            "    ie.jobs," +
            "    iw.dmg," +
            "    iw.delay," +
            "    iw.skill," +
            "    iw.ilvl_skill," +
            "    iw.ilvl_parry," +
            "    iw.ilvl_macc " +
            "FROM japanese_item ji " +
            "INNER JOIN item_basic ib ON ji.itemid = ib.itemid " +
            "LEFT JOIN item_equipment ie ON ji.itemid = ie.itemId " +
            "LEFT JOIN item_weapon iw ON ji.itemid = iw.itemId " +
            "WHERE ji.itemid in ({0});";

        /// <summary>
        /// MySqlDataReaderからレシピ一覧と使用アイテムIDセットを読み取る
        /// </summary>
        private static (Dictionary<int, SynthesisRecipe> recipes, Dictionary<int, bool> items)
            ReadRecipesFromReader(MySqlDataReader reader)
        {
            var recipes = new Dictionary<int, SynthesisRecipe>();
            var items = new Dictionary<int, bool>();

            while (reader.Read())
            {
                var recipe = new SynthesisRecipe
                {
                    Id = Convert.ToInt32(reader["ID"]),
                    Desynth = Convert.ToInt32(reader["Desynth"]),
                    KeyItem = Convert.ToInt32(reader["KeyItem"]),
                    CraftRank = [
                        Convert.ToInt32(reader["Wood"]),
                Convert.ToInt32(reader["Smith"]),
                Convert.ToInt32(reader["Gold"]),
                Convert.ToInt32(reader["Cloth"]),
                Convert.ToInt32(reader["Leather"]),
                Convert.ToInt32(reader["Bone"]),
                Convert.ToInt32(reader["Alchemy"]),
                Convert.ToInt32(reader["Cook"]),
            ],
                    Crystal = new Item { ItemId = Convert.ToInt32(reader["Crystal"]), Quantity = 1 },
                    HQCrystal = new Item { ItemId = Convert.ToInt32(reader["HQCrystal"]), Quantity = 1 },
                    IsOpen = reader["flag"] == DBNull.Value ? 0 : 1,
                };

                // 素材リスト（Ingredient1〜8）を読み取り、同一アイテムをまとめる
                var rawIngredients = Enumerable.Range(1, 8)
                    .Select(i => new Item
                    {
                        ItemId = Convert.ToInt32(reader[$"Ingredient{i}"]),
                        Quantity = 1,
                    })
                    .ToList();

                recipe.Result = new Item { ItemId = Convert.ToInt32(reader["Result"]), Quantity = Convert.ToInt32(reader["ResultQty"]) };
                recipe.ResultHQ1 = new Item { ItemId = Convert.ToInt32(reader["ResultHQ1"]), Quantity = Convert.ToInt32(reader["ResultHQ1Qty"]) };
                recipe.ResultHQ2 = new Item { ItemId = Convert.ToInt32(reader["ResultHQ2"]), Quantity = Convert.ToInt32(reader["ResultHQ2Qty"]) };
                recipe.ResultHQ3 = new Item { ItemId = Convert.ToInt32(reader["ResultHQ3"]), Quantity = Convert.ToInt32(reader["ResultHQ3Qty"]) };

                recipe.Ingredient = MergeIngredients(rawIngredients);

                recipes.Add(recipe.Id, recipe);

                // 後でアイテム情報を一括取得するためにIDを収集
                items[recipe.Crystal.ItemId] = true;
                items[recipe.HQCrystal.ItemId] = true;
                items[recipe.Result.ItemId] = true;
                items[recipe.ResultHQ1.ItemId] = true;
                items[recipe.ResultHQ2.ItemId] = true;
                items[recipe.ResultHQ3.ItemId] = true;
                foreach (var ing in rawIngredients)
                    items[ing.ItemId] = true;
            }

            return (recipes, items);
        }

        /// <summary>
        /// 素材リストの連続する同一アイテムをまとめる
        /// </summary>
        private static List<Item> MergeIngredients(List<Item> rawIngredients)
        {
            var merged = new List<Item>();
            var current = new Item { ItemId = 0 };

            foreach (var item in rawIngredients)
            {
                if (item.ItemId <= 0) continue;

                if (current.ItemId != item.ItemId)
                {
                    if (current.ItemId > 0) merged.Add(current);
                    current = item;
                }
                else
                {
                    current.Quantity++;
                }
            }
            if (current.ItemId > 0) merged.Add(current);

            return merged;
        }

        /// <summary>
        /// アイテムIDの一覧からアイテム情報をDBから取得する
        /// </summary>
        private Dictionary<int, Item> FetchItemDatabase(IEnumerable<int> itemIds)
        {
            var ids = itemIds.ToList();

            // IN句のパラメータ化
            var paramNames = ids.Select((_, i) => $"@itemId{i}").ToList();
            var cmd = new MySqlCommand();
            for (int i = 0; i < ids.Count; i++)
                cmd.Parameters.AddWithValue(paramNames[i], ids[i]);

            var database = new Dictionary<int, Item>();
            using MySqlCommand command = new(string.Format(ItemSelectQuery, string.Join(",", paramNames)), _connection);
            foreach (MySqlParameter p in cmd.Parameters)
                command.Parameters.Add(p.ParameterName, p.MySqlDbType).Value = p.Value;

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var item = new Item
                {
                    ItemId = Convert.ToInt32(reader["itemid"]),
                    SubId = Convert.ToInt32(reader["subid"]),
                    Name = Convert.ToString(reader["name"]) ?? "",
                    Description = Convert.ToString(reader["description"]) ?? "",
                    StackSize = Convert.ToInt32(reader["stackSize"]),
                    AuctionHouseId = Convert.ToInt32(reader["aH"]),
                    Level = reader.IsDBNull(reader.GetOrdinal("level")) ? 0 : Convert.ToInt32(reader["level"]),
                    ItemLevel = reader.IsDBNull(reader.GetOrdinal("ilevel")) ? 0 : Convert.ToInt32(reader["ilevel"]),
                    SuLevel = reader.IsDBNull(reader.GetOrdinal("su_level")) ? 0 : Convert.ToInt32(reader["su_level"]),
                    Jobs = reader.IsDBNull(reader.GetOrdinal("jobs")) ? 0 : Convert.ToInt32(reader["jobs"]),
                };
                database[item.ItemId] = item;
            }

            return database;
        }

        /// <summary>
        /// レシピ辞書の各アイテムにDB情報をマージする
        /// </summary>
        private static void MergeItemsIntoRecipes(Dictionary<int, SynthesisRecipe> recipes, Dictionary<int, Item> itemDatabase)
        {
            foreach (var key in recipes.Keys.ToList())
            {
                var recipe = recipes[key];
                recipe.Crystal = GetMergeItem(recipe.Crystal, itemDatabase[recipe.Crystal.ItemId]);
                recipe.HQCrystal = GetMergeItem(recipe.HQCrystal, itemDatabase[recipe.HQCrystal.ItemId]);
                recipe.Ingredient = [.. recipe.Ingredient
                    .Where(i => i.ItemId > 0)
                    .Select(i => GetMergeItem(i, itemDatabase[i.ItemId]))];
                recipe.Result = GetMergeItem(recipe.Result, itemDatabase[recipe.Result.ItemId]);
                recipe.ResultHQ1 = GetMergeItem(recipe.ResultHQ1, itemDatabase[recipe.ResultHQ1.ItemId]);
                recipe.ResultHQ2 = GetMergeItem(recipe.ResultHQ2, itemDatabase[recipe.ResultHQ2.ItemId]);
                recipe.ResultHQ3 = GetMergeItem(recipe.ResultHQ3, itemDatabase[recipe.ResultHQ3.ItemId]);
                recipes[key] = recipe;
            }
        }

        /// <summary>
        /// レシピ一覧をキャッシュに登録する
        /// </summary>
        private static void UpdateCache(IEnumerable<SynthesisRecipe> recipes)
        {
            foreach (var recipe in recipes)
                CacheSynthesisRecipes.AddOrUpdate(recipe.Id, recipe, (_, _) => recipe);
        }

        /// <summary>
        /// 合成レシピ情報取得（ギルド・ランク指定）
        /// </summary>
        public List<SynthesisRecipe> GetSynthesisRecipes(int charaId, GuildId guildId, CraftRank rank)
        {
            var craftColumn = guildId switch
            {
                GuildId.WOODWORKING => ("Wood", "Smith, Gold, Cloth, Leather, Bone, Alchemy, Cook"),
                GuildId.SMITHING => ("Smith", "Wood, Gold, Cloth, Leather, Bone, Alchemy, Cook"),
                GuildId.GOLDSMITHING => ("Gold", "Wood, Smith, Cloth, Leather, Bone, Alchemy, Cook"),
                GuildId.CLOTHCRAFT => ("Cloth", "Wood, Smith, Gold, Leather, Bone, Alchemy, Cook"),
                GuildId.LEATHERCRAFT => ("Leather", "Wood, Smith, Gold, Cloth, Bone, Alchemy, Cook"),
                GuildId.BONECRAFT => ("Bone", "Wood, Smith, Gold, Cloth, Leather, Alchemy, Cook"),
                GuildId.ALCHEMY => ("Alchemy", "Wood, Smith, Gold, Cloth, Leather, Bone, Cook"),
                GuildId.COOKING => ("Cook", "Wood, Smith, Gold, Cloth, Leather, Bone, Alchemy"),
                _ => (null, null)
            };
            if (craftColumn.Item1 is null) return [];

            var (col, others) = craftColumn;
            var minRank = (int)rank * 10 + 1;
            var maxRank = (int)rank * 10 + 10;

            string query =
                RecipeSelectClause +
                "FROM synth_recipes AS sr " +
                "LEFT JOIN custom_open_recipes AS cor " +
                "ON cor.charid = @CharaId AND sr.ID = cor.ID " +
                $"WHERE {col} BETWEEN @rankMin AND @rankMax AND {col} >= GREATEST({others})";

            Dictionary<int, SynthesisRecipe> recipes;
            Dictionary<int, bool> items;

            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@CharaId", charaId);
                command.Parameters.AddWithValue("@rankMin", minRank);
                command.Parameters.AddWithValue("@rankMax", maxRank);
                using var reader = command.ExecuteReader();
                (recipes, items) = ReadRecipesFromReader(reader);
            }

            if (items.Count == 0) return [];

            MergeItemsIntoRecipes(recipes, FetchItemDatabase(items.Keys));

            int sortIndex = guildId - GuildId.WOODWORKING;
            var sorted = recipes.Values
                .OrderBy(r => r.CraftRank.ElementAtOrDefault(sortIndex))
                .ThenBy(r => r.Result.ItemId)
                .ThenBy(r => r.Id)
                .ToList();

            UpdateCache(sorted);
            return sorted;
        }

        /// <summary>
        /// アイテム別合成レシピ情報取得
        /// </summary>
        public List<SynthesisRecipe> GetSynthesisRecipesByItem(int charaId, AuctionHouseId ahId, int level)
        {
            string query =
                RecipeSelectClause +
                "FROM synth_recipes AS sr " +
                "INNER JOIN item_basic AS ib ON ib.itemid = sr.Result " +
                "LEFT JOIN item_equipment AS ie ON ie.itemId = sr.Result " +
                "LEFT JOIN custom_open_recipes AS cor ON cor.ID = sr.ID AND cor.charid = @CharaId " +
                "WHERE ib.aH = @ahId AND (ie.itemId IS NULL OR (GREATEST(ie.level, ie.ilevel) >= @level AND GREATEST(ie.level, ie.ilevel) < (@level + 10)));";

            Dictionary<int, SynthesisRecipe> recipes;
            Dictionary<int, bool> items;

            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@CharaId", charaId);
                command.Parameters.AddWithValue("@ahId", ahId);
                command.Parameters.AddWithValue("@level", level);
                using var reader = command.ExecuteReader();
                (recipes, items) = ReadRecipesFromReader(reader);
            }

            if (items.Count == 0) return [];

            MergeItemsIntoRecipes(recipes, FetchItemDatabase(items.Keys));

            var sorted = recipes.Values
                .OrderBy(r => Math.Max(r.Result.Level, r.Result.ItemLevel))
                .ThenBy(r => r.Result.ItemId)
                .ThenBy(r => r.Id)
                .ToList();

            UpdateCache(sorted);
            return sorted;
        }

        /// <summary>
        /// IDによる合成レシピ情報取得
        /// </summary>
        public List<SynthesisRecipe> GetSynthesisRecipesById(int charaId, int recipeId)
        {
            string query =
                RecipeSelectClause +
                "FROM synth_recipes AS sr " +
                "LEFT JOIN custom_open_recipes AS cor " +
                "ON cor.charid = @CharaId AND sr.ID = cor.ID " +
                "WHERE sr.ID = @recipeId";

            Dictionary<int, SynthesisRecipe> recipes;
            Dictionary<int, bool> items;

            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@charaId", charaId);
                command.Parameters.AddWithValue("@recipeId", recipeId);
                using var reader = command.ExecuteReader();
                (recipes, items) = ReadRecipesFromReader(reader);
            }

            if (items.Count == 0) return [];

            MergeItemsIntoRecipes(recipes, FetchItemDatabase(items.Keys));

            UpdateCache(recipes.Values);
            return [.. recipes.Values];
        }

        /// <summary>
                 /// レシピをオープンする
                 /// </summary>
                 /// <param name="characterId"></param>
                 /// <param name="id"></param>
        public void InsertOpenRecipe(int charaId, int id)
        {
            string query = "INSERT INTO custom_open_recipes (charid, ID) VALUES (@CharaId, @id);";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@CharaId", charaId);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 合成素材かチェックする
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool CheckIngredientItem(int itemId)
        {
            var result = false;

            string query = "SELECT Result " +
                "FROM synth_recipes " +
                "WHERE" +
                "    Ingredient1 = @itemId1 OR" +
                "    Ingredient2 = @itemId2 OR" +
                "    Ingredient3 = @itemId3 OR" +
                "    Ingredient4 = @itemId4 OR" +
                "    Ingredient5 = @itemId5 OR" +
                "    Ingredient6 = @itemId6 OR" +
                "    Ingredient7 = @itemId7 OR" +
                "    Ingredient8 = @itemId8;";

            using (MySqlCommand command = new(query, _connection))
            {
                command.Parameters.AddWithValue("@itemId1", itemId);
                command.Parameters.AddWithValue("@itemId2", itemId);
                command.Parameters.AddWithValue("@itemId3", itemId);
                command.Parameters.AddWithValue("@itemId4", itemId);
                command.Parameters.AddWithValue("@itemId5", itemId);
                command.Parameters.AddWithValue("@itemId6", itemId);
                command.Parameters.AddWithValue("@itemId7", itemId);
                command.Parameters.AddWithValue("@itemId8", itemId);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 図鑑のリスト取得
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="id"></param>
        public List<int> GetCollectionList(int charaId)
        {
            // CompendiumTypeの数のリストにする
            var list = new List<int>();
            for (int i = 0; i < (int)CompendiumType.Max; i++)
            {
                list.Add(0);
            }

            // アイテム図鑑の達成率を取得する
            try
            {
                string query = @"
                    SELECT
                        COUNT(*) AS total_count,
                        SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                    FROM item_basic AS ib
                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                    WHERE ib.type NOT IN (2, 8) AND ib.itemid NOT IN (4204, 4206, 4208);
                ";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var total_count = Convert.ToInt32(reader["total_count"]);
                    var flag_sum = Convert.ToInt32(reader["flag_sum"]);
                    if (total_count > 0)
                    {
                        var percentage = (int)((double)flag_sum / total_count * 10000);
                        list[(int)CompendiumType.Item] = percentage;
                    }
                }
            }
            catch (Exception)
            {
                return list;
            }

            // 魔法図鑑の達成率を取得する
            try
            {
                string query = "SELECT COUNT(*) AS total_count, SUM(CASE WHEN cs.spellid IS NULL THEN 0 ELSE 1 END) AS flag_sum FROM spell_list AS sl LEFT JOIN char_spells AS cs ON cs.spellid = sl.spellid AND cs.charid = @CharaId";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var total_count = Convert.ToInt32(reader["total_count"]);
                    var flag_sum = Convert.ToInt32(reader["flag_sum"]);

                    if (total_count > 0)
                    {
                        var percentage = (int)((double)flag_sum / total_count * 10000);
                        list[(int)CompendiumType.Magic] = percentage;
                    }
                }
            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// アイテム図鑑のリスト取得
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="id"></param>
        public List<int> GetItemCollectionList(int charaId)
        {
            // ItemDispIdの数のリストにする
            var list = new List<int>();
            for (int i = 0; i < (int)ItemBookCategory.MAX; i++)
            {
                list.Add(0);
            }

            // アイテム図鑑の達成率を取得する
            try
            {
                string[] query_list = [
                    // 武器
                    @"
                        SELECT
                            COUNT(*) AS total_count,
                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM (
                            SELECT ib.itemid
                            FROM item_basic AS ib
                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                            WHERE ib.type = 7 AND ie.slot IN (1, 3)

                            UNION ALL

                            SELECT ib.itemid
                            FROM item_basic AS ib
                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                            WHERE ie.slot = 4 AND iw.skill IN (25, 26, 27, 41, 42, 45)

                            UNION ALL

                            SELECT ib.itemid
                            FROM item_basic AS ib
                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                            WHERE ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ie.jobs <> 131072 AND ib.aH <> 48 AND iw.dmg = 0 AND ie.level > 1 AND iw.delay = 999

                            UNION ALL

                            SELECT ib.itemid
                            FROM item_basic AS ib
                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                            WHERE ie.slot = 8 AND iw.skill = 0 AND stackSize = 1

                            UNION ALL

                            SELECT ib.itemid
                            FROM item_basic AS ib
                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                            WHERE ie.slot = 8 AND iw.skill IN (25, 26, 27)
                        ) AS list
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId;
                    ",
                    // 防具
                    @"
                        SELECT
                            COUNT(*) AS total_count,
                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM
                            item_basic AS ib
                        INNER JOIN
                            item_equipment AS ie ON ie.itemid = ib.itemid
                        LEFT JOIN
                            custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                        WHERE
                            ib.type = 6 AND ie.slot IN (2,16, 32, 64, 128, 256, 512, 1024, 6144, 24576, 32768)
                    ",
                    // その他装備
                    @"
                        SELECT
                            COUNT(*) AS total_count,
                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM (
                            SELECT ib.itemid FROM item_basic AS ib
                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                            WHERE
                                  (ie.slot IN (4, 8) AND iw.skill = 48)
                                  OR (ie.slot = 4 AND iw.skill = 0)
                                  OR (ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs = 256)
                                  OR (ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs = 131072)
                                  OR (ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ie.jobs <> 131072 AND ib.aH = 48)
                                  OR (ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ie.jobs <> 131072 AND ib.aH <> 48
                                      AND (iw.dmg > 0 OR ie.level = 1 OR (iw.dmg = 0 AND ie.level > 1 AND iw.delay < 999)))

                            UNION ALL

                            SELECT ib.itemid FROM item_basic AS ib
                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                            WHERE ie.slot = 2
                        ) AS list
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId;
                    ",
                    // 魔法スクロール
                    @"
                        SELECT
                            COUNT(*) AS total_count,
                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM (
                            SELECT ib.itemid
                            FROM item_basic AS ib
                            WHERE ib.type = 5 AND ib.ah IN (28, 29, 30, 31, 32, 45, 60)
                            UNION ALL
                            SELECT itemid
                            FROM item_basic
                            WHERE subid > 0 AND flags = 61504
                        ) AS list
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId;
                    ",
                    // 薬品
                    @"
                        SELECT
                            COUNT(*) AS total_count,
                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM (
                            SELECT ib.itemid FROM item_basic AS ib
                            WHERE ib.type IN (1, 5) AND ib.ah = 33

                            UNION ALL

                            SELECT ib.itemid FROM item_basic AS ib
                            WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4146, 4147, 4200, 4202, 4210, 4212, 4214, 4254, 4255, 5241, 5242, 5243, 5244, 5245, 5246, 5247, 5248, 5249, 5250, 5251, 5252, 5385, 5386, 5387, 5388, 5389, 5390, 5391, 5392, 5393, 5394, 5395, 5396, 5397, 5434, 5435, 5439, 5440)
                        ) AS list
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId;
                    ",
                    // 調度品
                    @"
                        SELECT COUNT(*) AS total_count, SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM item_basic AS ib
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                        WHERE ib.type = 3
                    ",
                    // 素材
                    @"
                        SELECT COUNT(*) AS total_count, SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM item_basic AS ib
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                        WHERE ib.type IN (1, 5) AND ib.ah IN (38, 39, 40, 41, 42, 43, 44, 63)
                    ",
                    // 食品
                    @"
                        SELECT
                            COUNT(*) AS total_count,
                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM (
                            SELECT ib.itemid
                            FROM item_basic AS ib
                            WHERE
                                ib.type = 5
                                AND (
                                    ib.ah IN (51, 52, 53, 54, 55, 56, 57, 58, 59)
                                    OR (ib.ah = 0 AND ib.itemid IN (
                                        4501, 4562,
                                        5226,
                                        5227,
                                        4511, 4569, 5210, 5222,
                                        5224, 5228, 5229,
                                        5223,
                                        4513, 5221,
                                        4508, 4526, 4600, 5154, 5208, 5209, 5225
                                    ))
                                )

                            UNION ALL

                            SELECT
                                ib.itemid
                            FROM item_basic AS ib
                            WHERE ib.type = 1 AND ib.ah IN (51, 59)
                       ) AS list
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId;
                    ",
                    // クリスタル
                    @"
                        SELECT
                            COUNT(*) AS total_count,
                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM (
                            SELECT ib.itemid
                            FROM item_basic AS ib
                            WHERE
                                ib.type = 5
                                AND (
                                    ib.ah = 35
                                    OR (ib.ah = 0 AND ib.itemid IN (
                                        4238, 4239, 4240, 4241, 4242, 4243, 4244, 4245, 6506, 6507, 6508, 6509, 6510, 6511, 6512, 6513
                                    ))
                                )
                       ) AS list
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId;
                    ",
                    // その他
                    @"
                        SELECT
                            COUNT(*) AS total_count,
                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                        FROM (
                            SELECT ib.itemid
                            FROM item_basic AS ib
                            WHERE ib.type = 5
                                AND (
                                    ib.ah IN (46, 64, 15, 36, 49, 37, 48)
                                    OR (ib.ah = 0 AND ib.itemid IN (
                                        4176, 4177, 4178, 4179, 4180,
                                        4230, 4231, 4232, 4233, 4236, 4237,
                                        4351, 4368, 4369,
                                        5109, 5110, 5111, 5112, 5113, 5114, 5115, 5116, 5117, 5118, 5119,
                                        5203, 5204, 5205, 5206,
                                        5256, 5257, 5258, 5259, 5260,
                                        5269, 5270, 5271, 5272, 5273, 5274, 5275, 5276, 5277, 5278, 5279,
                                        5280, 5281, 5282, 5283, 5284, 5285,
                                        5294, 5295, 5296, 5297,
                                        5300, 5301, 5302, 5303,

                                        4181, 4182, 4187, 4188, 4189, 4190, 4191, 4192, 4193, 4194, 4195,
                                        4198, 4247, 4248, 4249,
                                        4258, 4259, 4260, 4261, 4262, 4263, 4264, 4265,
                                        5428, 5988, 5989, 5990
                                    ))
                                )

                            UNION ALL

                            SELECT ib.itemid
                            FROM item_basic AS ib
                            WHERE ib.type = 1
                                AND (
                                    ib.ah IN (46, 64, 65, 50, 36, 49, 37)
                                    OR ib.ah = 0
                                )
                        ) AS list
                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId;
                    ",
                ];
                for (int i = 0; i < query_list.Length; i++)
                {
                    using var command = new MySqlCommand(query_list[i], _connection);
                    command.Parameters.AddWithValue("@CharaId", charaId);

                    using var reader0 = command.ExecuteReader();
                    while (reader0.Read())
                    {
                        var total_count = Convert.ToInt32(reader0["total_count"]);
                        var flag_sum = Convert.ToInt32(reader0["flag_sum"]);

                        if (total_count > 0)
                        {
                            var percentage = (int)((double)flag_sum / total_count * 10000);
                            list[i] = percentage;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// アイテムグループ別図鑑のリスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<int> GetItemGroupCollectionList(int charaId, ItemBookCategory groupId)
        {
            var list = new List<int>();
            try
            {
                switch (groupId)
                {
                    // 武器
                    case ItemBookCategory.WEAPON:
                        for (int i = 0; i < (int)ItemBookWeaponList.MAX; i++)
                        {
                            list.Add(0);
                        }
                        {
                            string query = @"
                                SELECT
                                    iw.skill AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 7 AND ie.slot IN (1, 3)
                                GROUP BY iw.skill

                                UNION ALL

                                SELECT
                                    iw.skill - 12 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 4 AND iw.skill IN (25, 26)
                                GROUP BY iw.skill

                                UNION ALL

                                SELECT
                                    14 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    WHERE ie.slot = 4 AND iw.skill = 27

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    WHERE ie.slot = 8 AND iw.skill = 27

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    WHERE ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1
                                      AND ie.jobs <> 256 AND ie.jobs <> 131072
                                      AND ib.aH <> 48 AND iw.dmg = 0 AND ie.level > 1 AND iw.delay = 999

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    WHERE ie.slot = 8 AND iw.skill = 0 AND ib.stackSize = 1
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    iw.skill - 26 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 4 AND iw.skill IN (41, 42)
                                GROUP BY iw.skill

                                UNION ALL

                                SELECT
                                    17 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 4 AND iw.skill = 45
                                GROUP BY iw.skill

                                UNION ALL

                                SELECT
                                    iw.skill - 7 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 8 AND iw.skill IN (25, 26)
                                GROUP BY iw.skill;
                            ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);

                            using var reader_0_11 = command.ExecuteReader();
                            while (reader_0_11.Read())
                            {
                                var skillid = Convert.ToInt32(reader_0_11["skill"]);
                                var total_count = Convert.ToInt32(reader_0_11["total_count"]);
                                var flag_sum = Convert.ToInt32(reader_0_11["flag_sum"]);
                                if (total_count > 0)
                                {
                                    var percentage = (int)((double)flag_sum / total_count * 10000);
                                    list[skillid] = percentage;
                                }
                            }
                        }
                        break;
                    // 防具
                    case ItemBookCategory.DEFENSE:
                        for (int i = 0; i < (int)ItemBookDefenseList.MAX; i++)
                        {
                            list.Add(0);
                        }
                        {
                            string query = @"
                                SELECT
                                    0 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 2

                                UNION ALL

                                SELECT
                                    1 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 16

                                UNION ALL

                                SELECT
                                    2 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 32

                                UNION ALL

                                SELECT
                                    3 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 64

                                UNION ALL

                                SELECT
                                    4 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 128

                                UNION ALL

                                SELECT
                                    5 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 256

                                UNION ALL

                                SELECT
                                    6 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 512

                                UNION ALL

                                SELECT
                                    7 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 1024

                                UNION ALL

                                SELECT
                                    8 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 32768

                                UNION ALL

                                SELECT
                                    9 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 6144

                                UNION ALL

                                SELECT
                                    10 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 6 AND ie.slot = 24576;
                            ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);

                            using var reader_0_11 = command.ExecuteReader();
                            while (reader_0_11.Read())
                            {
                                var skillid = Convert.ToInt32(reader_0_11["skill"]);
                                var total_count = Convert.ToInt32(reader_0_11["total_count"]);
                                var flag_sum = Convert.ToInt32(reader_0_11["flag_sum"]);
                                if (total_count > 0)
                                {
                                    var percentage = (int)((double)flag_sum / total_count * 10000);
                                    list[skillid] = percentage;
                                }
                            }
                        }
                        break;
                    // その他装備
                    case ItemBookCategory.OTHER_EQUIPMENT:
                        for (int i = 0; i < (int)ItemBookOtherEquipmentList.MAX; i++)
                        {
                            list.Add(0);
                        }
                        {
                            string query = @"
                                SELECT
                                    0 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 4 AND iw.skill = 0 AND iw.subskill > 0

                                UNION ALL

                                SELECT
                                    1 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 2

                                UNION ALL

                                SELECT
                                    2 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    WHERE ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ie.jobs <> 131072 AND ib.aH <> 48 AND iw.dmg > 0

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    WHERE ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ie.jobs <> 131072 AND ib.aH <> 48 AND iw.dmg = 0 AND ie.level > 1 AND iw.delay < 999
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    3 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    WHERE ie.slot = 4 AND iw.skill = 48
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    4 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 8 AND iw.skill = 48

                                UNION ALL

                                SELECT
                                    5 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs = 256

                                UNION ALL

                                SELECT
                                    6 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ib.aH = 48

                                UNION ALL

                                SELECT
                                    8 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs = 131072

                                UNION ALL

                                SELECT
                                    9 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ie.slot = 4 AND iw.skill = 0 AND iw.subskill = 0

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                    INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ie.jobs <> 131072 AND ib.aH <> 48 AND iw.dmg = 0 AND ie.level = 1
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                            ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);

                            using var reader_0_11 = command.ExecuteReader();
                            while (reader_0_11.Read())
                            {
                                var skillid = Convert.ToInt32(reader_0_11["skill"]);
                                var total_count = Convert.ToInt32(reader_0_11["total_count"]);
                                var flag_sum = Convert.ToInt32(reader_0_11["flag_sum"]);
                                if (total_count > 0)
                                {
                                    var percentage = (int)((double)flag_sum / total_count * 10000);
                                    list[skillid] = percentage;
                                }
                            }
                        }
                        break;
                    // 魔法スクロール
                    case ItemBookCategory.MAGIC:
                        for (int i = 0; i < (int)ItemBookMagicList.MAX; i++)
                        {
                            list.Add(0);
                        }
                        {
                            string query = @"
                                SELECT
                                    0 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 28

                                UNION ALL

                                SELECT
                                    1 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 29

                                UNION ALL

                                SELECT
                                    2 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 32

                                UNION ALL

                                SELECT
                                    3 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 31

                                UNION ALL

                                SELECT
                                    4 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 30

                                UNION ALL

                                SELECT
                                    5 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 60

                                UNION ALL

                                SELECT
                                    6 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 45

                                UNION ALL

                                SELECT
                                    7 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    subid > 0 AND flags = 61504
                            ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);

                            using var reader_0_11 = command.ExecuteReader();
                            while (reader_0_11.Read())
                            {
                                var skillid = Convert.ToInt32(reader_0_11["skill"]);
                                var total_count = Convert.ToInt32(reader_0_11["total_count"]);
                                var flag_sum = Convert.ToInt32(reader_0_11["flag_sum"]);
                                if (total_count > 0)
                                {
                                    var percentage = (int)((double)flag_sum / total_count * 10000);
                                    list[skillid] = percentage;
                                }
                            }
                        }
                        break;
                    // 薬品
                    case ItemBookCategory.MEDICINES:
                        // サブカテゴリなし
                        break;
                    // 調度品
                    case ItemBookCategory.FURNISHINGS:
                        // サブカテゴリなし
                        break;
                    // 素材
                    case ItemBookCategory.MATERIALS:
                        for (int i = 0; i < (int)ItemBookMaterialList.MAX; i++)
                        {
                            list.Add(0);
                        }
                        {
                            string query = @"
                                SELECT
                                    0 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 1 AND ib.ah = 38

                                UNION ALL

                                SELECT
                                    1 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 39

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 1 AND ib.ah = 39
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    2 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 40

                                UNION ALL

                                SELECT
                                    3 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 41

                                UNION ALL

                                SELECT
                                    4 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 42

                                UNION ALL

                                SELECT
                                    5 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 43

                                UNION ALL

                                SELECT
                                    6 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 44

                                UNION ALL

                                SELECT
                                    7 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 63
                            ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);

                            using var reader_0_11 = command.ExecuteReader();
                            while (reader_0_11.Read())
                            {
                                var skillid = Convert.ToInt32(reader_0_11["skill"]);
                                var total_count = Convert.ToInt32(reader_0_11["total_count"]);
                                var flag_sum = Convert.ToInt32(reader_0_11["flag_sum"]);
                                if (total_count > 0)
                                {
                                    var percentage = (int)((double)flag_sum / total_count * 10000);
                                    list[skillid] = percentage;
                                }
                            }
                        }
                        break;
                    // 食品
                    case ItemBookCategory.FOOD:
                        for (int i = 0; i < (int)ItemBookFoodList.MAX; i++)
                        {
                            list.Add(0);
                        }
                        {
                            string query = @"
                                SELECT
                                    0 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 52

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (5226)
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    1 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 53

                                UNION ALL

                                SELECT
                                    2 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 54

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (5227)
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    3 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 55

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4511, 4569, 5210, 5222)
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL
                                SELECT
                                    4 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 56

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (5224, 5228, 5229)
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    5 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 57

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (5223)
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    6 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 58

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4513, 5221)
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    7 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 59

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4508, 4526, 4600, 5154, 5208, 5209, 5225)
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    8 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 1 AND ib.ah = 59

                                UNION ALL

                                SELECT
                                    9 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 51

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4501, 4562)

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 1 AND ib.ah = 51
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                            ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);

                            using var reader_0_11 = command.ExecuteReader();
                            while (reader_0_11.Read())
                            {
                                var skillid = Convert.ToInt32(reader_0_11["skill"]);
                                var total_count = Convert.ToInt32(reader_0_11["total_count"]);
                                var flag_sum = Convert.ToInt32(reader_0_11["flag_sum"]);
                                if (total_count > 0)
                                {
                                    var percentage = (int)((double)flag_sum / total_count * 10000);
                                    list[skillid] = percentage;
                                }
                            }
                        }
                        break;
                    // クリスタル
                    case ItemBookCategory.CRYSTAL:
                        // サブカテゴリなし
                        break;
                    // その他
                    case ItemBookCategory.OTHER:
                        for (int i = 0; i < (int)ItemBookOtherList.MAX; i++)
                        {
                            list.Add(0);
                        }
                        {
                            string query = @"
                                SELECT
                                    0 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 46

                                UNION ALL

                                SELECT
                                    1 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 64

                                UNION ALL

                                SELECT
                                    2 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 65

                                UNION ALL

                                SELECT
                                    3 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 1 AND ib.ah IN (0, 47)

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4176, 4177, 4178, 4179, 4180, 4230, 4231, 4232, 4233, 4236, 4237, 4351, 4368, 4369, 5109, 5110, 5111, 5112, 5113, 5114, 5115, 5116, 5117, 5118, 5119, 5203, 5204, 5205, 5206, 5206, 5256, 5257, 5258, 5259, 5260, 5269, 5270, 5271, 5272, 5273, 5274, 5275, 5276, 5277, 5278, 5279, 5280, 5281, 5282, 5283, 5284, 5285, 5284, 5285, 5294, 5295, 5296, 5297, 5300, 5301, 5302, 5303)
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    4 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 15

                                UNION ALL

                                SELECT
                                    5 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 50

                                UNION ALL

                                SELECT
                                    6 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 36

                                UNION ALL

                                SELECT
                                    7 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 49

                                UNION ALL

                                SELECT
                                    8 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type IN (1, 5) AND ib.ah = 37

                                UNION ALL

                                SELECT
                                    9 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM (
                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 4

                                    UNION ALL

                                    SELECT ib.itemid FROM item_basic AS ib
                                    LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                    WHERE ib.type = 1 AND ib.ah = 61
                                ) AS list
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId

                                UNION ALL

                                SELECT
                                    10 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 48

                                UNION ALL

                                SELECT
                                    11 AS skill,
                                    COUNT(*) AS total_count,
                                    SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                FROM
                                    item_basic AS ib
                                LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                WHERE
                                    ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4181, 4182, 4187, 4188, 4189, 4190, 4191, 4192, 4193, 4194, 4195, 4198, 4247, 4248, 4249, 4258, 4259, 4260, 4261, 4262, 4263, 4264, 4265, 5428, 5988, 5989, 5990)
                            ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);

                            using var reader_0_11 = command.ExecuteReader();
                            while (reader_0_11.Read())
                            {
                                var skillid = Convert.ToInt32(reader_0_11["skill"]);
                                var total_count = Convert.ToInt32(reader_0_11["total_count"]);
                                var flag_sum = Convert.ToInt32(reader_0_11["flag_sum"]);
                                if (total_count > 0)
                                {
                                    var percentage = (int)((double)flag_sum / total_count * 10000);
                                    list[skillid] = percentage;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// アイテムレベル別図鑑のリスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="groupId"></param>
        /// <param name="subGroupId"></param>
        /// <returns></returns>
        public List<int> GetItemLevelCollectionList(int charaId, ItemBookCategory groupId, int subGroupId)
        {
            int[] minLevelList99 = [1, 11, 21, 31, 41, 51, 61, 71, 81, 91];
            int[] maxLevelList99 = [10, 20, 30, 40, 50, 60, 70, 80, 90, 99];
            int[] minLevelList119 = [1, 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 111, 119];
            int[] maxLevelList119 = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 118, 119];
            var list = new List<int>();
            try
            {
                switch (groupId)
                {
                    // 武器
                    case ItemBookCategory.WEAPON:
                        switch ((ItemBookWeaponList)subGroupId)
                        {
                            case ItemBookWeaponList.H2H:
                            case ItemBookWeaponList.DAGGER:
                            case ItemBookWeaponList.SWORD:
                            case ItemBookWeaponList.GREATSWORD:
                            case ItemBookWeaponList.AXE:
                            case ItemBookWeaponList.GREATAXE:
                            case ItemBookWeaponList.SCYTHE:
                            case ItemBookWeaponList.POLEARM:
                            case ItemBookWeaponList.KATANA:
                            case ItemBookWeaponList.GREATKATANA:
                            case ItemBookWeaponList.CLUB:
                            case ItemBookWeaponList.STAFF:
                                {
                                    int[] total_count_level = new int[(int)ItemBookLevelList119.MAX];
                                    int[] flag_sum_level = new int[(int)ItemBookLevelList119.MAX];
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        list.Add(0);
                                        total_count_level[i] = 0;
                                        flag_sum_level[i] = 0;
                                    }
                                    string query = @"
                                        SELECT
                                            list.level,
                                            COUNT(*) AS total_count,
                                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                        FROM (
                                            SELECT ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                            WHERE ib.type = 7 AND ie.slot IN (1, 3) AND iw.skill = @SubGroupId + 1
                                        ) AS list
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                                        GROUP BY list.level
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    using var reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        var level = Convert.ToInt32(reader["level"]);
                                        var total_count = Convert.ToInt32(reader["total_count"]);
                                        var flag_sum = Convert.ToInt32(reader["flag_sum"]);
                                        for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                        {
                                            if (level >= minLevelList119[i] && level <= maxLevelList119[i])
                                            {
                                                total_count_level[i] += total_count;
                                                flag_sum_level[i] += flag_sum;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        if (total_count_level[i] > 0)
                                        {
                                            var percentage = (int)((double)flag_sum_level[i] / total_count_level[i] * 10000);
                                            list[i] = percentage;
                                        }
                                        else
                                        {
                                            list[i] = 0;
                                        }
                                    }
                                }
                                break;
                            case ItemBookWeaponList.ARCHERY:
                            case ItemBookWeaponList.MARKSMANSHIP:
                                {
                                    int[] total_count_level = new int[(int)ItemBookLevelList119.MAX];
                                    int[] flag_sum_level = new int[(int)ItemBookLevelList119.MAX];
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        list.Add(0);
                                        total_count_level[i] = 0;
                                        flag_sum_level[i] = 0;
                                    }
                                    string query = @"
                                        SELECT
                                            list.level,
                                            COUNT(*) AS total_count,
                                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                        FROM (
                                            SELECT iw.skill, ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                            WHERE ie.slot = 4 AND iw.skill = @SubGroupId + 13
                                        ) AS list
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                                        GROUP BY list.level
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    using var reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        var level = Convert.ToInt32(reader["level"]);
                                        var total_count = Convert.ToInt32(reader["total_count"]);
                                        var flag_sum = Convert.ToInt32(reader["flag_sum"]);
                                        for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                        {
                                            if (level >= minLevelList119[i] && level <= maxLevelList119[i])
                                            {
                                                total_count_level[i] += total_count;
                                                flag_sum_level[i] += flag_sum;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        if (total_count_level[i] > 0)
                                        {
                                            var percentage = (int)((double)flag_sum_level[i] / total_count_level[i] * 10000);
                                            list[i] = percentage;
                                        }
                                        else
                                        {
                                            list[i] = 0;
                                        }
                                    }
                                }
                                break;
                            case ItemBookWeaponList.THROWING:
                                {
                                    int[] total_count_level = new int[(int)ItemBookLevelList119.MAX];
                                    int[] flag_sum_level = new int[(int)ItemBookLevelList119.MAX];
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        list.Add(0);
                                        total_count_level[i] = 0;
                                        flag_sum_level[i] = 0;
                                    }
                                    string query = @"
                                        SELECT
                                            list.level,
                                            COUNT(*) AS total_count,
                                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                        FROM (
                                            SELECT iw.skill, ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            WHERE ie.slot IN (4, 8) AND iw.skill = 27

                                            UNION ALL

                                            SELECT iw.skill, ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            WHERE ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ie.jobs <> 131072 AND ib.aH <> 48 AND iw.dmg = 0 AND ie.level > 1 AND iw.delay = 999

                                            UNION ALL

                                            SELECT iw.skill, ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            WHERE ie.slot = 8 AND iw.skill = 0 AND ib.stackSize = 1
                                        ) AS list
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                                        GROUP BY list.level;
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        var level = Convert.ToInt32(reader["level"]);
                                        var total_count = Convert.ToInt32(reader["total_count"]);
                                        var flag_sum = Convert.ToInt32(reader["flag_sum"]);
                                        for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                        {
                                            if (level >= minLevelList119[i] && level <= maxLevelList119[i])
                                            {
                                                total_count_level[i] += total_count;
                                                flag_sum_level[i] += flag_sum;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        if (total_count_level[i] > 0)
                                        {
                                            var percentage = (int)((double)flag_sum_level[i] / total_count_level[i] * 10000);
                                            list[i] = percentage;
                                        }
                                        else
                                        {
                                            list[i] = 0;
                                        }
                                    }
                                }
                                break;
                            case ItemBookWeaponList.STRINGED_INSTRUMENTS:
                            case ItemBookWeaponList.WIND_INSTRUMENT:
                                {
                                    int[] total_count_level = new int[(int)ItemBookLevelList99.MAX];
                                    int[] flag_sum_level = new int[(int)ItemBookLevelList99.MAX];
                                    for (int i = 0; i < (int)ItemBookLevelList99.MAX; i++)
                                    {
                                        list.Add(0);
                                        total_count_level[i] = 0;
                                        flag_sum_level[i] = 0;
                                    }
                                    string query = @"
                                        SELECT
                                            list.level,
                                            COUNT(*) AS total_count,
                                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                        FROM (
                                            SELECT iw.skill, ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                            WHERE ie.slot = 4 AND iw.skill = @SubGroupId + 26
                                        ) AS list
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                                        GROUP BY list.level
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    using var reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        var level = Convert.ToInt32(reader["level"]);
                                        var total_count = Convert.ToInt32(reader["total_count"]);
                                        var flag_sum = Convert.ToInt32(reader["flag_sum"]);
                                        for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                        {
                                            if (level >= minLevelList99[i] && level <= maxLevelList99[i])
                                            {
                                                total_count_level[i] += total_count;
                                                flag_sum_level[i] += flag_sum;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < (int)ItemBookLevelList99.MAX; i++)
                                    {
                                        if (total_count_level[i] > 0)
                                        {
                                            var percentage = (int)((double)flag_sum_level[i] / total_count_level[i] * 10000);
                                            list[i] = percentage;
                                        }
                                        else
                                        {
                                            list[i] = 0;
                                        }
                                    }
                                }
                                break;
                            case ItemBookWeaponList.GEOMANTIC_HANDBELL:
                                // レベル別なし
                                break;
                            case ItemBookWeaponList.AMMUNITION:
                            case ItemBookWeaponList.BULLETS:
                                {
                                    int[] total_count_level = new int[(int)ItemBookLevelList119.MAX];
                                    int[] flag_sum_level = new int[(int)ItemBookLevelList119.MAX];
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        list.Add(0);
                                        total_count_level[i] = 0;
                                        flag_sum_level[i] = 0;
                                    }
                                    string query = @"
                                        SELECT
                                            list.level,
                                            COUNT(*) AS total_count,
                                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                        FROM (
                                            SELECT iw.skill, ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                            WHERE ie.slot = 4 AND iw.skill = @SubGroupId + 7
                                        ) AS list
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                                        GROUP BY list.level
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    using var reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        var level = Convert.ToInt32(reader["level"]);
                                        var total_count = Convert.ToInt32(reader["total_count"]);
                                        var flag_sum = Convert.ToInt32(reader["flag_sum"]);
                                        for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                        {
                                            if (level >= minLevelList119[i] && level <= maxLevelList119[i])
                                            {
                                                total_count_level[i] += total_count;
                                                flag_sum_level[i] += flag_sum;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        if (total_count_level[i] > 0)
                                        {
                                            var percentage = (int)((double)flag_sum_level[i] / total_count_level[i] * 10000);
                                            list[i] = percentage;
                                        }
                                        else
                                        {
                                            list[i] = 0;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    // 防具
                    case ItemBookCategory.DEFENSE:
                        int[] defenseSlotList = [2, 16, 32, 64, 128, 256, 512, 1024, 32768, 6144, 24576];
                        switch ((ItemBookDefenseList)subGroupId)
                        {
                            case ItemBookDefenseList.SHIELD:
                            case ItemBookDefenseList.HEAD:
                            case ItemBookDefenseList.BODY:
                            case ItemBookDefenseList.HANDS:
                            case ItemBookDefenseList.LEGS:
                            case ItemBookDefenseList.FEET:
                                {
                                    int[] total_count_level = new int[(int)ItemBookLevelList119.MAX];
                                    int[] flag_sum_level = new int[(int)ItemBookLevelList119.MAX];
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        list.Add(0);
                                        total_count_level[i] = 0;
                                        flag_sum_level[i] = 0;
                                    }
                                    string query = @"
                                        SELECT
                                            list.level,
                                            COUNT(*) AS total_count,
                                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                        FROM (
                                            SELECT ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            WHERE ib.type = 6 AND ie.slot= @SlotId
                                        ) AS list
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                                        GROUP BY list.level
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SlotId", defenseSlotList[subGroupId]);
                                    using var reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        var level = Convert.ToInt32(reader["level"]);
                                        var total_count = Convert.ToInt32(reader["total_count"]);
                                        var flag_sum = Convert.ToInt32(reader["flag_sum"]);
                                        for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                        {
                                            if (level >= minLevelList119[i] && level <= maxLevelList119[i])
                                            {
                                                total_count_level[i] += total_count;
                                                flag_sum_level[i] += flag_sum;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < (int)ItemBookLevelList119.MAX; i++)
                                    {
                                        if (total_count_level[i] > 0)
                                        {
                                            var percentage = (int)((double)flag_sum_level[i] / total_count_level[i] * 10000);
                                            list[i] = percentage;
                                        }
                                        else
                                        {
                                            list[i] = 0;
                                        }
                                    }
                                }
                                break;
                            case ItemBookDefenseList.NECKLACE:
                            case ItemBookDefenseList.WAIST:
                            case ItemBookDefenseList.BACK:
                            case ItemBookDefenseList.EARRINGS:
                            case ItemBookDefenseList.RING:
                                {
                                    int[] total_count_level = new int[(int)ItemBookLevelList99.MAX];
                                    int[] flag_sum_level = new int[(int)ItemBookLevelList99.MAX];
                                    for (int i = 0; i < (int)ItemBookLevelList99.MAX; i++)
                                    {
                                        list.Add(0);
                                        total_count_level[i] = 0;
                                        flag_sum_level[i] = 0;
                                    }
                                    string query = @"
                                        SELECT
                                            list.level,
                                            COUNT(*) AS total_count,
                                            SUM(CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END) AS flag_sum
                                        FROM (
                                            SELECT ie.itemid, CASE WHEN ie.level > ie.ilevel THEN ie.level ELSE ie.ilevel END AS level
                                            FROM item_basic AS ib
                                            INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                            WHERE ib.type = 6 AND ie.slot= @SlotId
                                        ) AS list
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = list.itemid AND cib.charid = @CharaId
                                        GROUP BY list.level
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SlotId", defenseSlotList[subGroupId]);
                                    using var reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        var level = Convert.ToInt32(reader["level"]);
                                        var total_count = Convert.ToInt32(reader["total_count"]);
                                        var flag_sum = Convert.ToInt32(reader["flag_sum"]);
                                        for (int i = 0; i < (int)ItemBookLevelList99.MAX; i++)
                                        {
                                            if (level >= minLevelList99[i] && level <= maxLevelList99[i])
                                            {
                                                total_count_level[i] += total_count;
                                                flag_sum_level[i] += flag_sum;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < (int)ItemBookLevelList99.MAX; i++)
                                    {
                                        if (total_count_level[i] > 0)
                                        {
                                            var percentage = (int)((double)flag_sum_level[i] / total_count_level[i] * 10000);
                                            list[i] = percentage;
                                        }
                                        else
                                        {
                                            list[i] = 0;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    // その他装備
                    case ItemBookCategory.OTHER_EQUIPMENT:
                        // レベル別なし
                        break;
                    // 魔法スクロール
                    case ItemBookCategory.MAGIC:
                        int[] MagicAhList = [28, 29, 32, 31, 30, 60, 45];
                        switch ((ItemBookMagicList)subGroupId)
                        {
                            case ItemBookMagicList.WHITE:
                            case ItemBookMagicList.BLACK:
                            case ItemBookMagicList.SONG:
                            case ItemBookMagicList.NINJUTSU:
                            case ItemBookMagicList.SUMMONING:
                            case ItemBookMagicList.DIE:
                            case ItemBookMagicList.GEOMANCY:
                                {
                                    int[] total_count_level = new int[(int)ItemBookLevelList99.MAX];
                                    int[] flag_sum_level = new int[(int)ItemBookLevelList99.MAX];
                                    for (int i = 0; i < (int)ItemBookLevelList99.MAX; i++)
                                    {
                                        list.Add(0);
                                        total_count_level[i] = 0;
                                        flag_sum_level[i] = 0;
                                    }
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            sl.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN spell_list AS sl ON sl.spellid = ib.subid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type = 5 AND ib.ah = @MagicAh
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@MagicAh", MagicAhList[(int)subGroupId]);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        MagicDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };

                                        var jobs = (reader["jobs"] as byte[]) ?? new byte[(int)JobId.MAX];
                                        if (jobs.All(b => b == 0))
                                        {
                                            // jobsが全て0の場合はリストに入れない
                                            continue;
                                        }
                                        else
                                        {
                                            info.Jobs = [.. jobs.Select(b => (int)b)];
                                            info.MinLevel = jobs.Where(b => b > 0).Min();

                                            for (int i = 0; i < (int)ItemBookLevelList99.MAX; i++)
                                            {
                                                if (info.MinLevel >= minLevelList99[i] && info.MinLevel <= maxLevelList99[i])
                                                {
                                                    total_count_level[i]++;
                                                    flag_sum_level[i] += info.Flag;
                                                }
                                            }
                                        }
                                    }
                                    for (int i = 0; i < (int)ItemBookLevelList99.MAX; i++)
                                    {
                                        if (total_count_level[i] > 0)
                                        {
                                            var percentage = (int)((double)flag_sum_level[i] / total_count_level[i] * 10000);
                                            list[i] = percentage;
                                        }
                                        else
                                        {
                                            list[i] = 0;
                                        }
                                    }
                                }
                                break;
                            // フェイス
                            case ItemBookMagicList.TRUST:
                                // レベル別なし
                                break;
                            default:
                                break;
                        }
                        break;
                    // 薬品
                    case ItemBookCategory.MEDICINES:
                        // レベル別なし
                        break;
                    // 調度品
                    case ItemBookCategory.FURNISHINGS:
                        // レベル別なし
                        break;
                    // 素材
                    case ItemBookCategory.MATERIALS:
                        // レベル別なし
                        break;
                    // 食品
                    case ItemBookCategory.FOOD:
                        // レベル別なし
                        break;
                    // クリスタル
                    case ItemBookCategory.CRYSTAL:
                        // レベル別なし
                        break;
                    // その他
                    case ItemBookCategory.OTHER:
                        // レベル別なし
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// アイテム図鑑の装備詳細リスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="groupId"></param>
        /// <param name="subGroupId"></param>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public List<EquipmentDetailInfo> GetItemEquipmentCollectionListDetail(int charaId, ItemBookCategory groupId, int subGroupId, int minLevel, int maxLevel)
        {
            var list = new List<EquipmentDetailInfo>();
            try
            {
                switch (groupId)
                {
                    // 武器
                    case ItemBookCategory.WEAPON:
                        switch ((ItemBookWeaponList)subGroupId)
                        {
                            case ItemBookWeaponList.H2H:
                            case ItemBookWeaponList.DAGGER:
                            case ItemBookWeaponList.SWORD:
                            case ItemBookWeaponList.GREATSWORD:
                            case ItemBookWeaponList.AXE:
                            case ItemBookWeaponList.GREATAXE:
                            case ItemBookWeaponList.SCYTHE:
                            case ItemBookWeaponList.POLEARM:
                            case ItemBookWeaponList.KATANA:
                            case ItemBookWeaponList.GREATKATANA:
                            case ItemBookWeaponList.CLUB:
                            case ItemBookWeaponList.STAFF:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ib.type = 7
                                            AND ie.slot IN (1, 3)
                                            AND iw.skill = @SubGroupId + 1
                                            AND (
                                                (ie.ilevel <> 0 AND ie.ilevel >= @MinLevel AND ie.ilevel <= @MaxLevel)
                                                OR (ie.ilevel = 0 AND ie.level >= @MinLevel AND ie.level <= @MaxLevel)
                                            )
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    command.Parameters.AddWithValue("@MinLevel", minLevel);
                                    command.Parameters.AddWithValue("@MaxLevel", maxLevel);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            case ItemBookWeaponList.ARCHERY:
                            case ItemBookWeaponList.MARKSMANSHIP:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 4
                                            AND iw.skill = @SubGroupId + 13
                                            AND (
                                                (ie.ilevel <> 0 AND ie.ilevel >= @MinLevel AND ie.ilevel <= @MaxLevel)
                                                OR (ie.ilevel = 0 AND ie.level >= @MinLevel AND ie.level <= @MaxLevel)
                                            )
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    command.Parameters.AddWithValue("@MinLevel", minLevel);
                                    command.Parameters.AddWithValue("@MaxLevel", maxLevel);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            case ItemBookWeaponList.THROWING:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            (
                                                (ie.slot IN (4, 8) AND iw.skill = @SubGroupId + 13)
                                                OR (
                                                    ie.slot = 8
                                                    AND iw.skill = 0
                                                    AND ib.stackSize > 1
                                                    AND ie.jobs <> 256
                                                    AND ie.jobs <> 131072
                                                    AND ib.aH <> 48
                                                    AND iw.dmg = 0
                                                    AND ie.level > 1
                                                    AND iw.delay = 999
                                                )
                                                OR (ie.slot = 8 AND iw.skill = 0 AND ib.stackSize = 1)
                                            )
                                            AND (
                                                (ie.ilevel <> 0 AND ie.ilevel >= @MinLevel AND ie.ilevel <= @MaxLevel)
                                                OR (ie.ilevel = 0 AND ie.level >= @MinLevel AND ie.level <= @MaxLevel)
                                            )
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    command.Parameters.AddWithValue("@MinLevel", minLevel);
                                    command.Parameters.AddWithValue("@MaxLevel", maxLevel);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            case ItemBookWeaponList.STRINGED_INSTRUMENTS:
                            case ItemBookWeaponList.WIND_INSTRUMENT:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 4
                                            AND iw.skill = @SubGroupId + 26
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            case ItemBookWeaponList.GEOMANTIC_HANDBELL:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 4
                                            AND iw.skill = 45
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            case ItemBookWeaponList.AMMUNITION:
                            case ItemBookWeaponList.BULLETS:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 4
                                            AND iw.skill = @SubGroupId + 7
                                            AND (
                                                (ie.ilevel <> 0 AND ie.ilevel >= @MinLevel AND ie.ilevel <= @MaxLevel)
                                                OR (ie.ilevel = 0 AND ie.level >= @MinLevel AND ie.level <= @MaxLevel)
                                            )
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@SubGroupId", subGroupId);
                                    command.Parameters.AddWithValue("@MinLevel", minLevel);
                                    command.Parameters.AddWithValue("@MaxLevel", maxLevel);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    // 防具
                    case ItemBookCategory.DEFENSE:
                        if (subGroupId < 0 || subGroupId > (int)ItemBookDefenseList.MAX)
                        {
                            break;
                        }
                        {
                            int[] defenseSlotList = [2, 16, 32, 64, 128, 256, 512, 1024, 32768, 6144, 24576];
                            string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ib.type = 6
                                            AND ie.slot = @SlotId
                                            AND (
                                                (ie.ilevel <> 0 AND ie.ilevel >= @MinLevel AND ie.ilevel <= @MaxLevel)
                                                OR (ie.ilevel = 0 AND ie.level >= @MinLevel AND ie.level <= @MaxLevel)
                                            )
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);
                            command.Parameters.AddWithValue("@SlotId", defenseSlotList[subGroupId]);
                            command.Parameters.AddWithValue("@MinLevel", minLevel);
                            command.Parameters.AddWithValue("@MaxLevel", maxLevel);
                            using var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                EquipmentDetailInfo info = new()
                                {
                                    Id = Convert.ToInt32(reader["itemid"]),
                                    Level = Convert.ToInt32(reader["level"]),
                                    ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                    Jobs = Convert.ToInt32(reader["jobs"]),
                                    Flag = Convert.ToInt32(reader["flag"])
                                };
                                list.Add(info);
                            }
                        }
                        break;
                    // その他装備
                    case ItemBookCategory.OTHER_EQUIPMENT:
                        switch ((ItemBookOtherEquipmentList)subGroupId)
                        {
                            // ストリンガー
                            case ItemBookOtherEquipmentList.ANIMATOR:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 4
                                            AND iw.skill = 0
                                            AND iw.subskill > 0
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // グリップ
                            case ItemBookOtherEquipmentList.GRIPS:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 2
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 補助装備
                            case ItemBookOtherEquipmentList.SUPPORT_EQUIPMENT:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 8
                                            AND iw.skill = 0
                                            AND ib.stackSize > 1
                                            AND ie.jobs <> 256
                                            AND ie.jobs <> 131072
                                            AND ib.aH <> 48
                                            AND (
                                                iw.dmg > 0
                                                OR (iw.dmg = 0 AND ie.level > 1 AND iw.delay < 999)
                                            )
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 釣り竿
                            case ItemBookOtherEquipmentList.FISHING_ROD:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 4 AND iw.skill = 48
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 釣り餌
                            case ItemBookOtherEquipmentList.FISHING_BAIT:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 8 AND iw.skill = 48
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 獣呼び出しアイテム
                            case ItemBookOtherEquipmentList.PET_ITEMS:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs = 256
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // ペットフード
                            case ItemBookOtherEquipmentList.PET_FOOD:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs <> 256 AND ib.aH = 48
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // からくり回復アイテム
                            case ItemBookOtherEquipmentList.AUTOMATON:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            ie.slot = 8 AND iw.skill = 0 AND ib.stackSize > 1 AND ie.jobs = 131072
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 撮影機
                            case ItemBookOtherEquipmentList.CAMERA:
                                {
                                    string query = @"
                                        SELECT
                                            ie.itemid,
                                            ie.level,
                                            ie.ilevel,
                                            ie.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN item_equipment AS ie ON ie.itemid = ib.itemid
                                        INNER JOIN item_weapon AS iw ON iw.itemid = ib.itemid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            iw.skill = 0
                                            AND (
                                                (ie.slot = 4 AND iw.subskill = 0)
                                                OR (
                                                    ie.slot = 8
                                                    AND ib.stackSize > 1
                                                    AND ie.jobs <> 256
                                                    AND ie.jobs <> 131072
                                                    AND ib.aH <> 48
                                                    AND iw.dmg = 0
                                                    AND ie.level = 1
                                                )
                                            )
                                        ORDER BY ie.ilevel, ie.level, ib.itemid
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        EquipmentDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Level = Convert.ToInt32(reader["level"]),
                                            ItemLevel = Convert.ToInt32(reader["ilevel"]),
                                            Jobs = Convert.ToInt32(reader["jobs"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// アイテム図鑑の魔法詳細リスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="groupId"></param>
        /// <param name="subGroupId"></param>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public List<MagicDetailInfo> GetItemMagicCollectionListDetail(int charaId, ItemBookCategory groupId, int subGroupId, int minLevel, int maxLevel)
        {
            var list = new List<MagicDetailInfo>();
            try
            {
                switch (groupId)
                {
                    // 魔法スクロール
                    case ItemBookCategory.MAGIC:
                        int[] MagicAhList = [28, 29, 32, 31, 30, 60, 45];
                        switch ((ItemBookMagicList)subGroupId)
                        {
                            case ItemBookMagicList.WHITE:
                            case ItemBookMagicList.BLACK:
                            case ItemBookMagicList.SONG:
                            case ItemBookMagicList.NINJUTSU:
                            case ItemBookMagicList.SUMMONING:
                            case ItemBookMagicList.DIE:
                            case ItemBookMagicList.GEOMANCY:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            sl.jobs,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN spell_list AS sl ON sl.spellid = ib.subid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type = 5 AND ib.ah = @MagicAh
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    command.Parameters.AddWithValue("@MagicAh", MagicAhList[(int)subGroupId]);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        MagicDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };

                                        var jobs = (reader["jobs"] as byte[]) ?? new byte[(int)JobId.MAX];
                                        if (jobs.All(b => b == 0))
                                        {
                                            // jobsが全て0の場合はリストに入れない
                                            continue;
                                        }
                                        else
                                        {
                                            info.Jobs = [.. jobs.Select(b => (int)b)];
                                            info.MinLevel = jobs.Where(b => b > 0).Min();

                                            if (info.MinLevel > maxLevel || info.MinLevel < minLevel)
                                            {
                                                // レベル条件に合わない場合はリストに入れない
                                            continue;
                                            }
                                            list.Add(info);
                                        }
                                    }
                                    // MinLevelでソートする
                                    list = list.OrderBy(info => info.MinLevel).ThenBy(info => info.Id).ToList();
                                }
                                break;
                            // フェイス
                            case ItemBookMagicList.TRUST:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        INNER JOIN spell_list AS sl ON sl.spellid = ib.subid
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.subid > 0 AND ib.flags = 61504
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        MagicDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// アイテム図鑑の詳細リスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="groupId"></param>
        /// <param name="subGroupId"></param>
        /// <returns></returns>
        public List<ItemDetailInfo> GetItemCollectionListDetail(int charaId, ItemBookCategory groupId, int subGroupId)
        {
            var list = new List<ItemDetailInfo>();
            try
            {
                switch (groupId)
                {
                    // 薬品
                    case ItemBookCategory.MEDICINES:
                        {
                            string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            (ib.type IN (1, 5) AND ib.ah = 33)
                                            OR (
                                                ib.type = 5
                                                AND ib.ah = 0
                                                AND ib.itemid IN (
                                                    4146, 4147, 4200, 4202, 4210, 4212, 4214, 4254, 4255,
                                                    5241, 5242, 5243, 5244, 5245, 5246, 5247, 5248, 5249,
                                                    5250, 5251, 5252, 5385, 5386, 5387, 5388, 5389, 5390,
                                                    5391, 5392, 5393, 5394, 5395, 5396, 5397, 5434, 5435,
                                                    5439, 5440
                                                )
                                            );
                                    ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);
                            using var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                ItemDetailInfo info = new()
                                {
                                    Id = Convert.ToInt32(reader["itemid"]),
                                    Flag = Convert.ToInt32(reader["flag"])
                                };
                                list.Add(info);
                            }
                        }
                        break;
                    // 調度品
                    case ItemBookCategory.FURNISHINGS:
                        {
                            string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type  = 3
                                    ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);
                            using var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                ItemDetailInfo info = new()
                                {
                                    Id = Convert.ToInt32(reader["itemid"]),
                                    Flag = Convert.ToInt32(reader["flag"])
                                };
                                list.Add(info);
                            }
                        }
                        break;
                    // 素材
                    case ItemBookCategory.MATERIALS:
                        {
                            int[] MaterialList = [38, 39, 40, 41, 42, 43, 44, 63];
                            string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = @MaterialId
                                    ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);
                            command.Parameters.AddWithValue("@MaterialId", MaterialList[(int)subGroupId]);
                            using var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                ItemDetailInfo info = new()
                                {
                                    Id = Convert.ToInt32(reader["itemid"]),
                                    Flag = Convert.ToInt32(reader["flag"])
                                };
                                list.Add(info);
                            }
                        }
                        break;
                    // 食品
                    case ItemBookCategory.FOOD:
                        {
                            int[] FoodList = [52, 53, 54, 55, 56, 57, 58, 59, 51];
                            string[] FoodAddList = [
                                " OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (5226))",
                                "",
                                " OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (5227))",
                                " OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4511, 4569, 5210, 5222))",
                                " OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (5224, 5228, 5229))",
                                " OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (5223))",
                                " OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4513, 5221))",
                                " OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4508, 4526, 4600, 5154, 5208, 5209, 5225))",
                                " OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4501, 4562))"
                            ];
                            string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE (ib.type IN (1, 5) AND ib.ah = @FoodId)
                                    " + FoodAddList[(int)subGroupId];
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);
                            command.Parameters.AddWithValue("@FoodId", FoodList[(int)subGroupId]);
                            using var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                ItemDetailInfo info = new()
                                {
                                    Id = Convert.ToInt32(reader["itemid"]),
                                    Flag = Convert.ToInt32(reader["flag"])
                                };
                                list.Add(info);
                            }
                        }
                        break;
                    // クリスタル
                    case ItemBookCategory.CRYSTAL:
                        {
                            string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE (ib.type = 5 AND ib.ah = 35)
                                            OR (ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4238, 4239, 4240, 4241, 4242, 4243, 4244, 4245, 6506, 6507, 6508, 6509, 6510, 6511, 6512, 6513))
                                    ";
                            using var command = new MySqlCommand(query, _connection);
                            command.Parameters.AddWithValue("@CharaId", charaId);
                            using var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                ItemDetailInfo info = new()
                                {
                                    Id = Convert.ToInt32(reader["itemid"]),
                                    Flag = Convert.ToInt32(reader["flag"])
                                };
                                list.Add(info);
                            }
                        }
                        break;
                    // その他
                    case ItemBookCategory.OTHER:
                        switch ((ItemBookOtherList)subGroupId)
                        {
                            // 雑貨1
                            case ItemBookOtherList.MISC:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 46
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 雑貨2
                            case ItemBookOtherList.MISC_2:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 64
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 雑貨3
                            case ItemBookOtherList.MISC_3:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 65
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 雑貨4
                            case ItemBookOtherList.MISC_4:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE
                                            (ib.type = 1 AND ib.ah IN (0, 47))
                                            OR (
                                                ib.type = 5
                                                AND ib.ah = 0
                                                AND ib.itemid IN (
                                                    4176, 4177, 4178, 4179, 4180, 4230, 4231, 4232, 4233,
                                                    4236, 4237, 4351, 4368, 4369, 5109, 5110, 5111, 5112,
                                                    5113, 5114, 5115, 5116, 5117, 5118, 5119, 5203, 5204,
                                                    5205, 5206, 5256, 5257, 5258, 5259, 5260, 5269, 5270,
                                                    5271, 5272, 5273, 5274, 5275, 5276, 5277, 5278, 5279,
                                                    5280, 5281, 5282, 5283, 5284, 5285, 5294, 5295, 5296,
                                                    5297, 5300, 5301, 5302, 5303
                                                )
                                            );
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 矢・弾
                            case ItemBookOtherList.AMMUNITION:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 15
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 獣人製品
                            case ItemBookOtherList.BEAST_MADE:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 50
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // カード
                            case ItemBookOtherList.CARDS:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 36
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 忍具
                            case ItemBookOtherList.NINJUTSU_TOOLS:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 49
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 呪物
                            case ItemBookOtherList.CURSED_ITEMS:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 37
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // からくり部品
                            case ItemBookOtherList.AUTOMATON:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE (ib.type = 4) OR (ib.type = 1 AND ib.ah = 61)
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // チョコボの餌
                            case ItemBookOtherList.CHOCOBO_FOOD:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type IN (1, 5) AND ib.ah = 48
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            // 強化アイテム
                            case ItemBookOtherList.POWER_ITEM:
                                {
                                    string query = @"
                                        SELECT
                                            ib.itemid,
                                            CASE WHEN cib.itemid IS NULL THEN 0 ELSE 1 END AS flag
                                        FROM item_basic AS ib
                                        LEFT JOIN custom_item_book AS cib ON cib.itemid = ib.itemid AND cib.charid = @CharaId
                                        WHERE ib.type = 5 AND ib.ah = 0 AND ib.itemid IN (4181, 4182, 4187, 4188, 4189, 4190, 4191, 4192, 4193, 4194, 4195, 4198, 4247, 4248, 4249, 4258, 4259, 4260, 4261, 4262, 4263, 4264, 4265, 5428, 5988, 5989, 5990)
                                    ";
                                    using var command = new MySqlCommand(query, _connection);
                                    command.Parameters.AddWithValue("@CharaId", charaId);
                                    using var reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        ItemDetailInfo info = new()
                                        {
                                            Id = Convert.ToInt32(reader["itemid"]),
                                            Flag = Convert.ToInt32(reader["flag"])
                                        };
                                        list.Add(info);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// 魔法図鑑のリスト取得
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="id"></param>
        public List<int> GetMagicCollectionList(int charaId)
        {
            // MagicDispIdの数のリストにする
            var list = new List<int>();
            for (int i = 0; i < (int)MagicDispId.MAX; i++)
            {
                list.Add(0);
            }

            // 魔法図鑑の達成率を取得する
            try
            {
                string query = "SELECT sl.group, COUNT(*) AS total_count, SUM(CASE WHEN cs.spellid IS NULL THEN 0 ELSE 1 END) AS flag_sum FROM spell_list AS sl LEFT JOIN char_spells AS cs ON cs.spellid = sl.spellid AND cs.charid = @CharaId GROUP BY sl.group";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var groupid = Convert.ToInt32(reader["group"]);
                    var total_count = Convert.ToInt32(reader["total_count"]);
                    var flag_sum = Convert.ToInt32(reader["flag_sum"]);

                    if (total_count > 0)
                    {
                        if (groupid <= (int)SpellGroup.NONE || groupid >= (int)SpellGroup.MAX)
                        {
                            continue;
                        }
                        var dispId = SpellGroupConverter.ConvertToDispId((SpellGroup)groupid);

                        var percentage = (int)((double)flag_sum / total_count * 10000);
                        list[(int)dispId] = percentage;
                    }
                }
            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// 魔法グループ別図鑑のリスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<MagicGroupInfo> GetMagicGroupCollectionList(int charaId, SpellGroup groupId)
        {
            var list = new List<MagicGroupInfo>();
            try
            {
                string query = @"
                    SELECT
                        sl.spellid,
                        sl.jobs,
                        CASE WHEN cs.spellid IS NULL THEN 0 ELSE 1 END AS flag
                    FROM spell_list AS sl
                    LEFT JOIN char_spells AS cs ON cs.spellid = sl.spellid AND cs.charid = @CharaId
                    WHERE sl.group = @GroupId
                ";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);
                command.Parameters.AddWithValue("@GroupId", groupId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var info = new MagicGroupInfo
                    {
                        Id = Convert.ToInt32(reader["spellid"])
                    };
                    var jobs = (reader["jobs"] as byte[]) ?? new byte[(int)JobId.MAX];
                    if (jobs.All(b => b == 0))
                    {
                        // jobsが全て0の場合はリストに入れない
                        continue;
                    }
                    else
                    {
                        info.Jobs = [.. jobs.Select(b => (int)b)];
                        info.MinLevel = jobs.Where(b => b > 0).Min();
                        info.Flag = Convert.ToInt32(reader["flag"]);

                        list.Add(info);
                    }
                }
            }
            catch (Exception)
            {
                return list;
            }

            // MinLevelで昇順、Idで昇順にソートする
            list = [.. list.OrderBy(m => m.MinLevel).ThenBy(m => m.Id)];

            return list;
        }

        /// <summary>
        /// ミッションステータス
        /// </summary>
        public enum MissionStatus
        {
            COMPLETED = -1,     // クリア済み
            NOT_STARTED = 0,    // 未受託
        }

        /// <summary>
        /// ミッションリストを取得する
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="id"></param>
        public MissionClearInfo GetMissionList(int charaId)
        {
            var list = new MissionClearInfo();
            try
            {
                string query = "SELECT missions FROM chars WHERE charid = @CharaId";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    MissionInfo missionInfo = ExtractMissionsFromBlob(reader, "missions");
                    CacheMissionInfo.AddOrUpdate(charaId, missionInfo, (key, oldInfo) => missionInfo);

                    // サンドリアの進捗状況
                    {
                        var sandoriaMission = missionInfo.Tables[(int)MissionId.SANDORIA];
                        for (var missionId = 0; missionId < MissionSandoriaTable.GetMissionMax(); missionId++)
                        {
                            var mission = MissionSandoriaTable.ToMissionId(missionId);
                            if (sandoriaMission.Complete[(int)mission])
                            {
                                list.Sandoria[missionId] = (int)MissionStatus.COMPLETED;
                            }
                            else
                            {
                                list.Sandoria[missionId] = (int)MissionStatus.NOT_STARTED;
                            }
                            if ((int)mission == sandoriaMission.Current)
                            {
                                list.Sandoria[missionId] = sandoriaMission.StatusLower;
                            }
                        }
                    }

                    // バストゥーク進捗状況
                    {
                        var bastokMission = missionInfo.Tables[(int)MissionId.BASTOK];
                        for (var missionId = 0; missionId < MissionBastokTable.GetMissionMax(); missionId++)
                        {
                            var mission = MissionBastokTable.ToMissionId(missionId);
                            if (bastokMission.Complete[(int)mission])
                            {
                                list.Bastok[missionId] = (int)MissionStatus.COMPLETED;
                            }
                            else
                            {
                                list.Bastok[missionId] = (int)MissionStatus.NOT_STARTED;
                            }
                            if ((int)mission == bastokMission.Current)
                            {
                                list.Bastok[missionId] = bastokMission.StatusLower;
                            }
                        }
                    }

                    // ウィンダス進捗状況
                    {
                        var windurstMission = missionInfo.Tables[(int)MissionId.WINDURST];
                        for (var missionId = 0; missionId < MissionWindurstTable.GetMissionMax(); missionId++)
                        {
                            var mission = MissionWindurstTable.ToMissionId(missionId);
                            if (windurstMission.Complete[(int)mission])
                            {
                                list.Windurst[missionId] = (int)MissionStatus.COMPLETED;
                            }
                            else
                            {
                                list.Windurst[missionId] = (int)MissionStatus.NOT_STARTED;
                            }
                            if ((int)mission == windurstMission.Current)
                            {
                                list.Windurst[missionId] = windurstMission.StatusLower;
                            }
                        }
                    }

                    // ジラート進捗状況
                    {
                        var zilartMission = missionInfo.Tables[(int)MissionId.ZILART];
                        for (var missionId = 0; missionId < MissionZilartTable.GetMissionMax(); missionId++)
                        {
                            var mission = MissionZilartTable.ToMissionId(missionId);
                            if (zilartMission.Complete[(int)mission])
                            {
                                list.Zilart[missionId] = (int)MissionStatus.COMPLETED;
                            }
                            else
                            {
                                list.Zilart[missionId] = (int)MissionStatus.NOT_STARTED;
                            }
                            if ((int)mission == zilartMission.Current)
                            {
                                list.Zilart[missionId] = zilartMission.StatusLower;
                            }
                        }
                    }

                    // プロマシア進捗状況
                    {
                        var copMission = missionInfo.Tables[(int)MissionId.COP];
                        for (var missionId = 0; missionId < MissionCOPTable.GetMissionMax(); missionId++)
                        {
                            var mission = MissionCOPTable.ToMissionId(missionId);
                            if ((int)mission < copMission.Current)
                            {
                                list.Cop[missionId] = (int)MissionStatus.COMPLETED;
                            }
                            else
                            {
                                list.Cop[missionId] = (int)MissionStatus.NOT_STARTED;
                            }
                            if ((int)mission == copMission.Current)
                            {
                                list.Cop[missionId] = copMission.StatusLower;
                            }
                        }
                    }

                    // アトルガン進捗状況
                    {
                        var toauMission = missionInfo.Tables[(int)MissionId.TOAU];
                        for (var missionId = 0; missionId < (int)MissionTOAU.MAX; missionId++)
                        {
                            if (toauMission.Complete[missionId])
                            {
                                list.Toau[missionId] = (int)MissionStatus.COMPLETED;
                            }
                            else
                            {
                                list.Toau[missionId] = (int)MissionStatus.NOT_STARTED;
                            }
                            if (missionId == toauMission.Current)
                            {
                                list.Toau[missionId] = toauMission.StatusLower;
                            }
                        }
                    }

                    // アルタナ進捗状況
                    {
                        var wotgMission = missionInfo.Tables[(int)MissionId.WOTG];
                        for (var missionId = 0; missionId < (int)MissionWOTG.MAX; missionId++)
                        {
                            if (wotgMission.Complete[missionId])
                            {
                                list.Wotg[missionId] = (int)MissionStatus.COMPLETED;
                            }
                            else
                            {
                                list.Wotg[missionId] = (int)MissionStatus.NOT_STARTED;
                            }
                            if (missionId == wotgMission.Current)
                            {
                                list.Wotg[missionId] = wotgMission.StatusLower;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// エミネンス・レコードのリスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public EminenceRecord GetEminenceRecordList(int charaId)
        {
            var list = new EminenceRecord();
            try
            {
                string query = "SELECT category, item, status FROM custom_char_reward WHERE charid = @CharaId";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@CharaId", charaId);
                using var reader = command.ExecuteReader();
                list = ExtractEminenceRecordFromBlob(reader);
            }
            catch (Exception)
            {
                return list;
            }

            return list;
        }

        /// <summary>
        /// エミネンス・レコードを達成にする
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="category"></param>
        /// <param name="id"></param>
        public void EminenceRecordAchieve(int charaId, EminenceRecordCategory category, int id)
        {
            string item = string.Empty;
            switch (category)
            {
                case EminenceRecordCategory.MISSION:
                    if (id < 0 || id >= (int)EminenceRecordMission.MAX)
                    {
                        return;
                    }
                    item = ((EminenceRecordMission)id).ToString();
                    break;
                case EminenceRecordCategory.AREA:
                    if (id < 0 || id >= (int)EminenceRecordArea.MAX)
                    {
                        return;
                    }
                    item = ((EminenceRecordArea)id).ToString();
                    break;
                case EminenceRecordCategory.FACE:
                    if (id < 0 || id >= (int)EminenceRecordFace.MAX)
                    {
                        return;
                    }
                    item = ((EminenceRecordFace)id).ToString();
                    break;
            }

            string query = "INSERT INTO custom_char_reward (charid, category, item, status) VALUES (@CharaId, @Category, @Item, 1) ON DUPLICATE KEY UPDATE status = 1";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@CharaId", charaId);
            command.Parameters.AddWithValue("@Category", category.ToString());
            command.Parameters.AddWithValue("@Item", item);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// エミネンス・レコードの報酬を受け取る
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="category"></param>
        /// <param name="id"></param>
        public void ReceiveEminenceRecordReward(int charaId, EminenceRecordCategory category, int id)
        {
            string item = string.Empty;
            switch (category)
            {
                case EminenceRecordCategory.MISSION:
                    if (id < 0 || id >= (int)EminenceRecordMission.MAX)
                    {
                        return;
                    }
                    item = ((EminenceRecordMission)id).ToString();
                    break;
                case EminenceRecordCategory.AREA:
                    if (id < 0 || id >= (int)EminenceRecordArea.MAX)
                    {
                        return;
                    }
                    item = ((EminenceRecordArea)id).ToString();
                    break;
                case EminenceRecordCategory.FACE:
                    if (id < 0 || id >= (int)EminenceRecordFace.MAX)
                    {
                        return;
                    }
                    item = ((EminenceRecordFace)id).ToString();
                    break;
            }

            string query = "UPDATE custom_char_reward SET status = 2 WHERE charid = @CharaId AND category = @Category AND item = @Item;";
            using MySqlCommand command = new(query, _connection);
            command.Parameters.AddWithValue("@CharaId", charaId);
            command.Parameters.AddWithValue("@Category", category.ToString());
            command.Parameters.AddWithValue("@Item", item);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// キャッシュから合成レシピ情報を取得する。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SynthesisRecipe? GetCachedSynthesisRecipe(int recipeId)
        {
            if (CacheSynthesisRecipes.TryGetValue(recipeId, out var recipe))
            {
                return recipe;
            }
            return null;
        }

        /// <summary>
        /// すべてのキャッシュをクリアする。
        /// </summary>
        public void ClearAllCaches()
        {
            CacheMissionInfo.Clear();
            CacheQuestInfo.Clear();
            CacheZoneList.Clear();
            CacheKeyItems.Clear();
            CacheVarList.Clear();
            CacheTeleportInfo.Clear();
            CacheNation.Clear();
            CacheCampaignAllegiance.Clear();
            CacheProfile.Clear();
            CacheInventory.Clear();
            CacheCharaEffect.Clear();
            CacheCharaSkill.Clear();
            CacheCharaStatus.Clear();
            CacheCharaJob.Clear();
            CacheCharaMagic.Clear();
            CacheSynthesisRecipes.Clear();
        }

        /// <summary>
        /// キャラクタのミッション情報をキャッシュから取得する。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="missionId"></param>
        /// <returns></returns>
        public bool IsMissionNull(int charaId)
        {
            if (!CacheMissionInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheMissionInfo[charaId].Tables.Length == 0;
        }

        /// <summary>
        /// キャラクタのミッション情報をキャッシュから取得する。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="missionId"></param>
        /// <returns></returns>
        public MissionRecord GetMissionInfo(int charaId, MissionId missionId)
        {
            if (!CacheMissionInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheMissionInfo[charaId].Tables[(int)missionId];
        }

        /// <summary>
        /// キャラクタのミッション進行状況をキャッシュから取得する。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="missionId"></param>
        /// <returns></returns>
        public int GetMissionCurrent(int charaId, MissionKind missionId)
        {
            if (!CacheMissionInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            var result = Convert.ToInt32(CacheMissionInfo[charaId].Tables[(int)missionId].Current);

            return result;
        }

        /// <summary>
        /// キャラクタのミッションStatusUpperをキャッシュから取得する。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="missionId"></param>
        /// <returns></returns>
        public int GetMissionStatusUpper(int charaId, int missionId)
        {
            if (!CacheMissionInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            var result = Convert.ToInt32(CacheMissionInfo[charaId].Tables[missionId].StatusUpper);

            return result;
        }

        /// <summary>
        /// キャラクタのミッションStatusLowerをキャッシュから取得する。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="missionId"></param>
        /// <returns></returns>
        public int GetMissionStatusLower(int charaId, MissionKind missionId)
        {
            if (!CacheMissionInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            var result = Convert.ToInt32(CacheMissionInfo[charaId].Tables[(int)missionId].StatusLower);

            return result;
        }

        /// <summary>
        /// キャラクタのミッションが現在進行中かをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="missionId"></param>
        /// <param name="currentId"></param>
        /// <returns></returns>
        public bool HasMissionCurrent(int charaId, MissionId missionId, int currentId)
        {
            if (!CacheMissionInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheMissionInfo[charaId].HasMissionCurrent(missionId, currentId);
        }

        /// <summary>
        /// キャラクタのミッションが完了しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="missionId"></param>
        /// <param name="currentId"></param>
        /// <returns></returns>
        public bool HasMissionComplete(int charaId, MissionId missionId, int currentId)
        {
            if (!CacheMissionInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheMissionInfo[charaId].HasMissionComplete(missionId, currentId);
        }

        /// <summary>
        /// キャラクタのクエストが現在進行中かをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="questAreaId"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        public bool HasQuestCurrent(int charaId, QuestId questAreaId, int questId)
        {
            if (!CacheQuestInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheQuestInfo[charaId].HasQuestCurrent(questAreaId, questId);
        }

        /// <summary>
        /// キャラクタのクエストが完了しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="questAreaId"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        public bool HasQuestComplete(int charaId, QuestId questAreaId, int questId)
        {
            if (!CacheQuestInfo.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheQuestInfo[charaId].HasQuestComplete(questAreaId, questId);
        }

        /// <summary>
        /// キャラクタがゾーンを解放しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="zondId"></param>
        /// <returns></returns>
        public bool HasZoneId(int charaId, ZoneId zoneId)
        {
            if (!CacheZoneList.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheZoneList[charaId].HasZone(zoneId);
        }

        /// <summary>
        /// キャラクタの変数をキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        public int GetVarNum(int charaId, string varName)
        {
            if (!CacheVarList.ContainsKey(charaId))
            {
                LoadVariables(charaId);
            }

            var result = CacheVarList[charaId].VarList.TryGetValue(varName, out CharacterVariable value) ? value.Value : 0;

            return result;
        }

        /// <summary>
        /// キャラクタがPastMawを解放しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="pastMaw"></param>
        /// <returns></returns>
        public bool IsPastMawOpen(int charaId, int pastMaw)
        {
            if (!CacheTeleportInfo.ContainsKey(charaId))
            {
                LoadTeleportInfo(charaId);
            }

            return CacheTeleportInfo[charaId].IsPastMawOpen(pastMaw);
        }

        /// <summary>
        /// キャラクタがrunicPortalを解放しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="portal"></param>
        /// <returns></returns>
        public bool IsRunicPortalOpen(int charaId, RunicPortalId portal)
        {
            if (!CacheTeleportInfo.ContainsKey(charaId))
            {
                LoadTeleportInfo(charaId);
            }

            return CacheTeleportInfo[charaId].IsRunicPortalOpen(portal);
        }

        /// <summary>
        /// キャラクタがhomepointを解放しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="homepoint"></param>
        /// <returns></returns>
        public bool IsHomePointOpen(int charaId, HomePointId homepoint)
        {
            if (!CacheTeleportInfo.ContainsKey(charaId))
            {
                LoadTeleportInfo(charaId);
            }

            return CacheTeleportInfo[charaId].IsHomePointOpen(homepoint);
        }

        /// <summary>
        /// キャラクタがsurvivalを解放しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        public bool IsSurvivalOpen(int charaId, SurvivalId book)
        {
            if (!CacheTeleportInfo.ContainsKey(charaId))
            {
                LoadTeleportInfo(charaId);
            }

            return CacheTeleportInfo[charaId].IsSurvivalOpen(book);
        }

        /// <summary>
        /// キャラクタがkeyitemを所持しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="keyitemId"></param>
        /// <returns></returns>
        public bool HasKeyItem(int charaId, KeyItemId keyitemId)
        {
            if (!CacheKeyItems.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheKeyItems[charaId].HasKeyItem(keyitemId);
        }

        /// <summary>
        /// キャラクタの所属国をキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public NationId GetNation(int charaId)
        {
            if (!CacheNation.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return (NationId)CacheNation[charaId];
        }

        /// <summary>
        /// アルタナ所属国をキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public byte GetCampaignAllegiance(int charaId)
        {
            if (!CacheCampaignAllegiance.ContainsKey(charaId))
            {
                LoadChars(charaId);
            }

            return CacheCampaignAllegiance[charaId];
        }

        /// <summary>
        /// アイテムを所持しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool HasItem(int charaId, ItemId itemId)
        {
            if (!CacheInventory.ContainsKey(charaId))
            {
                GetDBInventoryList(charaId);
            }

            return CacheInventory[charaId].Any(item => item.itemId == (int)itemId);
        }

        /// <summary>
        /// アイテムを何個持っているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int GetItemCount(int charaId, ItemId itemId)
        {
            if (!CacheInventory.ContainsKey(charaId))
            {
                GetDBInventoryList(charaId);
            }
            return CacheInventory[charaId].Where(item => item.itemId == (int)itemId).Sum(item => item.quantity);
        }

        /// <summary>
        /// 効果IDが存在するかチェック
        /// </summary>
        /// <param name="effectlist"></param>
        /// <param name="checkId"></param>
        /// <returns></returns>
        public bool IsEffect(int charaId, EffectId checkId)
        {
            if (!CacheCharaEffect.ContainsKey(charaId))
            {
                GetCharaEffect(charaId);
            }

            if (CacheCharaEffect[charaId].Any(effect => effect.effecctid == (int)checkId))
            {
                return true;
            }

            return false;
        }

        // スキルリストを取得
        public Dictionary<int, CharaSkill> GetSkillList(int charaId)
        {
            if (!CacheCharaSkill.ContainsKey(charaId))
            {
                GetDBCharaSkill(charaId);
            }

            return CacheCharaSkill[charaId];
        }

        // ステータスを取得
        public CharaStatus GetStatus(int charaId)
        {
            if (!CacheCharaStatus.ContainsKey(charaId))
            {
                GetDBCharaStats(charaId);
            }

            return CacheCharaStatus[charaId];
        }

        // ジョブ情報を取得
        public CharaJob GetJob(int charaId)
        {
            if (!CacheCharaJob.ContainsKey(charaId))
            {
                GetDBCharaJob(charaId);
            }

            return CacheCharaJob[charaId];
        }

        // 魔法情報を取得
        public CharaMagic GetMagic(int charaId)
        {
            if (!CacheCharaMagic.ContainsKey(charaId))
            {
                GetCharaMagic(charaId);
            }

            return CacheCharaMagic[charaId];
        }

        // プロフィールを取得
        public CharacterProfile GetProfile(int charaId)
        {
            if (!CacheProfile.ContainsKey(charaId))
            {
                LoadProfile(charaId);
            }

            return CacheProfile[charaId];
        }

        // クエストが完了しているかをキャッシュから取得する。キャッシュにない場合はDBから読み込む。
        internal bool HasQuestComplete(int charaId, object oTHER_AREAS, int tHE_OLD_LADY)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// クエスチョンマークが必要化チェックする
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        public bool IsExclamationMark(int charaId)
        {
            var result = false;

            // エミネンス・レコードの情報を取得する
            var list = GetEminenceRecordList(charaId);

            // ミッション
            for (var i = 0; i < list.Mission.Length; i++)
            {
                // 未達成を調査する
                if (list.Mission[i] == (int)EminenceRecordStatus.NOT_ACHIEVED)
                {
                    list.Mission[i] = (int)CheckEminenceRecord(charaId, EminenceRecordCategory.MISSION, i);
                    if (list.Mission[i] == (int)EminenceRecordStatus.ACHIEVED)
                    {
                        EminenceRecordAchieve(charaId, EminenceRecordCategory.MISSION, i);
                    }
                }
                // 達成項目があるか調べる
                if (list.Mission[i] == (int)EminenceRecordStatus.ACHIEVED)
                {
                    result = true;
                }
            }
            for (var i = 0; i < list.Area.Length; i++)
            {
                // 未達成を調査する
                if (list.Area[i] == (int)EminenceRecordStatus.NOT_ACHIEVED)
                {
                    list.Area[i] = (int)CheckEminenceRecord(charaId, EminenceRecordCategory.AREA, i);
                    if (list.Area[i] == (int)EminenceRecordStatus.ACHIEVED)
                    {
                        EminenceRecordAchieve(charaId, EminenceRecordCategory.AREA, i);
                    }
                }
                // 達成項目があるか調べる
                if (list.Area[i] == (int)EminenceRecordStatus.ACHIEVED)
                {
                    result = true;
                }
            }
            for (var i = 0; i < list.Face.Length; i++)
            {
                // 未達成を調査する
                if (list.Face[i] == (int)EminenceRecordStatus.NOT_ACHIEVED)
                {
                    list.Face[i] = (int)CheckEminenceRecord(charaId, EminenceRecordCategory.FACE, i);
                    if (list.Face[i] == (int)EminenceRecordStatus.ACHIEVED)
                    {
                        EminenceRecordAchieve(charaId, EminenceRecordCategory.FACE, i);
                    }
                }
                // 達成項目があるか調べる
                if (list.Face[i] == (int)EminenceRecordStatus.ACHIEVED)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// エミネンス・レコードの達成状況を調査する
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="category"></param>
        /// <param name="id"></param>
        public EminenceRecordStatus CheckEminenceRecord(int charaId, EminenceRecordCategory category, int id)
        {
            switch (category)
            {
                case EminenceRecordCategory.MISSION:
                    switch ((EminenceRecordMission)id)
                    {
                        case EminenceRecordMission.TUTORIAL:
                            // チュートリアルミッションの達成状況を調査する
                            {
                                LoadVariables(charaId);
                                var tutorialProgress = GetVarNum(charaId, "TutorialProgress");
                                if (tutorialProgress == 0)
                                {
                                    // チュートリアルをクリアしている
                                    return EminenceRecordStatus.ACHIEVED;
                                }
                            }
                            break;
                        case EminenceRecordMission.MISSION_RANK_3:
                            // ミッションランク3の達成状況を調査する
                            {
                                var isSandoria = HasMissionComplete(charaId, MissionId.SANDORIA, (int)MissionSandoria.JOURNEY_ABROAD);
                                var isBastok = HasMissionComplete(charaId, MissionId.BASTOK, (int)MissionBastok.THE_EMISSARY);
                                var isWindurst = HasMissionComplete(charaId, MissionId.WINDURST, (int)MissionWindurst.THE_THREE_KINGDOMS);

                                if (isSandoria || isBastok || isWindurst)
                                {
                                    return EminenceRecordStatus.ACHIEVED;
                                }
                            }
                            break;
                        case EminenceRecordMission.MISSION_RANK_4:
                            // ミッションランク4の達成状況を調査する
                            {
                                var isSandoria = HasMissionComplete(charaId, MissionId.SANDORIA, (int)MissionSandoria.APPOINTMENT_TO_JEUNO);
                                var isBastok = HasMissionComplete(charaId, MissionId.BASTOK, (int)MissionBastok.JEUNO);
                                var isWindurst = HasMissionComplete(charaId, MissionId.WINDURST, (int)MissionWindurst.A_NEW_JOURNEY);

                                if (isSandoria || isBastok || isWindurst)
                                {
                                    return EminenceRecordStatus.ACHIEVED;
                                }
                            }
                            break;
                        case EminenceRecordMission.MISSION_RANK_5:
                            // ミッションランク5の達成状況を調査する
                            {
                                var isSandoria = HasMissionComplete(charaId, MissionId.SANDORIA, (int)MissionSandoria.MAGICITE);
                                var isBastok = HasMissionComplete(charaId, MissionId.BASTOK, (int)MissionBastok.MAGICITE);
                                var isWindurst = HasMissionComplete(charaId, MissionId.WINDURST, (int)MissionWindurst.MAGICITE);

                                if (isSandoria || isBastok || isWindurst)
                                {
                                    return EminenceRecordStatus.ACHIEVED;
                                }
                            }
                            break;
                        case EminenceRecordMission.ZILART_COMPLETE:
                            // ジラートミッションの達成状況を調査する
                            if (HasMissionComplete(charaId, MissionId.ZILART, (int)MissionZilart.THE_CELESTIAL_NEXUS))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordMission.COP_PARTNERS_WITHOUT_FAME:
                            // プロマシアミッションの「名声なしでパートナーと共にクリア」の達成状況を調査する
                            if (HasMissionComplete(charaId, MissionId.COP, (int)MissionCOP.PARTNERS_WITHOUT_FAME))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordMission.COP_COMPLETE:
                            // プロマシアミッションの達成状況を調査する
                            if (HasMissionComplete(charaId, MissionId.COP, (int)MissionCOP.DAWN))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordMission.TOAU_COMPLETE:
                            // アトルガンミッションの達成状況を調査する
                            if (HasMissionComplete(charaId, MissionId.TOAU, (int)MissionTOAU.THE_EMPRESS_CROWNED))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                    }
                    break;
                case EminenceRecordCategory.AREA:
                    // 未実装
                    return EminenceRecordStatus.NOT_ACHIEVED;
                case EminenceRecordCategory.FACE:
                    switch ((EminenceRecordFace)id)
                    {
                        case EminenceRecordFace.SANDORIA_FACE:
                            // サンドリアのフェイス使用許可証を入手する
                            if (HasKeyItem(charaId, KeyItemId.SAN_DORIA_TRUST_PERMIT))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.BASTOK_FACE:
                            // バストゥークのフェイス使用許可証を入手する
                            if (HasKeyItem(charaId, KeyItemId.BASTOK_TRUST_PERMIT))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.WINDURST_FACE:
                            // ウィンダスのフェイス使用許可証を入手する
                            if (HasKeyItem(charaId, KeyItemId.WINDURST_TRUST_PERMIT))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.KORUMORU:
                            // クエスト「錬金術の実験」をクリアする
                            if (HasQuestComplete(charaId, QuestId.WINDURST, (int)QuestWindurst.NOTHING_MATTERS))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.AAHM:
                            // ジラートミッションで無知のかけらを入手する
                            if (HasKeyItem(charaId, KeyItemId.SHARD_OF_APATHY))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.AAEV:
                            // ジラートミッションで驕慢のかけらを入手する
                            if (HasKeyItem(charaId, KeyItemId.SHARD_OF_ARROGANCE))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.AAMR:
                            // ジラートミッションで怯懦のかけらを入手する
                            if (HasKeyItem(charaId, KeyItemId.SHARD_OF_ENVY))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.AATT:
                            // ジラートミッションで嫉妬のかけらを入手する
                            if (HasKeyItem(charaId, KeyItemId.SHARD_OF_COWARDICE))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.AAGK:
                            // ジラートミッションで憎悪のかけらを入手する
                            if (HasKeyItem(charaId, KeyItemId.SHARD_OF_RAGE))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        case EminenceRecordFace.MONBERAUX:
                            // プロマシアミッション烙印ありてをクリアする
                            if (HasMissionComplete(charaId, MissionId.COP, (int)MissionCOP.SPIRAL))
                            {
                                return EminenceRecordStatus.ACHIEVED;
                            }
                            break;
                        default:
                            break;
                    }

                    break;
            }

            return EminenceRecordStatus.NOT_ACHIEVED;
        }
    }
}
