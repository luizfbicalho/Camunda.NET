using System;

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
namespace org.camunda.bpm.engine.impl.form.validator
{

	/// <summary>
	/// Runtime exception for validation of form fields.
	/// 
	/// @author Thomas Skjolberg
	/// </summary>
	public class FormFieldValidatorException : FormException
	{

	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// bpmn element id </summary>
	  protected internal readonly string id;
	  protected internal readonly string name;
	  protected internal readonly string config;
	  protected internal readonly object value;

	  public FormFieldValidatorException(string id, string name, string config, object value, string message, Exception cause) : base(message, cause)
	  {

		this.id = id;
		this.name = name;
		this.config = config;
		this.value = value;
	  }

	  public FormFieldValidatorException(string id, string name, string config, object value, string message) : base(message)
	  {

		this.id = id;
		this.name = name;
		this.config = config;
		this.value = value;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Config
	  {
		  get
		  {
			return config;
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	}

}