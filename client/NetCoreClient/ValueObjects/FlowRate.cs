namespace NetCoreClient.ValueObjects
{
    internal class FlowRate
    {
        public int Value { get; private set; }

        public FlowRate(int value)
        {
            this.Value = value;
        }

    }
}
