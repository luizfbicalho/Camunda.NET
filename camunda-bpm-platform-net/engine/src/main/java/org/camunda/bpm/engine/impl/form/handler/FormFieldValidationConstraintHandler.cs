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
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FormFieldValidationConstraint = org.camunda.bpm.engine.form.FormFieldValidationConstraint;
	using FormFieldValidationException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidationException;
	using FormFieldValidator = org.camunda.bpm.engine.impl.form.validator.FormFieldValidator;
	using FormFieldValidatorContext = org.camunda.bpm.engine.impl.form.validator.FormFieldValidatorContext;
	using FormFieldValidatorException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidatorException;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// <para>Wrapper for a validation constraint</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class FormFieldValidationConstraintHandler
	{

	  protected internal string name;
	  protected internal string config;
	  protected internal FormFieldValidator validator;

	  public virtual FormFieldValidationConstraint createValidationConstraint(ExecutionEntity execution)
	  {
		return new FormFieldValidationConstraintImpl(name, config);
	  }

	  // submit /////////////////////////////////

	  public virtual void validate(object submittedValue, VariableMap submittedValues, FormFieldHandler formFieldHandler, VariableScope variableScope)
	  {
		try
		{

		  FormFieldValidatorContext context = new DefaultFormFieldValidatorContext(variableScope, config, submittedValues, formFieldHandler);
		  if (!validator.validate(submittedValue, context))
		  {
			throw new FormFieldValidatorException(formFieldHandler.Id, name, config, submittedValue, "Invalid value submitted for form field '" + formFieldHandler.Id + "': validation of " + this + " failed.");
		  }
		}
		catch (FormFieldValidationException e)
		{
		  throw new FormFieldValidatorException(formFieldHandler.Id, name, config, submittedValue, "Invalid value submitted for form field '" + formFieldHandler.Id + "': validation of " + this + " failed.", e);
		}
	  }

	  // getter / setter ////////////////////////

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string Config
	  {
		  set
		  {
			this.config = value;
		  }
		  get
		  {
			return config;
		  }
	  }


	  public virtual FormFieldValidator Validator
	  {
		  set
		  {
			this.validator = value;
		  }
		  get
		  {
			return validator;
		  }
	  }


	  public override string ToString()
	  {
		return name + (!string.ReferenceEquals(config, null) ? ("(" + config + ")") : "");
	  }

	}

}