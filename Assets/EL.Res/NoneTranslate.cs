namespace EL.Res
{
    public class NoneTranslate : ITranslate
    {
        public string Translate(string key)
        {
            return key.Replace('.', ' ');
        }
    }
}