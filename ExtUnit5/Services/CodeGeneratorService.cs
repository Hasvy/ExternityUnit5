namespace ExtUnit5.Services
{
    public class CodeGeneratorService
    {
        private Random random = new Random();
        private string characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string GenerateCode(int length)
        {
            return new string(Enumerable.Repeat(characterSet, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
