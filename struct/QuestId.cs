namespace LsbDatabaseApi.@struct
{
    // クエストエリア
    public enum QuestId
    {
        SANDORIA = 0,   // サンドリア
        BASTOK,         // バストゥーク
        WINDURST,       // ウィンダス
        JEUNO,          // ジュノ
        OTHER_AREAS,    // その他
        OUTLANDS,       // 辺境
        AHT_URHGAN,     // アトルガン
        CRYSTAL_WAR,    // 過去世界
        ABYSSEA,        // アビセア
        ADOULIN,        // アドゥリン
        COALITION,      // ワークス

        MAX_QUESTAREA
    }

    // サンドリアクエスト
    public enum QuestSandoria
    {
        TRUST_SANDORIA = 119,               // 新魔法フェイス（サンドリア）
    }

    // バストゥーククエスト
    public enum QuestBastok
    {
        TRUST_BASTOK = 92,                  // 新魔法フェイス（バストゥーク）
    }

    // ウィンダスクエスト
    public enum QuestWindurst
    {
        TRUST_WINDURST = 96,                // 新魔法フェイス（ウィンダス）
    }

    // ジュノクエスト
    public enum QuestJeuno
    {
        CREST_OF_DAVOI = 0,                 // ダボイ村の紋章
        CHOCOBOS_WOUNDS = 4,                // 傷ついたチョコボ
        TENSHODO_MEMBERSHIP = 17,           // 天晶堂入会
        MYSTERIES_OF_BEADEAUX_I = 31,       // ベドーの謎その1
        MYSTERIES_OF_BEADEAUX_II = 32,      // ベドーの謎その2
        EMPTY_MEMORIES = 70,                // 虚ろなる記憶
        STORMS_OF_FATE = 86,                // 日輪を担いて
        SHADOWS_OF_THE_DEPARTED = 88,       // 亡者の影
        APOCALYPSE_NIGH = 89,               // 世界に在りて君は何を想うのか？
        THE_ROAD_TO_AHT_URHGAN = 91,        // アトルガン皇国へ
        IN_DEFIANT_CHALLENGE = 128,         // 限界への挑戦
        ATOP_THE_HIGHEST_MOUNTAINS = 129,   // すべての高い山に登れ
        WHENCE_BLOWS_THE_WIND = 130,        // 風の行方は
        RIDING_ON_THE_CLOUDS = 131,         // 天かける雲のごとく
        SHATTERING_STARS = 132,             // 星の輝きを手に
        FULL_SPEED_AHEAD = 179,             // ライドオン！
    }

    // その他エリアクエスト
    public enum OtherAreas
    {
        THE_OLD_LADY = 10,          // ごうつくばあさん
        A_HARD_DAYS_KNIGHT = 64,    // タブナジア侯国騎士団
        FLY_HIGH = 71,              // その翼、空高く
        BOMBS_AWAY = 96,            // スノールと狩人
    }

    // 辺境エリアクエスト
    public enum QuestOutlands
    {
        DIVINE_MIGHT = 163,         // 神威
        OPEN_SESAME = 165,          // 1人でも開いちゃう？
    }

    // 過去世界エリアクエスト
    public enum QuestCrystalWar
    {
        THE_FIGHTING_FOURTH = 7,            // 第四共和軍団入団試験
        SNAKE_ON_THE_PLAINS = 8,            // コブラ傭兵団入団試験
        STEAMED_RAMS = 9,                   // 王立騎士団入団試験
        BETTER_PART_OF_VALOR = 12,          // 沈黙の契約
        FIRES_OF_DISCONTENT = 13,           // 静かなる警鐘
        GIFTS_OF_THE_GRIFFON = 15,          // 少年たちの贈り物
        CLAWS_OF_THE_GRIFFON = 16,          // オーク軍団掃討作戦
        THE_TIGRESS_STIRS = 17,             // 胎動、牙持つ乙女
        THE_TIGRESS_STRIKES = 18,           // 禍つ闇、襲来
        LIGHT_IN_THE_DARKNESS = 19,         // 解明への灯
        BURDEN_OF_SUSPICION = 20,           // 新たなる猜疑
        BOY_AND_THE_BEAST = 24,             // 巨人偵察作戦II（ツー）
        WRATH_OF_THE_GRIFFON = 25,          // ちいさな勝利、ひとつの決意
        KNOT_QUITE_THERE = 27,              // 憂国の使者
        A_MANIFEST_PROBLEM = 28,            // 降臨、異貌の徒
        STORM_ON_THE_HORIZON = 35,          // 騒乱の行方
        FIRE_IN_THE_HOLE = 36,              // 隠滅の炎
        PERILS_OF_THE_GRIFFON = 37,         // 羽撃け、鷲獅子
        IN_A_HAZE_OF_GLORY = 38,            // それぞれの死地へ
        WHEN_ONE_MAN_IS_NOT_ENOUGH = 39,    // 勃発、ミスラ大戦
        A_FEAST_FOR_GNATS = 40,             // 淑女たちの饗宴
        Q7,
        Q8,
        Q9,
        Q10,
        Q11,
        Q12,
    }

    /// <summary>
    /// 名声エリア
    /// </summary>
    public enum FameArea
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
}
