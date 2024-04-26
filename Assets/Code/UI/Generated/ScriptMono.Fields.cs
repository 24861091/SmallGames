
namespace Code.Core.UI.SciptMono
{
    public partial class ScriptMono
    {
        public virtual UnityEngine.UI.Image ImageBackGround { get; set; }

        public virtual void InitializeBindingFields()
        {
            ImageBackGround = GetCompBindingValue<UnityEngine.UI.Image>("ImageBackGround");

        }
    }  
}
