until dotnet CoachBot.API.dll
do
    echo "CoachBot.API crashed with exit code $?.  Respawning.." >&2
    sleep 1
done
