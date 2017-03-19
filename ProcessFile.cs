using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinancialReport
{
    public class ProcessFile
    {
        public const string processed = "processed";
        public const string pending = "pending";
        public const string reports = "reports";
        public static string workingDirectory;

        public static void readFile(string fullPath, string filename)
        {
            HashSet<string> accounts = new HashSet<string>();
            double credits = 0;
            double debits = 0;
            int skippedTransactions = 0;
            
            using (FileStream fileStream = File.OpenRead(fullPath))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                bool isHeader = true;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    List<string> parts = line.Split(',').Select(p => p.Trim()).ToList();

                    if (isHeader)
                    {
                        isHeader = false;
                    }
                    else if (parts.Count == 2)
                    {
                        Regex regex = new Regex("^[0-9]+$");

                        string accountNumber = parts[0];

                        // Check account number is only numbers
                        if (!regex.IsMatch(accountNumber))
                        {
                            skippedTransactions++;
                        }
                        else
                        {
                            try
                            {
                                double amount = double.Parse(parts[1]);
                                bool isDebit = amount < 0;
                                amount = Math.Abs(amount);

                                if (isDebit)
                                {
                                    debits += amount;
                                }
                                else
                                {
                                    credits += amount;
                                }

                                if (!accounts.Contains(accountNumber))
                                {
                                    accounts.Add(accountNumber);
                                }
                            }
                            catch (Exception ex)
                            {
                                skippedTransactions++;
                            }
                        }
                    }
                    else
                    {
                        skippedTransactions++;
                    }
                }
            }

            outputResults(filename, credits, debits, accounts.Count, skippedTransactions);

            moveToProcessed(fullPath, filename);
        }

        private static void moveToProcessed(string filePath, string filename)
        {
            File.Move(filePath, workingDirectory + processed + @"\" + filename);
        }

        private static void outputResults(string filename,
            double credits,
            double debits,
            int accountSize,
            int skippedTransactions)
        {
            string strippedFilename = filename.Substring(0, filename.LastIndexOf("."));
            using (StreamWriter file = new StreamWriter(workingDirectory + reports + @"\" + strippedFilename + ".txt", true))
            {
                file.WriteLine("File Processed: " + filename);
                file.WriteLine("Total Accounts: " + accountSize);
                file.WriteLine("Total Credits : " + credits.ToString("C", CultureInfo.CurrentCulture));
                file.WriteLine("Total Debits  : " + debits.ToString("C", CultureInfo.CurrentCulture));
                file.WriteLine("Skipped Transactions: " + skippedTransactions);
            }
        }
    }
}
