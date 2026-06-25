"""ArcGIS Pro ArcPy conversion entry point for KML/KMZ files."""

import argparse
import os
import sys

import arcpy


def parse_args():
    parser = argparse.ArgumentParser(description="Convert a KML/KMZ file with ArcGIS Pro's KMLToLayer tool.")
    parser.add_argument("input_path", help="Path to the source KML or KMZ file.")
    parser.add_argument("output_directory", help="Directory where ArcGIS Pro should write conversion output.")
    parser.add_argument("output_name", help="Base name for the generated ArcGIS layer output.")
    return parser.parse_args()


def validate_args(input_path, output_directory, output_name):
    if not os.path.isfile(input_path):
        raise FileNotFoundError(f"Input file was not found: {input_path}")

    extension = os.path.splitext(input_path)[1].lower()
    if extension not in {".kml", ".kmz"}:
        raise ValueError(f"Unsupported input extension for ArcGIS conversion: {extension}")

    if not output_name.strip():
        raise ValueError("Output name is required.")

    os.makedirs(output_directory, exist_ok=True)


def convert(input_path, output_directory, output_name):
    arcpy.env.overwriteOutput = True
    arcpy.conversion.KMLToLayer(input_path, output_directory, output_name)


def main():
    args = parse_args()
    input_path = os.path.abspath(args.input_path)
    output_directory = os.path.abspath(args.output_directory)
    output_name = args.output_name.strip()

    validate_args(input_path, output_directory, output_name)
    print(f"ArcPy KMLToLayer started: {input_path}")
    convert(input_path, output_directory, output_name)
    print(f"ArcPy KMLToLayer completed: {os.path.join(output_directory, output_name)}")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        print(f"ArcPy KMLToLayer failed: {exc}", file=sys.stderr)
        raise SystemExit(1)
