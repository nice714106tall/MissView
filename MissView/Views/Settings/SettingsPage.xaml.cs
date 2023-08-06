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
		var accts = new Libs.AccountsIO();
		if (accts.IsAccountsAvailable())
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
			//add account button
			var addAccountButton = new Button { Text = " + " };
		}
	}



}