using MDS.Services.DTO.VnPay;

namespace MDS.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequest request);
        VnPayResponse PaymentExecute(IQueryCollection collections);

    }
}
