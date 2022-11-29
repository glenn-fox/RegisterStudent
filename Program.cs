using MongoDataAccess.DataAccess;
using MongoDB.Driver;
using MongoDataAccess.Models;

RegristrationDataAccess db = new RegristrationDataAccess();

var courses = await db.GetAllCourses();
string yesOrNo;
//StudentModel foundStudent;

do
{
    // main menu
    writePrompt();
    int choice = Convert.ToInt32(Console.ReadLine());
    //StudentModel foundStudent;


    switch (choice)
    {
        case 1:
            // register student
            // get student ID
            var student = new StudentModel();

            Console.WriteLine("Please input a student Id");
            string studentId = getString();

            StudentModel foundStudent = db.LoadStudentByStudentId(studentId);

            // check if student is in DB
            if (foundStudent.Id is not null)
            {
                student = foundStudent;
                Console.WriteLine($"{student.FullName} was found and loaded");
            }
            else
            {
                Console.WriteLine($"No student found with Id of {studentId}, Please create student");

                student = foundStudent;

                Console.WriteLine("Please input first name: ");
                student.FirstName = getString();

                Console.WriteLine("Plese input last name");
                student.LastName = getString();

                // add student to database
                await db.CreateStudent(student);
            }


            // check if student can register for a class
            while (student.RegisteredCourses.Count() < 3)
            {
                var chosenCourse = db.ListCourse(courses);

                if (chosenCourse != null && validateChoice(student, chosenCourse))
                {
                    student.RegisteredCourses.Add(chosenCourse);
                }

                Console.WriteLine("Add another Course? Y/n");

                string userChoice = getString();

                if (userChoice.ToLower() == "n")
                {
                    break;
                }
            }
            await db.UpdateStudent(student);
            Console.WriteLine($"{student.FullName} was written to database");
            break;

        case 2:
            // view all students
            var students = await db.GetAllStudents();

            foreach (var person in students)
            {
                Console.WriteLine($"{person.StudentId}: {person.FullName}");
            }
            break;

        case 3:
            // delete student
            Console.WriteLine("Please enter the Student Id of the student to delete");
            foundStudent = new StudentModel();

            studentId = getString();

            foundStudent = db.LoadStudentByStudentId(studentId);
            await db.DeleteStudent(foundStudent);

            break;
    }
    Console.WriteLine("Continue program? Y/n");
    yesOrNo = getString().ToUpper();
} while (yesOrNo != "N");

// makes sure student is not already registered for a course
bool validateChoice(StudentModel student, CoursesModel course)
{
    foreach(var registeredCourse in student.RegisteredCourses)
    {
        if (registeredCourse.Equals(course))
        {
            Console.WriteLine("Course already registered");
            return false;
        }
    }
    return true;
}

// function that writes the prompt
void writePrompt()
{
    Console.Clear();
    Console.WriteLine("Choose an option");
    Console.WriteLine("[1] Register Student");
    Console.WriteLine("[2] View All Students");
    Console.WriteLine("[3] Remove Student");
    Console.Write("Enter choice: ");
}

// function to get a string from the user and outputs an error if blank
string getString()
{
    while (true)
    {
        string input = Console.ReadLine();

        if (input is null)
        {
            Console.Write("Invalid input, try again");
        }
        else
        {
            return input;
        }
    }
}