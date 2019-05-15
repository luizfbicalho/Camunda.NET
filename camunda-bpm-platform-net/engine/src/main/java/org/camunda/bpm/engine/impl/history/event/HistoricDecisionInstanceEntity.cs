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
namespace org.camunda.bpm.engine.impl.history.@event
{

	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;

	/// <summary>
	/// History entry for an evaluated decision.
	/// 
	/// @author Philipp Ossler
	/// @author Ingo Richtsmeier
	/// 
	/// </summary>
	[Serializable]
	public class HistoricDecisionInstanceEntity : HistoryEvent, HistoricDecisionInstance
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  private const long serialVersionUID = 1L;

	  protected internal string decisionDefinitionId;
	  protected internal string decisionDefinitionKey;
	  protected internal string decisionDefinitionName;

	  protected internal string activityInstanceId;
	  protected internal string activityId;

	  protected internal DateTime evaluationTime;

	  protected internal double? collectResultValue;

	  protected internal string rootDecisionInstanceId;
	  protected internal string decisionRequirementsDefinitionId;
	  protected internal string decisionRequirementsDefinitionKey;

	  protected internal string userId;
	  protected internal string tenantId;

	  protected internal IList<HistoricDecisionInputInstance> inputs;
	  protected internal IList<HistoricDecisionOutputInstance> outputs;

	  public virtual string DecisionDefinitionId
	  {
		  get
		  {
			return decisionDefinitionId;
		  }
		  set
		  {
			this.decisionDefinitionId = value;
		  }
	  }


	  public virtual string DecisionDefinitionKey
	  {
		  get
		  {
			return decisionDefinitionKey;
		  }
		  set
		  {
			this.decisionDefinitionKey = value;
		  }
	  }


	  public virtual string DecisionDefinitionName
	  {
		  get
		  {
			return decisionDefinitionName;
		  }
		  set
		  {
			this.decisionDefinitionName = value;
		  }
	  }


	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual DateTime EvaluationTime
	  {
		  get
		  {
			return evaluationTime;
		  }
		  set
		  {
			this.evaluationTime = value;
		  }
	  }


	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			this.userId = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual IList<HistoricDecisionInputInstance> Inputs
	  {
		  get
		  {
			if (inputs != null)
			{
			  return inputs;
			}
			else
			{
			  throw LOG.historicDecisionInputInstancesNotFetchedException();
			}
		  }
		  set
		  {
			this.inputs = value;
		  }
	  }

	  public virtual IList<HistoricDecisionOutputInstance> Outputs
	  {
		  get
		  {
			if (outputs != null)
			{
			  return outputs;
			}
			else
			{
			  throw LOG.historicDecisionOutputInstancesNotFetchedException();
			}
		  }
		  set
		  {
			this.outputs = value;
		  }
	  }



	  public virtual void delete()
	  {
		Context.CommandContext.DbEntityManager.delete(this);
	  }

	  public virtual void addInput(HistoricDecisionInputInstance decisionInputInstance)
	  {
		if (inputs == null)
		{
		  inputs = new List<HistoricDecisionInputInstance>();
		}
		inputs.Add(decisionInputInstance);
	  }

	  public virtual void addOutput(HistoricDecisionOutputInstance decisionOutputInstance)
	  {
		if (outputs == null)
		{
		  outputs = new List<HistoricDecisionOutputInstance>();
		}
		outputs.Add(decisionOutputInstance);
	  }

	  public virtual double? CollectResultValue
	  {
		  get
		  {
			return collectResultValue;
		  }
		  set
		  {
			this.collectResultValue = value;
		  }
	  }


	  public virtual string RootDecisionInstanceId
	  {
		  get
		  {
			return rootDecisionInstanceId;
		  }
		  set
		  {
			this.rootDecisionInstanceId = value;
		  }
	  }


	  public virtual string DecisionRequirementsDefinitionId
	  {
		  get
		  {
			return decisionRequirementsDefinitionId;
		  }
		  set
		  {
			this.decisionRequirementsDefinitionId = value;
		  }
	  }


	  public virtual string DecisionRequirementsDefinitionKey
	  {
		  get
		  {
			return decisionRequirementsDefinitionKey;
		  }
		  set
		  {
			this.decisionRequirementsDefinitionKey = value;
		  }
	  }

	}

}