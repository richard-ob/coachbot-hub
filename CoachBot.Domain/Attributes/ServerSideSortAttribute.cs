using System;

namespace CoachBot.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ServerSideSort : Attribute
    {

    }
}
