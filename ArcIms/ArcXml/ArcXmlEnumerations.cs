//  Copyright 2012 Applied Geographics, Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  public enum ArcMapType
  {
    Group,
    Annotation,
    MultiPatch,
    None
  }
  
  public enum BufferUnits
	{
		DecimalDegrees,
		Miles,
		Feet,
		Kilometers,
		Meters,
		Default
	}

	public enum CapType
	{
		Butt,
		Round,
		Square
	}

  public enum ChartSymbolMode
  {
    Pie,
    Bar
  }

	public enum DatasetType
	{
		Point,
		Line,
		Polygon,
		Image,
		None
	}

	public enum ExactMethod
	{
		IsExact,
		IsContained
	}

	public enum ExtensionType
	{
		Geocode,
		StoredQuery,
		Extract
	}

	public enum FClassType
	{
		Point,
		Line,
		Polygon
	}

	public enum FieldType
	{
		None,
		RowID,
		Shape,
		Boolean,
		BigInteger,
		Character,
		Integer,
		SmallInteger,
		Float,
		Double,
		String,
		Clob,
		NVarChar,
		NClob,
		Date
	}

	public enum FillType
	{
		Solid,
		BDiagonal,
		FDiagonal,
		Cross,
		DiagCross,
		Horizontal,
		Vertical,
		Gray,
		LightGray,
		DarkGray
	}

	public enum FontStyle
	{
		Regular,
		Bold,
		Italic,
		Underline,
		Outline,
		BoldItalic
	}

  public enum GradientFillType
  {
    BDiagonal,
    FDiagonal,
    Horizontal,
    Vertical
  }

	public enum HashLineType
	{
		Foreground,
		Background
	}

	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	public enum HowManyLabels
	{
		OneLabelPerName,
		OneLabelPerShape,
		OneLabelPerPart
	}

	public enum ImageType
	{
		Default,
		AI,
		Bmp,
		Emf,
		Eps,
		Gif,
		Jpg,
		Pdf,
		Png,
		Png8,
		Png24
	}

	public enum JoinType
	{
		Round,
		Miter,
		Bevel
	}

	public enum LabelWeight
	{
		NoWeight,
		MedWeight,
		HighWeight
	}

	public enum LayerType
	{
		FeatureClass,
		Image,
		Acetate,
		None
	}

	public enum LineLabelPosition
	{
		PlaceAbove,
		PlaceBelow,
		PlaceOnTop,
		PlaceLeft,
		PlaceRight,
		PlaceAboveBelow,
		PlaceLeftRight,
		PlaceInLine,
		PlaceParallel,
		PlaceOnTopHorizontal,
		PlaceAtStartAbove,
		PlaceAtStartOnTop,
		PlaceAtStartBelow,
		PlaceAtEndAbove,
		PlaceAtEndOnTop,
		PlaceAtEndBelow,
		PlaceEitherEndAbove,
		PlaceEitherEndOnTop,
		PlaceEitherEndBelow
	}

	public enum LineType
	{
		Solid,
		Dash,
		Dot,
		DashDot,
		DashDotDot
	}

	public enum MarkerType
	{
		Circle,
		Triangle,
		Square,
		Cross,
		Star
	}

	public enum ObjectUnits
	{
		Database,
		Pixel
	}

  public enum ObjectAlignment
  {
    BottomLeft,
    BottomCenter,
    BottomRight,
    CenterLeft,
    Center,
    CenterRight,
    TopLeft,
    TopCenter,
    TopRight
  }

	public enum OutputMode
	{
		Binary,
		Xml,
		NewXml
	}

	public enum PageUnits
	{
		None,
		Inches,
		Feet,
		Yards,
		Centimeters,
		Millimeters,
		Decimeters,
		Meters,
		Points
	}

	public enum PrintMode
	{
		TitleCaps,
		AllUpper,
		AllLower,
		None
	}

	public enum RangeEquality
	{
		All,
		Upper,
		Lower,
		None
	}

  public enum ReturnGeometry
  {
    All,
    None,
    XmlMode
  }

	public enum RotateMethod
	{
		Geographic,
		Arithmetic,
		ModArithmetic
	}

	public enum ScaleBarMode
	{
		Cartesian,
		Geodesic
	}

	public enum ScaleUnits
	{
		Centimeter,
    Decimeter,
    Feet,
    Inches,
		Kilometers,
		Meters,
    Miles,
    Points,
    NauticalMiles,
    UKNauticalMiles,
    USNauticalMiles,
    USSurveyInches,
    USSurveyFeet,
    USSurveyMiles,
    USSurveyYards,
    Yards
  }

	public enum SearchOrder
	{
		Optimize,
		SpatialFirst,
		AttributeFirst
	}

	public enum ServiceAccess
	{
		Public,
		Private
	}

	public enum ServiceStatus
	{
		Enabled,
		Disabled
	}

	public enum ShieldType
	{
		Interstate,
		USRoad,
		Rect,
		Oval
	}

	public enum ShieldLabelMode
	{
		Full,
		NumericOnly
	}

  public enum ShowType
  {
    Layers,
    Strict,
    LayersStrict,
    None
  }

	public enum SpatialRelation
	{
		AreaIntersection,
		EnvelopeIntersection
	}

	public enum Units
	{
		Degrees,
		Feet,
		Meters
	}

	public enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}

	public static class ArcXmlEnumConverter
	{
		public static object ToEnum(Type type, string arcXmlValue)
		{
			switch (type.Name)
			{
				case "BufferUnits":
					switch (arcXmlValue)
					{
						case "decimal_degrees":
						  return BufferUnits.DecimalDegrees;
						case "":
							return BufferUnits.Default;
					}
					break;

				case "DatasetType":
					if (arcXmlValue.Length == 0)
					{
						return DatasetType.None;
					}
					break;

				case "FieldType":
					switch (arcXmlValue)
					{
						case "-99": return FieldType.RowID;
						case "-98": return FieldType.Shape;
						case "-7": return FieldType.Boolean;
						case "-5": return FieldType.BigInteger;
						case "1": return FieldType.Character;
						case "4": return FieldType.Integer;
						case "5": return FieldType.SmallInteger;
						case "6": return FieldType.Float;
						case "8": return FieldType.Double;
						case "12": return FieldType.String;
						case "13": return FieldType.Clob;
						case "14": return FieldType.NVarChar;
						case "15": return FieldType.NClob;
						case "91": return FieldType.Date;
						default: return FieldType.None;
					}

				case "HowManyLabels":
					switch (arcXmlValue)
					{
						case "one_label_per_name": return HowManyLabels.OneLabelPerName;
						case "one_label_per_shape": return HowManyLabels.OneLabelPerShape;
						case "one_label_per_part": return HowManyLabels.OneLabelPerPart;
					}
					break;

				case "ImageType":
					if (arcXmlValue.Length == 0)
					{
						return ImageType.Default;
					}
					break;

				case "LabelWeight":
					switch (arcXmlValue)
					{
						case "no_weight": return LabelWeight.NoWeight;
						case "med_weight": return LabelWeight.MedWeight;
						case "high_weight": return LabelWeight.HighWeight;
					}
					break;

				case "LayerType":
					if (arcXmlValue.Length == 0)
					{
						return LayerType.None;
					}
					break;

				case "LineType":
					switch (arcXmlValue)
					{
						case "dash_dot": return LineType.DashDot;
						case "dash_dot_dot": return LineType.DashDotDot;
					}
					break;

        case "ObjectAlignment":
          switch (arcXmlValue)
          {
            case "bottom_left": return ObjectAlignment.BottomLeft;
            case "bottom_center": return ObjectAlignment.BottomCenter;
            case "bottom_right": return ObjectAlignment.BottomRight;
            case "center_left": return ObjectAlignment.CenterLeft;
            case "center_right": return ObjectAlignment.CenterRight;
            case "top_left": return ObjectAlignment.TopLeft;
            case "top_center": return ObjectAlignment.TopCenter;
            case "top_right": return ObjectAlignment.TopRight;
          }
          break;

				case "PageUnits":
					if (arcXmlValue.Length == 0)
					{
						return PageUnits.None;
					}
					break;

				case "RotateMethod":
					switch (arcXmlValue)
					{
						case "mod_arithmetic": return RotateMethod.ModArithmetic;
					}
					break;

        case "ScaleUnits":
          switch (arcXmlValue)
          {
            case "nautical_miles": return ScaleUnits.NauticalMiles;
            case "uk_nautical_miles": return ScaleUnits.UKNauticalMiles;
            case "us_nautical_miles": return ScaleUnits.USNauticalMiles;
            case "us_survey_inches": return ScaleUnits.USSurveyInches;
            case "us_survey_feet": return ScaleUnits.USSurveyFeet;
            case "us_survey_miles": return ScaleUnits.USSurveyMiles;
            case "us_survey_yards": return ScaleUnits.USSurveyYards;
          }
          break;

        case "ShowType":
          switch (arcXmlValue)
          {
            case "layers strict": return ShowType.LayersStrict;
          }
          break;

				case "SpatialRelation":
					switch (arcXmlValue)
					{
						case "area_intersection": return SpatialRelation.AreaIntersection;
						case "envelope_intersection": return SpatialRelation.EnvelopeIntersection;
					}
					break;

				case "Units":
					switch (arcXmlValue)
          {
            case "decimal_degrees": return Units.Degrees;
            case "us_survey_feet": return Units.Feet;
          }
					break;
			}

			return Enum.Parse(type, arcXmlValue, true);
		}

		public static string ToArcXml(Type type, object enumValue)
		{
			switch (type.Name)
			{
				case "BufferUnits":
					switch ((BufferUnits)enumValue)
					{
						case BufferUnits.DecimalDegrees: return "decimal_degrees";
						case BufferUnits.Default: return "";
					}
					break;

				case "DatasetType":
					if ((DatasetType)enumValue == DatasetType.None)
					{
						return "";
					}
					break;
        
        case "FieldType":
					switch ((FieldType)enumValue)
					{
						case FieldType.RowID: return "-99";
						case FieldType.Shape: return "-98";
						case FieldType.Boolean: return "-7";
						case FieldType.BigInteger: return "-5";
						case FieldType.Character: return "1";
						case FieldType.Integer: return "4";
						case FieldType.SmallInteger: return "5";
						case FieldType.Float: return "6";
						case FieldType.Double: return "8";
						case FieldType.String: return "12";
						case FieldType.Date: return "91";
						default: return "";
					}

				case "HowManyLabels":
					switch ((HowManyLabels)enumValue)
					{
						case HowManyLabels.OneLabelPerName: return "one_label_per_name";
						case HowManyLabels.OneLabelPerShape: return "one_label_per_shape";
						case HowManyLabels.OneLabelPerPart: return "one_label_per_part";
					}
					break;

				case "ImageType":
					if ((ImageType)enumValue == ImageType.Default)
					{
						return "";
					}
					break;

				case "LabelWeight":
					switch ((LabelWeight)enumValue)
					{
						case LabelWeight.NoWeight: return "no_weight";
						case LabelWeight.MedWeight: return "med_weight";
						case LabelWeight.HighWeight: return "high_weight";
					}
					break;

				case "LayerType":
					if ((LayerType)enumValue == LayerType.None)
					{
						return "";
					}
					break;

				case "LineType":
					switch ((LineType)enumValue)
					{
						case LineType.DashDot: return "dash_dot";
						case LineType.DashDotDot: return "dash_dot_dot";
					}
					break;

        case "ObjectAlignment":
          switch ((ObjectAlignment)enumValue)
          {
            case ObjectAlignment.BottomLeft: return "bottom_left";
            case ObjectAlignment.BottomCenter: return "bottom_center";
            case ObjectAlignment.BottomRight: return "bottom_right";
            case ObjectAlignment.CenterLeft: return "center_left";
            case ObjectAlignment.CenterRight: return "center_right";
            case ObjectAlignment.TopLeft: return "top_left";
            case ObjectAlignment.TopCenter: return "top_center";
            case ObjectAlignment.TopRight: return "top_right";
          }
          break;

				case "PageUnits":
					if ((PageUnits)enumValue == PageUnits.None)
					{
						return "";
					}
					break;

				case "RotateMethod":
					switch ((RotateMethod)enumValue)
					{
						case RotateMethod.ModArithmetic: return "mod_arithmetic";
					}
					break;

        case "ScaleUnits":
          switch ((ScaleUnits)enumValue)
          {
            case ScaleUnits.NauticalMiles: return "nautical_miles";
            case ScaleUnits.UKNauticalMiles: return "uk_nautical_miles";
            case ScaleUnits.USNauticalMiles: return "us_nautical_miles";
            case ScaleUnits.USSurveyInches: return "us_survey_inches";
            case ScaleUnits.USSurveyFeet: return "us_survey_feet";
            case ScaleUnits.USSurveyMiles: return "us_survey_miles";
            case ScaleUnits.USSurveyYards: return "us_survey_yards";
          }
          break;

        case "ShowType":
          switch ((ShowType)enumValue)
          {
            case ShowType.LayersStrict: return "layers strict";
          }
          break;

				case "SpatialRelation":
					switch ((SpatialRelation)enumValue)
					{
						case SpatialRelation.AreaIntersection: return "area_intersection";
						case SpatialRelation.EnvelopeIntersection: return "envelope_intersection";
					}
					break;

				case "Units":
					if ((Units)enumValue == Units.Degrees)
					{
						return "decimal_degrees";
					}
					break;
			}

			return Enum.GetName(type, enumValue).ToLower();
		}
	}
}
