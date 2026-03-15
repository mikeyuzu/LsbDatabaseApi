using Google.Protobuf.WellKnownTypes;
using LsbDatabaseApi.Controllers;
using System;
using System.Reflection.Metadata.Ecma335;
using static LsbDatabaseApi.DatabaseApi;

namespace LsbDatabaseApi.@struct
{
    /// <summary>
    /// エミネンス・レコードの進捗
    /// </summary>
    public enum EminenceRecordStatus
    {
        NOT_ACHIEVED = 0,       // 0: 未達成
        ACHIEVED = 1,           // 1: 達成
        REWARD_RECEIVED = 2,    // 2: 報酬受取済み
    }

    /// <summary>
    /// エミネンス・レコードのカテゴリ
    /// </summary>
    public enum EminenceRecordCategory
    {
        MISSION = 0,    // ミッション
        AREA,           // エリア
        FACE,           // フェイス

        MAX
    }

    /// <summary>
    /// エミネンス・レコード：ミッション
    /// </summary>
    public enum EminenceRecordMission
    {
        TUTORIAL = 0,               // チュートリアルをクリアした
        MISSION_RANK_3,             // 初めてミッションランクが３になる
        MISSION_RANK_4,             // 初めてミッションランクが４になる
        MISSION_RANK_5,             // 初めてミッションランクが５になる
        ZILART_COMPLETE,            // ジラートミッションをクリアした
        COP_PARTNERS_WITHOUT_FAME,  // プロマシアミッション「みっつの道」をクリア
        COP_COMPLETE,               // プロマシアミッションをクリアした
        TOAU_COMPLETE,              // アトルガンミッションをクリアした

        MAX
    }

    /// <summary>
    /// エミネンス・レコード：エリア
    /// </summary>
    public enum EminenceRecordArea
    {
        DUMMY = 0,

        MAX
    }

    /// <summary>
    /// エミネンス・レコード：フェイス
    /// </summary>
    public enum EminenceRecordFace
    {
        SANDORIA_FACE = 0,  // サンドリアのフェイス使用許可証を入手する
        BASTOK_FACE,        // バストゥークのフェイス使用許可証を入手する
        WINDURST_FACE,      // ウィンダスのフェイス使用許可証を入手する
        KORUMORU,           // クエスト「錬金術の実験」をクリアする
        AAHM,               // ジラートミッションで無知のかけらを入手する
        AAEV,               // ジラートミッションで驕慢のかけらを入手する
        AAMR,               // ジラートミッションで怯懦のかけらを入手する
        AATT,               // ジラートミッションで嫉妬のかけらを入手する
        AAGK,               // ジラートミッションで憎悪のかけらを入手する
        MONBERAUX,          // プロマシアミッション烙印ありてをクリアする

        MAX
    }

    /// <summary>
    /// エミネンス・レコードの情報
    /// <summary>
    public struct EminenceRecord
    {
        public int[] Mission { get; set; }     // ミッション
        public int[] Area { get; set; }        // エリア
        public int[] Face { get; set; }        // フェイス

        public EminenceRecord()
        {
            Mission = new int[(int)EminenceRecordMission.MAX];
            Area = new int[(int)EminenceRecordArea.MAX];
            Face = new int[(int)EminenceRecordFace.MAX];
        }
    }

    /// <summary>
    /// 報酬受取先
    /// </summary>
    public enum EminenceRecordRewardDestination
    {
        DELIVERY_BOX = 0,   // ポスト
        KEY_ITEM,           // キーアイテム
        MAGIC,              // 魔法

        MAX
    }

    /// <summary>
    /// エミネンス・レコードの報酬を渡す処理
    /// </summary>
    public class EminenceRecordRewardProcessor
    {
        public static EminenceRecordRewardDestination ProcessReward(DatabaseApi database, int charaId, EminenceRecordCategory category, int item)
        {
            var result = EminenceRecordRewardDestination.DELIVERY_BOX; // デフォルトはポスト

            switch (category)
            {
                case EminenceRecordCategory.MISSION:
                    switch ((EminenceRecordMission)item)
                    {
                        case EminenceRecordMission.TUTORIAL:
                            {
                                var extra = "0001000000D0000000000000000000000000000000000000";
                                database.InsertDeliveryBoxItem(charaId, 11811, 0, 1, extra);                    // デストリアキャップ
                                database.InsertDeliveryBoxItem(charaId, 10293, 0, 1, extra);                    // チョコボシャツ
                            }
                            break;
                        case EminenceRecordMission.MISSION_RANK_3:
                            {
                                database.LoadKeyItems(charaId);
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_WHITE, true);        // 星唄の煌めき【一奏】
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_UMBER, true);        // 星唄の煌めき【二奏】
                                database.UpdateKeyItems(charaId);
                            }
                            result = EminenceRecordRewardDestination.KEY_ITEM;
                            break;
                        case EminenceRecordMission.MISSION_RANK_4:
                            {
                                database.LoadKeyItems(charaId);
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_AZURE, true);        // 星唄の煌めき【三奏】
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_CRIMSON, true);      // 星唄の煌めき【四奏】
                                database.UpdateKeyItems(charaId);
                            }
                            result = EminenceRecordRewardDestination.KEY_ITEM;
                            break;
                        case EminenceRecordMission.MISSION_RANK_5:
                            {
                                database.LoadKeyItems(charaId);
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_EMERALD, true);      // 星唄の煌めき【五奏】
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_MAUVE, true);        // 星唄の煌めき【六奏】
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_FUCHSIA, true);      // 星唄の煌めき【七奏】
                                database.UpdateKeyItems(charaId);
                            }
                            result = EminenceRecordRewardDestination.KEY_ITEM;
                            break;
                        case EminenceRecordMission.ZILART_COMPLETE:
                            {
                                database.LoadKeyItems(charaId);
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_PUCE, true);         // 星唄の煌めき【八奏】
                                database.UpdateKeyItems(charaId);
                            }
                            result = EminenceRecordRewardDestination.KEY_ITEM;
                            break;
                        case EminenceRecordMission.COP_PARTNERS_WITHOUT_FAME:
                            {
                                int[] itemIds = [11273, 11274, 11275, 11276, 11277, 11278, 11279, 11280];       // 種族別水着のアイテムID
                                var look = database.GetLook(charaId);
                                var extra = "0001000000D0000000000000000000000000000000000000";
                                database.InsertDeliveryBoxItem(charaId, itemIds[(int)look.race], 0, 1, extra);  // 種族別水着
                            }
                            break;
                        case EminenceRecordMission.COP_COMPLETE:
                            {
                                database.LoadKeyItems(charaId);
                                database.UpdateKeyItemCache(charaId, KeyItemId.RHAPSODY_IN_OCHRE, true);        // 星唄の煌めき【九奏】
                                database.UpdateKeyItems(charaId);
                            }
                            result = EminenceRecordRewardDestination.KEY_ITEM;
                            break;
                        case EminenceRecordMission.TOAU_COMPLETE:
                            {
                                database.LoadKeyItems(charaId);
                                database.UpdateKeyItemCache(charaId, KeyItemId.SCINTILLATING_RHAPSODY, true);   // 星唄の煌めき【終奏】
                                database.UpdateKeyItems(charaId);
                            }
                            result = EminenceRecordRewardDestination.KEY_ITEM;
                            break;
                    }
                    break;
                case EminenceRecordCategory.AREA:
                    break;
                case EminenceRecordCategory.FACE:
                    switch ((EminenceRecordFace)item)
                    {
                        case EminenceRecordFace.SANDORIA_FACE:
                            database.LearnMagic(charaId, MagicId.ADELHEID);         // アーデルハイト
                            break;
                        case EminenceRecordFace.BASTOK_FACE:
                            database.LearnMagic(charaId, MagicId.MIHLI_ALIAPOH);    // ミリ・アリアポー
                            break;
                        case EminenceRecordFace.WINDURST_FACE:
                            database.LearnMagic(charaId, MagicId.VALAINERAL);       // ヴァレンラール
                            break;
                        case EminenceRecordFace.KORUMORU:
                            database.LearnMagic(charaId, MagicId.KORU_MORU);        // コルモル
                            break;
                        case EminenceRecordFace.AAHM:
                            database.LearnMagic(charaId, MagicId.AAHM);             // AAHM
                            break;
                        case EminenceRecordFace.AAEV:
                            database.LearnMagic(charaId, MagicId.AAEV);             // AAEV
                            break;
                        case EminenceRecordFace.AAMR:
                            database.LearnMagic(charaId, MagicId.AAMR);             // AAMR
                            break;
                        case EminenceRecordFace.AATT:
                            database.LearnMagic(charaId, MagicId.AATT);             // AATT
                            break;
                        case EminenceRecordFace.AAGK:
                            database.LearnMagic(charaId, MagicId.AAGK);             // AAGK
                            break;
                        case EminenceRecordFace.MONBERAUX:
                            database.LearnMagic(charaId, MagicId.MONBERAUX);        // モンブロー
                            break;
                    }
                    result = EminenceRecordRewardDestination.MAGIC;
                    break;
            }

            return result;
        }
    }
}
