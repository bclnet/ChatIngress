namespace Infrastructure.Mediator
{
    public abstract class HandlerBase
    {
        protected HandlerBase(HandlerContext context) => Context = context;
        protected HandlerContext Context { get; }
    }
}
