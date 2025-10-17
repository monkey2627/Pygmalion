using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Component = UnityEngine.Component;

public static class Utils 
{
    public static Dictionary<string, string> ParseLine(string line)
    {
        // ������ʽƥ�� [��ǩ ����=ֵ]
        Regex tagRegex = new Regex(@"\[(.*?)\]");
        Match match = tagRegex.Match(line);

        if (match.Success)
        {
            string tagContent = match.Groups[1].Value.Trim();
            Dictionary<string, string> tagAttributes = new Dictionary<string, string>();

            // �ָ��ǩ���ݣ��Կո�Ϊ�ָ���
            string[] parts = tagContent.Split(' ');
            string tagName = parts[0]; // ��һ�������Ǳ�ǩ��
            tagAttributes["tag"] = tagName;

            // ��������������
            for (int i = 1; i < parts.Length; i++)
            {
                string attribute = parts[i];
                if (attribute.Contains("="))
                {
                    string[] keyValue = attribute.Split(new char[] { '=' }, 2);
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        tagAttributes[key] = value;
                    }
                }
            }

            return tagAttributes;
        }

        return null; // ���û��ƥ�䵽��ǩ������ null
    }
    public static void ModifyField(Component component, string fieldName, object newValue)
    {
        Type componentType = component.GetType();
        FieldInfo fieldInfo = componentType.GetField(
            fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(component, newValue);
        }
        else
        {
            Debug.LogError("Field " + fieldName + " not found in " + componentType.Name);
        }
    }
    public static object InvokeMethod(Component comp, string methodName, params object[] args)
    {
        Type t = comp.GetType();
        var flags = BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static;

        MethodInfo[] methods = t.GetMethods(flags).Where(m => m.Name == methodName).ToArray();
        MethodInfo target = methods.Length == 1
            ? methods[0]
            : methods.FirstOrDefault(m =>
            {
                ParameterInfo[] pi = m.GetParameters();
                if (pi.Length != (args?.Length ?? 0)) return false;
                for (int i = 0; i < pi.Length; i++)
                    if (args[i] != null && !pi[i].ParameterType.IsInstanceOfType(args[i]))
                        return false;
                return true;
            }) ?? throw new InvalidOperationException("找不到匹配的重载");

        return target.Invoke(comp, args);
    }
    public static void ModifyProperty(Component component, string propertyName, object newValue)
    {
        // ��ȡ���������
        Type componentType = component.GetType();
        Debug.Log(propertyName);
        Debug.Log(componentType.Name);

        // ʹ�� BindingFlags ���������Ի��ֶ�
        PropertyInfo propertyInfo = componentType.GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        );
        if (propertyInfo != null)
        {
            // ��������Ƿ��� setter
            if (propertyInfo.CanWrite)
            {
                // �޸�����ֵ
                propertyInfo.SetValue(component, newValue);
            }
            else
            {
                Debug.LogError("Property " + propertyName + " is read-only and cannot be modified.");
            }
        }
        else
        {
            Debug.LogError("Property " + propertyName + " not found in " + componentType.Name);
        }
    }
    public static Transform FindChildInTransform(Transform parent, string child)
    {
        if (parent.name == child)
            return parent;
        Transform childTF = parent.Find(child);
        if (childTF != null)
            return childTF;
        for (int i = 0; i < parent.childCount; ++i)
        {
            childTF = FindChildInTransform(parent.GetChild(i), child);
            if (childTF != null)
                return childTF;
        }
        return childTF;
    }
}
