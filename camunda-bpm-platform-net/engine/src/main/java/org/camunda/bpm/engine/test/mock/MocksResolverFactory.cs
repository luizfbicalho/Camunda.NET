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
namespace org.camunda.bpm.engine.test.mock
{
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using Resolver = org.camunda.bpm.engine.impl.scripting.engine.Resolver;
	using ResolverFactory = org.camunda.bpm.engine.impl.scripting.engine.ResolverFactory;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class MocksResolverFactory : ResolverFactory, Resolver
	{

	  public virtual Resolver createResolver(VariableScope variableScope)
	  {
		return this;
	  }

	  public virtual bool containsKey(object key)
	  {
		return Mocks.get(key) != null;
	  }

	  public virtual object get(object key)
	  {
		return Mocks.get(key);
	  }

	  public virtual ISet<string> keySet()
	  {
		return Mocks.Mocks.Keys;
	  }

	}

}