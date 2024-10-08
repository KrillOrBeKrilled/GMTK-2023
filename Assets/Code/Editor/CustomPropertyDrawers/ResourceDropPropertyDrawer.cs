using UnityEditor;
using KrillOrBeKrilled.Model;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace KrillOrBeKrilled {
  [CustomPropertyDrawer(typeof(ResourceDrop))]
  public class ResourceDropPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      // Create the root VisualElement
      VisualElement container = new() {
        style = { flexDirection = FlexDirection.Row }
      };
      
      // Create the EnumField for the 'Type' property
      SerializedProperty typeProperty = property.FindPropertyRelative("resourceType");
      EnumField typeField = new(typeProperty.displayName) {
        bindingPath = typeProperty.propertyPath,
        style = { width = 125, marginRight = 2}
      };
      typeField.labelElement.style.display = DisplayStyle.None;

      // Create the IntegerField for the 'Amount' property
      SerializedProperty amountProperty = property.FindPropertyRelative("weight");
      IntegerField amountField = new(amountProperty.displayName)
      {
        bindingPath = amountProperty.propertyPath,
        style = { flexGrow = 1 }
      };
      amountField.labelElement.style.marginRight = 5;
      amountField.labelElement.style.paddingRight = 5;
      amountField.labelElement.style.marginLeft = 1;
      amountField.labelElement.style.paddingLeft = 1;
      amountField.labelElement.style.minWidth = 0;
      
      // Add the fields to the container
      container.Add(typeField);
      container.Add(amountField);

      // Bind the container to the serialized property
      container.Bind(property.serializedObject);

      return container;
    }
  }
}