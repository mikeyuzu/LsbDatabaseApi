namespace LsbDatabaseApi.@struct
{
    /// <summary>
    /// 所属国
    /// </summary>
    public enum NationId
    {
        SANDORIA = 0,   // サンドリア
        BASTOK,         // バストゥーク
        WINDURST,       // ウィンダス

        END
    }

    public enum MissionId
    {
        SANDORIA = 0,   // サンドリアミッション
        BASTOK = 1,     // バストゥークミッション
        WINDURST = 2,   // ウィンダスミッション
        ZILART = 3,     // ジラートの幻影
        TOAU = 4,       // アトルガンの秘宝
        WOTG = 5,       // アルタナの神兵
        COP = 6,        // プロマシアの呪縛
        ASSAULT = 7,    // アサルト
        CAMPAIGN = 8,   // カンパニエ
        ACP = 9,        // 追加シナリオ「石を見る夢」
        AMK = 10,       // 追加シナリオ「戦慄！モグ祭りの夜」
        ASA = 11,       // 追加シナリオ「シャントット帝国の陰謀」
        SOA = 12,       // アドゥリンの魔境
        ROV = 13,       // 星唄ミッション

        MAX_MISSIONAREA = 15
    }

    public enum MissionSandoria
    {
        SMASH_THE_ORCISH_SCOUTS = 0,
        BAT_HUNT = 1,
        SAVE_THE_CHILDREN = 2,
        THE_RESCUE_DRILL = 3,
        THE_DAVOI_REPORT = 4,
        JOURNEY_ABROAD = 5,
        JOURNEY_TO_BASTOK = 6,
        JOURNEY_TO_WINDURST = 7,
        JOURNEY_TO_BASTOK2 = 8,
        JOURNEY_TO_WINDURST2 = 9,
        INFILTRATE_DAVOI = 10,
        THE_CRYSTAL_SPRING = 11,
        APPOINTMENT_TO_JEUNO = 12,
        MAGICITE = 13,
        THE_RUINS_OF_FEI_YIN = 14,
        THE_SHADOW_LORD = 15,
        LEAUTES_LAST_WISHES = 16,
        RANPERRES_FINAL_REST = 17,
        PRESTIGE_OF_THE_PAPSQUE = 18,
        THE_SECRET_WEAPON = 19,
        COMING_OF_AGE = 20,
        LIGHTBRINGER = 21,
        BREAKING_BARRIERS = 22,
        THE_HEIR_TO_THE_LIGHT = 23,
        NONE = 65535,
    }

    public enum MissionBastok
    {
        THE_ZERUHN_REPORT = 0,
        GEOLOGICAL_SURVEY = 1,
        FETICHISM = 2,
        THE_CRYSTAL_LINE = 3,
        WADING_BEASTS = 4,
        THE_EMISSARY = 5,
        THE_EMISSARY_SANDORIA = 6,
        THE_EMISSARY_WINDURST = 7,
        THE_EMISSARY_SANDORIA2 = 8,
        THE_EMISSARY_WINDURST2 = 9,
        THE_FOUR_MUSKETEERS = 10,
        TO_THE_FORSAKEN_MINES = 11,
        JEUNO = 12,
        MAGICITE = 13,
        DARKNESS_RISING = 14,
        XARCABARD_LAND_OF_TRUTHS = 15,
        RETURN_OF_THE_TALEKEEPER = 16,
        THE_PIRATES_COVE = 17,
        THE_FINAL_IMAGE = 18,
        ON_MY_WAY = 19,
        THE_CHAINS_THAT_BIND_US = 20,
        ENTER_THE_TALEKEEPER = 21,
        THE_SALT_OF_THE_EARTH = 22,
        WHERE_TWO_PATHS_CONVERGE = 23,
        NONE = 65535,
    }

    /// <summary>
    /// ウィンダスミッション
    /// </summary>
    public enum MissionWindurst
    {
        THE_HORUTOTO_RUINS_EXPERIMENT = 0,  // ホルトト遺跡の大実験
        THE_HEART_OF_THE_MATTER = 1,        // カーディアンの心
        THE_PRICE_OF_PEACE = 2,             // 平和のために
        LOST_FOR_WORDS = 3,                 // 白き書
        A_TESTING_TIME = 4,                 // 試験の行方
        THE_THREE_KINGDOMS = 5,             // 三大強国
        THE_THREE_KINGDOMS_SANDORIA = 6,    // 三大強国 サンドリアルート
        THE_THREE_KINGDOMS_BASTOK = 7,      // 三大強国 バストゥークルート
        THE_THREE_KINGDOMS_SANDORIA2 = 8,   // 三大強国 バストゥークからサンドリアルート
        THE_THREE_KINGDOMS_BASTOK2 = 9,     // 三大強国 サンドリアからバストゥークルート
        TO_EACH_HIS_OWN_RIGHT = 10,         // それぞれの正義
        WRITTEN_IN_THE_STARS = 11,          // 星読み
        A_NEW_JOURNEY = 12,                 // 新たなる旅立ち
        MAGICITE = 13,                      // 魔晶石を奪え
        THE_FINAL_SEAL = 14,                // 最後の護符
        THE_SHADOW_AWAITS = 15,             // 最果てにて君を待つ闇
        FULL_MOON_FOUNTAIN = 16,            // 満月の泉
        SAINTLY_INVITATION = 17,            // 聖者の招待
        THE_SIXTH_MINISTRY = 18,            // 第6の院
        AWAKENING_OF_THE_GODS = 19,         // 蘇る神々
        VAIN = 20,                          // ヴェイン
        THE_JESTER_WHOD_BE_KING = 21,       // 王と道化師
        DOLL_OF_THE_DEAD = 22,              // 死者の人形
        MOON_READING = 23,                  // 月詠み

        ROLANBERRY = 65534,                 // ロランベリーを渡して魔封門の札をもらう
        NONE = 65535                        // ミッション未受託
    }

    /// <summary>
    /// ジラートの幻影ミッション
    /// </summary>
    public enum MissionZilart
    {
        THE_NEW_FRONTIER = 0,               // 新たなる世界
        WELCOME_TNORG = 4,                  // 海賊の巣窟ノーグ
        KAZHAMS_CHIEFTAINESS = 6,           // カザム族長
        THE_TEMPLE_OF_UGGALEPIH = 8,        // ウガレピ寺院
        HEADSTONE_PILGRIMAGE = 10,          // 古代石碑巡礼
        THROUGH_THE_QUICKSAND_CAVES = 12,   // 流砂洞を越えて
        THE_CHAMBER_OF_ORACLES = 14,        // 宣託の間
        RETURN_TO_DELKFUTTS_TOWER = 16,     // デルクフの塔再び
        ROMAEVE = 18,                       // ロ・メーヴ
        THE_TEMPLE_OF_DESOLATION = 20,      // 聖地ジ・タ～滅びの神殿
        THE_HALL_OF_THE_GODS = 22,          // 神々の間
        THE_MITHRA_AND_THE_CRYSTAL = 23,    // ミスラとクリスタル
        THE_GATE_OF_THE_GODS = 24,          // 神の扉トゥー・リア
        ARK_ANGELS = 26,                    // アーク・ガーディアン
        THE_SEALED_SHRINE = 27,             // 閉ざされし門
        THE_CELESTIAL_NEXUS = 28,           // 宿星の座
        AWAKENING = 30,                     // 1万年の夢の終わりに
        THE_LAST_VERSE = 31,
        NONE = 65535,
    }

    /// <summary>
    /// プロマシアの呪縛ミッション
    /// </summary>
    public enum MissionCOP
    {
        ANCIENT_FLAMES_BECKON = 101,        // 第1章 誘うは古のほむら
        THE_RITES_OF_LIFE = 110,            // 命の洗礼
        BELOW_THE_ARKS = 118,               // 楼閣の下に
        THE_MOTHERCRYSTALS = 128,           // 母なる石
        // THE_ISLE_OF_FORGOTTEN_SAINTS     // 第2章 忘らるる聖者の島
        AN_INVITATION_WEST = 138,           // 西への誘い
        THE_LOST_CITY = 218,                // 忘却の町
        DISTANT_BELIEFS = 228,              // 隔たれし信仰
        AN_ETERNAL_MELODY = 238,            // とこしえに響く歌
        ANCIENT_VOWS = 248,                 // 誓いの雄叫び
        A_TRANSIENT_DREAM = 257,            // 第3章 うたかたなる夢
        THE_CALL_OF_THE_WYRMKING = 258,     // 龍王の導き
        A_VESSEL_WITHOUT_A_CAPTAIN = 318,   // 主のなき都
        THE_ROAD_FORKS = 325,               // ふたつの道
        // EMERALD_WATERS                   // 瑠璃色の川
        // VICISSITUDES                     // 流転
        DESCENDANTS_OF_A_LINE_LOST = 335,   // 累家の末流
        // LOUVERANCE                       // ルーヴランスという者
        // MEMORIES_OF_A_MAIDEN             // をとめの記憶
        COMEDY_OF_ERRORS_ACT_I = 341,       // をかしき祖国
        // COMEDY_OF_ERRORS_ACT_II          // をかしき再会
        // EXIT_STAGE_LEFT                  // をかしき旅立ち
        TENDING_AGED_WOUNDS = 350,          // 戦慄き
        DARKNESS_NAMED = 358,               // 神を名乗りて
        // THE_CRADLES_OF_CHILDREN_LOST     // 第4章 迷い子の揺りかご
        SHELTERING_DOUBT = 368,             // よりしろ
        THE_SAVAGE = 418,                   // 猛き者たちよ
        THE_SECRETS_OF_WORSHIP = 428,       // 礼拝の意味
        SLANDEROUS_UTTERINGS = 438,         // そしりを受けつつも
        // THE_RETURN_HOME                  // 第5章 帰路を踏みしめ
        THE_ENDURING_TUMULT_OF_WAR = 448,   // 鍔音やむことなく
        DESIRES_OF_EMPTINESS = 518,         // 願わくば闇よ
        THREE_PATHS = 530,                  // みっつの道
        // PAST_SINS                        // 汝の罪は
        // SOUTHERN_LEGEND                  // 南方の伝説
        PARTNERS_WITHOUT_FAME = 543,        // 名捨て人ふたり
        // A_CENTURY_OF_HARDSHIP            // なにゆえにその子は
        // DEPARTURES                       // 永いお別れ
        // THE_PURSUIT_OF_PARADISE          // 楽園を求めるは
        SPIRAL = 552,                       // 螺旋
        // BRANDED                          // 烙印ありて
        // PRIDE_AND_HONOR                  // 礼賛者
        // AND_THE_COMPASS_GUIDES           // 羅針の示すもの
        WHERE_MESSENGERS_GATHER = 560,      // 群れ立つ使者は
        // ENTANGLEMENT                     // 結び目
        // HEAD_WIND                        // 向かい風
        FLAMES_FOR_THE_DEAD = 568,          // 迎え火
        // ECHOES_OF_TIME                   // 第6章 時過ぎて鳴り響く
        FOR_WHOM_THE_VERSE_IS_SUNG = 578,   // 歌うは誰がため
        A_PLACE_TO_RETURN = 618,            // ゐぬる場所
        MORE_QUESTIONS_THAN_ANSWERS = 628,  // 望むはあらゆる答え
        ONE_TO_BE_FEARED = 638,             // 畏れよ、我を
        // IN_THE_LIGHT_OF_THE_CRYSTAL      // 第7章 眩き石の御許にて
        CHAINS_AND_BONDS = 648,             // 鎖と絆
        FLAMES_IN_THE_DARKNESS = 718,       // 闇に炎
        FIRE_IN_THE_EYES_OF_MEN = 728,      // 眦決して
        CALM_BEFORE_THE_STORM = 738,        // 決別の前
        THE_WARRIORS_PATH = 748,            // 武士道とは
        EMPTINESS_BLEEDS = 758,             // 第8章 深淵の流す血
        GARDEN_OF_ANTIQUITY = 800,          // 古代の園
        A_FATE_DECIDED = 818,               // 選ばれし死
        WHEN_ANGELS_FALL = 828,             // 天使たちの抗い
        DAWN = 840,                         // 暁
        THE_LAST_VERSE = 850,               // すべての終わりが閉ざされん
    }

    /// <summary>
    /// アトルガンの秘宝ミッション
    /// </summary>
    public enum MissionTOAU
    {
        LAND_OF_SACRED_SERPENTS = 0,    // 聖蛇の国
        IMMORTAL_SENTRIES       = 1,    // 不滅の防人
        PRESIDENT_SALAHEEM      = 2,    // 山猫の社長
        KNIGHT_OF_GOLD          = 3,    // 黄金の騎士
        CONFESSIONS_OF_ROYALTY  = 4,    // 王子の告白
        EASTERLY_WINDS          = 5,    // 東風
        WESTERLY_WINDS          = 6,    // 西風
        A_MERCENARY_LIFE        = 7,    // 傭兵のつとめ
        UNDERSEA_SCOUTING       = 8,    // 聖跡の巡視
        ASTRAL_WAVES            = 9,    // 星気の笛音
        IMPERIAL_SCHEMES        = 10,   // 双蛇の謀
        ROYAL_PUPPETEER         = 11,   // 無手の傀儡師
        LOST_KINGDOM            = 12,   // 亡国の墳墓
        THE_DOLPHIN_CREST       = 13,   // 海豚の紋章
        THE_BLACK_COFFIN        = 14,   // 漆黒の柩
        GHOSTS_OF_THE_PAST      = 15,   // 幽冥の海賊
        GUESTS_OF_THE_EMPIRE    = 16,   // 賓客の資格
        PASSING_GLORY           = 17,   // 泡沫の宝冠
        SWEETS_FOR_THE_SOUL     = 18,   // 遇人の内懐
        TEAHOUSE_TUMULT         = 19,   // 茶屋の厄難
        FINDERS_KEEPERS         = 20,   // 千古の渦紋
        SHIELD_OF_DIPLOMACY     = 21,   // 特使の御楯
        SOCIAL_GRACES           = 22,   // 宴遊の終幕
        FOILED_AMBITION         = 23,   // 悪魔と悪鬼と
        PLAYING_THE_PART        = 24,   // 運命の歯車
        SEAL_OF_THE_SERPENT     = 25,   // 魔蛇の封蝋
        MISPLACED_NOBILITY      = 26,   // 貴人の失踪
        BASTION_OF_KNOWLEDGE    = 27,   // 古寺の所縁
        PUPPET_IN_PERIL         = 28,   // 少女の傀儡
        PREVALENCE_OF_PIRATES   = 29,   // 海賊の利
        SHADES_OF_VENGEANCE     = 30,   // 暗雲の去来
        IN_THE_BLOOD            = 31,   // 逢魔が時
        SENTINELS_HONOR         = 32,   // 砂上の楼閣
        TESTING_THE_WATERS      = 33,   // 山猫の皮算用
        LEGACY_OF_THE_LOST      = 34,   // 亡国の遺産
        GAZE_OF_THE_SABOTEUR    = 35,   // 天狗の慧眼
        PATH_OF_BLOOD           = 36,   // 修羅の道
        STIRRINGS_OF_WAR        = 37,   // 戦乱の兆し
        ALLIED_RUMBLINGS        = 38,   // 連合の混迷
        UNRAVELING_REASON       = 39,   // 紐解ける理
        LIGHT_OF_JUDGMENT       = 40,   // 審判の光
        PATH_OF_DARKNESS        = 41,   // 巨人の懐へ
        FANGS_OF_THE_LION       = 42,   // 蒼獅子の最期
        NASHMEIRAS_PLEA         = 43,   // 少女の決意
        RAGNAROK                = 44,   // ラグナロク
        IMPERIAL_CORONATION     = 45,   // 戴冠の儀
        THE_EMPRESS_CROWNED     = 46,   // 大団円
        ETERNAL_MERCENARY       = 47,   // 永遠の傭兵
    }

    /// <summary>
    /// アルタナの神兵ミッション
    /// </summary>
    public enum MissionWOTG
    {
        CAVERNOUS_MAWS             = 0,     // 忘らるる口
        BACK_TO_THE_BEGINNING      = 1,     // はじまりの刻
        CAIT_SITH                  = 2,     // ケット・シー、馳せる
        THE_QUEEN_OF_THE_DANCE     = 3,     // 舞姫、来たりて
        WHILE_THE_CAT_IS_AWAY      = 4,     // 玉冠の獣、ふたたび
        A_TIMESWEPT_BUTTERFLY      = 5,     // 梢の胡蝶
        PURPLE_THE_NEW_BLACK       = 6,     // 紫電、劈く
        IN_THE_NAME_OF_THE_FATHER  = 7,     // 天涯の娘
        DANCERS_IN_DISTRESS        = 8,     // 踊り子の憂慮
        DAUGHTER_OF_A_KNIGHT       = 9,     // 白い涙、黒い泪
        A_SPOONFUL_OF_SUGAR        = 10,    // 彼の世に至る病
        AFFAIRS_OF_STATE           = 11,    // 国務、携えし
        BORNE_BY_THE_WIND          = 12,    // 威風凛凛
        A_NATION_ON_THE_BRINK      = 13,    // ジュノ、擾乱
        CROSSROADS_OF_TIME         = 14,    // 宙の座
        SANDSWEPT_MEMORIES         = 15,    // 砂の記憶
        NORTHLAND_EXPOSURE         = 16,    // 娘、北進して
        TRAITOR_IN_THE_MIDST       = 17,    // 紫雲か、暗雲か
        BETRAYAL_AT_BEAUCEDINE     = 18,    // 黒き奸計の尾
        ON_THIN_ICE                = 19,    // 盤上の罠
        PROOF_OF_VALOR             = 20,    // 勇胆の証
        A_SANGUINARY_PRELUDE       = 21,    // 衝突、会戦の序
        DUNGEONS_AND_DANCERS       = 22,    // 囚われの迷宮で
        DISTORTER_OF_TIME          = 23,    // 禁断の口
        THE_WILL_OF_THE_WORLD      = 24,    // 喰らわれし未来
        FATE_IN_HAZE               = 25,    // 傾ぐ天秤
        THE_SCENT_OF_BATTLE        = 26,    // 死闘の萌芽
        ANOTHER_WORLD              = 27,    // 現世と隠世と
        A_HAWK_IN_REPOSE           = 28,    // 勇鷹の墓標
        THE_BATTLE_OF_XARCABARD    = 29,    // 決戦、ザルカバード
        PRELUDE_TO_A_STORM         = 30,    // 雪上の嵐：翠
        STORMS_CRESCENDO           = 31,    // 雪上の嵐：藍
        INTO_THE_BEASTS_MAW        = 32,    // 闇の牙城
        THE_HUNTER_ENSNARED        = 33,    // 殲撃、響きて
        FLIGHT_OF_THE_LION         = 34,    // 獅子たちの帰還
        FALL_OF_THE_HAWK           = 35,    // 鉄鷹、旋回す
        DARKNESS_DESCENDS          = 36,    // 黒天、閃電
        ADIEU_LILISETTE            = 37,    // さようなら、リリゼット
        BY_THE_FADING_LIGHT        = 38,    // 鳥籠の宇宙
        EDGE_OF_EXISTENCE          = 39,    // 記憶の最果て
        HER_MEMORIES               = 40,    // 彼女の想ひ出
        FORGET_ME_NOT              = 41,    // 揺籃の宙
        PILLAR_OF_HOPE             = 42,    // 光陰の御許に
        GLIMMER_OF_LIFE            = 43,    // さかしまの時
        TIME_SLIPS_AWAY            = 44,    // 霧らふ世界
        WHEN_WILLS_COLLIDE         = 45,    // 夢見果てし時
        WHISPERS_OF_DAWN           = 46,    // 映りしは、暁の
        A_DREAMY_INTERLUDE         = 47,    // 瞼に見るもの
        CAIT_IN_THE_WOODS          = 48,    // 羅針の行方
        FORK_IN_THE_ROAD           = 49,    // 轍辿りて
        MAIDEN_OF_THE_DUSK         = 50,    // 翼もつ女神
        WHERE_IT_ALL_BEGAN         = 51,    // はじまりの地
        A_TOKEN_OF_TROTH           = 52,    // 約束の刻
        LEST_WE_FORGET             = 53,    // 忘らるる君へ
    }
}
