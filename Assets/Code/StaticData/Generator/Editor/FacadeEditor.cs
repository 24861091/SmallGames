namespace StaticDataEditor
{
    public partial class FacadeEditor : CodeSingleton<FacadeEditor>
    {
        //private ConcretDependency _constant = new ConcretDependency();
        private GeneratorDependencyInterface generator = null;

        #region interface 
        public void TryInitGenerator()
        {
            if (generator == null)
            {
                generator = _interfaces.Get("GeneratorDependencyInterfaceEditor") as GeneratorDependencyInterface;
            }

        }
        public string GenerateCSPath
        {
            get
            {
                return generator.GetGenerateCSPath();
            }
        }

        public string GenerateCSSummaryPath
        {
            get
            {
                return generator.GetGenerateCSSummaryPath();
            }
        }

        public string GenerateCSDefinationPath
        {
            get
            {
                return generator.GetGenerateCSDefinationPath();
            }
        }

        public string GenerateCSEditorPath
        {
            get
            {
                return generator.GetGenerateCSEditorPath();
            }
        }

        public string GenerateCSEditorSummaryPath
        {
            get
            {
                return generator.GetGenerateCSEditorSummaryPath();
            }
        }

        public string GenerateLuaPath
        {
            get
            {
                return generator.GetGenerateLuaPath();
            }
        }
        #endregion
    }
}