using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// GameObject的对象池类
/// </summary>
public class GameObjectPool : PoolBase<GameObject>
{
    /// <summary>
    /// 初始化对象用的方法，这里Monobehavior与new方法冲突，因此需要外部传入
    /// </summary>
    readonly Func<GameObject, Transform, GameObject> Instantiate;
    /// <summary>
    /// 每次实例化的预制体
    /// </summary>
    GameObject prefab = null;
    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="prefab">用于管理的对象的预制体</param>
    /// <param name="Instantiate">Instantiate方法，因为new()和Monobehavior继承冲突</param>
    public GameObjectPool(GameObject prefab, Func<GameObject, Transform, GameObject> Instantiate) : base()
    {
        this.prefab = prefab;
        this.Instantiate = Instantiate;
    }
    /// <summary>
    /// 取得一个可以用的GameObject
    /// </summary>
    /// <param name="parent">父节点</param>
    /// <param name="position">对象的位置</param>
    /// <returns></returns>
    public GameObject Get(Transform parent, Vector3 position)
    {
        foreach (PoolStruct item in objects)
        {
            if (!item.IsUsing)
            {
                item.IsUsing = true;
                item.t.transform.SetParent(parent);
                item.t.transform.position = position;
                return item.t;
            }
        }
        PoolStruct s = new(Instantiate(prefab, parent), true);
        s.t.transform.position = position;
        objects.Add(s);
        return s.t;
    }
}
/// <summary>
/// 非GameObject对象的对象池
/// </summary>
/// <typeparam name="T">对象类型，需要满足new()约束条件</typeparam>
public class UnGameObjectPool<T> : PoolBase<T> where T : new()
{
    public UnGameObjectPool() : base() { }
    /// <summary>
    /// 获取一个新的
    /// </summary>
    /// <returns>获取到的T</returns>
    public T Get()
    {
        foreach (PoolStruct item in objects)
        {
            if (!item.IsUsing)
            {
                item.IsUsing = true;
                return item.t;
            }
        }
        PoolStruct s = new(new(), true);
        objects.Add(s);
        return s.t;
    }
}

/// <summary>
/// 对象池的基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolBase<T>
{
    /// <summary>
    /// 用于存储每一个T和对应状态的类
    /// </summary>
    public class PoolStruct
    {
        public T t;
        private bool isUsing;
        /// <summary>
        /// 当前块是否正在使用，如果不再使用将会进行隐藏
        /// </summary>
        public bool IsUsing
        {
            get
            {
                return this.isUsing;
            }
            set
            {
                this.isUsing = value;
                // 隐藏当前块
                if (typeof(T) == typeof(GameObject))
                {
                    (t as GameObject).SetActive(value);
                }
            }
        }
        public PoolStruct(T a, bool b)
        {
            this.t = a;
            this.IsUsing = b;
        }
    }

    /// <summary>
    /// 对象池中所有的对象
    /// </summary>
    protected List<PoolStruct> objects;
    protected PoolBase()
    {
        objects = new();
    }
    /// <summary>
    /// 移除指定的内容
    /// 这里会进行隐藏显示
    /// </summary>
    /// <param name="g">指定的内容</param>
    /// <returns>是否隐藏成功</returns>
    public bool Remove(T item)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i].t.Equals(item))
            {
                objects[i].IsUsing = false;
                return true;
            }
        }
        return false;
    }
}
