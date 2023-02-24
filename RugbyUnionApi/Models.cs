namespace RugbyUnionApi
{
    public class Player
    {
        public int Id { get; set; } // Unique identifier for player
        public string Name { get; set; } // Player's name
        public DateTime BirthDate { get; set; } // Player's birth date
        public int Height { get; set; } // Player's height in cm
        public int Weight { get; set; } // Player's weight in kg
        public string? PlaceOfBirth { get; set; } // Player's place of birth
        public int? TeamId { get; set; } // ID of the team the player is signed with (null if not signed)
    }

    public class Team
    {
        public int Id { get; set; } // Unique identifier for team
        public string Name { get; set; } // Team name (unique)
        public string Ground { get; set; } // Team's home ground
        public string Coach { get; set; } // Team's coach
        public int FoundedYear { get; set; } // Year the team was founded
        public string Region? { get; set; } // Region the team is based in
        public ICollection<Player>? Players { get; set; } // List of players signed with the team
    }
}
