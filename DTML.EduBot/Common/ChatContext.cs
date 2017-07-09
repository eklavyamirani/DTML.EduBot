using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTML.EduBot.Common
{
    public class ChatContext
    {
        private static string UserName = "My Friend";

        public static string Username
        {
            get
            {
                return UserName;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    UserName = value;
                }

            }
        }
    }
}