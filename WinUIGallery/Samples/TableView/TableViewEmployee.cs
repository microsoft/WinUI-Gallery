// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinUIGallery.ControlPages;

// Row model shared by the TableView samples. Editable properties raise PropertyChanged so an
// edited cell writes back to the item and every bound cell refreshes.
[WinRT.GeneratedBindableCustomProperty]
public sealed partial class TableViewEmployee : INotifyPropertyChanged
{
    private string _name;
    private string _role;
    private string _notes;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id { get; }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public string Department { get; }
    public string Location { get; }
    public string Email { get; }
    public int Performance { get; }
    public string Status { get; }
    public string Initials { get; }
    public double PerformanceValue => Performance;

    public TableViewEmployee(string id, string name, string role, string department, string location, int performance, string status, string notes)
    {
        Id = id;
        _name = name;
        _role = role;
        _notes = notes;
        Department = department;
        Location = location;
        Performance = performance;
        Status = status;
        Email = name.ToLowerInvariant().Replace(' ', '.') + "@contoso.com";

        string[] parts = name.Split(' ');
        Initials = parts.Length > 1 ? $"{parts[0][0]}{parts[^1][0]}" : name[..1];
    }

    // Returns new instances on every call so each sample owns its rows.
    public static ObservableCollection<TableViewEmployee> CreateSampleData()
    {
        return
        [
            new("E-1001", "Amina Yusuf", "Program manager", "Operations", "Seattle", 92, "Active", "Leads the Q4 rollout."),
            new("E-1002", "Leo Martins", "Software engineer", "Engineering", "Lisbon", 78, "Active", "On-call this week."),
            new("E-1003", "Priya Shah", "Data analyst", "Finance", "Pune", 85, "Away", "Quarterly forecast owner."),
            new("E-1004", "Marek Nowak", "UX designer", "Design", "Warsaw", 64, "Active", "Working on the new theme."),
            new("E-1005", "Sofia Rossi", "Finance partner", "Finance", "Milan", 71, "Offline", "Out until Monday."),
            new("E-1006", "Kenji Sato", "Software engineer", "Engineering", "Tokyo", 88, "Active", "Owns the build pipeline."),
            new("E-1007", "Grace Okafor", "Recruiter", "People", "Lagos", 69, "Away", "Interview loop on Friday."),
            new("E-1008", "Noah Fischer", "Support engineer", "Operations", "Berlin", 57, "Active", "Escalation queue."),
            new("E-1009", "Isabella Garcia", "Product designer", "Design", "Madrid", 95, "Active", "Presenting at the design review."),
            new("E-1010", "Omar Haddad", "Security engineer", "Engineering", "Dubai", 81, "Offline", "Pen-test follow-ups."),
            new("E-1011", "Chloe Martin", "People partner", "People", "Paris", 74, "Active", "Onboarding new hires."),
            new("E-1012", "Arjun Mehta", "Site reliability engineer", "Operations", "Bengaluru", 90, "Active", "Capacity planning."),
        ];
    }

    private void SetProperty(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<string>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
