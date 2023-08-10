using System.Collections.ObjectModel;

namespace MissView.Views.Settings;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
		LoadPreferences();
	}

	void LoadPreferences()
	{
		if (Libs.AccountsIO.IsAccountsAvailable())
		{
			var Accounts = Preferences.Get("Accounts", "");
			var AccountsJson = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(Accounts);
			foreach (var Account in AccountsJson)
			{
				Frame acctInfo = new();

				Label TitleLabel = new()
				{
					Text = "アカウント情報(" + (AccountsJson.IndexOf(Account) + 1) + ")"
				};
				Label InstanceNameLabel = new()
				{
					Text = "インスタンス名: " + Account["InstanceName"]
				};
				Label AccessTokenLabel = new()
				{
					Text = "アクセストークン: " + Account["AccessToken"]
				};
				Label UserIDLabel = new()
				{
					Text = "ユーザーID: " + Account["UserID"]
				};
				Label UserNameLabel = new()
				{
					Text = "ユーザー名: " + Account["UserName"]
				};
				Button DeleteButton = new()
				{
					Text = "🗑️"
				};

				acctInfo.Content = new StackLayout
				{
					Children =
					{
						TitleLabel,
						InstanceNameLabel,
						AccessTokenLabel,
						UserIDLabel,
						UserNameLabel,
						DeleteButton
					}
				};

			}
		}
		else
		{
			settingsVerticalStackLayout.Children.Add(new Label { Text = "アカウント情報がありません。" });
			//settingsVerticalStackLayoutに アカウントを追加する ボタンを追加
			Button AddAccountButton = new()
			{
				Text = "アカウントを追加する"
			};
			AddAccountButton.Clicked += async (sender, e) => await AddAccountButton_Clicked(sender, e);
			settingsVerticalStackLayout.Children.Add(AddAccountButton);
		}
	}

	//addAccountButtonが押されたらAddAccountPageを開く
	async Task AddAccountButton_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new AddAccountPage());
	}

}