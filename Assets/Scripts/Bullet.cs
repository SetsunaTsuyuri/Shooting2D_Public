using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	// ---------------メンバ変数------------------------

	// 移動速度
	[SerializeField]
	float moveSpeed = 20.0f;

	// ダメージ値
	[SerializeField]
	int damageValue = 5;

	// 弾の分裂先
	[SerializeField]
	GameObject splitBulletPrefab = null;

	// 分裂するまでにかかる時間
	[SerializeField]
	float splitTime = 0.0f;

	// 弾が生まれてから経過した時間
	float bulletTimeCount;

	Vector3 moveDir = Vector3.zero;
	Rect screenRect = new Rect();

	//ダメージ倍率
	float damageScale = 1.0f;
	
	// ------------------------------------------------

	/// <summary>
	/// 移動開始
	/// </summary>
	/// <param name="moveDir"></param>
	public void StartMove(Vector3 createPosition, Vector3 moveDir, Rect screenRect, float damageScale)
	{
		this.moveDir = moveDir;
		this.screenRect = screenRect;
		this.damageScale = damageScale;

		transform.position = createPosition;

		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, moveDir);
		transform.rotation = rotation;
	}

	private void Update()
	{
		transform.position += moveDir * moveSpeed * Time.deltaTime;

		// 画面外に行ったので、削除する
		if (screenRect.y < transform.position.y)
		{
			Destroy(gameObject);
		}
		else if (screenRect.height > transform.position.y)
		{
			Destroy(gameObject);
		}
		// 真横に撃った弾も消えるようにする
		else if (screenRect.x > transform.position.x)
        {
			Destroy(gameObject);
        }
        else if (screenRect.width < transform.position.x)
        {
            Destroy(gameObject);
        }

		// 弾が分裂するなら、分裂処理を行う
		bulletTimeCount += Time.deltaTime;
		if (splitBulletPrefab != null)
		{
			if (bulletTimeCount >= splitTime)
			{
				SplitBullet();
				Destroy(gameObject);
			}
		}


	}

	/// <summary>
	/// 弾の分裂処理
	/// </summary>
	public void SplitBullet()
    {
        for (float i = 0.0f; i < 360.0f; i += 30)
        {
            // 弾を生成する
            GameObject instance = Instantiate<GameObject>(splitBulletPrefab);

            // Bulletコンポーネントを取得する
            Bullet bullet = instance.GetComponent<Bullet>();

            // 弾の移動開始
            bullet.StartMove(transform.position, GetDirection(i), screenRect, damageScale);
        }
    }

	/// <summary>
	/// 角度から方向を取得
	/// </summary>
	/// <param name="angle"></param>
	/// <returns></returns>
	Vector3 GetDirection(float angle)
	{
		return new Vector3
		(
			Mathf.Sin(angle * Mathf.Deg2Rad),
			Mathf.Cos(angle * Mathf.Deg2Rad),
			0.0f
		).normalized;
		// normalizedで0~1の間に収まる
	}

	/// <summary>
	/// ダメージ値を取得
	/// </summary>
	/// <returns></returns>
	public int GetDamageValue()
	{
		return (int)(damageValue * damageScale);
	}
}
