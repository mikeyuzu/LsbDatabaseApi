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
}
