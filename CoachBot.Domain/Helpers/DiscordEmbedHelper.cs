using CoachBot.Domain.Model;
using CoachBot.Shared.Extensions;
using Discord;
using System.Text;

namespace CoachBot.Tools
{
    public static class DiscordEmbedHelper
    {
        private const uint DEFAULT_EMBED_COLOUR = 1131364;

        public static Embed GenerateSimpleEmbed(string content)
        {
            return GenerateSimpleEmbed(content, null);
        }

        public static Embed GenerateSimpleEmbed(string content, string title = null)
        {
            var embedBuilder = new EmbedBuilder()
                .WithCurrentTimestamp()
                .WithDescription(content)
                .WithRequestedBy()
                .WithDefaultColour();

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
            var embedBuilder = new EmbedBuilder();

            if (!serviceResponse.Message.StartsWith(':'))
            {
                switch (serviceResponse.Status)
                {
                    case ServiceResponseStatus.Warning:
                        message.Append(":warning: ");
                        embedBuilder.WithColor(new Color(255, 204, 77));
                        break;

                    case ServiceResponseStatus.Success:
                        message.Append(":white_check_mark: ");
                        embedBuilder.WithColor(new Color(119, 178, 85));
                        break;

                    case ServiceResponseStatus.Failure:
                        message.Append(":no_entry: ");
                        embedBuilder.WithColor(new Color(190, 25, 49));
                        break;

                    case ServiceResponseStatus.NegativeSuccess:
                        message.Append(":negative_squared_cross_mark: ");
                        embedBuilder.WithColor(new Color(119, 178, 85));
                        break;

                    case ServiceResponseStatus.Info:
                        message.Append(":information_source: ");
                        embedBuilder.WithColor(new Color(87, 164, 175));
                        break;

                    default:
                        embedBuilder.WithDefaultColour();
                        break;
                }
            }
            else
            {
                embedBuilder.WithDefaultColour();
            }

            message.Append(serviceResponse.Message);

            embedBuilder
                .WithCurrentTimestamp()
                .WithDescription(message.ToString())
                .WithRequestedBy();

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

        public static EmbedBuilder WithDefaultColour(this EmbedBuilder embedBuilder)
        {
            return embedBuilder.WithColor(new Color(DEFAULT_EMBED_COLOUR));
        }
    }
}