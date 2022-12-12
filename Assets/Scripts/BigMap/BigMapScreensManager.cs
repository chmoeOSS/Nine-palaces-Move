using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 九宫格的管理类，单例
/// </summary>
public class BigMapScreensManager:Singleton<BigMapScreensManager>
{
    /// <summary>
    /// 请勿删除, 单例公共类必须要写一个私有构造方法，防止报错
    /// </summary>
    BigMapScreensManager() { }


    /// <summary>
    /// 块的对象池(GameObject)
    /// </summary>
    GameObjectPool blockPool;
    /// <summary>
    /// 块的数据对象池(BigMapBlockData)
    /// </summary>
    UnGameObjectPool<BigMapBlockData> blockDataPool;
    /// <summary>
    /// 九宫格中每个屏幕的数据管理类
    /// </summary>
    UnGameObjectPool<BigMapOneScreenBlocks> screenBlocksPool;
    /// <summary>
    /// 所有格子的父级节点
    /// </summary>
    Transform parent;
    /// <summary>
    /// 起始坐标第X行的屏幕
    /// </summary>
    public int X { get; private set; } = 0;
    /// <summary>
    /// 起始坐标第Y列的屏幕
    /// </summary>
    public int Y { get; private set; } = 0;
    /// <summary>
    /// 起始位置的第一个块的坐标
    /// </summary>
    Vector3 StartScreenFirstBlockPosition
    {
        get
        {
            return new(this.parent.position.x - (BigMapConfigs.OneScreenCol - 1) / 2.0f * BigMapConfigs.BlockWidth, 0
                , this.parent.position.z + (BigMapConfigs.OneScreenRow - 1) / 2.0f * BigMapConfigs.BlockHeight);
        }
    }

    /// <summary>
    /// [结构体]屏幕坐标(x,y)和中心点position
    /// </summary>
    struct Coordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly Vector3 center;
        public Coordinate(int x, int y, Vector3 center) : this()
        {
            this.x = x;
            this.y = y;
            this.center = center;
        }
    }
    /// <summary>
    /// Int类型的二维坐标
    /// </summary>
    public struct RectangularCoordinates
    {
        public readonly int x, y;
        public RectangularCoordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        // 重载判断运算
        public static bool operator ==(RectangularCoordinates lhs, RectangularCoordinates rhs)
        {
            bool status = false;
            if (lhs.x == rhs.x && lhs.y == rhs.y)
            {
                status = true;
            }
            return status;
        }
        // 重载判断运算
        public static bool operator !=(RectangularCoordinates lhs, RectangularCoordinates rhs)
        {
            bool status = false;
            if (lhs.x != rhs.x || lhs.y != rhs.y)
            {
                status = true;
            }
            return status;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    /// <summary>
    /// [属性]主城地图中的所有屏幕数据
    /// </summary>
    public List<List<BigMapOneScreenBlocks>> Screens
    {
        get
        {
            if (null == this.screens)
            {
                this.Init();
            }
            return this.screens;
        }
        set
        {
            this.screens = value;
        }
    }
    /// <summary>
    /// [变量]主城地图中的所有屏幕数据
    /// </summary>
    private List<List<BigMapOneScreenBlocks>> screens;

    /// <summary>
    /// 内部初始化方法
    /// </summary>
    private void Init()
    {
        // 保存地图所有屏幕的List
        this.Screens = new();
        for (int i = 0; i < BigMapConfigs.MapScreenRow; i++)
        {
            List<BigMapOneScreenBlocks> tmp = new();
            for (int j = 0; j < BigMapConfigs.MapScreenCol; j++)
            {
                tmp.Add(null);
            }
            this.Screens.Add(tmp);
        }
    }
    /// <summary>
    /// 设置起始坐标
    /// </summary>
    /// <param name="x">起始坐标x</param>
    /// <param name="y">起始坐标Y</param>
    /// <returns>脚本自身</returns>
    public BigMapScreensManager SetStart(int x, int y)
    {
        this.X = x;
        this.Y = y;
        return this;
    }
    /// <summary>
    /// 外部初始化方法
    /// </summary>
    /// <param name="blockPool">格子的对象池</param>
    /// <param name="blockDataPool">格子数据的对象池</param>
    /// <param name="parent">所有格子的父级节点</param>
    /// <param name="startPosition">初始位置</param>
    /// <returns>脚本自身</returns>
    public BigMapScreensManager Init(GameObjectPool blockPool, UnGameObjectPool<BigMapBlockData> blockDataPool, Transform parent, Vector3 startPosition)
    {
        // 初始化管理单个屏幕数据的对象池
        this.screenBlocksPool = new();
        // 设置所有块的对象池
        this.blockPool = blockPool;
        // 设置所有块数据的对象池
        this.blockDataPool = blockDataPool;
        // 设置父级节点
        this.parent = parent;
        #region 初始化的方法，后续可能会在其他地方调用
        InitGenerateBlocks(startPosition, X, Y);
        // 生成周边
        GenerateArround(this.Screens[X][Y]);
        #endregion 初始化的方法
        return this;
    }
    /// <summary>
    /// 生成指定位置所在的区域的所有block，从左上角开始生成
    /// </summary>
    /// <param name="centerPosition">需要生成的区域的中心点</param>
    /// <param name="X">当前区域所在的行数</param>
    /// <param name="Y">当前区域所在的列数</param>
    /// <returns>返回一个屏幕的数据</returns>
    BigMapOneScreenBlocks InitGenerateBlocks(Vector3 centerPosition, int X, int Y)
    {
        // 计算左上角的位置
        float x, y, z; // unity中的坐标，x为向右，z为向上
        x = centerPosition.x - ((BigMapConfigs.OneScreenCol - 1) / 2.0f * BigMapConfigs.BlockWidth);
        y = 0;
        z = centerPosition.z + ((BigMapConfigs.OneScreenRow - 1) / 2.0f * BigMapConfigs.BlockHeight);
        // 生成一个屏幕的数据
        BigMapOneScreenBlocks oneScreenBlocks = this.screenBlocksPool.Get().SetPosition(X, Y);
        for (int i = 0; i < BigMapConfigs.OneScreenRow; i++)
        {
            for (int j = 0; j < BigMapConfigs.OneScreenCol; j++)
            {
                oneScreenBlocks.blocks[i][j] = blockPool.Get(this.parent, new(x + j * BigMapConfigs.BlockWidth, y, z - i * BigMapConfigs.BlockHeight));
                var pos = oneScreenBlocks.GetBigMapMapCoordinates(i, j);
                // TODO 设置资源行为
                oneScreenBlocks.blocks[i][j].GetComponent<BigMapBlock>().SetText($"{pos.x}, {pos.y}");
                // TODO 设置资源数据
                oneScreenBlocks.datas[i][j] = blockDataPool.Get();
            }
        }
        this.Screens[X][Y] = oneScreenBlocks;
        return oneScreenBlocks;
    }
    /// <summary>
    /// 判断RectangularCoordinates数组中是否存在某个指定的RectangularCoordinates
    /// </summary>
    /// <param name="a">RectangularCoordinates的List</param>
    /// <param name="x">待判断的RectangularCoordinates的x值</param>
    /// <param name="y">待判断的RectangularCoordinates的y值</param>
    /// <returns>是否存在</returns>
    bool InNeedShowScreen(List<RectangularCoordinates> a, int x, int y)
    {
        foreach (RectangularCoordinates item in a)
        {
            if (x == item.x && y == item.y)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// [外部]根据当前移动的格子数生成其所在的屏幕和周围的屏幕
    /// </summary>
    /// <param name="x">水平方向移动了多少格子</param>
    /// <param name="y">垂直方向移动了多少格子</param>
    public void GenerateNineScreenAtPosition(int x, int y)
    {
        // 检测格子所在的屏幕
        var needLoadScreen = GetNeedLoadScreenCoordinate(x, y);
        x = needLoadScreen.x;
        y = needLoadScreen.y;
        // 取得当前应该显示的屏幕和其周围一圈的屏幕对应坐标(X, Y)
        List<RectangularCoordinates> needShowScreens = GetAroundScreenCoordinate(x, y);
        // 计算哪些需要进行移除并且移除对应的格子
        for (int i = 0; i < BigMapConfigs.MapScreenRow; i++)
        {
            for (int j = 0; j < BigMapConfigs.MapScreenCol; j++)
            {
                // 是本次需要显示的 保留
                if (InNeedShowScreen(needShowScreens, i, j))
                {
                    continue;
                }
                // 不是本次需要显示的 应当清除
                else
                {
                    if (null != this.Screens[i][j])
                    {
                        this.Screens[i][j].Clear(blockPool, blockDataPool);
                        this.screenBlocksPool.Remove(this.Screens[i][j]);
                        this.Screens[i][j] = null;
                    }
                }
            }
        }
        // 当前位置有则生成周围, 没有则先生成当前位置, 然后再生成周围
        GenerateArround(this.Screens[x][y] ?? InitGenerateBlocks(GetScreenPosition(x, y), x, y));
    }
    /// <summary>
    /// 获取指定屏幕对应的位置
    /// </summary>
    /// <param name="row">指定屏幕的x行</param>
    /// <param name="col">指定屏幕的y列</param>
    /// <returns></returns>
    Vector3 GetScreenPosition(int row, int col)
    {
        float x, y, z;
        x = this.StartScreenFirstBlockPosition.x + (col - Y) * BigMapConfigs.OneScreenCol * BigMapConfigs.BlockWidth + (BigMapConfigs.OneScreenCol - 1) / 2.0f * BigMapConfigs.BlockWidth;
        y = this.StartScreenFirstBlockPosition.y;
        z = this.StartScreenFirstBlockPosition.z - (row - X) * BigMapConfigs.OneScreenRow * BigMapConfigs.BlockHeight - (BigMapConfigs.OneScreenRow - 1) / 2.0f * BigMapConfigs.BlockHeight;

        return new(x, y, z);
    }

    /// <summary>
    /// [内部]生成当前块周围的所有块
    /// </summary>
    /// <param name="currentBlocks">当前屏幕的块</param>
    void GenerateArround(BigMapOneScreenBlocks currentBlocks)
    {
        /* 计算需要生成周围的哪些快
         * 1. 可能存在边界位置
         * 2. 可能存在已经生成的位置
         */
        // 计算周围需要生成的块
        List<Coordinate> aroundPositions = GetAroundScreenCoordinate(currentBlocks);
        // 根据当前区域中的第一个（左上角）进行计算
        // 计算出该区域周围其他区域的位置
        // 调用InitGenerateBlocks()方法生成每个区域
        foreach (var item in aroundPositions)
        {
            InitGenerateBlocks(item.center, item.x, item.y);
        }
    }
    /// <summary>
    /// 查找指定位置的周围是否有可以创建的位置
    /// </summary>
    /// <param name="block">指定的块</param>
    /// <returns></returns>
    List<Coordinate> GetAroundScreenCoordinate(BigMapOneScreenBlocks block)
    {
        int x = block.X, y = block.Y;
        List<Coordinate> result = new();
        // 上一行
        if (x - 1 >= 0)
        {
            // 左边
            if (y - 1 >= 0 && null == this.Screens[x - 1][y - 1])
            {
                result.Add(new(x - 1, y - 1, GetScreenPosition(x - 1, y - 1)));
            }
            // 中间
            if (null == this.Screens[x - 1][y])
            {
                result.Add(new(x - 1, y, GetScreenPosition(x - 1, y)));
            }
            // 右边
            if (y + 1 < BigMapConfigs.MapScreenCol && null == this.Screens[x - 1][y + 1])
            {
                result.Add(new(x - 1, y + 1, GetScreenPosition(x - 1, y + 1)));
            }
        }
        // 本行
        {
            // 左边
            if (y - 1 >= 0 && null == this.Screens[x][y - 1])
            {
                result.Add(new(x, y - 1, GetScreenPosition(x, y - 1)));
            }
            // 右边
            if (y + 1 < BigMapConfigs.MapScreenCol && null == this.Screens[x][y + 1])
            {
                result.Add(new(x, y + 1, GetScreenPosition(x, y + 1)));
            }
        }
        // 下一行
        if (x + 1 < BigMapConfigs.MapScreenRow)
        {
            // 左边
            if (y - 1 >= 0 && null == this.Screens[x + 1][y - 1])
            {
                result.Add(new(x + 1, y - 1, GetScreenPosition(x + 1, y - 1)));
            }
            // 中间
            if (null == this.Screens[x + 1][y])
            {
                result.Add(new(x + 1, y, GetScreenPosition(x + 1, y)));
            }
            // 右边
            if (y + 1 < BigMapConfigs.MapScreenCol && null == this.Screens[x + 1][y + 1])
            {
                result.Add(new(x + 1, y + 1, GetScreenPosition(x + 1, y + 1)));
            }
        }
        return result;
    }
    /// <summary>
    /// 获取指定屏幕和周围的一圈的屏幕坐标的列表
    /// </summary>
    /// <param name="x">指定屏幕的行</param>
    /// <param name="y">指定屏幕的列</param>
    /// <returns>一个存有坐标的List</returns>
    List<RectangularCoordinates> GetAroundScreenCoordinate(int x, int y)
    {
        List<RectangularCoordinates> result = new();
        // 上一行
        if (x - 1 >= 0)
        {
            // 左边
            if (y - 1 >= 0)
            {
                result.Add(new(x - 1, y - 1));
            }
            // 中间
            result.Add(new(x - 1, y));
            // 右边
            if (y + 1 < BigMapConfigs.MapScreenCol)
            {
                result.Add(new(x - 1, y + 1));
            }
        }
        // 本行
        {
            // 左边
            if (y - 1 >= 0)
            {
                result.Add(new(x, y - 1));
            }
            // 自己
            result.Add(new(x, y));
            // 右边
            if (y + 1 < BigMapConfigs.MapScreenCol)
            {
                result.Add(new(x, y + 1));
            }
        }
        // 下一行
        if (x + 1 < BigMapConfigs.MapScreenRow)
        {
            // 左边
            if (y - 1 >= 0)
            {
                result.Add(new(x + 1, y - 1));
            }
            // 中间
            result.Add(new(x + 1, y));
            // 右边
            if (y + 1 < BigMapConfigs.MapScreenCol)
            {
                result.Add(new(x + 1, y + 1));
            }
        }
        return new();
    }
    /// <summary>
    /// 根据当前移动的格子数获取其对应的九宫格屏幕坐标
    /// </summary>
    /// <param name="x">水平方向移动了多少格子</param>
    /// <param name="y">垂直方向移动了多少格子</param>
    /// <returns>当前显示的屏幕坐标x和y</returns>
    private RectangularCoordinates GetNeedLoadScreenCoordinate(int x, int y)
    {
        // 初始化为最开始的地方
        int resY = Y, resX = X;
        // 父级向左移动(显示更加右边的内容，屏幕数增大)
        if (x < 0)
        {
            resY += Mathf.Abs(x) / BigMapConfigs.OneScreenCol + (Mathf.Abs(x) % BigMapConfigs.OneScreenCol > 0 ? 1 : 0);
            if (resY >= BigMapConfigs.MapScreenCol)
            {
                resY = BigMapConfigs.MapScreenCol - 1;
            }
        }
        // 父级向右移动(显示更小的内容)
        else if (x > 0)
        {
            resY -= Mathf.Abs(x) / BigMapConfigs.OneScreenCol + (Mathf.Abs(x) % BigMapConfigs.OneScreenCol > 0 ? 1 : 0);
            if (resY < 0)
            {
                resY = 0;
            }
        }
        // 父级向上移动（显示更大的内容）
        if (y > 0)
        {
            resX += Mathf.Abs(y) / BigMapConfigs.OneScreenRow + (Mathf.Abs(x) % BigMapConfigs.OneScreenRow > 0 ? 1 : 0);
            if (resX >= BigMapConfigs.MapScreenRow)
            {
                resX = BigMapConfigs.MapScreenRow - 1;
            }
        }
        // 父级向下移动（显示更小的内容）
        else if (y < 0)
        {
            resX -= Mathf.Abs(y) / BigMapConfigs.OneScreenRow + (Mathf.Abs(x) % BigMapConfigs.OneScreenRow > 0 ? 1 : 0);
            if (resX < 0)
            {
                resX = 0;
            }
        }
        return new(resX, resY);
    }


}
