using ClientContactAPI.Classes;
using ClientContactAPI.ViewModels;

namespace ClientContactAPI.Interfaces
{
    public interface IAccess
    {
        public APIResponse Login(LoginViewModel model);
    }
}
