using SlackNet;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreValidation.Bindings
{
    public class ViewInfoBinding : AbstractBinding
    {
        public static AbstractBinding Default = new ViewInfoBinding();

        ViewInfoBinding() { }

        public Dictionary<string, object> Errors { get; set; } = new Dictionary<string, object>();
        public override IDictionary<string, object> GetErrors(object source) => Errors;
        public override void SetErrors(object source, IDictionary<string, object> errors) => Errors = (Dictionary<string, object>)errors;
        public override IDictionary<string, object> GetState(object source, object opts) =>
            ((ViewInfo)source).State.Values.ToDictionary(x => x.Key, x =>
            {
                var element = x.Value.FirstOrDefault().Value;
                var value = element.Type switch
                {
                    "multi_channels_select" => (object)((ChannelMultiSelectValue)element).SelectedChannels,
                    "channels_select" => ((ChannelSelectValue)element).SelectedChannel,
                    "checkboxes" => ((CheckboxGroupValue)element).SelectedOptions?.Select(x => x.Value).ToArray(),
                    "multi_conversations_select" => ((ConversationMultiSelectValue)element).SelectedConversations,
                    "conversations_select" => ((ConversationSelectValue)element).SelectedConversation,
                    "datepicker" => ((DatePickerValue)element).SelectedDate,
                    "external_select" => ((ExternalSelectValue)element).SelectedOption,
                    "overflow" => ((OverflowValue)element).SelectedOption?.Value,
                    "plain_text_input" => ((PlainTextInputValue)element).Value,
                    "radio_buttons" => ((RadioButtonGroupValue)element).SelectedOption?.Value,
                    "multi_static_select" => ((StaticMultiSelectValue)element).SelectedOptions?.Select(x => x.Value).ToArray(),
                    "static_select" => ((StaticSelectValue)element).SelectedOption?.Value,
                    "multi_users_select" => ((UserMultiSelectValue)element).SelectedUsers,
                    "users_select" => ((UserSelectValue)element).SelectedUser,
                    _ => throw new ArgumentOutOfRangeException(nameof(element.Type), element.Type),
                };
                return value;
            });

        public override void SetState(object source, object opts, IDictionary<string, object> values) => throw new NotSupportedException();
    }
}