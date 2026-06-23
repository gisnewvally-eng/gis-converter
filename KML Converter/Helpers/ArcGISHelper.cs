namespace GISUniversalConverterPro.Helpers
{
    public static class ArcGISHelper
    {
        public static string GetArcGISStatusMessage(bool isInstalled)
        {
            return isInstalled ? "ArcGIS Pro detected" : "ArcGIS Pro not detected; internal engine will be used";
        }
    }
}
