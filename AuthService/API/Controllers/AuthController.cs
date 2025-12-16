using Application.DTOs.User;
using Application.Features.User.Commands.ConfirmEmail;
using Application.Features.User.Commands.ForgotPassword;
using Application.Features.User.Commands.Login;
using Application.Features.User.Commands.RefreshToken;
using Application.Features.User.Commands.Register;
using Application.Features.User.Commands.ResetPasswordCommand;
using Application.Features.User.Queries.GetInfoDetail;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using System.Security.Claims;

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

        [Route("GetnInfo")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<GetInfoDetailQuery>.ErrorResponse("User not authenticated"));
            }

            var result = await _mediator.Send(new GetInfoDetailQuery
            {
                UserId = userId
            });

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<InforDto>.SuccessResponse(result.Value));
            }
            return BadRequest(ApiResponse<InforDto>.ErrorResponse(
                result.Error.Code,
                new List<string> { result.Error.Message }
            ));
        }


        [Authorize]
        [HttpPost]
        [Route("send-confirmemail")]
        public async Task<IActionResult> SendConfirmEmail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(
                new GetInfoDetailQuery
                {
                    UserId = userId
                });

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.Code, new List<string> { result.Error.Message }));
            }

            return Ok(ApiResponse<string>.SuccessResponse("Email has been sent"));
        }

        
        [HttpGet]
        [Route("email-confirmation")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("EMPTY.TOKEN", new List<string> { "User Id and Token are required" }));
            }

            var result = await _mediator.Send(new ConfirmEmailCommand
            {
                UserId = userId,
                Token = token
            });

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<string>.SuccessResponse(result.Value, "Email confirmed successfully."));
            }

            return BadRequest(ApiResponse<string>.ErrorResponse(
                result.Error.Code,
                new List<string> { result.Error.Message }
            ));
        }


        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };

            return Challenge(properties, "Google");
        }


        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordCommand request)
        {
            
            var result = await _mediator.Send(new ForgotPasswordCommand
            {
                Email = request.Email,
                ClientUrl = request.ClientUrl
            });

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<string>.SuccessResponse(null,"A password reset link has been sent"));
            }

            return BadRequest(
                ApiResponse<string>.ErrorResponse(result.Error.Code, new List<string> { result.Error.Message }));
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(
                    ApiResponse<string>.ErrorResponse(
                        result.Error.Code,
                        new List<string> { result.Error.Message }
                    )
                );
            }

            return Ok(ApiResponse<string>.SuccessResponse(result.Value));
        }
    }
}
