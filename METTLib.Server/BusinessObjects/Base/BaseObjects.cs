using System;
using Singular;

namespace MELib
{
  /// <summary>
  /// METTBusinessBase Class (Base class for business objects)
  /// </summary>
  /// <typeparam name="C">Type based on METTBusinessBase</typeparam>
  [Serializable]
  public class MEBusinessBase<C> : SingularBusinessBase<C>
    where C : MEBusinessBase<C>
  {
  }

  /// <summary> 
  /// METTBusinessListBase Class (Base class for business object lists)
  /// </summary>
  /// <typeparam name="T">Type based on METTBusinessListBase</typeparam>
  /// <typeparam name="C">Type based on METTBusinessBase</typeparam>
  [Serializable]
  public class MEBusinessListBase<T, C> : SingularBusinessListBase<T, C>
    where T : MEBusinessListBase<T, C>
    where C : MEBusinessBase<C>
  {
  }

  /// <summary>
  /// METTReadOnlyBase Class (Base class for read only business objects)
  /// </summary>
  /// <typeparam name="C">Type based on METTReadOnlyBase</typeparam>
  public class MEReadOnlyBase<C> : SingularReadOnlyBase<C>
    where C: MEReadOnlyBase<C>
  {
  }

  /// <summary>
  /// METTReadOnlyListBase Class (Base class for read only business object lists)
  /// </summary>
  /// <typeparam name="T">Type based on METTReadOnlyListBase</typeparam>
  /// <typeparam name="C">Type based on METTReadOnlyBase</typeparam>
  public class MEReadOnlyListBase<T, C> : SingularReadOnlyListBase<T, C>
    where T: MEReadOnlyListBase<T, C>
    where C: MEReadOnlyBase<C>
  {
  }
}
