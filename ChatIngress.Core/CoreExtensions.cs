using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatIngress
{
    public static class CoreExtensions
    {
        public static DateTime ToStartOfWeek(this DateTime source, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            var diff = (7 + (source.DayOfWeek - startOfWeek)) % 7;
            return source.AddDays(-1 * diff).Date;
        }

        public static IList<IActionElement> Dismiss => new[]
        {
            new Button { ActionId = DismissActionHandler.DISMISS, Text = new PlainText("Dismiss Message"), Style = ButtonStyle.Danger, }
        };

        public static IList<IActionElement> DismissAndStamp => new[]
        {
            new Button { ActionId = DismissActionHandler.DISMISS, Text = new PlainText("Dismiss Message"), Style = ButtonStyle.Danger, },
            //new Button { ActionId = DismissActionHandler.STAMP, Text = new PlainText("Stamp Message"), Style = ButtonStyle.Danger, }
        };

        public static Message AckMessage => new()
        {
            Blocks = new[] { new Markdown($"ok, got that").ToSectionBlock() },
        };

        public static Message ToResponse(this Exception source) =>
             new Message
             {
                 Text = source is SlackException slack && slack.ErrorMessages.Count > 0
                 ? string.Join("\n", slack.ErrorMessages)
                 : source.Message
             }.AddDismissAttachment();

        public static Message AddDismissAttachment(this Message message, bool canStamp = false)
        {
            var elements = canStamp ? DismissAndStamp : Dismiss;
            if (message.Attachments?.Count > 0)
            {
                var lastAttachment = message.Attachments.LastOrDefault(a => a.Blocks != null && a.Blocks.Any(b => b.GetType() == typeof(ActionsBlock)));
                if (lastAttachment == null)
                    message.Attachments = message.Attachments.AsEnumerable().Append(new Attachment
                    {
                        Blocks = new[] { new ActionsBlock { Elements = elements } }
                    }).ToList();
                else
                {
                    var attachIndex = message.Attachments.IndexOf(lastAttachment);
                    var lastActionBlock = (ActionsBlock)lastAttachment.Blocks.LastOrDefault(b => b.GetType() == typeof(ActionsBlock));
                    var actionBlockIndex = lastAttachment.Blocks.IndexOf(lastActionBlock);
                    lastActionBlock.Elements = lastActionBlock.Elements.Concat(elements).ToList();
                    lastAttachment.Blocks[actionBlockIndex] = lastActionBlock;
                    message.Attachments[attachIndex] = lastAttachment;
                }
            }
            else
                message.Attachments = new[] { new Attachment
                {
                    Blocks = new[] { new ActionsBlock { Elements = elements } }
                }};
            return message;
        }
    }
}
