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
namespace org.camunda.bpm.engine.impl.form
{

	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormFieldValidationConstraint = org.camunda.bpm.engine.form.FormFieldValidationConstraint;
	using FormType = org.camunda.bpm.engine.form.FormType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class FormFieldImpl : FormField
	{

	  protected internal bool businessKey;
	  protected internal string id;
	  protected internal string label;
	  protected internal FormType type;
	  protected internal object defaultValue;
	  protected internal TypedValue value;
	  protected internal IList<FormFieldValidationConstraint> validationConstraints = new List<FormFieldValidationConstraint>();
	  protected internal IDictionary<string, string> properties = new Dictionary<string, string>();

	  // getters / setters ///////////////////////////////////////////

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


	  public virtual string Label
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


	  public virtual FormType Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return type.Name;
		  }
	  }


	  public virtual object DefaultValue
	  {
		  get
		  {
			return defaultValue;
		  }
		  set
		  {
			this.defaultValue = value;
		  }
	  }

	  public virtual TypedValue Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }



	  public virtual IDictionary<string, string> Properties
	  {
		  get
		  {
			return properties;
		  }
		  set
		  {
			this.properties = value;
		  }
	  }


	  public virtual IList<FormFieldValidationConstraint> ValidationConstraints
	  {
		  get
		  {
			return validationConstraints;
		  }
		  set
		  {
			this.validationConstraints = value;
		  }
	  }


	  public virtual bool BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }

	}

}