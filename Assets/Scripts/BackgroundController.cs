using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    // スクロールする速さ
    [SerializeField]
    float scrollSpeed = 0.1f;

    MeshRenderer meshRenderer = new MeshRenderer();

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

 
    void Update()
    {
        // テクスチャの移動先を計算する
        // Time.time == ゲーム開始からの時間(秒)
        // Mathf.Repeat 第1引数が0～第2引数の間でループする 剰余演算子に似ている
        float y = Mathf.Repeat(scrollSpeed * Time.time, 1.0f);

        Vector2 offset = new Vector2(0, y);

        // マテリアルのテクスチャを位置を変更する
        meshRenderer.material.SetTextureOffset("_MainTex", offset);
        
    }
}
