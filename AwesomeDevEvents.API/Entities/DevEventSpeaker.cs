namespace AwesomeDevEvents.API.Entities
{
    public class DevEventSpeaker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TalkTitle { get; set; }
        public string TalkDescription { get; set; }
        public string LinkedInProfile { get; set; }
        public int DevEventId { get; set; }
    }
}