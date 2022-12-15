using System.Linq;

namespace ISTU_MFC
{
    public static class TranslitController
    {
        private static string[] _rus = {"А","Б","В","Г","Д","Е","Ё","Ж", "З","И","Й","К","Л","М", "Н",
            "О","П","Р","С","Т","У","Ф","Х", "Ц", "Ч", "Ш", "Щ",   "Ъ", "Ы","Ь", 
            "Э","Ю", "Я" };
        private static string[] _eng = {"A","B","V","G","D","E","E","ZH","Z","I","Y","K","L","M","N",
            "O","P","R","S","T","U","F","KH","TS","CH","SH","SHCH","_","","Y","",
            "E","YU","YA"};
        public static string ToEn(string s)
        {
            string res = "";
            foreach (var c in s)
            {
                bool found = false;
                for (var i = 0; i < _rus.Length; i++)
                {
                    bool isLower = char.IsLower(c);
                    if (char.ToUpper(c).ToString() == _rus[i])
                    {
                        found = true;
                        string eng = _eng[i];
                        if (isLower)
                        {
                            eng = eng.ToLower();
                        }
                        res += eng;
                    }
                }
                if(!found)
                {
                    res += c;
                }
            }
            return res;
        }
    }
}