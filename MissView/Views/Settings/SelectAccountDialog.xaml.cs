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
		var accts = new Libs.AccountsIO();
		var AccountsJson = Libs.AccountsIO.GetAccountsJson();
		int i=1;
		Libs.AccountsIO.ShowAccountsList().ForEach((Account) =>
		{
			Frame acctInfo = new();

			Label TitleLabel = new()
			{
				Text = "アカウント情報(" + i + ")"
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

			SelectAccountDialogLayout.Children.Add(acctInfo);
			i++;
		});
	}	
}