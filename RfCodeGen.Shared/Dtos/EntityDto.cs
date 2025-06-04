using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.Shared.Dtos;

public record EntityDto(string FullName) : INotifyPropertyChanged
{
    public string FileName => Path.GetFileName(this.FullName);

    public string Name => Path.GetFileNameWithoutExtension(this.FullName);

    //public override string ToString() => this.FullName;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

