using Google.Protobuf.WellKnownTypes;
using LsbDatabaseApi.Controllers;
using System;
using System.Reflection.Metadata.Ecma335;
using static LsbDatabaseApi.DatabaseApi;

namespace LsbDatabaseApi.@struct
{
    /// <summary>
    /// クリップカテゴリ
    /// </summary>
    public enum ClipCategory
    {
        NONE = 0,           // 無し
        QUEST,              // クエスト
        ITEM,               // アイテム
        MAGIC,              // 魔法

        MAX
    }

    /// <summary>
    /// クリップ情報
    /// </summary>
    public class ClipInfo
    {
        /// <summary>
        /// 新しいクリップID
        /// </summary>
        public int NewClipId { get; set; }

        /// <summary>
        /// クリップメッセージ
        /// </summary>
        public string ClipMessage { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ClipInfo()
        {
            NewClipId = 0;
            ClipMessage = string.Empty;
        }
    }

    /// <summary>
    /// カスタムメニュー情報
    /// <summary>
    public class CustomMenuInfo
    {
        /// <summary>
        /// ナビゲーションメッセージ
        /// </summary>
        public string MainNaviMessage { get; set; }

        /// <summary>
        /// クリップ１情報
        /// </summary>
        public ClipInfo Clip1 { get; set; }

        /// <summary>
        /// クリップ２情報
        /// </summary>
        public ClipInfo Clip2 { get; set; }

        /// <summary>
        /// クリップ３情報
        /// </summary>
        public ClipInfo Clip3 { get; set; }

        /// <summary>
        /// クリップ４情報
        /// </summary>
        public ClipInfo Clip4 { get; set; }

        /// <summary>
        /// ビックリマーク
        /// </summary>
        public int ExclamationMark { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomMenuInfo()
        {
            MainNaviMessage = string.Empty;
            Clip1 = new ClipInfo();
            Clip2 = new ClipInfo();
            Clip3 = new ClipInfo();
            Clip4 = new ClipInfo();
            ExclamationMark = 0;
        }
    }
}
