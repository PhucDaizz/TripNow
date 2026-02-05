using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.EscrowAccount;
using PaymentService.Domain.Events.EscrowAccount;

namespace PaymentService.Application.Features.EscrowAccount.EventHandlers
{
    public class EscrowCreatedEventHandler : INotificationHandler<EscrowCreatedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public EscrowCreatedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(EscrowCreatedEvent notification, CancellationToken cancellationToken)
        {
            var escrowCreatedIntegrationEvent = new EscrowCreated
            {
                BookingId = notification.BookingId
            };

            await _integrationEventService.PublishAsync<EscrowCreated>(escrowCreatedIntegrationEvent, "payment.events", "topic", "payment.success", cancellationToken);
        }
    }
}
