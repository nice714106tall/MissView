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
				acctInfo.AutomationId = "acctInfo";

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
		//削除処理
		//削除ボタンが押されたFrameのインデックスを取得

		int index = settingsVerticalStackLayout.Children.IndexOf((View)sender);
		if (index >= 0)
		{
			Dictionary<string, string> AccountToDelete = Libs.AccountsIO.ShowAccount(index);
			string AccountToDeleteHostName = AccountToDelete["URL"].Replace("https://", "");
			AccountToDeleteHostName = AccountToDeleteHostName.Replace("http://", "");
			string AccountToDeleteText =
				"[" + index + "]:" + AccountToDelete["InstanceName"] + "\n\n" +
				"(@" + AccountToDelete["UserName"] + "@" + AccountToDeleteHostName + ")";

			bool DeleteConfirmResult = await DisplayAlert("確認", "以下のアカウントを削除しますか？\n\n" + index + ":\n\n" + AccountToDelete, "OK", "キャンセル");
			if (DeleteConfirmResult)
			{
				//AccountsIOのDeleteAccountメソッドを呼び出し、アカウントを削除
				Libs.AccountsIO.DeleteAccount(index);
				//settingsVerticalStackLayoutを空にする
				settingsVerticalStackLayout.Children.Clear();
				//LoadPreferencesメソッドを呼び出し、アカウント情報を再読み込み
				LoadPreferences();
			}
			else
			{
				//キャンセルされたら何もしない
			}
		}
		else
		{
			await DisplayAlert("エラー", "内部エラーが発生しました。\n\n(index=" + index + ")", "OK");
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