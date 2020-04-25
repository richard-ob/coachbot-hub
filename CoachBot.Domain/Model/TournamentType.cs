using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public enum TournamentType
    {
        Knockout,
        RoundRobin,
        DoubleRoundRobin,
        KnockoutWithSeeding,
        RoundRobinLadder,
        DoubleRoundRobinLadder,
        RoundRobinAndKnockout,
        DoubleRoundRobinAndKnockout
    }
}
