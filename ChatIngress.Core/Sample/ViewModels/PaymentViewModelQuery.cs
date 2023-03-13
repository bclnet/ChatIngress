using MediatR;

namespace Sample.ViewModels
{
    public class PaymentViewModelQuery : IRequest<PaymentViewModel>
    {
        public int? AccountID { get; set; }
        public int? PageID { get; set; }
    }
}
