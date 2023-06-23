using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace footballKursova
{
    public partial class Form1 : Form
    {
        private List<Tournament> tournaments;
        public Form1()
        {
            InitializeComponent();
            tournaments = new List<Tournament>();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadTournaments();
        }

        private void LoadTournaments()
        {
            tournamentsListBox.Items.Clear();
            foreach (Tournament tournament in tournaments)
            {
                tournamentsListBox.Items.Add(tournament.Name);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            string name = nameTextBox.Text;
            TournamentType type = (TournamentType)typeComboBox.SelectedIndex;
            bool hasMatches = matchesCheckBox.Checked;

            Tournament newTournament = new Tournament(name, type, hasMatches);
            tournaments.Add(newTournament);

            nameTextBox.Text = string.Empty;
            typeComboBox.SelectedIndex = -1;
            matchesCheckBox.Checked = false;

            LoadTournaments();
            typeComboBox.Items.Add(TournamentType.Championship);
            typeComboBox.Items.Add(TournamentType.Cup);
            typeComboBox.SelectedIndex = -1;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = tournamentsListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < tournaments.Count)
            {
                string newName = nameTextBox.Text;
                TournamentType newType = (TournamentType)typeComboBox.SelectedIndex;
                bool newHasMatches = matchesCheckBox.Checked;

                tournaments[selectedIndex].Name = newName;
                tournaments[selectedIndex].Type = newType;
                tournaments[selectedIndex].HasMatches = newHasMatches;

                nameTextBox.Text = string.Empty;
                typeComboBox.SelectedIndex = -1;
                matchesCheckBox.Checked = false;

                LoadTournaments();
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = tournamentsListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < tournaments.Count)
            {
                tournaments.RemoveAt(selectedIndex);

                LoadTournaments();
            }
        }
        private void tournamentsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = tournamentsListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < tournaments.Count)
            {
                Tournament selectedTournament = tournaments[selectedIndex];

                // Відображення календаря турніру на DataGridView
                DisplayTournamentCalendar(selectedTournament);

                // Відображення загальної схеми турніру
                DisplayTournamentScheme(selectedTournament);
            }
        }

        private void DisplayTournamentCalendar(Tournament tournament)
        {
            tournamentCalendarGridView.Rows.Clear();

            if (tournament.Type == TournamentType.Championship)
            {
                // Додавання матчів для чемпіонату
                for (int i = 0; i < tournament.Teams.Count; i++)
                {
                    for (int j = i + 1; j < tournament.Teams.Count; j++)
                    {
                        tournamentCalendarGridView.Rows.Add(tournament.Teams[i], tournament.Teams[j]);
                    }
                }
            }
            else if (tournament.Type == TournamentType.Cup && tournament.HasMatches)
            {
                // Додавання матчів першого туру для кубка
                for (int i = 0; i < tournament.Teams.Count; i += 2)
                {
                    tournamentCalendarGridView.Rows.Add(tournament.Teams[i], tournament.Teams[i + 1]);
                }
            }
        }

        private void DisplayTournamentScheme(Tournament tournament)
        {
            schemeLabel.Text = tournament.Scheme;
        }

        private void addResultButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = tournamentsListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < tournaments.Count)
            {
                int selectedRowIndex = tournamentCalendarGridView.SelectedCells[0].RowIndex;
                string homeTeam = tournamentCalendarGridView.Rows[selectedRowIndex].Cells[0].Value.ToString();
                string awayTeam = tournamentCalendarGridView.Rows[selectedRowIndex].Cells[1].Value.ToString();

                // Відкрити форму для оновлення результату матчу
                UpdateResultForm updateResultForm = new UpdateResultForm(homeTeam, awayTeam);
                updateResultForm.ShowDialog();

                if (updateResultForm.ResultUpdated)
                {
                    string result = updateResultForm.Result;
                    // Занесення результату у відповідну клітинку DataGridView
                    tournamentCalendarGridView.Rows[selectedRowIndex].Cells[2].Value = result;
                }
            }
        }
        private void UpdateTournamentTable(Tournament tournament)
        {
            tournamentTableGridView.Rows.Clear();

            if (tournament.Type == TournamentType.Championship)
            {
                // Оновлення турнірної таблиці для чемпіонату
                foreach (string team in tournament.Teams)
                {
                    int matchesPlayed = GetMatchesPlayed(tournament, team);
                    int wins = GetWins(tournament, team);
                    int draws = GetDraws(tournament, team);
                    int losses = GetLosses(tournament, team);
                    int goalsFor = GetGoalsFor(tournament, team);
                    int goalsAgainst = GetGoalsAgainst(tournament, team);
                    int goalDifference = goalsFor - goalsAgainst;
                    int points = (wins * 3) + draws;

                    tournamentTableGridView.Rows.Add(team, matchesPlayed, wins, draws, losses, goalsFor, goalsAgainst, goalDifference, points);
                }
            }
            else if (tournament.Type == TournamentType.Cup && tournament.HasMatches)
            {
                // Оновлення турнірної таблиці для кубка
                // Відображення результатів зіграних матчів
                foreach (MatchResult result in tournament.MatchResults)
                {
                    tournamentTableGridView.Rows.Add(result.HomeTeam, result.AwayTeam, result.Result);
                }
            }
        }

        private int GetMatchesPlayed(Tournament tournament, string team)
        {
            int matchesPlayed = 0;


            if (tournament.Type == TournamentType.Championship)
            {
                matchesPlayed = tournamentCalendarGridView.Rows.Count;
            }
            else if (tournament.Type == TournamentType.Cup && tournament.HasMatches)
            {
                matchesPlayed = (int)tournament.MatchResults.LongCount();
            }

            return matchesPlayed;
        }

        private int GetWins(Tournament tournament, string team)
        {
            int wins = 0;

            if (tournament.Type == TournamentType.Championship)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if ((result.HomeTeam == team && result.Result.StartsWith("W")) ||
                        (result.AwayTeam == team && result.Result.EndsWith("W")))
                    {
                        wins++;
                    }
                }
            }
            else if (tournament.Type == TournamentType.Cup && tournament.HasMatches)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if ((result.HomeTeam == team && result.Result.StartsWith("W")) ||
                        (result.AwayTeam == team && result.Result.EndsWith("W")))
                    {
                        wins++;
                    }
                }
            }

            return wins;
        }

        private int GetDraws(Tournament tournament, string team)
        {
            int draws = 0;

            if (tournament.Type == TournamentType.Championship)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if (result.Result == "D")
                    {
                        draws++;
                    }
                }
            }
            else if (tournament.Type == TournamentType.Cup && tournament.HasMatches)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if (result.Result == "D")
                    {
                        draws++;
                    }
                }
            }

            return draws;
        }

        private int GetLosses(Tournament tournament, string team)
        {
            int losses = 0;

            if (tournament.Type == TournamentType.Championship)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if ((result.HomeTeam == team && result.Result.EndsWith("W")) ||
                        (result.AwayTeam == team && result.Result.StartsWith("W")))
                    {
                        losses++;
                    }
                }
            }
            else if (tournament.Type == TournamentType.Cup && tournament.HasMatches)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if ((result.HomeTeam == team && result.Result.EndsWith("W")) ||
                        (result.AwayTeam == team && result.Result.StartsWith("W")))
                    {
                        losses++;
                    }
                }
            }

            return losses;
        }

        private int GetGoalsFor(Tournament tournament, string team)
        {
            int goalsFor = 0;

            if (tournament.Type == TournamentType.Championship)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if (result.HomeTeam == team)
                    {
                        goalsFor += GetHomeTeamGoals(result.Result);
                    }
                    else if (result.AwayTeam == team)
                    {
                        goalsFor += GetAwayTeamGoals(result.Result);
                    }
                }
            }
            else if (tournament.Type == TournamentType.Cup && tournament.HasMatches)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if (result.HomeTeam == team)
                    {
                        goalsFor += GetHomeTeamGoals(result.Result);
                    }
                    else if (result.AwayTeam == team)
                    {
                        goalsFor += GetAwayTeamGoals(result.Result);
                    }
                }
            }

            return goalsFor;
        }

        private int GetGoalsAgainst(Tournament tournament, string team)
        {
            int goalsAgainst = 0;

            if (tournament.Type == TournamentType.Championship)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if (result.HomeTeam == team)
                    {
                        goalsAgainst += GetAwayTeamGoals(result.Result);
                    }
                    else if (result.AwayTeam == team)
                    {
                        goalsAgainst += GetHomeTeamGoals(result.Result);
                    }
                }
            }
            else if (tournament.Type == TournamentType.Cup && tournament.HasMatches)
            {
                foreach (MatchResult result in tournament.MatchResults)
                {
                    if (result.HomeTeam == team)
                    {
                        goalsAgainst += GetAwayTeamGoals(result.Result);
                    }
                    else if (result.AwayTeam == team)
                    {
                        goalsAgainst += GetHomeTeamGoals(result.Result);
                    }
                }
            }

            return goalsAgainst;
        }

        private int GetHomeTeamGoals(string result)
        {
            string[] parts = result.Split(':');
            return int.Parse(parts[0]);
        }

        private int GetAwayTeamGoals(string result)
        {
            string[] parts = result.Split(':');
            return int.Parse(parts[1]);
        }

        private void UpdateTeamStatistics(Tournament tournament)
        {
            teamStatisticsGridView.Rows.Clear();

            if (tournament.Type == TournamentType.Championship)
            {
                foreach (string team in tournament.Teams)
                {
                    int maxUnbeatenStreak = GetMaxUnbeatenStreak(tournament, team);
                    int maxWinningStreak = GetMaxWinningStreak(tournament, team);
                    int maxLosingStreak = GetMaxLosingStreak(tournament, team);
                    int bigWins = GetBigWins(tournament, team);

                    teamStatisticsGridView.Rows.Add(team, maxUnbeatenStreak, maxWinningStreak, maxLosingStreak, bigWins);
                }
            }
        }

        private int GetMaxUnbeatenStreak(Tournament tournament, string team)
        {
            List<bool> unbeatenStreaks = new List<bool>();
            int maxUnbeatenStreak = 0;
            int currentUnbeatenStreak = 0;

            foreach (MatchResult result in tournament.MatchResults)
            {
                if (result.HomeTeam == team && result.Result.EndsWith("W") || result.AwayTeam == team && result.Result.StartsWith("W") || result.Result == "D")
                {
                    unbeatenStreaks.Add(true);
                    currentUnbeatenStreak++;

                    if (currentUnbeatenStreak > maxUnbeatenStreak)
                    {
                        maxUnbeatenStreak = currentUnbeatenStreak;
                    }
                }
                else
                {
                    unbeatenStreaks.Add(false);
                    currentUnbeatenStreak = 0;
                }
            }

            return maxUnbeatenStreak;
        }

        private int GetMaxWinningStreak(Tournament tournament, string team)
        {
            List<bool> winningStreaks = new List<bool>();
            int maxWinningStreak = 0;
            int currentWinningStreak = 0;

            foreach (MatchResult result in tournament.MatchResults)
            {
                if (result.HomeTeam == team && result.Result.EndsWith("W") || result.AwayTeam == team && result.Result.StartsWith("W"))
                {
                    winningStreaks.Add(true);
                    currentWinningStreak++;

                    if (currentWinningStreak > maxWinningStreak)
                    {
                        maxWinningStreak = currentWinningStreak;
                    }
                }
                else
                {
                    winningStreaks.Add(false);
                    currentWinningStreak = 0;
                }
            }

            return maxWinningStreak;
        }

        private int GetMaxLosingStreak(Tournament tournament, string team)
        {
            List<bool> losingStreaks = new List<bool>();
            int maxLosingStreak = 0;
            int currentLosingStreak = 0;

            foreach (MatchResult result in tournament.MatchResults)
            {
                if ((result.HomeTeam == team && result.Result.EndsWith("W")) || (result.AwayTeam == team && result.Result.StartsWith("W")))
                {
                    losingStreaks.Add(false);
                    currentLosingStreak = 0;
                }
                else
                {
                    losingStreaks.Add(true);
                    currentLosingStreak++;

                    if (currentLosingStreak > maxLosingStreak)
                    {
                        maxLosingStreak = currentLosingStreak;
                    }
                }
            }

            return maxLosingStreak;
        }

        private int GetBigWins(Tournament tournament, string team)
        {
            int bigWins = 0;

            foreach (MatchResult result in tournament.MatchResults)
            {
                if (result.HomeTeam == team && GetHomeTeamGoals(result.Result) >= 3)
                {
                    bigWins++;
                }
                else if (result.AwayTeam == team && GetAwayTeamGoals(result.Result) >= 3)
                {
                    bigWins++;
                }
            }

            return bigWins;
        }

        private void printTableButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = tournamentsListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < tournaments.Count)
            {
                Tournament selectedTournament = tournaments[selectedIndex];

                DataTable table = new DataTable();

                // Додавання стовпців до таблиці
                table.Columns.Add("Команда", typeof(string));
                table.Columns.Add("Матчі", typeof(int));
                table.Columns.Add("Перемоги", typeof(int));
                table.Columns.Add("Нічиї", typeof(int));
                table.Columns.Add("Поразки", typeof(int));
                table.Columns.Add("Забито", typeof(int));
                table.Columns.Add("Пропущено", typeof(int));
                table.Columns.Add("Різниця", typeof(int));
                table.Columns.Add("Очки", typeof(int));

                if (selectedTournament.Type == TournamentType.Championship)
                {
                    foreach (string team in selectedTournament.Teams)
                    {
                        int matchesPlayed = GetMatchesPlayed(selectedTournament, team);
                        int wins = GetWins(selectedTournament, team);
                        int draws = GetDraws(selectedTournament, team);
                        int losses = GetLosses(selectedTournament, team);
                        int goalsFor = GetGoalsFor(selectedTournament, team);
                        int goalsAgainst = GetGoalsAgainst(selectedTournament, team);
                        int goalDifference = goalsFor - goalsAgainst;
                        int points = (wins * 3) + draws;

                        table.Rows.Add(team, matchesPlayed, wins, draws, losses, goalsFor, goalsAgainst, goalDifference, points);
                    }
                }

                dataGridView.DataSource = table;
            }
        }

        private void displayStateButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = tournamentsListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < tournaments.Count)
            {
                Tournament selectedTournament = tournaments[selectedIndex];

                DataTable table = new DataTable();

                // Додавання стовпців до таблиці
                table.Columns.Add("Команда", typeof(string));
                table.Columns.Add("Макс. серія безпрограшних", typeof(int));
                table.Columns.Add("Макс. серія перемог", typeof(int));
                table.Columns.Add("Макс. серія поразок", typeof(int));
                table.Columns.Add("Великі перемоги", typeof(int));

                if (selectedTournament.Type == TournamentType.Championship)
                {
                    foreach (string team in selectedTournament.Teams)
                    {
                        int maxUnbeatenStreak = GetMaxUnbeatenStreak(selectedTournament, team);
                        int maxWinningStreak = GetMaxWinningStreak(selectedTournament, team);
                        int maxLosingStreak = GetMaxLosingStreak(selectedTournament, team);
                        int bigWins = GetBigWins(selectedTournament, team);

                        table.Rows.Add(team, maxUnbeatenStreak, maxWinningStreak, maxLosingStreak, bigWins);
                    }
                }

                dataGridView.DataSource = table;
            }
        }
    }

    internal class MatchResult
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Result { get; set; }
    }

    public class Tournament
    {
        internal IEnumerable<MatchResult> MatchResults;

        public string Name { get; set; }
        public TournamentType Type { get; set; }
        public bool HasMatches { get; set; }
        public List<string> Teams { get; set; }
        public string Scheme { get; set; }

        public Tournament(string name, TournamentType type, bool hasMatches)
        {
            Name = name;
            Type = type;
            HasMatches = hasMatches;
            Teams = new List<string>();
            Scheme = "";
        }
    }

    public enum TournamentType
    {
        Championship,
        Cup
    }
}
