using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//------------------------------------------------------------------------------
/**
    @file	: SirenixProxy

    @brief	: 			

    @date	: 7/12/2019 10:14:18 AM

    @author	: fishdai
*/
//------------------------------------------------------------------------------

namespace MS.Framework.SirenixProxy
{
    public enum ButtonStyle
    {
        CompactBox = 0,
        FoldoutButton = 1,
        Box = 2
    }

    [Conditional("UNITY_EDITOR")]
    public class ButtonAttribute : ShowInInspectorAttribute
    {
        /// <summary>
        /// Gets the height of the button. If it's zero or below then use default.
        /// </summary>
        public int ButtonHeight;

        /// <summary>Use this to override the label on the button.</summary>
        public string Name;

        /// <summary>
        /// If the button contains parameters, you can disable the foldout it creates by setting this to true.
        /// </summary>
        public bool Expanded;

        public ButtonAttribute() { }
        public ButtonAttribute(ButtonSizes size) { }
        public ButtonAttribute(int buttonSize) { }
        public ButtonAttribute(string name) { }
        public ButtonAttribute(ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(string name, ButtonSizes buttonSize) { }
        public ButtonAttribute(string name, int buttonSize) { }
        public ButtonAttribute(int buttonSize, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(ButtonSizes size, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(string name, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(string name, ButtonSizes buttonSize, ButtonStyle parameterBtnStyle) { }
        public ButtonAttribute(string name, int buttonSize, ButtonStyle parameterBtnStyle) { }
    }

    [Conditional("UNITY_EDITOR")]
      public class GUIColorAttribute : Attribute
      {
        /// <summary>The GUI color of the property.</summary>
        public Color Color;
        /// <summary>
        /// The name of a local field, member or property that returns a Color. Both static and instance methods are supported.
        /// </summary>
        public string GetColor;

        /// <summary>Sets the GUI color for the property.</summary>
        /// <param name="r">The red channel.</param>
        /// <param name="g">The green channel.</param>
        /// <param name="b">The blue channel.</param>
        /// <param name="a">The alpha channel.</param>
        public GUIColorAttribute(float r, float g, float b, float a = 1f)
        {
        }

        /// <summary>Sets the GUI color for the property.</summary>
        /// <param name="getColor">Specify the name of a local field, member or property that returns a Color.</param>
        public GUIColorAttribute(string getColor)
        {
        }
      }
    public enum ButtonSizes
    {
        /// <summary>
        /// Small button size, fits well with properties in the inspector.
        /// </summary>
        Small = 0,

        /// <summary>A larger button.</summary>
        Medium = 22, // 0x00000016

        /// <summary>A very large button.</summary>
        Large = 31, // 0x0000001F

        /// <summary>A gigantic button. Twice as big as Large</summary>
        Gigantic = 62, // 0x0000003E
    }

    [Conditional("UNITY_EDITOR")]
    public sealed class SceneObjectsOnlyAttribute : Attribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class DisplayAsStringAttribute : Attribute
    {
        /// <summary>
        /// If <c>true</c> the string will overflow to multiple lines, if there's not enough space when drawn.
        /// </summary>
        public bool Overflow;

        /// <summary>Displays the property as a string in the inspector.</summary>
        public DisplayAsStringAttribute()
        {
            this.Overflow = true;
        }

        /// <summary>Displays the property as a string in the inspector.</summary>
        /// <param name="overflow">Value indicating if the string should overflow to multiple lines or not.</param>
        public DisplayAsStringAttribute(bool overflow)
        {
            this.Overflow = overflow;
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class TableColumnWidthAttribute : Attribute
    {
        /// <summary>
        /// Whether the column should be resizable. True by default.
        /// </summary>
        public bool Resizable = true;

        /// <summary>The width of the column.</summary>
        public int Width;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sirenix.OdinInspector.TableColumnWidthAttribute" /> class.
        /// </summary>
        /// <param name="width">The width of the column in pixels.</param>
        /// <param name="resizable">If <c>true</c> then the column can be resized in the inspector.</param>
        public TableColumnWidthAttribute(int width, bool resizable = true)
        {
            this.Width = width;
            this.Resizable = resizable;
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class BoxGroupAttribute : Attribute
    {
        public BoxGroupAttribute(string group, bool showLabel = true, bool centerLabel = false, int order = 0)
        {
        }

        public BoxGroupAttribute()
        {
            
        }
        
    }

    [Conditional("UNITY_EDITOR")]
    public class FoldoutGroupAttribute : Attribute
    {
        public string GroupID;
        public string GroupName;
        public int Order;

        public FoldoutGroupAttribute(string groupName, bool expanded = false, int order = 0)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class HorizontalGroupAttribute : Attribute
    {
        public HorizontalGroupAttribute(float width = 0.0f, int marginLeft = 0, int marginRight = 0, int order = 0)
        {
        }

        public HorizontalGroupAttribute(
            string group,
            float width = 0.0f,
            int marginLeft = 0,
            int marginRight = 0,
            int order = 0)
        {
        }

        public int LabelWidth;
    }

    [Conditional("UNITY_EDITOR")]
    public class PropertyOrderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sirenix.OdinInspector.PropertyOrderAttribute" /> class.
        /// </summary>
        public PropertyOrderAttribute()
        {
        }

        /// <summary>Defines a custom order for the property.</summary>
        /// <param name="order">The order for the property.</param>
        public PropertyOrderAttribute(int order)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class HideReferenceObjectPickerAttribute : Attribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class LabelTextAttribute : Attribute
    {
        public LabelTextAttribute(string text)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class EnumToggleButtons : Attribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class ValueDropdownAttribute : Attribute
    {
        public bool FlattenTreeView;

        public ValueDropdownAttribute(string memberName)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class HideInEditorModeAttribute : Attribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class ShowInInspectorAttribute : Attribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class HideInPlayModeAttribute : Attribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class ListDrawerSettingsAttribute : Attribute
    {
        public bool HideAddButton;
        public bool AddCopiesLastElement;
        public string OnEndListElementGUI;
        public string OnBeginListElementGUI;
        public string CustomRemoveElementFunction;
        public bool AlwaysAddDefaultValue;
        public string CustomAddFunction;
        public string ListElementLabelName;
        public bool HideRemoveButton;
        public string CustomRemoveIndexFunction;
        public bool PagingHasValue { get; }
        public bool IsReadOnlyHasValue { get; }
        public bool DraggableHasValue { get; }
        public bool NumberOfItemsPerPageHasValue { get; }
        public bool ShowItemCountHasValue { get; }
        public string OnTitleBarGUI { get; set; }
        public bool DraggableItems { get; set; }
        public bool Expanded { get; set; }
        public bool ShowItemCount { get; set; }
        public bool IsReadOnly { get; set; }
        public int NumberOfItemsPerPage { get; set; }
        public bool ExpandedHasValue { get; }
        public bool ShowPaging { get; set; }
        public bool ShowIndexLabels { get; set; }
        public bool ShowIndexLabelsHasValue { get; }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class ShowIfAttribute : Attribute
    {
        public ShowIfAttribute(string memberName, bool animate = true)
        {
        }

        public ShowIfAttribute(string memberName, object optionalValue, bool animate = true)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class HideIfAttribute : Attribute
    {
        public HideIfAttribute(string memberName, bool animate = true)
        {
        }

        public HideIfAttribute(string memberName, object optionalValue, bool animate = true)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : Attribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class EnableIfAttribute : Attribute
    {
        public EnableIfAttribute(string memberName)
        {
        }

        public EnableIfAttribute(string memberName, object optionalValue)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class DisableIfAttribute : Attribute
    {
        public DisableIfAttribute(string memberName)
        {
        }

        public DisableIfAttribute(string memberName, object optionalValue)
        {
        }
    }


    [Conditional("UNITY_EDITOR")]
    public class TitleAttribute : Attribute
    {
        public TitleAttribute(string text)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class IndentAttribute : Attribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class MinMaxSliderAttribute : Attribute
    {
        public MinMaxSliderAttribute(float minValue, float maxValue, bool showFields = false)
        {
        }
    }

    public enum InfoMessageType
    {
        //
        // 摘要:
        //     /// Generic message box with no type. ///
        None,

        //
        // 摘要:
        //     /// Information message box. ///
        Info,

        //
        // 摘要:
        //     /// Warning message box. ///
        Warning,

        //
        // 摘要:
        //     /// Error message box. ///
        Error
    }

    [Conditional("UNITY_EDITOR")]
    public class InfoBoxAttribute : Attribute
    {
        public string Message;
        public InfoMessageType InfoMessageType;
        public string VisibleIf;
        public bool GUIAlwaysEnabled;

        public InfoBoxAttribute(string message)
        {
        }

        public InfoBoxAttribute(string message, string visibleIfMemberName)
        {
        }

        public InfoBoxAttribute(string message, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIfMemberName = null)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class MinValue : Attribute
    {
        public MinValue(float value)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class MaxValue : Attribute
    {
        public MaxValue(float value)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class HideLabelAttribute : Attribute
    {
        public HideLabelAttribute()
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class FilePathAttribute : Attribute
    {
        public bool AbsolutePath;
        public string Extensions;
        public string ParentFolder;
        public bool RequireExistingPath;
        public bool UseBackslashes;

        public FilePathAttribute()
        {
        }
    }


    [Conditional("UNITY_EDITOR")]
    public class FolderPathAttribute : Attribute
    {
        public bool AbsolutePath;
        public string ParentFolder;
        public bool RequireValidPath;
        public bool RequireExistingPath;
        public bool UseBackslashes;

        public FolderPathAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class OnValueChangedAttribute : Attribute
    {
        public string MethodName;


        public bool IncludeChildren;


        public OnValueChangedAttribute(string methodName, bool includeChildren = false)
        {
            MethodName = methodName;
            IncludeChildren = includeChildren;
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class OnInspectorGUIAttribute : Attribute
    {
        public string PrependMethodName;
        public string AppendMethodName;

        public OnInspectorGUIAttribute()
        {
        }

        public OnInspectorGUIAttribute(string methodName, bool append = true)
        {
        }

        public OnInspectorGUIAttribute(string prependMethodName, string appendMethodName)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class TableListAttribute : Attribute
    {
        public int NumberOfItemsPerPage;

        public bool IsReadOnly;
        public int DefaultMinColumnWidth = 40;
        public bool ShowIndexLabels;
        public bool DrawScrollView = true;
        public int MinScrollViewHeight = 350;
        public int MaxScrollViewHeight;
        public bool AlwaysExpanded;
        public bool HideToolbar = false;
        public int CellPadding = 2;
    }


    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class InlinePropertyAttribute : Attribute
    {
        public int LabelWidth;

        public InlinePropertyAttribute()
        {
        }
    }


    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class PropertyTooltipAttribute : Attribute
    {
        public string Tooltip;

        public PropertyTooltipAttribute(string tooltip)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All)]
    public class DrawWithUnityAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.All)]
    [Conditional("UNITY_EDITOR")]
    public class DisableInPlayModeAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.All)]
    public class DisableInEditorModeAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class PropertyGroupAttribute : Attribute
    {
        //
        // Summary:
        //     The ID used to grouping properties together.
        public string GroupID;
        //
        // Summary:
        //     The name of the group. This is the last part of the group ID if there is a path,
        //     otherwise it is just the group ID.
        public string GroupName;
        //
        // Summary:
        //     The order of the group.
        public int Order;

        public PropertyGroupAttribute(){ }

        //
        // Summary:
        //     Initializes a new instance of the Sirenix.OdinInspector.PropertyGroupAttribute
        //     class.
        //
        // Parameters:
        //   groupId:
        //     The group identifier.
        public PropertyGroupAttribute(string groupId){ }
        //
        // Summary:
        //     Initializes a new instance of the Sirenix.OdinInspector.PropertyGroupAttribute
        //     class.
        //
        // Parameters:
        //   groupId:
        //     The group identifier.
        //
        //   order:
        //     The group order.
        public PropertyGroupAttribute(string groupId, int order){ }
    }

    public class ShowIfGroupAttribute : PropertyGroupAttribute
    {
        //
        // Summary:
        //     Whether or not to slide the properties in and out when the state changes.
        public bool Animate;
        //
        // Summary:
        //     The optional member value.
        public object Value;

        //
        // Summary:
        //     Makes a group that can be shown or hidden based on a condition.
        //
        // Parameters:
        //   path:
        //     The group path.
        //
        //   animate:
        //     If true then a fade animation will be played when the group is hidden or shown.
        public ShowIfGroupAttribute(string path, bool animate = true){ }
        //
        // Summary:
        //     Makes a group that can be shown or hidden based on a condition.
        //
        // Parameters:
        //   path:
        //     The group path.
        //
        //   value:
        //     The value the member should equal for the property to shown.
        //
        //   animate:
        //     If true then a fade animation will be played when the group is hidden or shown.
        public ShowIfGroupAttribute(string path, object value, bool animate = true){ }

        //
        // Summary:
        //     Name of member to use when to show the group. Defaults to the name of the group,
        //     by can be overriden by setting this property.
        public string MemberName { get; set; }

    }
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class CustomValueDrawerAttribute : Attribute
    {
        /// <summary>Name of the custom drawer method.</summary>
        public string MethodName;

        /// <summary>
        /// Instead of making a new attribute, and a new drawer, for a one-time thing, you can with this attribute, make a method that acts as a custom property drawer.
        /// These drawers will out of the box have support for undo/redo and multi-selection.
        /// </summary>
        /// <param name="methodName">The name of the method to draw the value.</param>
        public CustomValueDrawerAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}