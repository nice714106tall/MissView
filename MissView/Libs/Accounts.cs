using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissView.Libs
{
    class AccountsIO
    {
		public static List<Dictionary<string, string>> ShowAccountsList()
        {
			if (AccountsIO.IsAccountsAvailable())
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

		public static string GetAccountsJson()
		{
			if (IsAccountsAvailable())
			{
				return Preferences.Get("Accounts", "");
			}
			else
			{
				return null;
			}
		}

		public static List<Dictionary<string, string>> GetDeserializedAccounts()
		{
			if (AccountsIO.IsAccountsAvailable())
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

		public static bool IsAccountsAvailable()
		{
			return Microsoft.Maui.Storage.Preferences.ContainsKey("Accounts");
		}

		public static Dictionary<string, string> ShowAccount(int index)
		{
			if (AccountsIO.IsAccountsAvailable())
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

		public static void SetLastUsedAccount(int index)
		{
			Preferences.Set("LastUsedAccount", index);
		}

		public static int GetLastUsedAccount()
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

		public static bool SaveAccount(string content)
		{
			try
			{
				//既存のアカウントリストを取得し、新規データを末尾に追加する
				List<Dictionary<string, string>> Accounts = ShowAccountsList();
				Accounts ??= new List<Dictionary<string, string>>();
				Dictionary<string, string> newAccount = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
				Accounts.Add(newAccount);
				//アカウントリストを保存する
				Preferences.Set("Accounts", System.Text.Json.JsonSerializer.Serialize(Accounts));
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool DeleteAccount(int index)
		{
			try
			{
				//既存のアカウントリストを取得し、指定されたインデックスのデータを削除する
				var Accounts = Preferences.Get("Accounts", "");
				var AccountsJson = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(Accounts);
				AccountsJson.RemoveAt(index);
				//アカウントリストを保存する
				Preferences.Set("Accounts", System.Text.Json.JsonSerializer.Serialize(AccountsJson));
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
