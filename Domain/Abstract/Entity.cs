using System;

namespace Domain.Abstract
{
    public abstract class Entity
    {
        public long Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
    }
}
