using MongoDataAccess.Models;
using MongoDB.Driver;
using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MongoDataAccess.DataAccess;

public class RegristrationDataAccess
{
    private const string ConnectionString = "mongodb://127.0.0.1:27017";
    private const string DatabaseName = "registrationdb";
    private const string StudentCollection = "students";
    private const string CoursesCollection = "courses";

    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public StudentModel LoadStudentByStudentId(string studentId)
    {
        var studentCollection = ConnectToMongo<StudentModel>(StudentCollection);

        var filter = Builders<StudentModel>.Filter.Eq("StudentId", studentId);

        StudentModel result;

        try
        {
            result = studentCollection.Find(filter).First();
        }
        catch(System.InvalidOperationException)
        {
            return new StudentModel(studentId);
        }

        return result;
    }

    public CoursesModel SearchCourses(string search)
    {
        var coursesCollection = ConnectToMongo<CoursesModel>(CoursesCollection);
        var filter = Builders<CoursesModel>.Filter.Eq("Title", search);

        CoursesModel result;

        try
        {
            result = coursesCollection.Find(filter).First();
        }
        catch (System.InvalidOperationException)
        {
            return null;
        }

        return result;
    }

    public async Task<List<CoursesModel>> GetAllCourses()
    {
        var coursesCollection = ConnectToMongo<CoursesModel>(CoursesCollection);
        var results = await coursesCollection.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<List<StudentModel>> GetAllStudents()
    {
        var studentCollection = ConnectToMongo<StudentModel>(StudentCollection);
        var results = await studentCollection.FindAsync(_ => true);
        return results.ToList();
    }


    public Task CreateStudent(StudentModel student)
    {
        var studentCollection = ConnectToMongo<StudentModel>(StudentCollection);
        return studentCollection.InsertOneAsync(student);
    }

    public Task CreateCourse(CoursesModel course)
    {
        var coursesCollection = ConnectToMongo<CoursesModel>(CoursesCollection);
        return coursesCollection.InsertOneAsync(course);
    }

    public Task UpdateStudent(StudentModel student)
    {
        var studentCollection = ConnectToMongo<StudentModel>(StudentCollection);
        var filter = Builders<StudentModel>.Filter.Eq("Id", student.Id);
        return studentCollection.ReplaceOneAsync(filter, student, new ReplaceOptions { IsUpsert = true });
    }

    public Task DeleteStudent(StudentModel student)
    {
        var studentCollection = ConnectToMongo<StudentModel>(StudentCollection);
        return studentCollection.DeleteOneAsync(c => c.Id == student.Id);
    }

    public CoursesModel ListCourse(List<CoursesModel> courses)
    {
        int i = 0;
        int step = 1;
        int input;
        int end = step * 10;

        while (true)
        {
            Console.WriteLine("Please select a course: ");
            while (i < end && i < courses.Count())
            {
                Console.WriteLine($"{i + 1}: {courses[i].Course}: {courses[i].Title}");
                i++;
            }
            Console.WriteLine("Enter choice or press 'Enter' for more courses or 0 to exit");
            try
            {
                input = Convert.ToInt32(Console.ReadLine());
                if (input == 0)
                {
                    return null;
                }
                else
                {
                    return courses[input - 1];
                }
               
            }
            catch(System.FormatException)
            {
                if (i < courses.Count())
                {
                    step++;
                    end = step * 10;
                }
                else
                {
                    step = 1;
                    i = 0;
                    end = step * 10;
                }
                
            }
            
        }
    }

}
