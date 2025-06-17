using ClientContactAPI.Classes;

namespace ClientContactAPI.Interfaces
{
    public interface ITemplate
    {
        public Task<bool> CreateTemplateDB();
        public Task<List<Template>> GetTemplates();
        public Task<Template?> GetTemplate(string id);
        public Task<APIResponse> RegisterTemplate(Template template);
        public Task<APIResponse> UpdateTemplate(Template template);
        public Task<APIResponse> DeleteTemplate(string id);

    }
}
