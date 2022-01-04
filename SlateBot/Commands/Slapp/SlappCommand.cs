using CsHelper;
using Discord;
using SlateBot.Language;
using SplatTagCore;
using SplatTagDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SlateBot.Commands.Slapp
{
  public class SlappCommand : Command
  {
    private static readonly SplatTagController splatTagController;
    private static readonly SplatTagJsonDatabase jsonDatabase;
    private static readonly MultiDatabase splatTagDatabase;
    private static readonly GenericFilesImporter multiSourceImporter;
    private static readonly string saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SplatTag");
    private static readonly Task initialiseTask;

    private readonly LanguageHandler languageHandler;
    private readonly IAsyncResponder asyncResponder;

    static SlappCommand()
    {
      // Initialise Slapp
      try
      {
        // Construct the database
        jsonDatabase = new SplatTagJsonDatabase(saveFolder);
        multiSourceImporter = new GenericFilesImporter(saveFolder);
        splatTagDatabase = new MultiDatabase(saveFolder, jsonDatabase, multiSourceImporter);

        // Construct the controller.
        splatTagController = new SplatTagController(splatTagDatabase);

        initialiseTask = Task.Run(() =>
        {
          // Load the database
          splatTagController.Initialise();
        });
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Exception: " + ex.ToString());
      }
    }

    internal SlappCommand(LanguageHandler languageHandler, IAsyncResponder asyncResponder, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Slapp, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
      this.asyncResponder = asyncResponder;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      // Responds asynchronously.
      Task.Run(async () =>
      {
        var now = DateTime.UtcNow;
        ServerSettings serverSettings = senderDetail.ServerSettings;
        CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
        string query = command.CommandDetail;

        Player[] matchedPlayers;
        Team[] matchedTeams;
        string title;

        // Call the Slapp
        if (string.IsNullOrWhiteSpace(query))
        {
          matchedPlayers = new Player[0];
          matchedTeams = new Team[0];
          title = "Nothing to search!";
        }
        else
        {
          Task.WaitAll(initialiseTask);
          matchedPlayers = splatTagController.MatchPlayer(query);
          matchedTeams = splatTagController.MatchTeam(query);
          title = $"Found {matchedPlayers.Length} player{((matchedPlayers.Length == 1) ? "" : "s")} and {matchedTeams.Length} team{((matchedTeams.Length == 1) ? "" : "s")}! ";
        }

        bool hasMatchedPlayers = matchedPlayers.Length != 0;
        bool hasMatchedTeams = matchedTeams.Length != 0;
        bool showLimitedMessage = matchedPlayers.Length > 9 || matchedTeams.Length > 9;

        Color color =
          (hasMatchedPlayers && hasMatchedTeams) ? Color.Green :
          (hasMatchedPlayers && !hasMatchedTeams) ? Color.Blue :
          (!hasMatchedPlayers && hasMatchedTeams) ? Color.Gold :
          (!hasMatchedPlayers && !hasMatchedTeams) ? Color.Red :
          Color.DarkRed;

        var builder = new EmbedBuilder()
          .WithColor(color)
          .WithAuthor(author => author.WithName(title));

        if (hasMatchedPlayers)
        {
          for (int i = 0; i < matchedPlayers.Length && i < 9; i++)
          {
            var p = matchedPlayers[i];
            var teams = p.Teams.Select(id => splatTagController.GetTeamById(id));
            string oldTeams = teams.GetOldTeamsStrings();
            var currentTeam = teams.FirstOrDefault();
            string info = $"Plays for {currentTeam} {oldTeams}\n _{string.Join(", ", p.Sources)}_";

            var playerField = new EmbedFieldBuilder()
              .WithIsInline(false)
              .WithName(p.Name.Truncate(1000, ""))
              .WithValue(info.Truncate(1000, "…_"));
            builder.AddField(playerField);
          }
        }

        if (hasMatchedTeams)
        {
          for (int i = 0; i < matchedTeams.Length && i < 9; i++)
          {
            var t = matchedTeams[i];
            string[] players = t.GetTeamPlayersStrings(splatTagController);
            string divPhrase = t.GetBestTeamPlayerDivString(splatTagController);
            string info = $"{t.Div}. {divPhrase} Players: {string.Join(matchedTeams.Length == 1 ? ",\n" : ", ", players)}\n _{string.Join(", ", t.Sources)}_";

            var teamField = new EmbedFieldBuilder()
              .WithIsInline(false)
              .WithName($"{t.Tag} {t.Name}".Truncate(1000, ""))
              .WithValue(info.Truncate(1000, "…_"));
            builder.AddField(teamField);
          }
        }
        builder.WithFooter($"Fetched in {Math.Floor((DateTime.UtcNow - now).TotalMilliseconds)} milliseconds. {(showLimitedMessage ? "Only the first 9 results are shown for players and teams." : "")}",
          "https://media.discordapp.net/attachments/471361750986522647/758104388824072253/icon.png");

        Response asyncResponse = new Response
        {
          Embed = builder,
          ResponseType = ResponseType.Default,
          Message = ""
        };

        await asyncResponder.SendResponseAsync(args, asyncResponse).ConfigureAwait(false);
      });

      // Return out the lifecycle with no response.
      if (!initialiseTask.IsCompleted)
      {
        // Also react with a Sloth if the database hasn't loaded yet.
        return new[] { Response.WaitForAsync, Response.CreateFromReact(Emojis.TurtleUnicode) };
      }
      else
      {
        return new[] { Response.WaitForAsync };
      }
    }
  }
}