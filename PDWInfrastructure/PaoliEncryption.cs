using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Xml;

namespace PDWInfrastructure
{
	public class PaoliEncryption
	{
		private static string _dataPassPhrase = null;
		public static string DataPassPhrase
		{
			get
			{
				if( _dataPassPhrase != null )
				{
					return _dataPassPhrase;
				}
				EncryptionConfiguration encConfig = (EncryptionConfiguration)ConfigurationManager.GetSection( "encSettings" );
				_dataPassPhrase = encConfig.DataPassPhrase.Value;
				return _dataPassPhrase;
			}
		}

		private byte[] localSalt = new byte[] { 0x24, 0x65, 0x4F, 0x0A, 0xC9, 0xB6, 0xF3, 0x9D, 0xF9, 0x82, 0xA3, 0x64, 0x6e, 0x20, 0x49, 0x4d };
		private string localPassPhrase { get; set; }

		public PaoliEncryption( string passPhrase )
		{
			localPassPhrase = passPhrase;
		}

		public string GetRandomCharacters( bool viewableOnly, int length )
		{
			string characterList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";

			if( viewableOnly )
			{
				characterList = string.Join( "", characterList.Where( c => !("aeiouAEIOUl0!@#$%^&*()".Contains( c )) ) );
			}

			string retVal = string.Empty;
			Random rdm = new Random();
			while( retVal.Length < length )
			{
				retVal += characterList[rdm.Next( characterList.Length - 1 )];
			}

			return retVal;
		}

		private byte[] Encrypt( byte[] clearData, byte[] Key, byte[] IV )
		{
			MemoryStream ms = new MemoryStream();

			Rijndael alg = Rijndael.Create();

			alg.Key = Key;
			alg.IV = IV;

			CryptoStream cs = new CryptoStream( ms,
			   alg.CreateEncryptor(), CryptoStreamMode.Write );

			cs.Write( clearData, 0, clearData.Length );

			cs.Close();

			byte[] encryptedData = ms.ToArray();

			return encryptedData;
		}

		public string Encrypt( string clearText )
		{
			string salt = GetNewSalt();
			string cipherData = Encrypt( clearText, salt );
			return salt + cipherData;
		}

		public string Encrypt( string clearText, string salt )
		{
			byte[] clearBytes =
			  System.Text.Encoding.Unicode.GetBytes( salt + clearText );

			Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes( localPassPhrase, localSalt );

			byte[] encryptedData = Encrypt( clearBytes,
					 pdb.GetBytes( 32 ), pdb.GetBytes( 16 ) );

			return Convert.ToBase64String( encryptedData );
		}

		public string Decrypt( string cipherDataAndSalt )
		{
			string salt = cipherDataAndSalt.Substring( 0, 24 );
			string cipherData = cipherDataAndSalt.Remove( 0, 24 );

			return Decrypt( cipherData, salt );
		}

		public string Decrypt( string cipherData, string salt )
		{
			byte[] cipherBytes =
				Convert.FromBase64String( cipherData );

			Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes( localPassPhrase, localSalt );

			string returnValue = Decrypt( cipherBytes, pdb.GetBytes( 32 ), pdb.GetBytes( 16 ) );
			if( returnValue.IndexOf( salt ) != 0 )
			{
				throw new Exception( "salt does not match for decryption" );
			}
			return returnValue.Remove( 0, salt.Length );
		}

		private string Decrypt( byte[] cipherData,
								byte[] Key, byte[] IV )
		{
			MemoryStream ms = new MemoryStream();

			Rijndael alg = Rijndael.Create();

			alg.Key = Key;
			alg.IV = IV;

			CryptoStream cs = new CryptoStream( ms,
				alg.CreateDecryptor(), CryptoStreamMode.Write );

			cs.Write( cipherData, 0, cipherData.Length );

			cs.Close();

			byte[] decryptedData = ms.ToArray();

			return (new System.Text.UnicodeEncoding()).GetString( decryptedData );
		}

		public string GetNewSalt()
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] bytes = new byte[16];
			rng.GetBytes( bytes );
			return Convert.ToBase64String( bytes );
		}

		public CryptoStream GetFileEncryptedStream( FileStream fStream )
		{
			Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes( localPassPhrase, localSalt );

			return GetFileEncryptedStream( fStream, pdb.GetBytes( 32 ), pdb.GetBytes( 16 ) );
		}

		private CryptoStream GetFileEncryptedStream( FileStream fStream, byte[] Key, byte[] IV )
		{
			Rijndael alg = Rijndael.Create();

			alg.Key = Key;
			alg.IV = IV;

			return new CryptoStream( fStream,
				alg.CreateEncryptor(),
				CryptoStreamMode.Write );
		}

		public CryptoStream GetFileDecryptedStream( FileStream fStream )
		{
			Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes( localPassPhrase, localSalt );

			return GetFileDecryptedStream( fStream, pdb.GetBytes( 32 ), pdb.GetBytes( 16 ) );
		}

		private CryptoStream GetFileDecryptedStream( FileStream fStream, byte[] Key, byte[] IV )
		{
			Rijndael alg = Rijndael.Create();

			alg.Key = Key;
			alg.IV = IV;

			return new CancellableCryptoStream( fStream,
				alg.CreateDecryptor(),
				CryptoStreamMode.Read );
		}

		private class CancellableCryptoStream : CryptoStream
		{
			private readonly CancellableCryptoTransform _transform;

			public CancellableCryptoStream( Stream stream, ICryptoTransform transform, CryptoStreamMode mode )
				: this( stream, new CancellableCryptoTransform( transform ), mode )
			{
			}

			private CancellableCryptoStream( Stream stream, CancellableCryptoTransform transform, CryptoStreamMode mode )
				: base( stream, transform, mode )
			{
				_transform = transform;
			}

			protected override void Dispose( bool disposing )
			{
				try
				{
					if( disposing )
					{
						_transform.Dispose();
					}
				}
				finally
				{
					base.Dispose( disposing );
				}
			}

			private class CancellableCryptoTransform : ICryptoTransform
			{
				private readonly ICryptoTransform _transform;
				private bool _disposed;

				public CancellableCryptoTransform( ICryptoTransform transform )
				{
					_transform = transform;
				}

				public bool CanReuseTransform
				{
					get { return _transform.CanReuseTransform; }
				}

				public bool CanTransformMultipleBlocks
				{
					get { return _transform.CanTransformMultipleBlocks; }
				}

				public int InputBlockSize
				{
					get { return _transform.InputBlockSize; }
				}

				public int OutputBlockSize
				{
					get { return _transform.OutputBlockSize; }
				}

				public int TransformBlock( byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset )
				{
					return _transform.TransformBlock( inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset );
				}

				public byte[] TransformFinalBlock( byte[] inputBuffer, int inputOffset, int inputCount )
				{
					if( _disposed )
						return new byte[0];

					return _transform.TransformFinalBlock( inputBuffer, inputOffset, inputCount );
				}

				public void Dispose()
				{
					_disposed = true;
				}
			}
		}

	}
}
