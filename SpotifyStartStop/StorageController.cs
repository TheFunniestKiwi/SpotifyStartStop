using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyStartStop
{
    public static class StorageController
    {

        public static void SaveToken(string token)
        {
            File.WriteAllText("token.txt", token);
        }

        public static string LoadToken()
        {
            if (File.Exists("token.txt"))
                return File.ReadAllText("token.txt");

            return "";
        }
    }
}