namespace MissView.Views.Settings;

public partial class SelectAccountDialog : ContentPage
{
	public SelectAccountDialog()
	{
		InitializeComponent();
		ShowAccountsList();
	}

	void ShowAccountsList()
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
				acctInfo.Content = new StackLayout
				{
					Children =
					{
						TitleLabel,
						InstanceNameLabel,
						UrlLabel,
						AccessTokenLabel,
						UserIDLabel,
						UserNameLabel
					}
				};
				SelectAccountDialogLayout.Children.Add(acctInfo);

				SelectAccountDialogLayout.Children.Add(new Button
				{
					Text = "このアカウントを選択",
					Command = new Command(async () =>
					{
						Libs.AccountsIO.SetLastUsedAccount(Accounts.IndexOf(Account));
						await Navigation.PushAsync(new Views.Timeline.TimelinePage());
					})
				});
			}

			Button GotoSettingsButton = new()
			{
				Text = "設定を開く"
			};
			GotoSettingsButton.Clicked += async (sender, e) => await Navigation.PushAsync(new Views.Settings.SettingsPage());
			SelectAccountDialogLayout.Children.Add(GotoSettingsButton);

		}
	}	
}