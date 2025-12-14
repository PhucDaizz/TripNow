using Application.DTOs.User;
using Application.Features.User.Commands.Login;
using Application.Features.User.Commands.RefreshToken;
using Application.Features.User.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> Register([FromBody]RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                var response = ApiResponse<string>.SuccessResponse(
                    result.Value,
                    "User created successfully."
                );

                return Ok(response);
            }

            return BadRequest(ApiResponse<string>.ErrorResponse(
                result.Error.Code,
                new List<string> { result.Error.Message }
            ));
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result.Value, "Login Success"));
            }
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(result.Error.Message));
        }

        [Route("Refreshtoken")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenCommand refreshTokenModel)
        {

            var result = await _mediator.Send(refreshTokenModel);
            if (result.IsFailure)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result.Value));
        }

    }
}
