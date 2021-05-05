using System;

namespace Products.Model.Broadband
{
    [Serializable]
    public class LineAvailability
    {
        public bool Fallout { get; set; }
        public bool InstallLine { get; set; }
        public bool BackOfficeFile { get; set; }
    }
}