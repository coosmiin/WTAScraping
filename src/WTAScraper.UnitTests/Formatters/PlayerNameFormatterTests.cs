﻿using WTAScraper.Formatters;
using Xunit;

namespace WTAScraper.UnitTests.Formatters
{
	public class PlayerNameFormatterTests
	{
		[Fact]
		public void GetPlayerName_NameIsFormattedCorrectly()
		{
			var formatter = new PlayerNameFormatter();

			string name = formatter.GetPlayerName("LN, FN");

			Assert.Equal("FN LN", name);
		}
	}
}