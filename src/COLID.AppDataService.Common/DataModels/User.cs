using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace COLID.AppDataService.Common.DataModel
{
    public class User : Entity<Guid>
    {
        [Key]
        public override Guid Id { get; set; }

        public string EmailAddress { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastLoginDataMarketplace { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastLoginEditor { get; set; }

        // Date of the manual checking for notification
        [DataType(DataType.Date)]
        public DateTime? LastTimeChecked { get; set; }

        public virtual ConsumerGroup DefaultConsumerGroup { get; set; }

        public virtual SearchFilterEditor SearchFilterEditor { get; set; }

        public virtual int? DefaultSearchFilterDataMarketplace { get; set; }

        public virtual ICollection<SearchFilterDataMarketplace> SearchFiltersDataMarketplace { get; set; }

        public virtual ICollection<StoredQuery> StoredQueries { get; set; }

        public virtual ICollection<ColidEntrySubscription> ColidEntrySubscriptions { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual MessageConfig MessageConfig { get; set; }
    }
}
