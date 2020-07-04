using System;

namespace CoachBot.LegacyImporter.Model
{
    public class LegacyPosition
    {
        public LegacyPosition()
        {
        }

        public LegacyPosition(string position)
        {
            PositionName = position;
        }

        public Guid Id { get; set; }
        public string PositionName { get; set; }
    }
}