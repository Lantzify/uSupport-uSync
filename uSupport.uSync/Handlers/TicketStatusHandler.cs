using uSync.Core;
using uSync.BackOffice;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Cache;
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Strings;
using uSync.BackOffice.Services;
using Microsoft.Extensions.Logging;
using uSupport.Services.Interfaces;
using uSync.BackOffice.SyncHandlers;
using uSync.BackOffice.Configuration;

namespace uSupport_uSync.Handlers
{
	[SyncHandler("uSupportTicketStatus", "Ticket statuses", "uSupport\\TicketStatus", 2, Icon = "icon-file-cabinet", EntityType = "uSupportTicketStatus")]
	public class TicketStatusHandler : SyncHandlerRoot<uSupportTicketStatus, uSupportTicketStatus>, ISyncHandler,
		 INotificationHandler<CreateTicketStatusNotification>,
		 INotificationHandler<UpdateTicketStatusNotification>,
		 INotificationHandler<DeleteTicketStatusNotification>
	{
		private readonly IuSupportTicketStatusService _ticketStatusService;

		public TicketStatusHandler(IuSupportTicketStatusService ticketStatusService,
			ILogger<SyncHandlerRoot<uSupportTicketStatus, uSupportTicketStatus>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
		{
			_ticketStatusService = ticketStatusService;
		}

		public override string Group => "uSupport";

		public override IEnumerable<uSyncAction> ExportAll(uSupportTicketStatus parent, string folder, HandlerSettings config, SyncUpdateCallback callback)
		{
			var actions = new List<uSyncAction>();
			foreach (var item in _ticketStatusService.GetAll())
				actions.AddRange(Export(item, folder, config));

			return actions;
		}

		private bool ShouldProcess()
		{
			if (_mutexService.IsPaused || !DefaultConfig.Enabled)
				return false;

			return true;
		}

		public void Handle(CreateTicketStatusNotification notification)
		{
			if (!ShouldProcess())
				return;

			var attempts = Export(notification.TicketStatus, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
			foreach (var attempt in attempts.Where(x => x.Success))
				CleanUp(notification.TicketStatus, attempt.FileName, Path.Combine(rootFolder, DefaultFolder));
		}

		public void Handle(UpdateTicketStatusNotification notification)
		{
			if (!ShouldProcess())
				return;

			var attempts = Export(notification.TicketStatus, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
			foreach (var attempt in attempts.Where(x => x.Success))
				CleanUp(notification.TicketStatus, attempt.FileName, Path.Combine(rootFolder, DefaultFolder));
		}

		public void Handle(DeleteTicketStatusNotification notification)
		{
			if (!ShouldProcess())
				return;

			ExportDeletedItem(notification.TicketStatus, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
		}

		protected override IEnumerable<uSyncAction> DeleteMissingItems(uSupportTicketStatus parent, IEnumerable<Guid> keysToKeep, bool reportOnly) => Enumerable.Empty<uSyncAction>();
		protected override IEnumerable<uSupportTicketStatus> GetChildItems(uSupportTicketStatus parent) => Enumerable.Empty<uSupportTicketStatus>();
		protected override IEnumerable<uSupportTicketStatus> GetFolders(uSupportTicketStatus parent) => Enumerable.Empty<uSupportTicketStatus>();
		protected override uSupportTicketStatus GetFromService(uSupportTicketStatus item) => _ticketStatusService.Get(item.Id) ?? new uSupportTicketStatus();
		protected override string GetItemName(uSupportTicketStatus item) => item.Name;
	}
}
