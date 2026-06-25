using SummitGrid.Core.Entities;

namespace SummitGrid.Api.Models;

public class RegisterRequest
{
    public string Username {get; set;} = string.Empty;

    public string Password {get; set;} = string.Empty;

    public UserRole Role { get; set; }
}