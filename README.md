# MvvmDemo — Simple WPF MVVM example

This version keeps SQLite persistence, adding/deleting/editing people, live name search, and LINQ sort buttons. Person now has Name and Age (plus Id as the database key).

## Three pieces to teach

| Role | File | Responsibility |
| --- | --- | --- |
| Model | Models/Person.cs | Name and Age data, with change notifications |
| View | Views/MainWindow.xaml | Layout and bindings |
| ViewModel | ViewModels/MainViewModel.cs | People, SelectedPerson, SearchText, Status, and commands |

Two small helpers support WPF: ObservableObject implements INotifyPropertyChanged, and RelayCommand wraps a method as ICommand. Data/PeopleContext.cs remains the EF Core database context. To keep this introductory example small, the view model uses it directly; there are no repositories, dependency injection containers, MVVM libraries, or navigation services.

## Walk through the bindings

1. MainWindow sets its DataContext to a MainViewModel. Its only other code disposes that view model when the window closes.
2. ItemsSource binds to People. Replacing that collection raises PropertyChanged, so no code assigns the grid's ItemsSource.
3. SelectedItem binds two-way to SelectedPerson. The editor binds to SelectedPerson.Name and SelectedPerson.Age.
4. SearchText updates on every keystroke. Its setter reruns the LINQ Where query over loaded EF entities.
5. Sort commands select OrderBy(Name) or OrderBy(Age). Name order is the default. Sorting runs when searching, adding, deleting, or clicking a sort button; property edits do not immediately reposition rows.
6. Buttons bind to ICommand properties instead of Click handlers. DeleteCommand.CanExecute disables Delete when there is no selection.
7. SaveCommand calls SaveChanges. The status label binds to Status, including database errors. The view model never references a window, grid, text box, or MessageBox.

ObservableCollection reports changes to a collection; INotifyPropertyChanged reports changes to a property. ICommand connects a view action to view-model behavior.

## Run and persistence

Open MvvmDemo.csproj in Visual Studio and press F5, or run `dotnet run` with the .NET 9 SDK on Windows. NuGet restores EF Core and its SQLite provider.

This version creates `%LOCALAPPDATA%\DataGridDemo\people-mvvm.db` with four sample people on first use. The older people.db file is left intact because its FirstName/LastName schema differs. EnsureCreated does not migrate existing schemas.

Use Add Person to create and select a new person. Edit Name/Age, then click Save Changes. Pending changes are discarded when closing without saving; the previous close confirmation and automatic editor focus were omitted to keep the MVVM example small. The editor is disabled without a selection. Invalid age text has a validation border and disables Save until corrected (switching rows discards invalid text).

Search includes unsaved additions/edits and is case-insensitive. It changes the displayed results, not EF tracking. Sorting and searching hold references to the same tracked Person objects.

Keyboard shortcuts: Alt+E Search, Alt+N Sort by Name, Alt+O Sort by Age, Alt+M Name, Alt+G Age, Alt+A Add, Alt+D Delete, Alt+S Save. Tab order follows the layout.

## Project layout

```text
Models/
  Person.cs
Data/
  PeopleContext.cs
Views/
  MainWindow.xaml
  MainWindow.xaml.cs
ViewModels/
  MainViewModel.cs
  RelayCommand.cs
App.xaml
App.xaml.cs
ObservableObject.cs
```

Namespaces follow the four folders. ObservableObject stays at the project root because it is shared by the model and view model. App.xaml starts Views/MainWindow.xaml.

