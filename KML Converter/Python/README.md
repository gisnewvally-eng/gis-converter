# ArcGIS Python integration

`arcgis_kml_to_layer.py` is executed by `ArcGISEngine` with the ArcGIS Pro Python environment.
It expects ArcPy to be available and runs `arcpy.conversion.KMLToLayer` for one KML/KMZ input.

Arguments:

1. Source KML/KMZ path
2. Output directory
3. Output layer name
