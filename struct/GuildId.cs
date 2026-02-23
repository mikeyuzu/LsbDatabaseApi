namespace LsbDatabaseApi.@struct
{
    /// <summary>
    /// 合成ギルド
    /// </summary>
    public enum GuildId
    {
        FISHING = 0,        // 釣りギルド
        WOODWORKING = 1,    // 木工ギルド
        SMITHING = 2,       // 鍛冶ギルド
        GOLDSMITHING = 3,   // 彫金ギルド
        CLOTHCRAFT = 4,     // 織工ギルド
        LEATHERCRAFT = 5,   // 革工ギルド
        BONECRAFT = 6,      // 骨工ギルド
        ALCHEMY = 7,        // 錬金術ギルド
        COOKING = 8,        // 調理ギルド

        MAX
    }

    public enum GuildSkillId
    {
        FISHING = 48,
        WOODWORKING = 49,
        SMITHING = 50,
        GOLDSMITHING = 51,
        CLOTHCRAFT = 52,
        LEATHERCRAFT = 53,
        BONECRAFT = 54,
        ALCHEMY = 55,
        COOKING = 56,
    }

    public enum CraftRank
    {
        AMATEUR = 0,        // 素人
        RECRUIT = 1,        // 見習
        INITIATE = 2,       // 徒弟
        NOVICE = 3,         // 下級職人
        APPRENTICE = 4,     // 名取
        JOURNEYMAN = 5,     // 目録
        CRAFTSMAN = 6,      // 印可
        ARTISAN = 7,        // 高弟
        ADEPT = 8,          // 皆伝
        VETERAN = 9,        // 師範
        EXPERT = 10,        // 高級職人
        AUTHORITY = 11,     // 権威
        LUMINARY = 12,      // 大家
        MASTER = 13,        // 師匠
        GRANDMASTER = 14,   // 総帥
        LEGEND = 15,        // 伝説

        MAX
    }
}
