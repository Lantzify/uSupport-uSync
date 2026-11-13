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
using uSync.BackOffice.SyncHandlers.Models;
using uSync.BackOffice.SyncHandlers.Interfaces;
using uSync.BackOffice.Configuration;

namespace uSupport_uSync.Handlers
{
	[SyncHandler("uSupportTicketStatus", "Ticket statuses", "uSupport\\TicketStatus", 2, Icon = "icon-file-cabinet", EntityType = "uSupportTicketStatus")]
	public class TicketStatusHandler : SyncHandlerRoot<uSupportTicketStatus, uSupportTicketStatus>, ISyncHandler,
		 INotificationHandler<CreateTicketStatusNotification>,
		 INotificationHandler<UpdateTicketStatusNotification>,
		 INotificationHandler<DeleteTicketStatusNotification>
	{
		private readonly IuSupportTicketStatusService _service;

		public TicketStatusHandler(IuSupportTicketStatusService service,
			ILogger<SyncHandlerRoot<uSupportTicketStatus, uSupportTicketStatus>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, ISyncFileService syncFileService, ISyncEventService mutexService, ISyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
		{
			_service = service;
		}

		public override string Group => "uSupport";

		public void Handle(CreateTicketStatusNotification notification) => ExportItem(notification.TicketStatus);
		public void Handle(UpdateTicketStatusNotification notification) => ExportItem(notification.TicketStatus);

		public void Handle(DeleteTicketStatusNotification notification)
		{
			if (ShouldProcessEvent())
				ExportDeletedItemAsync(notification.TicketStatus, GetDefaultHandlerFolders(), DefaultConfig).GetAwaiter().GetResult();
		}

		private void ExportItem(uSupportTicketStatus item)
		{
			if (!ShouldProcessEvent())
				return;

			var folders = GetDefaultHandlerFolders();
			foreach (var attempt in ExportAsync(item, folders, DefaultConfig).GetAwaiter().GetResult())
				if (attempt.Success && attempt.FileName != null)
					CleanUpAsync(item, attempt.FileName, folders.Last()).GetAwaiter().GetResult();
		}

		protected override Task<IEnumerable<uSyncAction>> DeleteMissingItemsAsync(uSupportTicketStatus parent, IEnumerable<Guid> keysToKeep, bool reportOnly) => Task.FromResult(Enumerable.Empty<uSyncAction>());
		protected override Task<IEnumerable<uSupportTicketStatus>> GetChildItemsAsync(uSupportTicketStatus? parent) => Task.FromResult(parent == null ? _service.GetAll() : Enumerable.Empty<uSupportTicketStatus>());
		protected override Task<IEnumerable<uSupportTicketStatus>> GetFoldersAsync(uSupportTicketStatus? parent) => Task.FromResult(Enumerable.Empty<uSupportTicketStatus>());
		protected override Task<uSupportTicketStatus?> GetFromServiceAsync(uSupportTicketStatus? item) => Task.FromResult<uSupportTicketStatus?>(item == null ? null : _service.Get(item.Id));
		protected override string GetItemName(uSupportTicketStatus item) => item.Name;
	}
}
