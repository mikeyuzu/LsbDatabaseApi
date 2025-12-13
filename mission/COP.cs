using LsbDatabaseApi.@struct;
using MySql.Data.MySqlClient;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class COPMission
    {
        private static string Coordinates = "";
        private static string PreCoordinates = "";

        public static MessageParam GetMessageCOP(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();
            if (QuestMission.GetLevelCapMessage(database, charaInfo, 75, ref message))
            {
                return message;
            }

            var copMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.COP);
            message.missionKind = MissionKind.MissionCOP;
            message.missionType = ((MissionCOP)copMission.Current).ToString();
            message.missionPhase = copMission.StatusLower.ToString();
            if (copMission.StatusUpper > 0)
            {
                message.missionPhase += "_upper" + copMission.StatusUpper.ToString();
            }
            var statusString = $"Mission[{(int)MissionId.COP}][{(int)copMission.Current}]Status";
            var status = database.GetVarNum(charaInfo.CharaId, statusString);
            if (status > 0)
            {
                message.missionPhase += "_" + status.ToString();
            }
            var optionString = $"Mission[{(int)MissionId.COP}][{(int)copMission.Current}]Option";
            var option = database.GetVarNum(charaInfo.CharaId, optionString);
            if (option > 0)
            {
                message.missionPhase += "_option" + option.ToString();
            }
            switch ((MissionCOP)copMission.Current)
            {
                // 第1章 誘うは古のほむら
                case MissionCOP.ANCIENT_FLAMES_BECKON:
                    break;
                // 命の洗礼
                case MissionCOP.THE_RITES_OF_LIFE:
                    break;
                // 楼閣の下に
                case MissionCOP.BELOW_THE_ARKS:
                    if (status == 1)
                    {
                        if (charaInfo.Coordinates != Coordinates)
                        {
                            PreCoordinates = Coordinates;
                            Coordinates = charaInfo.Coordinates;
                        }
                        if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.EMPTY_MEMORIES)
                            && !database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.EMPTY_MEMORIES))
                        {
                            message.missionKind = MissionKind.Quest;
                            message.missionType = QuestJeuno.EMPTY_MEMORIES.ToString();
                            message.missionPhase = "0";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.LIGHT_OF_HOLLA))
                        {
                            if (!database.HasItem(charaInfo.CharaId, ItemId.BOTTLE_OF_HYSTEROANIMA)
                                && database.HasItem(charaInfo.CharaId, ItemId.RECOLLECTION_OF_PAIN))
                            {
                                message.missionKind = MissionKind.Quest;
                                message.missionType = QuestJeuno.EMPTY_MEMORIES.ToString();
                                message.missionPhase = "2";
                                return message;
                            }

                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.SPIRE_OF_HOLLA:
                                    break;
                                case ZoneId.PROMYVION_HOLLA:
                                    if (!database.HasItem(charaInfo.CharaId, ItemId.BOTTLE_OF_HYSTEROANIMA)
                                        && !database.HasItem(charaInfo.CharaId, ItemId.RECOLLECTION_OF_PAIN))
                                    {
                                        message.missionKind = MissionKind.Quest;
                                        message.missionType = QuestJeuno.EMPTY_MEMORIES.ToString();
                                        message.missionPhase = "1";
                                        return message;
                                    }

                                    switch (CheckProvimonArea(charaInfo.ZoneId, Coordinates, PreCoordinates))
                                    {
                                        case 1:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString() + "_1_2";
                                            return message;
                                        case 2:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString() + "_2_3";
                                            return message;
                                        case 3:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString() + "_3_4";
                                            return message;
                                        case 4:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString() + "_3_4_2";
                                            return message;
                                        default:
                                            break;
                                    }

                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.SPIRE_OF_HOLLA.ToString();
                                    message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString();
                                    return message;
                                case ZoneId.HALL_OF_TRANSFERENCE:
                                    if (option > 0)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                        message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString() + "_OPTION_1";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                    message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                    message.missionPhase = ZoneId.LA_THEINE_PLATEAU.ToString();
                                    return message;
                            }
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.LIGHT_OF_DEM))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.SPIRE_OF_DEM:
                                    break;
                                case ZoneId.PROMYVION_DEM:
                                    if (!database.HasItem(charaInfo.CharaId, ItemId.BOTTLE_OF_HYSTEROANIMA)
                                        && !database.HasItem(charaInfo.CharaId, ItemId.RECOLLECTION_OF_PAIN))
                                    {
                                        message.missionKind = MissionKind.Quest;
                                        message.missionType = QuestJeuno.EMPTY_MEMORIES.ToString();
                                        message.missionPhase = "1";
                                        return message;
                                    }

                                    switch (CheckProvimonArea(charaInfo.ZoneId, Coordinates, PreCoordinates))
                                    {
                                        case 1:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_DEM.ToString() + "_1_2";
                                            return message;
                                        case 2:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_DEM.ToString() + "_2_3";
                                            return message;
                                        case 3:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_DEM.ToString() + "_3_4";
                                            return message;
                                        case 4:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_DEM.ToString() + "_3_4_2";
                                            return message;
                                        default:
                                            break;
                                    }

                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.SPIRE_OF_DEM.ToString();
                                    message.missionPhase = ZoneId.PROMYVION_DEM.ToString();
                                    return message;
                                case ZoneId.HALL_OF_TRANSFERENCE:
                                    if (option > 0)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                        message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString() + "_OPTION_1";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                    message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                    message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                    return message;
                            }
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.LIGHT_OF_MEA))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.SPIRE_OF_MEA:
                                    break;
                                case ZoneId.PROMYVION_MEA:
                                    if (!database.HasItem(charaInfo.CharaId, ItemId.BOTTLE_OF_HYSTEROANIMA)
                                        && !database.HasItem(charaInfo.CharaId, ItemId.RECOLLECTION_OF_PAIN))
                                    {
                                        message.missionKind = MissionKind.Quest;
                                        message.missionType = QuestJeuno.EMPTY_MEMORIES.ToString();
                                        message.missionPhase = "1";
                                        return message;
                                    }

                                    switch (CheckProvimonArea(charaInfo.ZoneId, Coordinates, PreCoordinates))
                                    {
                                        case 1:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_MEA.ToString() + "_1_2";
                                            return message;
                                        case 2:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_MEA.ToString() + "_2_3";
                                            return message;
                                        case 3:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_MEA.ToString() + "_3_4";
                                            return message;
                                        case 4:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                            message.missionPhase = ZoneId.PROMYVION_MEA.ToString() + "_3_4_2";
                                            return message;
                                        default:
                                            break;
                                    }

                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.SPIRE_OF_MEA.ToString();
                                    message.missionPhase = ZoneId.PROMYVION_MEA.ToString();
                                    return message;
                                case ZoneId.HALL_OF_TRANSFERENCE:
                                    if (option > 0)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                        message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString() + "_OPTION_1";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                    message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                    message.missionPhase = ZoneId.TAHRONGI_CANYON.ToString();
                                    return message;
                            }
                        }
                    }
                    break;
                // 忘却の町
                case MissionCOP.THE_LOST_CITY:
                    if (status == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.TAVNAZIAN_SAFEHOLD:
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.TAVNAZIAN_SAFEHOLD.ToString();
                                    message.missionPhase = ZoneId.TAVNAZIAN_SAFEHOLD.ToString() + "_2_1";
                                    return message;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (status == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.TAVNAZIAN_SAFEHOLD:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.TAVNAZIAN_SAFEHOLD.ToString();
                                    message.missionPhase = ZoneId.TAVNAZIAN_SAFEHOLD.ToString() + "_1_2";
                                    return message;
                                }
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.TAVNAZIAN_SAFEHOLD.ToString();
                                    message.missionPhase = ZoneId.TAVNAZIAN_SAFEHOLD.ToString() + "_2_3";
                                    return message;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                // 隔たれし信仰
                case MissionCOP.DISTANT_BELIEFS:
                    if (status == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.PHOMIUNA_AQUEDUCTS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PHOMIUNA_AQUEDUCTS.ToString();
                                    message.missionPhase = ZoneId.PHOMIUNA_AQUEDUCTS.ToString() + "_1_2";
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PHOMIUNA_AQUEDUCTS.ToString();
                                message.missionPhase = ZoneId.TAVNAZIAN_SAFEHOLD.ToString();
                                return message;
                        }
                    }
                    else if (status == 1)
                    {
                        if (!database.HasItem(charaInfo.CharaId, ItemId.BRONZE_KEY)
                            && charaInfo.Coordinates != "(E-8)"
                            && charaInfo.Coordinates != "(F-8)"
                            && charaInfo.Coordinates != "(G-8)")
                        {
                            message.missionPhase = ItemId.BRONZE_KEY.ToString();
                            return message;
                        }
                    }
                    else if (status == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.PHOMIUNA_AQUEDUCTS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PHOMIUNA_AQUEDUCTS.ToString();
                                    message.missionPhase = ZoneId.PHOMIUNA_AQUEDUCTS.ToString() + "_1_2";
                                    return message;
                                }
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PHOMIUNA_AQUEDUCTS.ToString();
                                    message.missionPhase = ZoneId.PHOMIUNA_AQUEDUCTS.ToString() + "_2_3";
                                    return message;
                                }
                                if (charaInfo.MapId == 3)
                                {
                                    if (charaInfo.Coordinates != "(E-8)" && charaInfo.Coordinates != "(F-8)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PHOMIUNA_AQUEDUCTS.ToString();
                                        message.missionPhase = ZoneId.PHOMIUNA_AQUEDUCTS.ToString() + "_3_F7";
                                        return message;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                // 誓いの雄叫び
                case MissionCOP.ANCIENT_VOWS:
                    if (status == 2)
                    {
                        if (charaInfo.Coordinates != Coordinates)
                        {
                            PreCoordinates = Coordinates;
                            Coordinates = charaInfo.Coordinates;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.MONARCH_LINN:
                                break;
                            case ZoneId.RIVERNE_SITE_A01:
                                if (charaInfo.MapId == 1)
                                {
                                    if ((Coordinates == "(G-9)") && (PreCoordinates == "(H-9)")
                                        || (Coordinates == "(G-10)") && (PreCoordinates == "(G-9)" || PreCoordinates == "(H-9)" || PreCoordinates == "(H-10)")
                                        || (Coordinates == "(H-8)") && (PreCoordinates == "(I-8)" || PreCoordinates == "(H-9)")
                                        || Coordinates == "(H-9)"
                                        || (Coordinates == "(H-10)") && (PreCoordinates == "(H-9)" || PreCoordinates == "(G-10)")
                                        || Coordinates == "(I-8)" || Coordinates == "(I-9)" || Coordinates == "(I-10)"
                                        || Coordinates == "(J-8)" || Coordinates == "(J-9)"
                                        || (Coordinates == "(J-10)") && (PreCoordinates == "(I-10)" || PreCoordinates == "(J-9)")
                                        || Coordinates == "(K-8)" || Coordinates == "(K-9)"
                                        || (Coordinates == "(K-10)") && (PreCoordinates == "(K-9)")
                                        || Coordinates == "(L-8)" || Coordinates == "(L-9)"
                                        )
                                    {
                                        if (database.GetItemCount(charaInfo.CharaId, ItemId.GIANT_SCALE) < 2 && Coordinates != "(G-10)")
                                        {
                                            message.missionPhase = ItemId.GIANT_SCALE.ToString();
                                            return message;
                                        }
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.RIVERNE_SITE_A01.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_A01.ToString() + "_2";
                                        return message;
                                    }

                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    if (Coordinates == "(L-8)" || Coordinates == "(L-9)"
                                        || Coordinates == "(M-8)" || Coordinates == "(M-9)"
                                        )
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.RIVERNE_SITE_A01.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_A01.ToString() + "_1";
                                        return message;
                                    }
                                    else if (Coordinates == "(E-10)"
                                        || (Coordinates == "(F-9)" && (PreCoordinates == "(F-10)"))
                                        || Coordinates == "(F-10)" || Coordinates == "(G-10)"
                                        )
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.RIVERNE_SITE_A01.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_A01.ToString() + "_3";
                                        return message;
                                    }
                                    else if (Coordinates == "(D-9)"
                                        || (Coordinates == "(E-9)" && (PreCoordinates == "(D-9)"))
                                        )
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.RIVERNE_SITE_A01.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_A01.ToString() + "_4";
                                        return message;
                                    }
                                    else if (Coordinates == "(E-8)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.MONARCH_LINN.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_A01.ToString();
                                        return message;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                // ふたつの道
                case MissionCOP.THE_ROAD_FORKS:
                    if (copMission.StatusLower == 5)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CARPENTERS_LANDING:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CARPENTERS_LANDING.ToString();
                                message.missionPhase = ZoneId.JUGNER_FOREST.ToString();
                                return message;
                        }
                    }
                    else if (copMission.StatusLower == 142)
                    {
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.MIMEO_FEATHER))
                        {
                            message.missionPhase = copMission.StatusLower.ToString() + "_" + KeyItemId.MIMEO_FEATHER.ToString();
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ATTOHWA_CHASM:
                                break;
                            case ZoneId.MAZE_OF_SHAKHRAMI:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ATTOHWA_CHASM.ToString();
                                message.missionPhase = ZoneId.MAZE_OF_SHAKHRAMI.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MAZE_OF_SHAKHRAMI.ToString();
                                message.missionPhase = ZoneId.TAHRONGI_CANYON.ToString();
                                return message;
                        }
                    }
                    else if (copMission.StatusLower > 142)
                    {
                        message.missionPhase = copMission.StatusLower.ToString();
                        return message;
                    }
                    break;
                // 神を名乗りて
                case MissionCOP.DARKNESS_NAMED:
                    if (status == 2)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.CARMINE_CHIP)
                            || database.HasItem(charaInfo.CharaId, ItemId.CYAN_CHIP)
                            || database.HasItem(charaInfo.CharaId, ItemId.GRAY_CHIP))
                        {
                            message.missionPhase = status.ToString() + "end";
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.PSOXJA:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PSOXJA.ToString();
                                message.missionPhase = ZoneId.BEAUCEDINE_GLACIER.ToString();
                                return message;
                        }
                    }
                    if (status == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.THE_SHROUDED_MAW:
                                break;
                            case ZoneId.PSOXJA:
                                if (charaInfo.MapId == 8)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PSOXJA.ToString();
                                    message.missionPhase = ZoneId.PSOXJA.ToString() + "_8_9";
                                    return message;
                                }
                                else if (charaInfo.MapId == 9)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PSOXJA.ToString();
                                    message.missionPhase = ZoneId.PSOXJA.ToString() + "_9_10";
                                    return message;
                                }
                                else if (charaInfo.MapId == 10)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PSOXJA.ToString();
                                    message.missionPhase = ZoneId.PSOXJA.ToString() + "_10_11";
                                    return message;
                                }
                                else if (charaInfo.MapId == 11)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PSOXJA.ToString();
                                    message.missionPhase = ZoneId.PSOXJA.ToString() + "_11_16";
                                    return message;
                                }
                                else if (charaInfo.MapId == 16)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.THE_SHROUDED_MAW.ToString();
                                    message.missionPhase = ZoneId.PSOXJA.ToString();
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PSOXJA.ToString();
                                message.missionPhase = ZoneId.BEAUCEDINE_GLACIER.ToString();
                                return message;
                        }
                    }
                    break;
                // 猛き者たちよ
                case MissionCOP.THE_SAVAGE:
                    if (status == 1)
                    {
                        if (!database.HasItem(charaInfo.CharaId, ItemId.MISTMELT)
                            && database.GetItemCount(charaInfo.CharaId, ItemId.HIPPOGRYPH_TAILFEATHER) >= 2)
                        {
                            message.missionKind = MissionKind.Quest;
                            message.missionType = OtherAreas.FLY_HIGH.ToString();
                            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.OTHER_AREAS, (int)OtherAreas.FLY_HIGH))
                            {
                                message.missionPhase = "0";
                                return message;
                            }
                            message.missionPhase = "2";
                            return message;
                        }
                        if (charaInfo.Coordinates != Coordinates)
                        {
                            PreCoordinates = Coordinates;
                            Coordinates = charaInfo.Coordinates;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.MONARCH_LINN:
                                break;
                            case ZoneId.RIVERNE_SITE_B01:
                                if (charaInfo.MapId == 1)
                                {
                                    if (Coordinates == "(G-8)" || Coordinates == "(G-9)" || Coordinates == "(G-10)"
                                        || Coordinates == "(H-8)" || Coordinates == "(H-9)"
                                        || Coordinates == "(I-8)" || Coordinates == "(I-9)" || Coordinates == "(I-10)"
                                        || (Coordinates == "(J-7)") && (PreCoordinates == "(J-8)" || PreCoordinates == "(K-7)" || PreCoordinates == "(K-8)")
                                        || Coordinates == "(J-8)" || Coordinates == "(J-9)" || Coordinates == "(J-10)"
                                        || Coordinates == "(K-8)" || Coordinates == "(K-9)" || Coordinates == "(K-10)"
                                        || Coordinates == "(L-8)" || Coordinates == "(L-9)" || Coordinates == "(L-10)"
                                        )
                                    {
                                        if (!database.HasItem(charaInfo.CharaId, ItemId.MISTMELT)
                                            && database.GetItemCount(charaInfo.CharaId, ItemId.HIPPOGRYPH_TAILFEATHER) < 2)
                                        {
                                            message.missionKind = MissionKind.Quest;
                                            message.missionType = OtherAreas.FLY_HIGH.ToString();
                                            message.missionPhase = "1";
                                            return message;
                                        }
                                        if (!database.HasItem(charaInfo.CharaId, ItemId.GIANT_SCALE))
                                        {
                                            message.missionPhase = ItemId.GIANT_SCALE.ToString();
                                            return message;
                                        }
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.RIVERNE_SITE_B01.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_B01.ToString() + "_2";
                                        return message;
                                    }

                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    if (Coordinates == "(M-9)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.RIVERNE_SITE_B01.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_B01.ToString() + "_1";
                                        return message;
                                    }
                                    else if ((Coordinates == "(F-6)" && (PreCoordinates == "(F-7)" || PreCoordinates == "(G-6)" || PreCoordinates == "(G-7)"))
                                        || (Coordinates == "(F-7)" && (PreCoordinates == "(F-6)" || PreCoordinates == "(G-6)" || PreCoordinates == "(G-7)"))
                                        || Coordinates == "(G-6)" || Coordinates == "(G-7)"
                                        )
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.RIVERNE_SITE_B01.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_B01.ToString() + "_3";
                                        return message;
                                    }
                                    else if (Coordinates == "(E-6)" || Coordinates == "(E-7)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.RIVERNE_SITE_B01.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_B01.ToString() + "_4";
                                        return message;
                                    }
                                    else if (Coordinates == "(E-8)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.MONARCH_LINN.ToString();
                                        message.missionPhase = ZoneId.RIVERNE_SITE_B01.ToString();
                                        return message;
                                    }
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.RIVERNE_SITE_B01.ToString();
                                message.missionPhase = ZoneId.MISAREAUX_COAST.ToString();
                                return message;
                        }
                    }
                    break;
                // 礼拝の意味
                case MissionCOP.THE_SECRETS_OF_WORSHIP:
                    if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.OTHER_AREAS, (int)OtherAreas.A_HARD_DAYS_KNIGHT))
                    {
                        message.missionKind = MissionKind.Quest;
                        message.missionType = OtherAreas.A_HARD_DAYS_KNIGHT.ToString();
                        if (database.HasQuestCurrent(charaInfo.CharaId, QuestId.OTHER_AREAS, (int)OtherAreas.A_HARD_DAYS_KNIGHT))
                        {
                            var progString = $"Quest[{(int)QuestId.OTHER_AREAS}][{(int)OtherAreas.A_HARD_DAYS_KNIGHT}]Prog";
                            var prog = database.GetVarNum(charaInfo.CharaId, progString);
                            if (prog == 1)
                            {
                                message.missionPhase = "2";
                                return message;
                            }
                            message.missionPhase = "1";
                            return message;
                        }
                        message.missionPhase = "0";
                        return message;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.TEMPLE_KNIGHT_KEY)
                        && database.HasItem(charaInfo.CharaId, ItemId.SEALION_CREST_KEY)
                        && database.HasItem(charaInfo.CharaId, ItemId.CORAL_CREST_KEY)
                        )
                    {
                        message.missionPhase = "238_2_3";
                        return message;
                    }
                    if (status == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.SACRARIUM:
                                if (charaInfo.MapId == 1)
                                {
                                    var weekOffset = VanaDay.GetVanaDayOfWeek();
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.SACRARIUM.ToString();
                                    message.missionPhase = ZoneId.SACRARIUM.ToString() + "_WEEKLY_" + weekOffset.ToString();
                                    return message;
                                }
                                if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.TEMPLE_KNIGHT_KEY))
                                {
                                    if (!database.HasItem(charaInfo.CharaId, ItemId.CORAL_CREST_KEY))
                                    {
                                        message.missionPhase = "238_2_1";
                                        return message;
                                    }
                                    if (!database.HasItem(charaInfo.CharaId, ItemId.SEALION_CREST_KEY))
                                    {
                                        message.missionPhase = "238_2_2";
                                        return message;
                                    }
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SACRARIUM.ToString();
                                message.missionPhase = ZoneId.MISAREAUX_COAST.ToString();
                                return message;
                        }
                    }
                    if (status == 3)
                    {
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.RELIQUIARIUM_KEY))
                        {
                            message.missionPhase += "_1";
                            return message;
                        }
                    }
                    break;
                // そしりを受けつつも
                case MissionCOP.SLANDEROUS_UTTERINGS:
                    if (status == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.SEALIONS_DEN:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SEALIONS_DEN.ToString();
                                message.missionPhase = ZoneId.TAVNAZIAN_SAFEHOLD.ToString();
                                return message;
                        }
                    }
                    break;
                // 願わくば闇よ
                case MissionCOP.DESIRES_OF_EMPTINESS:
                    if (charaInfo.Coordinates != Coordinates)
                    {
                        PreCoordinates = Coordinates;
                        Coordinates = charaInfo.Coordinates;
                    }
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.SPIRE_OF_VAHZL:
                            break;
                        case ZoneId.PROMYVION_VAHZL:
                            if (Coordinates == "(G-11)" || Coordinates == "(G-12)"
                                || (Coordinates == "(H-8)" && (PreCoordinates == "(H-9)" || PreCoordinates == "(I-8)" || PreCoordinates == "(I-9)"))
                                || Coordinates == "(H-9)" || Coordinates == "(H-10)" || Coordinates == "(H-11)" || Coordinates == "(H-12)"
                                || (Coordinates == "(I-8)" && (PreCoordinates == "(H-8)" || PreCoordinates == "(H-9)" || PreCoordinates == "(I-9)"))
                                || Coordinates == "(I-9)" || Coordinates == "(I-10)" || Coordinates == "(I-11)" || Coordinates == "(I-12)"
                                || (Coordinates == "(J-10)" && (PreCoordinates == "(I-10)" || PreCoordinates == "(I-11)" || PreCoordinates == "(J-11)"))
                                || Coordinates == "(J-11)" || Coordinates == "(J-12)"
                                )
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PROMYVION_VAHZL.ToString();
                                message.missionPhase = ZoneId.PROMYVION_VAHZL.ToString() + "_1_2";
                                return message;
                            }
                            else if (Coordinates == "(F-4)" || Coordinates == "(F-5)" || Coordinates == "(F-6)"
                                || Coordinates == "(G-5)" || Coordinates == "(G-6)"
                                || Coordinates == "(H-4)" || Coordinates == "(H-5)"
                                || (Coordinates == "(H-6)" && (PreCoordinates == "(G-6)" || PreCoordinates == "(G-5)" || PreCoordinates == "(H-5)" || PreCoordinates == "(I-5)" || PreCoordinates == "(I-6)"))
                                || Coordinates == "(I-4)" || Coordinates == "(I-5)"
                                || (Coordinates == "(I-6)" && (PreCoordinates == "(H-6)" || PreCoordinates == "(H-5)" || PreCoordinates == "(I-5)"))
                                || Coordinates == "(J-3)" || Coordinates == "(J-4)"
                                || (Coordinates == "(J-5)" && (PreCoordinates == "(I-5)" || PreCoordinates == "(I-4)" || PreCoordinates == "(H-4)"))
                                )
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PROMYVION_VAHZL.ToString();
                                message.missionPhase = ZoneId.PROMYVION_VAHZL.ToString() + "_2_3";
                                return message;
                            }
                            else if (Coordinates == "(J-8)" || Coordinates == "(J-9)"
                                || (Coordinates == "(J-10)" && (PreCoordinates == "(J-9)" || PreCoordinates == "(K-9)" || PreCoordinates == "(K-10)"))
                                || Coordinates == "(K-8)" || Coordinates == "(K-9)" || Coordinates == "(K-10)" || Coordinates == "(K-11)"
                                || Coordinates == "(L-8)" || Coordinates == "(L-9)" || Coordinates == "(L-10)" || Coordinates == "(L-11)"
                                || Coordinates == "(M-8)" || Coordinates == "(M-9)" || Coordinates == "(M-10)" || Coordinates == "(M-11)"
                                )
                            {
                                if ((option & 1) <= 0)
                                {
                                    message.missionPhase = $"{copMission.StatusLower}";
                                    return message;
                                }
                                if ((option & 8) <= 0)
                                {
                                    message.missionPhase = $"{copMission.StatusLower}_option1";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PROMYVION_VAHZL.ToString();
                                message.missionPhase = ZoneId.PROMYVION_VAHZL.ToString() + "_3_4";
                                return message;
                            }
                            else if ((Coordinates == "(I-6)" && (PreCoordinates == "(I-7)" || PreCoordinates == "(J-6)" || PreCoordinates == "(J-7)"))
                                || (Coordinates == "(I-7)" && (PreCoordinates == "(I-6)" || PreCoordinates == "(J-6)" || PreCoordinates == "(J-7)"))
                                || Coordinates == "(J-6)" || Coordinates == "(J-7)"
                                || Coordinates == "(K-4)" || Coordinates == "(K-5)" || Coordinates == "(K-6)" || Coordinates == "(K-7)"
                                || Coordinates == "(L-4)" || Coordinates == "(L-5)" || Coordinates == "(L-6)" || Coordinates == "(L-7)"
                                || Coordinates == "(M-4)" || Coordinates == "(M-5)" || Coordinates == "(M-6)" || Coordinates == "(M-7)"
                                || Coordinates == "(N-4)" || Coordinates == "(N-5)" || Coordinates == "(N-6)" || Coordinates == "(N-7)"
                                )
                            {
                                if ((option & 2) <= 0)
                                {
                                    message.missionPhase = $"{copMission.StatusLower}_{status}_option9";
                                    return message;
                                }
                                if ((option & 16) <= 0)
                                {
                                    message.missionPhase = $"{copMission.StatusLower}_{status}_option11";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PROMYVION_VAHZL.ToString();
                                message.missionPhase = ZoneId.PROMYVION_VAHZL.ToString() + "_4_5";
                                return message;
                            }
                            else if (Coordinates == "(B-6)" || Coordinates == "(B-7)"
                                || Coordinates == "(C-6)" || Coordinates == "(C-7)"
                                || Coordinates == "(D-5)" || Coordinates == "(D-6)" || Coordinates == "(D-7)" || Coordinates == "(D-8)"
                                || Coordinates == "(E-5)" || Coordinates == "(E-6)" || Coordinates == "(E-7)" || Coordinates == "(E-8)" || Coordinates == "(E-9)"
                                || Coordinates == "(F-7)" || Coordinates == "(F-8)" || Coordinates == "(F-9)" || Coordinates == "(F-10)" || Coordinates == "(F-11)"
                                || Coordinates == "(G-7)" || Coordinates == "(G-8)" || Coordinates == "(G-9)" || Coordinates == "(G-10)"
                                || (Coordinates == "(H-6)" && (PreCoordinates == "(H-7)" || PreCoordinates == "(I-6)" || PreCoordinates == "(I-7)"))
                                || Coordinates == "(H-7)"
                                || (Coordinates == "(H-8)" && (PreCoordinates == "(G-8)" || PreCoordinates == "(G-7)" || PreCoordinates == "(H-7)" || PreCoordinates == "(I-7)" || PreCoordinates == "(I-8)"))
                                || (Coordinates == "(I-6)" && (PreCoordinates == "(H-6)" || PreCoordinates == "(H-7)" || PreCoordinates == "(I-7)"))
                                || (Coordinates == "(I-7)" && (PreCoordinates == "(H-6)" || PreCoordinates == "(H-7)" || PreCoordinates == "(H-8)" || PreCoordinates == "(I-6)" || PreCoordinates == "(I-8)"))
                                || (Coordinates == "(I-8)" && (PreCoordinates == "(H-7)" || PreCoordinates == "(H-8)" || PreCoordinates == "(I-7)"))
                                )
                            {
                                if ((option & 4) <= 0)
                                {
                                    message.missionPhase = $"{copMission.StatusLower}_{status}_option27";
                                    return message;
                                }
                                if ((option & 32) <= 0)
                                {
                                    message.missionPhase = $"{copMission.StatusLower}_{status}_option31";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SPIRE_OF_VAHZL.ToString();
                                message.missionPhase = ZoneId.PROMYVION_VAHZL.ToString();
                                return message;
                            }
                            // 5層開始位置が座標だけで判断できないので、暫定対応
                            if ((option & 4) <= 0)
                            {
                                message.missionPhase = $"{copMission.StatusLower}_{status}_option27";
                                return message;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                // みっつの道
                case MissionCOP.THREE_PATHS:
                    if (copMission.StatusLower == 1006)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ABDHALJS_ISLE_PURGONORGO:
                                break;
                            case ZoneId.MANACLIPPER:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MANACLIPPER.ToString();
                                message.missionPhase = ZoneId.BIBIKI_BAY.ToString();
                                return message;
                            case ZoneId.BIBIKI_BAY:
                                if (charaInfo.MapId == 2)
                                {
                                    break;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ABDHALJS_ISLE_PURGONORGO.ToString();
                                message.missionPhase = ZoneId.BIBIKI_BAY.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BIBIKI_BAY.ToString();
                                message.missionPhase = ZoneId.BUBURIMU_PENINSULA.ToString();
                                return message;
                        }
                    }
                    else if (copMission.StatusLower == 1774)
                    {
                        if (!database.HasItem(charaInfo.CharaId, ItemId.SNOW_LILY))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.ULEGUERAND_RANGE:
                                    message.missionPhase = ItemId.SNOW_LILY.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.ULEGUERAND_RANGE.ToString();
                                    message.missionPhase = ZoneId.XARCABARD.ToString();
                                    return message;
                            }
                        }
                        else
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.MINE_SHAFT_2716:
                                    message.missionPhase = "2286_2";
                                    return message;
                                case ZoneId.OLDTON_MOVALPOLOS:
                                    break;
                                case ZoneId.GUSGEN_MINES:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.OLDTON_MOVALPOLOS.ToString();
                                    message.missionPhase = ZoneId.GUSGEN_MINES.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.GUSGEN_MINES.ToString();
                                    message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                    return message;
                            }
                        }
                    }
                    else if (copMission.StatusLower == 2286)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.MINE_SHAFT_2716:
                                message.missionPhase = "2286_2";
                                return message;
                            case ZoneId.OLDTON_MOVALPOLOS:
                                break;
                            case ZoneId.GUSGEN_MINES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.OLDTON_MOVALPOLOS.ToString();
                                message.missionPhase = ZoneId.GUSGEN_MINES.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GUSGEN_MINES.ToString();
                                message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                return message;
                        }
                    }
                    else if (copMission.StatusLower == 3054)
                    {
                        if (!database.HasItem(charaInfo.CharaId, ItemId.GOLD_KEY))
                        {
                            message.missionPhase = ItemId.GOLD_KEY.ToString();
                            return message;
                        }
                    }
                    else if (copMission.StatusLower == 12014)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.PSOXJA:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PSOXJA.ToString();
                                    message.missionPhase = ZoneId.PSOXJA.ToString() + "_1_2";
                                    return message;
                                }
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PSOXJA.ToString();
                                message.missionPhase = ZoneId.BEAUCEDINE_GLACIER.ToString() + "_2";
                                return message;
                        }
                    }
                    else if (copMission.StatusLower == 36590)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.LOWER_DELKFUTTS_TOWER:
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.LOWER_DELKFUTTS_TOWER.ToString();
                                message.missionPhase = ZoneId.QUFIM_ISLAND.ToString();
                                return message;
                        }
                    }
                    else if (copMission.StatusLower == 48878)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.PSOXJA:
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PSOXJA.ToString();
                                    message.missionPhase = ZoneId.PSOXJA.ToString() + "_3_4";
                                    return message;
                                }
                                return message;
                            default:
                                return message;
                        }
                    }
                    else if (copMission.StatusLower == 61166)
                    {
                        if (copMission.StatusUpper == 7)
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.BONEYARD_GULLY:
                                    break;
                                case ZoneId.ATTOHWA_CHASM:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BONEYARD_GULLY.ToString();
                                    message.missionPhase = ZoneId.ATTOHWA_CHASM.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.ATTOHWA_CHASM.ToString();
                                    message.missionPhase = ZoneId.MAZE_OF_SHAKHRAMI.ToString();
                                    return message;
                            }
                        }
                        else if (copMission.StatusUpper == 8)
                        {
                            if (!database.HasItem(charaInfo.CharaId, ItemId.CHUNK_OF_SHUMEYO_SALT)
                                && charaInfo.ZoneId != ZoneId.BEARCLAW_PINNACLE)
                            {
                                if (database.GetItemCount(charaInfo.CharaId, ItemId.CLUSTER_CORE) < 2)
                                {
                                    message.missionKind = MissionKind.Quest;
                                    message.missionType = OtherAreas.BOMBS_AWAY.ToString();
                                    message.missionPhase = "1";
                                    return message;
                                }
                            }
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.BEARCLAW_PINNACLE:
                                    break;
                                case ZoneId.ULEGUERAND_RANGE:
                                    if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.OTHER_AREAS, (int)OtherAreas.BOMBS_AWAY)
                                        && !database.HasQuestComplete(charaInfo.CharaId, QuestId.OTHER_AREAS, (int)OtherAreas.BOMBS_AWAY))
                                    {
                                        message.missionKind = MissionKind.Quest;
                                        message.missionType = OtherAreas.BOMBS_AWAY.ToString();
                                        message.missionPhase = "0";
                                        return message;
                                    }
                                    if (!database.HasItem(charaInfo.CharaId, ItemId.CHUNK_OF_SHUMEYO_SALT))
                                    {
                                        message.missionKind = MissionKind.Quest;
                                        message.missionType = OtherAreas.BOMBS_AWAY.ToString();
                                        message.missionPhase = "2";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BEARCLAW_PINNACLE.ToString();
                                    message.missionPhase = ZoneId.ULEGUERAND_RANGE.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.ULEGUERAND_RANGE.ToString();
                                    message.missionPhase = ZoneId.XARCABARD.ToString();
                                    return message;
                            }
                        }
                    }
                    break;
                // 鎖と絆
                case MissionCOP.CHAINS_AND_BONDS:
                    if (status == 2)
                    {
                        message.missionPhase = $"{copMission.StatusLower}_upper{copMission.StatusUpper}_status{status}";
                        if ((option & 1) == 0)
                        {
                            message.missionPhase += "";
                        }
                        else if ((option & 2) == 0)
                        {
                            message.missionPhase += "_option5";
                        }
                        else if ((option & 4) == 0)
                        {
                            message.missionPhase += "_option1";
                        }
                        return message;
                    }
                    break;
                // 眦決して
                case MissionCOP.FIRE_IN_THE_EYES_OF_MEN:
                    if (status == 1)
                    {
                        var timerString = $"Mission[{(int)MissionId.COP}][{(int)copMission.Current}]Timer";
                        var timer = database.GetVarNum(charaInfo.CharaId, timerString);
                        if (timer > 0)
                        {
                            message.missionPhase += "_timer";
                            return message;
                        }
                    }
                    break;
                // 決別の前
                case MissionCOP.CALM_BEFORE_THE_STORM:
                    if (copMission.StatusUpper == 14)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CARPENTERS_LANDING:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CARPENTERS_LANDING.ToString();
                                message.missionPhase = ZoneId.JUGNER_FOREST.ToString() + "_2";
                                return message;
                        }
                    }
                    else if ((copMission.StatusUpper & 512) == 0)
                    {
                        break;
                    }
                    else if ((copMission.StatusUpper & 1024) == 0)
                    {
                        message.missionPhase = $"{copMission.StatusLower}_upper526";
                        return message;
                    }
                    else if ((copMission.StatusUpper & 256) == 0)
                    {
                        message.missionPhase = $"{copMission.StatusLower}_upper1550";
                        return message;
                    }
                    if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.LETTERS_FROM_ULMIA_AND_PRISHE))
                    {
                        message.missionPhase += "_next";
                        return message;
                    }
                    break;
                // 古代の園
                case MissionCOP.GARDEN_OF_ANTIQUITY:
                    if (status == 1)
                    {
                        if ((copMission.StatusUpper & 8192) == 0)
                        {
                            message.missionPhase = $"{copMission.StatusLower}_upper1806_status{status}";
                        }
                        else if ((copMission.StatusUpper & 2048) == 0)
                        {
                            message.missionPhase = $"{copMission.StatusLower}_upper9998_status{status}";
                        }
                        else if ((copMission.StatusUpper & 4096) == 0)
                        {
                            message.missionPhase = $"{copMission.StatusLower}_upper12046_status{status}";
                        }
                    }
                    break;
                // 選ばれし死
                case MissionCOP.A_FATE_DECIDED:
                    if (status == 0)
                    {
                        if (charaInfo.Coordinates != Coordinates)
                        {
                            PreCoordinates = Coordinates;
                            Coordinates = charaInfo.Coordinates;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.GRAND_PALACE_OF_HUXZOI:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                    message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString() + "_1_2";
                                    return message;
                                }
                                else if (charaInfo.MapId == 15)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                    if (Coordinates == "(I-6)" || Coordinates == "(I-7)" || Coordinates == "(I-8)" || Coordinates == "(I-9)" || Coordinates == "(I-10)" || Coordinates == "(I-11)" || Coordinates == "(I-12)" || Coordinates == "(I-13)"
                                        || Coordinates == "(J-7)" || Coordinates == "(J-8)" || Coordinates == "(J-9)" || Coordinates == "(J-10)" || Coordinates == "(J-11)" || Coordinates == "(J-12)" || Coordinates == "(J-13)"
                                        || Coordinates == "(K-7)" || Coordinates == "(K-8)" || Coordinates == "(K-11)" || Coordinates == "(K-12)" || Coordinates == "(K-13)"
                                        || Coordinates == "(L-6)" || Coordinates == "(L-7)" || Coordinates == "(L-8)" || Coordinates == "(L-9)"
                                        || Coordinates == "(M-7)" || Coordinates == "(M-8)")
                                    {
                                        message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString() + "_2_3";
                                        return message;
                                    }
                                    message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString() + "_2_3_2";
                                    return message;
                                }
                                else if (charaInfo.MapId == 16)
                                {
                                    if (Coordinates == "(G-4)" || Coordinates == "(G-5)"
                                        || Coordinates == "(H-4)" || Coordinates == "(H-5)" || Coordinates == "(H-6)" || Coordinates == "(H-7)" || Coordinates == "(H-8)"
                                        || Coordinates == "(I-4)" || Coordinates == "(I-5)")
                                    {
                                        break;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                    message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString() + "_3_2";
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString() + "_1_2";
                                return message;
                        }
                    }
                    break;
                // 天使たちの抗い
                case MissionCOP.WHEN_ANGELS_FALL:
                    if (status == 1)
                    {
                        var look = database.GetLook(charaInfo.CharaId);
                        switch (look.race)
                        {
                            case RaceId.HUME_MALE:
                            case RaceId.HUME_FEMALE:
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.THE_GARDEN_OF_RUHMET:
                                        if (charaInfo.MapId == 1)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_1_2_H";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 2)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_2_3_H";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 3)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_3_4_H";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 4)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_4_H";
                                            return message;
                                        }
                                        break;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                        message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                        return message;
                                }
                                break;
                            case RaceId.ELVAAN_MALE:
                            case RaceId.ELVAAN_FEMALE:
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.THE_GARDEN_OF_RUHMET:
                                        if (charaInfo.MapId == 1)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_1_2_E";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 2)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_2_3_E";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 3)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_3_4_E";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 4)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_4_E";
                                            return message;
                                        }
                                        break;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                        message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                        return message;
                                }
                                break;
                            case RaceId.TARUTARU_MALE:
                            case RaceId.TARUTARU_FEMALE:
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.THE_GARDEN_OF_RUHMET:
                                        if (charaInfo.MapId == 1)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_1_2_T";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 2)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_2_3_T";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 3)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_3_4_T";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 4)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_4_T";
                                            return message;
                                        }
                                        break;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                        message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                        return message;
                                }
                                break;
                            case RaceId.MITHRA:
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.THE_GARDEN_OF_RUHMET:
                                        if (charaInfo.MapId == 1)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_1_2_M";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 2)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_2_3_M";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 3)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_3_4_M";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 4)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_4_M";
                                            return message;
                                        }
                                        break;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                        message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                        return message;
                                }
                                break;
                            case RaceId.GALKA:
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.THE_GARDEN_OF_RUHMET:
                                        if (charaInfo.MapId == 1)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_1_2_G";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 2)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_2_3_G";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 3)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_3_4_G";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 4)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                            message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_4_G";
                                            return message;
                                        }
                                        break;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                        message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                        return message;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (status == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.THE_GARDEN_OF_RUHMET:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                    message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString() + "_1_2_BRAND_OF_DAWN";
                                    return message;
                                }
                                if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.BRAND_OF_DAWN))
                                {
                                    message.missionPhase = KeyItemId.BRAND_OF_DAWN.ToString();
                                    return message;
                                }
                                if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.BRAND_OF_TWILIGHT))
                                {
                                    message.missionPhase = KeyItemId.BRAND_OF_TWILIGHT.ToString();
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                return message;
                        }
                        break;
                    }
                    break;
                // 暁
                case MissionCOP.DAWN:
                    if (status == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.EMPYREAL_PARADOX:
                                break;
                            case ZoneId.THE_GARDEN_OF_RUHMET:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EMPYREAL_PARADOX.ToString();
                                message.missionPhase = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                return message;
                            case ZoneId.GRAND_PALACE_OF_HUXZOI:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.THE_GARDEN_OF_RUHMET.ToString();
                                message.missionPhase = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GRAND_PALACE_OF_HUXZOI.ToString();
                                message.missionPhase = ZoneId.ALTAIEU.ToString();
                                return message;
                        }
                        break;
                    }
                    else if (status == 4)
                    {
                        if ((option & 16) > 0)
                        {
                            message.missionPhase = $"{copMission.StatusLower}_upper{copMission.StatusUpper}_status{status}_option31";
                            return message;
                        }
                        else if ((option & 2) > 0)
                        {
                            message.missionPhase = $"{copMission.StatusLower}_upper{copMission.StatusUpper}_status{status}_option15";
                            var progString = $"Mission[{(int)MissionId.COP}][{(int)copMission.Current}]CProg";
                            var prog = database.GetVarNum(charaInfo.CharaId, progString);
                            if (prog > 0)
                            {
                                message.missionPhase += $"_{prog}";
                            }
                            return message;
                        }
                        else if ((option & 1) > 0)
                        {
                            message.missionPhase = $"{copMission.StatusLower}_upper{copMission.StatusUpper}_status{status}_option13";
                            var progString = $"Mission[{(int)MissionId.COP}][{(int)copMission.Current}]LProg";
                            var prog = database.GetVarNum(charaInfo.CharaId, progString);
                            if (prog > 0)
                            {
                                message.missionPhase += $"_{prog}";
                            }
                            return message;
                        }
                        else if ((option & 4) > 0)
                        {
                            message.missionPhase = $"{copMission.StatusLower}_upper{copMission.StatusUpper}_status{status}_option12";
                            return message;
                        }
                        else if ((option & 8) > 0)
                        {
                            message.missionPhase = $"{copMission.StatusLower}_upper{copMission.StatusUpper}_status{status}_option8";
                            return message;
                        }
                    }
                    else if (status == 6)
                    {
                        var ringString = $"Mission[{(int)MissionId.COP}][{(int)copMission.Current}]firstRing";
                        var ring = database.GetVarNum(charaInfo.CharaId, ringString);
                        if (ring > 0)
                        {
                            message.missionPhase += "_ring";
                        }
                    }
                    else if (status == 8)
                    {
                        if (charaInfo.Coordinates != Coordinates)
                        {
                            PreCoordinates = Coordinates;
                            Coordinates = charaInfo.Coordinates;
                        }

                        if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.STORMS_OF_FATE))
                        {
                            message.missionKind = MissionKind.Quest;
                            message.missionType = QuestJeuno.STORMS_OF_FATE.ToString();

                            var progString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.STORMS_OF_FATE}]Prog";
                            var prog = database.GetVarNum(charaInfo.CharaId, progString);
                            message.missionPhase = prog.ToString();
                            return message;
                        }
                        if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.SHADOWS_OF_THE_DEPARTED))
                        {
                            message.missionKind = MissionKind.Quest;
                            message.missionType = QuestJeuno.SHADOWS_OF_THE_DEPARTED.ToString();

                            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.SHADOWS_OF_THE_DEPARTED))
                            {
                                message.missionPhase = "0";
                                return message;
                            }
                            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.PROMYVION_HOLLA_SLIVER))
                            {
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.PROMYVION_HOLLA:
                                        switch (CheckProvimonArea(charaInfo.ZoneId, Coordinates, PreCoordinates))
                                        {
                                            case 1:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString() + "_1_2";
                                                return message;
                                            case 2:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString() + "_2_3";
                                                return message;
                                            case 3:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString() + "_3_4";
                                                return message;
                                            case 4:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_HOLLA.ToString() + "_3_4_2";
                                                return message;
                                            default:
                                                break;
                                        }
                                        break;
                                    case ZoneId.HALL_OF_TRANSFERENCE:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PROMYVION_HOLLA.ToString();
                                        message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString() + "_OPTION_1";
                                            return message;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                        message.missionPhase = ZoneId.LA_THEINE_PLATEAU.ToString();
                                        return message;
                                }

                                message.missionPhase = KeyItemId.PROMYVION_HOLLA_SLIVER.ToString();
                                return message;
                            }
                            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.PROMYVION_DEM_SLIVER))
                            {
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.PROMYVION_DEM:
                                        switch (CheckProvimonArea(charaInfo.ZoneId, Coordinates, PreCoordinates))
                                        {
                                            case 1:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_DEM.ToString() + "_1_2";
                                                return message;
                                            case 2:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_DEM.ToString() + "_2_3";
                                                return message;
                                            case 3:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_DEM.ToString() + "_3_4";
                                                return message;
                                            case 4:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_DEM.ToString() + "_3_4_2";
                                                return message;
                                            default:
                                                break;
                                        }
                                        break;
                                    case ZoneId.HALL_OF_TRANSFERENCE:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PROMYVION_DEM.ToString();
                                        message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString() + "_OPTION_1";
                                        return message;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                        message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                        return message;
                                }

                                message.missionPhase = KeyItemId.PROMYVION_DEM_SLIVER.ToString();
                                return message;
                            }
                            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.PROMYVION_MEA_SLIVER))
                            {
                                switch (charaInfo.ZoneId)
                                {
                                   case ZoneId.PROMYVION_MEA:
                                        switch (CheckProvimonArea(charaInfo.ZoneId, Coordinates, PreCoordinates))
                                        {
                                            case 1:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_MEA.ToString() + "_1_2";
                                                return message;
                                            case 2:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_MEA.ToString() + "_2_3";
                                                return message;
                                            case 3:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_MEA.ToString() + "_3_4";
                                                return message;
                                            case 4:
                                                message.missionKind = MissionKind.Area;
                                                message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                                message.missionPhase = ZoneId.PROMYVION_MEA.ToString() + "_3_4_2";
                                                return message;
                                            default:
                                                break;
                                        }
                                        break;
                                    case ZoneId.HALL_OF_TRANSFERENCE:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PROMYVION_MEA.ToString();
                                        message.missionPhase = ZoneId.HALL_OF_TRANSFERENCE.ToString() + "_OPTION_1";
                                        return message;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.HALL_OF_TRANSFERENCE.ToString();
                                        message.missionPhase = ZoneId.TAHRONGI_CANYON.ToString();
                                        return message;
                                }

                                message.missionPhase = KeyItemId.PROMYVION_MEA_SLIVER.ToString();
                                return message;
                            }
                            message.missionPhase = "0_end";
                            return message;
                        }
                        if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.APOCALYPSE_NIGH))
                        {
                            message.missionKind = MissionKind.Quest;
                            message.missionType = QuestJeuno.APOCALYPSE_NIGH.ToString();
                            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.APOCALYPSE_NIGH))
                            {
                                message.missionPhase = "Start";
                                return message;
                            }

                            var progString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.APOCALYPSE_NIGH}]Prog";
                            var prog = database.GetVarNum(charaInfo.CharaId, progString);
                            message.missionPhase = prog.ToString();
                            return message;
                        }
                    }
                    break;
                default:
                    break;
            }

            return message;
        }

        private static int CheckProvimonArea(ZoneId zoneid, string Coordinates, string PreCoordinates)
        {
            switch (zoneid)
            {
                case ZoneId.PROMYVION_HOLLA:
                    if (Coordinates == "(H-6)" || Coordinates == "(H-7)"
                        || Coordinates == "(I-6)" || Coordinates == "(I-7)"
                        || Coordinates == "(J-6)" || Coordinates == "(J-7)"
                        || (Coordinates == "(I-5)" && (PreCoordinates == "(H-5)" || PreCoordinates == "(H-6)" || PreCoordinates == "(I-6)"))
                        )
                    {
                        return 1;
                    }
                    else if (Coordinates == "(E-8)" || Coordinates == "(E-9)"
                        || Coordinates == "(F-8)" || Coordinates == "(F-9)" || Coordinates == "(F-10)"
                        || Coordinates == "(G-8)" || Coordinates == "(G-9)" || Coordinates == "(G-10)" || Coordinates == "(G-11)"
                        || Coordinates == "(H-8)" || Coordinates == "(H-9)" || Coordinates == "(H-10)"
                        || Coordinates == "(I-8)" || (Coordinates == "(I-9)" && (PreCoordinates == "(H-9)"))
                        )
                    {
                        return 2;
                    }
                    else if (Coordinates == "(C-6)"
                        || Coordinates == "(D-5)" || Coordinates == "(D-6)" || Coordinates == "(D-7)"
                        || Coordinates == "(E-5)" || Coordinates == "(E-6)" || Coordinates == "(E-7)"
                        || Coordinates == "(F-6)" || Coordinates == "(F-7)"
                        || Coordinates == "(G-6)" || Coordinates == "(G-7)"
                        )
                    {
                        return 3;
                    }
                    else if (Coordinates == "(I-4)" || (Coordinates == "(I-5)" && PreCoordinates == "(I-4)" || PreCoordinates == "(J-5)")
                        || Coordinates == "(J-4)" || Coordinates == "(J-5)"
                        || Coordinates == "(K-3)" || Coordinates == "(K-4)" || Coordinates == "(K-5)"
                        || Coordinates == "(L-4)" || Coordinates == "(L-5)" || Coordinates == "(L-6)"
                        || Coordinates == "(M-5)" || Coordinates == "(M-6)"
                        )
                    {
                        return 4;
                    }
                    break;
                case ZoneId.PROMYVION_DEM:
                    if ((Coordinates == "(I-12)" && PreCoordinates == "(J-12)")
                        || (Coordinates == "(J-9)" && (PreCoordinates == "(J-10)" || PreCoordinates == "(K-9)"))
                        || Coordinates == "(J-10)" || Coordinates == "(J-11)" || Coordinates == "(J-12)"
                        || (Coordinates == "(K-9)" && (PreCoordinates == "(J-9)" || PreCoordinates == "(K-10)" || PreCoordinates == "(A-1)"))
                        || Coordinates == "(K-10)" || Coordinates == "(K-11)" || Coordinates == "(K-12)"
                        )
                    {
                        return 1;
                    }
                    else if ((Coordinates == "(D-7)" && (PreCoordinates == "(D-8)" || PreCoordinates == "(E-7)"))
                        || Coordinates == "(D-8)" || Coordinates == "(D-9)" || Coordinates == "(D-10)"
                        || (Coordinates == "(E-7)" && (PreCoordinates == "(D-7)" || PreCoordinates == "(E-8)" || PreCoordinates == "(F-7)"))
                        || Coordinates == "(E-8)" || Coordinates == "(E-9)" || Coordinates == "(E-10)" || Coordinates == "(E-11)"
                        || (Coordinates == "(F-7)" && (PreCoordinates == "(E-7)" || PreCoordinates == "(F-8)"))
                        || Coordinates == "(F-8)" || Coordinates == "(F-9)" || Coordinates == "(F-10)" || Coordinates == "(F-11)"
                        || (Coordinates == "(G-7)" && PreCoordinates == "(G-8)")
                        || Coordinates == "(G-8)" || Coordinates == "(G-9)" || Coordinates == "(G-10)"
                        || (Coordinates == "(H-9)" && PreCoordinates == "(G-9)")
                        || (Coordinates == "(H-10)" && PreCoordinates == "(G-10)")
                        )
                    {
                        return 2;
                    }
                    else if (Coordinates == "(D-6)"
                        || (Coordinates == "(D-7)" && (PreCoordinates == "(D-6)" || PreCoordinates == "(E-7)"))
                        || Coordinates == "(E-4)" || Coordinates == "(E-5)" || Coordinates == "(E-6)"
                        || (Coordinates == "(E-7)" && (PreCoordinates == "(E-6)" || PreCoordinates == "(D-7)"))
                        || Coordinates == "(F-3)" || Coordinates == "(F-4)" || Coordinates == "(F-5)" || Coordinates == "(F-6)"
                        || (Coordinates == "(F-7)" && PreCoordinates == "(G-7)")
                        || Coordinates == "(G-5)" || Coordinates == "(G-6)"
                        || (Coordinates == "(G-7)" && PreCoordinates == "(F-7)")
                        || Coordinates == "(H-4)" || Coordinates == "(H-5)" || Coordinates == "(H-6)"
                        || (Coordinates == "(H-7)" && PreCoordinates == "(H-6)")
                        || (Coordinates == "(I-6)" && PreCoordinates == "(H-6)")
                        )
                    {
                        return 3;
                    }
                    else if (Coordinates == "(G-11)" || Coordinates == "(G-12)" || Coordinates == "(G-13)" || Coordinates == "(G-14)"
                        || Coordinates == "(H-11)" || Coordinates == "(H-12)" || Coordinates == "(H-13)"
                        || Coordinates == "(I-11)" || Coordinates == "(I-13)"
                        || (Coordinates == "(I-12)" && (PreCoordinates == "(I-10)" || PreCoordinates == "(H-12)"))
                        )
                    {
                        return 4;
                    }
                    break;
                case ZoneId.PROMYVION_MEA:
                    if (Coordinates == "(D-5)"
                        || Coordinates == "(E-4)" || Coordinates == "(E-5)"
                        || Coordinates == "(F-4)" || Coordinates == "(F-5)"
                        || (Coordinates == "(F-6)" && (PreCoordinates == "(F-5)" || PreCoordinates == "(G-6)"))
                        || (Coordinates == "(G-5)" && (PreCoordinates == "(F-5)" || PreCoordinates == "(G-6)"))
                        || (Coordinates == "(G-6)" && (PreCoordinates == "(F-5)" || PreCoordinates == "(G-6)"))
                        || (Coordinates == "(G-6)" && !(PreCoordinates == "(H-6)" || PreCoordinates == "(G-7)"))
                        )
                    {
                        return 1;
                    }
                    else if (Coordinates == "(D-10)" || Coordinates == "(D-11)" || Coordinates == "(D-12)" || Coordinates == "(D-13)"
                        || (Coordinates == "(E-9)" && (PreCoordinates == "(E-10)" || PreCoordinates == "(F-9)"))
                        || Coordinates == "(E-10)" || Coordinates == "(E-11)" || Coordinates == "(E-12)" || Coordinates == "(E-13)"
                        || (Coordinates == "(F-9)" && (PreCoordinates == "(E-9)" || PreCoordinates == "(F-10)" || PreCoordinates == "(G-9)"))
                        || Coordinates == "(F-10)" || Coordinates == "(F-11)" || Coordinates == "(F-12)" || Coordinates == "(F-13)"
                        || (Coordinates == "(G-8)" && PreCoordinates == "(G-9)")
                        || Coordinates == "(G-9)" || Coordinates == "(G-10)" || Coordinates == "(G-11)" || Coordinates == "(G-12)"
                        || Coordinates == "(H-9)" || Coordinates == "(H-10)" || Coordinates == "(H-11)" || Coordinates == "(H-12)" || Coordinates == "(H-13)"
                        || (Coordinates == "(I-10)" && (PreCoordinates == "(H-10)" || PreCoordinates == "(I-11)"))
                        || (Coordinates == "(I-11)" && (PreCoordinates == "(I-10)" || PreCoordinates == "(H-11)" || PreCoordinates == "(I-12)"))
                        || Coordinates == "(H-12)" || Coordinates == "(H-13)"
                        )
                    {
                        return 2;
                    }
                    else if (Coordinates == "(C-9)"
                        || Coordinates == "(D-7)" || Coordinates == "(D-8)" || Coordinates == "(D-9)"
                        || Coordinates == "(E-6)" || Coordinates == "(E-7)" || Coordinates == "(E-8)"
                        || (Coordinates == "(E-9)" && (PreCoordinates == "(E-8)" || PreCoordinates == "(F-9)"))
                        || (Coordinates == "(F-6)" && (PreCoordinates == "(F-7)" || PreCoordinates == "(G-8)"))
                        || Coordinates == "(F-7)" || Coordinates == "(F-8)"
                        || (Coordinates == "(G-6)" && (PreCoordinates == "(F-6)" || PreCoordinates == "(G-7)"))
                        || Coordinates == "(G-7)"
                        || (Coordinates == "(G-8)" && (PreCoordinates == "(F-8)" || PreCoordinates == "(G-7)" || PreCoordinates == "(G-8)"))
                        || (Coordinates == "(H-7)" && (PreCoordinates == "(G-7)" || PreCoordinates == "(H-8)"))
                        || Coordinates == "(H-8)"
                        )
                    {
                        return 3;
                    }
                    else if ((Coordinates == "(I-8)" && (PreCoordinates == "(I-9)" || PreCoordinates == "(J-8)"))
                        || Coordinates == "(I-9)"
                        || (Coordinates == "(I-10)" && (PreCoordinates == "(I-9)" || PreCoordinates == "(J-10)" || PreCoordinates == "(I-11)"))
                        || (Coordinates == "(I-11)" && (PreCoordinates == "(I-10)" || PreCoordinates == "(J-11)"))
                        || (Coordinates == "(J-8)" && (PreCoordinates == "(I-8)" || PreCoordinates == "(J-9)" || PreCoordinates == "(K-8)"))
                        || Coordinates == "(J-9)" || Coordinates == "(J-10)" || Coordinates == "(J-11)" || Coordinates == "(J-12)"
                        || Coordinates == "(K-8)" || Coordinates == "(K-9)" || Coordinates == "(K-10)" || Coordinates == "(K-11)" || Coordinates == "(K-12)"
                        || Coordinates == "(L-9)" || Coordinates == "(L-10)" || Coordinates == "(L-11)"
                        || Coordinates == "(M-9)" || Coordinates == "(M-10)"
                        )
                    {
                        return 4;
                    }
                    break;
                default:
                    break;
            }
            return 0;
        }
    }
}
