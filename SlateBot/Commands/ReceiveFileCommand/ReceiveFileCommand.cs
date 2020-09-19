using CsHelper;
using Discord;
using Discord.WebSocket;
using SlateBot.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SlateBot.Commands.ReceiveFile
{
  public class ReceiveFileCommand : Command
  {
    private readonly SlateBotDAL dal;
    private readonly IAsyncResponder asyncResponder;
    private readonly CommandController commandController;

    internal ReceiveFileCommand(SlateBotDAL dal, CommandController commandController, IAsyncResponder asyncResponder)
      : base(CommandHandlerType.ReceiveFile, new string[] { "" }, "This mechanism allows Slate to load commands on the fly and receive external files.", "", ModuleType.BotAdmin)
    {
      this.dal = dal;
      this.asyncResponder = asyncResponder;
      this.commandController = commandController;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      UserSettings userSettings = senderDetail.UserSettings;

      // If the message has a file and was sent to us in private, save that file.
      if (Constants.IsBotOwner(userSettings.UserId)
         && args is SocketMessageWrapper socketMessage
         && socketMessage.IsPrivate
         && socketMessage.socketMessage is SocketUserMessage socketUserMessage)
      {
        if (socketUserMessage.Attachments.Count > 0 || socketUserMessage.Embeds.Count > 0)
        {
          Task.Run(async () =>
          {
            List<string> receivedFiles = new List<string>();
            foreach (var attachment in socketUserMessage.Attachments)
            {
              var result = await WebHelper.DownloadFile(attachment.Url).ConfigureAwait(false);
              if (result?.Item2 != null)
              {
                string path = Path.Combine(dal.receivedFilesFolder, attachment.Filename);
                await File.WriteAllBytesAsync(path, result.Item2).ConfigureAwait(false);
                receivedFiles.Add(path);
              }
            }

            foreach (var embed in socketUserMessage.Embeds)
            {
              if (embed.Image.HasValue)
              {
                var image = (EmbedImage)embed.Image;
                var result = await WebHelper.DownloadFile(image.Url).ConfigureAwait(false);
                if (result?.Item2 != null)
                {
                  string path = Path.Combine(dal.receivedFilesFolder, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff"));
                  await File.WriteAllBytesAsync(path, result.Item2).ConfigureAwait(false);
                  receivedFiles.Add(path);
                }
              }
            }

            try
            {
              // If sent with no command, scan the file if it's an xml
              // to check if the file is a new command.
              if (string.IsNullOrWhiteSpace(args.Message))
              {
                var commandFiles = receivedFiles.Select(file => dal.LoadCopySingleCommand(file));
                if (commandFiles.Any(f => f != null))
                {
                  // Reinitialise the command controller to ensure the new files are accepted.
                  commandController.Initialise();
                  await asyncResponder.SendResponseAsync(args, Response.CreateFromString("Reinitialising the commands.")).ConfigureAwait(false);
                }
                await asyncResponder.SendResponseAsync(args, Response.CreateFromReact(Emojis.DiscUnicode)).ConfigureAwait(false);
              }
              // Otherwise if sent with an update command, overwrite the program file
              else if (args.Message.Equals("update", StringComparison.OrdinalIgnoreCase) || args.Message.Equals(Emojis.DiscUnicode))
              {
                foreach (var received in receivedFiles)
                {
                  string fileName = Path.GetFileName(received);
                  string destination = Path.Combine(dal.programFolder, fileName);
                  if (File.Exists(destination))
                  {
                    File.Move(destination, Path.Combine(dal.programFolderOld, fileName), true);
                  }
                  File.Copy(received, destination);
                }
              }
            }
            catch (Exception ex)
            {
              dal.errorLogger.LogException(ex, Errors.ErrorSeverity.Error);
              dal.errorLogger.LogDebug("Error occurred in receiving file for update/transfer.");
            }
          });

          return new Response[] { Response.WaitForAsync };
        }
      }
      return Response.NoResponse;
    }
  }
}