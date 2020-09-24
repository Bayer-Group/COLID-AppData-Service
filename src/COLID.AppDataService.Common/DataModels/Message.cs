using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COLID.AppDataService.Common.DataModel
{
    public class Message : Entity<int>
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        // When the message has been read
        [DataType(DataType.Date)]
        public DateTime? ReadOn { get; set; }

        // When should the message be send to the user
        [DataType(DataType.Date)]
        public DateTime? SendOn { get; set; }

        // When the message should be deleted
        [DataType(DataType.Date)]
        public DateTime? DeleteOn { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
