using System;
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

	using FormData = org.camunda.bpm.engine.form.FormData;
	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormProperty = org.camunda.bpm.engine.form.FormProperty;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public abstract class FormDataImpl : FormData
	{

	  private const long serialVersionUID = 1L;

	  protected internal string formKey;
	  protected internal string deploymentId;
	  protected internal IList<FormProperty> formProperties = new List<FormProperty>();

	  protected internal IList<FormField> formFields = new List<FormField>();

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string FormKey
	  {
		  get
		  {
			return formKey;
		  }
		  set
		  {
			this.formKey = value;
		  }
	  }
	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }
	  public virtual IList<FormProperty> FormProperties
	  {
		  get
		  {
			return formProperties;
		  }
		  set
		  {
			this.formProperties = value;
		  }
	  }




	  public virtual IList<FormField> FormFields
	  {
		  get
		  {
			return formFields;
		  }
		  set
		  {
			this.formFields = value;
		  }
	  }
	}

}