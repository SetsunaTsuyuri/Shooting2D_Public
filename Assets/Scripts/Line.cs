using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Line : MonoBehaviour
{
    /// <summary>
    /// 線の太さ
    /// </summary>
    [SerializeField]
    float _lineSize = 0.1f;

    /// <summary>
    /// 線の位置リスト
    /// </summary>
    readonly List<Vector3> _positionList = new();

    /// <summary>
    /// 頂点座標リスト
    /// </summary>
    readonly List<Vector3> _vertexList = new();

    /// <summary>
    /// UV座標リスト
    /// </summary>
    readonly List<Vector2> _uvList = new();

    /// <summary>
    /// インデックスリスト
    /// </summary>
    readonly List<int> _indexList = new();

    /// <summary>
    /// UV座標オフセット
    /// </summary>
    float _uvOffset = 0.0f;

    /// <summary>
    /// インデックスオフセット
    /// </summary>
    int _indexOffset = 0;

    /// <summary>
    /// メッシュ
    /// </summary>
    Mesh _mesh = null;

    /// <summary>
    /// メッシュフィルター
    /// </summary>
    MeshFilter _meshFilter = null;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    /// <summary>
    /// 初期化する
    /// </summary>
    private void Initialize()
    {
        _positionList.Clear();
        _vertexList.Clear();
        _uvList.Clear();
        _indexList.Clear();
        _mesh = new();
    }

    /// <summary>
    /// 線引きを開始する
    /// </summary>
    /// <param name="position"></param>
    public void StartLine(Vector3 position)
    {
        Initialize();

        _positionList.Add(position);

        _vertexList.Add(position);
        _vertexList.Add(position);

        _uvList.Add(new(0.0f, 1.0f));
        _uvList.Add(new(0.0f, 0.0f));

        _indexOffset = 0;
    }

    /// <summary>
    /// 線を更新する
    /// </summary>
    /// <param name="position"></param>
    public void UpdateLine(Vector3 position)
    {
        _positionList.Add(position);

        // 直前の位置
        int previousPositionIndex = _positionList.Count - 2;
        Vector2 previousPosition = _positionList[previousPositionIndex];

        // 現在の位置
        int currentPositionIndex = _positionList.Count - 1;
        Vector2 currentPosition = _positionList[currentPositionIndex];

        // 線を引く方向
        Vector2 direction = (currentPosition - previousPosition).normalized;

        Vector2 minus90Position = currentPosition + new Vector2(direction.y, -direction.x) * _lineSize;
        Vector2 plus90Position = currentPosition + new Vector2(-direction.y, direction.x) * _lineSize;

        // 頂点座標
        _vertexList.Add(minus90Position);
        _vertexList.Add(plus90Position);

        // UV座標
        _uvList.Add(new(_uvOffset, 0.0f));
        _uvList.Add(new(_uvOffset, 1.0f));
        _uvOffset += (currentPosition - previousPosition).magnitude;

        // インデックス
        _indexList.Add(_indexOffset);
        _indexList.Add(_indexOffset + 1);
        _indexList.Add(_indexOffset + 2);
        _indexList.Add(_indexOffset + 1);
        _indexList.Add(_indexOffset + 3);
        _indexList.Add(_indexOffset + 2);
        _indexOffset += 2;

        // メッシュ更新
        _mesh.vertices = _vertexList.ToArray();
        _mesh.uv = _uvList.ToArray();
        _mesh.triangles = _indexList.ToArray();
        _meshFilter.sharedMesh = _mesh;
    }

    /// <summary>
    /// 位置を取得する
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosition()
    {
        Vector3 result = Vector3.zero;
        if (_positionList.Count > 0)
        {
            result = _positionList[0];
        }

        return result;
    }
}
