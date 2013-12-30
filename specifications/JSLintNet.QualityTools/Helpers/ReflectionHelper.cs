namespace JSLintNet.QualityTools.Helpers
{
    using System.Reflection;

    public static class ReflectionHelper
    {
        private static readonly BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static PropertyInfo GetProperty(object instance, string name)
        {
            return instance.GetType().GetProperty(name, MemberBindingFlags);
        }

        public static FieldInfo GetField(object instance, string name)
        {
            return instance.GetType().GetField(name, MemberBindingFlags);
        }

        public static T GetPropertyValue<T>(object instance, string name)
        {
            return (T)GetProperty(instance, name).GetValue(instance, null);
        }

        public static T GetFieldValue<T>(object instance, string name)
        {
            return (T)GetField(instance, name).GetValue(instance);
        }

        public static void SetPropertyValue<T>(object instance, string name, T value)
        {
            GetProperty(instance, name).SetValue(instance, value, null);
        }

        public static void SetFieldValue<T>(object instance, string name, T value)
        {
            GetField(instance, name).SetValue(instance, value);
        }
    }
}
