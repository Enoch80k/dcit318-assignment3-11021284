using System;
using System.Collections.Generic;
using System.IO;

// a. Student Class
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";  // Below 50
    }
}

// b. Custom Exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class InvalidIdFormatException : Exception
{
    public InvalidIdFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// d. StudentResultProcessor Class
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath, string errorLogPath)
    {
        List<Student> students = new List<Student>();
        List<string> errorLines = new List<string>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                try
                {
                    string[] parts = line.Split(',');

                    if (parts.Length < 3)
                        throw new MissingFieldException($"Line {lineNumber}: Missing fields. Expected 3, got {parts.Length}.");

                    string idStr = parts[0].Trim();
                    string fullName = parts[1].Trim();
                    string scoreStr = parts[2].Trim();

                    if (string.IsNullOrEmpty(idStr) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(scoreStr))
                        throw new MissingFieldException($"Line {lineNumber}: One or more fields are empty.");

                    if (!int.TryParse(idStr, out int id))
                        throw new InvalidIdFormatException($"Line {lineNumber}: Invalid ID format '{idStr}'.");

                    if (!int.TryParse(scoreStr, out int score))
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format '{scoreStr}'.");

                    if (score < 0 || score > 100)
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Score {score} is out of valid range (0-100).");

                    students.Add(new Student(id, fullName, score));
                }
                catch (Exception ex)
                {
                   
                    errorLines.Add(ex.Message);
                }
            }
        }

        
        if (errorLines.Count > 0)
        {
            File.WriteAllLines(errorLogPath, errorLines);
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                string line = $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}";
                writer.WriteLine(line);
            }
        }
    }
}

// Main class implementation
class Program
{
    static void Main()
    {
        var processor = new StudentResultProcessor();

        string inputFilePath = "students.txt";           
        string outputFilePath = "summary_report.txt";    
        string errorLogPath = "error_log.txt";           

        try
        {
            var students = processor.ReadStudentsFromFile(inputFilePath, errorLogPath);
            processor.WriteReportToFile(students, outputFilePath);

            Console.WriteLine($"Report successfully generated at '{outputFilePath}'.");
            if (File.Exists(errorLogPath))
                Console.WriteLine($"Some errors occurred. See '{errorLogPath}' for details.");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: Input file '{inputFilePath}' not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}
