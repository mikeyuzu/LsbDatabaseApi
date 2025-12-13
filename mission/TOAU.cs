using LsbDatabaseApi.@struct;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class TOAUMission
    {
        private static string Coordinates = "";
        private static string PreCoordinates = "";

        public static MessageParam GetMessageTOAU(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();
            if (QuestMission.GetLevelCapMessage(database, charaInfo, 75, ref message))
            {
                return message;
            }

            if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.THE_ROAD_TO_AHT_URHGAN))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestJeuno.THE_ROAD_TO_AHT_URHGAN.ToString();
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.THE_ROAD_TO_AHT_URHGAN))
                {
                    message.missionPhase = "Start";
                    return message;
                }
                var progString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.THE_ROAD_TO_AHT_URHGAN}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, progString);

                message.missionPhase = prog.ToString();
                return message;
            }

            var toauMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.TOAU);
            message.missionKind = MissionKind.MissionTOAU;
            message.missionType = ((MissionTOAU)toauMission.Current).ToString();
            message.missionPhase = toauMission.StatusLower.ToString();
            var optionString = $"Mission[{(int)MissionId.TOAU}][{(int)toauMission.Current}]Option";
            var option = database.GetVarNum(charaInfo.CharaId, optionString);
            if (option > 0)
            {
                message.missionPhase = "option" + option.ToString();
            }
            switch ((MissionTOAU)toauMission.Current)
            {
                case MissionTOAU.LAND_OF_SACRED_SERPENTS:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.AHT_URHGAN_WHITEGATE:
                            break;
                        case ZoneId.OPEN_SEA_ROUTE_TO_AL_ZAHBI:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                            message.missionPhase = ZoneId.OPEN_SEA_ROUTE_TO_AL_ZAHBI.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.OPEN_SEA_ROUTE_TO_AL_ZAHBI.ToString();
                            message.missionPhase = ZoneId.MHAURA.ToString();
                            return message;
                    }
                    break;
                case MissionTOAU.IMMORTAL_SENTRIES:
                    if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.SUPPLIES_PACKAGE))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CAEDARVA_MIRE:
                                break;
                            case ZoneId.NASHMAU:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                message.missionPhase = ZoneId.NASHMAU.ToString();
                                return message;
                            case ZoneId.SILVER_SEA_ROUTE_TO_NASHMAU:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NASHMAU.ToString();
                                message.missionPhase = ZoneId.SILVER_SEA_ROUTE_TO_NASHMAU.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SILVER_SEA_ROUTE_TO_NASHMAU.ToString();
                                message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                return message;
                        }
                        message.missionPhase = "0";
                    }
                    else
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.AHT_URHGAN_WHITEGATE:
                                break;
                            case ZoneId.CAEDARVA_MIRE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                message.missionPhase = ZoneId.CAEDARVA_MIRE.ToString();
                                return message;
                            default:
                                break;
                        }
                        message.missionPhase = "1";
                    }
                    break;
                case MissionTOAU.PRESIDENT_SALAHEEM:
                    if (toauMission.StatusLower == 1)
                    {
                        var toau3prog = database.GetVarNum(charaInfo.CharaId, "ToAU3Progress");
                        if (toau3prog == 0)
                        {
                            message.missionPhase = "Rytaal";
                            return message;
                        }
                    }
                    break;
                case MissionTOAU.UNDERSEA_SCOUTING:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.ALZADAAL_UNDERSEA_RUINS:
                            break;
                        case ZoneId.BHAFLAU_THICKETS:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                            message.missionPhase = ZoneId.BHAFLAU_THICKETS.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.BHAFLAU_THICKETS.ToString();
                            message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                            return message;
                    }
                    break;
                case MissionTOAU.ASTRAL_WAVES:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.AHT_URHGAN_WHITEGATE:
                            break;
                        case ZoneId.ALZADAAL_UNDERSEA_RUINS:
                            if (charaInfo.MapId == 8)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                message.missionPhase = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString() + "_8_3";
                                return message;
                            }
                            else if (charaInfo.MapId == 3)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                message.missionPhase = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString() + "_3_5";
                                return message;
                            }
                            else if (charaInfo.MapId == 5)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                message.missionPhase = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                return message;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case MissionTOAU.ROYAL_PUPPETEER:
                    if (toauMission.StatusLower == 1)
                    {
                        if (!database.HasItem(charaInfo.CharaId, ItemId.VIAL_OF_JODYS_ACID))
                        {
                            message.missionPhase = ItemId.VIAL_OF_JODYS_ACID.ToString();
                            return message;
                        }
                    }
                    break;
                case MissionTOAU.LOST_KINGDOM:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.CAEDARVA_MIRE:
                            break;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                            message.missionPhase = ZoneId.NASHMAU.ToString() + "_2";
                            return message;
                    }
                    break;
                case MissionTOAU.THE_BLACK_COFFIN:
                    if (toauMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ARRAPAGO_REEF:
                                break;
                            case ZoneId.CAEDARVA_MIRE:
                                if (!database.IsRunicPortalOpen(charaInfo.CharaId, RunicPortalId.DVUCCA))
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    message.missionPhase = ZoneId.CAEDARVA_MIRE.ToString();
                                    return message;
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.ARRAPAGO_REEF.ToString();
                                    message.missionPhase = ZoneId.CAEDARVA_MIRE.ToString();
                                    return message;
                                }
                            case ZoneId.ALZADAAL_UNDERSEA_RUINS:
                                if (charaInfo.MapId == 5)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                    message.missionPhase = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString() + "_5_2";
                                    return message;
                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                    message.missionPhase = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString() + "_2_7";
                                    return message;
                                }
                                else if (charaInfo.MapId == 7)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                    message.missionPhase = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                    return message;
                                }
                                break;
                            default:
                                if (!database.IsRunicPortalOpen(charaInfo.CharaId, RunicPortalId.DVUCCA))
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                    message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    return message;
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                    message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    return message;
                                }
                        }
                    }
                    break;
                case MissionTOAU.TEAHOUSE_TUMULT:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.AYDEEWA_SUBTERRANE:
                            if (charaInfo.MapId == 2)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.AYDEEWA_SUBTERRANE.ToString();
                                message.missionPhase = ZoneId.AYDEEWA_SUBTERRANE.ToString() + "_2_5";
                                return message;
                            }
                            break;
                        case ZoneId.WAJAOM_WOODLANDS:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.AYDEEWA_SUBTERRANE.ToString();
                            message.missionPhase = ZoneId.WAJAOM_WOODLANDS.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.WAJAOM_WOODLANDS.ToString();
                            message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                            return message;
                    }
                    break;
                case MissionTOAU.SHIELD_OF_DIPLOMACY:
                    if (toauMission.StatusLower == 0)
                    {
                        if (charaInfo.Coordinates != Coordinates)
                        {
                            PreCoordinates = Coordinates;
                            Coordinates = charaInfo.Coordinates;
                        }

                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.NAVUKGO_EXECUTION_CHAMBER:
                                break;
                            case ZoneId.MOUNT_ZHAYOLM:
                                if (!database.IsRunicPortalOpen(charaInfo.CharaId, RunicPortalId.HALVUNG))
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    message.missionPhase = ZoneId.MOUNT_ZHAYOLM.ToString();
                                    return message;
                                }
                                else if (Coordinates == "(G-6)" || Coordinates == "(G-7)" || Coordinates == "(G-8)"
                                    || Coordinates == "(H-7)" || Coordinates == "(H-8)" || Coordinates == "(H-9)" || Coordinates == "(H-10)"
                                    || Coordinates == "(I-7)" || Coordinates == "(I-8)" || Coordinates == "(I-9)" || Coordinates == "(I-10)"
                                    || Coordinates == "(J-7)" || Coordinates == "(J-8)" || Coordinates == "(J-9)"
                                    || Coordinates == "(K-6)" || Coordinates == "(K-7)" || Coordinates == "(K-8)"
                                    || Coordinates == "(L-6)" || Coordinates == "(L-7)" || Coordinates == "(L-8)"
                                    || Coordinates == "(M-6)" || Coordinates == "(M-7)"
                                    )
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.HALVUNG.ToString();
                                    message.missionPhase = ZoneId.MOUNT_ZHAYOLM.ToString();
                                    return message;
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.NAVUKGO_EXECUTION_CHAMBER.ToString();
                                    message.missionPhase = ZoneId.MOUNT_ZHAYOLM.ToString();
                                    return message;
                                }
                            case ZoneId.HALVUNG:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MOUNT_ZHAYOLM.ToString();
                                    message.missionPhase = ZoneId.HALVUNG.ToString();
                                    return message;
                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MOUNT_ZHAYOLM.ToString();
                                    message.missionPhase = ZoneId.HALVUNG.ToString() + "_2";
                                    return message;
                                }
                                break;
                            case ZoneId.BHAFLAU_THICKETS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.HALVUNG.ToString();
                                message.missionPhase = ZoneId.BHAFLAU_THICKETS.ToString();
                                return message;
                            default:
                                if (!database.IsRunicPortalOpen(charaInfo.CharaId, RunicPortalId.HALVUNG))
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BHAFLAU_THICKETS.ToString();
                                    message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    return message;
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MOUNT_ZHAYOLM.ToString();
                                    message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    return message;
                                }
                        }
                    }
                    break;
                case MissionTOAU.MISPLACED_NOBILITY:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.AYDEEWA_SUBTERRANE:
                            if (charaInfo.MapId == 2)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.AYDEEWA_SUBTERRANE.ToString();
                                message.missionPhase = ZoneId.AYDEEWA_SUBTERRANE.ToString() + "_2_5";
                                return message;
                            }
                            break;
                        case ZoneId.WAJAOM_WOODLANDS:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.AYDEEWA_SUBTERRANE.ToString();
                            message.missionPhase = ZoneId.WAJAOM_WOODLANDS.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.WAJAOM_WOODLANDS.ToString();
                            message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                            return message;
                    }
                    break;
                case MissionTOAU.PUPPET_IN_PERIL:
                    if (toauMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.JADE_SEPULCHER:
                                break;
                            case ZoneId.BHAFLAU_THICKETS:
                                if (!database.IsRunicPortalOpen(charaInfo.CharaId, RunicPortalId.MAMOOL))
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    message.missionPhase = ZoneId.BHAFLAU_THICKETS.ToString();
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.JADE_SEPULCHER.ToString();
                                message.missionPhase = ZoneId.BHAFLAU_THICKETS.ToString();
                                return message;
                            case ZoneId.MAMOOK:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BHAFLAU_THICKETS.ToString();
                                message.missionPhase = ZoneId.MAMOOK.ToString();
                                return message;
                            case ZoneId.WAJAOM_WOODLANDS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MAMOOK.ToString();
                                message.missionPhase = ZoneId.WAJAOM_WOODLANDS.ToString();
                                return message;
                            default:
                                if (database.IsRunicPortalOpen(charaInfo.CharaId, RunicPortalId.MAMOOL))
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BHAFLAU_THICKETS.ToString();
                                    message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString() + "_2";
                                    return message;
                                }
                                else if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.MAMOOK))
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MAMOOK.ToString();
                                    message.missionPhase = "SURVIVAL";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WAJAOM_WOODLANDS.ToString();
                                message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                return message;
                        }
                    }
                    break;
                case MissionTOAU.PREVALENCE_OF_PIRATES:
                    if (toauMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ARRAPAGO_REEF:
                                break;
                            case ZoneId.CAEDARVA_MIRE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ARRAPAGO_REEF.ToString();
                                message.missionPhase = ZoneId.CAEDARVA_MIRE.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                return message;
                        }
                    }
                    break;
                case MissionTOAU.SHADES_OF_VENGEANCE:
                    if (toauMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.AHT_URHGAN_WHITEGATE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                return message;
                            default:
                                break;
                        }
                    }
                    break;
                case MissionTOAU.TESTING_THE_WATERS:
                    if (toauMission.StatusLower == 0)
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.EPHRAMADIAN_GOLD_COIN))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.CAEDARVA_MIRE:
                                    message.missionPhase = "EPHRAMADIAN_GOLD_COIN";
                                    return message;
                                default:
                                    if (database.IsHomePointOpen(charaInfo.CharaId, HomePointId.CAEDARVA_MIRE))
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                        message.missionPhase = "HOMEPOINT";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                    message.missionPhase = ZoneId.NASHMAU.ToString() + "_2";
                                    return message;
                            }
                        }
                        else
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.ARRAPAGO_REEF:
                                    break;
                                case ZoneId.CAEDARVA_MIRE:
                                    if (charaInfo.MapId == 2)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.ARRAPAGO_REEF.ToString();
                                        message.missionPhase = ZoneId.CAEDARVA_MIRE.ToString();
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                    message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CAEDARVA_MIRE.ToString();
                                    message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                    return message;
                            }
                        }
                    }
                    break;
                case MissionTOAU.GAZE_OF_THE_SABOTEUR:
                    if (toauMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.TALACCA_COVE:
                                message.missionPhase = ZoneId.TALACCA_COVE.ToString();
                                break;
                            case ZoneId.CAEDARVA_MIRE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.HAZHALM_TESTING_GROUNDS.ToString();
                                message.missionPhase = ZoneId.CAEDARVA_MIRE.ToString();
                                return message;
                            default:
                                break;
                        }
                    }
                    break;
                case MissionTOAU.PATH_OF_DARKNESS:
                    if (toauMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ALZADAAL_UNDERSEA_RUINS:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                return message;
                        }
                    }
                    else if (toauMission.StatusLower == 1)
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.NYZUL_ISLE_ROUTE)
                            && charaInfo.ZoneId != ZoneId.NYZUL_ISLE)
                        {
                            message.missionPhase = KeyItemId.NYZUL_ISLE_ROUTE.ToString();
                            return message;
                        }                            
                    }
                    break;
                case MissionTOAU.NASHMEIRAS_PLEA:
                    if (toauMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ALZADAAL_UNDERSEA_RUINS:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ALZADAAL_UNDERSEA_RUINS.ToString();
                                message.missionPhase = ZoneId.AHT_URHGAN_WHITEGATE.ToString();
                                return message;
                        }
                    }
                    break;
                default:
                    break;
            }

            return message;
        }
    }
}
