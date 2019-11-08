using CoachBot.Domain.Model;
using Discord;
using System.Text;

namespace CoachBot.Tools
{
    public static class EmbedTools
    {
        public static Embed GenerateSimpleEmbed(string content)
        {
            return new EmbedBuilder()
                .WithDescription(content)
                .Build(); 
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

            return new EmbedBuilder().WithCurrentTimestamp().WithDescription(message.ToString());
        }
    }
}
