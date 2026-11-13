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
	[SyncHandler("uSupportTicketType", "Ticket types", "uSupport\\TicketType", 1, Icon = "icon-ticket", EntityType = "uSupportTicketType")]
	public class TicketTypeHandler : SyncHandlerRoot<uSupportTicketType, uSupportTicketType>, ISyncHandler,
		 INotificationHandler<CreateTicketTypeNotification>,
		 INotificationHandler<UpdateTicketTypeNotification>,
		 INotificationHandler<DeleteTicketTypeNotification>
	{
		private readonly IuSupportTicketTypeService _service;

		public TicketTypeHandler(IuSupportTicketTypeService service,
			ILogger<SyncHandlerRoot<uSupportTicketType, uSupportTicketType>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, ISyncFileService syncFileService, ISyncEventService mutexService, ISyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
		{
			_service = service;
		}

		public override string Group => "uSupport";

		// uSupport publishes with the sync IEventAggregator.Publish, which only runs INotificationHandler<T>,
		// so these stay sync and block on uSync's async API.
		public void Handle(CreateTicketTypeNotification notification) => ExportItem(notification.TicketType);
		public void Handle(UpdateTicketTypeNotification notification) => ExportItem(notification.TicketType);

		public void Handle(DeleteTicketTypeNotification notification)
		{
			if (ShouldProcessEvent())
				ExportDeletedItemAsync(notification.TicketType, GetDefaultHandlerFolders(), DefaultConfig).GetAwaiter().GetResult();
		}

		private void ExportItem(uSupportTicketType item)
		{
			if (!ShouldProcessEvent())
				return;

			var folders = GetDefaultHandlerFolders();
			foreach (var attempt in ExportAsync(item, folders, DefaultConfig).GetAwaiter().GetResult())
				if (attempt.Success && attempt.FileName != null)
					CleanUpAsync(item, attempt.FileName, folders.Last()).GetAwaiter().GetResult();
		}

		protected override Task<IEnumerable<uSyncAction>> DeleteMissingItemsAsync(uSupportTicketType parent, IEnumerable<Guid> keysToKeep, bool reportOnly) => Task.FromResult(Enumerable.Empty<uSyncAction>());
		protected override Task<IEnumerable<uSupportTicketType>> GetChildItemsAsync(uSupportTicketType? parent) => Task.FromResult(parent == null ? _service.GetAll() : Enumerable.Empty<uSupportTicketType>());
		protected override Task<IEnumerable<uSupportTicketType>> GetFoldersAsync(uSupportTicketType? parent) => Task.FromResult(Enumerable.Empty<uSupportTicketType>());
		protected override Task<uSupportTicketType?> GetFromServiceAsync(uSupportTicketType? item) => Task.FromResult<uSupportTicketType?>(item == null ? null : _service.Get(item.Id));
		protected override string GetItemName(uSupportTicketType item) => item.Name;
	}
}
