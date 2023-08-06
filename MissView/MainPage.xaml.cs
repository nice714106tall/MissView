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
		if(acct.IsAccountsAvailable())
		{
			if(acct.GetLastUsedAccount() != -1)
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

}

