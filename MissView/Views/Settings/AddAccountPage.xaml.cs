using System;

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
	readonly string ApplicationName = "MissView";
	readonly string Permissions = "read:account,read:blocks,read:drive,read:favorites,read:following,read:messaging,read:mutes,read:notifications,read:reactions,read:votes,write:account,write:blocks,write:drive,write:favorites,write:following,write:messaging,write:mutes,write:notes,write:reactions,write:votes";
	string sessionID = Guid.NewGuid().ToString();


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
		GetAccessTokenButton.Clicked += async (sender, e) => await GetAccessTokenButton_Clicked(sender, e);
		AddAccountPageVerticalStackLayout.Children.Add(GetAccessTokenButton);
	}

	async Task MiAuthButton_Clicked(object sender, EventArgs e)
	{
		var URL = URLEntry.Text;
		var miAuthURL = URL + "/miauth/" + sessionID + "?name=" + ApplicationName + "&permission=" + Permissions;
		if (miAuthURL.Last() == '/') miAuthURL = miAuthURL.Remove(miAuthURL.Length - 1);
		await Browser.OpenAsync(miAuthURL, BrowserLaunchMode.SystemPreferred);
		GetAccessTokenButton.IsEnabled = true;
	}

	async Task GetAccessTokenButton_Clicked(object sender, EventArgs e)
	{
		Console.WriteLine("GetAccessTokenButton_Clicked");
		Console.WriteLine("SessionID=" + sessionID);
		AddAccountPageVerticalStackLayout.Children.Add(new Label { Text = "アクセストークンを取得しています。" });
		AddAccountPageVerticalStackLayout.Children.Add(new Label { Text = "SessionID=" + sessionID });

		var URL = URLEntry.Text;
		// URL末尾の/を削除する
		if (URL.Last() == '/') URL = URL.Remove(URL.Length - 1);
		var miAuthResultURL = URL + "/api/miauth/" + sessionID + "/check";
		AddAccountPageVerticalStackLayout.Children.Add(new Label { Text = "MiAuthResultURL=" + miAuthResultURL });

		//miAuthURLにPOSTリクエストを送信してアクセストークンを取得する
		var client = new HttpClient();
		HttpResponseMessage response = null;
		try
		{ 
			response = await client.PostAsync(miAuthResultURL, null);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
		//レスポンスが帰ってきたら、レスポンスの中身を読み取る
		if (response != null)
		{
			var responseString = await response.Content.ReadAsStringAsync();
			//debug
			AddAccountPageVerticalStackLayout.Children.Add(new Label { Text = "responseString=" + responseString });


			/*
			 * ここから先のどこかで例外が発生している
			 */

			var responseJson = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);
			var AccessToken = responseJson["Token"];
			var userID = responseJson["user"];
			// /api/metaを叩いてインスタンス名を取得する
			var metaURL = URL + "/api/meta";
			var metaResponse = await client.GetAsync(metaURL);
			var metaResponseString = await metaResponse.Content.ReadAsStringAsync();
			var metaResponseJson = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(metaResponseString);
			var instanceName = metaResponseJson["name"];

			//アカウント情報を保存する
			var account = new Dictionary<string, string>
			{
				{ "URL", URL },
				{ "AccessToken", AccessToken },
				{ "UserID", userID },
				{ "InstanceName", instanceName },
				{ "UserName", "" }
			};
			Libs.AccountsIO.SaveAccount(System.Text.Json.JsonSerializer.Serialize(account));
			await Navigation.PopAsync();
		}
		else
		{ 
			Label FailedToGetAccessToken = new() { Text = "アクセストークンの取得に失敗しました。" };
			AddAccountPageVerticalStackLayout.Children.Add(FailedToGetAccessToken);
		}
	}
}