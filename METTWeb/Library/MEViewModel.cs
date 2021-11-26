using System;
using System.Web;
using Singular.Web;

namespace MEWeb
{
  /// <summary>
  /// The MEStatelessViewModel class
  /// </summary>
  /// <typeparam name="ModelType"></typeparam>
  [Serializable]
  public class MEStatelessViewModel<ModelType>: StatelessViewModel<ModelType>
    where ModelType: StatelessViewModel<ModelType>
  {

	}
}
