using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
	Ready,
	Play,
	GameOver
}

/// <summary>
/// ゲーム全体を管理するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
	[SerializeField]
	AudioSource stageBGM = null;

	[SerializeField]
	Player player = null;

	[SerializeField]
	EnemyPoint[] enemyPointList = null;

	[SerializeField]
	Item[] itemList = null;

	// GameStateLabelのTextコンポーネント
	[SerializeField]
	Text gameStateLabelText = null;


	// ゲームステート
	public GameState gameState = GameState.Ready;

	// 画面の左・右・上・下の境界の位置
	public Rect screenRect = new Rect();
	
	/// <summary>
	/// GameManagerのゲームオブジェクトが作られた瞬間に呼ばれる関数（メソッド）
	/// </summary>
    private void Awake()
    {
		//ゲームステートをReadyにする
		GameReady();


		// あらかじめ、画面上下左右の縁がワールド空間上でどこに位置するか調べておく
		var mainCamera = Camera.main;
		var positionZ = this.transform.position.z;
		var topRight = mainCamera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, positionZ));
		var bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, positionZ));
		screenRect.x = bottomLeft.x;
		screenRect.width = topRight.x;
		screenRect.y = topRight.y;
		screenRect.height = bottomLeft.y;

		// プレイヤー初期化
		player.Init(screenRect);

		// 敵出現ポイントの初期化
		for (int i = 0; i < enemyPointList.Length; i++)
        {
			enemyPointList[i].Init(screenRect);
        }

		// アイテム初期化
		for (int i = 0; i < itemList.Length; i++)
        {
			itemList[i].Init(screenRect);
        }
	}

    private void Start()
    {
		//AudioSorceコンポーネントを取得する
		stageBGM = gameObject.GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
		// Rキーが押されたら、シーンを読み込みなおすことでリセットする
        if (Input.GetKeyDown(KeyCode.R))
        {
			SceneManager.LoadScene("GameScene");
        }

		// ゲームステートごとの処理
		switch (gameState)
        {
			case GameState.Ready:

				// いずれかのキーが押されたら、ゲームを開始する
				if (Input.anyKey)
                {
					GameStart();
                }

				break;

			case GameState.Play:

				// プレイヤーが死亡したら、ゲームオーバーステートに移行する
				if (player.IsDead())
                {
					GameOver();
                }
				break;

			default:
				break;
        }
    }

	/// <summary>
	/// ゲーム開始前の準備状態の処理
	/// </summary>
	void GameReady()
    {
		gameState = GameState.Ready;
    }

	/// <summary>
	/// ゲーム開始時に行う処理
	/// </summary>
	void GameStart()
    {

		// ゲームステートのTextを空文字にする
		if (gameStateLabelText != null)
        {
			gameStateLabelText.text = "";
        }

		// 敵を生成する
		for (int i = 0; i < enemyPointList.Length; i++)
		{
			enemyPointList[i].CreateEnemy();
		}

		// ステージBGMを再生する
		stageBGM.Play();

		// ゲームステートをPlayに移行する
		gameState = GameState.Play;
	}

	/// <summary>
	/// ゲームオーバー時の処理
	/// </summary>
	void GameOver()
    {
		gameState = GameState.GameOver;

		// ゲームステートのTextをゲームオーバーにする
		if (gameStateLabelText != null)
		{
			gameStateLabelText.text = "Game Over\nPress R key to continue";
		}
	}

}
