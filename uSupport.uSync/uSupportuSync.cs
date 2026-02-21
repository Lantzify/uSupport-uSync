using uSync.BackOffice.Models;

namespace uSupport_uSync
{
	public class uSupportuSync : ISyncAddOn
	{
		public string Name => "uSupport.uSync";
		public string Version => "1.0.0";
		public string Icon => "icon-help";
		public string View => string.Empty;
		public string Alias => "uSupportSync";
		public string DisplayName => "uSync for uSupport";
		public int SortOrder => 20;
	}
}
