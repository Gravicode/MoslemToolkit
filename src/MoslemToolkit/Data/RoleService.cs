using MoslemToolkit.Models;

namespace MoslemToolkit.Data
{
    //public enum Roles { User,Pengurus, KU, PJKBM }
    public class RoleInfo
    {
        static UserProfileService svc;
        public static bool HasRole(string Username, Roles role)
        {
            if (svc == null) svc = new UserProfileService();
            var selUser = svc.GetUserByEmail(Username);
            return selUser.Role == role;
        }
        public static bool HasRole(string Username, params Roles[] roles)
        {
            if (svc == null) svc = new UserProfileService();
            var selUser = svc.GetUserByEmail(Username);

            var contain = roles.Contains(selUser.Role);// == role;

            return contain;

        }
    }
    public class RoleService
    {
        public RoleService(UserProfileService svc)
        {
            this.svc = svc;

        }
        UserProfileService svc;
        public bool HasRole(string Username, Roles role)
        {
            var selUser = svc.GetUserByEmail(Username);
            return selUser.Role == role;
        }
        
        public Roles GetRole(string Username)
        {
            var selUser = svc.GetUserByEmail(Username);
            return selUser.Role;
        }

        public bool HasRole(string Username, params Roles[] roles)
        {
            var selUser = svc.GetUserByEmail(Username);

            var contain = roles.Contains(selUser.Role);// == role;

            return contain;

        }
    }
}
