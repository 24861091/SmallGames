using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StaticData
{
    public partial struct InternationalText
    {
        public override string ToString()
        {
            //I18nTemplate template = TemplateManager.Instance.GetTemplate(typeof(I18nTemplate).Name, ID) as I18nTemplate;
            //if (template == null)
            //{
            //    StaticDataUtility.LogDesinerError("i18n 的配置文件没有找到(I18nTemplate). id = " + ID.ToString());
            //    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            //    {
            //        return string.Empty;
            //    }
            //    return ID.ToString();
            //}
            //return template.Text;
            return "";
        }

    }
}