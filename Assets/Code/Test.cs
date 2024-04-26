using Code.Core.GameConf;
using Code.Core.Utils;
using StaticData;
using UnityEngine;

public class Test : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        // var config = StaticData.TemplateManager.Instance.GetTemplate(typeof(TemplateTest), 1) as TemplateTest;
        // CoreLog.Log($"mingcai config.ID = {config.ID}  config.id = {config.platform3}");

        var config = GameConf.Get<TemplateTest>("ming") as TemplateTest;
        CoreLog.Log($"mingcai config.ID = {config.ID}  config.id = {config.platform3}");
    }
}
