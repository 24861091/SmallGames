/************************************************
 created : 2018.8
 author : caiming
************************************************/
#pragma warning disable 0219
using System;


namespace StaticData
{
    public class TemplateManager : CodeSingleton<TemplateManager>
    {
        public TemplateManager()
        {
        }
        private string _Key(Type type)
        {
            return type.Name;
        }
        public bool Remove(Template template)
        {
            if(template == null)
            {
                return false;
            }
            return Remove(template.GetType(), template.ID);
        }
        public bool Remove(Type type, int id)
        {
            return _Remove(_Key(type), id);
        }
        private bool _Remove(string key, int id)
        {
            return templates.Remove(key, id);
        }


        public bool Add(Template value)
        {
            if (value != null)
            {
                return _Add(_Key(value.GetType()), value);
            }
            return false;
        }
        private bool _Add(string key, Template value)
        {
            return templates.Add(key, value.ID, value, false);
        }
        public bool Remove(Type type)
        {
            return templates.Remove(_Key(type));
        }

        public void Clear()
        {
            templates.Clear();
            _templates = null;
        }
        private Template[] _LoadTemplates(Type type, bool checkError = true)
        {
            return _templateLoader.LoadTemplates(_Key(type), Add, checkError);
        }


        public Template[] GetTemplates(Type type, bool checkError = true)
        {
            string key = _Key(type);
            Template[] temp = null;
            _LoadTemplates(type, checkError);

            return templates.GetTemplates(key);
        }





        private Template _LoadTemplate(Type type, int id , bool checkError = true)
        {
            return _templateLoader.LoadTemplate(_Key(type), id, Add, checkError);
        }

        public Template GetTemplate(string typeString, int id, bool checkError = true)
        {
            return GetTemplate(Facade.Instance.GetType(typeString), id, checkError);
        }
        public Template GetTemplate(Type type, int id, bool checkError = true)
        {
            if (type == null)
            {
                StaticDataUtility.LogError(string.Format("Template 类型是不存在的."));
            }
            
            string key = _Key(type);
            Template temp = null;
            if (!templates.Contains(key, id))
            {
                temp = _LoadTemplate(type, id, checkError);
            }
            if (!templates.Contains(key, id))
            {
                return null;
            }
            return templates.GetTemplate(key, id);
        }

        public T GetTemplate<T>(int id, bool checkError = true) where T : Template
        {
            var type = typeof(T);
            var key = type.Name;
            T temp = null;
            if (!templates.Contains(key, id))
            {
                temp = _LoadTemplate(type, id, checkError) as T;
            }
            if (!templates.Contains(key, id))
            {
                return null;
            }
            return templates.GetTemplate(key, id) as T;
        }

        private IReferenceTemplateDatas templates
        {
            get
            {
                if(_templates == null)
                {
                    if (Constant.IsCSharpWeakReference)
                    {
                        _templates = new WeakReferenceTemplateDatas();
                    }
                    else
                    {
                        _templates = new ReferenceTemplateDatas();
                    }
                }
                return _templates;
            }
        }
        private ITemplateLoader _templateLoader = new BinaryTemplateLoader();
        private IReferenceTemplateDatas _templates = null;
    }
}