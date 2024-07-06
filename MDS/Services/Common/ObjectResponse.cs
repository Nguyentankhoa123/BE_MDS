using System.Text.Json.Serialization;

namespace MDS.Services.Common
{
    public class ObjectResponse<T> : BaseResponse where T : class
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
        public int? TotalPages { get; set; }
    }
}
