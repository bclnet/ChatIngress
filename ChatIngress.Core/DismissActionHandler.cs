using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using System;
using System.Linq;
using System.Threading.Tasks;
using Message = SlackNet.WebApi.Message;

namespace ChatIngress
{
    public class DismissActionHandler : IBlockActionHandler<ButtonAction>
    {
        public static readonly string DISMISS = "dismiss";
        public static readonly string STAMP = "stamp";

        readonly ISlackApiClient _slack;

        public DismissActionHandler(ISlackApiClient slack) => _slack = slack;

        public async Task Handle(ButtonAction action, BlockActionRequest request)
        {
            if (action.ActionId != STAMP)
            {
                await _slack.Respond(request.ResponseUrl, new MessageUpdateResponse(new MessageResponse
                {
                    DeleteOriginal = true
                }), null);
                return;
            }
            // stamp
            try
            {
                var val = await _slack.Conversations.History(request.Channel.Id, request.Container.MessageTs, inclusive: true, limit: 1);
                var blocks = val.Messages.FirstOrDefault().Blocks;
                var message = new Message
                {
                    Blocks = blocks
                };
                await _slack.Respond(request.ResponseUrl, new MessageUpdateResponse(new MessageResponse
                {
                    Message = message,
                    ReplaceOriginal = true
                }), null);
            }
            catch (Exception e)
            {
                await _slack.Respond(request.ResponseUrl, new MessageUpdateResponse(new MessageResponse
                {
                    Message = e.ToResponse(),
                    //ReplaceOriginal = true,
                }), null);
            }
        }
    }
}
