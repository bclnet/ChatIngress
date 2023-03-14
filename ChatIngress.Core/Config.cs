using ChatIngress.Slack;
using ChatIngress.Teams;
using Contoso.Extensions;
using KFrame;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Security;
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

        public static NetworkCredential Slack = new ParsedConnectionString(Configuration.GetConnectionString("Slack")).Credential;
        public static NetworkCredential Teams = new ParsedConnectionString(Configuration.GetConnectionString("Teams")).Credential;

        public static string ConnectionString => Configuration.GetConnectionString("Main");
        public static string ThisUrl => Configuration.GetValue<string>("ThisUrl");
        public static string KframeUrl => Configuration.GetValue<string>("KframeUrl");
        public static string KframeCertificate => Configuration.GetValue<string>("KframeCertificate");

        public static readonly List<string> Administrators = new() { "U037F0NPE" };
    }
}