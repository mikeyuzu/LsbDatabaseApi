using LsbDatabaseApi.@struct;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class WOTGMission
    {
        public static MessageParam GetMessageWOTG(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();
            if (QuestMission.GetLevelCapMessage(database, charaInfo, 75, ref message))
            {
                return message;
            }

            var wotgMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.WOTG);
            message.missionKind = MissionKind.MissionWOTG;
            message.missionType = ((MissionWOTG)wotgMission.Current).ToString();
            message.missionPhase = wotgMission.StatusLower.ToString();
            var optionString = $"Mission[{(int)MissionId.WOTG}][{(int)wotgMission.Current}]Option";
            var option = database.GetVarNum(charaInfo.CharaId, optionString);
            if (option > 0)
            {
                message.missionPhase = "option" + option.ToString();
            }
            switch ((MissionWOTG)wotgMission.Current)
            {
                case MissionWOTG.CAVERNOUS_MAWS:
                    // M1 忘らるる口
                    break;
                case MissionWOTG.BACK_TO_THE_BEGINNING:
                    // M2 はじまりの刻
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            if (QuestMission.GetWOTGQuestSandoria1Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.BASTOK:
                            if (QuestMission.GetWOTGQuestBastok1Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.WINDURST:
                            if (QuestMission.GetWOTGQuestWindurst1Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case MissionWOTG.CAIT_SITH:
                    // M3 ケット・シー、馳せる
                    {
                        if (charaInfo.ZoneId != ZoneId.EAST_RONFAURE_S
                            && database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.EAST_RONFAURE_S))
                        {
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.EAST_RONFAURE_S.ToString();
                            message.missionPhase = "SURVIVAL";
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.EAST_RONFAURE_S:
                                break;
                            case ZoneId.JUGNER_FOREST_S:
                                if (charaInfo.PreZoneId == ZoneId.BATALLIA_DOWNS_S)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.VUNKERL_INLET_S.ToString();
                                    message.missionPhase = ZoneId.JUGNER_FOREST_S.ToString();
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EAST_RONFAURE_S.ToString();
                                message.missionPhase = ZoneId.JUGNER_FOREST_S.ToString();
                                return message;
                            case ZoneId.VUNKERL_INLET_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.JUGNER_FOREST_S.ToString();
                                message.missionPhase = ZoneId.VUNKERL_INLET_S.ToString();
                                return message;
                            case ZoneId.BATALLIA_DOWNS_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.JUGNER_FOREST_S.ToString();
                                message.missionPhase = ZoneId.BATALLIA_DOWNS_S.ToString();
                                return message;
                            case ZoneId.BATALLIA_DOWNS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BATALLIA_DOWNS_S.ToString();
                                message.missionPhase = ZoneId.BATALLIA_DOWNS.ToString();
                                return message;
                            default:
                                break;
                        }
                    }
                    break;
                case MissionWOTG.THE_QUEEN_OF_THE_DANCE:
                    // M4 舞姫、来たりて
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            if (QuestMission.GetWOTGQuestSandoria2Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.BASTOK:
                            if (QuestMission.GetWOTGQuestBastok2Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.WINDURST:
                            if (QuestMission.GetWOTGQuestWindurst2Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        default:
                            break;
                    }
                    break;

                case MissionWOTG.WHILE_THE_CAT_IS_AWAY:
                    // M5 玉冠の獣、ふたたび
                    break;
                case MissionWOTG.A_TIMESWEPT_BUTTERFLY:
                    // M6 梢の胡蝶
                    break;
                case MissionWOTG.PURPLE_THE_NEW_BLACK:
                    // M7 紫電、劈く
                    break;
                case MissionWOTG.IN_THE_NAME_OF_THE_FATHER:
                    // M8 天涯の娘
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            if (QuestMission.GetWOTGQuestSandoria3Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.BASTOK:
                            if (QuestMission.GetWOTGQuestBastok3Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.WINDURST:
// 対応中
                            if (QuestMission.GetWOTGQuestSandoria3Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            if (QuestMission.GetWOTGQuestBastok3Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            if (QuestMission.GetWOTGQuestWindurst3Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        default:
                            break;
                    }

                    break;

                case MissionWOTG.DANCERS_IN_DISTRESS:
                    // M9 踊り子の憂慮
                    break;
                case MissionWOTG.DAUGHTER_OF_A_KNIGHT:
                    // M10 白い涙、黒い泪
                    break;
                case MissionWOTG.A_SPOONFUL_OF_SUGAR:
                    // M11 彼の世に至る病
                    break;
                case MissionWOTG.AFFAIRS_OF_STATE:
                    // M12 国務、携えし
                    break;
                case MissionWOTG.BORNE_BY_THE_WIND:
                    // M13 威風凛凛
                    break;
                case MissionWOTG.A_NATION_ON_THE_BRINK:
                    // M14 ジュノ、擾乱
                    break;
                case MissionWOTG.CROSSROADS_OF_TIME:
                    // M15 宙の座
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            if (QuestMission.GetWOTGQuestSandoria4Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.BASTOK:
                            if (QuestMission.GetWOTGQuestBastok4Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.WINDURST:
                            if (QuestMission.GetWOTGQuestSandoria4Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            if (QuestMission.GetWOTGQuestBastok4Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            if (QuestMission.GetWOTGQuestWindurst4Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        default:
                            break;
                    }

                    break;

                case MissionWOTG.SANDSWEPT_MEMORIES:
                    // M16 砂の記憶
                    break;
                case MissionWOTG.NORTHLAND_EXPOSURE:
                    // M17 娘、北進して
                    break;
                case MissionWOTG.TRAITOR_IN_THE_MIDST:
                    // M18 紫雲か、暗雲か
                    break;
                case MissionWOTG.BETRAYAL_AT_BEAUCEDINE:
                    // M19 黒き奸計の尾
                    break;
                case MissionWOTG.ON_THIN_ICE:
                    // M20 盤上の罠
                    break;
                case MissionWOTG.PROOF_OF_VALOR:
                    // M21 勇胆の証
                    break;
                case MissionWOTG.A_SANGUINARY_PRELUDE:
                    // M22 衝突、会戦の序
                    break;
                case MissionWOTG.DUNGEONS_AND_DANCERS:
                    // M23 囚われの迷宮で
                    break;
                case MissionWOTG.DISTORTER_OF_TIME:
                    // M24 禁断の口
                    break;
                case MissionWOTG.THE_WILL_OF_THE_WORLD:
                    // M25 喰らわれし未来
                    break;
                case MissionWOTG.FATE_IN_HAZE:
                    // M26 傾ぐ天秤
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            if (QuestMission.GetWOTGQuestSandoria5Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.BASTOK:
                            if (QuestMission.GetWOTGQuestBastok5Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.WINDURST:
                            if (QuestMission.GetWOTGQuestSandoria5Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            if (QuestMission.GetWOTGQuestBastok5Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            if (QuestMission.GetWOTGQuestWindurst5Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        default:
                            break;
                    }

                    break;

                case MissionWOTG.THE_SCENT_OF_BATTLE:
                    // M27 死闘の萌芽
                    break;
                case MissionWOTG.ANOTHER_WORLD:
                    // M28 現世と隠世と
                    break;
                case MissionWOTG.A_HAWK_IN_REPOSE:
                    // M29 勇鷹の墓標
                    break;
                case MissionWOTG.THE_BATTLE_OF_XARCABARD:
                    // M30 決戦、ザルカバード
                    break;
                case MissionWOTG.PRELUDE_TO_A_STORM:
                    // M31 雪上の嵐：翠
                    break;
                case MissionWOTG.STORMS_CRESCENDO:
                    // M32 雪上の嵐：藍
                    break;
                case MissionWOTG.INTO_THE_BEASTS_MAW:
                    // M33 闇の牙城
                    break;
                case MissionWOTG.THE_HUNTER_ENSNARED:
                    // M34 殲撃、響きて
                    break;
                case MissionWOTG.FLIGHT_OF_THE_LION:
                    // M35 獅子たちの帰還
                    break;
                case MissionWOTG.FALL_OF_THE_HAWK:
                    // M36 鉄鷹、旋回す
                    break;
                case MissionWOTG.DARKNESS_DESCENDS:
                    // M37 黒天、閃電
                    break;
                case MissionWOTG.ADIEU_LILISETTE:
                    // M38 さようなら、リリゼット
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            if (QuestMission.GetWOTGQuestSandoria6Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.BASTOK:
                            if (QuestMission.GetWOTGQuestBastok6Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        case NationId.WINDURST:
                            if (QuestMission.GetWOTGQuestSandoria6Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            if (QuestMission.GetWOTGQuestBastok6Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            if (QuestMission.GetWOTGQuestWindurst6Message(database, charaInfo, ref message))
                            {
                                return message;
                            }
                            break;
                        default:
                            break;
                    }

                    break;

                case MissionWOTG.BY_THE_FADING_LIGHT:
                    // M39 鳥籠の宇宙
                    break;
                case MissionWOTG.EDGE_OF_EXISTENCE:
                    // M40 記憶の最果て
                    break;
                case MissionWOTG.HER_MEMORIES:
                    // M41 彼女の想ひ出
                    break;
                case MissionWOTG.FORGET_ME_NOT:
                    // M42 揺籃の宙
                    break;
                case MissionWOTG.PILLAR_OF_HOPE:
                    // M43 光陰の御許に
                    break;
                case MissionWOTG.GLIMMER_OF_LIFE:
                    // M44 さかしまの時
                    break;
                case MissionWOTG.TIME_SLIPS_AWAY:
                    // M45 霧らふ世界
                    break;
                case MissionWOTG.WHEN_WILLS_COLLIDE:
                    // M46 夢見果てし時
                    break;
                case MissionWOTG.WHISPERS_OF_DAWN:
                    // M47 映りしは、暁の
                    break;
                case MissionWOTG.A_DREAMY_INTERLUDE:
                    // M48 瞼に見るもの
                    break;
                case MissionWOTG.CAIT_IN_THE_WOODS:
                    // M49 羅針の行方
                    break;
                case MissionWOTG.FORK_IN_THE_ROAD:
                    // M50 轍辿りて
                    break;
                case MissionWOTG.MAIDEN_OF_THE_DUSK:
                    // M51 翼もつ女神
                    break;
                case MissionWOTG.WHERE_IT_ALL_BEGAN:
                    // M52 はじまりの地
                    break;
                case MissionWOTG.A_TOKEN_OF_TROTH:
                    // M53 約束の刻
                    break;
                case MissionWOTG.LEST_WE_FORGET:
                    // M54 忘らるる君へ
                    break;
                default:
                    break;
            }

            return message;
        }
    }
}
