using System;

namespace EconomicOnParcs.Classes
{
    public interface IResource : IEquatable<IResource>
    {
        //I need this interface to make my dictionaries functional
        //So that the dictionary can contain both BasicResources and Industries at the same time
        bool SameType(IResource other);

        bool SameName(IResource other);
    }
}
