using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.Shared;

public interface ITextTemplate
{
    EntityDescriptorDto EntityDescriptor { get; }
    string TransformText();
}
