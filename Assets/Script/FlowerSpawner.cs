using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 花を生成し、プーリング／ランダムサイズを管理するスクリプト
/// </summary>
public class FlowerSpawner : MonoBehaviour
{
    [Tooltip("生成する花のプレハブ")]
    public GameObject flowerPrefab;

    [Tooltip("最小生成間隔 (秒)")]
    public float minSpawnTime = 2.0f;

    [Tooltip("最大生成間隔 (秒)")]
    public float maxSpawnTime = 4.0f;

    [Tooltip("同時に存在できる最大数")]
    public int maxFlowers = 20;

    [Tooltip("生成エリアの高さ")]
    public float spawnAreaHeight = 10.0f;

    [Tooltip("生成位置のX座標")]
    public float spawnPositionX = 10f;

    [Header("ランダムスケール設定")]
    [Tooltip("花びらの最小スケール倍率")]
    public float minScale = 0.8f;

    [Tooltip("花びらの最大スケール倍率")]
    public float maxScale = 1.2f;

    // プールとアクティブ管理
    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeFlowers = new List<GameObject>();
    private Coroutine spawnRoutine;

    void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnFlowersRoutine());
    }

    void OnDisable()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnFlowersRoutine()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            // アクティブ数制限
            if (activeFlowers.Count < maxFlowers && flowerPrefab != null)
                SpawnFlower();
        }
    }

    private void SpawnFlower()
    {
        // プールから取得 or 新規インスタンス
        GameObject flower;
        if (pool.Count > 0)
        {
            flower = pool.Dequeue();
            flower.SetActive(true);
        }
        else
        {
            flower = Instantiate(flowerPrefab);
            var mv = flower.GetComponent<FlowerMovement>();
            if (mv != null)
                mv.SetSpawner(this);
        }

        // 位置・回転リセット
        float randomY = Random.Range(-spawnAreaHeight / 2f, spawnAreaHeight / 2f);
        flower.transform.position = new Vector3(spawnPositionX, transform.position.y + randomY, 0f);
        flower.transform.rotation = Quaternion.identity;

        // ランダムスケール適用
        float scale = Random.Range(minScale, maxScale);
        flower.transform.localScale = Vector3.one * scale;

        activeFlowers.Add(flower);
    }

    /// <summary>
    /// FlowerMovement から呼び出し: プールに戻す
    /// </summary>
    public void ReleaseFlower(GameObject flower)
    {
        if (activeFlowers.Remove(flower))
        {
            flower.SetActive(false);
            pool.Enqueue(flower);
        }
    }
}


