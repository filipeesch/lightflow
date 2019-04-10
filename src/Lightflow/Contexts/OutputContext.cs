namespace Lightflow.Contexts
{
    public class OutputContext<TOutput> : ILightflowContext
    {
        public TOutput Output { get; set; }
    }
}
