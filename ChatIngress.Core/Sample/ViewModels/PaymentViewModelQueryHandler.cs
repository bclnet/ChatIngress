using Infrastructure.Mediator;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class PaymentViewModelQueryHandler : QueryHandlerBase, IRequestHandler<PaymentViewModelQuery, PaymentViewModel>
    {
        public PaymentViewModelQueryHandler(HandlerContext context) : base(context) { }

        public Task<PaymentViewModel> Handle(PaymentViewModelQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new PaymentViewModel
            {
            });
        }
    }
}
