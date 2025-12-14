using LsbDatabaseApi.@struct;
using Microsoft.AspNetCore.Mvc;

namespace LsbDatabaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Database(IConfiguration configuration, ILogger<Database> logger) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<Database> _logger = logger;

        /// <summary>
        /// ナビゲーションメッセージを返す
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMessage")]
        public ActionResult<string> GetNaviMessage(int charaId, int zoneId, int mapId, string coordinates, int preZoneId, int preMapId, string preCoordinates)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            database.LoadChars(charaId);
            database.LoadVariables(charaId);
            database.LoadTeleportInfo(charaId);
            database.LoadProfile(charaId);

            var messageParam = new MessageParam();
            var message = messageParam.GetMessageParam(database, charaId, zoneId, mapId, coordinates, preZoneId, preMapId, preCoordinates);

            return Ok(message);
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

        // 合成倉庫から取り出す
        [HttpGet("RemoveSynergyInventoryItem")]
        public ActionResult RemoveSynergyInventoryItem(int charaId, int itemId, int subId, int usenum, int quantity)
        {
            string? connectionString = _configuration.GetConnectionString("LandSandBoat");
            var database = new DatabaseApi();
            database.DatabaseInitialize(connectionString);

            // ポストに入れる
            var extra = "000000000000000000000000000000000000000000000000";
            database.InsertDeliveryBoxItem(charaId, itemId, subId, usenum, extra);

            // データベースを更新
            if (quantity <= usenum)
            {
                // 在庫が0以下になったらレコードを削除
                database.DeleteCustomInventory(charaId, (ItemId)itemId);
            }
            else
            {
                // 在庫が残っている場合はレコードを更新
                database.UpdateCustomInventory(charaId, (ItemId)itemId, usenum);
            }

            return Ok();
        }
    }
}
﻿