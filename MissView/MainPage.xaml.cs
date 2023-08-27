namespace MissView;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		LoadPreferences();
	}

	void LoadPreferences()
	{
		Libs.AccountsIO acct = new();
		if(Libs.AccountsIO.IsAccountsAvailable())
		{
			if(Libs.AccountsIO.GetLastUsedAccountIdx() != -1)
			{
				//show timeline page
				Navigation.PushAsync(new Views.Timeline.TimelinePage());
			}
			else
			{
				ShowAccountsList();
			}
		}
		else
		{
			myVerticalStackLayout.Children.Add(new Button
			{
				Text = "Open Settings",
				Command = new Command(async () =>
				{
					await Navigation.PushAsync(new Views.Settings.SettingsPage());
				})
			});
		}
	}

	void ShowAccountsList()
	{
		//SelectAccountsDialogをポップアップ表示する
		Navigation.PushAsync(new Views.Settings.SelectAccountDialog());
	}

}

