using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エフェクトの制御クラス
/// </summary>
public class Effect : MonoBehaviour
{
	SpriteRenderer spriteRenderer = null;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		// 画像描画が有効ではなくなったら、ゲームオブジェクトを破棄する
		if (spriteRenderer.enabled == false)
		{
			Destroy(gameObject);
		}
	}

}
