namespace ModelsData
{
    public class ServiceModel
    {
        public string Name { get; set; }
        public string Info { get; set; }
        public int Id { get; set; }
        public int SubdivisionServiceId { get; set; }
        public string DocumentLink { get; set; }
        public string FormLink { get; set; }
        public string Status { get; set; }
        public bool Availability { get; set; }
    }
}