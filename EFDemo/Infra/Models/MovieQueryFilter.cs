public record MovieQueryFilter(int? MinViewCount,
                               int? MaxViewCount,
                               DateTime? FromCreatedAt,
                               string Name);

