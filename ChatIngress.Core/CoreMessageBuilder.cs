using KFrame;
using SlackNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Option = SlackNet.Blocks.Option;

namespace ChatIngress
{
    public class CoreMessageBuilder
    {
        readonly ISlackApiClient _slack;
        readonly IDictionary<string, FrameObject> _frame;
        readonly TimeSpan _frameExpires;

        public CoreMessageBuilder(ISlackApiClient slack)
        {
            _slack = slack;
            _frame = KFrameManager.GetFrame();
            _frameExpires = new TimeSpan(0, 5, 0);
        }

        public Task<IList<Option>> GetOptions((string key, Func<Dictionary<string, object>, string> value, Func<Dictionary<string, object>, string> text) source, Func<KeyValuePair<object, Dictionary<string, object>>, bool> predicate, string text, int take = 10) => Task.FromResult(
            KFrameManager.CheckFrame(_frame, _frameExpires).TryGetValue(source.key, out var z)
            ? z.Where(predicate).Select(x => (Value: source.value(x.Value) ?? x.Key.ToString(), Text: source.text(x.Value)))
                .Where(x => x.Text.Contains(text, StringComparison.OrdinalIgnoreCase))
                .Take(take).Select(x => new Option { Value = x.Value, Text = x.Text })
                .ToList()
            : (IList<Option>)Array.Empty<Option>());
    }
}
