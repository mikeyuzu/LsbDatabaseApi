using LsbDatabaseApi.mission;
using LsbDatabaseApi.@struct;
using Newtonsoft.Json.Linq;

namespace LsbDatabaseApi
{
    public class MessageParam
    {
        private JObject messages; // JSONデータを格納するフィールド

        // ミッションID
        public MissionKind missionKind;
        // ミッション種別
        public string missionType;
        // ミッションの進捗
        public string missionPhase;
        // ミッションパラメータ
        public int missionParam;

        // ミッションのカテゴリ
        public enum MissionKind
        {
            None = 0,
            Coupon,             // 冒険者優待券
            Tutorial,           // チュートリアル
            MissionSandoria,    // サンドリアミッション
            MissionBastok,      // バストゥークミッション
            MissionWindurst,    // ウィンダスミッション
            MissionZilart,      // ジラートの幻影ミッション
            MissionCOP,         // プロマシアの呪縛ミッション
            MissionTOAU,        // アトルガンの秘宝ミッション
            MissionWOTG,        // アルタナの神兵ミッション
            Trust,              // フェイスクエスト
            Support,            // サポートジョブクエスト
            Quest,              // クエスト
            Fame,               // 名声
            Area,               // エリア

            End
        }

        // 冒険者優待券の種類
        public enum CouponType
        {
            None = 0,
            SOUTHERN_SAN_DORIA,     // 南サンドリア
            NORTHERN_SAN_DORIA,     // 北サンドリア
            PORT_SAN_DORIA,         // サンドリア港
            BASTOK_MINES,           // バストゥーク鉱山区
            PORT_BASTOK,            // バストゥーク港
            BASTOK_MARKETS,         // バストゥーク商業区
            WINDURST_WATERS,        // ウィンダス水の区
            WINDURST_WALLS,         // ウィンダス石の区
            PORT_WINDURST,          // ウィンダス港
            WINDURST_WOODS,         // ウィンダス森の区
        }

        /// <summary>
        /// メッセージパラメータ
        /// </summary>
        public MessageParam()
        {
            messages = [];
            LoadMessagesFromJson("messages.json"); // JSONファイルを読み込む
            missionKind = MissionKind.None;
            missionType = "0";
            missionPhase = "0";
            missionParam = 0;
        }

        /// <summary>
        /// メッセージパラメータ
        /// </summary>
        private void SetMessageParam(MessageParam param)
        {
            missionKind = param.missionKind;
            missionType = param.missionType;
            missionPhase = param.missionPhase;
            missionParam = param.missionParam;
        }

        /// <summary>
        /// JSONファイルからメッセージを読み込む
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadMessagesFromJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                messages = JObject.Parse(json);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"JSONファイルが見つかりません: {ex.Message}");
                // 適切なエラー処理を行う（例：デフォルトメッセージを設定、例外を再スローなど）
                messages = []; // 空のJObjectで初期化
            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                Console.WriteLine($"JSONファイルの形式が不正です: {ex.Message}");
                messages = []; // 空のJObjectで初期化
            }
            catch (Exception ex)
            {
                Console.WriteLine($"予期せぬエラーが発生しました: {ex.Message}");
                messages = []; // 空のJObjectで初期化
            }
        }

        /// <summary>
        /// メッセージを取得
        /// </summary>
        /// <returns></returns>
        private string GetMessage()
        {
            string message = string.Empty;

            switch (missionKind)
            {
                case MissionKind.Coupon:
                    message = messages[missionKind.ToString()]?[missionType]?.ToString() ?? "";
                    break;
                case MissionKind.Tutorial:
                case MissionKind.MissionSandoria:
                case MissionKind.MissionBastok:
                case MissionKind.MissionWindurst:
                case MissionKind.MissionZilart:
                case MissionKind.MissionCOP:
                case MissionKind.MissionTOAU:
                case MissionKind.MissionWOTG:
                case MissionKind.Trust:
                case MissionKind.Support:
                case MissionKind.Quest:
                case MissionKind.Area:
                    message = messages[missionKind.ToString()]?[missionType]?[missionPhase]?.ToString() ?? "";
                    if (missionKind == MissionKind.MissionWindurst &&
                        missionType == MissionWindurst.A_TESTING_TIME.ToString() &&
                        (missionPhase == "2" || missionPhase == "2end"))
                    {
                        message += missionParam.ToString();
                    }
                    if (missionKind == MissionKind.Quest && missionType == "CHOCOBOS_WOUNDS" && missionParam >= 2)
                    {
                        message = messages[missionKind.ToString()]?[missionType]?[missionParam.ToString()]?.ToString() ?? "";
                        message += missionPhase;
                    }
                    break;
                case MissionKind.Fame:
                    message = messages[missionKind.ToString()]?[missionType]?[missionPhase]?.ToString() ?? "";
                    int femaRank = missionParam / 10000;
                    int femaPoint = missionParam % 10000;
                    message = message.Replace("{femaRank}", femaRank.ToString());
                    message = message.Replace("{femaPoint}", femaPoint.ToString());
                    break;
                default:
                    break;
            }

            if (message == string.Empty)
            {
                message = missionKind.ToString() + " / " + missionType + " / " + missionPhase;
            }

            return message;
        }

        /// <summary>
        /// メッセージのパラメータを取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="zoneId"></param>
        /// <param name="mapId"></param>
        /// <param name="coordinates"></param>
        /// <param name="preZoneId"></param>
        /// <param name="preMapId"></param>
        /// <param name="preCoordinates"></param>
        /// <returns></returns>
        public string GetMessageParam(DatabaseApi database, int charaId, int zoneId, int mapId, string coordinates, int preZoneId, int preMapId, string preCoordinates)
        {
            var charaInfo = new CharaInfo(charaId, zoneId, mapId, coordinates, preZoneId, preMapId, preCoordinates);

            if (TutorialMission.IsTutorialMission(database, charaInfo))
            {
                // チュートリアル中
                SetMessageParam(TutorialMission.GetMessageTutorial(database, charaInfo));
                return GetMessage();
            }
            else
            {
                // ミッション中
                if (database.IsMissionNull(charaInfo.CharaId))
                {
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            missionKind = MissionKind.MissionSandoria;
                            break;
                        case NationId.BASTOK:
                            missionKind = MissionKind.MissionBastok;
                            break;
                        case NationId.WINDURST:
                            missionKind = MissionKind.MissionWindurst;
                            break;
                    }
                    missionType = "65535";
                    missionPhase = "0";
                    return GetMessage();
                }
                // 3国ミッション前半
                switch (database.GetNation(charaInfo.CharaId))
                {
                    case NationId.SANDORIA:
                        if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.THE_RUINS_OF_FEI_YIN))
                        {
                            //TODO SetMessageParam(BastokMission.GetMessageSandoria(database, charaInfo));
                            return GetMessage();
                        }
                        break;
                    case NationId.BASTOK:
                        if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.DARKNESS_RISING))
                        {
                            //TODO SetMessageParam(BastokMission.GetMessageWindurst(database, charaInfo));
                            return GetMessage();
                        }
                        break;
                    case NationId.WINDURST:
                        if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_SHADOW_AWAITS))
                        {
                            SetMessageParam(WindurstMission.GetMessageWindurst(database, charaInfo));
                            return GetMessage();
                        }
                        break;
                    default:
                        break;
                }
                // ジラートミッション
                {
                    var zilartMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.ZILART);
                    var missionType = (MissionZilart)zilartMission.Current;
                    var missionPhase = zilartMission.StatusLower;

                    if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.ZILART, (int)MissionZilart.AWAKENING)
                        && !(missionType == MissionZilart.AWAKENING && missionPhase == 3))
                    {
                        SetMessageParam(ZilartMission.GetMessageZilart(database, charaInfo));
                        return GetMessage();
                    }
                }
                // 3国ミッション後半
                switch (database.GetNation(charaInfo.CharaId))
                {
                    case NationId.SANDORIA:
                        missionKind = MissionKind.MissionSandoria;
                        break;
                    case NationId.BASTOK:
                        missionKind = MissionKind.MissionBastok;
                        break;
                    case NationId.WINDURST:
                        if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.MOON_READING))
                        {
                            SetMessageParam(WindurstMission.GetMessageWindurst(database, charaInfo));
                            return GetMessage();
                        }
                        break;
                    default:
                        break;
                }
                // デュナミス
                {
                    var messageParam = new MessageParam();
                    if (QuestMission.GetDynamisMessage(database, charaInfo, ref messageParam))
                    {
                        SetMessageParam(messageParam);
                        return GetMessage();
                    }
                }
                // プロマシアミッション
                if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.APOCALYPSE_NIGH))
                {
                    SetMessageParam(COPMission.GetMessageCOP(database, charaInfo));
                    return GetMessage();
                }
                // アトルガンミッション
                if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.TOAU, (int)MissionTOAU.THE_EMPRESS_CROWNED))
                {
                    SetMessageParam(TOAUMission.GetMessageTOAU(database, charaInfo));
                    return GetMessage();
                }
                // アルタナミッション
                if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WOTG, (int)MissionWOTG.A_TOKEN_OF_TROTH))
                {
                    SetMessageParam(WOTGMission.GetMessageWOTG(database, charaInfo));
                    return GetMessage();
                }
            }

            return GetMessage();
        }
    }
}
