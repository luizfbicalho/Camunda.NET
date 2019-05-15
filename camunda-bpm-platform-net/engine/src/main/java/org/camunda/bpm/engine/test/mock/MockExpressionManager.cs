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
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using VariableContextElResolver = org.camunda.bpm.engine.impl.el.VariableContextElResolver;
	using VariableScopeElResolver = org.camunda.bpm.engine.impl.el.VariableScopeElResolver;
	using ArrayELResolver = org.camunda.bpm.engine.impl.javax.el.ArrayELResolver;
	using BeanELResolver = org.camunda.bpm.engine.impl.javax.el.BeanELResolver;
	using CompositeELResolver = org.camunda.bpm.engine.impl.javax.el.CompositeELResolver;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using ListELResolver = org.camunda.bpm.engine.impl.javax.el.ListELResolver;
	using MapELResolver = org.camunda.bpm.engine.impl.javax.el.MapELResolver;

	public class MockExpressionManager : ExpressionManager
	{

	  protected internal virtual ELResolver createElResolver(VariableScope scope)
	  {
		return createElResolver();
	  }

	  protected internal override ELResolver createElResolver()
	  {
		CompositeELResolver compositeElResolver = new CompositeELResolver();
		compositeElResolver.add(new VariableScopeElResolver());
		compositeElResolver.add(new VariableContextElResolver());
		compositeElResolver.add(new MockElResolver());
		compositeElResolver.add(new ArrayELResolver());
		compositeElResolver.add(new ListELResolver());
		compositeElResolver.add(new MapELResolver());
		compositeElResolver.add(new BeanELResolver());
		return compositeElResolver;
	  }

	}

}