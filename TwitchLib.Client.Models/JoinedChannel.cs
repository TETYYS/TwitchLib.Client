﻿namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a joined channel.</summary>
    public class JoinedChannel
    {        
        /// <summary>The current channel the TwitcChatClient is connected to.</summary>
        public string Channel { get; }
        /// <summary>Object representing current state of channel (r9k, slow, etc).</summary>
        public ChannelState ChannelState { get; protected set; }

        /// <summary>JoinedChannel object constructor.</summary>
        public JoinedChannel(string channel)
        {
            Channel = channel;
        }
    }
}
