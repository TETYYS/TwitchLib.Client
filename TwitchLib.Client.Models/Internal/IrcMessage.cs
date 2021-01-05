using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Enums.Internal;

namespace TwitchLib.Client.Models.Internal
{
    public class IrcMessage
    {
        private string Raw;

        /// <summary>
        /// The channel the message was sent in
        /// </summary>
        public string Channel {
            get {
                if (Parameters?.Length == 0)
                    return "";

                return Parameters[0].StartsWith("#") ? Parameters[0][1..] : Parameters[0];
            }
        }

        /// <summary>
        /// Message itself
        /// </summary>
        public string Message => Trailing;

        public string Trailing => Parameters != null && Parameters.Length > 1 ? Parameters[Parameters.Length - 1] : "";

        /// <summary>
        /// Command parameters
        /// </summary>
        public string[] Parameters { get; }

        /// <summary>
        /// The user whose message it is
        /// </summary>
        public readonly string User;

        /// <summary>
        /// Hostmask of the user
        /// </summary>
        public readonly string Hostmask;

        /// <summary>
        /// Raw Command
        /// </summary>
        public readonly IrcCommand Command;

        /// <summary>
        /// IRCv3 tags
        /// </summary>
        public readonly Dictionary<string, string> Tags;

        /// <summary>
        /// Create an INCOMPLETE IrcMessage only carrying username
        /// </summary>
        /// <param name="user"></param>
        public IrcMessage(string user)
        {
            Parameters = null;
            User = user;
            Hostmask = null;
            Command = IrcCommand.Unknown;
            Tags = null;
        }

        /// <summary>
        /// Create an IrcMessage
        /// </summary>
        /// <param name="command">IRC Command</param>
        /// <param name="parameters">Command params</param>
        /// <param name="hostmask">User</param>
        /// <param name="tags">IRCv3 tags</param>
        public IrcMessage(string raw, IrcCommand command, string[] parameters, string hostmask, Dictionary<string, string> tags = null)
        {
            var idx = hostmask.IndexOf('!');
            User = idx != -1 ? hostmask.Substring(0, idx) : hostmask;
            Hostmask = hostmask;
            Parameters = parameters;
            Command = command;
            Tags = tags;

            if (raw == null)
                Raw = GenerateRaw();
		    else
                Raw = raw;
        }

        private string GenerateRaw()
		{
            var raw = new StringBuilder(32);
            if (Tags != null)
            {
                var tags = new string[Tags.Count];
                var i = 0;
                foreach (var tag in Tags)
                {
                    tags[i] = tag.Key + "=" + tag.Value;
                    ++i;
                }
                if (tags.Length > 0)
                {
                    raw.Append("@").Append(string.Join(";", tags)).Append(" ");
                }
            }
            if (!string.IsNullOrEmpty(Hostmask))
            {
                raw.Append(":").Append(Hostmask).Append(" ");
            }
            raw.Append(Command.ToString().ToUpper().Replace("RPL_", ""));
            if (Parameters.Length <= 0) return raw.ToString();

            for (var x = 0;x < Parameters.Length - 1;x++) {
                raw.Append(" ").Append(Parameters[x]);
            }

            var lastIndex = Parameters.Length - 1;
            if (Parameters[lastIndex].Contains(" "))
                raw.Append(" :").Append(Parameters[lastIndex]);
            else
                raw.Append(" ").Append(Parameters[lastIndex]);

            return raw.ToString();
		}

        public override string ToString()
        {
            return Raw;
        }
    }
}