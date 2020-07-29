until dotnet CoachBot.DiscordBot.dll
do
    echo "CoachBot.DiscordBot crashed with exit code $?.  Respawning.." >&2
    sleep 1
done