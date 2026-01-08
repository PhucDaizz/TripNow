using BookingService.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public BookingController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _currentUserService = currentUserService;
            _mediator = mediator;
        }


    }
}
