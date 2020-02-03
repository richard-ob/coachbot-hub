using CoachBot.Domain.Model;
using CoachBot.Extensions;
using Discord;
using System.Text;

namespace CoachBot.Tools
{
    public static class EmbedTools
    {
        public static Embed GenerateSimpleEmbed(string content)
        {
            return GenerateSimpleEmbed(content, null);
        }

        public static Embed GenerateSimpleEmbed(string content, string title = null)
        {
            var embedBuilder = new EmbedBuilder().WithCurrentTimestamp().WithDescription(content).WithRequestedBy();

            if (!string.IsNullOrEmpty(title))
            {
                embedBuilder.WithTitle(title);
            }

            return embedBuilder.Build();
        }

        public static Embed GenerateEmbed(string content, ServiceResponseStatus status)
        {
            return GenerateEmbedFromServiceResponse(new ServiceResponse(status, content));
        }

        public static Embed GenerateEmbedFromServiceResponse(ServiceResponse serviceResponse)
        {
            var message = new StringBuilder();

            switch(serviceResponse.Status)
            {
                case ServiceResponseStatus.Success:
                    message.Append(":white_check_mark: ");
                    break;
                case ServiceResponseStatus.Failure:
                    message.Append(":no_entry: ");
                    break;
                case ServiceResponseStatus.NegativeSuccess:
                    message.Append(":negative_squared_cross_mark: ");
                    break;
                case ServiceResponseStatus.Info:
                    message.Append(":information_source: ");
                    break;
            }

            message.Append(serviceResponse.Message);

            var embedBuilder =  new EmbedBuilder().WithCurrentTimestamp().WithDescription(message.ToString()).WithRequestedBy();

            return embedBuilder.Build();
        }

        public static EmbedBuilder WithRequestedBy(this EmbedBuilder embedBuilder)
        {
            if (CallContext.GetData(CallContextDataType.DiscordUser) != null)
            {
                embedBuilder.WithFooter(new EmbedFooterBuilder().WithText("Requested by " + CallContext.GetData(CallContextDataType.DiscordUser)));
            }

            return embedBuilder;
        }
    }
}
