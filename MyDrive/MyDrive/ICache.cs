using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainService
{
    public interface ICache
    {
        User GetUser(string userId);
        File GetFile(string fileId);
        void UpdateUser(User user);
        void UpdateFile(File file);
    }
}
