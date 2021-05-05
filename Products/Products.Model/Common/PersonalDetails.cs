using System;

namespace Products.Model.Common
{
    [Serializable]
    public class PersonalDetails
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FormattedName => $"{Title} {FirstName} {LastName}";
        public string DateOfBirth { get; set; }
    }
}