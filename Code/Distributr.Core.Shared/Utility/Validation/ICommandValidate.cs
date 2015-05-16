namespace Distributr.Core.Utility.Validation
{
    public interface ICommandValidate 
    {
        bool CanDeserializeAndValidateCommand<T>(string jsonCommand, out T deserializedObject);
        bool CanDeserializeCommand<T>( string jsonCommand, out T deserializedObject)  ;
        bool IsValidCommand<T>( T command);
    }
}
