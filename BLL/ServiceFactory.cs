using DAL.DataProvider;
using DAL.Interfaces;

namespace BLL.Services
{
    public static class ServiceFactory
    {
        public static UserService CreateUserService()
        {
            IDataProvider<DAL.Entities.User> provider = new JsonUserDataProvider();
            return new UserService(provider);
        }
        public static DocumentService CreateDocumentService()
        {
            IDataProvider<DAL.Entities.Document> provider = new JsonDocumentDataProvider();
            return new DocumentService(provider);
        }
    }
}