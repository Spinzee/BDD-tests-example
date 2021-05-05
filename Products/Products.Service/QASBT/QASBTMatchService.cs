namespace Products.Service.QASBT
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using Model.Common;

    public sealed class QASBTMatchService
    {
        public static Tuple<int, int> ProcessQASBTMatch(BTAddress openReachAddress, QasAddress quickAddress)
        {
            Tuple<int, int> fieldMatchResults;
            var localityMatchResults = new Tuple<int, int>(0, 0);

            string compareAddress = quickAddress.PicklistEntry.ToUpper();
            string number = GetAddressValue(openReachAddress.ThoroughfareNumber);
            string name = GetAddressValue(openReachAddress.ThoroughfareName);
            string town = GetAddressValue(openReachAddress.PostTown);
            string premise = GetAddressValue(openReachAddress.PremiseName);
            string subPremises = GetAddressValue(openReachAddress.SubPremises);
            string locality = GetAddressValue(openReachAddress.Locality);

            if (!compareAddress.EndsWith(" "))
            {
                compareAddress = $"{compareAddress} ";
            }

            string numberAddressToCompare = $"{number}{name}{town}{premise}{subPremises}{locality}";

            Tuple<int, int> numberMatchResults = GetMatchResultsAllNumbers(numberAddressToCompare, compareAddress);

            int bitMask = SetBitMask(number, name, premise, subPremises, locality);

            switch (bitMask)
            {
                case 3:
                    fieldMatchResults = GetMatchResults3Fields(number, name, town, compareAddress);
                    break;
                case 6:
                    fieldMatchResults = GetMatchResults3Fields(town, name, premise, compareAddress);
                    break;
                case 7:
                    fieldMatchResults = GetMatchResults4Fields(number, name, town, premise, compareAddress);
                    break;
                case 11:
                    fieldMatchResults = GetMatchResults4Fields(number, name, town, subPremises, compareAddress);
                    break;
                case 14:
                    fieldMatchResults = GetMatchResults4Fields(name, town, premise, subPremises, compareAddress);
                    break;
                case 15:
                    fieldMatchResults = GetMatchResults5Fields(number, name, town, premise, subPremises, compareAddress);
                    break;
                case 19:
                    fieldMatchResults = GetMatchResults3Fields(number, name, town, compareAddress);
                    localityMatchResults = GetMatchResultsLocality(locality, compareAddress);
                    break;
                case 20:
                    fieldMatchResults = GetMatchResults2Fields(town, premise, compareAddress);
                    localityMatchResults = GetMatchResultsLocality(locality, compareAddress);
                    break;
                case 22:
                    fieldMatchResults = GetMatchResults3Fields(name, town, premise, compareAddress);
                    localityMatchResults = GetMatchResultsLocality(locality, compareAddress);
                    break;
                case 23:
                    fieldMatchResults = GetMatchResults4Fields(number, name, town, premise, compareAddress);
                    localityMatchResults = GetMatchResultsLocality(locality, compareAddress);
                    break;
                case 28:
                    fieldMatchResults = GetMatchResults3Fields(town, premise, subPremises, compareAddress);
                    localityMatchResults = GetMatchResultsLocality(locality, compareAddress);
                    break;
                case 30:
                    fieldMatchResults = GetMatchResults4Fields(name, town, premise, subPremises, compareAddress);
                    localityMatchResults = GetMatchResultsLocality(locality, compareAddress);
                    break;
                case 31:
                    fieldMatchResults = GetMatchResults5Fields(number, name, town, premise, subPremises, compareAddress);
                    localityMatchResults = GetMatchResultsLocality(locality, compareAddress);
                    break;
                default:
                    fieldMatchResults = new Tuple<int, int>(0, 0);
                    break;
            }

            return new Tuple<int, int>(numberMatchResults.Item1 + fieldMatchResults.Item1 + localityMatchResults.Item1, numberMatchResults.Item2 + fieldMatchResults.Item2 + localityMatchResults.Item2);
        }

        private static Tuple<int, int> GetMatchResults2Fields(string field1, string field2, string compareAddress)
        {
            int totalMatchTests = 0;
            int matches = 0;

            Tuple<int, int> field1Results = MatchUpper(field1, compareAddress);
            matches += field1Results.Item1;
            totalMatchTests += field1Results.Item2;

            Tuple<int, int> field2Results = MatchUpper(field2, compareAddress);
            matches += field2Results.Item1;
            totalMatchTests += field2Results.Item2;

            return new Tuple<int, int>(matches, totalMatchTests);
        }

        private static Tuple<int, int> GetMatchResults3Fields(string field1, string field2, string field3, string compareAddress)
        {
            int totalMatchTests = 0;
            int matches = 0;

            Tuple<int, int> twoFieldResults = GetMatchResults2Fields(field1, field2, compareAddress);
            matches += twoFieldResults.Item1;
            totalMatchTests += twoFieldResults.Item2;

            Tuple<int, int> field3Results = MatchUpper(field3, compareAddress);
            matches += field3Results.Item1;
            totalMatchTests += field3Results.Item2;

            return new Tuple<int, int>(matches, totalMatchTests);
        }

        private static Tuple<int, int> GetMatchResults4Fields(string field1, string field2, string field3, string field4, string compareAddress)
        {
            int totalMatchTests = 0;
            int matches = 0;

            Tuple<int, int> twoFieldResults = GetMatchResults3Fields(field1, field2, field3, compareAddress);
            matches += twoFieldResults.Item1;
            totalMatchTests += twoFieldResults.Item2;

            Tuple<int, int> field3Results = MatchUpper(field4, compareAddress);
            matches += field3Results.Item1;
            totalMatchTests += field3Results.Item2;

            return new Tuple<int, int>(matches, totalMatchTests);
        }

        private static Tuple<int, int> GetMatchResults5Fields(string field1, string field2, string field3, string field4, string field5, string compareAddress)
        {
            int totalMatchTests = 0;
            int matches = 0;

            Tuple<int, int> fourFieldResults = GetMatchResults4Fields(field1, field2, field3, field4, compareAddress);
            matches += fourFieldResults.Item1;
            totalMatchTests += fourFieldResults.Item2;

            Tuple<int, int> field5Results = MatchUpper(field5, compareAddress);
            matches += field5Results.Item1;
            totalMatchTests += field5Results.Item2;

            return new Tuple<int, int>(matches, totalMatchTests);
        }

        private static Tuple<int, int> GetMatchResultsLocality(string locality, string compareAddress)
        {
            int totalMatchTests = 0;
            int matches = 0;

            Tuple<int, int> localityResults = MatchUpper(locality, compareAddress);
            if (localityResults.Item1 > 0)
            {
                matches += localityResults.Item1;
                totalMatchTests += localityResults.Item2;
            }

            return new Tuple<int, int>(matches, totalMatchTests);
        }

        private static int SetBitMask(string number, string name, string premise, string subPremises, string locality)
        {
            int retVal = 0;

            if (!string.IsNullOrEmpty(number))
            {
                retVal++;
            }

            if (!string.IsNullOrEmpty(name))
            {
                retVal += 2;
            }

            if (!string.IsNullOrEmpty(premise))
            {
                retVal += 4;
            }

            if (!string.IsNullOrEmpty(subPremises))
            {
                retVal += 8;
            }

            if (!string.IsNullOrEmpty(locality))
            {
                retVal += 16;
            }

            return retVal;
        }

        private static Tuple<int, int> MatchUpper(string field, string toCompare)
        {
            int totalMatchTests = 0;
            int matches = 0;

            totalMatchTests++;
            if (MatchUpperDelimiter(field, string.Empty, toCompare))
            {
                matches++;
            }

            totalMatchTests++;
            if (MatchUpperDelimiter(field, ", ", toCompare))
            {
                matches++;
            }
            else
            {
                if (MatchUpperDelimiter(field, ",", toCompare))
                {
                    matches++;
                }
                else
                {
                    if (MatchUpperDelimiter(field, " ", toCompare))
                    {
                        matches++;
                    }
                }
            }

            return new Tuple<int, int>(matches, totalMatchTests);
        }

        private static bool MatchUpperDelimiter(string field, string delimiter, string toCompare)
        {
            return toCompare.ToUpper().Contains($"{field.ToUpper()}{delimiter}");
        }

        private static Tuple<int, int> GetMatchResultsAllNumbers(string address1, string address2)
        {
            int totalMatchTests = 0;
            int matches = 0;

            int address1Number = GetNumberFromString(address1);

            if (address1Number > 0)
            {
                totalMatchTests++;
                int address2Number = GetNumberFromString(address2);
                if (address1Number == address2Number)
                {
                    matches++;
                }
            }

            return new Tuple<int, int>(matches, totalMatchTests);
        }

        private static int GetNumberFromString(string inputValue)
        {
            int retVal = 0;
            const string splitPattern = @"[^\d]";

            if (!string.IsNullOrEmpty(inputValue))
            {
                string[] results = Regex.Split(inputValue, splitPattern);

                var inputValueBuilder = new StringBuilder();

                if (results.Length > 0)
                {
                    foreach (string s in results)
                    {
                        inputValueBuilder.Append(s);
                    }

                    if (inputValueBuilder.Length > 0)
                    {
                        for (int i = 0; i < inputValueBuilder.Length; i++)
                        {
                            string digit = inputValueBuilder.ToString().Substring(i, 1);
                            if (digit == "0")
                            {
                                retVal += 10;
                            }
                            else
                            {
                                retVal += Convert.ToInt32(digit);
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        private static string GetAddressValue(string addressField)
        {
            return string.IsNullOrEmpty(addressField) ? string.Empty : addressField;
        }
    }
}
