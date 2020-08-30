using System;
using System.Collections.Generic;
using System.Linq;

using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Internal.Parsing
{
    /// <summary>
    /// Class IrcParser.
    /// </summary>
    public class IrcParser
    {
        readonly char[] tagSeperators = new[] { ';', '=' };

        /// <summary>
        /// Builds an IrcMessage from a raw string
        /// </summary>
        /// <param name="raw">Raw IRC message</param>
        /// <returns>IrcMessage object</returns>
        public IrcMessage ParseIrcMessage(string raw)
        {
            IrcCommand command = IrcCommand.Unknown;
            Dictionary<string, string> tagDict = new Dictionary<string, string>(16);
            string[] parameters = Array.Empty<string>();
            string prefix = String.Empty;

            int nextPart;

            if (raw[0] == '@') {
                // v3 tags

                nextPart = raw.IndexOf(' ');
                if (nextPart == -1)
                    goto end;

                var rawTags = raw[1..nextPart];
                string key = null;

                for (int x = 0;x <= rawTags.Length;x++) {
                    int index;
                    if (key == null)
                        index = rawTags.IndexOfAny(tagSeperators, x);
                    else
                        index = rawTags.IndexOf(';', x);

                    if (index == -1) {
                        if (key != null)
                            tagDict[key] = rawTags[x..];
                        else
                            tagDict[rawTags[x..]] = "1";
                        break;
                    } else {
                        if (rawTags[index] == '=')
                            key = rawTags[x..index];
                        else {
                            if (key != null) {
                                tagDict[key] = rawTags[x..index];
                                key = null;
                            } else
                                tagDict[rawTags[x..index]] = "1";
                        }
					}

                    x = index;
                }

                raw = raw[(nextPart + 1)..];
            }

            if (raw[0] == ':') {
                nextPart = raw.IndexOf(' ');
                if (nextPart == -1)
                    goto end;

                prefix = raw[1..nextPart];
                raw = raw[(nextPart + 1)..];
            }

            nextPart = raw.IndexOf(' ');
            string commandRaw = null;

            if (nextPart == -1)
                nextPart = raw.Length;

            commandRaw = raw[..nextPart];
            raw = raw[nextPart..];

            if (raw.Length != 0) {
                var trailingSeperator = raw.IndexOf(':');
                if (trailingSeperator == -1) {
                    parameters = raw[1..].Split(' ');
			    } else {
                    Span<int> seperIndexes = stackalloc int[32];
                    int i = 1;

                    for (int x = 1;x < raw.Length;x++) {
                        if (i >= 32)
                            goto end;

                        if (raw[x] == ' ') {
                            if (raw[x+1] == ':') {
                                seperIndexes[i++] = x++;
                                break;
                            } else
                                seperIndexes[i++] = x;
                        }
                    }
                    seperIndexes[i] = raw.Length;

                    parameters = new string[i];

                    for (int x = 0;x < i;x++) {
                        parameters[x] = raw[(seperIndexes[x]+1)..seperIndexes[x+1]];
				    }

                    if (parameters[^1][0] == ':')
                        parameters[^1] = parameters[^1][1..];
                }
            }
            
            switch (commandRaw) {
                case "PRIVMSG":
                    command = IrcCommand.PrivMsg;
                    break;
                case "NOTICE":
                    command = IrcCommand.Notice;
                    break;
                case "PING":
                    command = IrcCommand.Ping;
                    break;
                case "PONG":
                    command = IrcCommand.Pong;
                    break;
                case "HOSTTARGET":
                    command = IrcCommand.HostTarget;
                    break;
                case "CLEARCHAT":
                    command = IrcCommand.ClearChat;
                    break;
                case "CLEARMSG":
                    command = IrcCommand.ClearMsg;
                    break;
                case "USERSTATE":
                    command = IrcCommand.UserState;
                    break;
                case "GLOBALUSERSTATE":
                    command = IrcCommand.GlobalUserState;
                    break;
                case "NICK":
                    command = IrcCommand.Nick;
                    break;
                case "JOIN":
                    command = IrcCommand.Join;
                    break;
                case "PART":
                    command = IrcCommand.Part;
                    break;
                case "PASS":
                    command = IrcCommand.Pass;
                    break;
                case "CAP":
                    command = IrcCommand.Cap;
                    break;
                case "001":
                    command = IrcCommand.RPL_001;
                    break;
                case "002":
                    command = IrcCommand.RPL_002;
                    break;
                case "003":
                    command = IrcCommand.RPL_003;
                    break;
                case "004":
                    command = IrcCommand.RPL_004;
                    break;
                case "353":
                    command = IrcCommand.RPL_353;
                    break;
                case "366":
                    command = IrcCommand.RPL_366;
                    break;
                case "372":
                    command = IrcCommand.RPL_372;
                    break;
                case "375":
                    command = IrcCommand.RPL_375;
                    break;
                case "376":
                    command = IrcCommand.RPL_376;
                    break;
                case "WHISPER":
                    command = IrcCommand.Whisper;
                    break;
                case "SERVERCHANGE":
                    command = IrcCommand.ServerChange;
                    break;
                case "RECONNECT":
                    command = IrcCommand.Reconnect;
                    break;
                case "ROOMSTATE":
                    command = IrcCommand.RoomState;
                    break;
                case "USERNOTICE":
                    command = IrcCommand.UserNotice;
                    break;
                case "MODE":
                    command = IrcCommand.Mode;
                    break;
            }

end:
            return new IrcMessage(command, parameters, prefix, tagDict);
        }
    }
}
