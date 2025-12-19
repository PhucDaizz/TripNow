using Application.Common.Interfaces;
using Application.Contracts;
using Application.DTOs.User;
using Domain.Common;
using Domain.Common.Response;
using MediatR;
using Nexus.BuildingBlocks.Interfaces;
using RabbitMQ.Client;

namespace Application.Features.User.Commands.RegisterHotelOwner
{
    public class RegisterHotelOwnerCommandHandler : IRequestHandler<RegisterHotelOwnerCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IMessagePublisher _messagePublisher;

        public RegisterHotelOwnerCommandHandler(IUnitOfWork unitOfWork, IEmailServices emailServices, IIdentityService identityService, IMessagePublisher messagePublisher)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _messagePublisher = messagePublisher;
        }

        public async Task<Result<string>> Handle(RegisterHotelOwnerCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _unitOfWork.Auth.GetUserByEmailAsync(command.Email);
            if (existingUser != null)
            {
                return Result.Failure<string>(new Error("Email.Exists", "Email already exists."));
            }

            var newUser = await _identityService.CreateUserAsync(new Register.RegisterCommand
            {
                Email = command.Email,
                Password = command.Password,
                FullName = command.FullName
            });

            if (newUser.IsFailure)
            {
                return Result.Failure<string>(new Error("Create.User.Error", string.Join(",", newUser.Error.Message)));
            }

            await _identityService.AssignRoleAsync(newUser.Value.Id, AppRoles.HotelOwner);

            await _messagePublisher.PublishAsync(
                    exchange: "user.events",
                    exchangeType: ExchangeType.Topic,
                    routingKey: "user.registered",
                    message: new SendEmailConfirmation
                    {
                        UserId = newUser.Value.Id,
                        Email = command.Email,
                        FullName = command.FullName
                    });

            return Result.Success("Create user successfully");
        }
    }
}
