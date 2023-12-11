using System.Globalization;

namespace BB84.Reporting.Tools;

/// <summary>
/// The color class.
/// </summary>
/// <remarks>
/// Contains everything related to coloring.
/// </remarks>
public class Coloring
{
	private static readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;

	#region public methods

	/// <summary>
	/// Returns the string represenation of a value as RGB color code.
	/// </summary>
	/// <param name="minValue">The minimum value.</param>
	/// <param name="maxValue">The maximum value.</param>
	/// <param name="value">The actual value to represent.</param>
	public static string GetHeatMapColorString(int minValue, int maxValue, int value)
		=> GetHeatMapColorString(minValue, maxValue, (double)value);

	/// <summary>
	/// Returns the string represenation of a value as RGB color code.
	/// </summary>
	/// <param name="minValue">The minimum value.</param>
	/// <param name="maxValue">The maximum value.</param>
	/// <param name="value">The actual value to represent.</param>
	public static string GetHeatMapColorString(double minValue, double maxValue, double value)
		=> GetHexStringFromColor(GetRgbValues(minValue, maxValue, value));

	#endregion public methods

	#region private methods

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

	private static string GetHexStringFromColor(RgbValues values)
		=> string.Concat("#", values.Red.ToString("X2", CultureInfo), values.Green.ToString("X2", CultureInfo), values.Blue.ToString("X2", CultureInfo));

	private sealed class RgbValues(int red, int green, int blue)
	{
		public int Red { get; } = red;
		public int Green { get; } = green;
		public int Blue { get; } = blue;
	}

	#endregion private methods
}
