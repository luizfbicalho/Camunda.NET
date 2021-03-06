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
namespace org.camunda.bpm.engine.impl.scripting.engine
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using Context = org.camunda.bpm.engine.impl.context.Context;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class BeansResolverFactory : ResolverFactory, Resolver
	{

	  public virtual Resolver createResolver(VariableScope variableScope)
	  {
		return this;
	  }

	  public virtual bool containsKey(object key)
	  {
		return Context.ProcessEngineConfiguration.Beans.ContainsKey(key);
	  }

	  public virtual object get(object key)
	  {
		return Context.ProcessEngineConfiguration.Beans[key];
	  }

	  public virtual ISet<string> keySet()
	  {
		return (ISet<object>) Context.ProcessEngineConfiguration.Beans.Keys;
	  }
	}

}