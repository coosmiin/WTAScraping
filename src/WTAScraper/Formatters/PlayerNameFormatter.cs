﻿using System;
using System.Linq;

namespace WTAScraper.Formatters
{
	public class PlayerNameFormatter : IPlayerNameFormatter
	{
		/// <summary>
		/// Changes name format from "LN, FN" to "FN LN"
		/// </summary>
		public string GetPlayerName(string name)
		{
			return string.Join(" ", name.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Reverse().Select(n => n.Trim()));
		}
	}
}
