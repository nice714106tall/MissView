using System.Collections.ObjectModel;
using System.Diagnostics;

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
			var Accounts = Libs.AccountsIO.GetDeserializedAccounts();
			foreach (var Account in Accounts)
			{
				Frame acctInfo = new();

				Label TitleLabel = new()
				{
					Text = "アカウント情報(" + (Accounts.IndexOf(Account) + 1) + ")"
				};
				Label UrlLabel = new()
				{
					Text = "URL: " + Account["URL"]
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
				DeleteButton.Clicked += async (sender, e) => await DeleteButton_Clicked(sender, e);

				acctInfo.Content = new StackLayout
				{
					Children =
					{
						TitleLabel,
						InstanceNameLabel,
						UrlLabel,
						AccessTokenLabel,
						UserIDLabel,
						UserNameLabel,
						DeleteButton
					}
				};
				settingsVerticalStackLayout.Children.Add(new Label { Text = "現在、これらのアカウントが登録されています。" });
				settingsVerticalStackLayout.Children.Add(acctInfo);

			}
			Button AddMoreAccountButton = new()
			{
				Text = "さらにアカウントを追加する"
			};
			AddMoreAccountButton.Clicked += async (sender, e) => await AddAccountButton_Clicked(sender, e);
			settingsVerticalStackLayout.Children.Add(new Label { Text = " " });
			settingsVerticalStackLayout.Children.Add(AddMoreAccountButton);
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

	async Task DeleteButton_Clicked(object sender, EventArgs e)
	{
		//削除の確認ダイアログを表示
		var result = await DisplayAlert("確認", "アカウントを削除しますか？", "はい", "いいえ");
		if (result)
		{
			//削除処理
			Debug.WriteLine("削除処理");
		}
	}
	protected override void OnAppearing()
	{
		base.OnAppearing();
		//settingsVerticalStackLayoutを空にする
		settingsVerticalStackLayout.Children.Clear();
		LoadPreferences();
	}

}