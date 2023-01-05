using SpriteGlow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤー側の操作・処理を制御するクラス
/// </summary>
public class Player : MonoBehaviour
{
    // GameManagerコンポーネント
    [SerializeField]
    GameManager gameManager = null;

    // Animatorコンポーネント
    Animator animator = null;

    // SpriteGlowコンポーネント
    SpriteGlowEffect spriteGlowEffect = null;

    // AudioSourceコンポーネントの配列
    AudioSource[] audioSourceList = null;

    // 自動射撃設定
    [SerializeField]
    bool isAutomaticFire = false;

    // 移動速度
    // SerializeField Unityの機能
    // privateだが、Unityのエディターで編集可能
    [SerializeField]
    float _moveSpeedX = 10.0f;

    [SerializeField]
    float _moveSpeedY = 10.0f;

    // 3D空間の横幅
    [SerializeField]
    float width = 1.0f;

    // 3D空間の縦幅
    [SerializeField]
    float height = 1.0f;

    // 弾のプレハブ
    [SerializeField]
    GameObject bulletPrefab = null;

    // HPバー
    [SerializeField]
    Image hpBar = null;

    // HP表示
    [SerializeField]
    Text hpText = null;

    // HP最大値
    [SerializeField]
    int maxHp = 100;

    // 死亡時に再生するエフェクト
    [SerializeField]
    GameObject effectDeadPrefab = null;

    // 死亡時に鳴らす効果音プレファブ
    [SerializeField]
    GameObject destroySEPrefab = null;

    // ショットを撃ってから、次のショットが撃てるようになるまでの秒数
    [SerializeField]
    float shotCoolTime = 0.1f;

    // ショットが可能か
    bool canShot = true;

    // 画面サイズ
    Rect screenRect = new Rect();

    // 現在のHP
    int currentHp = 0;

    // 死亡しているか
    bool isDead = false;

    // 被ダメージ倍率
    float defenseScale = 1.0f;

    // 被ダメージ倍率変化の効果時間カウント(現在値)
    float currentDefenseScaleEffectTimeCount = 0.0f;

    // 被ダメージ倍率変化の効果時間カウント(最大値)
    float maxDefenseScaleEffectTimeCount = 0.0f;

    // 被ダメージ倍率低下アイテム取得時の被ダメージ倍率
    [SerializeField]
    float defenseUPItemScale = 0.5f;

    // 被ダメージ倍率低下の効果時間
    [SerializeField]
    float defenseUpEffectTime = 10.0f;

    // 被ダメージ倍率低下のときのGlowColor
    [SerializeField]
    Color defenseUpGlowColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

    // 被ダメージ倍率低下時のバーの色
    [SerializeField]
    Color defenseUpEffectTimeBarColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

    // 被ダメージ倍率低下のとき、バーに表示する文字列
    [SerializeField]
    string defenseUpText = "DEFENSE UP";

    // 被ダメージ倍率上昇アイテム取得時の被ダメージ倍率
    [SerializeField]
    float defenseDownItemScale = 2.0f;

    // 被ダメージ倍率上昇の効果時間
    [SerializeField]
    float defenseDownEffectTime = 5.0f;

    // 被ダメージ倍率上昇時のGlowColor
    [SerializeField]
    Color defenseDownGlowColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);

    // 被ダメージ倍率上昇時のバーの色
    [SerializeField]
    Color defenseDownEffectTimeBarColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);

    // 被ダメージ倍率上昇時、バーに表示する文字列
    [SerializeField]
    string defenseDownText = "DEFENSE DOWN";

    // 移動速度変化の効果バー
    [SerializeField]
    Image defenseScaleEffectBar = null;

    // 移動速度変化の効果表示
    [SerializeField]
    Text defenseScaleEffectBarText = null;

    // 攻撃力の倍率
    float offenseScale = 1.0f;

    // 攻撃力変化の効果時間カウント(現在値)
    float currentOffenseScaleEffectTimeCount = 0.0f;

    // 攻撃力変化の効果時間カウント(最大値)
    float maxOffenseScaleEffectTimeCount = 0.0f;

    // 攻撃力アップアイテム取得時の倍率
    [SerializeField]
    float offenseUpItemScale = 2.0f;

    // 攻撃力アップの効果時間
    [SerializeField]
    float offenseUpEffectTime = 10.0f;

    // 攻撃力アップ時のGlowColor
    [SerializeField]
    Color offenseUpGlowColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    // 攻撃力アップ時のバーの色
    [SerializeField]
    Color offenseUpEffectTimeBarColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    // 攻撃力アップ時、バーに表示する文字列
    [SerializeField]
    string offenseUpText = "OFFENSE UP";

    // 攻撃力ダウンアイテム取得時の倍率
    [SerializeField]
    float offenseDownItemScale = 0.5f;

    // 攻撃力ダウンの効果時間
    [SerializeField]
    float offenseDownEffectTime = 5.0f;

    // 攻撃力ダウンのときのGlowColor
    [SerializeField]
    Color offenseDownGlowColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);

    // 攻撃力ダウン時のバーの色
    [SerializeField]
    Color offenseDownEffectTimeBarColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);

    // 攻撃力ダウンのとき、バーに表示する文字列
    [SerializeField]
    string offenseDownText = "OFFENSE DOWN";

    // 攻撃力変化の効果バー
    [SerializeField]
    Image offenseScaleEffectBar = null;

    // 攻撃力変化の効果表示
    [SerializeField]
    Text offenseScaleEffectBarText = null;

    // 通常時、バーに表示する文字列
    [SerializeField]
    string noEffectText = "NO EFFECT";

    /// <summary>
    /// 線を引くボタンの名前
    /// </summary>
    [SerializeField]
    string _lineButtonName = "Line";

    /// <summary>
    /// 線プレハブ
    /// </summary>
    [SerializeField]
    Line _linePrefab = null;

    /// <summary>
    /// 引かれている線
    /// </summary>
    Line _line = null;

    readonly List<Line> _lineList = new();

    /// <summary>
    /// 線引き中である
    /// </summary>
    bool _isDrawingLine = false;

    /// <summary>
    /// 線引き中の移動速度倍率
    /// </summary>
    [SerializeField]
    float _drawingLineMoveSpeedScale = 0.5f;

    /// <summary>
    /// 爆破中である
    /// </summary>
    bool _isExploding = false;

    /// <summary>
    /// 爆発の感覚
    /// </summary>
    [SerializeField]
    float _explosionInterval = 0.5f;

    /// <summary>
    /// プレイヤーが生成した瞬間に呼ばれます。
    /// </summary>
    private void Awake()
    {
        currentHp = maxHp;
    }

    /// <summary>
    /// プレイヤーが生成されて、1フレーム後に呼ばれます。
    /// </summary>
    private void Start()
    {
        // Animatorコンポーネントを取得する
        animator = gameObject.GetComponent<Animator>();

        // SpriteGlowコンポーネントを取得する
        spriteGlowEffect = gameObject.GetComponent<SpriteGlowEffect>();

        // AudioSourceコンポーネントをすべて取得する
        audioSourceList = gameObject.GetComponents<AudioSource>();
    }

    /// <summary>
    /// 初期化処理
    /// public：外部アクセスできる関数（メソッド）
    /// </summary>
    public void Init(Rect screenRect)
    {
        this.screenRect = screenRect;
    }

    /// <summary>
    /// 毎フレーム更新される関数（メソッド）
    /// </summary>
    private void Update()
    {
        // ゲームステートがPlayでない場合、何も起きない
        if (gameManager.gameState != GameState.Play)
        {
            return;
        }

        // 死亡したら、動かないようにする
        if (currentHp <= 0)
        {
            return;
        }

        // 移動
        UpdateMove();

        // 弾撃ち
        UpdateShot();

        // 線引き
        UpdateLine();

        // アイテムの効果
        UpdateEffect();

        // SpriteGlowの効果
        UpdateSpriteGlow();


    }

    /// <summary>
    /// 移動処理
    /// </summary>
    void UpdateMove()
    {
        // 一度、変数の格納する。
        // transform.position.x += 1.0f これはできない為！
        Vector3 position = transform.position;

        float moveSpeedX = _moveSpeedX;
        float moveSpeedY = _moveSpeedY;

        if (_isDrawingLine)
        {
            moveSpeedX *= _drawingLineMoveSpeedScale;
            moveSpeedY *= _drawingLineMoveSpeedScale;
        }

        // 左右の移動入力 X軸 +が→ -が←
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            // Time.deltaTime：処理落ちがあっても、移動量が変わらないようにする為に使用している。
            position.x -= moveSpeedX * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            position.x += moveSpeedX * Time.deltaTime;
        }

        // 上下の移動入力 Y軸は+が↑ -が↓ 左右移動とは別処理なので斜め移動が実現できる 
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            position.y += moveSpeedY * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            position.y -= moveSpeedY * Time.deltaTime;
        }

        // 画面外に行かせないようにしている。
        float halfWidth = width / 2.0f;
        float screenLeft = screenRect.x + halfWidth;
        float screenRight = screenRect.width - halfWidth;

        if (position.x < screenLeft)
        {
            position.x = screenLeft;
        }
        else if (position.x > screenRight)
        {
            position.x = screenRight;
        }

        // 縦方向の画面外へも行けないようにする
        float halfHeight = height / 2.0f;
        float screenUp = screenRect.y - halfHeight;
        float screenDown = screenRect.height + halfHeight;

        if (position.y > screenUp)
        {
            position.y = screenUp;
        }
        else if (position.y < screenDown)
        {
            position.y = screenDown;
        }

        // 反映
        transform.position = position;
    }

    /// <summary>
    /// 弾を発射する
    /// </summary>
    void UpdateShot()
    {
        if (isAutomaticFire || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Z))
        {
            // 弾が発射可能なら発射する
            if (canShot)
            {
                StartCoroutine(Shot());
            }
        }
    }

    /// <summary>
    /// 線の更新処理
    /// </summary>
    void UpdateLine()
    {
        if (Input.GetButtonDown(_lineButtonName))
        {
            _isDrawingLine = true;

            _line = Instantiate(_linePrefab);

            _lineList.Add(_line);

            _line.StartLine(transform.position);
        }
        else if (Input.GetButton(_lineButtonName) && _line)
        {
            _line.UpdateLine(transform.position);
        }
        else if (Input.GetButtonUp(_lineButtonName))
        {
            _isDrawingLine = false;
        }
        else if (!_isExploding && Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(ExplodeLines());
        }
    }

    /// <summary>
    /// 線を爆発させる
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExplodeLines()
    {
        WaitForSeconds wait = new(_explosionInterval);

        foreach (var line in _lineList)
        {
            Explosion explosion = line.GetComponent<Explosion>();
            explosion.Initialize(screenRect);
            explosion.StartExplode(line.GetPosition());

            yield return wait;
        }

        _lineList.Clear();
    }

    /// <summary>
    /// アイテムの効果処理
    /// </summary>
    void UpdateEffect()
    {
        // 被ダメージ倍率変化アイテムの効果時間カウント
        if (currentDefenseScaleEffectTimeCount > 0.0f)
        {
            currentDefenseScaleEffectTimeCount -= Time.deltaTime;
            if (currentDefenseScaleEffectTimeCount <= 0.0f)
            {
                currentDefenseScaleEffectTimeCount = 0.0f;
                defenseScale = 1.0f;
            }

            // ゲージ反映
            float defenseScaleEffectBarFill = currentDefenseScaleEffectTimeCount / maxDefenseScaleEffectTimeCount;
            defenseScaleEffectBar.fillAmount = defenseScaleEffectBarFill;

            // 表示
            if (defenseScale < 1.0f)
            {
                defenseScaleEffectBarText.text = defenseUpText;
            }
            else if (defenseScale > 1.0f)
            {
                defenseScaleEffectBarText.text = defenseDownText;
            }
            else
            {
                defenseScaleEffectBarText.text = noEffectText;
            }
        }

        // 攻撃力変化アイテムの効果時間カウント
        if (currentOffenseScaleEffectTimeCount > 0.0f)
        {
            currentOffenseScaleEffectTimeCount -= Time.deltaTime;
            if (currentOffenseScaleEffectTimeCount <= 0.0f)
            {
                currentOffenseScaleEffectTimeCount = 0.0f;
                offenseScale = 1.0f;
            }

            // ゲージ反映
            float attackScaleEffectBarFill = currentOffenseScaleEffectTimeCount / maxOffenseScaleEffectTimeCount;
            offenseScaleEffectBar.fillAmount = attackScaleEffectBarFill;

            // 表示
            if (offenseScale > 1.0f)
            {
                offenseScaleEffectBarText.text = offenseUpText;
            }
            else if (offenseScale < 1.0f)
            {
                offenseScaleEffectBarText.text = offenseDownText;
            }
            else
            {
                offenseScaleEffectBarText.text = noEffectText;
            }
        }
    }

    /// <summary>
    /// SpriteGlowの処理
    /// </summary>
    void UpdateSpriteGlow()
    {
        // 攻撃力、被ダメージ倍率の変化に応じて、SpriteGlowを変更する
        if (offenseScale > 1.0f)
        {
            spriteGlowEffect.GlowColor = offenseUpGlowColor;
        }
        else if (defenseScale < 1.0f)
        {
            spriteGlowEffect.GlowColor = defenseUpGlowColor;
        }
        else if (offenseScale < 1.0f)
        {
            spriteGlowEffect.GlowColor = offenseDownGlowColor;
        }
        else if (defenseScale > 1.0f)
        {
            spriteGlowEffect.GlowColor = defenseDownGlowColor;

        }
        else
        {
            // SpriteGlowの光を見えなくする
            spriteGlowEffect.OutlineWidth = 0;
        }
    }

    /// <summary>
    /// 弾を発射する
    /// </summary>
    IEnumerator Shot()
    {
        // 弾を生成する。
        GameObject instance = Instantiate(bulletPrefab);

        // Bulletコンポーネントを取得
        Bullet bullet = instance.GetComponent<Bullet>();

        // 移動開始（生成位置・方向・画面サイズ）
        bullet.StartMove(transform.position, GetDirection(0.0f), screenRect, offenseScale);

        // 弾が撃てなくなる
        canShot = false;

        // 指定された秒数待つ
        yield return new WaitForSeconds(shotCoolTime);

        // 再び弾が発射可能になる
        canShot = true;

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
            // HPを減らす
            Damage(bullet.GetDamageValue());

            // 弾を消す
            Destroy(bullet.gameObject);
        }

        // ヒットしたオブジェクトが、アイテムなのかをチェックする
        Item item = collision.gameObject.GetComponent<Item>();
        if (item != null)
        {
            if (item.GetItemType() == ItemType.DefenseUp)
            {
                // 強化効果音
                if (audioSourceList[0] != null)
                {
                    audioSourceList[0].Play();
                }

                // 被ダメージダウン
                defenseScale = defenseUPItemScale;
                defenseScaleEffectBar.color = defenseUpEffectTimeBarColor;
                currentDefenseScaleEffectTimeCount = defenseUpEffectTime;
                maxDefenseScaleEffectTimeCount = defenseUpEffectTime;
            }
            else if (item.GetItemType() == ItemType.offenseUp)
            {
                // 強化効果音
                if (audioSourceList[0] != null)
                {
                    audioSourceList[0].Play();
                }

                // 攻撃力アップ
                offenseScale = offenseUpItemScale;
                offenseScaleEffectBar.color = offenseUpEffectTimeBarColor;
                currentOffenseScaleEffectTimeCount = offenseUpEffectTime;
                maxOffenseScaleEffectTimeCount = offenseUpEffectTime;
            }
            else if (item.GetItemType() == ItemType.DefenseDown)
            {
                // 弱体効果音
                if (audioSourceList[1] != null)
                {
                    audioSourceList[1].Play();
                }

                // 被ダメージアップ
                defenseScale = defenseDownItemScale;
                defenseScaleEffectBar.color = defenseDownEffectTimeBarColor;
                currentDefenseScaleEffectTimeCount = defenseDownEffectTime;
                maxDefenseScaleEffectTimeCount = defenseDownEffectTime;

            }
            else if (item.GetItemType() == ItemType.offenseDown)
            {
                // 弱体効果音
                if (audioSourceList[1] != null)
                {
                    audioSourceList[1].Play();
                }

                // 攻撃力ダウン
                offenseScale = offenseDownItemScale;
                offenseScaleEffectBar.color = offenseDownEffectTimeBarColor;
                currentOffenseScaleEffectTimeCount = offenseDownEffectTime;
                maxOffenseScaleEffectTimeCount = offenseDownEffectTime;
            }

            // SpriteGlowの光を見えるようにする
            spriteGlowEffect.OutlineWidth = 1;

            // アイテムを消す
            Destroy(item.gameObject);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }

    /// <summary>
    /// ダメージを受けた！
    /// </summary>
    /// <param name="damageValue"></param>
    void Damage(int damageValue)
    {
        // 被ダメージ倍率を適用する
        damageValue = (int)(damageValue * defenseScale);
        currentHp -= damageValue;

        // 死亡した！
        if (currentHp <= 0)
        {
            // マイナス値になったら、０にする
            currentHp = 0;

            // 死亡効果音再生
            if (destroySEPrefab != null)
            {
                PlayDestroySound(destroySEPrefab);
            }

            // 死亡エフェクト再生
            GameObject instance = Instantiate(effectDeadPrefab);
            instance.transform.position = transform.position;

            //死亡フラグを立てる
            isDead = true;

            // ゲームオブジェクトを非アクティブにして、非表示にする メモリから消えたわけではない
            gameObject.SetActive(false);
        }
        else
        {
            // ダメージ効果音
            if (audioSourceList[2] != null)
            {
                audioSourceList[2].Play();
            }

            // ダメージエフェクト
            if (damageValue > 0)
            {
                animator.SetTrigger("Damage");
            }
        }

        // ゲージ反映
        float fill = (float)currentHp / (float)maxHp;
        hpBar.fillAmount = fill;

        // 表示
        hpText.text = currentHp.ToString() + " / " + maxHp.ToString();
    }

    /// <summary>
    /// 撃破された際に効果音を鳴らす
    /// </summary>
    public void PlayDestroySound(GameObject prefab)
    {
        // 効果音プレファブを生成する
        Instantiate<GameObject>(prefab);

    }

    /// <summary>
    /// 死亡しているかどうかを取得する
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }
}
