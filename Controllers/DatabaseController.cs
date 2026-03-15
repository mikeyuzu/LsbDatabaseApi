using Google.Protobuf.WellKnownTypes;
using LsbDatabaseApi.@struct;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Policy;
using static LsbDatabaseApi.DatabaseApi;

namespace LsbDatabaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Database(IConfiguration configuration, ILogger<Database> logger) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<Database> _logger = logger;

        /// <summary>
        /// カスタムメニューの情報を返す
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="zoneId"></param>
        /// <param name="mapId"></param>
        /// <param name="coordinates"></param>
        /// <param name="preZoneId"></param>
        /// <param name="preMapId"></param>
        /// <param name="preCoordinates"></param>
        /// <param name="clip1Category"></param>
        /// <param name="clip1Id"></param>
        /// <param name="clip2Category"></param>
        /// <param name="clip2Id"></param>
        /// <param name="clip3Category"></param>
        /// <param name="clip3Id"></param>
        /// <param name="clip4Category"></param>
        /// <param name="clip4Id"></param>
        /// <returns></returns>
        [HttpGet("GetCustomMenuInfo")]
        public ActionResult<CustomMenuInfo> GetCustomMenuInfo(int charaId, int zoneId, int mapId, string coordinates, int preZoneId, int preMapId, string preCoordinates, int clip1Category, int clip1Id, int clip2Category, int clip2Id, int clip3Category, int clip3Id, int clip4Category, int clip4Id)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            var customMenuInfo = new CustomMenuInfo();

            // ナビゲーションメッセージ
            database.LoadChars(charaId);
            database.LoadVariables(charaId);
            database.LoadTeleportInfo(charaId);
            database.LoadProfile(charaId);

            var messageParam = new MessageParam();
            var message = messageParam.GetMessageParam(database, charaId, zoneId, mapId, coordinates, preZoneId, preMapId, preCoordinates);
            customMenuInfo.MainNaviMessage = message;

            // クリップ1
            if ((ClipCategory)clip1Category != ClipCategory.NONE && clip1Id >= 0)
            {
                // クリップ処理
            }

            // クリップ2
            if ((ClipCategory)clip2Category != ClipCategory.NONE && clip2Id >= 0)
            {
                // クリップ処理
            }

            // クリップ3
            if ((ClipCategory)clip3Category != ClipCategory.NONE && clip3Id >= 0)
            {
                // クリップ処理
            }

            // クリップ4
            if ((ClipCategory)clip4Category != ClipCategory.NONE && clip4Id >= 0)
            {
                // クリップ処理
            }

            // ビックリマーク
            if (database.IsExclamationMark(charaId))
            {
                customMenuInfo.ExclamationMark = 1;
            }

            return Ok(customMenuInfo);
        }

        /// <summary>
        /// 合成倉庫のアイテムを返す
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        [HttpGet("GetSynergyInventoryItems")]
        public ActionResult<List<DatabaseApi.SynergyInventoryItem>> GetSynergyInventoryItems(int charaId)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            var items = database.GetSynergyInventoryItems(charaId);

            return Ok(items);
        }

        /// <summary>
        /// 合成倉庫のアイテムを削除する
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="itemId"></param>
        /// <param name="subId"></param>
        /// <param name="usenum"></param>
        /// <returns></returns>
        [HttpGet("RemoveSynergyInventoryItem")]
        public ActionResult RemoveSynergyInventoryItem(int charaId, int itemId, int subId, int usenum)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            // ポストに入れる
            var extra = "000000000000000000000000000000000000000000000000";
            database.InsertDeliveryBoxItem(charaId, itemId, subId, usenum, extra);

            // データベースを更新
            database.UpdateCustomInventory(charaId, (ItemId)itemId, usenum);

            return Ok();
        }

        /// <summary>
        /// 合成レシピを返す
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        [HttpGet("GetSynthesisRecipes")]
        public ActionResult<List<DatabaseApi.SynergyInventoryItem>> GetSynthesisRecipes(int charaId, int guildId, int rank)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            var recipes = database.GetSynthesisRecipes(charaId, (GuildId)guildId, (CraftRank)rank);

            return Ok(recipes);
        }

        /// <summary>
        /// アイテム別合成レシピを返す
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        [HttpGet("GetSynthesisRecipesByItem")]
        public ActionResult<List<DatabaseApi.SynergyInventoryItem>> GetSynthesisRecipesByItem(int charaId, int auctionHouseId, int minLevel)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            var recipes = database.GetSynthesisRecipesByItem(charaId, (AuctionHouseId)auctionHouseId, minLevel);

            return Ok(recipes);
        }

        /// <summary>
        /// 合成レシピを開放する
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("OpenRecipes")]
        public ActionResult OpenRecipes(int charaId, int id)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            database.InsertOpenRecipe(charaId, id);

            return Ok();
        }

        /// <summary>
        /// 合成結果情報
        /// </summary>
        public struct SynergyResult
        {
            public int StorageType { get; set; }    // 格納先 0:素材倉庫, 1:ポスト
            public int SkillId { get; set; }        // スキルID
            public int SkillLevel { get; set; }     // スキルレベル
        }

        /// <summary>
        /// アイテムを合成する
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="recipeId"></param>
        /// <param name="itemId"></param>
        /// <param name="subId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpGet("SynthesizeItem")]
        public ActionResult<SynergyResult> SynthesizeItem(int charaId, int skillId, int recipeId, int itemId, int subId, int quantity)
        {
            SynergyResult synergy_result = new();

            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);
            var recipeInfo = database.GetCachedSynthesisRecipe(recipeId);
            if (recipeInfo != null)
            {
                var isIngredient = database.CheckIngredientItem(itemId);

                if (isIngredient)
                {
                    // 素材倉庫に入れる
                    database.InsertCustomInventory(charaId, itemId, quantity);
                    synergy_result.StorageType = 0;
                }
                else
                {
                    // ポストに入れる
                    var extra = "000000000000000000000000000000000000000000000000";
                    database.InsertDeliveryBoxItem(charaId, itemId, subId, quantity, extra);
                    synergy_result.StorageType = 1;
                }

                // 合成倉庫から削除する (消費アイテム)
                // クリスタル
                database.UpdateCustomInventory(charaId, (ItemId)recipeInfo.Value.Crystal.ItemId, recipeInfo.Value.Crystal.Quantity);
                // 素材
                foreach (var ingredient in recipeInfo.Value.Ingredient)
                {
                    if (ingredient.ItemId > 0)
                    {
                        database.UpdateCustomInventory(charaId, (ItemId)ingredient.ItemId, ingredient.Quantity);
                    }
                }

                synergy_result.SkillId = skillId;
                var addSkillPoint = 10;
                synergy_result.SkillLevel = database.UpdateCharaSkill(charaId, (SkillId)synergy_result.SkillId, addSkillPoint);
            }

            return Ok(synergy_result);
        }

        /// <summary>
        /// 図鑑のリスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        [HttpGet("GetCollectionList")]
        public ActionResult<List<int>> GetCollectionList(int charaId)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            var list = database.GetCollectionList(charaId);

            return Ok(list);
        }

        /// <summary>
        /// ミッションのリスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        [HttpGet("GetMissionList")]
        public ActionResult<MissionClearInfo> GetMissionList(int charaId)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            var list = database.GetMissionList(charaId);

            return Ok(list);
        }

        /// <summary>
        /// エミネンス・レコードのリスト取得
        /// </summary>
        /// <param name="charaId"></param>
        /// <returns></returns>
        [HttpGet("GetEminenceRecordList")]
        public ActionResult<EminenceRecord> GetEminenceRecordList(int charaId)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);
            var list = database.GetEminenceRecordList(charaId);
            return Ok(list);
        }

        /// <summary>
        /// エミネンス・レコードの報酬を受け取る
        /// </summary>
        /// <param name="charaId"></param>
        /// <param name="category"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpGet("ReceiveEminenceRecordReward")]
        public ActionResult<EminenceRecordRewardDestination> ReceiveEminenceRecordReward(int charaId, int category, int item)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            // 報酬を処理する
            var result = EminenceRecordRewardProcessor.ProcessReward(database, charaId, (EminenceRecordCategory)category, item);
            // データベースを更新する
            database.ReceiveEminenceRecordReward(charaId, (EminenceRecordCategory)category, item);

            return Ok(result);
        }
    }
}
﻿