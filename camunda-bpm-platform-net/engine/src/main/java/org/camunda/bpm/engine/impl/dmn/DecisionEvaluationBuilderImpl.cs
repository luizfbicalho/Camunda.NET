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
namespace org.camunda.bpm.engine.impl.dmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;

	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DecisionsEvaluationBuilder = org.camunda.bpm.engine.dmn.DecisionsEvaluationBuilder;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using DecisionDefinitionNotFoundException = org.camunda.bpm.engine.exception.dmn.DecisionDefinitionNotFoundException;
	using EvaluateDecisionCmd = org.camunda.bpm.engine.impl.dmn.cmd.EvaluateDecisionCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	public class DecisionEvaluationBuilderImpl : DecisionsEvaluationBuilder
	{

	  private static readonly DecisionLogger LOG = ProcessEngineLogger.DECISION_LOGGER;

	  protected internal CommandExecutor commandExecutor;

	  protected internal string decisionDefinitionKey;
	  protected internal string decisionDefinitionId;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int? version_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IDictionary<string, object> variables_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string decisionDefinitionTenantId_Renamed;
	  protected internal bool isTenantIdSet = false;


	  public DecisionEvaluationBuilderImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  public virtual DecisionsEvaluationBuilder variables(IDictionary<string, object> variables)
	  {
		this.variables_Renamed = variables;
		return this;
	  }

	  public virtual DecisionsEvaluationBuilder version(int? version)
	  {
		this.version_Renamed = version;
		return this;
	  }

	  public virtual DecisionsEvaluationBuilder decisionDefinitionTenantId(string tenantId)
	  {
		this.decisionDefinitionTenantId_Renamed = tenantId;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual DecisionsEvaluationBuilder decisionDefinitionWithoutTenantId()
	  {
		this.decisionDefinitionTenantId_Renamed = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual DmnDecisionResult evaluate()
	  {
		 ensureOnlyOneNotNull(typeof(NotValidException), "either decision definition id or key must be set", decisionDefinitionId, decisionDefinitionKey);

		 if (isTenantIdSet && !string.ReferenceEquals(decisionDefinitionId, null))
		 {
		   throw LOG.exceptionEvaluateDecisionDefinitionByIdAndTenantId();
		 }

		try
		{
		  return commandExecutor.execute(new EvaluateDecisionCmd(this));
		}
		catch (NullValueException e)
		{
		  throw new NotValidException(e.Message, e);
		}
		catch (DecisionDefinitionNotFoundException e)
		{
		  throw new NotFoundException(e.Message, e);
		}
	  }

	  public static DecisionsEvaluationBuilder evaluateDecisionByKey(CommandExecutor commandExecutor, string decisionDefinitionKey)
	  {
		DecisionEvaluationBuilderImpl builder = new DecisionEvaluationBuilderImpl(commandExecutor);
		builder.decisionDefinitionKey = decisionDefinitionKey;
		return builder;
	  }

	  public static DecisionsEvaluationBuilder evaluateDecisionById(CommandExecutor commandExecutor, string decisionDefinitionId)
	  {
		DecisionEvaluationBuilderImpl builder = new DecisionEvaluationBuilderImpl(commandExecutor);
		builder.decisionDefinitionId = decisionDefinitionId;
		return builder;
	  }

	  // getters ////////////////////////////////////

	  public virtual string DecisionDefinitionKey
	  {
		  get
		  {
			return decisionDefinitionKey;
		  }
	  }

	  public virtual string DecisionDefinitionId
	  {
		  get
		  {
			return decisionDefinitionId;
		  }
	  }

	  public virtual int? Version
	  {
		  get
		  {
			return version_Renamed;
		  }
	  }

	  public virtual IDictionary<string, object> Variables
	  {
		  get
		  {
			return variables_Renamed;
		  }
	  }

	  public virtual string DecisionDefinitionTenantId
	  {
		  get
		  {
			return decisionDefinitionTenantId_Renamed;
		  }
	  }

	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	}

}