using Cipher;
using Cipher.Library;
using Cipher.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using System.Reflection;


namespace Cipher.Tests.Library
{
    [TestClass]
    public class VigenerEncryptorTest
    {
        string plainText = "Привет мир 123 Hello\nЭто тест!";
        string cipherText = "Ябсдйе ьщщ 123 Hello\nЯчб вхъф!";
        string keyWord = "Привет";

        private delegate short[] DelStrToIdxs(in string s);
        private delegate short[] DelGetKeyWordIdxs(in string s,VigenereEncryptor.Operation op);
        short[] keyWordIdxsCorrect = new short[] { };
        [TestMethod]
        public void StaticEncrypt()
        {
            Assert.AreEqual(cipherText, VigenereEncryptor.Encrypt(plainText, keyWord, VigenereEncryptor.Operation.Encrypt));
            Assert.AreEqual(plainText, VigenereEncryptor.Encrypt(cipherText, keyWord, VigenereEncryptor.Operation.Decrypt));
            Assert.AreEqual(null, VigenereEncryptor.Encrypt(cipherText, "bad keyWord", VigenereEncryptor.Operation.Decrypt));
        }
        [TestMethod]
        public void NonStaticEncrypt()
        {
            VigenereEncryptor ve = new VigenereEncryptor(keyWord, VigenereEncryptor.Operation.Encrypt);
            string[] input = plainText.Split('\n');
            string[] res = new string[input.Length];
            for (int i = 0; i < input.Length; ++i)
            {
                res[i] = ve.Encrypt(input[i]);
            }
            Assert.AreEqual(String.Join("\n",res), cipherText);
        }
        [TestMethod]
        public void EncryptChar()
        {
            char res;
            {
                Assert.IsTrue(VigenereEncryptor.EncryptChar('А', 5, out res));
                Assert.AreEqual('Е', res);
            }
            {
                Assert.IsFalse(VigenereEncryptor.EncryptChar('F', 5, out res));
                Assert.AreEqual('F', res);
            }
        }
        
        [TestMethod]
        public void StrToIdxs()
        {
            var strToIdxs = typeof(VigenereEncryptor).GetMethod("StrToIdxs", BindingFlags.NonPublic | BindingFlags.Static);
            var strToIdxsDel = strToIdxs.CreateDelegate(typeof(DelStrToIdxs)) as DelStrToIdxs;
            var result = strToIdxsDel(keyWord.ToLower());
            Assert.IsTrue(result.SequenceEqual(new short[] { 16, 17, 9, 2, 5, 19 }));
        }
        [TestMethod]
        public void GetKeyWordIdxs()
        {
            var strToIdxs = typeof(VigenereEncryptor).GetMethod("GetKeyWordIdxs", BindingFlags.NonPublic | BindingFlags.Static);
            var strToIdxsDel = strToIdxs.CreateDelegate(typeof(DelGetKeyWordIdxs)) as DelGetKeyWordIdxs;
            {
                var result = strToIdxsDel(keyWord.ToLower(), VigenereEncryptor.Operation.Encrypt);
                Assert.IsTrue(result.SequenceEqual(new short[] { 16, 17, 9, 2, 5, 19 }));
            }
            {
                var result = strToIdxsDel(keyWord.ToLower(), VigenereEncryptor.Operation.Decrypt);
                Assert.IsTrue(result.SequenceEqual(new short[] { 17,16,24,31,28,14 }));
            }
            { // invalid keyWord
                var result = strToIdxsDel("qwe", VigenereEncryptor.Operation.Decrypt);
                Assert.IsTrue(result == null);
            }
        }
    }
}
