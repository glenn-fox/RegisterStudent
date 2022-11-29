using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongoDataAccess.Models;

public class StudentModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string? StudentId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public List<CoursesModel> RegisteredCourses { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    public StudentModel()
    {
        RegisteredCourses = new List<CoursesModel>();
    }

    public StudentModel(string studentId)
    {
        StudentId = studentId;
        RegisteredCourses = new List<CoursesModel>();
    }

    public StudentModel CreateStudent(string studentId)
    {
        Console.WriteLine("Please Enter Student Id");
        StudentId = studentId;

        Console.WriteLine("Please Enter First Name");
        FirstName = Console.ReadLine();

        Console.WriteLine("Please Enter Last Name");
        LastName = Console.ReadLine();

        return this;
    }
}
