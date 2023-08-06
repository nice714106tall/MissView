namespace MissView.Views.Settings;

public partial class AddAccountPage : ContentPage
{
	Entry URLEntry = new Entry { Placeholder = "https://misskey.io" };
	Button miAuthButton = new Button { Text = "アカウント認証する" };
	Button getAccessTokenButton = new Button { Text = "アクセストークンを取得する" };
	Label URLLabel = new Label { Text = "URL" };
	Label spacer = new Label { Text = "" };
	string ApplicationName = "MissView";
	string Permissions = "read:account,read:blocks,read:drive,read:favorites,read:following,read:messaging,read:mutes,read:notifications,read:reactions,read:votes,write:account,write:blocks,write:drive,write:favorites,write:following,write:messaging,write:mutes,write:notes,write:reactions,write:votes";

	public AddAccountPage()
	{
		InitializeComponent();
		main();
	}

	public void main() 
	{
		getAccessTokenButton.IsEnabled = false;

		AddAccountPageVerticalStackLayout.Children.Add(URLLabel);
		AddAccountPageVerticalStackLayout.Children.Add(URLEntry);
		AddAccountPageVerticalStackLayout.Children.Add(spacer);
		AddAccountPageVerticalStackLayout.Children.Add(miAuthButton);
		AddAccountPageVerticalStackLayout.Children.Add(getAccessTokenButton);
	}

	async void miAuthButton_Clicked(object sender, EventArgs e)
	{
		var sessionID = Guid.NewGuid().ToString();
		var URL = URLEntry.Text;
		if (URL.Last() != '/') URL += "/";
		var miAuthURL = URL + "/miauth/" + sessionID + "?name=" + ApplicationName;
		await Browser.OpenAsync(miAuthURL, BrowserLaunchMode.SystemPreferred);
		getAccessTokenButton.IsEnabled = true;
	}

	async void getAccessTokenButton_Clicked(object sender, EventArgs e)
	{
		var sessionID = Guid.NewGuid().ToString();
		var URL = URLEntry.Text;
		if (URL.Last() != '/') URL += "/";
		var miAuthURL = URL + "/miauth/" + sessionID + "?name=" + ApplicationName + "&permission=" + Permissions;
		//miAuthURLにPOSTリクエストを送信してアクセストークンを取得する
		var client = new HttpClient();
		var response = await client.PostAsync(miAuthURL, null);
		//レスポンスが帰ってきたら、レスポンスの中身を読み取る
		if (response != null)
		{
			var responseString = await response.Content.ReadAsStringAsync();
			var responseJson = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);
			var AccessToken = responseJson["Token"];
			var userID = responseJson["user"];
			// /api/metaを叩いてインスタンス名を取得する
			var metaURL = URL + "/api/meta";
			var metaResponse = await client.GetAsync(metaURL);
			var metaResponseString = await metaResponse.Content.ReadAsStringAsync();
			var metaResponseJson = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(metaResponseString);
			var instanceName = metaResponseJson["name"];
			//アクセストークンをPreferencesに保存する

		}
	}
}