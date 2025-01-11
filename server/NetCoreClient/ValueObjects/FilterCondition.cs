namespace NetCoreClient.ValueObjects
{
    internal class FilterCondition
    {
        public bool Value { get; private set; }

        public FilterCondition(bool value)
        {
            this.Value = value;
        }
    }
}
