# LsbDatabaseApi

Final Fantasy XI (FF11) のナビゲーション メッセージを生成するための Web API です。

## 概要

このAPIは、キャラクターID、ゾーンID、座標などの情報を受け取り、それに応じたナビゲーションメッセージを返します。
LandSandBoat ナビゲーションシステムの一部として機能することを想定しています。

## 動作環境

*   .NET 8
*   MySQL データベース

## セットアップ

1.  **リポジトリをクローンします。**
    ```sh
    git clone <repository-url>
    cd LsbDatabaseApi
    ```

2.  **接続文字列を設定します。**
    `appsettings.json` ファイルを開き、`ConnectionStrings` セクションにある `LandSandBoat` の値を、ご自身のMySQLデータベースの接続文字列に書き換えてください。

    ```json
    {
      "ConnectionStrings": {
        "LandSandBoat": "server=your_server;port=your_port;database=your_database;uid=your_user;pwd=your_password"
      },
      // ...
    }
    ```

3.  **アプリケーションを実行します。**
    ```sh
    dotnet run
    ```
    APIは `http://localhost:5000` (または `launchSettings.json` で設定されたポート) で起動します。

## API エンドポイント

### ナビゲーションメッセージの取得

指定された情報に基づいてナビゲーションメッセージを返します。

*   **HTTP Method**: `GET`
*   **URL**: `/api/Database/GetMessage`
*   **Query Parameters**:
    *   `charaId` (integer): キャラクターID
    *   `zoneId` (integer): 現在のゾーンID
    *   `mapId` (integer): 現在のマップID
    *   `coordinates` (string): 現在の座標
    *   `preZoneId` (integer): 以前のゾーンID
    *   `preMapId` (integer): 以前のマップID
    *   `preCoordinates` (string): 以前の座標

*   **成功レスポンス**:
    *   **Code**: `200 OK`
    *   **Content**: `string` (生成されたナビゲーションメッセージ)

*   **使用例**:
    ```
    GET http://localhost:5000/api/Database/GetMessage?charaId=12345&zoneId=100&mapId=1&coordinates=0,0&preZoneId=101&preMapId=1&preCoordinates=1,1
    ```
