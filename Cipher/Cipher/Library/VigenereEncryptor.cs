using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Cipher.Library
{
    public class VigenereEncryptor
    {
        public enum Operation
        {
            Encrypt,
            Decrypt
        }
        private static Regex _rusOnly = new Regex("^[А-Яа-яЁё]+$");
        public static char[] _rawAlphabet { get; } = new char[] { 'а','б','в','г','д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к',
                                                                        'л', 'м', 'н', 'о', 'п', 'р', 'с','т','у','ф',
                                                                        'х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я'};
        private short[] _kwIdxs;
        private int _kwCharIdx = 0;
        private Operation _op;
        public VigenereEncryptor(string keyWord, Operation op)
        {
            _kwIdxs = GetKeyWordIdxs(keyWord.ToLower(),op);
            _op = op;
        }
        public string Encrypt(in string text)
        {
            char[] result = null;
            if (_kwIdxs != null)
            {
                result = new char[text.Length];
                for (int i = 0; i < text.Length; ++i)
                {
                    if (EncryptChar(text[i], _kwIdxs[_kwCharIdx % _kwIdxs.Length], out result[i]))
                    {
                        ++_kwCharIdx;
                    }
                }
            }
            return result != null ? new string(result) : null;
        }
        public static string Encrypt(in string text, in string keyWord, Operation op) //returns null if keyword invalid
        {
            char[] result = null;
            short[] kwIdxs = GetKeyWordIdxs(keyWord.ToLower(),op);
            if (kwIdxs != null)
            {
                result = new char[text.Length];
                int j = 0;
                for (int i = 0; i < text.Length; ++i)
                {
                    if (EncryptChar(text[i], kwIdxs[j % kwIdxs.Length], out result[i]))
                    {
                        ++j;
                    }
                }
            }
            return result != null ? new string(result) : null;
        }
        public static bool EncryptChar(char c, short kwCharIdx, out char result)
        {
            bool isLower = char.IsLower(c);
            short SymbIdx = (short)Array.IndexOf(_rawAlphabet, char.ToLower(c));
            if (SymbIdx == -1)
            {
                result = c;
                return false;
            }
            else
            {
                SymbIdx = (short)((SymbIdx + kwCharIdx) % _rawAlphabet.Length);
                result = isLower ? _rawAlphabet[SymbIdx] : char.ToUpper(_rawAlphabet[SymbIdx]);
                return true;
            }
        }

        private static short[] StrToIdxs(in string s)
        {
            short[] result = new short[s.Length];
            for (int i = 0; i < s.Length; ++i)
            {
                result[i] = (short)Array.IndexOf(_rawAlphabet, s[i]);
                if (result[i] == -1)
                {
                    return null;
                }
            }
            return result;
        }
        private static short[] GetKeyWordIdxs(in string keyWord, Operation op)
        {
            short[] kwIdxs = StrToIdxs(keyWord.ToLower());
            if (kwIdxs == null)
            {
                return null;
            }
            if (op == Operation.Decrypt)
            {
                for (int i = 0; i < kwIdxs.Length; ++i)
                {
                    kwIdxs[i] = (short)(_rawAlphabet.Length - kwIdxs[i]);
                }
            }
            return kwIdxs;
        }
        public static bool ValidateKeyWord(string KeyWord)
        {
            return _rusOnly.IsMatch(KeyWord);
        }
    }
}