using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissView.Libs
{
    class AccountsIO
    {
		public List<Dictionary<string, string>> ShowAccountsList()
        {
			if (IsAccountsAvailable())
			{
				var Accounts = Preferences.Get("Accounts", "");
				var AccountsJson = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(Accounts);
				return AccountsJson;
			}
			else
			{
				return null;
			}
		}

		public bool IsAccountsAvailable()
		{
			return Microsoft.Maui.Storage.Preferences.ContainsKey("Accounts");
		}

		public Dictionary<string, string> ShowAccount(int index)
		{
			if (IsAccountsAvailable())
			{
				var Accounts = Preferences.Get("Accounts", "");
				var AccountsJson = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(Accounts);
				return AccountsJson[index];
			}
			else
			{
				return null;
			}
		}

		public int LastUsedAccount = -1;

		public void SetLastUsedAccount(int index)
		{
			Preferences.Set("LastUsedAccount", index);
		}

		public int GetLastUsedAccount()
		{
			if (Microsoft.Maui.Storage.Preferences.ContainsKey("LastUsedAccount"))
			{
				return Preferences.Get("LastUsedAccount", -1);
			}
			else
			{
				return -1;
			}
		}


	}
}
