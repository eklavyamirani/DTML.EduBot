namespace DTML.EduBot.Extensions
{
    public static class Set
    {
        public static void FieldNotNull<T>(T argument, string argumentName, out T target)
        {
            argument.NotNull(argumentName);
            target = argument;
        }
    }
}