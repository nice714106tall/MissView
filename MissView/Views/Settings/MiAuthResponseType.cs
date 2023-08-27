using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissView.Views.Settings
{
	public class User
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Username { get; set; }
		// 他のプロパティも同様に追加
	}

	public class RootObject
	{
		public bool Ok { get; set; }
		public string Token { get; set; }
		public User User { get; set; }
		// 他のプロパティも同様に追加
	}

	public class NodeinfoRootObject
	{
		public NodeinfoMetadata Metadata { get; set; }
	}

	public class NodeinfoMetadata
	{
		public string NodeName { get; set; }
	}
}
