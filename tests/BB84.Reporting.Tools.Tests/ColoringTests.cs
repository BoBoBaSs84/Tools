using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BB84.Reporting.Tools.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class ColoringTests
{
	[TestMethod]
	public void GetHeatMapColorStringRedTest()
	{
		string expected = "#FF0000";
		double minValue = 0, maxValue = 100, value = 100;

		string colorString = Coloring.GetHeatMapColorString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}

	[TestMethod]
	public void GetHeatMapColorStringGreenTest()
	{
		string expected = "#00FF00";
		int minValue = 0, maxValue = 100, value = 50;

		string colorString = Coloring.GetHeatMapColorString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}

	[TestMethod]
	public void GetHeatMapColorStringBlueTest()
	{
		string expected = "#0000FF";
		double minValue = 0, maxValue = 100, value = 0;

		string colorString = Coloring.GetHeatMapColorString(minValue, maxValue, value);

		Assert.AreEqual(expected, colorString);
	}
}
