using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BB84.Reporting.Tools.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class ColoringTests
{
	[TestMethod]
	public void GetRgbHexStringGreenTest()
	{
		string expected = "#00FF00";
		int minValue = 0, maxValue = 100, value = 50;

		string colorString = Coloring.GetRgbHexString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}

	[TestMethod]
	public void GetRgbHexStringBlueTest()
	{
		string expected = "#0000FF";
		double minValue = 0, maxValue = 100, value = 0;

		string colorString = Coloring.GetRgbHexString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}

	[TestMethod]
	public void GetRgbHexStringRedTest()
	{
		string expected = "#FF0000";
		double minValue = 0, maxValue = 100, value = 100;

		string colorString = Coloring.GetRgbHexString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}

	[TestMethod]
	public void GetRgbGreenTest()
	{
		Color expected = Color.Lime;
		int minValue = 0, maxValue = 100, value = 50;

		Color color = Coloring.GetRgbColor(minValue, maxValue, value);

		Assert.AreEqual(expected.ToArgb(), color.ToArgb());
	}

	[TestMethod]
	public void GetRgbBlueTest()
	{
		Color expected = Color.Blue;
		double minValue = 0, maxValue = 100, value = 0;

		Color color = Coloring.GetRgbColor(minValue, maxValue, value);

		Assert.AreEqual(expected.ToArgb(), color.ToArgb());
	}

	[TestMethod]
	public void GetRgbRedTest()
	{
		Color expected = Color.Red;
		double minValue = 0, maxValue = 100, value = 100;

		Color color = Coloring.GetRgbColor(minValue, maxValue, value);

		Assert.AreEqual(expected.ToArgb(), color.ToArgb());
	}
}