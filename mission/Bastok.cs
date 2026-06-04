using LsbDatabaseApi.@struct;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class BastokMission
    {
        public static MessageParam GetMessageBastok(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();

            var BastokMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.BASTOK);
            message.missionKind = MissionKind.MissionBastok;
            message.missionType = ((MissionBastok)BastokMission.Current).ToString();
            message.missionPhase = BastokMission.StatusLower.ToString();
            switch ((MissionBastok)BastokMission.Current)
            {
                // ツェールン鉱山からの報告
                case MissionBastok.THE_ZERUHN_REPORT:
                    if (BastokMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ZERUHN_MINES:
                                break;
                            case ZoneId.BASTOK_MINES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ZERUHN_MINES.ToString();
                                message.missionPhase = ZoneId.BASTOK_MINES.ToString();
                                break;
                            case ZoneId.BASTOK_MARKETS:
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BASTOK_MINES.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                        }
                    }
                    else if (BastokMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.METALWORKS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.METALWORKS.ToString();
                                    message.missionPhase = ZoneId.METALWORKS.ToString() + "_1_2";
                                }
                                break;
                            case ZoneId.BASTOK_MARKETS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.METALWORKS.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                break;
                            case ZoneId.BASTOK_MINES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BASTOK_MARKETS.ToString();
                                message.missionPhase = ZoneId.BASTOK_MINES.ToString();
                                break;
                            case ZoneId.ZERUHN_MINES:
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BASTOK_MINES.ToString();
                                message.missionPhase = ZoneId.ZERUHN_MINES.ToString();
                                return message;
                        }
                    }
                    break;
                // 彼の名はシド
                case MissionBastok.GEOLOGICAL_SURVEY:
                    if (BastokMission.StatusLower == 1)
                    {
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.RED_ACIDITY_TESTER))
                        {
                            message.missionPhase = "RED_ACIDITY_TESTER";
                            return message;
                        }

                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.DANGRUF_WADI:
                                break;
                            case ZoneId.SOUTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.DANGRUF_WADI.ToString();
                                message.missionPhase = ZoneId.SOUTH_GUSTABERG.ToString();
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                        }
                    }
                    break;
                // 終わらぬ戦い
                case MissionBastok.FETICHISM:
                    if (BastokMission.StatusLower == 0)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.QUADAV_FETICH_HEAD)
                            && database.HasItem(charaInfo.CharaId, ItemId.QUADAV_FETICH_TORSO)
                            && database.HasItem(charaInfo.CharaId, ItemId.QUADAV_FETICH_ARMS)
                            &&database.HasItem(charaInfo.CharaId, ItemId.QUADAV_FETICH_LEGS))
                        {
                            return message;
                        }

                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.PALBOROUGH_MINES:
                                if (!database.HasItem(charaInfo.CharaId, ItemId.QUADAV_FETICH_HEAD))
                                {
                                    message.missionPhase = ItemId.QUADAV_FETICH_HEAD.ToString();
                                    return message;
                                }
                                if (!database.HasItem(charaInfo.CharaId, ItemId.QUADAV_FETICH_TORSO))
                                {
                                    message.missionPhase = ItemId.QUADAV_FETICH_TORSO.ToString();
                                    return message;
                                }
                                if (!database.HasItem(charaInfo.CharaId, ItemId.QUADAV_FETICH_ARMS))
                                {
                                    message.missionPhase = ItemId.QUADAV_FETICH_ARMS.ToString();
                                    return message;
                                }
                                if (!database.HasItem(charaInfo.CharaId, ItemId.QUADAV_FETICH_LEGS))
                                {
                                    message.missionPhase = ItemId.QUADAV_FETICH_LEGS.ToString();
                                    return message;
                                }
                                break;
                            case ZoneId.NORTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.NORTH_GUSTABERG.ToString();
                                break;
                            case ZoneId.PORT_BASTOK:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.PORT_BASTOK.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PORT_BASTOK.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                        }
                    }
                    break;
                // クリスタルライン
                case MissionBastok.THE_CRYSTAL_LINE:
                    if (BastokMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.KONSCHTAT_HIGHLANDS:
                                break;
                            case ZoneId.NORTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                message.missionPhase = ZoneId.NORTH_GUSTABERG.ToString();
                                break;
                            case ZoneId.SOUTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.SOUTH_GUSTABERG.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                        }
                    }
                    break;
                // 涸れ谷の怪物
                case MissionBastok.WADING_BEASTS:
                    break;
                // バストゥークを離れて
                case MissionBastok.THE_EMISSARY:
                    break;
                // 四銃士
                case MissionBastok.THE_FOUR_MUSKETEERS:
                    break;
                // 忘れられた鉱山にて
                case MissionBastok.TO_THE_FORSAKEN_MINES:
                    break;
                // ジュノへ
                case MissionBastok.JEUNO:
                    break;
                // 魔晶石を奪え
                case MissionBastok.MAGICITE:
                    break;
                // 闇、再び
                case MissionBastok.DARKNESS_RISING:
                    break;
                // ザルカバードに眠る真実
                case MissionBastok.XARCABARD_LAND_OF_TRUTHS:
                    break;
                // 語り部現る！？
                case MissionBastok.RETURN_OF_THE_TALEKEEPER:
                    break;
                // 海賊たちの唄
                case MissionBastok.THE_PIRATES_COVE:
                    break;
                // 完成品のイメージ
                case MissionBastok.THE_FINAL_IMAGE:
                    break;
                // それぞれの行方
                case MissionBastok.ON_MY_WAY:
                    break;
                // 流砂の鎖
                case MissionBastok.THE_CHAINS_THAT_BIND_US:
                    break;
                // その記憶を紡ぐ者
                case MissionBastok.ENTER_THE_TALEKEEPER:
                    break;
                // 最後の幻想
                case MissionBastok.THE_SALT_OF_THE_EARTH:
                    break;
                // 双刃の邂逅
                case MissionBastok.WHERE_TWO_PATHS_CONVERGE:
                    break;
                default:
                    break;
            }

            return message;
        }
    }
}
