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
	[SyncHandler("uSupportTicketType", "Ticket types", "uSupport\\TicketType", 1, Icon = "icon-ticket", EntityType = "uSupportTicketType")]
	public class TicketTypeHandler : SyncHandlerRoot<uSupportTicketType, uSupportTicketType>, ISyncHandler,
		 INotificationHandler<CreateTicketTypeNotification>,
		 INotificationHandler<UpdateTicketTypeNotification>,
		 INotificationHandler<DeleteTicketTypeNotification>
	{
		private readonly IuSupportTicketTypeService _ticketTypeService;

		public TicketTypeHandler(IuSupportTicketTypeService ticketTypeService,
			ILogger<SyncHandlerRoot<uSupportTicketType, uSupportTicketType>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
		{
			_ticketTypeService = ticketTypeService;
		}

		public override string Group => "uSupport";

		public override IEnumerable<uSyncAction> ExportAll(uSupportTicketType parent, string folder, HandlerSettings config, SyncUpdateCallback callback)
		{
			var actions = new List<uSyncAction>();
			foreach (var item in _ticketTypeService.GetAll())
				actions.AddRange(Export(item, folder, config));

			return actions;
		}

		private bool ShouldProcess()
		{
			if (_mutexService.IsPaused || !DefaultConfig.Enabled)
				return false;

			return true;
		}

		public void Handle(CreateTicketTypeNotification notification)
		{
			if (!ShouldProcess())
				return;

			var attempts = Export(notification.TicketType, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
			foreach (var attempt in attempts.Where(x => x.Success))
				CleanUp(notification.TicketType, attempt.FileName, Path.Combine(rootFolder, DefaultFolder));
		}

		public void Handle(UpdateTicketTypeNotification notification)
		{
			if (!ShouldProcess())
				return;

			var attempts = Export(notification.TicketType, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
			foreach (var attempt in attempts.Where(x => x.Success))
				CleanUp(notification.TicketType, attempt.FileName, Path.Combine(rootFolder, DefaultFolder));
		}



		public void Handle(DeleteTicketTypeNotification notification)
		{
			if (!ShouldProcess())
				return;

			ExportDeletedItem(notification.TicketType, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
		}

		protected override IEnumerable<uSyncAction> DeleteMissingItems(uSupportTicketType parent, IEnumerable<Guid> keysToKeep, bool reportOnly) => Enumerable.Empty<uSyncAction>();
		protected override IEnumerable<uSupportTicketType> GetChildItems(uSupportTicketType parent) => Enumerable.Empty<uSupportTicketType>();
		protected override IEnumerable<uSupportTicketType> GetFolders(uSupportTicketType parent) => Enumerable.Empty<uSupportTicketType>();
		protected override uSupportTicketType GetFromService(uSupportTicketType item) => _ticketTypeService.Get(item.Id) ?? new uSupportTicketType();
		protected override string GetItemName(uSupportTicketType item) => item.Name;
	}
}
