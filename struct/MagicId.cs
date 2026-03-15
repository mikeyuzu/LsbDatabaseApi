namespace LsbDatabaseApi.@struct
{
    /// <summary>
    /// グループID
    /// </summary>
    public enum SpellGroup
    {
        NONE = 0,
        SONG = 1,           // 歌
        BLACK = 2,          // 黒魔法
        BLUE = 3,           // 青魔法
        NINJUTSU = 4,       // 忍術
        SUMMONING = 5,      // 召喚魔法
        WHITE = 6,          // 白魔法
        GEOMANCY = 7,       // 風水魔法
        TRUST = 8,          // フェイス

        MAX
    }

    /// <summary>
    /// 魔法図鑑表示ID
    /// </summary>
    public enum MagicDispId
    {
        WHITE = 0,          // 白魔法
        BLACK,              // 黒魔法
        SONG,               // 歌
        NINJUTSU,           // 忍術
        SUMMONING,          // 召喚魔法
        BLUE,               // 青魔法
        GEOMANCY,           // 風水魔法
        TRUST,              // フェイス

        MAX
    }

    /// <summary>
    /// 魔法ID１
    /// </summary>
    public enum MagicId
    {
        NAJI = 897,                 // ナジ
        KUPIPI = 898,               // クピピ
        EXCENMILLE = 899,           // エグセニミル
        MIHLI_ALIAPOH = 909,        // ミリアポー
        VALAINERAL = 910,           // ヴァレンラール
        KORU_MORU = 952,            // コルモル
        ADELHEID = 968,             // アーデルハイト
        AAHM = 992,                 // アークHM
        AAEV = 993,                 // アークEV
        AAMR = 994,                 // アークMR
        AATT = 995,                 // アークTT
        AAGK = 996,                 // アークGK
        MONBERAUX = 999,            // モンブロー

    }

    /// <summary>
    /// 魔法グループ別情報
    /// </summary>
    public struct MagicGroupInfo
    {
        public int Id { get; set; }     // 魔法ID
        public int[] Jobs { get; set; }     // 使用可能なジョブレベル
        public int MinLevel { get; set; }   // 使用可能な最小レベル（ソート用）
        public int Flag { get; set; }       // 所持フラグ

        public MagicGroupInfo()
        {
            Jobs = new int[(int)JobId.MAX];
            Flag = 0;
        }
    }
}
