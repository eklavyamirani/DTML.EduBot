using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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