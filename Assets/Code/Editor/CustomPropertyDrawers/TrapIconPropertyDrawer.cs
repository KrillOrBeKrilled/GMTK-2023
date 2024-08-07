using UnityEditor;
using KrillOrBeKrilled.Model;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KrillOrBeKrilled {
  [CustomPropertyDrawer(typeof(TrapIcon))]
  public class TrapIconPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      // Create the root VisualElement
      VisualElement container = new() {
        style = { flexDirection = FlexDirection.Row }
      };
      
      // Create the EnumField for the 'Type' property
      SerializedProperty typeProperty = property.FindPropertyRelative("TrapType");
      EnumField typeField = new(typeProperty.displayName) {
        bindingPath = typeProperty.propertyPath,
        style = { flexGrow = 2, marginRight = 2}
      };
      typeField.labelElement.style.display = DisplayStyle.None;

      // Create the IntegerField for the 'Amount' property
      SerializedProperty amountProperty = property.FindPropertyRelative("Icon");
      ObjectField amountField = new(amountProperty.displayName)
      {
        bindingPath = amountProperty.propertyPath,
        objectType = typeof(Sprite),
        style = { flexGrow = 1 }
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