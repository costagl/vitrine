using Microsoft.AspNetCore.Identity;

public class CustomPasswordValidator : IPasswordValidator<LojistaAuth>
{
    public Task<IdentityResult> ValidateAsync(UserManager<LojistaAuth> manager, LojistaAuth user, string password)
    {
        var errors = new List<IdentityError>();

        // Validação personalizada: verifica se a senha tem ao menos um dígito
        if (!password.Any(char.IsDigit))
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordRequiresDigit",
                Description = "Sua senha deve conter ao menos um número."
            });
        }

        // Validação personalizada: verifica se a senha tem ao menos uma letra minúscula
        if (!password.Any(char.IsLower))
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordRequiresLower",
                Description = "Sua senha deve conter ao menos uma letra minúscula."
            });
        }

        // Validação personalizada: verifica se a senha tem ao menos uma letra maiúscula
        if (!password.Any(char.IsUpper))
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordRequiresUpper",
                Description = "Sua senha deve conter ao menos uma letra maiúscula."
            });
        }

        // Validação personalizada: verifica se a senha tem ao menos um caractere especial
        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = "Sua senha deve conter ao menos um caractere especial."
            });
        }

        // Validação personalizada: verifica o comprimento mínimo da senha
        if (password.Length < 6)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Sua senha deve ter no mínimo 6 caracteres."
            });
        }

        // Se houver erros, retorna os erros encontrados
        return errors.Any() ? Task.FromResult(IdentityResult.Failed(errors.ToArray())) : Task.FromResult(IdentityResult.Success);
    }
}
