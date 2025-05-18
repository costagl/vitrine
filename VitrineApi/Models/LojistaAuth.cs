using Microsoft.AspNetCore.Identity;

public class LojistaAuth : IdentityUser
{
    public string Cpf { get; set; }
}