using uSync.Core;
using System.Xml.Linq;
using uSupport.Helpers;
using uSync.Core.Models;
using uSupport.Dtos.Tables;
using uSync.Core.Serialization;
using Microsoft.Extensions.Logging;
using uSupport.Services.Interfaces;

namespace uSupport_uSync.Serializers
{
	[SyncSerializer("EC57BC65-93E8-4939-B9ED-75ED9B7B4AF6", "uSupport Ticket status Serializer", "uSupportTicketStatus")]
	public class TicketStatusSerializer : SyncSerializerRoot<uSupportTicketStatus>, ISyncSerializer<uSupportTicketStatus>
	{
		private readonly IuSupportTicketStatusService _ticketStatusService;

		public TicketStatusSerializer(IuSupportTicketStatusService ticketStatusService,
			ILogger<SyncSerializerRoot<uSupportTicketStatus>> logger) : base(logger)
		{
			_ticketStatusService = ticketStatusService;
		}


		protected override SyncAttempt<XElement> SerializeCore(uSupportTicketStatus item, SyncSerializerOptions options)
		{
			var node = new XElement("uSupportTicketStatus",
				new XAttribute("Key", item.Id),
				new XAttribute("Alias", item.Alias),
				new XElement("Name", item.Name),
				new XElement("Color", item.Color),
				new XElement("Icon", item.Icon),
				new XElement("Order", item.Order),
				new XElement("Default", item.Default),
				new XElement("Active", item.Active));

			return SyncAttempt<XElement>.Succeed(item.Name, node, typeof(uSupportTicketStatus), ChangeType.Export);
		}

		protected override SyncAttempt<uSupportTicketStatus> DeserializeCore(XElement node, SyncSerializerOptions options)
		{
			var item = FindItem(node);

			var schema = new uSupport.Migrations.Schemas.uSupportTicketStatusSchema
			{
				Id = item == null ? node.GetKey() : item.Id,
				Alias = node.GetAlias(),

				Name = node.Element("Name")?.Value ?? string.Empty,
				Color = node.Element("Color")?.Value ?? string.Empty,
				Icon = node.Element("Icon")?.Value ?? string.Empty,
				Order = node.Element("Order")?.ValueOrDefault(0) ?? 0,
				Default = node.Element("Default")?.ValueOrDefault(false) ?? false,
				Active = node.Element("Active")?.ValueOrDefault(true) ?? true
			};


			item = item == null ? _ticketStatusService.Create(schema) :
									_ticketStatusService.Update(schema);

			return SyncAttempt<uSupportTicketStatus>.Succeed(item.Name, item, ChangeType.Import, Array.Empty<uSyncChange>());
		}

		public override void DeleteItem(uSupportTicketStatus item) => _ticketStatusService.Delete(item.Id);
		public override uSupportTicketStatus FindItem(int id) => null;
		public override uSupportTicketStatus FindItem(Guid key) => _ticketStatusService.Get(key);
		public override uSupportTicketStatus FindItem(string alias) => _ticketStatusService.GetAll().FirstOrDefault(x => x.Alias == alias);
		public override string ItemAlias(uSupportTicketStatus item) => item.Alias;
		public override Guid ItemKey(uSupportTicketStatus item) => item.Id;
		public override void SaveItem(uSupportTicketStatus item) => _ticketStatusService.Update(item.ConvertDtoToSchema());
	}
}
