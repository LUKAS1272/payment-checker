using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

class PaymentChecker
{
    static void Main(string[] args)
    {
        // --------------------------------------------------------------------------------------------
        // Args start
        // --------------------------------------------------------------------------------------------
        
            string paymentsCsvPath = "payments.csv";    // Input payments file
            string peopleListPath = "people.txt";       // List of expected names
            int expectedAmount = 1500;                  // Set the expected amount here

            string accountNumber = "2002346557/2010";
            string eventName = "jarní STC teambuilding 2025";
            
            int noteIndex = 5;      // Index of the note column (counting from 0)
            int amountIndex = 1;    // Index of the amount column (counting from 0)

            string[] noteOptions = { "STC TMB J25", "STC TB J25" }; // Accepted formats of note

            char csvSplitter = ';'; // CSV column divider

        // --------------------------------------------------------------------------------------------
        // Args end
        // --------------------------------------------------------------------------------------------

        var paidPeople = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in File.ReadLines(paymentsCsvPath).Skip(1)) // Skip the header
        {
            var parts = line.Split(csvSplitter); // Split csv columns

            string note = parts[noteIndex].Trim().Replace("\"", ""); // Note
            string amountStr = parts[amountIndex].Trim().Replace("\"", ""); // Transaction amount

            if (!decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
            {
                Console.WriteLine("Could not parse amount!");
                continue;
            }

            if (amount == expectedAmount)
            {
                foreach (string noteOption in noteOptions)
                {
                    var match = Regex.Match(note, noteOption + "(?<name>.+)", RegexOptions.IgnoreCase);
                
                    if (match.Success)
                    {
                        string fullName = match.Groups["name"].Value.Trim();
                        paidPeople.Add(fullName);
                    }
                }
            }
            else
            {
                Console.WriteLine("Amount not matched!");
            }
        }

        var paid = new List<string>();
        var notPaid = new List<string>();

        foreach (var line in File.ReadLines(peopleListPath))
        {
            var name = line.Trim();
            if (string.IsNullOrWhiteSpace(name)) continue;

            bool hasPaid = paidPeople.Contains(name) || paidPeople.Contains(ConvertCzechToEnglish(name));
            var nameParts = name.Split(' ');
            string firstName = nameParts.Length > 0 ? nameParts[0] : "";
            string surname = nameParts.Length > 1 ? nameParts[1] : "";

            if (hasPaid) {
                paid.Add($"{firstName} {surname}");
            } else {
                notPaid.Add($"{firstName} {surname}");
            }
        }

        File.WriteAllLines("paid.txt", paid);
        File.WriteAllLines("notPaid.txt", notPaid);

        Console.WriteLine("Ahoj, přihlásil/a ses na " + eventName + ", ale stále jsme od tebe neobdrželi platbu. Máš stále zájem jet, nebo ne? Pokud ano, uhraď, prosím, co nejdříve částku " + expectedAmount + "Kč na účet " + accountNumber + " s poznámkou '" + noteOptions[0] + " jmeno prijmeni'.");
    }

    static string ConvertCzechToEnglish(string input)
    {
        StringBuilder output = new StringBuilder(input);
        output.Replace("á", "a")
              .Replace("č", "c")
              .Replace("ď", "d")
              .Replace("é", "e")
              .Replace("ě", "e")
              .Replace("í", "i")
              .Replace("ň", "n")
              .Replace("ó", "o")
              .Replace("ř", "r")
              .Replace("š", "s")
              .Replace("ť", "t")
              .Replace("ú", "u")
              .Replace("ů", "u")
              .Replace("ý", "y")
              .Replace("ž", "z")
              .Replace("Á", "A")
              .Replace("Č", "C")
              .Replace("Ď", "D")
              .Replace("É", "E")
              .Replace("Ě", "E")
              .Replace("Í", "I")
              .Replace("Ň", "N")
              .Replace("Ó", "O")
              .Replace("Ř", "R")
              .Replace("Š", "S")
              .Replace("Ť", "T")
              .Replace("Ú", "U")
              .Replace("Ů", "U")
              .Replace("Ý", "Y")
              .Replace("Ž", "Z");

        return output.ToString();
    }
}
