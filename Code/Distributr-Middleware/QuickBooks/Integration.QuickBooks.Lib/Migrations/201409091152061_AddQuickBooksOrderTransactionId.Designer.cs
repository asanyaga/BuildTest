// <auto-generated />
namespace Integration.QuickBooks.Lib.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    public sealed partial class AddQuickBooksOrderTransactionId : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AddQuickBooksOrderTransactionId));
        
        string IMigrationMetadata.Id
        {
            get { return "201409091152061_AddQuickBooksOrderTransactionId"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
