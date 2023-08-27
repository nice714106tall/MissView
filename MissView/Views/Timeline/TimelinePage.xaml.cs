using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading.Channels;

namespace MissView.Views.Timeline;

public partial class TimelinePage : ContentPage
{
	private readonly Libs.WebSocketClient _webSocketClient;

	public TimelinePage()
	{
		InitializeComponent();
		_webSocketClient = new();
		Connect();
	}

	private async void Connect()
	{
		var LastUsedAccount = Libs.AccountsIO.GetLastUsedAccount();
		Debug.WriteLine("URL=" + LastUsedAccount["URL"]);
		Debug.WriteLine("AccessToken=" + LastUsedAccount["AccessToken"]);
		Debug.WriteLine("UserID=" + LastUsedAccount["UserID"]);
		Debug.WriteLine("InstanceName=" + LastUsedAccount["InstanceName"]);
		Debug.WriteLine("UserName=" + LastUsedAccount["UserName"]);

		string baseURL = Libs.AccountsIO.GetLastUsedAccount()["URL"];
		string endpoint = "/streaming?i=" + Libs.AccountsIO.GetLastUsedAccount()["AccessToken"];
		Uri uri = new(baseURL + endpoint);
		await _webSocketClient.ConnectAsync(uri, CancellationToken.None);
		string Uuid = Guid.NewGuid().ToString();
		string payload =
			@"{
				type: 'connect',
				body: {
					channel: 'homeTimeline',
					id: " + Uuid + @",
				}
			}";
		await _webSocketClient.SendAsync(payload, CancellationToken.None);
	}

}