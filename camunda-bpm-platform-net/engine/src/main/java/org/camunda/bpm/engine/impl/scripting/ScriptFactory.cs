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
namespace org.camunda.bpm.engine.impl.scripting
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;

	/// <summary>
	/// <para>A script factory is responsible for creating a <seealso cref="ExecutableScript"/>
	/// instance. Users may customize (subclass) this class in order to customize script
	/// creation. For instance, some users may choose to pre-process scripts before
	/// they are created.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ScriptFactory
	{

	  public virtual ExecutableScript createScriptFromResource(string language, string resource)
	  {
		return new ResourceExecutableScript(language, resource);
	  }

	  public virtual ExecutableScript createScriptFromResource(string language, Expression resourceExpression)
	  {
		return new DynamicResourceExecutableScript(language, resourceExpression);
	  }

	  public virtual ExecutableScript createScriptFromSource(string language, string source)
	  {
		return new SourceExecutableScript(language, source);
	  }

	  public virtual ExecutableScript createScriptFromSource(string language, Expression sourceExpression)
	  {
		return new DynamicSourceExecutableScript(language, sourceExpression);
	  }

	}

}