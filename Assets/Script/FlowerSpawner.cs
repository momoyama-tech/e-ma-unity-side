using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ԃ𐶐����A�v�[�����O�^�����_���T�C�Y���Ǘ�����X�N���v�g
/// </summary>
public class FlowerSpawner : MonoBehaviour
{
    [Tooltip("��������Ԃ̃v���n�u")]
    public GameObject flowerPrefab;

    [Tooltip("�ŏ������Ԋu (�b)")]
    public float minSpawnTime = 2.0f;

    [Tooltip("�ő吶���Ԋu (�b)")]
    public float maxSpawnTime = 4.0f;

    [Tooltip("�����ɑ��݂ł���ő吔")]
    public int maxFlowers = 20;

    [Tooltip("�����G���A�̍���")]
    public float spawnAreaHeight = 10.0f;

    [Tooltip("�����ʒu��X���W")]
    public float spawnPositionX = 10f;

    [Header("�����_���X�P�[���ݒ�")]
    [Tooltip("�Ԃт�̍ŏ��X�P�[���{��")]
    public float minScale = 0.8f;

    [Tooltip("�Ԃт�̍ő�X�P�[���{��")]
    public float maxScale = 1.2f;

    // �v�[���ƃA�N�e�B�u�Ǘ�
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

            // �A�N�e�B�u������
            if (activeFlowers.Count < maxFlowers && flowerPrefab != null)
                SpawnFlower();
        }
    }

    private void SpawnFlower()
    {
        // �v�[������擾 or �V�K�C���X�^���X
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

        // �ʒu�E��]���Z�b�g
        float randomY = Random.Range(-spawnAreaHeight / 2f, spawnAreaHeight / 2f);
        flower.transform.position = new Vector3(spawnPositionX, transform.position.y + randomY, 0f);
        flower.transform.rotation = Quaternion.identity;

        // �����_���X�P�[���K�p
        float scale = Random.Range(minScale, maxScale);
        flower.transform.localScale = Vector3.one * scale;

        activeFlowers.Add(flower);
    }

    /// <summary>
    /// FlowerMovement ����Ăяo��: �v�[���ɖ߂�
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


