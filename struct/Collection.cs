using System.Collections.Generic;

namespace LsbDatabaseApi.@struct
{
    /// <summary>
    /// 図鑑リスト
    /// </summary>
    public enum CompendiumType
    {
        Mission = 0,    // ミッション図鑑
        Quest,          // クエスト図鑑
        Item,           // アイテム図鑑
        Monster,        // モンスター図鑑
        Magic,          // 魔法図鑑
        WeaponSkill,    // WS図鑑

        Max,            // 最大値
    };
}
