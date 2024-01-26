namespace StaticData
{
    public partial class Facade : CodeSingleton<Facade>
    {
        private Generator.GeneratorDependencyInterface _constant = null;
        private Interfaces _interfaces = new Interfaces();

        public void TryInitGenerator()
        {
            if (_constant == null)
            {
                _constant = _interfaces.Get("GeneratorDependencyInterface") as Generator.GeneratorDependencyInterface;
            }
        }

        public void Log(string log)
        {
            _constant.Log(log);
        }
        public void LogWarning(string log)
        {
            _constant.LogWarning(log);
        }
        public void LogError(string log)
        {
            _constant.LogError(log);
        }
        public void LogDesinerError(string log)
        {
            _constant.LogDesinerError(log);
        }

    }
}