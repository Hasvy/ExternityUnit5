namespace ExtUnit5.Helpers
{
    public static class EnumHelper
    {
        private static readonly Random _random = new Random();

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }
    }
}
