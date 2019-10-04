using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using BoilerStoreMonolith.Infrastructure.Abstract;
using static System.Web.Security.FormsAuthentication;

namespace BoilerStoreMonolith.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider {
        public bool Authenticate(string username, string password) {
            bool result = FormsAuthentication.Authenticate(username, password);
            if (result) {
                SetAuthCookie(username, false);
            }
            return result;
        }
    }
}