using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.form.handler
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormFieldValidationConstraint = org.camunda.bpm.engine.form.FormFieldValidationConstraint;
	using FormType = org.camunda.bpm.engine.form.FormType;
	using StartProcessVariableScope = org.camunda.bpm.engine.impl.el.StartProcessVariableScope;
	using AbstractFormFieldType = org.camunda.bpm.engine.impl.form.type.AbstractFormFieldType;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class FormFieldHandler
	{

	  protected internal string id;
	  protected internal Expression label;
	  protected internal AbstractFormFieldType type;
	  protected internal Expression defaultValueExpression;
	  protected internal IDictionary<string, string> properties = new Dictionary<string, string>();
	  protected internal IList<FormFieldValidationConstraintHandler> validationHandlers = new List<FormFieldValidationConstraintHandler>();
	  protected internal bool businessKey;

	  public virtual FormField createFormField(ExecutionEntity executionEntity)
	  {
		FormFieldImpl formField = new FormFieldImpl();

		// set id
		formField.Id = id;

		// set label (evaluate expression)
		VariableScope variableScope = executionEntity != null ? executionEntity : StartProcessVariableScope.SharedInstance;
		if (label != null)
		{
		  object labelValueObject = label.getValue(variableScope);
		  if (labelValueObject != null)
		  {
			formField.Label = labelValueObject.ToString();
		  }
		}

		formField.BusinessKey = businessKey;

		// set type
		formField.Type = type;

		// set default value (evaluate expression)
		object defaultValue = null;
		if (defaultValueExpression != null)
		{
		  defaultValue = defaultValueExpression.getValue(variableScope);

		  if (defaultValue != null)
		  {
			formField.DefaultValue = type.convertFormValueToModelValue(defaultValue);
		  }
		  else
		  {
			formField.DefaultValue = null;
		  }
		}

		// value
		TypedValue value = variableScope.getVariableTyped(id);
		if (value != null)
		{
		  formField.Value = type.convertToFormValue(value);
		}
		else
		{
		  // first, need to convert to model value since the default value may be a String Constant specified in the model xml.
		  TypedValue typedDefaultValue = type.convertToModelValue(Variables.untypedValue(defaultValue));
		  // now convert to form value
		  formField.Value = type.convertToFormValue(typedDefaultValue);
		}

		// properties
		formField.Properties = properties;

		// validation
		IList<FormFieldValidationConstraint> validationConstraints = formField.ValidationConstraints;
		foreach (FormFieldValidationConstraintHandler validationHandler in validationHandlers)
		{
		  // do not add custom validators
		  if (!"validator".Equals(validationHandler.name))
		  {
			validationConstraints.Add(validationHandler.createValidationConstraint(executionEntity));
		  }
		}

		return formField;
	  }

	  // submit /////////////////////////////////////////////

	  public virtual void handleSubmit(VariableScope variableScope, VariableMap values, VariableMap allValues)
	  {
		TypedValue submittedValue = (TypedValue) values.getValueTyped(id);
		values.remove(id);

		// perform validation
		foreach (FormFieldValidationConstraintHandler validationHandler in validationHandlers)
		{
		  object value = null;
		  if (submittedValue != null)
		  {
			value = submittedValue.Value;
		  }
		  validationHandler.validate(value, allValues, this, variableScope);
		}

		// update variable(s)
		TypedValue modelValue = null;
		if (submittedValue != null)
		{
		  if (type != null)
		  {
			modelValue = type.convertToModelValue(submittedValue);
		  }
		  else
		  {
			modelValue = submittedValue;
		  }
		}
		else if (defaultValueExpression != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.variable.value.TypedValue expressionValue = org.camunda.bpm.engine.variable.Variables.untypedValue(defaultValueExpression.getValue(variableScope));
		  TypedValue expressionValue = Variables.untypedValue(defaultValueExpression.getValue(variableScope));
		  if (type != null)
		  {
			// first, need to convert to model value since the default value may be a String Constant specified in the model xml.
			modelValue = type.convertToModelValue(Variables.untypedValue(expressionValue));
		  }
		  else if (expressionValue != null)
		  {
			modelValue = Variables.stringValue(expressionValue.Value.ToString());
		  }
		}

		if (modelValue != null)
		{
		  if (!string.ReferenceEquals(id, null))
		  {
			variableScope.setVariable(id, modelValue);
		  }
		}
	  }

	  // getters / setters //////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual Expression Label
	  {
		  get
		  {
			return label;
		  }
		  set
		  {
			this.label = value;
		  }
	  }


	  public virtual void setType(AbstractFormFieldType formType)
	  {
		this.type = formType;
	  }

	  public virtual IDictionary<string, string> Properties
	  {
		  set
		  {
			this.properties = value;
		  }
		  get
		  {
			return properties;
		  }
	  }


	  public virtual FormType getType()
	  {
		return type;
	  }

	  public virtual Expression DefaultValueExpression
	  {
		  get
		  {
			return defaultValueExpression;
		  }
		  set
		  {
			this.defaultValueExpression = value;
		  }
	  }


	  public virtual IList<FormFieldValidationConstraintHandler> ValidationHandlers
	  {
		  get
		  {
			return validationHandlers;
		  }
		  set
		  {
			this.validationHandlers = value;
		  }
	  }


	  public virtual bool BusinessKey
	  {
		  set
		  {
			this.businessKey = value;
		  }
		  get
		  {
			return businessKey;
		  }
	  }

	}

}