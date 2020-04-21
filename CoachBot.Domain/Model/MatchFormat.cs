using System.ComponentModel;

namespace CoachBot.Domain.Model
{
    public enum MatchFormat
    {
        [Description("1v1")]
        OneVsOne = 1,
        [Description("2v2")]
        TwoVsTwo,
        [Description("3v4")]
        ThreeVsThree,
        [Description("4v4")]
        FourVsFour,
        [Description("5v5")]
        FiveVsFive,
        [Description("6v6")]
        SixVsSix,
        [Description("7v7")]
        SevenVsSeven,
        [Description("8v8")]
        EightVsEight,
        [Description("9v9")]
        NineVsNine,
        [Description("10v10")]
        TenVsTen,
        [Description("11v11")]
        ElevenVsEleven
    }
}
