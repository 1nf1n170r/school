public abstract class Animal
{
    public string Name { get; set; }
    public abstract void MakeSound();
    public Animal(string name)
    {
        Name = name;
    }

}
