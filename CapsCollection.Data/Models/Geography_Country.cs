using System.Collections.Generic;

namespace CapsCollection.Data.Models
{
    public class Geography_Country
    {
        public int CountryID { get; set; }
        public int ContinentID { get; set; }
        public string EnglishCountryName { get; set; }
        public string EnglishCountryFullName { get; set; }
        public string NationalCountryName { get; set; }
        public string NationalCountryFullName { get; set; }
        public string Alpha2 { get; set; }
        public string Alpha3 { get; set; }
        public string ISO { get; set; }
        public string PreciseLocation { get; set; }

        public virtual Geography_Continent Continent { get; set; }

        HashSet<Geography_Region> _regions;
        public virtual ICollection<Geography_Region> Regions
        {
            get
            {
                if (_regions == null)
                    _regions = new HashSet<Geography_Region>();

                return _regions;
            }
            set
            {
                _regions = new HashSet<Geography_Region>(value);
            }
        }
    }
}
