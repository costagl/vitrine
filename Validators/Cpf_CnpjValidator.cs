namespace VitrineApi.Validators
{
    public class Cpf_CnpjValidator
    {
        public bool ValidarCPF(string cpf)
        {
            // Remover caracteres não numéricos
            cpf = cpf.Replace(".", "").Replace("-", "");

            // Verificar se o CPF tem 11 dígitos
            if (cpf.Length != 11)
            {
                return false;
            }

            // Verificar se o CPF é uma sequência repetida de números
            if (new string(cpf[0], 11) == cpf)
            {
                return false;
            }

            // Validar o primeiro dígito verificador
            int soma = 0;
            int[] pesos1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 9; i++)
            {
                soma += (cpf[i] - '0') * pesos1[i];
            }
            int digito1 = 11 - (soma % 11);
            if (digito1 >= 10)
            {
                digito1 = 0;
            }

            // Validar o segundo dígito verificador
            soma = 0;
            int[] pesos2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 10; i++)
            {
                soma += (cpf[i] - '0') * pesos2[i];
            }
            int digito2 = 11 - (soma % 11);
            if (digito2 >= 10)
            {
                digito2 = 0;
            }

            // Log dos dígitos verificadores calculados
            //Console.WriteLine($"Primeiro Dígito Verificador: {digito1}");
            //Console.WriteLine($"Segundo Dígito Verificador: {digito2}");

            // Verificar se os dígitos calculados são iguais aos do CPF
            return cpf[9] == (char)(digito1 + '0') && cpf[10] == (char)(digito2 + '0');
        }

        public bool ValidarCNPJ(string cnpj)
        {
            // Remover caracteres não numéricos
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            // Verificar se o CNPJ tem 14 dígitos
            if (cnpj.Length != 14)
            {
                return false;
            }

            // Verificar se o CNPJ é uma sequência repetida de números
            if (new string(cnpj[0], 14) == cnpj)
            {
                return false;
            }

            // Validar o primeiro dígito verificador
            int soma = 0;
            int[] pesos1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 12; i++)
            {
                soma += (cnpj[i] - '0') * pesos1[i];
            }
            int digito1 = 11 - (soma % 11);
            if (digito1 >= 10)
            {
                digito1 = 0;
            }

            // Validar o segundo dígito verificador
            soma = 0;
            int[] pesos2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 13; i++)
            {
                soma += (cnpj[i] - '0') * pesos2[i];
            }
            int digito2 = 11 - (soma % 11);
            if (digito2 >= 10)
            {
                digito2 = 0;
            }

            // Log dos dígitos verificadores calculados
            //Console.WriteLine($"Primeiro Dígito Verificador: {digito1}");
            //Console.WriteLine($"Segundo Dígito Verificador: {digito2}");

            // Verificar se os dígitos calculados são iguais aos do CNPJ
            return cnpj[12] == (char)(digito1 + '0') && cnpj[13] == (char)(digito2 + '0');
        }

    }
}
