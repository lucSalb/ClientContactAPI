using ClientContactAPI.Classes;

namespace ClientContactAPI.Interfaces
{
    public interface ITemplate
    {
        public List<Template> GetTemplates();
        public Template GetTemplate(string id);
        public OperationResult RegisterTemplate(Template template);
        public OperationResult UpdateTemplate(Template template);
        public OperationResult DeleteTemplate(string id);

    }
}
