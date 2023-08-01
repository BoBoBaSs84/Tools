using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BB84.Reporting.Tools.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class ColorTests
{
	[TestMethod]
	public void GetRgbHexStringMiddleTest()
	{
		string expected = "#00FF00";
		double minValue = 0, maxValue = 100, value = 50;

		var colorString = Color.GetRgbHexString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}

	[TestMethod]
	public void GetRgbHexStringMinimalTest()
	{
		string expected = "#0000FF";
		double minValue = 0, maxValue = 100, value = 0;

		var colorString = Color.GetRgbHexString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}

	[TestMethod]
	public void GetRgbHexStringMaximalTest()
	{
		string expected = "#FF0000";
		double minValue = 0, maxValue = 100, value = 100;

		var colorString = Color.GetRgbHexString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}
}