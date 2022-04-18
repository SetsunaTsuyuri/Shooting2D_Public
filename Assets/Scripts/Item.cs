using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    offenseUp,
    offenseDown,
    DefenseUp,
    DefenseDown
}

public class Item : MonoBehaviour
{
    // アイテムタイプ
    [SerializeField]
    ItemType itemType = ItemType.offenseUp;

    // 移動速度
    [SerializeField]
    float moveSpeed = 5.0f;

    Vector3 moveDir = Vector3.zero;
    Rect screenRect = new Rect();

    /// <summary>
    /// 初期化処理
    /// 最初から配置されているアイテムに必要
    /// </summary>
    public void Init(Rect screenRect)
    {
        this.screenRect = screenRect;
    }

    private void Update()
    {
        // 移動処理
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // 画面外に行ったら削除する
        if (screenRect.y < transform.position.y)
        {
            Destroy(gameObject);
        }
        else if (screenRect.height > transform.position.y)
        {
            Destroy(gameObject);
        }
        else if (screenRect.x > transform.position.x)
        {
            Destroy(gameObject);
        }
        else if (screenRect.width < transform.position.x)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 移動開始
    /// </summary>
    public void StartMove(Vector3 createPosition, Vector3 moveDir, Rect screenRect)
    {
        this.moveDir = moveDir;
        this.screenRect = screenRect;

        transform.position = createPosition;
        transform.rotation = Quaternion.FromToRotation(Vector3.zero, moveDir);
    }

    /// <summary>
    /// アイテムタイプを取得する
    /// </summary>
    public ItemType GetItemType()
    {
        return itemType;
    }

}
