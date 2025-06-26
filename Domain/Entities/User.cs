namespace Domain.Entytis;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class User
{
    public Guid Id { get; private set; }
    public string Login { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Gender Gender { get; private set; }
    public DateTime? Birthday { get; private set; }
    public bool Admin { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime ModifiedOn { get; private set; }
    public string ModifiedBy { get; private set; } = string.Empty;
    public DateTime? RevokedOn { get; private set; }
    public string? RevokedBy { get; private set; }

    public bool IsActive => !RevokedOn.HasValue;

    private User() { } // EF Core

    public User(string login, string password, string name, Gender gender, DateTime? birthday, bool admin, string createdBy)
    {
        Id = Guid.NewGuid();
        SetLogin(login);
        SetPassword(password);
        SetName(name);
        Gender = gender;
        Birthday = birthday;
        Admin = admin;
        CreatedOn = DateTime.UtcNow;
        CreatedBy = createdBy;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = createdBy;
    }

    public void UpdatePersonalInfo(string name, Gender gender, DateTime? birthday, string modifiedBy)
    {
        SetName(name);
        Gender = gender;
        Birthday = birthday;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void UpdatePassword(string password, string modifiedBy)
    {
        SetPassword(password);
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void UpdateLogin(string login, string modifiedBy)
    {
        SetLogin(login);
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void SoftDelete(string revokedBy)
    {
        RevokedOn = DateTime.UtcNow;
        RevokedBy = revokedBy;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = revokedBy;
    }

    public void Restore(string modifiedBy)
    {
        RevokedOn = null;
        RevokedBy = null;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    private void SetLogin(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login cannot be empty", nameof(login));
        
        if (!Regex.IsMatch(login, @"^[a-zA-Z0-9]+$"))
            throw new ArgumentException("Login can only contain latin letters and digits", nameof(login));
        
        Login = login;
    }

    private void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));
        
        if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]+$"))
            throw new ArgumentException("Password can only contain latin letters and digits", nameof(password));
        
        Password = password;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        
        if (!Regex.IsMatch(name, @"^[a-zA-Zа-яА-Я]+$"))
            throw new ArgumentException("Name can only contain latin and cyrillic letters", nameof(name));
        
        Name = name;
    }

    public int GetAge()
    {
        if (!Birthday.HasValue) return 0;
        
        var today = DateTime.Today;
        var age = today.Year - Birthday.Value.Year;
        if (Birthday.Value.Date > today.AddYears(-age)) age--;
        return age;
    }
}

public enum Gender
{
    Female = 0,
    Male = 1,
    Unknown = 2
}