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
			//�y�C���[�h�𑗐M
			await TimelineWebsocket.SendAsync(Encoding.UTF8.GetBytes(payload), WebSocketMessageType.Text, true, CancellationToken.None);
			Debug.WriteLine("�y�C���[�h���M����");

			//��M���[�v
			while (TimelineWebsocket.State == WebSocketState.Open)
			{
				Debug.WriteLine("�����ҋ@��");
				var buffer = new byte[1024 * 4];
				var result = await TimelineWebsocket.ReceiveAsync(buffer, CancellationToken.None);
				var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
				Debug.WriteLine("message=" + message);
			}

			if(TimelineWebsocket.State == WebSocketState.Closed)
			{
				Debug.WriteLine("Websocket���ؒf����܂����B");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			//System.Net.WebSockets.WebSocketException�̏ꍇ�͐ݒ�{�^����\������
			if (ex.GetType() == typeof(System.Net.WebSockets.WebSocketException))
			{
				//ex.message��"The server returned status code '401' when status code '101' was expected."�̏ꍇ�̓A�N�Z�X�g�[�N���������Ȃ̂Őݒ�{�^����\������
				Debug.WriteLine(ex.Message);
				if(ex.Message == "The server returned status code '401' when status code '101' was expected.") { 
					TimelinePageLayout.Children.Add(new Label()
					{
						Text = "�ڑ��Ɏ��s���܂����B�A�N�Z�X�g�[�N���������ł��B",
					});
					TimelinePageLayout.Children.Add(new Button()
					{
						Text = "�ݒ�",
						Command = new Command(() =>
						{
							//�ݒ�y�[�W�ɑJ�ڂ���
							Navigation.PushAsync(new Views.Settings.SettingsPage());
						}),
					});
				}
				else
				{
					TimelinePageLayout.Children.Add(new Label()
					{
						Text = "�ڑ��Ɏ��s���܂����B�l�b�g���[�N�̐ڑ����m�F���Ă��������B",
					});
					TimelinePageLayout.Children.Add(new Button()
					{
						Text = "�ݒ�",
						Command = new Command(() =>
						{
							//�ݒ�y�[�W�ɑJ�ڂ���
							Navigation.PushAsync(new Views.Settings.SettingsPage());
						}),
					});
				}
			}
		}
	}

}