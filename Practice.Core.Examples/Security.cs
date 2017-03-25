using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Cryptography;
using System.IO.Compression;

namespace Practice.Core.Examples
{
    public class Security
    {
        [DllImport("shell32.dll", EntryPoint = "#680")]
        static extern bool IsUserAdmin();

        public void Test()
        {
            CodeAccessPermission perm = new FileIOPermission(PermissionState.None);

            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            byte[] hash;
            using (Stream fs = File.OpenRead("checkme.doc"))
                hash = MD5.Create().ComputeHash(fs);        // 16 bytes long

            //also accepts a byte array, which is convenient for hashing passwords
            byte[] data = Encoding.UTF8.GetBytes("randomStrongPassword");
            //byte[] hash = SHA256.Create().ComputeHash(data);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "Claims", Name = "Jack")]
        public void SomeRandomClaimsFunctionality(int year)
        {

        }

        public void Encrypt()
        {
            byte[] key = { 145, 12, 32, 245, 98, 132, 98, 214, 6, 77, 131, 44, 221, 3, 9, 50 };
            byte[] iv = { 15, 122, 132, 5, 93, 198, 44, 31, 9, 39, 241, 49, 250, 188, 80, 7 };

            byte[] data = { 1, 2, 3, 4, 5 }; // This is what we're encrypting.

            using (SymmetricAlgorithm algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
            using (Stream f = File.Create("encrypted.bin"))
            using (Stream c = new CryptoStream(f, encryptor, CryptoStreamMode.Write))
            {
                c.Write(data, 0, data.Length);
            }

            // decryption:

            byte[] decrypted = new byte[5];
            using (SymmetricAlgorithm algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
            using (Stream f = File.OpenRead("encrypted.bin"))
            using (Stream c = new CryptoStream(f, decryptor, CryptoStreamMode.Read))
                for (int b; (b = c.ReadByte()) > -1;)
                    Console.Write(b + " "); // 1 2 3 4 5

        }

        public async void ChainStreams()
        {
            // Use default key/iv for demo.
            using (Aes algorithm = Aes.Create())
            {
                using (ICryptoTransform encryptor = algorithm.CreateEncryptor())
                using (Stream f = File.Create("serious.bin"))
                using (Stream c = new CryptoStream(f, encryptor, CryptoStreamMode.Write))
                using (Stream d = new DeflateStream(c, CompressionMode.Compress))
                using (StreamWriter w = new StreamWriter(d))
                    await w.WriteLineAsync("Small and secure!");

                using (ICryptoTransform decryptor = algorithm.CreateDecryptor())
                using (Stream f = File.OpenRead("serious.bin"))
                using (Stream c = new CryptoStream(f, decryptor, CryptoStreamMode.Read))
                using (Stream d = new DeflateStream(c, CompressionMode.Decompress))
                using (StreamReader r = new StreamReader(d))
                    Console.WriteLine(await r.ReadLineAsync()); // Small and secure!
            }
        }

        public void PublicEncryption()
        {
            byte[] data = { 1, 2, 3, 4, 5 }; // data to encrypt

            using (var rsa = new RSACryptoServiceProvider())
            {
                byte[] encrypted = rsa.Encrypt(data, true);
                byte[] decrypted = rsa.Decrypt(encrypted, true);
            }
        }

        public void SignHash()
        {
            byte[] data = Encoding.UTF8.GetBytes("Message to sign");
            byte[] publicKey;
            byte[] signature;
            object hasher = SHA1.Create(); // Chosen hashing algorithm.

            // Generate a new key pair, then sign the data with it:
            using (var publicPrivate = new RSACryptoServiceProvider())
            {
                signature = publicPrivate.SignData(data, hasher);
                publicKey = publicPrivate.ExportCspBlob(false); // get public key
            }

            // Create a fresh RSA using just the public key, then test the signature.
            using (var publicOnly = new RSACryptoServiceProvider())
            {
                publicOnly.ImportCspBlob(publicKey);
                Console.Write(publicOnly.VerifyData(data, hasher, signature)); // True

                // Let's now tamper with the data, and recheck the signature:
                data[0] = 0;
                Console.Write(publicOnly.VerifyData(data, hasher, signature)); // False

                // This throws an exception as there is no private key:
                signature = publicOnly.SignData(data, hasher);
            }

            using (var rsa = new RSACryptoServiceProvider())
            {
                byte[] hash = SHA1.Create().ComputeHash(data);
                signature = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
            }
        }

        /// <summary>
        /// Sandboxing an assembly
        /// </summary>
        static void Main()
        {
            string pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            string plugInPath = Path.Combine(pluginFolder, "plugin.exe");

            PermissionSet ps = new PermissionSet(PermissionState.None);

            ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            ps.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery |
            FileIOPermissionAccess.Read, plugInPath));
            ps.AddPermission(new UIPermission(PermissionState.Unrestricted));

            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain sandbox = AppDomain.CreateDomain("sbox", null, setup, ps);
            sandbox.ExecuteAssembly(plugInPath);
            AppDomain.Unload(sandbox);
        }
    }
}
