﻿using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace program;

public class InvalidNameException : Exception
{
    public InvalidNameException(string msg, Exception inner) : base(msg, inner) { }
    public InvalidNameException(string msg) : base(msg) { }
}
public class InvalidAgeException : Exception
{
    public InvalidAgeException(string msg, Exception inner) : base(msg, inner) { }
    public InvalidAgeException(string msg) : base(msg) { }
}
public class InvalidEmailException : Exception
{
    public InvalidEmailException(string msg, Exception inner) : base(msg, inner) { }
    public InvalidEmailException(string msg) : base(msg) { }
}
public struct User{
    public string Name {get; set;}
    public uint Age {get; set;}
    public string Email {get; set;}
};
public class Users : IDisposable
{
    private readonly List<User> m_users;
    private readonly string m_path;
    public bool IsValid => File.Exists(m_path);
    public Users(string path)
    {
        m_path = path;
        if (!IsValid)
            File.Create(m_path);
        m_users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(m_path)) ?? [];
    }
    public void Dispose()
    {
         var json = JsonConvert.SerializeObject(m_users, Formatting.Indented);
        if (!IsValid)
            File.Create(m_path);
        File.WriteAllText(m_path, json);
    }
    
    public void Add(User user)
    {
        this.m_users.Add(user);
    }
}
internal class Program{
    static void Main(){
        using var users = new Users(@"P:\School\H1\2024_11_11\FileHandler\assets\users.json");
        if (!users.IsValid)
            Console.WriteLine("Could not find users.json >> Creating user.json");
        const string BYPASS = "niels olesen";
        do
        {
            try
            {
                Console.Write("Create new user: ");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                    break;
                Console.WriteLine();
                var name = Lib.Input.AskCond<string>("Name: ", (i) => { return new Regex(@"^[A-Za-z\s]+$").IsMatch(i); }, false, (e) =>
                {
                    return e switch
                    {
                        ArgumentException => new InvalidNameException(e.Message),
                        _ => null,
                    };
                });
                var age = Lib.Input.AskCond<uint>("Age: ", (i) => { return new Regex(@"^(1[89]|[2-4][0-9]|50)$").IsMatch(i) || name.ToLower() == BYPASS; }, false, (e) =>
                {
                    return e switch
                    {
                        ArgumentException when name.ToLower() != BYPASS => new InvalidAgeException(e.Message),
                        _ => null,
                    };
                });
                var email = Lib.Input.AskCond<string>("Email: ", (i) => { return new Regex(@"@.*\.").IsMatch(i); }, false, (e) =>
                {
                    return e switch
                    {
                        ArgumentException => new InvalidEmailException(e.Message),
                        _ => null,
                    };
                });
                users.Add(new User
                {
                    Name = name,
                    Age = age,
                    Email = email,
                });
            }
            catch (InvalidNameException)
            {
                Console.WriteLine("Invalid name");
            }
            catch (InvalidAgeException)
            {
                Console.WriteLine("Invalid age");
            }
            catch (InvalidEmailException)
            {
                Console.WriteLine("Invalid email");
            }
            finally { Console.WriteLine("Another one!"); };

        } while (true);
        if (!users.IsValid)
            Console.WriteLine("Could not find users.json >> Creating user.json");
    }
}