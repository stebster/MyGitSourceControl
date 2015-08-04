namespace Stebster.Core.Repository.Interface
{
    using System.Collections.Generic;

    public interface ISqlRepository
    {
        void ExecuteProcedure(string procedureName, Dictionary<string, object> inputParameters = null, Dictionary<string, object> outputParameters = null);

        Dictionary<string, object>[] ExecuteProcedureGet(string procedureName, Dictionary<string, object> inputParameters = null, Dictionary<string, object> outputParameters = null);
    }
}
