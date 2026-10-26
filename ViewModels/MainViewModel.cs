using MvvmDemo.Data;
using MvvmDemo.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace MvvmDemo.ViewModels;

// ViewModel: exposes properties and commands, with no references to UI controls.
public class MainViewModel : ObservableObject, IDisposable
{
    private readonly PeopleContext database;
    private ObservableCollection<Person> people = new();
    private Person? selectedPerson;
    private string searchText = "";
    private string status = "Edit a person, then click Save Changes.";
    private bool sortByAge;
    private bool isReady;

    public ObservableCollection<Person> People
    {
        get => people;
        private set { people = value; OnPropertyChanged(); }
    }

    public Person? SelectedPerson
    {
        get => selectedPerson;
        set
        {
            if (selectedPerson == value) return;
            selectedPerson = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSelection));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public bool HasSelection => SelectedPerson is not null;

    public string SearchText
    {
        get => searchText;
        set
        {
            if (searchText == value) return;
            searchText = value;
            OnPropertyChanged();
            RefreshPeople();
        }
    }

    public string Status
    {
        get => status;
        private set { status = value; OnPropertyChanged(); }
    }

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SortByNameCommand { get; }
    public ICommand SortByAgeCommand { get; }

    public MainViewModel() : this(new PeopleContext()) { }

    // Accepting a context also lets us test with a temporary database.
    public MainViewModel(PeopleContext database)
    {
        this.database = database;
        AddCommand = new RelayCommand(AddPerson, () => isReady);
        DeleteCommand = new RelayCommand(DeletePerson, () => isReady && HasSelection);
        SaveCommand = new RelayCommand(Save, () => isReady);
        SortByNameCommand = new RelayCommand(() => SetSort(false), () => isReady);
        SortByAgeCommand = new RelayCommand(() => SetSort(true), () => isReady);

        try
        {
            if (database.Database.EnsureCreated())
            {
                database.People.AddRange(
                    new Person { Name = "Alex Johnson", Age = 20 },
                    new Person { Name = "Jordan Smith", Age = 22 },
                    new Person { Name = "Taylor Brown", Age = 19 },
                    new Person { Name = "Morgan Davis", Age = 21 });
                database.SaveChanges();
            }
            database.People.Load();
            isReady = true;
            RefreshPeople();
            SelectedPerson = People.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Status = $"Could not load the database: {ex.Message}";
        }
    }

    private void RefreshPeople()
    {
        var selection = SelectedPerson;
        string search = SearchText.Trim();

        // LINQ filters and sorts the same EF-tracked objects, including unsaved edits.
        var matches = database.People.Local
            .Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        var results = sortByAge
            ? matches.OrderBy(p => p.Age).ToList()
            : matches.OrderBy(p => p.Name).ToList();

        People = new ObservableCollection<Person>(results);
        SelectedPerson = selection is not null && People.Contains(selection) ? selection : null;
    }

    private void AddPerson()
    {
        SearchText = "";
        var person = new Person { Name = "New Person", Age = 18 };
        database.People.Add(person);
        RefreshPeople();
        SelectedPerson = person;
        Status = "Person added. Click Save Changes to keep your changes.";
    }

    private void DeletePerson()
    {
        if (SelectedPerson is null) return;
        database.People.Remove(SelectedPerson);
        RefreshPeople();
        SelectedPerson = People.FirstOrDefault();
        Status = "Person deleted. Click Save Changes to keep your changes.";
    }

    private void SetSort(bool byAge)
    {
        sortByAge = byAge;
        RefreshPeople();
        Status = byAge ? "Sorted by age (ascending)." : "Sorted by name (ascending).";
    }

    private void Save()
    {
        try
        {
            database.SaveChanges();
            Status = "Changes saved to SQLite.";
        }
        catch (Exception ex)
        {
            Status = $"Could not save changes: {ex.Message}";
        }
    }

    public void Dispose() => database.Dispose();
}


