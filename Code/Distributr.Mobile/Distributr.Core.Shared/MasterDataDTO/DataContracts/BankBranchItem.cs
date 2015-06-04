namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class BankBranchItem : MasterBaseItem
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public BankItem Bank { get; set; }
       public string Description { get; set; }
    }
}
