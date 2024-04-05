using Bogus;
using Microsoft.AspNetCore.Mvc;
using Task5.Enums;
using Task5.Models;
using Task5.ViewModels;

namespace Task5.Controllers
{
    public class HomeController : Controller
    {
        private static Dictionary<int, List<PersonEntity>> _fakeUserData = new Dictionary<int, List<PersonEntity>>();
        private static int _currentPage = 1;
        private static int _errorPerRecord = 0;
        private static int _seed = 0;

        public ActionResult Index()
        {
            return View(new FakeUserDataViewModel
            {
                Regions = GetRegions(),
                ErrorPerRecord = _errorPerRecord,
                Seed = _seed,
                FakeUserData = GetFakeUserData()
            });
        }

        [HttpPost]
        public ActionResult Index(FakeUserDataViewModel model)
        {
            _errorPerRecord = model.ErrorPerRecord;
            _seed = model.Seed;
            _currentPage = 1;

            if (_fakeUserData.TryGetValue(_seed, out var previouslyGeneratedData))
            {
                // Use the previously generated data
                _fakeUserData[_seed] = previouslyGeneratedData;
            }
            else
            {
                // Generate new data and store it
                _fakeUserData[_seed] = GenerateFakeUserData(model.SelectedRegion, model.ErrorPerRecord, model.Seed);
            }

            return View(new FakeUserDataViewModel
            {
                Regions = GetRegions(),
                ErrorPerRecord = model.ErrorPerRecord,
                Seed = model.Seed,
                FakeUserData = GetFakeUserData()
            });
        }

        [HttpPost]
        public ActionResult LoadMoreData(int page)
        {
            if (_fakeUserData.TryGetValue(_seed, out var generatedData))
            {
                int skipRecords = (page - 1) * 10;
                var data = generatedData.Skip(skipRecords).Take(10).ToList();
                return PartialView("_FakeUserDataTableRows", data);
            }
            else
            {
                return new EmptyResult();
            }
        }
        public ActionResult ExportToCSV()
        {
            var csvData = string.Join(Environment.NewLine, _fakeUserData.SelectMany(kvp => kvp.Value.Select(p => p.ToCsvString())));
            return new ContentResult
            {
                ContentType = "text/csv",
                Content = csvData
            };
            
        }

        private IEnumerable<PersonEntity> GetFakeUserData()
        {
            if (_fakeUserData.TryGetValue(_seed, out var generatedData))
            {
                int skipRecords = (_currentPage - 1) * 20;
                return generatedData.Skip(skipRecords).Take(20);
            }
            else
            {
                return Enumerable.Empty<PersonEntity>();
            }
        }

        private static List<PersonEntity> GenerateFakeUserData(Regions region, int errorPerRecord, int seed)
        {
            var faker = new Faker<PersonEntity>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.MiddleName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.Address, f => f.Address.FullAddress())
                .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber());

            // Apply region-specific settings
            switch (region)
            {
                case Regions.USA:
                    faker.RuleFor(p => p.FirstName, f => new Faker("en").Name.FirstName())
                         .RuleFor(p => p.MiddleName, f => new Faker("en").Name.FirstName())
                         .RuleFor(p => p.LastName, f => new Faker("en").Name.LastName())
                         .RuleFor(p => p.PhoneNumber, f => new Faker("en").Phone.PhoneNumber())
                         .RuleFor(p => p.Address, f => new Faker("en").Address.StreetAddress());
                         
                    break;
                case Regions.France:
                    faker.RuleFor(p => p.FirstName, f => new Faker("fr").Name.FirstName())
                        .RuleFor(p => p.MiddleName, f => new Faker("fr").Name.FirstName())
                        .RuleFor(p => p.LastName, f => new Faker("fr").Name.LastName())
                        .RuleFor(p => p.PhoneNumber, f => new Faker("fr").Phone.PhoneNumber())
                        .RuleFor(p => p.Address, f => new Faker("fr").Address.StreetAddress());
                    break;
                case Regions.Georgia:
                    faker.RuleFor(p => p.FirstName, f => new Faker("ge").Name.FirstName())
                        .RuleFor(p => p.MiddleName, f => new Faker("ge").Name.FirstName())
                        .RuleFor(p => p.LastName, f => new Faker("ge").Name.LastName())
                        .RuleFor(p => p.PhoneNumber, f => new Faker("ge").Phone.PhoneNumber())
                        .RuleFor(p => p.Address, f => new Faker("ge").Address.StreetAddress());
                    break;
            }

            // Apply errors
            var fakeUserData = faker.Generate(1000);
            fakeUserData.ForEach(x => ApplyErrors(x, errorPerRecord, seed));

            return fakeUserData;
        }

        private static void ApplyErrors(PersonEntity person, int errorPerRecord, int seed)
        {
            var random = new Random(seed);
            for (int i = 0; i < errorPerRecord; i++)
            {
                // Apply random errors
                switch (random.Next(3))
                {
                    case 0:
                        person.FirstName = DeleteCharacterAtRandomPosition(person.FirstName);
                        break;
                    case 1:
                        person.FirstName = AddCharacterAtRandomPosition(person.FirstName);
                        break;
                    case 2:
                        person.FirstName = SwapNearCharacters(person.FirstName);
                        break;
                }
                switch (random.Next(3))
                {
                    case 0:
                        person.MiddleName = DeleteCharacterAtRandomPosition(person.MiddleName);
                        break;
                    case 1:
                        person.MiddleName = AddCharacterAtRandomPosition(person.MiddleName);
                        break;
                    case 2:
                        person.MiddleName = SwapNearCharacters(person.MiddleName);
                        break;
                }

                switch (random.Next(3))
                {
                    case 0:
                        person.LastName = DeleteCharacterAtRandomPosition(person.LastName);
                        break;
                    case 1:
                        person.LastName = AddCharacterAtRandomPosition(person.LastName);
                        break;
                    case 2:
                        person.LastName = SwapNearCharacters(person.LastName);
                        break;
                }

                switch (random.Next(3))
                {
                    case 0:
                        person.Address = DeleteCharacterAtRandomPosition(person.Address);
                        break;
                    case 1:
                        person.Address = AddCharacterAtRandomPosition(person.Address);
                        break;
                    case 2:
                        person.Address = SwapNearCharacters(person.Address);
                        break;
                }

                switch (random.Next(3))
                {
                    case 0:
                        person.PhoneNumber = DeleteCharacterAtRandomPosition(person.PhoneNumber);
                        break;
                    case 1:
                        person.PhoneNumber = AddCharacterAtRandomPosition(person.PhoneNumber);
                        break;
                    case 2:
                        person.PhoneNumber = SwapNearCharacters(person.PhoneNumber);
                        break;
                }
            }
        }

        private static string DeleteCharacterAtRandomPosition(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int randomIndex = new Random().Next(0, input.Length);
            return input.Substring(0, randomIndex) + input.Substring(randomIndex + 1);
        }

        private static string AddCharacterAtRandomPosition(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int randomIndex = new Random().Next(0, input.Length);
            var randomChar = "abcdefghijklmnopqrstuvwxyz"[new Random().Next(0, 26)];
            return input.Substring(0, randomIndex) + randomChar + input.Substring(randomIndex);
        }

        private static string SwapNearCharacters(string input)
        {
            if (input.Length <= 1)
                return input;

            int randomIndex = new Random().Next(0, input.Length - 1);
            return input.Substring(0, randomIndex) + input[randomIndex + 1] + input[randomIndex] + input.Substring(randomIndex + 2);
        }

        private static IEnumerable<Regions> GetRegions()
        {
            return Enum.GetValues(typeof(Regions)).Cast<Regions>();
        }
    }
}
