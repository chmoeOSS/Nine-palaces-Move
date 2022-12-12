using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 所有块的父节点
/// 1. 控制移动
/// 2. 控制动态生成和隐藏
/// </summary>
public class BigMapBlocksParent : MonoBehaviour
{
    #region 测试用代码部分
    /// <summary>
    /// 每一个块的预制体
    /// </summary>
    public GameObject blockPrefab;
    /// <summary>
    /// 相机
    /// </summary>
    public Camera Camera;
    #endregion 测试用代码部分

    #region 正式代码部分
    /// <summary>
    /// 块的对象池(GameObject)
    /// </summary>
    GameObjectPool blockPool;
    /// <summary>
    /// 块的数据对象池(BigMapBlockData)
    /// </summary>
    UnGameObjectPool<BigMapBlockData> blockDataPool;

    #region 移动相关
    /// <summary>
    /// 是否正在移动地图
    /// </summary>
    bool startMove = false;
    /// <summary>
    /// 点击的点和地图中心点的差值
    /// </summary>
    Vector3 clickedDeltaPosition;
    /// <summary>
    /// 当前位置和起始点的差值
    /// </summary>
    Vector3 deltaWithStartPosition;
    /// <summary>
    /// 地图的起始点
    /// </summary>
    Vector3 startPosition;
    #endregion 移动相关

    # endregion 正式代码部分

    void Start()
    {
        // 初始化块的对象池
        blockPool = new(blockPrefab, Instantiate);
        // 初始化块数据的对象池
        blockDataPool = new();
        // 记录初始位置
        startPosition = this.transform.position;
        // 九宫格管理器初始化, 初始化第一个屏幕
        BigMapScreensManager.Instance.SetStart(0, 0).Init(blockPool, blockDataPool, this.transform, startPosition);
    }
    // Update is called once per frame
    void Update()
    {
        MoveMap();
        this.deltaWithStartPosition = this.transform.position - this.startPosition;
    }

    /// <summary>
    /// 移动相关代码
    /// </summary>
    void MoveMap()
    {
        Vector3 Mouseposition = Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 17));
        Mouseposition.y = 0;
        if (!startMove && Input.GetMouseButton(0))
        {
            startMove = true;
            clickedDeltaPosition = this.transform.position - Mouseposition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            startMove = false;
            // 检测是否需要加载
            NeedLoadCheck();
        }
        if (startMove)
        {
            // TODO 限制移动的范围
            this.transform.position = Mouseposition + clickedDeltaPosition;
        }
    }
    /// <summary>
    /// 检查是否需要加载周围的格子
    /// </summary>
    void NeedLoadCheck()
    {
        // 水平方向、竖直方向分别需要移动多少格子
        int x, y;
        x = (int)(deltaWithStartPosition.x / BigMapConfigs.BlockWidth);
        y = (int)(deltaWithStartPosition.z / BigMapConfigs.BlockHeight);
        // 通知该屏幕加载周围剩下的屏幕
        BigMapScreensManager.Instance.GenerateNineScreenAtPosition(x, y);
    }

}
