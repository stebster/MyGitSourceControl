namespace Stebster.Core.Repository.Interface
{
    using System.Collections.Generic;

    public interface ISqlRepository
    {
        string ServerName { get; }

        string DatabaseName { get; }

        void ExecuteProcedure(string procedureName, IDictionary<string, object> inputParameters = null, IDictionary<string, object> outputParameters = null);

        IEnumerable<IDictionary<string, object>> ExecuteProcedureGet(string procedureName, IDictionary<string, object> inputParameters = null, IDictionary<string, object> outputParameters = null);
    }
}
