# Coach Bot
A bot to provide International Online Soccer with matchmaking functionality, as well as other utilities.

### Compiling
This bot requires:
- .NET Core SDK 2.1

### Contributing
To develop, you need:
- Visual Studio 2017 or alternatively Visual Studio Code

### Running
To run the bot, you need .NET Core 2.1 Runtime installed and an app/bot set up on the Discord website (https://discordapp.com/developers/applications/me).

1. Rename sampleconfig.json to config.json, and add your bot's token from your Discord app/bot account
2. Navigate into the CoachBot directory via Command Line
3. Run the command `dotnet run`
4. The bot should now be running, and you can invite it to your server via the authorization link for your bot (https://discordapp.com/oauth2/authorize?client_id=BOT_CLIENT_ID_HERE&scope=bot)
