using GISUniversalConverterPro.Interfaces;

namespace GISUniversalConverterPro.Engines
{
    public sealed class ArcGISEngine : IConversionEngine
    {
        public string Name => "ArcGIS Pro Engine";
        public bool IsAvailable => false;

        public void Initialize()
        {
        }

        public void Convert()
        {
        }

        public bool Validate()
        {
            return false;
        }

        public void Cancel()
        {
        }
    }
}
