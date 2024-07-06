using MDS.Shared.Core.Enums;

namespace MDS.Services.Common
{
    public class BaseResponse
    {
        public ResponseCode StatusCode { get; set; } = ResponseCode.OK;

        public string? Message { get; set; }
    }
}
