namespace EventService.Core.DbModels
{
    // Main Event Table
    public class EventItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid UserId { get; set; }

        // Foreign Key → Category Table
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}