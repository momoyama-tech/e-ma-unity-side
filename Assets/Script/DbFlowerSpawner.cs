using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class DbFlowerSpawner : MonoBehaviour
{
    [Header("参照先")]
    [Tooltip("ActionCableClient を持つオブジェクト")]
    public ActionCableClient cableClient;

    [Tooltip("画像付き花びらプレハブ (SpriteRenderer＋Rigidbody2D＋ImageFlowerMovement)")]
    public GameObject flowerPrefab;

    [Header("生成設定")]
    [Tooltip("生成する X 座標 (画面右端など)")]
    public float spawnPositionX = 10f;

    [Tooltip("Y 軸のランダム範囲 (高さ)")]
    public float spawnAreaHeight = 10f;

    [Header("Ｚ軸生成設定")]
    [Tooltip("固定の Z 座標で出したい場合")]
    public float spawnPositionZ = 0f;

    [Tooltip("ランダムに Z を振りたい場合の下限")]
    public float minSpawnZ = 0f;

    [Tooltip("ランダムに Z を振りたい場合の上限")]
    public float maxSpawnZ = 0f;

    [Tooltip("Z をランダムに振るか (チェックを外すと spawnPositionZ を使う)")]
    public bool randomizeZ = false;

    void Start()
    {
        StartCoroutine(SpawnDbFlowersLoop());
    }

    private IEnumerator SpawnDbFlowersLoop()
    {
        while (true)
        {
            // URL キューが空なら待機
            while (!cableClient.HasFlowerUrl())
                yield return null;

            // キューから次の URL を取り出し
            string url = cableClient.DequeueFlowerUrl();

            // 【追加】どのオブジェクトがどの URL をリクエストしているかログ出力
            Debug.Log($"[DbFlowerSpawner] ダウンロード開始 → Spawner={gameObject.name}, URL={url}");

            // 画像をダウンロード
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            yield return req.SendWebRequest();

            // 失敗したら詳細ログを出して次へ
            if (req.result != UnityWebRequest.Result.Success)
            {
                // 取得ヘッダー
                var headers = req.GetResponseHeaders();
                string headerLog = "";
                if (headers != null)
                {
                    foreach (var kv in headers)
                        headerLog += $"{kv.Key}: {kv.Value}\n";
                }

                // レスポンスボディ（HTMLなど）を文字列で取得
                string body = "";
                try { body = req.downloadHandler.text; }
                catch { body = "(非テキストレスポンス)"; }

                Debug.LogError(
                    $"[DbFlowerSpawner] ダウンロード失敗：\n" +
                    $"  Spawner:  {gameObject.name}\n" +
                    $"  URL:      {req.url}\n" +
                    $"  Status:   HTTP {(long)req.responseCode}\n" +
                    $"  Error:    {req.error}\n" +
                    $"  Headers:\n{headerLog}\n" +
                    $"  Body:\n{body}"
                );
                continue;
            }


            try
            {
                // Texture2D → Sprite 生成
                Texture2D tex = ((DownloadHandlerTexture)req.downloadHandler).texture;
                Sprite sprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f)
                );

                // Y 位置ランダム
                float y = UnityEngine.Random.Range(-spawnAreaHeight / 2f, spawnAreaHeight / 2f);

                // Z 位置を 固定 or ランダム で選択
                float z = randomizeZ
                    ? UnityEngine.Random.Range(minSpawnZ, maxSpawnZ)
                    : spawnPositionZ;

                // 最終的な生成位置
                Vector3 pos = new Vector3(spawnPositionX, y, z);

                // プレハブを生成し、Sprite をセット
                var go = Instantiate(flowerPrefab, pos, Quaternion.identity);
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning("[DbFlowerSpawner] FlowerPrefab に SpriteRenderer がありません。");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DbFlowerSpawner] 例外発生: {ex.Message}\n{ex.StackTrace}");
            }

            // 次のフレームまで待機
            yield return null;
        }
    }
}
