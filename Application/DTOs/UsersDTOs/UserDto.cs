namespace Application.DTOs.UsersDTOs;

public class UserDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public required string Email { get; set; }
}