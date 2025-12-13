namespace LsbDatabaseApi
{
    class VanaDay
    {
        // ヴァナ・ディールの基準時刻（地球時間：2002年1月1日 00:00:00 JST）
        private static readonly DateTime VanaEpoch = new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddHours(-9);

        // 地球時間1日 = ヴァナ時間25日
        // 地球時間1時間 = ヴァナ時間25時間
        // ヴァナ時間1日 = 地球時間57分36秒 = 3456秒
        private const double VanaDayInSeconds = 3456.0;

        public static int GetVanaDayOfWeek()
        {
            // 現在時刻を取得
            DateTime now = DateTime.UtcNow;

            // 基準時刻からの経過秒数を計算
            TimeSpan elapsed = now - VanaEpoch;
            double elapsedSeconds = elapsed.TotalSeconds;

            // ヴァナ・ディールの経過日数を計算
            double vanaDays = elapsedSeconds / VanaDayInSeconds;

            // 曜日を計算（0-7）
            // ヴァナ・ディールは8曜日制
            int dayOfWeek = (int)Math.Floor(vanaDays) % 8;

            return dayOfWeek;
        }
    }
}
