namespace EventService.Core.Dtos
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid UserId { get; set; }

        public Guid CategoryId { get; set; } 
    }
}
