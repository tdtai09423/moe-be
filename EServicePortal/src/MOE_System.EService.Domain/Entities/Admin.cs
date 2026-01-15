namespace MOE_System.EService.Domain.Entities;

public class Admin
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
