using TSEnvironmentLib;

namespace WinPeOperator
{
    internal class tsManager
    {
        private TSEnvClass tsEnvirontment = new TSEnvClass();

        public string getTSVariableData(string variableName)
        {
            return tsEnvirontment[variableName].ToString();
        }

        public void setTSVariableData(string variableName, string variableData)
        {
            tsEnvirontment[variableName] = variableData;
        }

    }
}
