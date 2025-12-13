
using LsbDatabaseApi.@struct;

namespace LsbDatabaseApi
{
    /// <summary>
    /// キャラクター情報
    /// </summary>
    /// <param name="charaId"></param>
    /// <param name="zoneId"></param>
    /// <param name="mapId"></param>
    /// <param name="coordinates"></param>
    /// <param name="preZoneId"></param>
    /// <param name="preMapId"></param>
    /// <param name="preCoordinates"></param>
    internal class CharaInfo(int charaId, int zoneId, int mapId, string coordinates, int preZoneId, int preMapId, string preCoordinates)
    {
        public int CharaId = charaId;
        public ZoneId ZoneId = (ZoneId)zoneId;
        public int MapId = mapId;
        public string Coordinates = coordinates;
        public ZoneId PreZoneId = (ZoneId)preZoneId;
        public int PreMapId = preMapId;
        public string PreCoordinates = preCoordinates;
    }
}
