Namespace Localisation

  Public Enum DataLocalisationLevel
    ''' <summary>
    ''' All string properties will be localised. Specific properties can be ignored by using the LocaliseField attribute
    ''' </summary>
    AllStringProperties = 1
    ''' <summary>
    ''' Only properties marked with the LocaliseField attribute will be localised.
    ''' </summary>
    SpecifyProperties = 2
    ''' <summary>
    ''' This object will not be localised, but its children will be checked for the LocaliseData attribute.
    ''' </summary>
    ChildrenOnly = 3
  End Enum

  ''' <summary>
  ''' Marks a class for data localisation. Localised data will be stored in the LocalisationDataValues table.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Class)>
  Public Class LocaliseDataAttribute
    Inherits Attribute

    Public Property LocalisationLevel As DataLocalisationLevel

    Public Sub New()
      Me.LocalisationLevel = DataLocalisationLevel.AllStringProperties
    End Sub

    Public Sub New(LocalisationLevel As DataLocalisationLevel)
      Me.LocalisationLevel = LocalisationLevel
    End Sub

  End Class

  ''' <summary>
  ''' Marks a property for data localisation. Only required if the class localisation level is set to "SpecifyProperties", or if you want to exclude properties.
  ''' </summary>
  <AttributeUsage(AttributeTargets.Property)>
  Public Class LocaliseFieldAttribute
    Inherits Attribute

    Public Property Localise As Boolean

    Public Sub New()
      Me.Localise = True
    End Sub

    Public Sub New(Localise As Boolean)
      Me.Localise = Localise
    End Sub

  End Class

End Namespace
