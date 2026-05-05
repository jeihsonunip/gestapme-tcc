using System.ComponentModel.DataAnnotations;

namespace GestaPME.Validators;

public class ValidadorCpf : ValidationAttribute{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx){
        var cpf = (value as string ?? string.Empty).Replace(".", "").Replace("-", "").Trim();
        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            return new ValidationResult("CPF inválido.");
        if (cpf.Distinct().Count() == 1) return new ValidationResult("CPF inválido.");

        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        int soma = 0;
        for (int i = 0; i < 9; i++) soma += (cpf[i] - '0') * mult1[i];
        int d1 = soma % 11 < 2 ? 0 : 11 - soma % 11;

        soma = 0;
        for (int i = 0; i < 10; i++) soma += (cpf[i] - '0') * mult2[i];
        int d2 = soma % 11 < 2 ? 0 : 11 - soma % 11;

        if (cpf[9] - '0' != d1 || cpf[10] - '0' != d2)
            return new ValidationResult("CPF inválido.");

        return ValidationResult.Success;
    }
}