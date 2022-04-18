using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
	Normal,
	BossRed,
	BossBlue
}

/// <summary>
/// エネミーの制御クラス
/// </summary>
public class Enemy : MonoBehaviour
{
	// ---------------メンバ変数------------------------

	// animatorコンポーネント
	Animator animator = null;

	// Scoreコンポーネント
	Score score = null;

	// 敵タイプ
	[SerializeField]
	EnemyType enemyType = EnemyType.Normal;

	// 移動速度
	[SerializeField]
	float moveSpeed = 10.0f;

	// 縦方向の移動距離
	[SerializeField]
	float verticalMoveDistance = 6.0f;

	// 3D空間の横幅
	[SerializeField]
	float width = 1.0f;

	// 弾を打つ間隔
	[SerializeField]
	float shotIntervalTime = 1.0f;

	// 必殺弾を撃つタイミング
	// n回撃ったあと、1回必殺弾を撃つ
	[SerializeField]
	int lethalBulletIntervalCount = 5;

	// 弾のプレハブ
	[SerializeField]
	GameObject bulletPrefab = null;

	// 必殺弾のプレファブ
	[SerializeField]
	GameObject lethalBulletPrefab = null;

	// HPバー
	[SerializeField]
	Image hpBar = null;

	// HP表示
	[SerializeField]
	Text hpText = null;

	// HP最大値
	[SerializeField]
	int maxHp = 100;

	// 攻撃ヒット時にもらえるスコア
	[SerializeField]
	int hitScorePoint = 0;

	// 撃破時にもらえるスコア
	[SerializeField]
	int destroyScorePoint = 100;

	// 死亡時に再生するエフェクト
	[SerializeField]
	GameObject effectDeadPrefab = null;

	// 死亡時に鳴らす効果音プレファブ
	[SerializeField]
	GameObject destroySEPrefab = null;

	// 攻撃力倍率
	[SerializeField]
	float attackPowerScale = 1.0f;

	// アイテムのプレファブ
	[SerializeField]
	GameObject itemPrefab = null;

	// 画面サイズ
	Rect screenRect = new Rect();

	// 移動方向
	//[SerializeField]
	float moveDir = 1.0f;

	// 弾を打つタイミングを計る為に使う
	float shotTimingTime = 0.0f;

	// 現在のHP
	int currentHp = 0;

    //HPゲージの座標
	//[SerializeField]
    RectTransform hpGaugeTransform = null;

    //// HPゲージのオフセット
	//[SerializeField]
    //Vector3 hpGaugeOffset = Vector3.zero;

    // HPゲージのプレファブ
    [SerializeField]
	GameObject hpGaugePrefab = null;

	// HPゲージのインスタンス
	GameObject hpGaugeInstance = null;

	// 弾が撃てる
	bool canShot = true;

	// 移動できる
	bool canMove = true;

	// 行動回数
	int actionCount = 0;

	// ------------------------------------------------

	/// <summary>
	/// エネミーが生成した瞬間に呼ばれます。
	/// </summary>
	private void Awake()
	{
		currentHp = maxHp;
	}

	/// <summary>
	/// エネミーが生成されて、1フレーム後に呼ばれます。
	/// </summary>
	private void Start()
	{
		//Animatorコンポーネントを取得する
		animator = gameObject.GetComponent<Animator>();

		//撃ち始めるタイミングにランダム性を持たせる
		float timeLag = Random.value;
		shotTimingTime += timeLag;

		// 初めの横の移動方向をランダムにする
		int random = Random.Range(0, 100);
		if (random < 50)
        {
			moveDir *= -1;
        }

	}
	/// <summary>
	/// 初期化処理
	/// public：外部アクセスできる関数（メソッド）
	/// </summary>
	public void Init(Rect screenRect, RectTransform hpGaugeParent,Score score)
	{
		this.screenRect = screenRect;
		this.score = score;

		// HPゲージを生成する
        if (hpGaugePrefab != null && hpGaugeParent != null)
        {
            hpGaugeInstance = Instantiate(hpGaugePrefab, hpGaugeParent);
            hpGaugeTransform = hpGaugeInstance.GetComponent<RectTransform>();
            hpBar = hpGaugeTransform.Find("Image_Value").GetComponent<Image>();
            hpText = hpGaugeTransform.Find("Text_Value").GetComponent<Text>();
        }
    }

	/// <summary>
	/// 毎フレーム更新される関数（メソッド）
	/// </summary>
	private void Update()
	{
		// 死亡したら、動かないようにする
		if (currentHp <= 0)
		{
			return;
		}

		// 移動
		UpdateMove();

		// 弾発射
		UpdateShot();

        //// ワールド空間からスクリーン空間に変換する処理
        //if (hpGaugeTransform != null)
        //{
        //    Vector3 worldPosition = transform.position + hpGaugeOffset;
        //    Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
        //    hpGaugeTransform.position = screenPosition;
        //}
    }

	/// <summary>
	/// 移動処理
	/// </summary>
	void UpdateMove()
	{
		// 移動できないなら中断
		if (!canMove)
        {
			return;
        }

		// 一度、変数の格納する。
		// transform.position.x += 1.0f これはできない為！
		Vector3 position = transform.position;

		// 縦の移動が終わってから、横移動に入る
		if (verticalMoveDistance > 0.0f)
        {
			// 縦方向の移動
			float moveDistance = moveSpeed * Time.deltaTime;
			position.y -= moveDistance;
			verticalMoveDistance -= moveDistance;
		}
        else
        {
			// 横方向の移動
			position.x -= moveDir * moveSpeed * Time.deltaTime;

		}

		// 画面外に行かせないようにしている。
		float halfWidth = width / 2.0f;
		float screenLeft = screenRect.x + halfWidth;
		float screenRight = screenRect.width - halfWidth;
		if (position.x < screenLeft)
		{
			moveDir *= -1.0f;
			position.x = screenLeft;
		}
		else if (position.x > screenRight)
		{
			moveDir *= -1.0f;
			position.x = screenRight;
		}

		// 反映
		transform.position = position;
	}

	/// <summary>
	/// 弾を打つ
	/// </summary>
	void UpdateShot()
	{
		if (bulletPrefab == null)
		{
			return;
		}

		// 弾が撃てないなら中断
		if (!canShot)
        {
			return;
        }

		// 縦移動中は、弾を撃てない
		if (verticalMoveDistance > 0.0f)
        {
			return;
        }

		shotTimingTime += Time.deltaTime;
		if (shotTimingTime >= shotIntervalTime)
		{
			shotTimingTime = 0.0f;

			// 敵タイプによって攻撃方法を変える
			switch (GetEnemyType())
            {
				case EnemyType.Normal:

					// 通常弾
					Shot();
					break;
					
				case EnemyType.BossRed:

					// 必殺弾の発射タイミング
					if (actionCount >= lethalBulletIntervalCount)
                    {
						// 必殺弾
						StartCoroutine(ShotBigLethalBullet5Times());

						// 行動回数リセット
						actionCount = 0;
					}
                    else
                    {
						// 3way弾
						Shot5Way();

						// 行動回数加算
						actionCount++;
					}
					
					break;

				case EnemyType.BossBlue:

					// 必殺弾の発射タイミング
					if (actionCount >= lethalBulletIntervalCount)
					{
						// 必殺・炸裂弾
						StartCoroutine(ShotSplitBullet());

						// 行動回数リセット
						actionCount = 0;
					}
					else
					{
						// 連射弾
						StartCoroutine(RapidFire());

						// 行動回数加算
						actionCount++;
					}

					break;

				default:
					break;

            }
        }
	}

	/// <summary>
	/// 通常弾発射
	/// </summary>
	void Shot()
    {
		if (bulletPrefab == null)
        {
			return;
        }

		// 弾を生成する。
		GameObject instance = Instantiate<GameObject>(bulletPrefab);

		// Bulletコンポーネントを取得
		Bullet bullet = instance.GetComponent<Bullet>();

		// 移動開始
		bullet.StartMove(transform.position, GetDirection(0.0f), screenRect, attackPowerScale);
	}

	/// <summary>
	/// 通常・5WAY弾発射
	/// </summary>
	void Shot5Way()
    {
		if (bulletPrefab == null)
        {
			return;
        }

		for (float i = -40.0f; i <= 40.0f; i += 20.0f)
        {

			// 弾を生成する
			GameObject instance = Instantiate<GameObject>(bulletPrefab);

			// Bulletコンポーネントを取得する
			Bullet bullet = instance.GetComponent<Bullet>();

			// 弾の移動開始
			bullet.StartMove(transform.position, GetDirection(i), screenRect, attackPowerScale);
		}
    }

	/// <summary>
	/// 通常・連射弾発射
	/// </summary>
	IEnumerator RapidFire()
    {
		// 移動・ショットができなくなる
		canShot = false;
		canMove = false;

		// 1秒待つ
		yield return new WaitForSeconds(1.0f);

		if (bulletPrefab != null)
        {
			// 20回繰り返す
			for (int i = 0; i < 20; i++)
            {
				// 弾を生成する
				GameObject instance = Instantiate<GameObject>(bulletPrefab);

				// Bulletコンポーネントを取得する
				Bullet bullet = instance.GetComponent<Bullet>();

				// 偶数回なら右から、奇数回なら左から弾の移動開始
				Vector3 position = transform.position;

				if (i % 2 == 0)
                {
					position.x += 0.5f;
					bullet.StartMove(position, GetDirection(0.0f), screenRect, attackPowerScale);
				}
                else
                {
					position.x -= 0.5f;
					bullet.StartMove(position, GetDirection(0.0f), screenRect, attackPowerScale);
				}

				// 0.05秒待つ
				yield return new WaitForSeconds(0.05f);
			}


		}

		//移動・ショットが可能になる
		canShot = true;
		canMove = true;
    }

	/// <summary>
	/// 必殺・巨大弾5発射
	/// </summary>
	IEnumerator ShotBigLethalBullet5Times()
	{
		// 移動・ショットができなくなる
		canShot = false;
		canMove = false;

		// 1秒待つ
		yield return new WaitForSeconds(1.0f);

		if (lethalBulletPrefab != null)
        {
			// 5回繰り返す
			for (int i = 0; i < 5; i++)
			{
				// 弾を生成する
				GameObject instance = Instantiate<GameObject>(lethalBulletPrefab);

				// Bulletコンポーネントを取得する
				Bullet bullet = instance.GetComponent<Bullet>();

				// 弾の移動開始
				bullet.StartMove(transform.position, GetDirection(0.0f), screenRect, attackPowerScale);

				// 0.3秒待つ
				yield return new WaitForSeconds(0.3f);
			}
		}

		// 移動・ショットが可能になる
		canShot = true;
		canMove = true;
	}

	/// <summary>
	/// 必殺・炸裂弾発射
	/// </summary>
	IEnumerator ShotSplitBullet()
	{
		// 移動・ショットができなくなる
		canShot = false;
		canMove = false;

		// 0.5秒待つ
		yield return new WaitForSeconds(0.5f);

		if (lethalBulletPrefab != null)
		{
			// 弾を生成する。
			GameObject instance = Instantiate<GameObject>(lethalBulletPrefab);

			// Bulletコンポーネントを取得
			Bullet bullet = instance.GetComponent<Bullet>();

			// 移動開始
			bullet.StartMove(transform.position, GetDirection(0.0f), screenRect, attackPowerScale);

			// 1秒待つ
			yield return new WaitForSeconds(1.0f);
		}

		// 移動・ショットが可能になる
		canShot = true;
		canMove = true;

	}

	/// <summary>
	/// アイテムドロップ
	/// </summary>
	void DropItem()
    {
		if (itemPrefab == null)
        {
			return;
        }

		// アイテムを生成する
		GameObject instance = Instantiate<GameObject>(itemPrefab);

		// Itemコンポーネントを取得する
		Item item = instance.GetComponent<Item>();

		// アイテムの移動開始
		item.StartMove(transform.position, GetDirection(0.0f), screenRect);
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
			Mathf.Cos(angle * Mathf.Deg2Rad) * -1.0f,
			0.0f
		).normalized;
	}

	/// <summary>
	/// 何らかのColliderにヒットしたときに呼ばれる関数（メソッド）
	/// </summary>
	/// <param name="collision"></param>
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 自分の弾だったら、無視する
		if (collision.gameObject.layer == gameObject.layer)
		{
			// これ以上、実行されない
			return;
		}

		// ヒットしたゲームオブジェクトが、Bulletなのかをチェックする
		Bullet bullet = collision.gameObject.GetComponent<Bullet>();
		if (bullet != null)
		{
			// スコア加算
			score.AddScorePoint(hitScorePoint);

			// HPを減らす
			Damage(bullet.GetDamageValue());

			// 弾を消す
			Destroy(bullet.gameObject);
		}
	}

	/// <summary>
	/// ダメージを受けた！
	/// </summary>
	/// <param name="damageValue"></param>
	void Damage(int damageValue)
	{
		currentHp -= damageValue;

		// 死亡した！
		if (currentHp <= 0)
		{
			// マイナス値になったら、0にする
			currentHp = 0;

			// スコア加算
			score.AddScorePoint(destroyScorePoint);

			// 死亡エフェクト再生
			GameObject instance = Instantiate(effectDeadPrefab);
			instance.transform.position = transform.position;

			// 死亡効果音再生
			if (destroySEPrefab != null)
            {
				PlayDestroySound(destroySEPrefab);
			}

			// アイテムドロップ
			DropItem();

			// HPゲージを削除する
			Destroy(hpGaugeInstance);

			// ゲームオブジェクトを削除する
			Destroy(gameObject);
		}
        else
        {
			// ダメージエフェクト
			if (damageValue > 0)
			{
				animator.SetTrigger("Damage");
			}
		}

		// ゲージが存在する場合、HP表示を反映する
		if (hpBar != null)
        {
			// ゲージ反映
			float fill = (float)currentHp / (float)maxHp;
			hpBar.fillAmount = fill;

			// 表示
			hpText.text = currentHp.ToString() + " / " + maxHp.ToString();
		}
	}

	/// <summary>
	/// 敵タイプを取得する
	/// </summary>
	public EnemyType GetEnemyType()
    {
		return enemyType;
    }

	/// <summary>
	/// 撃破された際に効果音を鳴らす
	/// </summary>
	public void PlayDestroySound(GameObject prefab)
	{
		// 効果音プレファブを生成する
		Instantiate<GameObject>(prefab);

	}
}
