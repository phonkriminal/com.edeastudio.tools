using System;
using UnityEngine;

namespace edeastudio.Attributes
{
    /// <summary>
    /// The e toggle option attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class eToggleOptionAttribute : PropertyAttribute
    {
        /// <summary>
        /// Label.
        /// </summary>
        public string label, falseValue, trueValue;
        /// <summary>
        /// Initializes a new instance of the <see cref="eToggleOptionAttribute"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="falseValue">The false value.</param>
        /// <param name="trueValue">The true value.</param>
        public eToggleOptionAttribute(string label = "", string falseValue = "No", string trueValue = "Yes")
        {
            this.label = label;
            this.falseValue = falseValue;
            this.trueValue = trueValue;
        }
    }

}