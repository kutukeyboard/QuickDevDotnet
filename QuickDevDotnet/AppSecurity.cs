using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;


namespace QuickDevDotnet
{
	public class AppSecurity
	{
		#region "Encryption Declaration"

		static string PasswordHash = "T3stK3y";
		static readonly string SaltKey = "Ju5T1nT1M3";
		static readonly string VIKey = "@1B2c3D4e5F6g7H8";

		#endregion

		#region "Encryption"

		public string EncryptText(string input)
		{
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(input);
			byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
			var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
			byte[] cipherTextBytes;
			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipherTextBytes = memoryStream.ToArray();
					cryptoStream.Close();
				}
				memoryStream.Close();
			}
			return Convert.ToBase64String(cipherTextBytes);
		}

		public string DecryptText(string input)
		{
			byte[] cipherTextBytes = Convert.FromBase64String(input);
			byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };
			var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
			var memoryStream = new MemoryStream(cipherTextBytes);
			var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			byte[] plainTextBytes = new byte[cipherTextBytes.Length];
			int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
			memoryStream.Close();
			cryptoStream.Close();
			return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
		}

		public void EncryptFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				try
				{
					string myString = File.ReadAllText(filePath);
					myString = EncryptText(myString);
					File.WriteAllText(filePath, myString);
					PasswordHash = "";
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					return;
				}
			}
			else
			{
				return;
			}
		}

		public void DecryptFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				try
				{
					string myString = File.ReadAllText(filePath);
					myString = DecryptText(myString);
					File.WriteAllText(filePath, myString);
					PasswordHash = "";
				}
				catch (Exception ex)
				{
					return;
				}
			}
			else
			{
				return;
			}

		}

		#endregion

		

	}
}
