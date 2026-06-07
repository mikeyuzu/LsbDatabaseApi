using LsbDatabaseApi.@struct;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class BastokMission
    {
        public static MessageParam GetMessageBastok(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();

            var bastokMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.BASTOK);
            message.missionKind = MissionKind.MissionBastok;
            message.missionType = ((MissionBastok)bastokMission.Current).ToString();
            message.missionPhase = bastokMission.StatusLower.ToString();
            switch ((MissionBastok)bastokMission.Current)
            {
                // ツェールン鉱山からの報告
                case MissionBastok.THE_ZERUHN_REPORT:
                    if (bastokMission.StatusLower == 0)
                    {
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.ZERUHN_REPORT))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.METALWORKS:
                                    message.missionPhase = KeyItemId.ZERUHN_REPORT.ToString();
                                    return message;
                                case ZoneId.BASTOK_MARKETS:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.METALWORKS.ToString();
                                    message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                    return message;
                                case ZoneId.BASTOK_MINES:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BASTOK_MARKETS.ToString();
                                    message.missionPhase = ZoneId.BASTOK_MINES.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BASTOK_MINES.ToString();
                                    message.missionPhase = ZoneId.ZERUHN_MINES.ToString();
                                    return message;
                            }
                        }

                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ZERUHN_MINES:
                                break;
                            case ZoneId.BASTOK_MINES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ZERUHN_MINES.ToString();
                                message.missionPhase = ZoneId.BASTOK_MINES.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BASTOK_MINES.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                        }
                    }
                    break;
                // 彼の名はシド
                case MissionBastok.GEOLOGICAL_SURVEY:
                    if (bastokMission.StatusLower == 0)
                    {
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.RED_ACIDITY_TESTER))
                        {
                            message.missionPhase = KeyItemId.RED_ACIDITY_TESTER.ToString();
                            return message;
                        }

                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.BLUE_ACIDITY_TESTER))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.DANGRUF_WADI:
                                    message.missionPhase = KeyItemId.BLUE_ACIDITY_TESTER.ToString();
                                    return message;
                                case ZoneId.SOUTH_GUSTABERG:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.DANGRUF_WADI.ToString();
                                    message.missionPhase = ZoneId.SOUTH_GUSTABERG.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.SOUTH_GUSTABERG.ToString();
                                    message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                    return message;
                            }
                        }

                        
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.METALWORKS:
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.METALWORKS.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                        }
                    }
                    break;
                // 終わらぬ戦い
                case MissionBastok.FETICHISM:
                    if (bastokMission.StatusLower == 0)
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
                                message.missionType = ZoneId.PALBOROUGH_MINES.ToString();
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
                    if (bastokMission.StatusLower == 1)
                    {
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.C_L_REPORT))
                        {
                            message.missionPhase = KeyItemId.C_L_REPORT.ToString();
                            return message;
                        }

                        if (database.HasItem(charaInfo.CharaId, ItemId.FADED_CRYSTAL))
                        {
                            message.missionPhase = ItemId.FADED_CRYSTAL.ToString();
                            return message;
                        }

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
                    if (bastokMission.StatusLower == 0)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.LIZARD_EGG))
                        {
                            message.missionPhase = ItemId.LIZARD_EGG.ToString();
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
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                        }
                    }
                    break;
                // バストゥークを離れて
                case MissionBastok.THE_EMISSARY:
                    if (bastokMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.METALWORKS:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.METALWORKS.ToString();
                                    message.missionPhase = ZoneId.METALWORKS.ToString() + "_1_2";
                                    return message;
                                }
                                break;
                            case ZoneId.BASTOK_MARKETS:
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.METALWORKS.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                        }
                    }
                    else if (bastokMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.NORTHERN_SAN_DORIA:
                                break;
                            case ZoneId.SOUTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
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
                            case ZoneId.KONSCHTAT_HIGHLANDS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.VALKURM_DUNES.ToString();
                                message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                return message;
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
                    else if (bastokMission.StatusLower == 6)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.PORT_WINDURST:
                                break;
                            case ZoneId.EAST_SARUTABARUTA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WINDURST_WOODS.ToString();
                                message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                return message;
                            case ZoneId.TAHRONGI_CANYON:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EAST_SARUTABARUTA.ToString();
                                message.missionPhase = ZoneId.TAHRONGI_CANYON.ToString();
                                return message;
                            case ZoneId.BUBURIMU_PENINSULA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.TAHRONGI_CANYON.ToString();
                                message.missionPhase = ZoneId.BUBURIMU_PENINSULA.ToString();
                                return message;
                            case ZoneId.MHAURA:
                                if (charaInfo.Pos.X > -7.50 && charaInfo.Pos.Y < 38.0)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MHAURA.ToString();
                                    message.missionPhase = ZoneId.MHAURA.ToString();
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BUBURIMU_PENINSULA.ToString();
                                    message.missionPhase = ZoneId.MHAURA.ToString();
                                }
                                return message;
                            case ZoneId.SHIP_BOUND_FOR_MHAURA:
                            case ZoneId.SHIP_BOUND_FOR_MHAURA_PIRATES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MHAURA.ToString();
                                message.missionPhase = ZoneId.SHIP_BOUND_FOR_MHAURA.ToString();
                                return message;
                            case ZoneId.SELBINA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MHAURA.ToString();
                                message.missionPhase = ZoneId.SELBINA.ToString();
                                return message;
                            case ZoneId.VALKURM_DUNES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SELBINA.ToString();
                                message.missionPhase = ZoneId.VALKURM_DUNES.ToString();
                                return message;
                            case ZoneId.LA_THEINE_PLATEAU:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.VALKURM_DUNES.ToString();
                                message.missionPhase = ZoneId.LA_THEINE_PLATEAU.ToString();
                                return message;
                            case ZoneId.WEST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.LA_THEINE_PLATEAU.ToString();
                                message.missionPhase = ZoneId.WEST_RONFAURE.ToString();
                                return message;
                            case ZoneId.SOUTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    break;
                // バストゥークを離れて サンドリア前半ルート
                case MissionBastok.THE_EMISSARY_SANDORIA:
                    if (bastokMission.StatusLower == 3)
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
                    else if (bastokMission.StatusLower == 4)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.GHELSBA_OUTPOST:
                                break;
                            case ZoneId.WEST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GHELSBA_OUTPOST.ToString();
                                message.missionPhase = ZoneId.WEST_RONFAURE.ToString();
                                return message;
                            case ZoneId.NORTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                return message;
                        }
                    }
                    break;
                // バストゥークを離れて ウィンダス後半ルート
                case MissionBastok.THE_EMISSARY_WINDURST2:
                    if (bastokMission.StatusLower == 7)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.HEAVENS_TOWER:
                                break;
                            case ZoneId.WINDURST_WALLS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.HEAVENS_TOWER.ToString();
                                message.missionPhase = ZoneId.WINDURST_WALLS.ToString();
                                return message;
                            case ZoneId.WINDURST_WATERS:
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.WINDURST_WATERS.ToString();
                                    message.missionPhase = ZoneId.WINDURST_WATERS.ToString() + charaInfo.MapId.ToString();
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.WINDURST_WALLS.ToString();
                                    message.missionPhase = ZoneId.WINDURST_WATERS.ToString();
                                }
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WINDURST_WATERS.ToString();
                                message.missionPhase = ZoneId.PORT_WINDURST.ToString();
                                return message;
                        }
                    }
                    else if (bastokMission.StatusLower == 8)
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
                            case ZoneId.WEST_SARUTABARUTA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GIDDEUS.ToString();
                                message.missionPhase = ZoneId.WEST_SARUTABARUTA.ToString();
                                return message;
                            case ZoneId.WINDURST_WATERS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_SARUTABARUTA.ToString();
                                message.missionPhase = ZoneId.WINDURST_WATERS.ToString();
                                return message;
                            case ZoneId.WINDURST_WALLS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WINDURST_WATERS.ToString();
                                message.missionPhase = ZoneId.WINDURST_WALLS.ToString();
                                return message;
                            case ZoneId.HEAVENS_TOWER:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WINDURST_WALLS.ToString();
                                message.missionPhase = ZoneId.HEAVENS_TOWER.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WINDURST_WALLS.ToString();
                                message.missionPhase = ZoneId.WINDURST_WOODS.ToString();
                                return message;
                        }
                    }
                    break;
                // 四銃士
                case MissionBastok.THE_FOUR_MUSKETEERS:
                    if (bastokMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.BEADEAUX:
                                break;
                            case ZoneId.PASHHOW_MARSHLANDS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BEADEAUX.ToString();
                                message.missionPhase = ZoneId.PASHHOW_MARSHLANDS.ToString();
                                return message;
                            case ZoneId.KONSCHTAT_HIGHLANDS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PASHHOW_MARSHLANDS.ToString();
                                message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                return message;
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
                // 忘れられた鉱山にて
                case MissionBastok.TO_THE_FORSAKEN_MINES:
                    if (bastokMission.StatusLower == 0)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.CHUNK_OF_GLOCOLITE))
                        {
                            message.missionPhase = ItemId.CHUNK_OF_GLOCOLITE.ToString();
                            return message;
                        }

                        if (database.HasItem(charaInfo.CharaId, ItemId.HARE_MEAT))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.GUSGEN_MINES:
                                    message.missionPhase += "_HARE_MEAT";
                                    return message;
                                case ZoneId.KONSCHTAT_HIGHLANDS:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.GUSGEN_MINES.ToString();
                                    message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                    message.missionPhase = ZoneId.NORTH_GUSTABERG.ToString();
                                    return message;
                            }
                        }
                        else if (charaInfo.ZoneId != ZoneId.BASTOK_MINES)
                        {
                            message.missionPhase = ItemId.HARE_MEAT.ToString();
                            return message;
                        }
                    }
                    break;
                // ジュノへ
                case MissionBastok.JEUNO:
                    if (bastokMission.StatusLower == 1)
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
                    else if (bastokMission.StatusLower == 2)
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
                                    message.missionPhase += "_3";
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
                            case ZoneId.PORT_JEUNO:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.QUFIM_ISLAND.ToString();
                                message.missionPhase = ZoneId.PORT_JEUNO.ToString();
                                return message;
                            case ZoneId.LOWER_JEUNO:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PORT_JEUNO.ToString();
                                message.missionPhase = ZoneId.LOWER_JEUNO.ToString();
                                return message;
                            case ZoneId.UPPER_JEUNO:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.LOWER_JEUNO.ToString();
                                message.missionPhase = ZoneId.UPPER_JEUNO.ToString();
                                return message;
                            case ZoneId.RULUDE_GARDENS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.UPPER_JEUNO.ToString();
                                message.missionPhase = ZoneId.RULUDE_GARDENS.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.QUFIM_ISLAND.ToString();
                                message.missionPhase = ZoneId.PORT_JEUNO.ToString();
                                return message;
                        }

                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.DELKFUTT_KEY))
                        {
                            message.missionPhase += "_2";
                        }
                        else
                        {
                            // デルクフのカギを持っていない場合は、デルクフのカギを取得する必要がある
                            if (database.HasItem(charaInfo.CharaId, ItemId.DELKFUTT_KEY))
                            {
                                message.missionPhase = ItemId.DELKFUTT_KEY.ToString();
                            }
                        }
                    }
                    break;
                // 魔晶石を奪え
                case MissionBastok.MAGICITE:
                    if (bastokMission.StatusLower == 1)
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
                    if (bastokMission.StatusLower == 2)
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
                                case ZoneId.BATALLIA_DOWNS:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.JUGNER_FOREST.ToString();
                                    message.missionPhase = ZoneId.BATALLIA_DOWNS.ToString();
                                    return message;
                                case ZoneId.UPPER_JEUNO:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BATALLIA_DOWNS.ToString();
                                    message.missionPhase = ZoneId.UPPER_JEUNO.ToString();
                                    return message;
                                default:
                                    break;
                            }
                            message.missionPhase = "2_DAVOI";
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
                            message.missionPhase = "2_BEADEAUX";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.MAGICITE_ORASTONE))
                        {
                            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.YAGUDO_TORCH))
                            {
                                var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.BASTOK}][{bastokMission.Current}]Option");
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
                                case ZoneId.MERIPHATAUD_MOUNTAINS:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.MERIPHATAUD_MOUNTAINS.ToString();
                                    return message;
                                case ZoneId.SAUROMUGUE_CHAMPAIGN:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MERIPHATAUD_MOUNTAINS.ToString();
                                    message.missionPhase = ZoneId.SAUROMUGUE_CHAMPAIGN.ToString();
                                    return message;
                                case ZoneId.PORT_JEUNO:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.SAUROMUGUE_CHAMPAIGN.ToString();
                                    message.missionPhase = ZoneId.PORT_JEUNO.ToString();
                                    return message;
                                case ZoneId.LOWER_JEUNO:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.PORT_JEUNO.ToString();
                                    message.missionPhase = ZoneId.LOWER_JEUNO.ToString();
                                    return message;
                                default:
                                    break;
                            }
                            message.missionPhase = "2_CASTLE_OZTROJA";
                            return message;
                        }
                    }
                    break;
                // 闇、再び
                case MissionBastok.DARKNESS_RISING:
                    if (bastokMission.StatusLower == 10)
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
                            case ZoneId.WEST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EAST_RONFAURE.ToString();
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
                            case ZoneId.KONSCHTAT_HIGHLANDS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.VALKURM_DUNES.ToString();
                                message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                return message;
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
                            case ZoneId.BASTOK_MARKETS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                            case ZoneId.METALWORKS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BASTOK_MARKETS.ToString();
                                message.missionPhase = ZoneId.METALWORKS.ToString();
                                return message;
                            default:
                                break;
                        }
                    }
                    else if (bastokMission.StatusLower == 11)
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
                // ザルカバードに眠る真実
                case MissionBastok.XARCABARD_LAND_OF_TRUTHS:
                    if (QuestMission.GetLevelCapMessage(database, charaInfo, 60, ref message))
                    {
                        return message;
                    }
                    if (bastokMission.StatusLower == 2)
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
                            case ZoneId.BEAUCEDINE_GLACIER:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.XARCABARD.ToString();
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
                            case ZoneId.WEST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EAST_RONFAURE.ToString();
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
                            case ZoneId.KONSCHTAT_HIGHLANDS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.VALKURM_DUNES.ToString();
                                message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                return message;
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
                            case ZoneId.BASTOK_MARKETS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                            case ZoneId.METALWORKS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BASTOK_MARKETS.ToString();
                                message.missionPhase = ZoneId.METALWORKS.ToString();
                                return message;
                            default:
                                break;
                        }
                    }
                    break;
                // 語り部現る！？
                case MissionBastok.RETURN_OF_THE_TALEKEEPER:
                    if (bastokMission.StatusLower == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ZERUHN_MINES:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ZERUHN_MINES.ToString();
                                message.missionPhase = ZoneId.BASTOK_MINES.ToString();
                                return message;
                        }
                    }
                    else if (bastokMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.WESTERN_ALTEPA_DESERT:
                                break;
                            case ZoneId.EASTERN_ALTEPA_DESERT:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WESTERN_ALTEPA_DESERT.ToString();
                                message.missionPhase = ZoneId.EASTERN_ALTEPA_DESERT.ToString();
                                return message;
                            case ZoneId.KORROLOKA_TUNNEL:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.KORROLOKA_TUNNEL.ToString();
                                    message.missionPhase = ZoneId.KORROLOKA_TUNNEL.ToString() + "_1_2";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EASTERN_ALTEPA_DESERT.ToString();
                                message.missionPhase = ZoneId.KORROLOKA_TUNNEL.ToString();
                                return message;
                            case ZoneId.ZERUHN_MINES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.KORROLOKA_TUNNEL.ToString();
                                message.missionPhase = ZoneId.ZERUHN_MINES.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ZERUHN_MINES.ToString();
                                message.missionPhase = ZoneId.BASTOK_MINES.ToString();
                                return message;
                        }
                    }
                    break;
                // 海賊たちの唄
                case MissionBastok.THE_PIRATES_COVE:
                    if (bastokMission.StatusLower == 2)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.FRAG_ROCK))
                        {
                            message.missionPhase = ItemId.FRAG_ROCK.ToString();
                            return message;
                        }

                        if (!database.HasItem(charaInfo.CharaId, ItemId.CHUNK_OF_ADAMAN_ORE))
                        {
                            message.missionPhase = ItemId.CHUNK_OF_ADAMAN_ORE.ToString();
                            return message;
                        }

                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.IFRITS_CAULDRON:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.IFRITS_CAULDRON.ToString();
                                message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString() + "2";
                                return message;
                        }
                    }
                    break;
                // 完成品のイメージ
                case MissionBastok.THE_FINAL_IMAGE:
                    break;
                // それぞれの行方
                case MissionBastok.ON_MY_WAY:
                    break;
                // 流砂の鎖
                case MissionBastok.THE_CHAINS_THAT_BIND_US:
                    if (bastokMission.StatusLower == 1)
                    {
                        if (charaInfo.ZoneId == ZoneId.QUICKSAND_CAVES && charaInfo.MapId == 5)
                        {
                            break;
                        }
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                        message.missionPhase = ZoneId.WESTERN_ALTEPA_DESERT.ToString() + "_3";
                        return message;
                    }
                    else if (bastokMission.StatusLower == 3)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.QUICKSAND_CAVES:
                                if (charaInfo.MapId == 4)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                                    message.missionPhase = ZoneId.QUICKSAND_CAVES.ToString() + "_4_7";
                                    return message;
                                }
                                if (charaInfo.MapId == 7)
                                {
                                    break;
                                }
                                else
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                                    message.missionPhase = ZoneId.WESTERN_ALTEPA_DESERT.ToString() + "_2";
                                    return message;
                                }
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                                message.missionPhase = ZoneId.WESTERN_ALTEPA_DESERT.ToString() + "_2";
                                return message;
                        }
                    }
                    break;
                // その記憶を紡ぐ者
                case MissionBastok.ENTER_THE_TALEKEEPER:
                    break;
                // 最後の幻想
                case MissionBastok.THE_SALT_OF_THE_EARTH:
                    if (bastokMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.GUSTAV_TUNNEL:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.GUSTAV_TUNNEL.ToString();
                                    message.missionPhase = ZoneId.GUSTAV_TUNNEL.ToString() + "_1_2";
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GUSTAV_TUNNEL.ToString();
                                message.missionPhase = ZoneId.VALKURM_DUNES.ToString();
                                return message;
                        }
                    }
                    else if (bastokMission.StatusLower == 3)
                    {
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.MIRACLESALT))
                        {
                            message.missionPhase = KeyItemId.MIRACLESALT.ToString();
                            return message;
                        }
                    }
                    break;
                // 双刃の邂逅
                case MissionBastok.WHERE_TWO_PATHS_CONVERGE:
                    break;
                // ミッション未受託
                case MissionBastok.NONE:
                    if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.THE_ZERUHN_REPORT))
                    {
                        message.missionPhase = MissionBastok.THE_ZERUHN_REPORT.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.GEOLOGICAL_SURVEY))
                    {
                        message.missionPhase = MissionBastok.GEOLOGICAL_SURVEY.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.FETICHISM))
                    {
                        message.missionPhase = MissionBastok.FETICHISM.ToString();
                    }
                    else if (QuestMission.GetSupportJobMessage(database, charaInfo, ref message))
                    {
                        return message;
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.THE_CRYSTAL_LINE))
                    {
                        message.missionPhase = MissionBastok.THE_CRYSTAL_LINE.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.WADING_BEASTS))
                    {
                        message.missionPhase = MissionBastok.WADING_BEASTS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.THE_EMISSARY))
                    {
                        message.missionPhase = MissionBastok.THE_EMISSARY.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.THE_FOUR_MUSKETEERS))
                    {
                        message.missionPhase = MissionBastok.THE_FOUR_MUSKETEERS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.TO_THE_FORSAKEN_MINES))
                    {
                        message.missionPhase = MissionBastok.TO_THE_FORSAKEN_MINES.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.JEUNO))
                    {
                        message.missionPhase = MissionBastok.JEUNO.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.MAGICITE))
                    {
                        message.missionPhase = MissionBastok.MAGICITE.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.DARKNESS_RISING))
                    {
                        message.missionPhase = MissionBastok.DARKNESS_RISING.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.XARCABARD_LAND_OF_TRUTHS))
                    {
                        message.missionPhase = MissionBastok.XARCABARD_LAND_OF_TRUTHS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.RETURN_OF_THE_TALEKEEPER))
                    {
                        message.missionPhase = MissionBastok.RETURN_OF_THE_TALEKEEPER.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.THE_PIRATES_COVE))
                    {
                        message.missionPhase = MissionBastok.THE_PIRATES_COVE.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.THE_FINAL_IMAGE))
                    {
                        message.missionPhase = MissionBastok.THE_FINAL_IMAGE.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.ON_MY_WAY))
                    {
                        message.missionPhase = MissionBastok.ON_MY_WAY.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.THE_CHAINS_THAT_BIND_US))
                    {
                        var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.BASTOK}][{(int)MissionBastok.ON_MY_WAY}]GumbahDialog");
                        if (value == 0)
                        {
                            message.missionType = MissionBastok.ON_MY_WAY.ToString();
                            message.missionPhase = "GumbahDialog";
                        }
                        else if (value == 1)
                        {
                            message.missionType = MissionBastok.ON_MY_WAY.ToString();
                            message.missionPhase = "GumbahDialog1";
                        }
                        else
                        {
                            message.missionPhase = MissionBastok.THE_CHAINS_THAT_BIND_US.ToString();
                        }
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.ENTER_THE_TALEKEEPER))
                    {
                        message.missionPhase = MissionBastok.ENTER_THE_TALEKEEPER.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.THE_SALT_OF_THE_EARTH))
                    {
                        message.missionPhase = MissionBastok.THE_SALT_OF_THE_EARTH.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.BASTOK, (int)MissionBastok.WHERE_TWO_PATHS_CONVERGE))
                    {
                        message.missionPhase = MissionBastok.WHERE_TWO_PATHS_CONVERGE.ToString();
                    }
                    break;
                default:
                    break;
            }

            return message;
        }
    }
}
