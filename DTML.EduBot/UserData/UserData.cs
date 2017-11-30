namespace DTML.EduBot.UserData
{
    using System;

    [Serializable]
    public class UserData
    {
        private Gamification.GamerProfile _gamerProfile;

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string NativeLanguageIsoCode { get; set; }

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