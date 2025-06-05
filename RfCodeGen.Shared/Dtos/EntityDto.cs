using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.Shared.Dtos;

public record EntityDto(string FilePath)
{
    public string FileName => Path.GetFileName(this.FilePath);
    public string Name => Path.GetFileNameWithoutExtension(this.FilePath);
}

