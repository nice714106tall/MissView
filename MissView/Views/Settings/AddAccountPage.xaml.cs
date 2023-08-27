using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MissView.Views.Settings;

public partial class AddAccountPage : ContentPage
{
	readonly Entry URLEntry = new() { Placeholder = "https://misskey.io",
	Text = "https://honi.club/"
	};
	readonly Button MiAuthButton = new() { Text = "アカウント認証する" };
	readonly Button GetAccessTokenButton = new() { Text = "アクセストークンを取得する" };
	readonly Label URLLabel = new() { Text = "URL" };
	readonly Label spacer = new() { Text = "" };
	readonly Label spacer2 = new() { Text = "" };
	readonly Label spacer3 = new() { Text = "" };
	readonly string ApplicationName = "MissView";
	readonly string Permissions = "read:account,read:blocks,read:drive,read:favorites,read:following,read:messaging,read:mutes,read:notifications,read:reactions,read:votes,write:account,write:blocks,write:drive,write:favorites,write:following,write:messaging,write:mutes,write:notes,write:reactions,write:votes";
	readonly string sessionID = Guid.NewGuid().ToString();


	public AddAccountPage()
	{
		InitializeComponent();
		Main();
	}

	public void Main() 
	{
		GetAccessTokenButton.IsEnabled = false;
		AddAccountPageVerticalStackLayout.Children.Add(URLLabel);
		AddAccountPageVerticalStackLayout.Children.Add(URLEntry);
		AddAccountPageVerticalStackLayout.Children.Add(spacer);
		MiAuthButton.Clicked += async (sender, e) => await MiAuthButton_Clicked(sender, e);
		AddAccountPageVerticalStackLayout.Children.Add(MiAuthButton);
		AddAccountPageVerticalStackLayout.Children.Add(spacer2);
		GetAccessTokenButton.Clicked += async (sender, e) => await GetAccessTokenButton_Clicked(sender, e);
		AddAccountPageVerticalStackLayout.Children.Add(GetAccessTokenButton);
		AddAccountPageVerticalStackLayout.Children.Add(spacer3);
	}

	async Task MiAuthButton_Clicked(object sender, EventArgs e)
	{
		var URL = URLEntry.Text;
		if (URL.Last() == '/') URL = URL.Remove(URL.Length - 1);
		var miAuthURL = URL + "/miauth/" + sessionID + "?name=" + ApplicationName + "&permission=" + Permissions;
		await Browser.OpenAsync(miAuthURL, BrowserLaunchMode.SystemPreferred);
		GetAccessTokenButton.IsEnabled = true;
	}

	async Task GetAccessTokenButton_Clicked(object sender, EventArgs e)
	{
		Debug.WriteLine("SessionID=" + sessionID);

		var URL = URLEntry.Text;
		// URL末尾の/を削除する
		if (URL.Last() == '/') URL = URL.Remove(URL.Length - 1);
		var miAuthResultURL = URL + "/api/miauth/" + sessionID + "/check";
		AddAccountPageVerticalStackLayout.Children.Add(new Label { Text = "MiAuthResultURL=" + miAuthResultURL });

		//miAuthURLにPOSTリクエストを送信してアクセストークンを取得する
		var client = new HttpClient();
		try
		{
			HttpResponseMessage response = await client.PostAsync(miAuthResultURL, null);
			//レスポンスが帰ってきたら、レスポンスの中身を読み取る
			if (response != null)
			{
				var responseString = await response.Content.ReadAsStringAsync();
				if (responseString != null)
				{
					//jsonのデシリアライズにnewtonsoft.jsonを使う
					RootObject responseJson = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(responseString);
					string AccessToken = responseJson.Token;
					var User = responseJson.User;

					// /nodeinfo/2.0を叩いてインスタンス名を取得する
					string nodeinfoURL = URL + "/nodeinfo/2.0";
					var nodeinfoResponse = await client.GetAsync(nodeinfoURL);
					if (nodeinfoResponse != null)
					{
						string nodeinfoResponseString = await nodeinfoResponse.Content.ReadAsStringAsync();
						if (nodeinfoResponseString != null)
						{
							NodeinfoRootObject nodeinfoResponseJson = Newtonsoft.Json.JsonConvert.DeserializeObject<NodeinfoRootObject>(nodeinfoResponseString);
							//アカウント情報を保存する
							var account = new Dictionary<string, string>
								{
									{ "URL", URL },
									{ "AccessToken", AccessToken },
									{ "UserID", User.Id },
									{ "InstanceName", nodeinfoResponseJson.Metadata.NodeName },
									{ "UserName", User.Username }
								};
							string serializedAccount = System.Text.Json.JsonSerializer.Serialize(account);
							Libs.AccountsIO.SaveAccount(serializedAccount);
							Debug.WriteLine("account=" + serializedAccount);
							await Navigation.PopAsync();
						}
						else
						{
							Debug.WriteLine("nodeinfoResponseString is null");
						}
					}
					else
					{
						Debug.WriteLine("nodeinfoResponse is null");
					}
				}
				else
				{
					Debug.WriteLine("responseString is null");
				}
			}
			else
			{
				AddAccountPageVerticalStackLayout.Children.Add(new Label { Text = "アクセストークンの取得に失敗しました。" });
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}

	}
}