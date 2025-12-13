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
    }
}
﻿