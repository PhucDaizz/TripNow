using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.RoomType;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomType.Queries.GetRoomTypesByHotel
{
    public class GetRoomTypesByHotelQueryHandler : IRequestHandler<GetRoomTypesByHotelQuery, Result<List<RoomTypeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _context;

        public GetRoomTypesByHotelQueryHandler(IUnitOfWork unitOfWork, IApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Result<List<RoomTypeDto>>> Handle(GetRoomTypesByHotelQuery request, CancellationToken cancellationToken)
        {
            var targetDate = request.CheckInDate ?? DateTime.Today;

            var hotel = await _unitOfWork.Hotel.GetHotelCatalogAsync(request.HotelId, targetDate, cancellationToken);

            if (hotel == null) return Result.Failure<List<RoomTypeDto>>(new Error("Hotel.NotFound", "Not found"));


            var dtos = hotel.RoomTypes.Select(rt =>
            {
                var specialPriceObj = rt.Prices.FirstOrDefault();
                decimal finalPrice = specialPriceObj?.Price ?? rt.BasePrice;
                bool isDiscounted = specialPriceObj != null && finalPrice != rt.BasePrice;

                if (specialPriceObj != null)
                {
                    finalPrice = specialPriceObj.Price;
                    isDiscounted = finalPrice != rt.BasePrice;
                }
                else
                {
                    finalPrice = rt.BasePrice;
                }

                return new RoomTypeDto
                {
                    Id = rt.Id,
                    Name = rt.Name,

                    BasePrice = rt.BasePrice,      
                    CurrentPrice = finalPrice,     
                    IsDiscounted = isDiscounted,   

                    Capacity = rt.Capacity,
                    SizeM2 = rt.SizeM2,
                    MainImage = rt.Images.FirstOrDefault(i => i.IsMainImage)?.ImageUrl,
                    CancellationPolicy = rt.CancellationPolicy == null ? null : new DTOs.CancellationPolicy.CancellationPolicyDto
                    {
                        Id = rt.CancellationPolicy.Id,
                        Name = rt.CancellationPolicy.Name,
                        Description = rt.CancellationPolicy.Description,
                        Type = rt.CancellationPolicy.Type.ToString(),

                        Rules = rt.CancellationPolicy.Rules
                            .OrderByDescending(r => r.HoursBeforeCheckIn)
                            .Select(r => new DTOs.CancellationPolicy.CancellationRuleDto
                            {
                                Id = r.Id,
                                HoursBeforeCheckIn = r.HoursBeforeCheckIn,
                                RefundPercentage = r.RefundPercentage
                            }).ToList()
                    }
                };
            }).ToList();

            return Result.Success(dtos);
        }
    }
}
