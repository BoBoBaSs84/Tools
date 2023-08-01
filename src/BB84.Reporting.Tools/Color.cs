using System.Globalization;

namespace BB84.Reporting.Tools;

/// <summary>
/// The color class.
/// </summary>
/// <remarks>
/// Contains everything related to coloring.
/// </remarks>
public class Color
{
	private static readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;

	/// <summary>
	/// Returns the string represenation of a value as RGB color code.
	/// </summary>
	/// <param name="minValue">The minimum value.</param>
	/// <param name="maxValue">The maximum value.</param>
	/// <param name="value">The actual value to represent.</param>
	public static string GetRgbHexString(double minValue, double maxValue, double value)
	{
		RgbValues rgbValues = GetRgbValues(minValue, maxValue, value);

		return string.Concat(
			"#",
			rgbValues.Red.ToString("X2", CultureInfo),
			rgbValues.Green.ToString("X2", CultureInfo),
			rgbValues.Blue.ToString("X2", CultureInfo)
			);
	}

	private static RgbValues GetRgbValues(double minValue, double maxValue, double value)
	{
		double normalizedValue = Normalize(minValue, maxValue, value);

		return new RgbValues(
			red: Distance(normalizedValue, 2),
			green: Distance(normalizedValue, 1),
			blue: Distance(normalizedValue, 0)
			);
	}

	private static double Normalize(double minValue, double maxValue, double value)
		=> (value - minValue) / (maxValue - minValue) * 2;

	private static int Distance(double value, double color)
	{
		double distance = Math.Abs(value - color);

		double colorStrength = 1 - distance;

		if (colorStrength < 0)
			colorStrength = 0;

		return (int)Math.Round(colorStrength * 255);
	}

	private sealed class RgbValues
	{
		public RgbValues(int red, int green, int blue)
		{
			Red = red;
			Green = green;
			Blue = blue;
		}

		public int Red { get; }
		public int Green { get; }
		public int Blue { get; }
	}
}
