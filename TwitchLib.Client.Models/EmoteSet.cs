using System;
using System.Collections.Generic;

namespace TwitchLib.Client.Models
{
    /// <summary>Object representing emote set from a chat message.</summary>
    public class EmoteSet
    {
        /// <summary>List containing all emotes in the message.</summary>
        public List<Emote> Emotes { get; }
        /// <summary>The raw emote set string obtained from Twitch, for legacy purposes.</summary>
        public string RawEmoteSetString { get; }

        /// <summary>Constructor for ChatEmoteSet object.</summary>
        /// <param name="emoteSetData"></param>
        /// <param name="message"></param>
        public EmoteSet(string emoteSetData, string message)
        {
            Emotes = new List<Emote>();
            RawEmoteSetString = emoteSetData;
            if (string.IsNullOrEmpty(emoteSetData))
                return;

            foreach (var emoteData in emoteSetData.Split('/'))
            {
                var imageIdSeperatorIndex = emoteData.IndexOf(':');
                if (imageIdSeperatorIndex == -1)
                    continue;

                var emoteId = emoteData.Substring(0, imageIdSeperatorIndex);

                // Get rid of emote ID and : at the start
                foreach (var emote in emoteData.Substring(imageIdSeperatorIndex + 1).Split(','))
                    AddEmote(emote, emoteId, message);
            }
        }

        private void AddEmote(string emoteData, string emoteId, string message)
        {
            // emoteData: 43-49
            // emoteId: 451521

            var split = emoteData.Split('-');

            if (split.Length != 2)
                return;

			if (!Int32.TryParse(split[0], out var startIndex))
				return;
			if (!Int32.TryParse(split[1], out var endIndex))
                return;

            if (endIndex >= message.Length)
                return;

            Emotes.Add(new Emote(emoteId, message.Substring(startIndex, endIndex - startIndex + 1), startIndex, endIndex));
        }

        /// <summary>
        /// Object representing an emote in an EmoteSet in a chat message.
        /// </summary>
        public class Emote
        {
            /// <summary>Twitch-assigned emote Id.</summary>
            public string Id { get; }
            /// <summary>The name of the emote. For example, if the message was "This is Kappa test.", the name would be 'Kappa'.</summary>
            public string Name { get; }
            /// <summary>Character starting index. For example, if the message was "This is Kappa test.", the start index would be 8 for 'Kappa'.</summary>
            public int StartIndex { get; }
            /// <summary>Character ending index. For example, if the message was "This is Kappa test.", the start index would be 12 for 'Kappa'.</summary>
            public int EndIndex { get; }
            /// <summary>URL to Twitch hosted emote image.</summary>
            public string ImageUrl { get; }

            /// <summary>
            /// Emote constructor.
            /// </summary>
            /// <param name="emoteId"></param>
            /// <param name="name"></param>
            /// <param name="emoteStartIndex"></param>
            /// <param name="emoteEndIndex"></param>
            public Emote(string emoteId, string name, int emoteStartIndex, int emoteEndIndex)
            {
                Id = emoteId;
                Name = name;
                StartIndex = emoteStartIndex;
                EndIndex = emoteEndIndex;
                ImageUrl = $"https://static-cdn.jtvnw.net/emoticons/v1/{emoteId}/1.0";
            }
        }
    }
}
