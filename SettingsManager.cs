using System.Reflection;

namespace Featherline;

public class Setting : Attribute
{
    public string Input;
    public Setting() => Input ??= "";
}

public class SettingsManager
{
    public const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private object settings;
    private MemberInfo[] fields;
    private (object container, MemberInfo? member)[] targets;

    public SettingsManager(object container, object target)
    {
        settings = container;

        var settLs = new List<MemberInfo>();
        var targetLs = new List<(object, MemberInfo?)>();

        foreach (var field in container.GetType().GetFields().Concat<MemberInfo>(container.GetType().GetProperties())) {
            var attr = field.GetCustomAttribute<Setting>();
            if (attr != null) {
                var chain = attr.Input.Split('.');
                var fieldCont = target;

                foreach (var step in chain[..^1])
                    fieldCont = fieldCont?.GetMember(step);

                settLs.Add(field);
                targetLs.Add((fieldCont, fieldCont.GetMemberInfo(chain[^1])));
            }
        }

        fields = settLs.ToArray();
        targets = targetLs.ToArray();
    }

    public void Load()
    {
        for (int i = 0; i < fields.Length; i++) {
            var value = fields[i].GetValue(settings);
            var input = targets[i].member;
            if (input.VariableType() == typeof(decimal))
                value = Convert.ToDecimal(value);
            input.SetValue(targets[i].container, value);
        }
    }

    public void Save()
    {
        for (int i = 0; i < fields.Length; i++) {
            var inputVal = targets[i].member?.GetValue(targets[i].container);
            var setting = fields[i];
            if (inputVal is decimal)
                inputVal = Convert.ChangeType(inputVal, setting.VariableType());
            setting.SetValue(settings, inputVal);
        }
    }
}

public static class MemberInfoExtension
{
    public static object? GetValue(this MemberInfo info, object source) =>
        info is FieldInfo fi ? fi.GetValue(source) : ((PropertyInfo)info).GetValue(source);

    public static void SetValue(this MemberInfo info, object target, object? value)
    {
        if (info is FieldInfo fi)
            fi.SetValue(target, value);
        else
            ((PropertyInfo)info).SetValue(target, value);
    }

    public static MemberInfo? GetMemberInfo(this object obj, string name)
    {
        var type = obj.GetType();
        return type.GetField(name, SettingsManager.flags) ?? (type.GetProperty(name, SettingsManager.flags) as MemberInfo);
    }

    public static object? GetMember(this object obj, string name) => obj.GetMemberInfo(name)?.GetValue(obj);

    public static Type? VariableType(this MemberInfo info) => info switch {
        FieldInfo fi => fi.FieldType.UnderlyingSystemType,
        PropertyInfo pi => pi.PropertyType.UnderlyingSystemType,
        _ => null
    };
}
