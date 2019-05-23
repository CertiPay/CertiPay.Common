using System;

namespace CertiPay.Common.Notifications
{
    [Obsolete(message: "HipChat is no longer in service")]
    public class HipChatNotification : Notification
    {
        public static string QueueName { get; } = "HipChatNotifications";

        public String From { get; set; }

        public Boolean Notify { get; set; } = false;

        /// <summary>
        /// Defaults to yellow
        /// </summary>
        public TextColor Color { get; set; } = TextColor.yellow;

        /// <summary>
        /// Defaults to Html
        /// </summary>
        public MessageFormat Format { get; set; } = MessageFormat.text;

        public enum MessageFormat
        {
            /// <summary>
            /// Message is rendered at HTML and receives no special treatment.
            /// Must be valid HTMl and entities must be escaped. May contain basic tags:
            /// a, b, i, strong, em, br, img, pre, code, lists, tables.
            /// </summary>
            html,

            /// <summary>
            /// Message is treated just like a message sent by a user. Can include @mentions, emoticons,
            /// pastes, and auto-detected URLs.
            /// </summary>
            text
        }

        public enum TextColor { yellow, green, red, purple, gray, random }
    }
}
