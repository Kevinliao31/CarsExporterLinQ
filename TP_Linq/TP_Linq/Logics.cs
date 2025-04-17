using Newtonsoft.Json;
using TP_Linq.Models;

namespace TP_Linq
{
    public class Logics
    {
        public static string? Search()
        {
            Console.WriteLine("Saisissez votre recherche : ");
            string? search = Console.ReadLine();

            if (search != null)
            {
                return search;
            }
            return null;
        }

        #region Méthodes GET
        public static List<Car>? GetAllCars()
        {
            string path = Path.Combine("Json", "Cars.json");

            string json = File.ReadAllText(path);

            List<Car>? cars = JsonConvert.DeserializeObject<List<Car>>(json);

            return cars;
        }

        public static List<Supplier>? GetAllSuppliers()
        {
            string path = Path.Combine("Json", "Suppliers.json");

            string json = File.ReadAllText(path);

            List<Supplier>? suppliers = JsonConvert.DeserializeObject<List<Supplier>>(json);

            return suppliers;
        }
        #endregion

        #region Méthodes DISPLAY
        public static void DisplayCarsBySearch()
        {
            List<Car>? cars = GetAllCars();
            string? search = Search();

            if (cars != null && !string.IsNullOrEmpty(search))
            {
                IEnumerable<Car> filteredCars = cars.Where(car =>
                    car.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    car.Year.ToString().Contains(search) ||
                    car.Price.ToString().Contains(search) ||
                    car.SupplierId.ToString().Contains(search));

                if (!filteredCars.Any())
                {
                    Console.WriteLine("Aucun résultat");
                }
                else
                {
                    foreach (Car car in filteredCars)
                    {
                        Console.WriteLine($"ID: {car.Id}, Model: {car.Model}, Year: {car.Year}, Price: {car.Price}, SupplierID: {car.SupplierId}");
                    }

                    UserExportChoice(filteredCars);
                }
            }
        }
        #endregion

        #region Méthodes SORT
        public static void SortCarsBySupplier()
        {
            List<Car>? cars = GetAllCars();
            List<Supplier>? suppliers = GetAllSuppliers();
            string? search = Search();

            if (suppliers != null && cars != null && !string.IsNullOrEmpty(search))
            {
                IEnumerable<Car> filteredCars = cars.Where(car =>
                    car.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    car.Year.ToString().Contains(search) ||
                    car.Price.ToString().Contains(search) ||
                    car.SupplierId.ToString().Contains(search));

                IEnumerable<Car> sortedCars = filteredCars
                    .Join(suppliers, car => car.SupplierId, supplier => supplier.Id, (car, supplier) => new { car, supplier })
                    .OrderBy(x => x.supplier.Id)
                    .Select(x => x.car);

                if (!sortedCars.Any())
                {
                    Console.WriteLine("Aucun résultat");
                }
                else
                {
                    foreach (Car car in sortedCars)
                    {
                        Console.WriteLine($"SupplierID: {car.SupplierId} ID: {car.Id}, Model: {car.Model}, Year: {car.Year}, Price: {car.Price}");
                    }

                    UserExportChoice(filteredCars);
                }
            }
        }

        public static void SortCarsByPrice()
        {
            List<Car>? cars = GetAllCars();
            List<Supplier>? suppliers = GetAllSuppliers();
            string? search = Search();

            if (suppliers != null && cars != null && !string.IsNullOrEmpty(search))
            {
                IEnumerable<Car> filteredCars = cars.Where(car =>
                    car.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    car.Year.ToString().Contains(search) ||
                    car.Price.ToString().Contains(search) ||
                    car.SupplierId.ToString().Contains(search));

                IEnumerable<Car> sortedCars = filteredCars
                    .Join(suppliers, car => car.SupplierId, supplier => supplier.Id, (car, supplier) => new { car, supplier })
                    .OrderBy(x => x.car.Price)
                    .Select(x => x.car);

                if (!sortedCars.Any())
                {
                    Console.WriteLine("Aucun résultat");
                }
                else
                {
                    foreach (Car car in sortedCars)
                    {
                        Console.WriteLine($"SupplierID: {car.SupplierId} ID: {car.Id}, Model: {car.Model}, Year: {car.Year}, Price: {car.Price}");
                    }

                    UserExportChoice(filteredCars);
                }
            }
        }
        #endregion

        #region Méthodes GROUP
        public static void GroupCarsBySupplier()
        {
            List<Car>? cars = GetAllCars();
            List<Supplier>? suppliers = GetAllSuppliers();
            string? search = Search();

            if (suppliers != null && cars != null && !string.IsNullOrEmpty(search))
            {
                IEnumerable<Car> filteredCars = cars.Where(car =>
                    car.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    car.Year.ToString().Contains(search) ||
                    car.Price.ToString().Contains(search) ||
                    car.SupplierId.ToString().Contains(search));

                var groupedCars = filteredCars
                    .Join(suppliers, car => car.SupplierId, supplier => supplier.Id, (car, supplier) => new { car, supplier })
                    .GroupBy(x => x.supplier.Name)
                    .Select(g => new { SupplierName = g.Key, Cars = g.Select(x => x.car) });

                if (!groupedCars.Any())
                {
                    Console.WriteLine("Aucun résultat");
                }
                else
                {
                    foreach (var group in groupedCars)
                    {
                        Console.WriteLine($"Supplier: {group.SupplierName}");
                        foreach (var car in group.Cars)
                        {
                            Console.WriteLine($"  ID: {car.Id}, Model: {car.Model}, Year: {car.Year}, Price: {car.Price}");
                        }
                    }

                    UserExportChoice(filteredCars);
                }
            }
        }
        #endregion

        public static void UserExportChoice(IEnumerable<Car> filteredCars)
        {
            Console.WriteLine("Souhaitez-vous exporter les résultats en CSV ? (y/n)");
            string? choix = Console.ReadLine();

            if (choix?.Trim().ToLower() == "y")
            {
                Console.WriteLine("Quelles propriétés voulez-vous exporter ? (laisser vide pour tout exporter)");
                Console.WriteLine("Options possibles : id,model,year,price,supplierid");
                string? fieldsInput = Console.ReadLine();

                List<string> selectedFields;

                if (string.IsNullOrWhiteSpace(fieldsInput))
                {
                    // Tout exporter par défaut
                    selectedFields = ["id", "model", "year", "price", "supplierid"];
                }
                else
                {
                    selectedFields = fieldsInput
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(f => f.Trim().ToLower())
                        .ToList();
                }

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string fileName = $"resultats_{timestamp}.csv";

                ExportToCsv(filteredCars, fileName, selectedFields);
            }
        }

        public static void ExportToCsv(IEnumerable<Car> cars, string fileName, List<string> selectedFields)
        {
            string directoryPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "CSV");
            Directory.CreateDirectory(directoryPath);

            string fullPath = Path.Combine(directoryPath, fileName);

            using (StreamWriter writer = new(fullPath))
            {
                // En-tête du fichier CSV
                writer.WriteLine(string.Join(",", selectedFields));

                foreach (Car car in cars)
                {
                    List<string> values = [];

                    foreach (string field in selectedFields)
                    {
                        switch (field.ToLower())
                        {
                            case "id":
                                values.Add(car.Id.ToString());
                                break;
                            case "model":
                                values.Add(car.Model);
                                break;
                            case "year":
                                values.Add(car.Year.ToString());
                                break;
                            case "price":
                                values.Add(car.Price.ToString());
                                break;
                            case "supplierid":
                                values.Add(car.SupplierId.ToString());
                                break;
                            default:
                                values.Add("");
                                break;
                        }
                    }

                    writer.WriteLine(string.Join(",", values));
                }
            }

            Console.WriteLine($"Résultats exportés dans {fullPath}");
        }
    }
}
