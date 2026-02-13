using Application.Common.Interfaces;
using Application.DTOs.StaffProfile;
using Application.DTOs.User;
using Application.Features.StaffProfile.Commands.CreateStaffProfile;
using Application.Features.StaffProfile.Commands.DeleteStaffProfile;
using Application.Features.StaffProfile.Commands.UpdateStaffProfile;
using Application.Features.StaffProfile.Queries.GetStaffProfile;
using Application.Features.User.Commands.ConfirmEmail;
using Application.Features.User.Commands.ForgotPassword;
using Application.Features.User.Commands.Login;
using Application.Features.User.Commands.RefreshToken;
using Application.Features.User.Commands.Register;
using Application.Features.User.Commands.RegisterHotelOwner;
using Application.Features.User.Commands.ResetPasswordCommand;
using Application.Features.User.Commands.UpdateInfor;
using Application.Features.User.Commands.UploadAvatar;
using Application.Features.User.Queries.GetInfoDetail;
using Application.Features.User.Queries.GetUsersWithPagination;
using Application.Features.User.Queries.IsUserExisting;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using System.Security.Claims;
using System.Threading;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Route("register-customer")]
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


        [Authorize(Roles = AppRoles.SysAdmin)]
        [HttpPost("register-hotel-owner")]
        public async Task<IActionResult> RegisterHotelOwner(
            RegisterHotelOwnerCommand command, CancellationToken cancellationToken)
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

        [HttpGet("me")]
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

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateInfor([FromBody]UpdateInforCommand command)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not found!"));
            }

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<string>.SuccessResponse(result.Value, "User information updated successfully."));
            }
            return BadRequest(ApiResponse<string>.ErrorResponse(
                result.Error.Code,
                new List<string> { result.Error.Message }
            ));

        }

        [HttpPost("upload-avatar")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
        [RequestFormLimits(MultipartBodyLengthLimit = 5 * 1024 * 1024)]
        public async Task<IActionResult> UploadAvatar(IFormFile file, [FromQuery] bool deleteOld = true)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var result = await _mediator.Send(new UploadAvatarCommand
                {
                    File = file,
                    UserId = userId,
                    DeleteOldAvatar = deleteOld,
                    OptimizeImage = true
                });

                return Ok(ApiResponse<UploadAvatarResult>.SuccessResponse(result.Value, "Avatar uploaded successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("UPLOAD_FAILED", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = AppRoles.SysAdmin)]
        public async Task<IActionResult> GetUserInfo(string userId)
        {
            var result = await _mediator.Send(new GetInfoDetailQuery
            {
                UserId = userId
            });

            return result.IsSuccess
                ? Ok(ApiResponse<InforDto>.SuccessResponse(result.Value))
                : NotFound(ApiResponse<InforDto>.ErrorResponse(result.Error.Code));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersWithPaginationQuery query)
        {
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            var currentHotelId = _currentUserService.HotelId;

            if (currentRole == AppRoles.SysAdmin)
            {
            }
            else if (currentRole == AppRoles.HotelOwner)
            {
                if (currentHotelId == null)
                {
                    return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResponse(PagedResult<UserDto>.Empty()));
                }

                query.HotelId = currentHotelId;
            }
            else if (currentRole == AppRoles.Receptionist)
            {
                if (currentHotelId == null)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Staff account implies a hotel association but none found."));
                }
                query.HotelId = currentHotelId;
            }
            else
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<Domain.Common.Models.PagedResult<UserDto>>.SuccessResponse(result.Value));
        }



        [HttpPost("staff-profile")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner}")]
        public async Task<IActionResult> CreateStaffProfile(
            [FromBody]CreateStaffProfileDto command)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newStaffProfile = new CreateStaffProfileCommand
            {
                Email = command.Email,
                HotelId = command.HotelId,
                Position = command.Position,
                CreatedByUserId = currentUserId
            };

            var result = await _mediator.Send(newStaffProfile);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<StaffProfileDto>.SuccessResponse(
                    result.Value,
                    "Staff profile created successfully"
                ));
            }

            return BadRequest(ApiResponse<string>.ErrorResponse(
                result.Error.Message,
                new List<string> { result.Error.Code }
            ));
        }


        [HttpDelete("staff/{userid}")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner}")]
        public async Task<IActionResult> DeleteStaffProfile(Guid userid)
        {
            var command = new DeleteStaffProfileCommand
            {
                UserId = userid,
                DeletedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return NoContent();

            return BadRequest(result.Error.Message);
        }

        [HttpPut("staff/{userId}")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner}")]
        public async Task<IActionResult> UpdateStaffProfile(
            Guid userId,
            [FromBody] UpdateStaffProfileDto command)
        {

            var staffProfileUpdate = new UpdateStaffProfileCommand
            {
                StaffProfileId = userId,
                NewPosition = command.NewPosition,
                HotelId = command.HotelId,
                UpdatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            var result = await _mediator.Send(staffProfileUpdate);

            if (result.IsSuccess)
                return Ok(ApiResponse<StaffProfileDto>.SuccessResponse(result.Value));

            return BadRequest(ApiResponse<StaffProfileDto>.ErrorResponse(result.Error.Message));
        }

        [HttpGet("staff-profile")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner},{AppRoles.Housekeeping},{AppRoles.Receptionist}")]
        public async Task<IActionResult> GetStaffProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new GetStaffProfileQuery { UserId = userId });

            if (result.IsFailure)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Employee information not found."));
            }

            return Ok(ApiResponse<StaffProfileDto>.SuccessResponse(result.Value));
        }

        [HttpGet("user-existing")]
        public async Task<IActionResult> IsUsserExisting([FromQuery]Guid userId)
        {
            var request = new IsUserExistingQuery
            {
                UserId = userId
            };

            var result = await _mediator.Send(request);
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }
    }
}
