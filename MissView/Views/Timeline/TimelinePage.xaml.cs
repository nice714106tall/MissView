using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;


namespace MissView.Views.Timeline;

public partial class TimelinePage : ContentPage
{
	public TimelinePage()
	{
		InitializeComponent();
		Connect();
	}

	private async void Connect()
	{
		try
		{
			var TimelineWebsocket = new ClientWebSocket();

			var LastUsedAccount = Libs.AccountsIO.GetLastUsedAccount();
			Debug.WriteLine("URL=" + LastUsedAccount["URL"]);
			Debug.WriteLine("AccessToken=" + LastUsedAccount["AccessToken"]);
			Debug.WriteLine("UserID=" + LastUsedAccount["UserID"]);
			Debug.WriteLine("InstanceName=" + LastUsedAccount["InstanceName"]);
			Debug.WriteLine("UserName=" + LastUsedAccount["UserName"]);

			string WebsocketUrl = Libs.AccountsIO.GetLastUsedAccount()["URL"].Replace("https://", "wss://");
			WebsocketUrl = WebsocketUrl.Replace("http://", "wss://");
			WebsocketUrl += "/streaming?i=" + Libs.AccountsIO.GetLastUsedAccount()["AccessToken"];
			Debug.WriteLine("WebsocketUrl=" + WebsocketUrl);
			Uri WebsocketUri = new(WebsocketUrl);
			await TimelineWebsocket.ConnectAsync(WebsocketUri, CancellationToken.None);
			string Uuid = Guid.NewGuid().ToString();
			string payload =
				@"{
					type: 'connect',
					body: {
						channel: 'homeTimeline',
						id: " + Uuid + @",
					}
				}";
			Debug.WriteLine("payload=" + payload);
			//ペイロードを送信
			await TimelineWebsocket.SendAsync(Encoding.UTF8.GetBytes(payload), WebSocketMessageType.Text, true, CancellationToken.None);
			Debug.WriteLine("ペイロード送信完了");

			//受信ループ
			while (TimelineWebsocket.State == WebSocketState.Open)
			{
				Debug.WriteLine("応答待機中");
				var buffer = new byte[1024 * 4];
				var result = await TimelineWebsocket.ReceiveAsync(buffer, CancellationToken.None);
				var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
				Debug.WriteLine("message=" + message);
			}

			if(TimelineWebsocket.State == WebSocketState.Closed)
			{
				Debug.WriteLine("Websocketが切断されました。");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			//System.Net.WebSockets.WebSocketExceptionの場合は設定ボタンを表示する
			if (ex.GetType() == typeof(System.Net.WebSockets.WebSocketException))
			{
				//ex.messageが"The server returned status code '401' when status code '101' was expected."の場合はアクセストークンが無効なので設定ボタンを表示する
				Debug.WriteLine(ex.Message);
				if(ex.Message == "The server returned status code '401' when status code '101' was expected.") { 
					TimelinePageLayout.Children.Add(new Label()
					{
						Text = "接続に失敗しました。アクセストークンが無効です。",
					});
					TimelinePageLayout.Children.Add(new Button()
					{
						Text = "設定",
						Command = new Command(() =>
						{
							//設定ページに遷移する
							Navigation.PushAsync(new Views.Settings.SettingsPage());
						}),
					});
				}
				else
				{
					TimelinePageLayout.Children.Add(new Label()
					{
						Text = "接続に失敗しました。ネットワークの接続を確認してください。",
					});
					TimelinePageLayout.Children.Add(new Button()
					{
						Text = "設定",
						Command = new Command(() =>
						{
							//設定ページに遷移する
							Navigation.PushAsync(new Views.Settings.SettingsPage());
						}),
					});
				}
			}
		}
	}

}