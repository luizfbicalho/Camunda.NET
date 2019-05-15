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
namespace org.camunda.bpm.engine.impl.externaltask
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[Serializable]
	public class TopicFetchInstruction
	{

	  private const long serialVersionUID = 1L;

	  protected internal string topicName;
	  protected internal string businessKey;
	  protected internal string processDefinitionId;
	  protected internal string[] processDefinitionIds;
	  protected internal string processDefinitionKey;
	  protected internal string[] processDefinitionKeys;
	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
	  protected internal IList<string> variablesToFetch;

	  protected internal IList<QueryVariableValue> filterVariables;
	  protected internal long lockDuration;
	  protected internal bool deserializeVariables = false;
	  protected internal bool localVariables = false;

	  public TopicFetchInstruction(string topicName, long lockDuration)
	  {
		this.topicName = topicName;
		this.lockDuration = lockDuration;
		this.filterVariables = new List<QueryVariableValue>();
	  }

	  public virtual IList<string> VariablesToFetch
	  {
		  get
		  {
			return variablesToFetch;
		  }
		  set
		  {
			this.variablesToFetch = value;
		  }
	  }


	  public virtual string BusinessKey
	  {
		  set
		  {
			this.businessKey = value;
		  }
		  get
		  {
			return businessKey;
		  }
	  }


	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
		  get
		  {
			return processDefinitionId;
		  }
	  }


	  public virtual string[] ProcessDefinitionIds
	  {
		  set
		  {
			this.processDefinitionIds = value;
		  }
		  get
		  {
			return processDefinitionIds;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
		  }
		  get
		  {
			return processDefinitionKey;
		  }
	  }


	  public virtual string[] ProcessDefinitionKeys
	  {
		  set
		  {
			this.processDefinitionKeys = value;
		  }
		  get
		  {
			return processDefinitionKeys;
		  }
	  }


	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
		  set
		  {
			this.isTenantIdSet = value;
		  }
	  }


	  public virtual string[] TenantIds
	  {
		  get
		  {
			return tenantIds;
		  }
		  set
		  {
			isTenantIdSet = true;
			this.tenantIds = value;
		  }
	  }


	  public virtual IList<QueryVariableValue> getFilterVariables()
	  {
		return filterVariables;
	  }

	  public virtual void setFilterVariables(IDictionary<string, object> filterVariables)
	  {
		QueryVariableValue variableValue;
		foreach (KeyValuePair<string, object> filter in filterVariables.SetOfKeyValuePairs())
		{
		  variableValue = new QueryVariableValue(filter.Key, filter.Value, null, false);
		  this.filterVariables.Add(variableValue);
		}
	  }

	  public virtual void addFilterVariable(string name, object value)
	  {
		QueryVariableValue variableValue = new QueryVariableValue(name, value, QueryOperator.EQUALS, true);
		this.filterVariables.Add(variableValue);
	  }

	  public virtual long? LockDuration
	  {
		  get
		  {
			return lockDuration;
		  }
	  }

	  public virtual string TopicName
	  {
		  get
		  {
			return topicName;
		  }
	  }

	  public virtual bool DeserializeVariables
	  {
		  get
		  {
			return deserializeVariables;
		  }
		  set
		  {
			this.deserializeVariables = value;
		  }
	  }


	  public virtual void ensureVariablesInitialized()
	  {
		if (filterVariables.Count > 0)
		{
		  VariableSerializers variableSerializers = Context.ProcessEngineConfiguration.VariableSerializers;
		  foreach (QueryVariableValue queryVariableValue in filterVariables)
		  {
			queryVariableValue.initialize(variableSerializers);
		  }
		}
	  }

	  public virtual bool LocalVariables
	  {
		  get
		  {
			return localVariables;
		  }
		  set
		  {
			this.localVariables = value;
		  }
	  }


	}

}