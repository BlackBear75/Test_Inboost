using Test_INBOOST.Entity.User;

namespace Test_INBOOST.Models.UsersModel;

public class GetUserResponce
{
    public Guid Id { get; set; }
    
    public UserRole Role { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}