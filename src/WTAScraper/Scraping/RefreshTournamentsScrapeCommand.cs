﻿using System;
using System.Collections.Generic;
using System.Linq;
using WTAScraper.Data;
using Logging;
using WTAScraper.Tournaments;
using WTAScraper.Website;

namespace WTAScraper.Scraping
{
	public class RefreshTournamentsScrapeCommand : IScrapeCommand
	{
		public const string REFRESH_TOURNAMENTS_COMMAND = "refresh-tournaments";

		private readonly IWtaWebsite _wtaWebsite;
		private readonly ITournamentRepository _tournamentRepository;
		private readonly ILogger _logger;
		private readonly DateTime _currentDate;

		public RefreshTournamentsScrapeCommand(
			IWtaWebsite wtaWebsite, ITournamentRepository tournamentRepository, ILogger logger, DateTime currentDate)
		{
			_wtaWebsite = wtaWebsite;
			_tournamentRepository = tournamentRepository;
			_logger = logger;
			_currentDate = currentDate;
		}

		public void RefreshData()
		{
			try
			{
				IEnumerable<Tournament> newTournaments = _wtaWebsite.GetCurrentAndUpcomingTournaments();

				_tournamentRepository.AddTournaments(newTournaments.AsTournamentDetails());

				IEnumerable<TournamentDetails> tournamentsDetails =
					_tournamentRepository
						.GetTournaments(
							t => (t.Status == TournamentStatus.Current || t.Status == TournamentStatus.Upcomming)
								&& (t.SeededPlayerNames == null || !t.SeededPlayerNames.Any()));

				tournamentsDetails = _wtaWebsite.RefreshSeededPlayers(tournamentsDetails);

				_tournamentRepository.UpdateTournaments(tournamentsDetails);

				_logger.Log(REFRESH_TOURNAMENTS_COMMAND, BuildLogMessage());
			}
			catch (Exception ex)
			{
				_logger.Log(REFRESH_TOURNAMENTS_COMMAND, $"Error - {ex.Message}");
			}
		}

		private string BuildLogMessage()
		{
			IEnumerable<TournamentDetails> tournamentsDetails = 
			_tournamentRepository
				.GetTournaments(
					t => t.Status == TournamentStatus.Current 
						|| ((t.Date < _currentDate.AddDays(2) && (t.Date > _currentDate.AddDays(-2)))
							&& (t.Status == TournamentStatus.Upcomming || t.Status == TournamentStatus.Invalid)));

			return
				string.Join(", ", 
					tournamentsDetails
						.OrderBy(t => t.Status)
						.Select(t => $"{t.Name} [{t.Status.ToString().Substring(0, 1)}{GetPlayerStatusAcronym(t.SeededPlayerNames)}P]"));
		}

		private string GetPlayerStatusAcronym(IEnumerable<string> players)
		{
			return players == null || !players.Any() ? "no" : "w";
		}
	}
}
