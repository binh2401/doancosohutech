using WebBanHang.Models;

namespace WebBanHang.Services
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
		VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
	}
}
