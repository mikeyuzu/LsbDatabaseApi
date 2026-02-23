namespace LsbDatabaseApi.@struct
{
    public enum JobId
    {
        WAR = 0,    // 戦士
        MNK,        // モンク
        WHM,        // 白魔道士
        BLM,        // 黒魔道士
        RDM,        // 赤魔道士
        THF,        // シーフ
        PLD,        // ナイト
        DRK,        // 暗黒騎士
        BST,        // 獣使い
        BRD,        // 詩人
        RNG,        // 狩人
        SAM,        // 侍
        NIN,        // 忍者
        DRG,        // 竜騎士
        SMN,        // 召喚士
        BLU,        // 青魔道士
        COR,        // コルセア
        PUP,        // からくり士
        DNC,        // 踊り子
        SCH,        // 学者
        GEO,        // 風水士
        RUN,        // 魔導剣士

        MAX
    }

    /// <summary>
    /// ペットタイプ
    /// </summary>
    public enum PetType
    {
        PET = 0,        // ペット 
        AVATAR = 1,     // 召喚獣
        WYVERN = 2,     // 飛竜
        AUTOMATON1 = 3, // オートマトン
        AUTOMATON2 = 4, // オートマトン
        AUTOMATON3 = 5, // オートマトン
        AUTOMATON4 = 6, // オートマトン
        AUTOMATON5 = 7, // オートマトン
        LUOPAN = 8,     // 羅盤

        NONE
    }
}
