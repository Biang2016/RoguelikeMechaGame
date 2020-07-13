using UnityEngine;

/// <summary>
/// 用于自定义显示Hierarchy和Project视图
/// </summary>
public class AssetIconDescAttribute : System.Attribute
{
    private System.Type m_classType;
    private string m_desc;

    /// <summary>
    /// 构造自定义显示的属性
    /// </summary>
    /// <param name="_classType">对应的monobehavior组件类</param>
    public AssetIconDescAttribute(System.Type _classType) : this(_classType, _classType.Name)
    {

    }

    /// <summary>
    /// 构造自定义显示的属性
    /// </summary>
    /// <param name="_classType">对应的monobehavior组件类</param>
    /// <param name="_desc">自定义显示描述</param>
    public AssetIconDescAttribute(System.Type _classType, string _desc)
    {
        m_classType = _classType;
        m_desc = _desc;
    }

    public System.Type ClassType
    {
        get { return m_classType; }
    }

    public string Desc
    {
        get { return m_desc; }
    }
}