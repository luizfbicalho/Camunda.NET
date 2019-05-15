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
namespace org.camunda.bpm.engine.impl.core.model
{
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;

	public class BaseCallableElement
	{

	  protected internal ParameterValueProvider definitionKeyValueProvider;
	  protected internal CallableElementBinding binding;
	  protected internal ParameterValueProvider versionValueProvider;
	  protected internal ParameterValueProvider versionTagValueProvider;
	  protected internal ParameterValueProvider tenantIdProvider;
	  protected internal string deploymentId;

	  public sealed class CallableElementBinding
	  {
		public static readonly CallableElementBinding LATEST = new CallableElementBinding("LATEST", InnerEnum.LATEST, "latest");
		public static readonly CallableElementBinding DEPLOYMENT = new CallableElementBinding("DEPLOYMENT", InnerEnum.DEPLOYMENT, "deployment");
		public static readonly CallableElementBinding VERSION = new CallableElementBinding("VERSION", InnerEnum.VERSION, "version");
		public static readonly CallableElementBinding VERSION_TAG = new CallableElementBinding("VERSION_TAG", InnerEnum.VERSION_TAG, "versionTag");

		private static readonly IList<CallableElementBinding> valueList = new List<CallableElementBinding>();

		static CallableElementBinding()
		{
			valueList.Add(LATEST);
			valueList.Add(DEPLOYMENT);
			valueList.Add(VERSION);
			valueList.Add(VERSION_TAG);
		}

		public enum InnerEnum
		{
			LATEST,
			DEPLOYMENT,
			VERSION,
			VERSION_TAG
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		internal string value;

		internal CallableElementBinding(string name, InnerEnum innerEnum, string value)
		{
		  this.value = value;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public string Value
		{
			get
			{
			  return value;
			}
		}

		  public static IList<CallableElementBinding> values()
		  {
			  return valueList;
		  }

		  public int ordinal()
		  {
			  return ordinalValue;
		  }

		  public override string ToString()
		  {
			  return nameValue;
		  }

		  public static CallableElementBinding valueOf(string name)
		  {
			  foreach (CallableElementBinding enumInstance in CallableElementBinding.valueList)
			  {
				  if (enumInstance.nameValue == name)
				  {
					  return enumInstance;
				  }
			  }
			  throw new System.ArgumentException(name);
		  }
	  }

	  public virtual string getDefinitionKey(VariableScope variableScope)
	  {
		object result = definitionKeyValueProvider.getValue(variableScope);

		if (result != null && !(result is string))
		{
		  throw new System.InvalidCastException("Cannot cast '" + result + "' to String");
		}

		return (string) result;
	  }

	  public virtual ParameterValueProvider DefinitionKeyValueProvider
	  {
		  get
		  {
			return definitionKeyValueProvider;
		  }
		  set
		  {
			this.definitionKeyValueProvider = value;
		  }
	  }


	  public virtual CallableElementBinding Binding
	  {
		  get
		  {
			return binding;
		  }
		  set
		  {
			this.binding = value;
		  }
	  }


	  public virtual bool LatestBinding
	  {
		  get
		  {
			CallableElementBinding binding = Binding;
			return binding == null || CallableElementBinding.LATEST.Equals(binding);
		  }
	  }

	  public virtual bool DeploymentBinding
	  {
		  get
		  {
			CallableElementBinding binding = Binding;
			return CallableElementBinding.DEPLOYMENT.Equals(binding);
		  }
	  }

	  public virtual bool VersionBinding
	  {
		  get
		  {
			CallableElementBinding binding = Binding;
			return CallableElementBinding.VERSION.Equals(binding);
		  }
	  }

	  public virtual bool VersionTagBinding
	  {
		  get
		  {
			CallableElementBinding binding = Binding;
			return CallableElementBinding.VERSION_TAG.Equals(binding);
		  }
	  }

	  public virtual int? getVersion(VariableScope variableScope)
	  {
		object result = versionValueProvider.getValue(variableScope);

		if (result != null)
		{
		  if (result is string)
		  {
			return Convert.ToInt32((string) result);
		  }
		  else if (result is int?)
		  {
			return (int?) result;
		  }
		  else
		  {
			throw new ProcessEngineException("It is not possible to transform '" + result + "' into an integer.");
		  }
		}

		return null;
	  }

	  public virtual ParameterValueProvider VersionValueProvider
	  {
		  get
		  {
			return versionValueProvider;
		  }
		  set
		  {
			this.versionValueProvider = value;
		  }
	  }


	  public virtual string getVersionTag(VariableScope variableScope)
	  {
		object result = versionTagValueProvider.getValue(variableScope);

		if (result != null)
		{
		  if (result is string)
		  {
			return (string) result;
		  }
		  else
		  {
			throw new ProcessEngineException("It is not possible to transform '" + result + "' into a string.");
		  }
		}

		return null;
	  }


	  public virtual ParameterValueProvider VersionTagValueProvider
	  {
		  get
		  {
			return versionTagValueProvider;
		  }
		  set
		  {
			this.versionTagValueProvider = value;
		  }
	  }


	  public virtual ParameterValueProvider TenantIdProvider
	  {
		  set
		  {
			this.tenantIdProvider = value;
		  }
		  get
		  {
			return tenantIdProvider;
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


	  public virtual string getDefinitionTenantId(VariableScope variableScope)
	  {
		return (string) tenantIdProvider.getValue(variableScope);
	  }


	}

}