using FlowEngine.Core.elements.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Core.elements.interfaces
{
    public interface IElement
    {
        ElementType getType();
        string getElementName();
        ICollection<IAttribute> getAttributes();
        IAttribute getAttribute(string name);

        void InitializeElement();

        void ValidateRequiredAttributes();
    }
}
