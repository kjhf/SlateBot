# SlateBot

### Help
  - [Help page](https://splatoonwiki.org/wiki/Blog:KJ_Bot/Commands)
  - [Development server](https://discord.gg/Px5Bhny)

If you'd like to help, come talk to me on Discord in the development server or submit a pull request or issue report if you're that way inclined.

### Summary
Slate Bot is a C# General Purpose / Splatoon-orientated Discord Bot built with [Discord.NET](https://github.com/RogueException/Discord.Net).

### Licensing
Slate Bot is provided as-is and this source code is meant as a reference. You may pick out bits for your own project, just don't pass off the whole project as your own, kay?

### TODO
- Implement the following modules left over from KJ_Bot in priority order:
    - Convert (General - Converts given measurement into other units (whatever makes sense))
    - Info (BotOwner - Displays information about a specified id or command)
    - Rate (Admin - Toggles the channel as a Rate channel where the bot will react with a thumbsup & down)
    - Clean (Admin - cleans n messages)
    - DecodeEncode (Decode/Encode - Decodes/Encodes a string. A little work to tie into a pre-existing project.)
    - Users (Admin - collates user id data and roles for all the users on the server)
    - Wave (Image - Manipulate an image by shifting rows by a random amount.)
    - ChangeServerSetting (Admin - changes server settings e.g. language. Large amount of work. Is needed to use French.)
    - Manipulate (Image - Manipulate an image. This work is huge.)
    - RemindMe (Scheduler - this was kinda working...ish. Needs a lot of thought.)
    - Add (BotOwner - Add a game, meme, response, or other command to the bot. Not needed, is a nicety.)
- Better detection for a dice command -- it currently conflicts with other commands e.g. b642d
- Overhaul commands DAL into JSON -- this greatly simplifies the DAL layers and allows commands to extend ExtraData.
