
/// <summary>
/// 大地图的各种参数
/// </summary>
public class BigMapConfigs
{
    /// <summary>
    /// 一个屏幕中显示的行数（有多少行格子）
    /// </summary>
    public static int OneScreenRow { get; private set; } = 10;
    /// <summary>
    /// 一个屏幕中显示的列数（每行有多少个格子）
    /// </summary>
    public static int OneScreenCol { get; private set; } = 5;
    /// <summary>
    /// 大地图共有多少行
    /// </summary>
    public static int MapRow { get; private set; } = 1000;
    /// <summary>
    /// 大地图共有多少列
    /// </summary>
    public static int MapCol { get; private set; } = 1000;
    /// <summary>
    /// 大地图共有多少行屏幕大小
    /// </summary>
    public static int MapScreenRow { get; private set; } = MapRow / OneScreenRow;
    /// <summary>
    /// 大地图共有多少列屏幕大小
    /// </summary>
    public static int MapScreenCol { get; private set; } = MapCol / OneScreenCol;
    /// <summary>
    /// 最大物体半径
    /// </summary>
    public static float StarRadius { get; private set; } = 0.5f;
    /// <summary>
    /// 每个地图格子的宽度
    /// 星球直径的二倍
    /// </summary>
    public static float BlockWidth { get; private set; } = 4 * StarRadius;
    /// <summary>
    /// 每个地图格子的长度
    /// 星球直径的二倍
    /// </summary>
    public static float BlockHeight { get; private set; } = 4 * StarRadius;

}
