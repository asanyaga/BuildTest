using System;

namespace Distributr.WPF.Lib.Services.Payment
{
    public class ClientRequestResponseBase
    {
        public Guid Id { get; set; }
        public Guid DistributorCostCenterId { get; set; }
        public string TransactionRefId { get; set; }//maps to ExternalTransactionId / TransactionRefId / 
        public ClientRequestResponseType ClientRequestResponseType { get; set; }
        public DateTime DateCreated { get; set; }

        

        //protected override object GetValue(FieldInfo field)
        //{
        //    if (field.DeclaringType == typeof(ClientRequestResponseBase))
        //        return field.GetValue(this);
        //    else
        //        return base.GetValue(field);
        //}
        ////this method is required because Silverlight cannot access private members by reflection from another class,
        ////only by call from an inside method
        //protected override void SetValue(FieldInfo field, object value)
        //{
        //    if (field.DeclaringType == typeof(ClientRequestResponseBase))
        //        field.SetValue(this, value);
        //    else
        //        base.SetValue(field, value);

        //}
    }
}
