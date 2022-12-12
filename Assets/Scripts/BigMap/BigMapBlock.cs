using UnityEngine;
/// <summary>
/// 每个格子自身的行为
/// 点击事件等
/// </summary>
public class BigMapBlock : MonoBehaviour
{
    #region 测试代码
    TextMesh Text;
    private void Awake()
    {
        Text = this.transform.Find("Text").GetComponent<TextMesh>();
    }
    public void SetText(string msg)
    {
        this.Text.text = msg;
    }
    #endregion

    // TODO 每个格子自身的行为
}
