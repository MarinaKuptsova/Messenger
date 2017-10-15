using System;

public class User
{
	public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public byte[] Photo { get; set; }
    public bool IsActive { get; set; }
    public string UserGroup { get; set; }
}
