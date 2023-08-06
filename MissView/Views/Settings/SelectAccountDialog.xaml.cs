namespace MissView.Views.Settings;

public partial class SelectAccountDialog : ContentPage
{
	public SelectAccountDialog()
	{
		InitializeComponent();
	}

	void showAccountsList()
	{
		var accts = new Libs.AccountsIO();
		var AccountsJson = accts.GetAccountsJson();
		accts.ShowAccountsList().ForEach((Account) =>
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

			myVerticalStackLayout.Children.Add(acctInfo);
		});
	}	
}