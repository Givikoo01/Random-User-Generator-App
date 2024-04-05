using Task5.Enums;
using Task5.Models;

namespace Task5.ViewModels
{
    public class FakeUserDataViewModel
    {
        public IEnumerable<Regions> Regions { get; set; }
        public Regions SelectedRegion { get; set; }
        public int ErrorPerRecord { get; set; }
        public int Seed { get; set; }
        public IEnumerable<PersonEntity> FakeUserData { get; set; }

        public string ToCsvString(PersonEntity person)
        {
            return $"{person.Id},{person.FirstName},{person.MiddleName},{person.LastName},{person.Address},{person.PhoneNumber}";
        }
    }
}
