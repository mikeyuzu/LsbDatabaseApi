using LsbDatabaseApi.@struct;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class WindurstMission
    {
        public static MessageParam GetMessageWindurst(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();

            var windurstMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.WINDURST);
            message.missionKind = MissionKind.MissionWindurst;
            message.missionType = ((MissionWindurst)windurstMission.Current).ToString();
            message.missionPhase = windurstMission.StatusLower.ToString();
            switch ((MissionWindurst)windurstMission.Current)
            {
                // ホルトト遺跡の大実験
                case MissionWindurst.THE_HORUTOTO_RUINS_EXPERIMENT:
                    if (windurstMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.INNER_HORUTOTO_RUINS:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                return message;
                        }
                    }
                    if (windurstMission.StatusLower == 3)
                    {
                        var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.WINDURST}][{windurstMission.Current}]RandomGizmo");
                        message.missionPhase += "_" + value.ToString();
                        return message;
                    }
                    break;
                // カーディアンの心
                case MissionWindurst.THE_HEART_OF_THE_MATTER:
                    if (windurstMission.StatusLower == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.OUTER_HORUTOTO_RUINS:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.OUTER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                return message;
                        }
                        var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.WINDURST}][{windurstMission.Current}]OrbsPlaced");
                        if (value == 6)
                        {
                            message.missionPhase += "end";
                        }
                    }
                    break;
                // 平和のために
                case MissionWindurst.THE_PRICE_OF_PEACE:
                    if (windurstMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.GIDDEUS:
                                break;
                            case ZoneId.WINDURST_WATERS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_SARUTABARUTA.ToString();
                                message.missionPhase = ZoneId.WINDURST_WATERS.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GIDDEUS.ToString();
                                message.missionPhase = ZoneId.WEST_SARUTABARUTA.ToString();
                                return message;
                        }
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.DRINK_OFFERING))
                        {
                            message.missionPhase += "_" + KeyItemId.DRINK_OFFERING.ToString();
                            return message;
                        }
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.FOOD_OFFERING))
                        {
                            message.missionPhase += "_" + KeyItemId.FOOD_OFFERING.ToString();
                            return message;
                        }
                    }
                    break;
                // 白き書
                case MissionWindurst.LOST_FOR_WORDS:
                    if (windurstMission.StatusLower == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.MAZE_OF_SHAKHRAMI:
                                if (charaInfo.MapId != 16)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MAZE_OF_SHAKHRAMI.ToString();
                                    message.missionPhase = ZoneId.MAZE_OF_SHAKHRAMI.ToString() + "_2_3";
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MAZE_OF_SHAKHRAMI.ToString();
                                message.missionPhase = ZoneId.TAHRONGI_CANYON.ToString();
                                return message;
                        }
                        var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.WINDURST} ][ {windurstMission.Current}]Rock");
                        message.missionPhase += "_" + value.ToString();
                    }
                    else if (windurstMission.StatusLower == 5)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.INNER_HORUTOTO_RUINS:
                                if (charaInfo.MapId != 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_1_2";
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                return message;
                        }
                    }
                    break;
                // 試験の行方
                case MissionWindurst.A_TESTING_TIME:
                    if (windurstMission.StatusLower == 2)
                    {
                        var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.WINDURST} ][ {windurstMission.Current}]KillCount");
                        message.missionParam = value;
                        if (value >= 10)
                        {
                            message.missionPhase += "end";
                        }
                    }
                    break;
                // 三大強国
                case MissionWindurst.THE_THREE_KINGDOMS:
                    if (windurstMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.HEAVENS_TOWER:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.HEAVENS_TOWER.ToString();
                                message.missionPhase = ZoneId.WINDURST_WALLS.ToString();
                                break;
                        }
                        return message;
                    }
                    if (windurstMission.StatusLower == 1)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.CIPHER_OF_SEMIHS_ALTER_EGO))
                        {
                            message.missionKind = MissionKind.Trust;
                            message.missionType = "CIPHER";
                            message.missionPhase = ItemId.CIPHER_OF_SEMIHS_ALTER_EGO.ToString();
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.NORTHERN_SAN_DORIA:
                            case ZoneId.SOUTHERN_SAN_DORIA:
                                break;
                            case ZoneId.WEST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.WEST_RONFAURE.ToString();
                                return message;
                            case ZoneId.LA_THEINE_PLATEAU:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.LA_THEINE_PLATEAU.ToString();
                                return message;
                            case ZoneId.VALKURM_DUNES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.LA_THEINE_PLATEAU.ToString();
                                message.missionPhase = ZoneId.VALKURM_DUNES.ToString();
                                return message;
                            case ZoneId.SELBINA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.VALKURM_DUNES.ToString();
                                message.missionPhase = ZoneId.SELBINA.ToString();
                                return message;
                            case ZoneId.SHIP_BOUND_FOR_SELBINA:
                            case ZoneId.SHIP_BOUND_FOR_SELBINA_PIRATES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SELBINA.ToString();
                                message.missionPhase = ZoneId.SHIP_BOUND_FOR_SELBINA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SELBINA.ToString();
                                message.missionPhase = ZoneId.MHAURA.ToString();
                                return message;
                        }
                        if (QuestMission.GetTrustSandoriaMessage(database, charaInfo, ref message))
                        {
                            return message;
                        }
                    }
                    if (windurstMission.StatusLower == 6)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.BASTOK_MARKETS:
                            case ZoneId.PORT_BASTOK:
                            case ZoneId.BASTOK_MINES:
                            case ZoneId.METALWORKS:
                                break;
                            case ZoneId.SOUTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BASTOK_MARKETS.ToString();
                                message.missionPhase = ZoneId.SOUTH_GUSTABERG.ToString();
                                return message;
                            case ZoneId.NORTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.NORTH_GUSTABERG.ToString();
                                return message;
                            case ZoneId.KONSCHTAT_HIGHLANDS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                message.missionPhase = ZoneId.VALKURM_DUNES.ToString();
                                return message;
                        }
                        if (QuestMission.GetTrustBastokMessage(database, charaInfo, ref message))
                        {
                            return message;
                        }
                    }
                    break;
                // 三大強国 サンドリアルート
                case MissionWindurst.THE_THREE_KINGDOMS_SANDORIA:
                    if (windurstMission.StatusLower == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CHATEAU_DORAGUILLE:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    if (windurstMission.StatusLower == 4)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.GHELSBA_OUTPOST:
                                break;
                            case ZoneId.CHATEAU_DORAGUILLE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                return message;
                            case ZoneId.NORTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GHELSBA_OUTPOST.ToString();
                                message.missionPhase = ZoneId.WEST_RONFAURE.ToString();
                                return message;
                        }
                    }
                    break;
                // 三大強国 サンドリアからバストゥークルート
                case MissionWindurst.THE_THREE_KINGDOMS_BASTOK2:
                    if (windurstMission.StatusLower == 10)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.WAUGHROON_SHRINE:
                                break;
                            case ZoneId.NORTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PALBOROUGH_MINES.ToString();
                                message.missionPhase = ZoneId.NORTH_GUSTABERG.ToString();
                                return message;
                            case ZoneId.PALBOROUGH_MINES:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PALBOROUGH_MINES.ToString();
                                    message.missionPhase = ZoneId.PALBOROUGH_MINES.ToString() + "_1_3";
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.WAUGHROON_SHRINE.ToString();
                                    message.missionPhase = ZoneId.PALBOROUGH_MINES.ToString();
                                }
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.PORT_BASTOK.ToString();
                                return message;
                        }
                    }
                    break;
                // それぞれの正義
                case MissionWindurst.TO_EACH_HIS_OWN_RIGHT:
                    if (windurstMission.StatusLower == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CASTLE_OZTROJA:
                                break;
                            case ZoneId.MERIPHATAUD_MOUNTAINS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                message.missionPhase = ZoneId.MERIPHATAUD_MOUNTAINS.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MERIPHATAUD_MOUNTAINS.ToString();
                                message.missionPhase = ZoneId.TAHRONGI_CANYON.ToString();
                                return message;
                        }
                    }
                    break;
                // 星読み
                case MissionWindurst.WRITTEN_IN_THE_STARS:
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.PORTAL_CHARM))
                    {
                        message.missionKind = MissionKind.MissionWindurst;
                        message.missionType = MissionWindurst.ROLANBERRY.ToString();
                        if (!database.HasItem(charaInfo.CharaId, ItemId.ROLANBERRY))
                        {
                            message.missionPhase = "0";
                        }
                        else
                        {
                            message.missionPhase = "1";
                        }
                        return message;
                    }
                    if (windurstMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.INNER_HORUTOTO_RUINS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_1_2";
                                    return message;
                                }
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_2_3";
                                    return message;
                                }
                                if (charaInfo.MapId == 3)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_3_4";
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                return message;
                        }
                    }
                    else if (windurstMission.StatusLower == 3)
                    {
                        var num = database.GetItemCount(charaInfo.CharaId, ItemId.RUSTY_DAGGER);
                        if (num < 3)
                        {
                            message.missionPhase = ItemId.RUSTY_DAGGER.ToString();
                        }
                    }
                    break;
                // 新たなる旅立ち
                case MissionWindurst.A_NEW_JOURNEY:
                    if (windurstMission.StatusLower == 1)
                    {
                        if (QuestMission.GetChocobosWoundsMessage(database, charaInfo, ref message))
                        {
                            return message;
                        }
                        if (QuestMission.GetFullSpeedAheadMessage(database, charaInfo, ref message))
                        {
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.RULUDE_GARDENS:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.RULUDE_GARDENS.ToString();
                                message.missionPhase = ZoneId.UPPER_JEUNO.ToString();
                                return message;
                        }
                    }
                    else if (windurstMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.UPPER_DELKFUTTS_TOWER:
                                break;
                            case ZoneId.MIDDLE_DELKFUTTS_TOWER:
                                if (charaInfo.MapId == 4)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString() + "_4_5";
                                }
                                else if (charaInfo.MapId == 5)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString() + "_5_6";
                                }
                                else if (charaInfo.MapId == 6)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString() + "_6_7";
                                }
                                else if (charaInfo.MapId == 7)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString() + "_7_8";
                                }
                                else if (charaInfo.MapId == 8)
                                {
                                    if (charaInfo.Coordinates == "(F-9)"
                                        || charaInfo.Coordinates == "(F-10)"
                                        || charaInfo.Coordinates == "(G-9)"
                                        || charaInfo.Coordinates == "(G-10)"
                                        || charaInfo.Coordinates == "(H-10)"
                                        || charaInfo.Coordinates == "(I-9)"
                                        || charaInfo.Coordinates == "(I-10)"
                                        || charaInfo.Coordinates == "(J-9)"
                                        || charaInfo.Coordinates == "(J-10)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                        message.missionPhase = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString() + "_8_9_2";
                                    }
                                    else
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                        message.missionPhase = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString() + "_8_9";
                                    }
                                }
                                else if (charaInfo.MapId == 9)
                                {
                                    if (charaInfo.Coordinates == "(I-6)"
                                        || charaInfo.Coordinates == "(I-7)"
                                        || charaInfo.Coordinates == "(I-9)"
                                        || charaInfo.Coordinates == "(I-10)"
                                        || charaInfo.Coordinates == "(J-6)"
                                        || charaInfo.Coordinates == "(J-7)"
                                        || charaInfo.Coordinates == "(J-8)"
                                        || charaInfo.Coordinates == "(J-9)"
                                        || charaInfo.Coordinates == "(J-10)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                        message.missionPhase = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString() + "_9_8";
                                    }
                                    else
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.UPPER_DELKFUTTS_TOWER.ToString();
                                        message.missionPhase = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                    }
                                }
                                return message;
                            case ZoneId.LOWER_DELKFUTTS_TOWER:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.LOWER_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.LOWER_DELKFUTTS_TOWER.ToString() + "_1_2";
                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.LOWER_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.LOWER_DELKFUTTS_TOWER.ToString() + "_2_3";
                                }
                                else if (charaInfo.MapId == 15)
                                {
                                    message.missionPhase += "_4";
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MIDDLE_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.LOWER_DELKFUTTS_TOWER.ToString();
                                }
                                return message;
                            case ZoneId.QUFIM_ISLAND:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.LOWER_DELKFUTTS_TOWER.ToString();
                                message.missionPhase = ZoneId.QUFIM_ISLAND.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.QUFIM_ISLAND.ToString();
                                message.missionPhase = ZoneId.PORT_JEUNO.ToString();
                                return message;
                        }

                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.DELKFUTT_KEY))
                        {
                            message.missionPhase += "_3";
                        }
                        else
                        {
                            // デルクフのカギを持っていない場合は、デルクフのカギを取得する必要がある
                            if (database.HasItem(charaInfo.CharaId, ItemId.DELKFUTT_KEY))
                            {
                                message.missionPhase += "_2";
                            }
                        }
                    }
                    break;
                // 魔晶石を奪え
                case MissionWindurst.MAGICITE:
                    if (windurstMission.StatusLower == 2)
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.TENSHODO_MEMBERS_CARD))
                        {
                            var prof = database.GetProfile(charaInfo.CharaId);
                            var fame_point = prof.FameJeuno + ((prof.FameWindurst + prof.FameBastok + prof.FameSandoria) / 3);
                            if (fame_point < 125)
                            {
                                message.missionParam = 10000 + prof.FameJeuno + ((prof.FameWindurst + prof.FameBastok + prof.FameSandoria) / 3);
                                if (fame_point >= 50)
                                {
                                    message.missionParam = 20000 + prof.FameJeuno + ((prof.FameWindurst + prof.FameBastok + prof.FameSandoria) / 3);
                                }
                                message.missionKind = MissionKind.Fame;
                                message.missionType = "Jeuno";
                                message.missionPhase = "3";
                                return message;
                            }
                            var varProgString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.TENSHODO_MEMBERSHIP}]Prog";
                            var value = database.GetVarNum(charaInfo.CharaId, varProgString);
                            message.missionKind = MissionKind.Quest;
                            message.missionType = QuestJeuno.TENSHODO_MEMBERSHIP.ToString();
                            message.missionPhase = value.ToString();
                            if (value == 1 && database.HasKeyItem(charaInfo.CharaId, KeyItemId.TENSHODO_APPLICATION_FORM))
                            {
                                message.missionPhase = "2";
                                return message;
                            }
                            return message;
                        }
                    }
                    if (windurstMission.StatusLower == 3)
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.MAGICITE_OPTISTONE))
                        {
                            if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.CREST_OF_DAVOI))
                            {
                                message.missionKind = MissionKind.Quest;
                                message.missionType = QuestJeuno.CREST_OF_DAVOI.ToString();
                                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.CREST_OF_DAVOI))
                                {
                                    message.missionPhase = "0";
                                }
                                else if (!database.HasItem(charaInfo.CharaId, ItemId.SLICE_OF_COEURL_MEAT))
                                {
                                    message.missionPhase = "1";
                                }
                                else
                                {
                                    message.missionPhase = "1end";
                                }
                                return message;
                            }
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.MONASTIC_CAVERN:
                                    break;
                                case ZoneId.DAVOI:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MONASTIC_CAVERN.ToString();
                                    message.missionPhase = ZoneId.DAVOI.ToString();
                                    return message;
                                case ZoneId.JUGNER_FOREST:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.DAVOI.ToString();
                                    message.missionPhase = ZoneId.JUGNER_FOREST.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.JUGNER_FOREST.ToString();
                                    message.missionPhase = ZoneId.BATALLIA_DOWNS.ToString();
                                    return message;
                            }
                            message.missionPhase = "3_DAVOI";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.MAGICITE_AURASTONE))
                        {
                            if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.MYSTERIES_OF_BEADEAUX_I)
                                || !database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.MYSTERIES_OF_BEADEAUX_II))
                            {
                                message.missionKind = MissionKind.Quest;
                                message.missionType = "MYSTERIES_OF_BEADEAUX_I_AND_II";
                                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.MYSTERIES_OF_BEADEAUX_I)
                                    && !database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.MYSTERIES_OF_BEADEAUX_I))
                                {
                                    message.missionPhase = "0";
                                    return message;
                                }
                                if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.CORUSCANT_ROSARY)
                                    && database.HasItem(charaInfo.CharaId, ItemId.QUADAV_CHARM)
                                    && (database.HasKeyItem(charaInfo.CharaId, KeyItemId.BLACK_MATINEE_NECKLACE)
                                    || database.HasItem(charaInfo.CharaId, ItemId.QUADAV_AUGURY_SHELL)))
                                {
                                    message.missionPhase = "3";
                                    return message;
                                }
                                if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.BLACK_MATINEE_NECKLACE)
                                    && database.HasItem(charaInfo.CharaId, ItemId.QUADAV_AUGURY_SHELL)
                                    && (database.HasKeyItem(charaInfo.CharaId, KeyItemId.CORUSCANT_ROSARY)
                                    || database.HasItem(charaInfo.CharaId, ItemId.QUADAV_CHARM)))
                                    {
                                    message.missionPhase = "4";
                                    return message;
                                }
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.BEADEAUX:
                                        if (charaInfo.MapId == 1 
                                            && !(charaInfo.PreMapId == 15
                                            && charaInfo.PreCoordinates == "(F-8)"))
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.BEADEAUX.ToString();
                                            message.missionPhase = ZoneId.BEADEAUX.ToString() + "_1_2";
                                            return message;
                                        }
                                        else if (charaInfo.MapId == 15)
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.BEADEAUX.ToString();
                                            message.missionPhase = ZoneId.BEADEAUX.ToString() + "_2_1";
                                            return message;
                                        }
                                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.CORUSCANT_ROSARY)
                                            && !database.HasItem(charaInfo.CharaId, ItemId.QUADAV_CHARM))
                                        {
                                            message.missionPhase = "1";
                                            return message;
                                        }
                                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.BLACK_MATINEE_NECKLACE)
                                            && !database.HasItem(charaInfo.CharaId, ItemId.QUADAV_AUGURY_SHELL))
                                        {
                                            message.missionPhase = "2";
                                            return message;
                                        }
                                        break;
                                    case ZoneId.PASHHOW_MARSHLANDS:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.BEADEAUX.ToString();
                                        message.missionPhase = ZoneId.PASHHOW_MARSHLANDS.ToString();
                                        return message;
                                    case ZoneId.ROLANBERRY_FIELDS:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PASHHOW_MARSHLANDS.ToString();
                                        message.missionPhase = ZoneId.ROLANBERRY_FIELDS.ToString();
                                        return message;
                                    default:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.ROLANBERRY_FIELDS.ToString();
                                        message.missionPhase = ZoneId.LOWER_JEUNO.ToString();
                                        return message;
                                }
                            }
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.QULUN_DOME:
                                    break;
                                case ZoneId.BEADEAUX:
                                    if (charaInfo.MapId == 1)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.BEADEAUX.ToString();
                                        message.missionPhase = ZoneId.BEADEAUX.ToString() + "_1_2";
                                        return message;
                                    }
                                    else if (charaInfo.MapId == 15)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.QULUN_DOME.ToString();
                                        message.missionPhase = ZoneId.BEADEAUX.ToString();
                                        return message;
                                    }
                                    break;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.QULUN_DOME.ToString();
                                    message.missionPhase = ZoneId.BEADEAUX.ToString();
                                    return message;
                            }
                            message.missionPhase = "3_BEADEAUX";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.MAGICITE_ORASTONE))
                        {
                            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.YAGUDO_TORCH))
                            {
                                var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.WINDURST} ][ {windurstMission.Current}]Option");
                                message.missionKind = MissionKind.Quest;
                                message.missionType = "YAGUDO_TORCH";
                                message.missionPhase = value.ToString();
                                return message;
                            }
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.ALTAR_ROOM:
                                    break;
                                case ZoneId.CASTLE_OZTROJA:
                                    if (charaInfo.MapId == 1)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_2_3";
                                        return message;
                                    }
                                    else if (charaInfo.MapId == 2)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_3_7";
                                        return message;
                                    }
                                    else if (charaInfo.MapId == 5)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_6_2";
                                        return message;
                                    }
                                    else if (charaInfo.MapId == 6)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_7_1";
                                        return message;
                                    }
                                    else if (charaInfo.MapId == 15)
                                    {
                                        if (charaInfo.PreMapId == 1
                                            && charaInfo.PreCoordinates == "(I-8)")
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                            message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_0_1";
                                            return message;
                                        }
                                        if (charaInfo.PreMapId == 0
                                            && charaInfo.PreCoordinates == "(A-1)")
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                            message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_1_6";
                                            return message;
                                        }
                                        else
                                        {
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.ALTAR_ROOM.ToString();
                                            message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString();
                                            return message;
                                        }
                                    }
                                    break;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_2_3";
                                    return message;
                            }
                            message.missionPhase = "3_CASTLE_OZTROJA";
                            return message;
                        }
                    }
                    break;
                // 最後の護符
                case MissionWindurst.THE_FINAL_SEAL:
                    if (windurstMission.StatusLower == 10)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.FEIYIN:
                                break;
                            case ZoneId.BEAUCEDINE_GLACIER:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FEIYIN.ToString();
                                message.missionPhase = ZoneId.BEAUCEDINE_GLACIER.ToString();
                                return message;
                            case ZoneId.RANGUEMONT_PASS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BEAUCEDINE_GLACIER.ToString();
                                message.missionPhase = ZoneId.RANGUEMONT_PASS.ToString();
                                return message;
                            case ZoneId.EAST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.RANGUEMONT_PASS.ToString();
                                message.missionPhase = ZoneId.EAST_RONFAURE.ToString();
                                return message;
                            case ZoneId.SOUTHERN_SAN_DORIA:
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EAST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    else if (windurstMission.StatusLower == 11)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.QUBIA_ARENA:
                                break;
                            case ZoneId.FEIYIN:
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.QUBIA_ARENA.ToString();
                                message.missionPhase = ZoneId.FEIYIN.ToString();
                                return message;
                        }
                    }
                    break;
                // 最果てにて君を待つ闇
                case MissionWindurst.THE_SHADOW_AWAITS:
                    if (QuestMission.GetLevelCapMessage(database, charaInfo, 60, ref message))
                    {
                        return message;
                    }
                    if (windurstMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.THRONE_ROOM:
                                break;
                            case ZoneId.CASTLE_ZVAHL_KEEP:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_ZVAHL_KEEP.ToString();
                                    message.missionPhase = ZoneId.CASTLE_ZVAHL_KEEP.ToString() + "_1_2";
                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_ZVAHL_KEEP.ToString();
                                    message.missionPhase = ZoneId.CASTLE_ZVAHL_KEEP.ToString() + "_2_3";
                                }
                                else if (charaInfo.MapId == 3)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_ZVAHL_KEEP.ToString();
                                    message.missionPhase = ZoneId.CASTLE_ZVAHL_KEEP.ToString() + "_3_4";
                                }
                                else if (charaInfo.MapId == 4)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.THRONE_ROOM.ToString();
                                    message.missionPhase = ZoneId.CASTLE_ZVAHL_KEEP.ToString();
                                }
                                return message;
                            case ZoneId.CASTLE_ZVAHL_BAILEYS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_ZVAHL_BAILEYS.ToString();
                                    message.missionPhase = ZoneId.CASTLE_ZVAHL_BAILEYS.ToString() + "_1_2";
                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    if (charaInfo.PreMapId == 1)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_ZVAHL_BAILEYS.ToString();
                                        message.missionPhase = ZoneId.CASTLE_ZVAHL_BAILEYS.ToString() + "_2_3";
                                    }
                                    else
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_ZVAHL_KEEP.ToString();
                                        message.missionPhase = ZoneId.CASTLE_ZVAHL_BAILEYS.ToString();
                                    }
                                }
                                else if (charaInfo.MapId == 15)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_ZVAHL_BAILEYS.ToString();
                                    message.missionPhase = ZoneId.CASTLE_ZVAHL_BAILEYS.ToString() + "_3_2";
                                }
                                return message;
                            case ZoneId.XARCABARD:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CASTLE_ZVAHL_BAILEYS.ToString();
                                message.missionPhase = ZoneId.XARCABARD.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.XARCABARD.ToString();
                                message.missionPhase = ZoneId.BEAUCEDINE_GLACIER.ToString();
                                return message;
                        }
                    }
                    break;
                // 満月の泉
                case MissionWindurst.FULL_MOON_FOUNTAIN:
                    if (windurstMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.OUTER_HORUTOTO_RUINS:
                                if (charaInfo.MapId == 4)
                                {
                                    break;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.OUTER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.WEST_SARUTABARUTA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.OUTER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.WEST_SARUTABARUTA.ToString();
                                return message;
                        }
                    }
                    if (windurstMission.StatusLower == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.FULL_MOON_FOUNTAIN:
                                message.missionPhase += "_end";
                                break;
                            case ZoneId.TORAIMARAI_CANAL:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FULL_MOON_FOUNTAIN.ToString();
                                message.missionPhase = ZoneId.TORAIMARAI_CANAL.ToString();
                                return message;
                            case ZoneId.INNER_HORUTOTO_RUINS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_1_2";
                                    return message;
                                }
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_2_3";
                                    return message;
                                }
                                if (charaInfo.MapId == 3)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_3_4";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.TORAIMARAI_CANAL.ToString();
                                message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                return message;
                            case ZoneId.PORT_WINDURST:
                            case ZoneId.OUTER_HORUTOTO_RUINS:
                            case ZoneId.WEST_SARUTABARUTA:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                return message;
                        }
                    }
                    break;
                // 聖者の招待
                case MissionWindurst.SAINTLY_INVITATION:
                    if (windurstMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.BALGAS_DAIS:
                                break;
                            case ZoneId.GIDDEUS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BALGAS_DAIS.ToString();
                                message.missionPhase = ZoneId.GIDDEUS.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GIDDEUS.ToString();
                                message.missionPhase = ZoneId.WEST_SARUTABARUTA.ToString();
                                return message;
                        }
                    }
                    if (windurstMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CASTLE_OZTROJA:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_2_3";
                                    return message;
                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_3_4";
                                    return message;
                                }
                                else if (charaInfo.MapId == 3)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_4_5";
                                    return message;
                                }
                                else if (charaInfo.MapId == 4)
                                {
                                    if (charaInfo.Coordinates == "(G-7)" || charaInfo.Coordinates == "(H-7)"
                                        || charaInfo.Coordinates == "(G-8)" || charaInfo.Coordinates == "(H-8)"
                                        || charaInfo.Coordinates == "(G-9)" || charaInfo.Coordinates == "(H-9)")
                                    {
                                        if (!database.HasItem(charaInfo.CharaId, ItemId.JUDGMENT_KEY))
                                        {
                                            message.missionPhase = ItemId.JUDGMENT_KEY.ToString();
                                            return message;
                                        }
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_5_5";
                                        return message;
                                    }
                                    break;
                                }
                                else if (charaInfo.MapId == 5)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_6_2";
                                    return message;
                                }
                                else if (charaInfo.MapId == 15)
                                {
                                    if (charaInfo.PreMapId == 1
                                        && charaInfo.PreCoordinates == "(I-8)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_0_1";
                                        return message;
                                    }
                                    if (charaInfo.PreMapId == 0
                                        && charaInfo.PreCoordinates == "(A-1)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_1_6";
                                        return message;
                                    }
                                    else
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.ALTAR_ROOM.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString();
                                        return message;
                                    }
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                message.missionPhase = ZoneId.MERIPHATAUD_MOUNTAINS.ToString();
                                return message;
                        }
                    }
                    break;
                // 第6の院
                case MissionWindurst.THE_SIXTH_MINISTRY:
                    if (windurstMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.TORAIMARAI_CANAL:
                                if (charaInfo.MapId == 2)
                                {
                                    if (charaInfo.Coordinates == "(H-8)")
                                    {
                                        break;
                                    }
                                    if (charaInfo.Coordinates != "(G-8)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.TORAIMARAI_CANAL.ToString();
                                        message.missionPhase = ZoneId.TORAIMARAI_CANAL.ToString() + "_2_B_M7_1";
                                        return message;
                                    }
                                    message.missionPhase = ZoneId.TORAIMARAI_CANAL.ToString();
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.TORAIMARAI_CANAL.ToString();
                                message.missionPhase = ZoneId.TORAIMARAI_CANAL.ToString() + "_1_2";
                                return message;
                            case ZoneId.INNER_HORUTOTO_RUINS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_1_2";
                                    return message;
                                }
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_2_3";
                                    return message;
                                }
                                if (charaInfo.MapId == 3)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_3_4";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.TORAIMARAI_CANAL.ToString();
                                message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                return message;
                        }
                    }
                    break;
                // 蘇る神々
                case MissionWindurst.AWAKENING_OF_THE_GODS:
                    if (windurstMission.StatusLower == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.YHOATOR_JUNGLE:
                                if (charaInfo.Coordinates == "(J-9)")
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                    message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString() + "_YU5";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString();
                                return message;
                            case ZoneId.TEMPLE_OF_UGGALEPIH:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.YHOATOR_JUNGLE.ToString();
                                    message.missionPhase = ZoneId.TEMPLE_OF_UGGALEPIH.ToString() + "_YU3";
                                    return message;
                                }
                                if (charaInfo.MapId == 3)
                                {
                                    if (!database.HasItem(charaInfo.CharaId, ItemId.CURSED_KEY))
                                    {
                                        message.missionPhase = ItemId.CURSED_KEY.ToString();
                                        return message;
                                    }
                                    break;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString();
                                return message;
                        }
                    }
                    break;
                // ヴェイン
                case MissionWindurst.VAIN:
                    if (windurstMission.StatusLower == 3)
                    {
                        if (!database.HasItem(charaInfo.CharaId, ItemId.CURSE_WAND))
                        {
                            message.missionPhase = ItemId.CURSE_WAND.ToString();
                            return message;
                        }
                    }
                    break;
                // 王と道化師
                case MissionWindurst.THE_JESTER_WHOD_BE_KING:
                    if (windurstMission.StatusLower == 1)
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.OPTISTERY_RING))
                        {
                            message.missionPhase = KeyItemId.OPTISTERY_RING.ToString();
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.AURASTERY_RING))
                        {
                            message.missionPhase = KeyItemId.AURASTERY_RING.ToString();
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.RHINOSTERY_RING))
                        {
                            if (charaInfo.ZoneId == ZoneId.FEIYIN && charaInfo.MapId == 15)
                            {
                                message.missionPhase = KeyItemId.RHINOSTERY_RING.ToString();
                                return message;
                            }
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.FEIYIN.ToString();
                            message.missionPhase = ZoneId.FEIYIN.ToString() + "_1_2";
                            return message;
                        }
                        break;
                    }
                    if (windurstMission.StatusLower == 4)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.OUTER_HORUTOTO_RUINS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.OUTER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.OUTER_HORUTOTO_RUINS.ToString() + "_1_5";
                                    return message;
                                }
                                if (charaInfo.MapId == 5)
                                {
                                    break;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.OUTER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString() + "_SH2";
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.OUTER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString() + "_SH2";
                                return message;
                        }
                    }
                    if (windurstMission.StatusLower == 9)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.INNER_HORUTOTO_RUINS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_1_2";
                                    return message;
                                }
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_2_3";
                                    return message;
                                }
                                if (charaInfo.MapId == 3)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                    message.missionPhase = ZoneId.INNER_HORUTOTO_RUINS.ToString() + "_3_4";
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.INNER_HORUTOTO_RUINS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                return message;
                        }
                    }
                    break;
                // 死者の人形
                case MissionWindurst.DOLL_OF_THE_DEAD:
                    if (windurstMission.StatusLower == 5)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.THE_BOYAHDA_TREE:
                                if (database.HasItem(charaInfo.CharaId, ItemId.CLUMP_OF_GOOBBUE_HUMUS))
                                {
                                    if (charaInfo.MapId == 2)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.THE_BOYAHDA_TREE.ToString();
                                        message.missionPhase = ZoneId.THE_BOYAHDA_TREE.ToString() + "_2_1";
                                        return message;
                                    }
                                    break;
                                }
                                message.missionPhase = ItemId.CLUMP_OF_GOOBBUE_HUMUS.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.THE_BOYAHDA_TREE.ToString();
                                message.missionPhase = ZoneId.THE_SANCTUARY_OF_ZITAH.ToString();
                                return message;
                        }
                    }
                    break;
                // 月詠み
                case MissionWindurst.MOON_READING:
                    if (windurstMission.StatusLower == 1)
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.ANCIENT_VERSE_OF_ROMAEVE))
                        {
                            message.missionPhase = "ANCIENT_VERSE_OF_ROMAEVE";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.ANCIENT_VERSE_OF_UGGALEPIH))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.TEMPLE_OF_UGGALEPIH:
                                    if (!database.HasItem(charaInfo.CharaId, ItemId.UGGALEPIH_KEY)
                                        && !(charaInfo.MapId == 2 && charaInfo.Coordinates == "(E-8)"))
                                    {
                                        message.missionPhase = ItemId.UGGALEPIH_KEY.ToString();
                                        return message;
                                    }
                                    if (charaInfo.MapId == 1)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.YHOATOR_JUNGLE.ToString();
                                        message.missionPhase = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                        return message;
                                    }
                                    break;
                                case ZoneId.YHOATOR_JUNGLE:
                                    if (charaInfo.PreZoneId == ZoneId.TEMPLE_OF_UGGALEPIH && charaInfo.PreCoordinates == "(F-5)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                        message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString() + "_2";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                    message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                    message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString();
                                    return message;
                            }
                            message.missionPhase = "ANCIENT_VERSE_OF_UGGALEPIH";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.ANCIENT_VERSE_OF_ALTEPA))
                        {
                            message.missionPhase = "ANCIENT_VERSE_OF_ALTEPA";
                            return message;
                        }
                    }
                    break;
                // ミッション未受託
                case MissionWindurst.NONE:
                    if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_HORUTOTO_RUINS_EXPERIMENT))
                    {
                        message.missionPhase = MissionWindurst.THE_HORUTOTO_RUINS_EXPERIMENT.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_HEART_OF_THE_MATTER))
                    {
                        message.missionPhase = MissionWindurst.THE_HEART_OF_THE_MATTER.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_PRICE_OF_PEACE))
                    {
                        message.missionPhase = MissionWindurst.THE_PRICE_OF_PEACE.ToString();
                    }
                    else if (QuestMission.GetSupportJobMessage(database, charaInfo, ref message))
                    {
                        return message;
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.LOST_FOR_WORDS))
                    {
                        message.missionPhase = MissionWindurst.LOST_FOR_WORDS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.A_TESTING_TIME))
                    {
                        message.missionPhase = MissionWindurst.A_TESTING_TIME.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_THREE_KINGDOMS))
                    {
                        message.missionPhase = MissionWindurst.THE_THREE_KINGDOMS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.TO_EACH_HIS_OWN_RIGHT))
                    {
                        message.missionPhase = MissionWindurst.TO_EACH_HIS_OWN_RIGHT.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.WRITTEN_IN_THE_STARS))
                    {
                        message.missionPhase = MissionWindurst.WRITTEN_IN_THE_STARS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.A_NEW_JOURNEY))
                    {
                        message.missionPhase = MissionWindurst.A_NEW_JOURNEY.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.MAGICITE))
                    {
                        message.missionPhase = MissionWindurst.MAGICITE.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_FINAL_SEAL))
                    {
                        message.missionPhase = MissionWindurst.THE_FINAL_SEAL.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_SHADOW_AWAITS))
                    {
                        message.missionPhase = MissionWindurst.THE_SHADOW_AWAITS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.FULL_MOON_FOUNTAIN))
                    {
                        message.missionPhase = MissionWindurst.FULL_MOON_FOUNTAIN.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.SAINTLY_INVITATION))
                    {
                        message.missionPhase = MissionWindurst.SAINTLY_INVITATION.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_SIXTH_MINISTRY))
                    {
                        message.missionPhase = MissionWindurst.THE_SIXTH_MINISTRY.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.AWAKENING_OF_THE_GODS))
                    {
                        message.missionPhase = MissionWindurst.AWAKENING_OF_THE_GODS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.VAIN))
                    {
                        message.missionPhase = MissionWindurst.VAIN.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.THE_JESTER_WHOD_BE_KING))
                    {
                        message.missionPhase = MissionWindurst.THE_JESTER_WHOD_BE_KING.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.DOLL_OF_THE_DEAD))
                    {
                        message.missionPhase = MissionWindurst.DOLL_OF_THE_DEAD.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.WINDURST, (int)MissionWindurst.MOON_READING))
                    {
                        message.missionPhase = MissionWindurst.MOON_READING.ToString();
                    }
                    break;
                default:
                    break;
            }

            return message;
        }
    }
}
