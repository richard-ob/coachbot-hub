namespace CoachBot.Model
{
    public class Server
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public int RegionId { get; set; }

        public Region Region { get; set; }

    }
}
