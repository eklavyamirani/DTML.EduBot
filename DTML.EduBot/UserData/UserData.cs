namespace DTML.EduBot.UserData
{
    using DTML.EduBot.Common;
    using System;

    [Serializable]
    public class UserData
    {
        private Gamification.GamerProfile _gamerProfile = new Gamification.GamerProfile();

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserEnglishLevel{ get; set; }

        public string NativeLanguageIsoCode { get; set; } = MessageTranslator.DEFAULT_LANGUAGE;

        public Gamification.GamerProfile GamerProfile
        {
            get
            {
                if (_gamerProfile == null)
                {
                    _gamerProfile = new Gamification.GamerProfile();
                }

                return _gamerProfile;
            }
            set
            {
                _gamerProfile = value;
            }
        }
    }
}