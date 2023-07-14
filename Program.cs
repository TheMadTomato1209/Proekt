using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ConsoleApp79
{
    class Program
    {
        public static string HiddenString() {
            StringBuilder stringBuilder = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo keyI = Console.ReadKey(true);
                char keyChar = keyI.KeyChar;
                //Console.WriteLine((int)keyChar);
                if (keyChar >= 32 && keyChar <= 126)
                {
                    Console.Write('*');
                    stringBuilder.Append(keyChar);
                }
                else if (keyI.Key == ConsoleKey.Enter) return stringBuilder.ToString();
                else if (keyI.Key == ConsoleKey.Backspace && Console.CursorLeft != 0 && stringBuilder.Length != 0) {
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                }
            }
        }
        public static bool PasswordThings(Account account, string password) {
            byte[] savedPasswordHash = Convert.FromBase64String(account.PasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(savedPasswordHash, 20, salt, 0, 16);

            var pdkf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] newPasswordHash = pdkf2.GetBytes(20);

            bool match = true;
            for (int i = 0; i < 20; i++)
            {
                if (savedPasswordHash[i] != newPasswordHash[i]) {
                    match = false;
                    break;
                }
            }
            return match;
        }
        public static string[] EnDecrypt(string input, string key) {
            string[] inBinary = new string[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                inBinary[i] = Convert.ToString((int)input[i], 2);
                if (inBinary[i].Length < 7)
                {
                    StringBuilder sB = new StringBuilder();
                    sB.Append(inBinary[i]);
                    for (int j = inBinary[i].Length; j < 7; j++)
                    {
                        // Console.WriteLine(sB + "alo");
                        sB.Insert(0, "0");
                    }
                    // Console.WriteLine(sB + "ale");
                    inBinary[i] = sB.ToString();
                }
                //b Console.Write(inBinary[i] + " ");
            }
            //a Console.WriteLine();

            string[] keyBinary = new string[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                keyBinary[i] = Convert.ToString((int)key[i], 2);
                if (keyBinary[i].Length < 7)
                {
                    StringBuilder sB = new StringBuilder();
                    sB.Append(keyBinary[i]);
                    for (int j = keyBinary[i].Length; j < 7; j++)
                    {
                        // Console.WriteLine(sB + "alo");
                        sB.Insert(0, "0");
                    }
                    // Console.WriteLine(sB + "ale");
                    keyBinary[i] = sB.ToString();
                }
                //b Console.Write(keyBinary[i] + " ");
            }

            //b Console.WriteLine();
            string[] encryptedBinary = new string[inBinary.Length];
            for (int i = 0; i < inBinary.Length; i++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int j = 0; j < inBinary[i].Length; j++)
                {
                    int ki = i;
                    while (ki >= key.Length) ki -= key.Length;
                    if (int.Parse(inBinary[i][j].ToString()) + int.Parse(keyBinary[ki][j].ToString()) == 1) stringBuilder.Append("1");
                    else stringBuilder.Append("0");
                }
                //b Console.Write(stringBuilder.ToString() + " ");
                encryptedBinary[i] = ((char)Convert.ToInt32(stringBuilder.ToString(), 2)).ToString();
                //a Console.Write(encryptedBinary[i]);

            }
        //    Console.WriteLine();
        //    foreach (var item in encryptedBinary)
        //    {
        //        Console.Write(item);
        //    }
              return encryptedBinary;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(HiddenString());
            Console.ReadKey();
            Menu:
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Welcome to Kryptor!");
                Console.WriteLine("1. Create account");
                Console.WriteLine("2. Log in");
                Console.WriteLine("3. Exit");
                Console.WriteLine("Enter the number of your choice:");
                string choice = Console.ReadLine();
                while (true)
                {
                    if (choice == "1")
                    {
                        Console.Write(new string(' ', 21));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Account account = new Account();
                        if (!Directory.Exists("Accounts")) Directory.CreateDirectory("Accounts");
                        Stream stream = new FileStream($"Accounts\\{account.userName}.dll", FileMode.Create, FileAccess.Write);
                        IFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, account);
                        stream.Close();
                        goto Menu;
                    }
                    else if (choice == "2")
                    {
                        Console.Write(new string(' ', 21));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        //Console.WriteLine("yahu");

                        Console.Write("Enter Username: ");
                        string uName = Console.ReadLine();

                        while (true)
                        {
                            if (Regex.IsMatch(uName, @"\A[\w ]+\z"))
                            {
                                if (File.Exists($"Accounts\\{uName}.dll")) break;
                                Console.Write("Account not found, try again: ");
                            }
                            else Console.WriteLine("Username can only contain letters, underscores and spaces!");

                            uName = Console.ReadLine();
                        }

                        Stream stream = new FileStream($"Accounts\\{uName}.dll", FileMode.Open, FileAccess.Read);
                        IFormatter formatter = new BinaryFormatter();
                        Account getAccount = (Account)formatter.Deserialize(stream);
                        stream.Close();
                        Console.Write("Enter password: ");
                        string password = HiddenString();
                        Console.WriteLine();
                        while (true)
                        {
                            if (PasswordThings(getAccount, password)) break;
                            Console.WriteLine("Wrong password!");
                            Console.Write("Enter password: ");
                            password = HiddenString();
                            Console.WriteLine();
                        }
                        Console.WriteLine("Successfully logged in! Do you wish to see your information?");
                        string cont = Console.ReadLine();
                        while (true)
                        {
                            if (Regex.IsMatch(cont, @"\A(y|Y|yes|Yes|YES)\z")) break;
                            else if (Regex.IsMatch(cont, @"\A(n|N|no|No|NO)\z")) goto Menu;
                            Console.WriteLine("Enter yes or no");
                            cont = Console.ReadLine();
                        }

                        Console.WriteLine(string.Join("", EnDecrypt(getAccount.RetrieveData(), password)));

                        Console.WriteLine("Do you wish to save new information?");
                        cont = Console.ReadLine();
                        while (true)
                        {
                            if (Regex.IsMatch(cont, @"\A(y|Y|yes|Yes|YES)\z")) break;
                            else if (Regex.IsMatch(cont, @"\A(n|N|no|No|NO)\z"))
                            {
                                Console.WriteLine("Logging out...");
                                Thread.Sleep(1000);
                                goto Menu;
                            }
                            Console.WriteLine("Enter yes or no");
                            cont = Console.ReadLine();
                        }

                        Console.WriteLine("Enter the information which you want to save:");
                        string dataIn = Console.ReadLine();
                        getAccount.data = Program.EnDecrypt(dataIn, password);
                        Console.WriteLine();
                        Console.WriteLine(string.Join("", Program.EnDecrypt(string.Join("", getAccount.data), password)));

                        Console.WriteLine("Do you wish to continue?");
                        cont = Console.ReadLine();
                        while (true)
                        {
                            if (Regex.IsMatch(cont, @"\A(y|Y|yes|Yes|YES)\z")) goto InfoDone;
                            else if (Regex.IsMatch(cont, @"\A(n|N|no|No|NO)\z")) break;
                            Console.WriteLine("Enter yes or no");
                            cont = Console.ReadLine();
                        }

                        InfoDone:
                        Console.Write(new string(' ', 21));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.WriteLine("\nInformation saved successfully!");
                        Console.ReadKey();

                        stream = new FileStream($"Accounts\\{getAccount.userName}.dll", FileMode.Create, FileAccess.Write);
                        formatter = new BinaryFormatter();
                        formatter.Serialize(stream, getAccount);
                        stream.Close();
                        goto Menu;
                    }
                    else if (choice == "3")
                    {
                        Console.WriteLine("Thank you for using Kryptor!");
                        Environment.Exit(0);
                    }
                    Console.Write("Enter a valid number!");
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string(' ', choice.Length));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    choice = Console.ReadLine();
                    //
                }

            }
        }
    }
}
