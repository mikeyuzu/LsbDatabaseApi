using LsbDatabaseApi.@struct;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using static LsbDatabaseApi.MessageParam;

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

        // キャッシュ
        private readonly ConcurrentDictionary<int, MissionInfo> CacheMissionInfo = new();                   // ミッション
        private readonly ConcurrentDictionary<int, QuestInfo> CacheQuestInfo = new();                       // クエスト
        private readonly ConcurrentDictionary<int, ZoneList> CacheZoneList = new();                         // ゾーン
        private readonly ConcurrentDictionary<int, KeyItems> CacheKeyItems = new();                        // だいじなもの
        private readonly ConcurrentDictionary<int, byte> CacheNation = new();                               // 所属国
        private readonly ConcurrentDictionary<int, byte> CacheCampaignAllegiance = new();                   // アルタナ所属国
        private readonly ConcurrentDictionary<int, CharacterVariableList> CacheVarList = new();             // 変数リスト
        private readonly ConcurrentDictionary<int, TeleportInfo> CacheTeleportInfo = new();                 // テレポート情報
        private readonly ConcurrentDictionary<int, CharacterProfile> CacheProfile = new();                  // プロフィール情報
        private readonly ConcurrentDictionary<int, List<Inventory>> CacheInventory = new();                // インベントリー情報
        private readonly ConcurrentDictionary<int, List<CharaEffect>> CacheCharaEffect = new();            // キャラクター効果情報
        private readonly ConcurrentDictionary<int, Dictionary<int, CharaSkill>> CacheCharaSkill = new();   // キャラクタースキル情報
        private readonly ConcurrentDictionary<int, CharaStatus> CacheCharaStatus = new();                  // ステータス情報
        private readonly ConcurrentDictionary<int, CharaJob> CacheCharaJob = new();                        // ジョブ情報
        private readonly ConcurrentDictionary<int, CharaMagic> CacheCharaMagic = new();                    // 魔法情報

        private MySqlConnection? _connection = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DatabaseApi()
        {
            // キャッシュ
            CacheMissionInfo = new();       // ミッション
            CacheQuestInfo = new();         // クエスト
            CacheZoneList = new();          // ゾーン
            CacheVarList = new();           // 変数リスト
            CacheTeleportInfo = new();      // テレポート情報
            CacheProfile = new();           // プロフィール情報
            CacheInventory = new();         // インベントリー情報
            CacheCharaEffect = new();       // キャラクター効果情報
            CacheCharaSkill = new();        // キャラクタースキル情報
            CacheCharaStatus = new();       // ステータス情報
            CacheCharaJob = new();          // ジョブ情報
            CacheCharaMagic = new();        // 魔法情報

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
                    throw new Exception("BLOB data size mismatch.");
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

            ClearAllCaches();
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
                command.Parameters.AddWithValue("@charaId", charaId);
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
                command.Parameters.AddWithValue("@charaId", charaId);
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
                command.Parameters.AddWithValue("@charaId", charaId);
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
                command.Parameters.AddWithValue("@charaId", charaId);
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
                command.Parameters.AddWithValue("@charaId", charaId);
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
            const string query = "SELECT face, race " +
                                 "FROM char_look WHERE charid = @Charid";
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
            CacheMissionInfo.Clear();
            CacheInventory.Clear();
            CacheCharaEffect.Clear();
            CacheCharaSkill.Clear();
            CacheCharaStatus.Clear();
            CacheCharaJob.Clear();
            CacheCharaMagic.Clear();
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

        internal bool HasQuestComplete(int charaId, object oTHER_AREAS, int tHE_OLD_LADY)
        {
            throw new NotImplementedException();
        }
    }
}
