Imports System.Reflection

Namespace Localisation.Data

  Public Module Helpers

    Public Const JSONPropertyName As String = "__LocalisedData"

    ''' <summary>
    ''' Returns true if localisation is enabled, the current language and variant are not the default, and the class has the LocaliseDataAttribute
    ''' </summary>
    Public Function CanPerformDataLocalisation(Type As ReflectionCached.TypeInfo) As Boolean

      If LocalisationEnabled AndAlso TypeSupportsDataLocalisation(Type) Then

        If Localisation.CurrentLanguageID <> 1 OrElse If(Localisation.CurrentVariant, 0) <> DefaultVariantID Then
          Return True
        End If

      End If

      Return False
    End Function

    Public Function TypeSupportsDataLocalisation(TypeInfo As ReflectionCached.TypeInfo) As Boolean
      Return Attribute.IsDefined(TypeInfo.Type, GetType(LocaliseDataAttribute))
    End Function

    Public Function GetDataLocalisedProperties(TypeInfo As ReflectionCached.TypeInfo) As List(Of PropertyInfo)

      Dim lda As LocaliseDataAttribute = Attribute.GetCustomAttribute(TypeInfo.Type, GetType(LocaliseDataAttribute))

      Dim LocalisedProperties As New List(Of PropertyInfo)

      If lda.LocalisationLevel <> DataLocalisationLevel.ChildrenOnly Then

        TypeInfo.Type.ForEachBrowsableProperty(
        Nothing,
        Sub(pi)

          If pi.PropertyType Is GetType(String) Then
            Dim ldfa = Singular.Reflection.GetAttribute(Of LocaliseFieldAttribute)(pi)

            If (lda.LocalisationLevel = DataLocalisationLevel.AllStringProperties AndAlso (ldfa Is Nothing OrElse ldfa.Localise)) OrElse
              (lda.LocalisationLevel = DataLocalisationLevel.SpecifyProperties AndAlso ldfa IsNot Nothing AndAlso ldfa.Localise) Then

              LocalisedProperties.Add(pi)

            End If

          End If

        End Sub)

      End If

      Return LocalisedProperties

    End Function

  End Module

End Namespace