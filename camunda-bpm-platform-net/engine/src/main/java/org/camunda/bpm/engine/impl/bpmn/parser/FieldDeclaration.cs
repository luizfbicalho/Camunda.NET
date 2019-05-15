﻿/*
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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{


	/// <summary>
	/// Represents a field declaration in object form:
	/// 
	/// &lt;field name='someField&gt; &lt;string ...
	/// 
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	public class FieldDeclaration
	{

	  protected internal string name;
	  protected internal string type;
	  protected internal object value;

	  public FieldDeclaration(string name, string type, object value)
	  {
		this.name = name;
		this.type = type;
		this.value = value;
	  }

	  public FieldDeclaration()
	  {

	  }

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
	  public virtual string Type
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
	  public virtual object Value
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

	}

}