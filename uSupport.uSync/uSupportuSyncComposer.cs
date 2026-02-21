using System;
using System.Linq;
using System.Text;
using Umbraco.Cms.Core;
using uSync.BackOffice;
using System.Threading.Tasks;
using uSupport.Notifications;
using uSupport_uSync.Handlers;
using uSync.BackOffice.Models;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace uSupport_uSync
{
	[ComposeBefore(typeof(uSyncBackOfficeComposer))]
	public class uSupportuSyncComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.AddNotificationHandler<CreateTicketTypeNotification, TicketTypeHandler>();
			builder.AddNotificationHandler<UpdateTicketTypeNotification, TicketTypeHandler>();
			builder.AddNotificationHandler<DeleteTicketTypeNotification, TicketTypeHandler>();

			builder.AddNotificationHandler<CreateTicketStatusNotification, TicketStatusHandler>();
			builder.AddNotificationHandler<UpdateTicketStatusNotification, TicketStatusHandler>();
			builder.AddNotificationHandler<DeleteTicketStatusNotification, TicketStatusHandler>();
		}
	}
}
