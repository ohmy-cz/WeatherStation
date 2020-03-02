namespace net.jancerveny.weatherstation.Common.Interfaces
{
    public interface IReadOut
    {
        int SourceId { get; set; }
        int? Temperature { get; set; }
        bool Stale { get; set; }
    }
}
