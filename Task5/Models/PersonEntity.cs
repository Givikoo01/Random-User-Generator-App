namespace Task5.Models
{
    public class PersonEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public string ToCsvString()
        {
            return $"{Id},{FirstName},{MiddleName},{LastName},{Address},{PhoneNumber}";
        }

    }
}
