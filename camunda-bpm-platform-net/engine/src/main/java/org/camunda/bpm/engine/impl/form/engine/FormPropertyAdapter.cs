﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl.form.engine
{

	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormFieldValidationConstraint = org.camunda.bpm.engine.form.FormFieldValidationConstraint;
	using FormProperty = org.camunda.bpm.engine.form.FormProperty;
	using FormType = org.camunda.bpm.engine.form.FormType;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class FormPropertyAdapter : FormField
	{

	  protected internal FormProperty formProperty;
	  protected internal IList<FormFieldValidationConstraint> validationConstraints;

	  public FormPropertyAdapter(FormProperty formProperty) : base()
	  {
		this.formProperty = formProperty;

		validationConstraints = new List<FormFieldValidationConstraint>();
		if (formProperty.Required)
		{
		  validationConstraints.Add(new FormFieldValidationConstraintImpl("required", null));
		}
		if (!formProperty.Writable)
		{
		  validationConstraints.Add(new FormFieldValidationConstraintImpl("readonly", null));
		}
	  }

	  public virtual string Id
	  {
		  get
		  {
			return formProperty.Id;
		  }
	  }

	  public virtual string Label
	  {
		  get
		  {
			return formProperty.Name;
		  }
	  }
	  public virtual FormType Type
	  {
		  get
		  {
			return formProperty.Type;
		  }
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return formProperty.Type.Name;
		  }
	  }

	  public virtual object DefaultValue
	  {
		  get
		  {
			return formProperty.Value;
		  }
	  }

	  public virtual IList<FormFieldValidationConstraint> ValidationConstraints
	  {
		  get
		  {
			return validationConstraints;
		  }
	  }

	  public virtual IDictionary<string, string> Properties
	  {
		  get
		  {
			return Collections.emptyMap();
		  }
	  }

	  public virtual bool BusinessKey
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual TypedValue DefaultValueTyped
	  {
		  get
		  {
			return Value;
		  }
	  }

	  public virtual TypedValue Value
	  {
		  get
		  {
			return Variables.stringValue(formProperty.Value);
		  }
	  }

	}

}