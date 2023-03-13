using SlackNet;
using System;
using System.Web;

namespace ChatIngress.Slack.Services
{
    public partial class SlackService
    {
        static string BuildStandardMessage(string content) => @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
	<title></title>
	<style type=""text/css"" media=""screen"">
	    body, p{margin:0; padding:0; margin-bottom:0; -webkit-text-size-adjust:none; -ms-text-size-adjust:none;} img{line-height:100%; outline:none; text-decoration:none; -ms-interpolation-mode: bicubic;} a img{border: none;} #backgroundTable {margin:0; padding:0; width:100% !important; } a, a:link{color:#2A5DB0; text-decoration: underline;} .ExternalClass {display: block !important; width:100%;} .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div { line-height: 100%; } table td {border-collapse:collapse;} sup{position: relative; top: 4px; line-height:7px !important;font-size:11px !important;} a[href^=""tel""], a[href^=""sms""] {text-decoration: none; color: #000001; /* set text color */ pointer-events: none; cursor: default;} .mobile_link a[href^=""tel""], .mobile_link a[href^=""sms""] {text-decoration: default; color: orange !important; /* set link color */ pointer-events: auto; cursor: default;} .no-detect a{text-decoration: none; color: #000001; /* set text color */ pointer-events: auto; cursor: default;} span {color: inherit; border-bottom: none;} span:hover { background-color: transparent; }
	    * {
		  -moz-box-sizing: border-box; -webkit-box-sizing: border-box; box-sizing: border-box;
		}
		.tabularData .row td {
            border-bottom: 1px solid #ddd;
        }
        @media only screen and (max-width: 480px) {
        	td[class=""wrap""] .tabularData .row {
        		display: block;
        		padding: 7px 0;
        	}
            td[class=""wrap""] .tabularData .row:nth-child(even) { background: #fafafa; }
            td[class=""wrap""] .tabularData .row td {
            	display: block;
            	border-bottom: 0;
            	padding-left: 14px !important;
            	padding-right:14px !important;
            	width: 100% !important;
            }
        	td[class=""wrap""] .tabularData .total { display: block; width: 100%; }
        	td[class=""wrap""] .tabularData .total td { 
        		padding-left: 14px !important; 
        		padding-right: 14px !important;
        		border-top: 1px solid #ddd;  
        		width: 100%;
        	}
        	td[class=""wrap""] .tabularData .total td:last-child { text-align: right; }
        }
    </style>
</head>
<body style=""background: #fff;font-family:Arial, Helvetica, sans-serif;font-size:12px;"">" + content + @"
</body>
</html>";

        static string BuildChannelEmail(Nameable<(string id, string email)> requester, Conversation s, Nameable<object> accountId) => BuildStandardMessage($@"
<table id=""backgroundTable"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
    <tr>
        <td align=""center"" valign=""top"" style=""background: #fff;"" width=""100%"">
            <table cellpadding=""0"" cellspacing=""0"">
                <tr>
                    <td class=""wrap"" width=""600"" style=""padding: 1em;"">
                        <table cellpadding=""0"" cellspacing=""0"">
                        	<tr>
        			            <td width=""600"" align=""left"" style=""font-family: arial,sans-serif; font-size: 28px; font-weight: bold;"">
        			                <a href=""{SlackRootUrl}messages/{HttpUtility.UrlEncode(s.Name)}/"">{HttpUtility.HtmlEncode($"{s.Name} #{s.Creator}")}</a>
        			            </td>
        			        </tr>
        			        <tr>
        			            <td width=""600"" align=""left"" style=""font-family: arial,sans-serif; font-size: 22px;"">
        			                {accountId.Name}
        			            </td>
        			        </tr>
        			        <tr>
        			            <td width=""600"" align=""left"" style=""font-family: arial,sans-serif; font-size: 14px;"">
        			                {(!string.IsNullOrEmpty(s.Purpose?.Value) ? HttpUtility.HtmlEncode(s.Purpose?.Value).Replace("\n", "<br />") : string.Empty)}
        			            </td>
        			        </tr>
        			        <tr>
        			            <td width=""600"" align=""left"" style=""font-family: arial,sans-serif; font-size: 14px;"">
        							<span style=""font-style: italic;"">Registered by: {(!string.IsNullOrEmpty(requester.Name) ? HttpUtility.HtmlEncode(requester.Name) : "N/A")}</span>
        			            </td>
        			        </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>");
    }
}