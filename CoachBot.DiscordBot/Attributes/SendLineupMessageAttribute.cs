using System;

namespace CoachBot
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SendLineupMessage : Attribute
    {
    }
}