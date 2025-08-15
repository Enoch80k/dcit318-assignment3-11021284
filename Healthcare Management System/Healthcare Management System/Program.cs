using System;
using System.Collections.Generic;
using System.Linq;

// a. Generic Repository class
public class Repository<T> where T : class
{
    private List<T> items = new List<T>();


    public void Add(T item)
    {
        items.Add(item);
    }

    
    public List<T> GetAll()
    {
        return new List<T>(items);

   
    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

   
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// b. Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"Patient(Id={Id}, Name={Name}, Age={Age}, Gender={Gender})";
    }
}

// c. Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription(Id={Id}, PatientId={PatientId}, Medication={MedicationName}, Date={DateIssued:d})";
    }
}

// g. HealthSystemApp class: manages data and interactions
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();

    
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    
    public void SeedData()
    {
        
        _patientRepo.Add(new Patient(1, "Alice Johnson", 30, "Female"));
        _patientRepo.Add(new Patient(2, "Bob Smith", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Charlie Davis", 28, "Male"));

        
        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-12)));
        _prescriptionRepo.Add(new Prescription(4, 2, "Lisinopril", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(5, 3, "Metformin", DateTime.Now.AddDays(-1)));
    }

    
    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();

        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    
    public void PrintAllPatients()
    {
        Console.WriteLine("All Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
        Console.WriteLine();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"No patient found with Id {patientId}");
            return;
        }

        Console.WriteLine($"Prescriptions for {patient.Name} (Id: {patient.Id}):");

        if (_prescriptionMap.TryGetValue(patientId, out var prescriptions) && prescriptions.Any())
        {
            foreach (var p in prescriptions)
            {
                Console.WriteLine(p);
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
        Console.WriteLine();
    }
}


class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();

        
        app.SeedData();

       
        app.BuildPrescriptionMap();

        
        app.PrintAllPatients();

        
        int patientIdToQuery = 2; 
        app.PrintPrescriptionsForPatient(patientIdToQuery);

        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
