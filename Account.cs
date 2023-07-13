using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;


namespace ConsoleApp79
{  
    [Serializable]
    class Account
    {
        public Account() {
            string uName;
            Console.Write("Enter username: ");
            uName = Console.ReadLine();
            while (true) {
                if (Regex.IsMatch(uName, @"\A[\w ]+\z")) {
                    if (!File.Exists($"Accounts\\{uName}.dll")) break;
                    Console.Write("Account name already taken, try a different one: ");
                }
                else Console.WriteLine("Username can only contain letters, underscores and spaces!");
                
                uName = Console.ReadLine();
            }
            //a Console.WriteLine("sukes");
            userName = uName;
            Console.WriteLine("Password must be between 8 and 16 characters and must contain letters and numbers!");
            Console.Write("Enter password: ");
            string password = Program.HiddenString();
            Console.WriteLine();
            while (true)
            {
                if (Regex.IsMatch(password, @"\A(?=.*[0-9]+)(?=.*\w+).{8,16}\z")) break;
                Console.WriteLine("password must be between 8 and 16 characters and must contain letters and numbers!");
                Console.Write("Enter password: ");
                password = Program.HiddenString();
                Console.WriteLine();
            }
            Console.Write("Repeat password: ");
            string repeatPassword = Program.HiddenString();
            Console.WriteLine();
            while (true){
                if (repeatPassword == "!") {
                    password = Program.HiddenString();
                    Console.WriteLine();
                    Console.Write("Repeat password: ");
                    repeatPassword = Program.HiddenString();
                    Console.WriteLine();
                    continue;
                }
                else if (password == repeatPassword) break;
                Console.Write("Passwords don't match. Try again ot enter \"!\" to enter a different password: ");
                repeatPassword = Program.HiddenString();
                Console.WriteLine();
            }

            byte[] salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);
            var pdfk = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pdfk.GetBytes(20);
            byte[] hashSalt = new byte[36];
            Array.Copy(hash, 0, hashSalt, 0, 20);
            Array.Copy(salt, 0, hashSalt, 20, 16);
            PasswordHash = Convert.ToBase64String(hashSalt);
            // Console.WriteLine(PasswordHash);

            
            while (true)
            {
                Console.WriteLine("Enter the information which you want to save:");
                string dataIn = Console.ReadLine();
                data = Program.EnDecrypt(dataIn, password);
                Console.WriteLine();
                Console.WriteLine(string.Join("", Program.EnDecrypt(string.Join("", data), password)));

                Console.WriteLine("Do you wish to continue?");
                string cont = Console.ReadLine();
                while (true)
                {
                    if (Regex.IsMatch(cont, @"\A(y|Y|yes|Yes|YES)\z")) goto InfoDone;
                    else if (Regex.IsMatch(cont, @"\A(n|N|no|No|NO)\z")) break;
                    Console.WriteLine("Enter yes or no");
                    cont = Console.ReadLine();
                }
                
            }

            InfoDone:
            Console.Write(new string(' ', 21));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("\nInformation saved successfully!");
            Console.ReadKey();
        }
        public string userName;
        string passwordHash;
        public string[] data { get; set; }

        public string PasswordHash { get; protected set; }

        public override string ToString()
        {
            return userName + PasswordHash;
        }
        public string RetrieveData() {
            return string.Join("", data);
        }
    }
}
