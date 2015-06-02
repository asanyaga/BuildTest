using SQLite.Net.Attributes;

namespace Distributr.Mobile.Core.Data.Sequences
{
    public enum SequenceName { DocumentReference }

    public class DatabaseSequence
    {
        [PrimaryKey]
        public SequenceName SequenceName { get; set; }
        
        public int NextValue { get; set; }
    }
}
