namespace DTML.EduBot.Extensions
{
    using System;

    public static class Require
    {
        public static void NotNull(this object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}