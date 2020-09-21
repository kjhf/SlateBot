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
        string message = "";
        ServerSettings serverSettings = senderDetail.ServerSettings;
        CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
        string query = command.CommandDetail;
        Color color = Color.Green;

        Player[] matchedPlayers;
        Team[] matchedTeams;

        // Call the Slapp
        if (string.IsNullOrWhiteSpace(query))
        {
          message = "Nothing to search!";
          color = Color.Red;
          matchedPlayers = new Player[0];
          matchedTeams = new Team[0];
        }
        else
        {
          Task.WaitAll(initialiseTask);
          matchedPlayers = splatTagController.MatchPlayer(query);
          matchedTeams = splatTagController.MatchTeam(query);
          message = $"Found {matchedPlayers.Length} players and {matchedTeams.Length} teams! ";

          bool appended = false;
          if (matchedPlayers.Length > 3)
          {
            matchedPlayers = matchedPlayers.Take(3).ToArray();
            message += "First three results shown.";
            appended = true;
          }

          if (matchedTeams.Length > 3)
          {
            matchedTeams = matchedTeams.Take(3).ToArray();

            if (!appended)
            {
              message += "First three results shown.";
            }
          }
        }

        IEnumerable<string> playerStrings =
          matchedPlayers.Length == 0 ?
          new string[1] { "(None)" } :
          matchedPlayers.Select(p =>
          {
            var teams = p.Teams.Select(id => splatTagController.GetTeamById(id));
            string oldTeams = teams.GetOldTeamsStrings();
            var currentTeam = teams.FirstOrDefault();
            return $"{p.Name} (Plays for {currentTeam} {oldTeams}";
          })
        ;

        IEnumerable<string> teamStrings =
          matchedTeams.Length == 0 ?
          new string[1] { "(None)" } :
          matchedTeams.Select(t =>
          {
            string[] players = t.GetTeamPlayersStrings(splatTagController);
            string divPhrase = t.GetBestTeamPlayerDivString(splatTagController);
            return $"{t.Tag} {t.Name} ({t.Div}). {divPhrase} Players: {string.Join(", ", players)}";
          })
        ;

        var playersField = matchedPlayers.Length > 0 ?
          new EmbedFieldBuilder()
            .WithIsInline(true)
            .WithName("Players")
            .WithValue(string.Join("\n\n", playerStrings))
          : null;

        var teamsField =
          new EmbedFieldBuilder()
            .WithIsInline(true)
            .WithName("Teams")
            .WithValue(string.Join("\n\n", teamStrings))
          ;

        var builder = new EmbedBuilder()
          .WithColor(color)
          .WithAuthor(author => author.WithName(message));

        if (playersField != null)
        {
          builder.AddField(playersField);
          message += $"\nPlayers:\n{string.Join('\n', playerStrings)}\n";
        }

        // If there are teams or there are no players (or both) -- we always want at least one field.
        if (matchedTeams.Length > 0 || playersField == null)
        {
          builder.AddField(teamsField);
          message += $"\nTeams:\n{string.Join('\n', teamStrings)}";
        }

        Response asyncResponse = new Response
        {
          Embed = builder,
          ResponseType = ResponseType.Default,
          Message = message
        };

        await asyncResponder.SendResponseAsync(args, asyncResponse).ConfigureAwait(false);
      });

      // Return out the lifecycle with no response.
      if (!initialiseTask.IsCompleted)
      {
        // Also react with a Sloth if the database hasn't loaded yet.
        return new[] { Response.WaitForAsync, Response.CreateFromReact(Emojis.SlothUnicode) };
      }
      else
      {
        return new[] { Response.WaitForAsync };
      }
    }
  }
}