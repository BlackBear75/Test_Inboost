using Test_INBOOST.Base;

namespace Test_INBOOST.Entity.User;

public class User : BaseData
{
    public long UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserRole Role { get; set; }
}

public enum UserRole
{
    Admin,
    User
}