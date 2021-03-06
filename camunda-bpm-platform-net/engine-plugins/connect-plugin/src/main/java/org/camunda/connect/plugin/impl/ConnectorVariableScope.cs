﻿using System;
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
namespace org.camunda.connect.plugin.impl
{

	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using SimpleVariableInstance = org.camunda.bpm.engine.impl.core.variable.scope.SimpleVariableInstance;
	using SimpleVariableInstanceFactory = org.camunda.bpm.engine.impl.core.variable.scope.SimpleVariableInstance.SimpleVariableInstanceFactory;
	using VariableInstanceFactory = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceFactory;
	using VariableInstanceLifecycleListener = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener;
	using VariableStore = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore;
	using ConnectorRequest = org.camunda.connect.spi.ConnectorRequest;
	using ConnectorResponse = org.camunda.connect.spi.ConnectorResponse;

	/// <summary>
	/// Exposes a connector request as variableScope.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class ConnectorVariableScope : AbstractVariableScope
	{

	  private const long serialVersionUID = 1L;

	  protected internal AbstractVariableScope parent;

	  protected internal VariableStore<SimpleVariableInstance> variableStore;

	  public ConnectorVariableScope(AbstractVariableScope parent)
	  {
		this.parent = parent;
		this.variableStore = new VariableStore<SimpleVariableInstance>();
	  }

	  public override string VariableScopeKey
	  {
		  get
		  {
			return "connector";
		  }
	  }

	  protected internal override VariableStore<CoreVariableInstance> VariableStore
	  {
		  get
		  {
			return (VariableStore) variableStore;
		  }
	  }

	  protected internal override VariableInstanceFactory<CoreVariableInstance> VariableInstanceFactory
	  {
		  get
		  {
			return (VariableInstanceFactory) SimpleVariableInstance.SimpleVariableInstanceFactory.INSTANCE;
		  }
	  }

	  protected internal override IList<VariableInstanceLifecycleListener<CoreVariableInstance>> VariableInstanceLifecycleListeners
	  {
		  get
		  {
			return Collections.emptyList();
		  }
	  }

	  public override AbstractVariableScope ParentVariableScope
	  {
		  get
		  {
			return parent;
		  }
	  }

	  public virtual void writeToRequest<T1>(ConnectorRequest<T1> request)
	  {
		foreach (CoreVariableInstance variable in variableStore.Variables)
		{
		  request.setRequestParameter(variable.Name, variable.getTypedValue(true).Value);
		}
	  }

	  public virtual void readFromResponse(ConnectorResponse response)
	  {
		IDictionary<string, object> responseParameters = response.ResponseParameters;
		foreach (KeyValuePair<string, object> entry in responseParameters.SetOfKeyValuePairs())
		{
		  setVariableLocal(entry.Key, entry.Value);
		}
	  }

	}

}