using UnityEditor;
using KrillOrBeKrilled.Model;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace KrillOrBeKrilled {
  [CustomPropertyDrawer(typeof(ResourceAmount))]
  public class ResourceAmountPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      // Create the root VisualElement
      VisualElement container = new();
      container.style.flexDirection = FlexDirection.Row;
      
      // Create the EnumField for the 'Type' property
      SerializedProperty typeProperty = property.FindPropertyRelative("Type");
      EnumField typeField = new(typeProperty.displayName) {
        bindingPath = typeProperty.propertyPath,
        style = { flexGrow = 1, marginRight = 2 }
      };
      typeField.labelElement.style.display = DisplayStyle.None;

      // Create the IntegerField for the 'Amount' property
      SerializedProperty amountProperty = property.FindPropertyRelative("Amount");
      IntegerField amountField = new(amountProperty.displayName)
      {
        bindingPath = amountProperty.propertyPath,
        style = { width = 50 }
      };
      amountField.labelElement.style.display = DisplayStyle.None;

      // Add the fields to the container
      container.Add(typeField);
      container.Add(amountField);

      // Bind the container to the serialized property
      container.Bind(property.serializedObject);

      return container;
    }
  }
}