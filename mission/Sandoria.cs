using LsbDatabaseApi.@struct;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class SandoriaMission
    {
        public static MessageParam GetMessageSandoria(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();

            var sandoriaMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.SANDORIA);
            message.missionKind = MissionKind.MissionSandoria;
            message.missionType = ((MissionSandoria)sandoriaMission.Current).ToString();
            message.missionPhase = sandoriaMission.StatusLower.ToString();

            switch ((MissionSandoria)sandoriaMission.Current)
            {
                // オークの斥候を倒せ
                case MissionSandoria.SMASH_THE_ORCISH_SCOUTS:
                    if (sandoriaMission.StatusLower == 0)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.ORCISH_AXE))
                        {
                            message.missionPhase = ItemId.ORCISH_AXE.ToString();
                            return message;
                        }
                    }
                    break;
                // コウモリ退治
                case MissionSandoria.BAT_HUNT:
                    if (sandoriaMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.KING_RANPERRES_TOMB:
                                break;
                            case ZoneId.EAST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.KING_RANPERRES_TOMB.ToString();
                                message.missionPhase = ZoneId.EAST_RONFAURE.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EAST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    if (sandoriaMission.StatusLower == 2)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.ORCISH_MAIL_SCALES))
                        {
                            message.missionPhase = ItemId.ORCISH_MAIL_SCALES.ToString();
                            return message;
                        }

                    }
                    break;
                // 子供の救助
                case MissionSandoria.SAVE_THE_CHILDREN:
                    if (sandoriaMission.StatusLower == 2)
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
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    break;
                // 救助訓練
                case MissionSandoria.THE_RESCUE_DRILL:
                    if (sandoriaMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.LA_THEINE_PLATEAU:
                                break;
                            case ZoneId.WEST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.LA_THEINE_PLATEAU.ToString();
                                message.missionPhase = ZoneId.WEST_RONFAURE.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    if (sandoriaMission.StatusLower == 5)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ORDELLES_CAVES:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ORDELLES_CAVES.ToString();
                                message.missionPhase = ZoneId.LA_THEINE_PLATEAU.ToString();
                                return message;
                        }
                    }
                    if (sandoriaMission.StatusLower == 8)
                    {
                        var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.SANDORIA}][{sandoriaMission.Current}]Option");
                        message.missionPhase += $"Option{value}";
                    }
                    break;
                // ダボイ調査報告
                case MissionSandoria.THE_DAVOI_REPORT:
                    if (sandoriaMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.DAVOI:
                                break;
                            case ZoneId.JUGNER_FOREST:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.DAVOI.ToString();
                                message.missionPhase = ZoneId.JUGNER_FOREST.ToString();
                                return message;
                            case ZoneId.LA_THEINE_PLATEAU:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.JUGNER_FOREST.ToString();
                                message.missionPhase = ZoneId.LA_THEINE_PLATEAU.ToString();
                                return message;
                            case ZoneId.WEST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.LA_THEINE_PLATEAU.ToString();
                                message.missionPhase = ZoneId.WEST_RONFAURE.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    break;
                // 他国を回れ
                case MissionSandoria.JOURNEY_ABROAD:
                    if (sandoriaMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CHATEAU_DORAGUILLE:
                                break;
                            case ZoneId.NORTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    else if (sandoriaMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CHATEAU_DORAGUILLE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                return message;
                            default:
                                break;
                        }
                        if (database.HasItem(charaInfo.CharaId, ItemId.CIPHER_OF_HALVERS_ALTER_EGO))
                        {
                            message.missionKind = MissionKind.Trust;
                            message.missionType = "CIPHER";
                            message.missionPhase = ItemId.CIPHER_OF_HALVERS_ALTER_EGO.ToString();
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.METALWORKS:
                            case ZoneId.BASTOK_MARKETS:
                            case ZoneId.PORT_BASTOK:
                            case ZoneId.BASTOK_MINES:
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
                            case ZoneId.VALKURM_DUNES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
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
                            case ZoneId.NORTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                            case ZoneId.CHATEAU_DORAGUILLE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                return message;
                            default:
                                break;
                        }
                        if (QuestMission.GetTrustBastokMessage(database, charaInfo, ref message))
                        {
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.METALWORKS:
                                break;
                            case ZoneId.BASTOK_MARKETS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.METALWORKS.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                                return message;
                            case ZoneId.PORT_BASTOK:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BASTOK_MARKETS.ToString();
                                message.missionPhase = ZoneId.PORT_BASTOK.ToString();
                                return message;
                            default:
                                break;
                        }
                    }
                    else if (sandoriaMission.StatusLower == 6)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.WINDURST_WOODS:
                            case ZoneId.PORT_WINDURST:
                            case ZoneId.WINDURST_WATERS:
                            case ZoneId.WINDURST_WALLS:
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
                            case ZoneId.KONSCHTAT_HIGHLANDS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.VALKURM_DUNES.ToString();
                                message.missionPhase = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                return message;
                            case ZoneId.NORTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.KONSCHTAT_HIGHLANDS.ToString();
                                message.missionPhase = ZoneId.NORTH_GUSTABERG.ToString();
                                return message;
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
                        if (QuestMission.GetTrustWindurstMessage(database, charaInfo, ref message))
                        {
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.WINDURST_WOODS:
                                break;
                            case ZoneId.WINDURST_WALLS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WINDURST_WOODS.ToString();
                                message.missionPhase = ZoneId.WINDURST_WALLS.ToString();
                                return message;
                            case ZoneId.HEAVENS_TOWER:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WINDURST_WALLS.ToString();
                                message.missionPhase = ZoneId.HEAVENS_TOWER.ToString();
                                return message;
                            default:
                                break;
                        }
                    }
                    break;
                // 他国を回れ バストゥーク前半ルート
                case MissionSandoria.JOURNEY_TO_BASTOK:
                    if (sandoriaMission.StatusLower == 5)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.ONZ_OF_MYTHRIL_SAND))
                        {
                            message.missionPhase = ItemId.ONZ_OF_MYTHRIL_SAND.ToString();
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.PALBOROUGH_MINES:
                                if (database.HasItem(charaInfo.CharaId, ItemId.CHUNK_OF_MINE_GRAVEL))
                                {
                                    if (charaInfo.MapId == 1)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PALBOROUGH_MINES.ToString();
                                        message.missionPhase = ZoneId.PALBOROUGH_MINES.ToString() + "_1_3";
                                        return message;
                                    }
                                    else if (charaInfo.MapId == 3)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PALBOROUGH_MINES.ToString();
                                        message.missionPhase = ZoneId.PALBOROUGH_MINES.ToString() + "_3_2";
                                        return message;
                                    }
                                    message.missionPhase = ItemId.CHUNK_OF_MINE_GRAVEL.ToString();
                                    return message;
                                }
                                else
                                {
                                    var inputValue = database.GetVarNum(charaInfo.CharaId, "refiner_input");
                                    if (inputValue == 1)
                                    {
                                        message.missionPhase = "REFINER_INPUT";
                                        return message;
                                    }
                                    var outputValue = database.GetVarNum(charaInfo.CharaId, "refiner_output");
                                    if (outputValue == 1)
                                    {
                                        message.missionPhase = "REFINER_OUTPUT";
                                        return message;
                                    }
                                }
                                break;
                            case ZoneId.NORTH_GUSTABERG:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PALBOROUGH_MINES.ToString();
                                message.missionPhase = ZoneId.NORTH_GUSTABERG.ToString();
                                return message;
                            case ZoneId.PORT_BASTOK:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTH_GUSTABERG.ToString();
                                message.missionPhase = ZoneId.PORT_BASTOK.ToString();
                                return message;
                            case ZoneId.BASTOK_MARKETS:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PORT_BASTOK.ToString();
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
                // 他国を回れ ウィンダス前半ルート
                case MissionSandoria.JOURNEY_TO_WINDURST:
                    break;
                // 他国を回れ ウィンダス後半ルート
                case MissionSandoria.JOURNEY_TO_BASTOK2:
                    break;
                // 他国を回れ バストゥーク後半ルート
                case MissionSandoria.JOURNEY_TO_WINDURST2:
                    if (sandoriaMission.StatusLower == 7)
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
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WINDURST_WALLS.ToString();
                                message.missionPhase = ZoneId.WINDURST_WOODS.ToString();
                                return message;
                        }
                    }
                    else if (sandoriaMission.StatusLower == 8)
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
                // ダボイ潜入計画
                case MissionSandoria.INFILTRATE_DAVOI:
                    if (sandoriaMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CHATEAU_DORAGUILLE:
                                break;
                            case ZoneId.NORTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    else if (sandoriaMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.DAVOI:
                                break;
                            case ZoneId.JUGNER_FOREST:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.DAVOI.ToString();
                                message.missionPhase = ZoneId.JUGNER_FOREST.ToString();
                                return message;
                            case ZoneId.LA_THEINE_PLATEAU:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.JUGNER_FOREST.ToString();
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
                            case ZoneId.NORTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                            case ZoneId.CHATEAU_DORAGUILLE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                return message;
                            default:
                                break;
                        }
                    }
                    break;
                // クリスタルの泉
                case MissionSandoria.THE_CRYSTAL_SPRING:
                    if (sandoriaMission.StatusLower == 0)
                    {
                        if (database.HasItem(charaInfo.CharaId, ItemId.CRYSTAL_BASS))
                        {
                            message.missionPhase = ItemId.CRYSTAL_BASS.ToString();
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.JUGNER_FOREST:
                                break;
                            case ZoneId.LA_THEINE_PLATEAU:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.JUGNER_FOREST.ToString();
                                message.missionPhase = ZoneId.LA_THEINE_PLATEAU.ToString();
                                return message;
                            case ZoneId.WEST_RONFAURE:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.LA_THEINE_PLATEAU.ToString();
                                message.missionPhase = ZoneId.WEST_RONFAURE.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_RONFAURE.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    else if (sandoriaMission.StatusLower == 2)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CHATEAU_DORAGUILLE:
                                break;
                            case ZoneId.NORTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    break;
                // ジュノ大使館へ赴任
                case MissionSandoria.APPOINTMENT_TO_JEUNO:
                    if (sandoriaMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CHATEAU_DORAGUILLE:
                                break;
                            case ZoneId.NORTHERN_SAN_DORIA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CHATEAU_DORAGUILLE.ToString();
                                message.missionPhase = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                                message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                                return message;
                        }
                    }
                    else if (sandoriaMission.StatusLower == 3)
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
                    else if (sandoriaMission.StatusLower == 4)
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
                case MissionSandoria.MAGICITE:
                    if (sandoriaMission.StatusLower == 2)
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
                    if (sandoriaMission.StatusLower == 3)
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
                                var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.SANDORIA}][{sandoriaMission.Current}]Option");
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
                            message.missionPhase = "3_CASTLE_OZTROJA";
                            return message;
                        }
                    }
                    break;
                // 廃墟フェ・イン
                case MissionSandoria.THE_RUINS_OF_FEI_YIN:
                    if (sandoriaMission.StatusLower == 10)
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
                    else if (sandoriaMission.StatusLower == 11)
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
                // 闇の王を討て！
                case MissionSandoria.THE_SHADOW_LORD:
                    break;
                // ローテ王妃の遺言
                case MissionSandoria.LEAUTES_LAST_WISHES:
                    break;
                // 龍王の眠る場所
                case MissionSandoria.RANPERRES_FINAL_REST:
                    break;
                // 教皇の威信
                case MissionSandoria.PRESTIGE_OF_THE_PAPSQUE:
                    break;
                // 獣人兵器の秘密
                case MissionSandoria.THE_SECRET_WEAPON:
                    break;
                // 成人の儀
                case MissionSandoria.COMING_OF_AGE:
                    break;
                // 聖剣探索
                case MissionSandoria.LIGHTBRINGER:
                    break;
                // 厚き壁
                case MissionSandoria.BREAKING_BARRIERS:
                    break;
                // 光の継承者
                case MissionSandoria.THE_HEIR_TO_THE_LIGHT:
                    break;
                // ミッション未受託
                case MissionSandoria.NONE:
                    if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.SMASH_THE_ORCISH_SCOUTS))
                    {
                        message.missionPhase = MissionSandoria.SMASH_THE_ORCISH_SCOUTS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.BAT_HUNT))
                    {
                        message.missionPhase = MissionSandoria.BAT_HUNT.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.SAVE_THE_CHILDREN))
                    {
                        message.missionPhase = MissionSandoria.SAVE_THE_CHILDREN.ToString();
                    }
                    else if (QuestMission.GetSupportJobMessage(database, charaInfo, ref message))
                    {
                        return message;
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.THE_RESCUE_DRILL))
                    {
                        var value = database.GetVarNum(charaInfo.CharaId, $"Mission[{(int)MissionId.SANDORIA}][{MissionSandoria.SAVE_THE_CHILDREN}]Option");
                        if (value != 0)
                        {
                            message.missionType = MissionSandoria.SAVE_THE_CHILDREN.ToString();
                            message.missionPhase = $"Option{value}";
                        }
                        message.missionPhase = MissionSandoria.THE_RESCUE_DRILL.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.THE_DAVOI_REPORT))
                    {
                        message.missionPhase = MissionSandoria.THE_DAVOI_REPORT.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.JOURNEY_ABROAD))
                    {
                        message.missionPhase = MissionSandoria.JOURNEY_ABROAD.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.INFILTRATE_DAVOI))
                    {
                        message.missionPhase = MissionSandoria.INFILTRATE_DAVOI.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.THE_CRYSTAL_SPRING))
                    {
                        message.missionPhase = MissionSandoria.THE_CRYSTAL_SPRING.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.APPOINTMENT_TO_JEUNO))
                    {
                        message.missionPhase = MissionSandoria.APPOINTMENT_TO_JEUNO.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.MAGICITE))
                    {
                        message.missionPhase = MissionSandoria.MAGICITE.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.THE_RUINS_OF_FEI_YIN))
                    {
                        if (sandoriaMission.StatusLower == 8)
                        {
                            message.missionPhase = MissionSandoria.THE_RUINS_OF_FEI_YIN.ToString() + "_8";
                        }
                        else
                        {
                            message.missionPhase = MissionSandoria.THE_RUINS_OF_FEI_YIN.ToString();
                        }
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.THE_SHADOW_LORD))
                    {
                        message.missionPhase = MissionSandoria.THE_SHADOW_LORD.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.LEAUTES_LAST_WISHES))
                    {
                        message.missionPhase = MissionSandoria.LEAUTES_LAST_WISHES.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.RANPERRES_FINAL_REST))
                    {
                        message.missionPhase = MissionSandoria.RANPERRES_FINAL_REST.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.PRESTIGE_OF_THE_PAPSQUE))
                    {
                        message.missionPhase = MissionSandoria.PRESTIGE_OF_THE_PAPSQUE.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.THE_SECRET_WEAPON))
                    {
                        message.missionPhase = MissionSandoria.THE_SECRET_WEAPON.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.COMING_OF_AGE))
                    {
                        message.missionPhase = MissionSandoria.COMING_OF_AGE.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.LIGHTBRINGER))
                    {
                        message.missionPhase = MissionSandoria.LIGHTBRINGER.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.BREAKING_BARRIERS))
                    {
                        message.missionPhase = MissionSandoria.BREAKING_BARRIERS.ToString();
                    }
                    else if (!database.HasMissionComplete(charaInfo.CharaId, MissionId.SANDORIA, (int)MissionSandoria.THE_HEIR_TO_THE_LIGHT))
                    {
                        message.missionPhase = MissionSandoria.THE_HEIR_TO_THE_LIGHT.ToString();
                    }
                    break;
                default:
                    break;
            }
            return message;
        }
    }
}
