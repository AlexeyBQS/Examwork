using Microsoft.Win32;
using Schedule.Database;
using Schedule.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Services
{
    public class PasswordManager
    {
        private PasswordManager() { }
        private PasswordManager(string hash)
        {
            Hash = hash;
        }
        private PasswordManager(Password password)
        {
            Hash = password?.Hash ?? string.Empty;
        }

        private static readonly string _salt = "Tn0JwhBECL52";

        public string Hash { get; set; } = null!;

        public static PasswordManager? GetPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return null;

            string passwordWithSalt = _salt + password + _salt;

            byte[] data = Encoding.UTF8.GetBytes(passwordWithSalt);

            using SHA512 alg = SHA512.Create();
            string hex = string.Concat(alg.ComputeHash(data).Select(x => string.Format("{0:x2}", x)));

            return new(hex);
        }

        public static void SetPasswordInDatabase(PasswordManager? passwordManager)
        {
            if (passwordManager! as object == null)
            {
                DeletePasswordFromDatabase();
                return;
            }

            using DatabaseContext context = new();

            bool existPassword = context.Passwords.FirstOrDefault(x => x.PasswordId == 1) != null;
            Password password = context.Passwords.FirstOrDefault(x => x.PasswordId == 1) ?? new();

            password.PasswordId = 1;
            password.Hash = passwordManager.Hash ?? string.Empty;

            if (!existPassword) context.Passwords.Add(password);

            context.SaveChanges();
        }

        public static PasswordManager? GetPasswordFromDatabase()
        {
            using DatabaseContext context = new();

            return context.Passwords.FirstOrDefault(x => x.PasswordId == 1) != null
                ? new(context.Passwords.First(x => x.PasswordId == 1))
                : null;
        }

        public static void DeletePasswordFromDatabase()
        {
            using DatabaseContext context = new();

            Password password = context.Passwords.FirstOrDefault(x => x.PasswordId == 1) ?? null!;

            if (password != null) context.Passwords.Remove(password);

            context.SaveChanges();
        }


        public override string ToString() => Hash;
        public override bool Equals(object? obj) => obj is PasswordManager passwordManager && Hash == passwordManager.Hash;
        public override int GetHashCode() => Hash.GetHashCode();

        public static bool operator ==(PasswordManager passwordManager1, PasswordManager passwordManager2)
        {
            if (passwordManager1 is null && passwordManager2 is null) return true;
            if (passwordManager1 is null || passwordManager2 is null) return false;

            return passwordManager1.Hash == passwordManager2.Hash;
        }
        public static bool operator !=(PasswordManager passwordManager1, PasswordManager passwordManager2)
        {
            if (passwordManager1 is null && passwordManager2 is null) return false;
            if (passwordManager1 is null || passwordManager2 is null) return true;

            return passwordManager1.Hash != passwordManager2.Hash;
        }
    }
}