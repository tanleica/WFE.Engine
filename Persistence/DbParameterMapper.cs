
using Dapper;

namespace WFE.Engine.Persistence
{
    public static class DbParameterMapper
    {
        public static DynamicParameters FromVariables(IDictionary<string, object> variables)
        {
            var parameters = new DynamicParameters();

            foreach (var pair in variables)
            {
                parameters.Add(pair.Key, pair.Value);
            }

            return parameters;
        }
    }
}