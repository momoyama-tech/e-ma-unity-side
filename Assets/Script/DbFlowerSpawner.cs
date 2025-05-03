using UnityEngine;
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

            // 画像をダウンロード
            var req = UnityWebRequestTexture.GetTexture(url);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[DbFlowerSpawner] ダウンロード失敗: {req.error}");
                continue;
            }

            // Texture2D → Sprite 生成
            Texture2D tex = ((DownloadHandlerTexture)req.downloadHandler).texture;
            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );

            // Y 位置ランダム
            float y = Random.Range(-spawnAreaHeight / 2f, spawnAreaHeight / 2f);

            // Z 位置を 固定 or ランダム で選択
            float z = randomizeZ
                ? Random.Range(minSpawnZ, maxSpawnZ)
                : spawnPositionZ;

            // 最終的な生成位置
            Vector3 pos = new Vector3(spawnPositionX, y, z);

            // プレハブを生成し、Sprite をセット
            var go = Instantiate(flowerPrefab, pos, Quaternion.identity);
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = sprite;
            else
                Debug.LogWarning("[DbFlowerSpawner] FlowerPrefab に SpriteRenderer がありません。");

            yield return null;
        }
    }
}
