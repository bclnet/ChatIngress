using ChatIngress.Slack;
using ChatIngress.Teams;
using Contoso.Extensions;
using KFrame;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace ChatIngress
{
    public class Config : ConfigBase, IKFrameConfig, ISlackConnectionString, ITeamsConnectionString
    {
        public const string Cmd = "ingress";
        public Config() => KFrameManager.Config = this;

        string IKFrameConfig.KframeUrl => KframeUrl;
        X509Certificate IKFrameConfig.Certificate => !string.IsNullOrEmpty(KframeCertificate) ? KFrameManager.FindCertificate(KframeCertificate) : null;
        string ISlackConnectionString.this[string name] => Configuration.GetConnectionString("Slack");
        string ITeamsConnectionString.this[string name] => Configuration.GetConnectionString("Teams");

        public static string ConnectionString => Configuration.GetConnectionString("Main");
        public static string ThisUrl => Configuration.GetValue<string>("ThisUrl");
        public static string KframeUrl => Configuration.GetValue<string>("KframeUrl");
        public static string KframeCertificate => Configuration.GetValue<string>("KframeCertificate");
        public static string Bot => Configuration.GetValue<string>("Bot");
        public static string ApiToken => Configuration.GetValue<string>($"ApiToken-{Bot}");
        public static string VerificationToken => Configuration.GetValue<string>($"VerificationToken-{Bot}");

        public static readonly List<string> Administrators = new()
        {
            "U037F0NPE", // Sky Morey
            "U1J2L71E2", // Neal Sharma
            "U1JFZ4N9J", // Scott Miles
        };
    }
}