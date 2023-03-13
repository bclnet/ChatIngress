namespace Infrastructure.Mediator
{
    public abstract class CommandHandlerBase : HandlerBase
    {
        protected CommandHandlerBase(HandlerContext context) : base(context) { }
    }
}
