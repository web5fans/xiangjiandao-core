using System;
using System.Reflection;

namespace Xiangjiandao.Web.Tests.Extensions;

public static class EntityExtensions
{
    public static void SetId<T, TKey>(this T entity, TKey value) where T : class
    {
        // 获取实体类型
        Type entityType = entity.GetType();
        // 尝试获取名为 "Id" 的属性
        PropertyInfo? idProperty = entityType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        if (idProperty != null && idProperty.CanWrite)
        {
            // 检查属性类型是否与 value 参数类型匹配
            if (idProperty.PropertyType == typeof(TKey))
            {
                // 为 Id 属性赋值
                idProperty.SetValue(entity, value);
            }
            else
            {
                throw new InvalidOperationException("属性类型与值类型不匹配。");
            }
        }
        else
        {
            throw new InvalidOperationException("未找到可写的 'Id' 属性。");
        }
    }
}