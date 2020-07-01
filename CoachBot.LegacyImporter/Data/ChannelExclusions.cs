using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Data
{
    public static class ChannelExclusions
    {
        public static ulong[] Channels => new ulong[]
           {
                645678070426107904, // RT
                456901950315823104 // NTA
           };
    }
}
