using EFDemo.Infra.Entities.Base;

namespace EFDemo.Infra.Entities.Discriminator;

public class VideoTypeEntity : BaseEntity
{
    public string Discriminator { get; set; }
    public Guid ReferenceVideoId { get; set; }
}

public class Documentary : VideoTypeEntity
{

}

public class TvShow : VideoTypeEntity
{
    public int SeasonCount { get; set; }
    public int EpisodeCountPerSeason { get; set; }
}
