
// Warning: all code of this file are generated automatically, so do not modify it manually ~
// Any questions are welcome, mailto:lixianmin@gmail.com

using System.Collections;

namespace Metadata
{
    public class OuterMetaFactory
    {
        [UnityEngine.Scripting.Preserve]
        private static Hashtable _GetLookupTableByName ()
        {
            return new Hashtable(1)
            {
                { "GameConfig", new MetaCreator(()=> new GameConfig()) },
            };
        }
    }
}
