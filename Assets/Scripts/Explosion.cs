using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆発
/// </summary>
public class Explosion : MonoBehaviour
{
    /// <summary>
    /// 弾のプレハブ
    /// </summary>
    [SerializeField]
    Bullet _bulletPrefab = null;

    /// <summary>
    /// 威力倍率
    /// </summary>
    [SerializeField]
    float _powerScale = 1.0f;

    /// <summary>
    /// 爆発SE
    /// </summary>
    [SerializeField]
    GameObject _destroySE = null;

    /// <summary>
    /// パンチ方向
    /// </summary>
    [SerializeField]
    Vector3[] _punchDirections = { };

    /// <summary>
    /// 揺れる時間
    /// </summary>
    [SerializeField]
    float _duration = 0.5f;

    /// <summary>
    /// 振動の強さ
    /// </summary>
    [SerializeField]
    int _vibrato = 25;

    Rect _screenRect = new Rect();

    /// <summary>
    /// 初期化する
    /// </summary>
    /// <param name="screenRect"></param>
    public void Initialize(Rect screenRect)
    {
        _screenRect = screenRect;
    }

    /// <summary>
    /// 爆発を開始する
    /// </summary>
    /// <param name="position"></param>
    public void StartExplode(Vector3 position)
    {
        int index = Random.Range(0, _punchDirections.Length);
        Vector3 direction = _punchDirections[index];
        transform.DOPunchPosition(direction, _duration, _vibrato)
            .SetLink(gameObject)
            .OnComplete(() => Explode(position));
    }

    /// <summary>
    /// 爆発する
    /// </summary>
    /// <param name="position"></param>
    private void Explode(Vector3 position)
    {
        Instantiate(_destroySE, position, Quaternion.identity);

        for (float i = 0.0f; i < 360.0f; i += 15)
        {
            Bullet bullet = Instantiate(_bulletPrefab);

            // 弾の移動開始
            bullet.StartMove(position, GetDirection(i), _screenRect, _powerScale);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 角度から方向を取得する
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    Vector3 GetDirection(float angle)
    {
        float x = Mathf.Sin(angle * Mathf.Deg2Rad);
        float y = Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = 0.0f;

        Vector3 direction = new Vector3(x, y, z).normalized;
        return direction;
    }
}
