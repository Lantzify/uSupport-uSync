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
	[SyncSerializer("57C9BD64-EA8C-49EA-AEF1-A86A4B438CF2", "uSupport Ticket type Serializer", "uSupportTicketType")]
	public class TicketTypeSerializer : SyncSerializerRoot<uSupportTicketType>, ISyncSerializer<uSupportTicketType>
	{
		private readonly IuSupportTicketTypeService _ticketTypeService;

		public TicketTypeSerializer(IuSupportTicketTypeService ticketTypeService, ILogger<SyncSerializerRoot<uSupportTicketType>> logger) : base(logger)
		{
			_ticketTypeService = ticketTypeService;
		}

		protected override Task<SyncAttempt<XElement>> SerializeCoreAsync(uSupportTicketType item, SyncSerializerOptions options)
		{
			var node = new XElement("uSupportTicketType",
				new XAttribute("Key", item.Id),
				new XAttribute("Alias", item.Alias),
				new XElement("Name", item.Name),
				new XElement("Description", item.Description),
				new XElement("Color", item.Color),
				new XElement("Icon", item.Icon),
				new XElement("Order", item.Order),
				new XElement("PropertyId", item.PropertyId),
				new XElement("PropertyName", item.PropertyName),
				new XElement("PropertyDescription", item.PropertyDescription),
				new XElement("PropertyView", item.PropertyView),
				item.DataTypeKey == null ? null : new XElement("DataTypeKey", item.DataTypeKey));

			return Task.FromResult(SyncAttempt<XElement>.Succeed(item.Name, node, typeof(uSupportTicketType), ChangeType.Export));
		}

		protected override async Task<SyncAttempt<uSupportTicketType>> DeserializeCoreAsync(XElement node, SyncSerializerOptions options)
		{
			var item = await FindItemAsync(node);

			var dto = new uSupportTicketType
			{
				Id = item == null ? node.GetKey() : item.Id,
				Alias = node.GetAlias(),
				Name = node.Element("Name")?.Value ?? string.Empty,
				Description = node.Element("Description")?.Value ?? string.Empty,
				Color = node.Element("Color")?.Value ?? string.Empty,
				Icon = node.Element("Icon")?.Value ?? string.Empty,
				Order = node.Element("Order")?.ValueOrDefault(0) ?? 0,
				PropertyId = node.Element("PropertyId")?.ValueOrDefault(0) ?? 0,
				PropertyName = node.Element("PropertyName")?.Value ?? string.Empty,
				PropertyDescription = node.Element("PropertyDescription")?.Value ?? string.Empty,
				PropertyView = node.Element("PropertyView")?.Value ?? string.Empty,
				DataTypeKey = (Guid?)node.Element("DataTypeKey")
			};

			item = item == null ? _ticketTypeService.Create(dto.ConvertDtoToSchema()) : dto;

			return SyncAttempt<uSupportTicketType>.Succeed(item.Name, item, ChangeType.Import, Array.Empty<uSyncChange>());
		}

		public override Task DeleteItemAsync(uSupportTicketType item) { _ticketTypeService.Delete(item.Id); return Task.CompletedTask; }
		public override Task<uSupportTicketType?> FindItemAsync(Guid key) => Task.FromResult<uSupportTicketType?>(_ticketTypeService.Get(key));
		public override Task<uSupportTicketType?> FindItemAsync(string alias) => Task.FromResult(_ticketTypeService.GetAll().FirstOrDefault(x => x.Alias == alias));
		public override string ItemAlias(uSupportTicketType item) => item.Alias;
		public override Guid ItemKey(uSupportTicketType item) => item.Id;
		public override Task SaveItemAsync(uSupportTicketType item) { _ticketTypeService.Update(item.ConvertDtoToSchema()); return Task.CompletedTask; }
	}
}