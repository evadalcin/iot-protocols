namespace NetCoreClient.ValueObjects
{
    internal class AvailableWaterAmount
    {
        public int Value { get; private set; }

        public AvailableWaterAmount(int value)
        {
            this.Value = value;
        }
    }
}
