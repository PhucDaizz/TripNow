using ChatService.Application.Common.Interfaces;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.ChatBot.Queries
{
    public class AskHotelBotQueryHandler : IRequestHandler<AskHotelBotQuery, Result<string>>
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IAiChatService _aiChatService;

        public AskHotelBotQueryHandler(
            IRecommendationService recommendationService,
            IAiChatService aiChatService)
        {
            _recommendationService = recommendationService;
            _aiChatService = aiChatService;
        }

        public async Task<Result<string>> Handle(AskHotelBotQuery request, CancellationToken cancellationToken)
        {
            var contextChunks = await _recommendationService.GetHotelChatContextAsync(
                request.HotelId,
                request.Message,
                limit: 3,
                cancellationToken);

            var contextString = contextChunks.Any()
                ? string.Join("\n\n", contextChunks)
                : "Không tìm thấy tài liệu nào khớp với câu hỏi của khách.";

            var systemPrompt = $@"Bạn là Lễ tân ảo chuyên nghiệp của khách sạn.

            QUY TẮC TỐI THƯỢNG:
            1. Xưng 'em' và gọi khách là 'anh/chị'. Thái độ niềm nở, dùng emoji lịch sự.
            2. CHỈ trả lời dựa vào [THÔNG TIN KHÁCH SẠN] bên dưới. TUYỆT ĐỐI KHÔNG bịa đặt.
            3. Nếu thông tin không có trong tài liệu, phải xin lỗi và đề nghị khách liên hệ hotline.
            4. Trình bày ngắn gọn. In đậm số liệu, giá cả, thời gian.

            [THÔNG TIN KHÁCH SẠN]
            {contextString}";

            var aiReply = await _aiChatService.GetChatCompletionAsync(systemPrompt, request.Message, cancellationToken);

            return Result.Success(aiReply);
        }
    }
}
