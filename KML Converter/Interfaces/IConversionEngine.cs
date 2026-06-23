namespace GISUniversalConverterPro.Interfaces
{
    public interface IConversionEngine
    {
        string Name { get; }
        bool IsAvailable { get; }

        void Initialize();
        void Convert();
        bool Validate();
        void Cancel();
    }
}
