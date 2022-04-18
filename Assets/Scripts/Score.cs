using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    // Textコンポーネント
    Text scoreText = null;

    // スコア
    int score = 0;

    private void Start()
    {
        // Textコンポーネントを取得する
        scoreText = gameObject.GetComponent<Text>();

        // スコアを初期化する
        InitScorePoit();
    }

    private void Update()
    {
        // スコアを表示を更新する
        scoreText.text = "Score : " + score.ToString();
    }

    /// <summary>
    /// スコアを加算する
    /// </summary>
    public void AddScorePoint(int point)
    {
        score += point;
    }

    /// <summary>
    /// スコアを初期化する
    /// </summary>
    public void InitScorePoit()
    {
        score = 0;
    }
}
