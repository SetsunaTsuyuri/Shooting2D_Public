using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyPointType
{
    Normal,
    Boss
}

public class EnemyPoint : MonoBehaviour
{
    // 敵出現タイプ
    [SerializeField]
    EnemyPointType enemyPointType = EnemyPointType.Normal;

    // GameManagerコンポーネント
    [SerializeField]
    GameManager gameManager = null;

    // スコア表示
    [SerializeField]
    Score score = null;
    
    // 敵が再出現するかどうか
    [SerializeField]
    bool isRepop = true;

    // 敵が再出現するまでの時間
    [SerializeField]
    float enemyRepopTime = 5.0f;

    // 残り時間カウント
    float enemyRepopTimeCount = 0.0f;

    // 生み出される通常の敵プレファブの配列
    [SerializeField]
    GameObject[] normalEnemyPrefabList = null;
    

    // 生み出されるレア敵プレファブの配列
    [SerializeField]
    GameObject[] rareEnemyPrefabList = null;

    // レア敵が生み出される確率
    [SerializeField]
    int rareEnemyAppearanceRate = 0;

    // HPゲージを表示する際、親となるキャンバス
    [SerializeField]
    RectTransform hpGaugeParent = null;

    // ボス限定 配列何番目の敵が出現するかのカウント
    int enemyAppearanceIndexCount = 0;

    // 画面サイズ
    Rect screenRect = new Rect();

    private void Start()
    {
        // 敵出現カウントを初期値にする
        enemyRepopTimeCount = enemyRepopTime;
    }

    private void Update()
    {
        // ゲームステートがPlayでないなら、何も起きない
        if (gameManager.gameState != GameState.Play)
        {
            return;
        }

        // 再出現しないなら中断
        if (!isRepop)
        {
            return;
        }

        // 子要素がいる(生み出した敵が生存している)なら中断
        int childCount = transform.childCount;
        if (childCount > 0)
        {
            return;
        }

        // 敵出現カウントを進める
        enemyRepopTimeCount -= Time.deltaTime;

        if (enemyRepopTimeCount <= 0)
        {
            // 敵を出現させる
            CreateEnemy();

            // 敵出現カウントを初期値に戻す
            enemyRepopTimeCount = enemyRepopTime;
        }
        
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Init(Rect screenRect)
    {
        this.screenRect = screenRect;
    }

    /// <summary>
    /// 配列の中からランダムに敵プレファブを選び、Instanciateする
    /// </summary>
    public void CreateEnemy()
    {
        // レア敵が出るかどうかの判定
        int random = Random.Range(0, 100);
        GameObject[] prefabList;

        if (random < rareEnemyAppearanceRate)
        {
            prefabList = rareEnemyPrefabList;
        }
        else
        {
            prefabList = normalEnemyPrefabList;
        }

        // 配列何番目の敵プレファブを生成するか
        int index;

        // ボスは順番 それ以外はランダム
        switch (enemyPointType)
        {
            case EnemyPointType.Boss:
                index = enemyAppearanceIndexCount;
                if (index > prefabList.Length - 1)
                {
                    index = 0;
                    enemyAppearanceIndexCount = 0;
                }
                break;

            case EnemyPointType.Normal:
                index = Random.Range(0, prefabList.Length);
                break;

            default:
                index = Random.Range(0, prefabList.Length);
                break;
        }

        // インスタンス生成
        GameObject instance = Instantiate<GameObject>(prefabList[index], transform);

        // position設定
        instance.transform.position = transform.position;

        // Enemyコンポーネント取得
        Enemy enemy = instance.GetComponent<Enemy>();

        // 初期化処理
        enemy.Init(screenRect, hpGaugeParent, score);

        // ボス限定カウントを進める
        enemyAppearanceIndexCount++;
    }
}
