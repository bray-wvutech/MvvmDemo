namespace MvvmDemo.Models;

// Model: one database row. Id is the database key; Name and Age are the data.
public class Person : ObservableObject
{
    public int Id { get; set; }
    private string name = "";
    private int age;

    public string Name
    {
        get => name;
        set
        {
            if (name == value) return;
            name = value;
            OnPropertyChanged();
        }
    }

    public int Age
    {
        get => age;
        set
        {
            if (age == value) return;
            age = value;
            OnPropertyChanged();
        }
    }
}


