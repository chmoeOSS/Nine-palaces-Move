
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 保存一个屏幕下的所有block
/// 包括数据对应的BigMapBLockData
/// 和每个格子对应的预制体的引用
/// </summary>
public class BigMapOneScreenBlocks
{
    /// <summary>
    /// 保存该区域的所有物体的引用
    /// </summary>
    public List<List<GameObject>> blocks;
    /// <summary>
    /// 保存该区域所有物体的数据
    /// </summary>
    public List<List<BigMapBlockData>> datas;
    /// <summary>
    /// 当前屏幕属于地图的第X行屏幕
    /// </summary>
    public int X { get; set; }
    /// <summary>
    /// 当前屏幕属于地图的第Y列屏幕
    /// </summary>
    public int Y { get; set; }
    /// <summary>
    /// 是否已经初始化过
    /// </summary>
    private bool isInited { get; set; } = false;
    /// <summary>
    /// [对象池调用]构造方法(初始化两个list)
    /// </summary>
    public BigMapOneScreenBlocks()
    {
        // 没有进行初始化才进行初始化，否则是重复使用，已经在Clear方法中清零过了。
        if (!isInited)
        {
            blocks = new();
            datas = new();
            for (int i = 0; i < BigMapConfigs.OneScreenRow; i++)
            {
                List<GameObject> tmpb = new();
                List<BigMapBlockData> tmpd = new();
                for (int j = 0; j < BigMapConfigs.OneScreenCol; j++)
                {
                    tmpb.Add(null);
                    tmpd.Add(null);
                }
                blocks.Add(tmpb);
                datas.Add(tmpd);
            }
            isInited = true;
        }
    }
    /// <summary>
    /// [外部调用]构造方法(初始化两个list)
    /// </summary>
    /// <param name="x">当前屏幕内容属于第x行</param>
    /// <param name="y">当前屏幕坐标属于第y列</param>
    public BigMapOneScreenBlocks SetPosition(int x, int y)
    {
        this.X = x;
        this.Y = y;
        return this;
    }
    /// <summary>
    /// 清空所有List(归零)
    /// </summary>
    /// <param name="blockPool">物体的池</param>
    /// <param name="blockDataPool">物体数据的池</param>
    public void Clear(GameObjectPool blockPool, UnGameObjectPool<BigMapBlockData> blockDataPool)
    {
        for (int i = 0; i < BigMapConfigs.OneScreenRow; i++)
        {
            for (int j = 0; j < BigMapConfigs.OneScreenCol; j++)
            {
                blockPool.Remove(blocks[i][j]);
                blocks[i][j] = null;
                blockDataPool.Remove(datas[i][j]);
                datas[i][j] = null;
            }
        }
    }

    #region 坐标换算相关
    /// <summary>
    /// 获取某个block对应的大地图的真实坐标
    /// </summary>
    /// <param name="x">当前屏幕块中的第x行</param>
    /// <param name="y">当前屏幕块中的第y列</param>
    /// <returns>指定block对应大地图的真实坐标</returns>
    public BigMapScreensManager.RectangularCoordinates GetBigMapMapCoordinates(int x, int y)
    {
        return new(x + X * BigMapConfigs.OneScreenRow, y + Y * BigMapConfigs.OneScreenCol);
    }
    /// <summary>
    /// 左上角对应大地图的真实坐标
    /// </summary>
    public BigMapScreensManager.RectangularCoordinates LeftUpTrueCoordinates
    {
        get
        {
            return GetBigMapMapCoordinates(0, 0);
        }
    }
    /// <summary>
    /// 右下角对应大地图的真实坐标
    /// </summary>
    public BigMapScreensManager.RectangularCoordinates RightDownTrueCoordinates
    {
        get
        {
            return GetBigMapMapCoordinates(BigMapConfigs.OneScreenRow - 1, BigMapConfigs.OneScreenCol - 1);
        }
    }

    /// <summary>
    /// 左上角对应大地图的真实坐标
    /// </summary>
    public BigMapScreensManager.RectangularCoordinates FirstTrueCoordinates
    {
        get
        {
            return GetBigMapMapCoordinates(0, 0);
        }
    }
    /// <summary>
    /// 右下角对应大地图的真实坐标
    /// </summary>
    public BigMapScreensManager.RectangularCoordinates LastTrueCoordinates
    {
        get
        {
            return GetBigMapMapCoordinates(BigMapConfigs.OneScreenRow - 1, BigMapConfigs.OneScreenCol - 1);
        }
    }
    #endregion 坐标换算相关
}
