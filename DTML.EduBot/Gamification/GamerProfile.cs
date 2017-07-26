using DTML.EduBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DTML.EduBot.Gamification
{
    [Serializable]
    public class GamerProfile
    {
        public ulong Points { get; set; }

        public List<string> Badges { get; set; } = new List<string>();

        public GamerProfile()
        {

        }
    }

    // TODO: Repository of all badges
    // This also checks if you can get a badge
    public class BadgeRepository
    {
        public List<Badge> Badges { get; set; } = new List<Badge>()
        {
            new Badge
            {
                Title = "Level 1",
                Description = "Achieved when you complete your first lesson.",
                Points = 10
            },
            new Badge
            {
                Title = "Level 2",
                Description = "Achieved when you complete your second lesson.",
                Points = 20
            },
            new Badge
            {
                Title = "Level 3",
                Description = "Achieved when you complete your third lesson.",
                Points = 30
            }
        };

        public List<string> GetEligibleBadges(GamerProfile profile, ulong startingSeedPoints)
        {
            Require.NotNull(profile, nameof(profile));
            var badges = Badges.Where(badge => badge.Points > startingSeedPoints && badge.Points <= profile.Points)
                .Select(badge => badge.Title);

            return badges != null ? badges.ToList() : new List<string>();
        }
    }

    public class Badge
    {
        public ulong Points { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string BadgeUri { get; set; }
    }
}