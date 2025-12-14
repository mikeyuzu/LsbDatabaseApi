using System.Collections.Generic;

namespace LsbDatabaseApi.@struct
{
    /// <summary>
    /// item_baseのah
    /// </summary>
    public enum AuctionHouseId
    {
        NONE = 0,           // なし
        H2H = 1,            // 武器/格闘
        DAGGER = 2,         // 武器/短剣
        SWORD = 3,          // 武器/片手剣
        GREATSWORD = 4,     // 武器/両手剣
        AXE = 5,            // 武器/片手斧
        GREATAXE = 6,       // 武器/両手斧
        SCYTHE = 7,         // 武器/両手鎌
        POLEARM = 8,        // 武器/両手槍
        KATANA = 9,         // 武器/片手刀
        GREATKATANA = 10,   // 武器/両手刀
        CLUB = 11,          // 武器/片手棍
        STAFF = 12,         // 武器/両手棍
        BOW = 13,           // 武器/レンジ武器等
        INSTRUMENTS = 14,   // 武器/楽器
        AMMUNITION = 15,    // 武器/矢・弾その他/矢・弾
        SHIELD = 16,        // 防具/盾
        HEAD = 17,          // 防具/頭
        BODY = 18,          // 防具/胴
        HANDS = 19,         // 防具/両手
        LEGS = 20,          // 防具/両脚
        FEET = 21,          // 防具/両足
        NECK = 22,          // 防具/首
        WAIST = 23,         // 防具/腰
        EARRINGS = 24,      // 防具/耳
        RINGS = 25,         // 防具/指
        BACK = 26,          // 防具/背
        UNUSED = 27,        // (未定義)
        WHITE_MAGIC = 28,   // 魔法スクロール/白魔法
        BLACK_MAGIC = 29,   // 魔法スクロール/黒魔法
        SUMMONING = 30,     // 魔法スクロール/召喚魔法
        NINJUTSU = 31,      // 魔法スクロール/忍術
        SONGS = 32,         // 魔法スクロール/歌
        MEDICINES = 33,     // 薬品
        FURNISHINGS = 34,   // 調度品
        CRYSTALS = 35,      // クリスタル
        CARDS = 36,         // その他/カード
        CURSED_ITEMS = 37,  // その他/呪物
        SMITHING = 38,      // 素材/金属材
        GOLDSMITHING = 39,  // 素材/貴金属材
        CLOTHCRAFT = 40,    // 素材/布材
        LEATHERCRAFT = 41,  // 素材/皮革材
        BONECRAFT = 42,     // 素材/骨材
        WOODWORKING = 43,   // 素材/木材
        ALCHEMY = 44,       // 素材/錬金術材
        GEOMANCER = 45,     // 魔法スクロール/風水魔法
        MISC = 46,          // その他/雑貨
        FISHING_GEAR = 47,  // 武器/矢・弾その他/釣り具
        PET_ITEMS = 48,     // 武器/矢・弾その他/獣の餌
        NINJA_TOOLS = 49,   // その他/忍具
        BEAST_MADE = 50,    // その他/獣人製品
        FISH = 51,          // 食品/水産物
        MEAT_EGGS = 52,     // 食品/料理/肉・卵料理
        SEAFOOD = 53,       // 食品/料理/魚介料理
        VEGETABLES = 54,    // 食品/料理/野菜料理
        SOUPS = 55,         // 食品/料理/スープ類
        BREADS_RICE = 56,   // 食品/料理/穀物料理
        SWEETS = 57,        // 食品/料理/スィーツ
        DRINKS = 58,        // 食品/料理/ドリンク
        INGREDIENTS = 59,   // 食品/食材
        DICE = 60,          // 魔法スクロール/ダイス
        AUTOMATON = 61,     // その他/からくり部品
        GRIPS = 62,         // 武器/矢・弾その他/グリップ
        ALCHEMY_2 = 63,     // 素材/錬金術材2
        MISC_2 = 64,        // その他/雑貨2
        MISC_3 = 65,        // その他/雑貨3

        INVALID = 255       // 最大値
    }

    /// <summary>
    /// 競売所の大カテゴリ
    /// </summary>
    public enum AuctionHouseKind
    {
        WEAPON = 0,     // 武器
        DEFENSE,        // 防具
        MAGIC,          // 魔法スクロール
        MEDICINES,      // 薬品
        FURNISHINGS,    // 調度品
        MATERIALS,      // 素材
        FOOD,           // 食品
        CRYSTAL,        // クリスタル
        OTHER,          // その他

        MAX
    }

    public enum AuctionHouseItem
    {
        ITEM = 0,   // 項目
        LIST,       // リスト

        MAX
    }

    /// <summary>
    /// 競売所のアイテム情報
    /// </summary>
    public struct AuctionHouseSubcategoryItem
    {
        public AuctionHouseItem Item;
        public AuctionHouseId Id;
        public List<AuctionHouseId> List;
    };

    public static class AuctionHouseData
    {
        /// <summary>
        /// 武器のカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemWeapon = new List<AuctionHouseSubcategoryItem>()
        {
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.H2H },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.DAGGER },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.SWORD },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.GREATSWORD },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.AXE },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.GREATAXE },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.SCYTHE },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.POLEARM },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.KATANA },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.GREATKATANA },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.CLUB },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.STAFF },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.BOW },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.INSTRUMENTS },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.LIST, List = new List<AuctionHouseId> { AuctionHouseId.AMMUNITION, AuctionHouseId.FISHING_GEAR, AuctionHouseId.PET_ITEMS, AuctionHouseId.GRIPS, } },
        };

        /// <summary>
        /// 防具のカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemDefense = new List<AuctionHouseSubcategoryItem>()
        {
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.SHIELD },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.NECK },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.HEAD },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.BODY },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.HANDS },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.WAIST },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.LEGS },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.FEET },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.BACK },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.EARRINGS },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.RINGS },
        };

        /// <summary>
        /// 魔法のカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemMagic = new List<AuctionHouseSubcategoryItem>()
        {
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.WHITE_MAGIC },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.BLACK_MAGIC },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.SONGS },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.NINJUTSU },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.SUMMONING },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.DICE },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.GEOMANCER },
        };

        /// <summary>
        /// 薬品のカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemMedicines = new List<AuctionHouseSubcategoryItem>()
        {
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.MEDICINES },
        };

        /// <summary>
        /// 調度品のカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemFurnishings = new List<AuctionHouseSubcategoryItem>()
        {
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.FURNISHINGS },
        };

        /// <summary>
        /// 素材のカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemMaterials = new List<AuctionHouseSubcategoryItem>()
        {
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.SMITHING },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.GOLDSMITHING },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.CLOTHCRAFT },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.LEATHERCRAFT },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.BONECRAFT },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.WOODWORKING },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.ALCHEMY },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.ALCHEMY_2 },
        };

        /// <summary>
        /// 食品のカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemFood = new List<AuctionHouseSubcategoryItem>()
        {
            // 料理
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.LIST, List = new List<AuctionHouseId> {
                AuctionHouseId.MEAT_EGGS,
                AuctionHouseId.SEAFOOD,
                AuctionHouseId.VEGETABLES,
                AuctionHouseId.SOUPS,
                AuctionHouseId.BREADS_RICE,
                AuctionHouseId.SWEETS,
                AuctionHouseId.DRINKS,
            } },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.INGREDIENTS},
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.FISH },
        };

        /// <summary>
        /// クリスタルのカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemCrystal = new List<AuctionHouseSubcategoryItem>()
        {
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.CRYSTALS },
        };

        /// <summary>
        /// その他のカテゴリ
        /// </summary>
        public static List<AuctionHouseSubcategoryItem> AuctionHouseItemOther = new List<AuctionHouseSubcategoryItem>()
        {
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.MISC },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.MISC_2 },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.MISC_3 },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.BEAST_MADE },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.CARDS },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.NINJA_TOOLS },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.CURSED_ITEMS },
            new AuctionHouseSubcategoryItem { Item = AuctionHouseItem.ITEM, Id = AuctionHouseId.AUTOMATON },
        };

        /// <summary>
        /// 競売所のリスト
        /// </summary>
        public static List<List<AuctionHouseSubcategoryItem>> AuctionHouseList = new List<List<AuctionHouseSubcategoryItem>>()
        {
            AuctionHouseItemWeapon,
            AuctionHouseItemDefense,
            AuctionHouseItemMagic,
            AuctionHouseItemMedicines,
            AuctionHouseItemFurnishings,
            AuctionHouseItemMaterials,
            AuctionHouseItemFood,
            AuctionHouseItemCrystal,
            AuctionHouseItemOther,
        };
    }
}
