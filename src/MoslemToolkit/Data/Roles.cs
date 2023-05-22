namespace MoslemToolkit.Data
{
    public enum Roles { User,Pengurus, KU, PJKBM }
    public class RoleInfo
    {
        public static bool HasRole(string Username, Roles role)
        {
            switch (role)
            {
                case Roles.Pengurus:
                    return Username == AppConstants.USER_ADMIN;
                case Roles.KU:
                    return Username == AppConstants.USER_KU;
                case Roles.PJKBM:
                    return Username == AppConstants.USER_PJKBM;
                case Roles.User:
                default:
                    return true; 
            }
        }
    }
}
