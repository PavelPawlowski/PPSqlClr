using Microsoft.SqlServer.Server;
using PP.SqlClrSafe.Cryptography;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Class containing functions for hanling Cryptography
/// </summary>
public class Cryptography
{
    /// <summary>
    /// Computes Hash of input data using specified Hash Algorithm
    /// </summary>
    /// <param name="algorithm">Hash Algorithm to be used for hash calculation</param>
    /// <param name="inputData">Input data to be hashed</param>
    /// <returns>hash of input data</returns>
    private static byte[] ComputeHash(string algorithm, byte[] inputData)
    {
#if NET35
        string suportedHashAlgorigthms = "MD5, RipeMD160, SHA1, SHA256, SHA284, SHA512, CRC32";
#else
    string suportedHashAlgorigthms = "MD5, RipeMD160, SHA1, CRC32";
#endif
        string ha = algorithm.ToUpper();

        HashAlgorithm alg;
        //Get CryptoServiceProvider based on algorithm
        switch (ha)
        {
            case "MD5":
                alg = new MD5CryptoServiceProvider();
                break;
            case "RIPEMD160":
                alg = new RIPEMD160Managed();
                break;
            case "SHA1":
                alg = new SHA1CryptoServiceProvider();
                break;
#if NET35
            case "SHA256":
                alg = new SHA256CryptoServiceProvider();
                break;
            case "SHA384":
                alg = new SHA384CryptoServiceProvider();
                break;
            case "SHA512":
                alg = new SHA512CryptoServiceProvider();
                break;
#endif
            case "CRC32":
                alg = new Crc32();
                break;
            default:
                throw new System.ArgumentException(string.Format("Unsupported HASH algorithm '{0}'. Supported algorithms are: {1}", algorithm, suportedHashAlgorigthms), "algorithm");
        }

        byte[] buffer = alg.ComputeHash(inputData);
        return buffer;
    }

    /// <summary>
    /// Computes hash of input data using specified hash algorithm
    /// </summary>
    /// <param name="algorithm">Hash algorithm to be used</param>
    /// <param name="inputData">Input data to be hashed</param>
    /// <returns>Hash of input data</returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_ComputeHashData")]
    public static SqlBytes ComputeHashData(SqlString algorithm, SqlBytes inputData)
    {
        byte[] buffer = inputData.IsNull ? null : inputData.Value;
        string alg = algorithm.IsNull ? string.Empty : algorithm.Value;

        byte[] hashBuffer = ComputeHash(alg, buffer);
        return new SqlBytes(hashBuffer);
    }

    /// <summary>
    /// Computes hash of the input string using specified algorithm and string encoding
    /// </summary>
    /// <param name="algorithm">Hash algorithm to be used for hash calculation</param>
    /// <param name="encoding"></param>
    /// <param name="inputString"></param>
    /// <returns></returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_ComputeHashString")]
    public static SqlBytes ComputeHashString(SqlString algorithm, SqlString encoding, SqlString inputString)
    {
        SqlBytes stringBuffer = StringsCommon.StringToByteArray(encoding, inputString);

        return ComputeHashData(algorithm, stringBuffer);
    }
}
