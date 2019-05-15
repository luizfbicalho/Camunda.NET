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
namespace org.camunda.bpm.engine.impl.@delegate
{

	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class ClassDelegate
	{

	  protected internal string className;
	  protected internal IList<FieldDeclaration> fieldDeclarations;

	  public ClassDelegate(string className, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.className = className;
		this.fieldDeclarations = fieldDeclarations;
	  }

	  public ClassDelegate(Type clazz, IList<FieldDeclaration> fieldDeclarations) : this(clazz.FullName, fieldDeclarations)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  }

	  public virtual string ClassName
	  {
		  get
		  {
			return className;
		  }
	  }

	  public virtual IList<FieldDeclaration> FieldDeclarations
	  {
		  get
		  {
			return fieldDeclarations;
		  }
	  }

	}

}