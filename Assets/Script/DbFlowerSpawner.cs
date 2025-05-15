using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class DbFlowerSpawner : MonoBehaviour
{
    [Header("�Q�Ɛ�")]
    [Tooltip("ActionCableClient �����I�u�W�F�N�g")]
    public ActionCableClient cableClient;

    [Tooltip("�摜�t���Ԃт�v���n�u (SpriteRenderer�{Rigidbody2D�{ImageFlowerMovement)")]
    public GameObject flowerPrefab;

    [Header("�����ݒ�")]
    [Tooltip("�������� X ���W (��ʉE�[�Ȃ�)")]
    public float spawnPositionX = 10f;

    [Tooltip("Y ���̃����_���͈� (����)")]
    public float spawnAreaHeight = 10f;

    [Header("�y�������ݒ�")]
    [Tooltip("�Œ�� Z ���W�ŏo�������ꍇ")]
    public float spawnPositionZ = 0f;

    [Tooltip("�����_���� Z ��U�肽���ꍇ�̉���")]
    public float minSpawnZ = 0f;

    [Tooltip("�����_���� Z ��U�肽���ꍇ�̏��")]
    public float maxSpawnZ = 0f;

    [Tooltip("Z �������_���ɐU�邩 (�`�F�b�N���O���� spawnPositionZ ���g��)")]
    public bool randomizeZ = false;

    void Start()
    {
        StartCoroutine(SpawnDbFlowersLoop());
    }

    private IEnumerator SpawnDbFlowersLoop()
    {
        while (true)
        {
            // URL �L���[����Ȃ�ҋ@
            while (!cableClient.HasFlowerUrl())
                yield return null;

            // �L���[���玟�� URL �����o��
            string url = cableClient.DequeueFlowerUrl();

            // �y�ǉ��z�ǂ̃I�u�W�F�N�g���ǂ� URL �����N�G�X�g���Ă��邩���O�o��
            Debug.Log($"[DbFlowerSpawner] �_�E�����[�h�J�n �� Spawner={gameObject.name}, URL={url}");

            // �摜���_�E�����[�h
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            yield return req.SendWebRequest();

            // ���s������ڍ׃��O���o���Ď���
            if (req.result != UnityWebRequest.Result.Success)
            {
                // �擾�w�b�_�[
                var headers = req.GetResponseHeaders();
                string headerLog = "";
                if (headers != null)
                {
                    foreach (var kv in headers)
                        headerLog += $"{kv.Key}: {kv.Value}\n";
                }

                // ���X�|���X�{�f�B�iHTML�Ȃǁj�𕶎���Ŏ擾
                string body = "";
                try { body = req.downloadHandler.text; }
                catch { body = "(��e�L�X�g���X�|���X)"; }

                Debug.LogError(
                    $"[DbFlowerSpawner] �_�E�����[�h���s�F\n" +
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
                // Texture2D �� Sprite ����
                Texture2D tex = ((DownloadHandlerTexture)req.downloadHandler).texture;
                Sprite sprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f)
                );

                // Y �ʒu�����_��
                float y = UnityEngine.Random.Range(-spawnAreaHeight / 2f, spawnAreaHeight / 2f);

                // Z �ʒu�� �Œ� or �����_�� �őI��
                float z = randomizeZ
                    ? UnityEngine.Random.Range(minSpawnZ, maxSpawnZ)
                    : spawnPositionZ;

                // �ŏI�I�Ȑ����ʒu
                Vector3 pos = new Vector3(spawnPositionX, y, z);

                // �v���n�u�𐶐����ASprite ���Z�b�g
                var go = Instantiate(flowerPrefab, pos, Quaternion.identity);
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning("[DbFlowerSpawner] FlowerPrefab �� SpriteRenderer ������܂���B");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DbFlowerSpawner] ��O����: {ex.Message}\n{ex.StackTrace}");
            }

            // ���̃t���[���܂őҋ@
            yield return null;
        }
    }
}
