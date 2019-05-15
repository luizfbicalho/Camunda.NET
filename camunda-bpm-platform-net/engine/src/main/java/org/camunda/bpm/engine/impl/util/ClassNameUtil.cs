using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
namespace org.camunda.bpm.engine.impl.util
{


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class ClassNameUtil
	{

	  protected internal static readonly IDictionary<Type, string> cachedNames = new ConcurrentDictionary<Type, string>();

	  public static string getClassNameWithoutPackage(object @object)
	  {
		return getClassNameWithoutPackage(@object.GetType());
	  }

	  public static string getClassNameWithoutPackage(Type clazz)
	  {
		string unqualifiedClassName = cachedNames[clazz];
		if (string.ReferenceEquals(unqualifiedClassName, null))
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  string fullyQualifiedClassName = clazz.FullName;
		  unqualifiedClassName = fullyQualifiedClassName.Substring(fullyQualifiedClassName.LastIndexOf('.') + 1);
		  cachedNames[clazz] = unqualifiedClassName;
		}
		return unqualifiedClassName;
	  }
	}

}