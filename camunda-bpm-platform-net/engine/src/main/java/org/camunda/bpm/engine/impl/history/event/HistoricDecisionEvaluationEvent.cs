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
namespace org.camunda.bpm.engine.impl.history.@event
{

	/// <summary>
	/// Container for history entities which belongs to a decision evaluation. Only
	/// the containing history entities should be persisted.
	/// </summary>
	[Serializable]
	public class HistoricDecisionEvaluationEvent : HistoryEvent
	{

	  private const long serialVersionUID = 1L;

	  protected internal HistoricDecisionInstanceEntity rootHistoricDecisionInstance;

	  protected internal ICollection<HistoricDecisionInstanceEntity> requiredHistoricDecisionInstances = new List<HistoricDecisionInstanceEntity>();

	  public virtual HistoricDecisionInstanceEntity RootHistoricDecisionInstance
	  {
		  get
		  {
			return rootHistoricDecisionInstance;
		  }
		  set
		  {
			this.rootHistoricDecisionInstance = value;
		  }
	  }


	  public virtual ICollection<HistoricDecisionInstanceEntity> RequiredHistoricDecisionInstances
	  {
		  get
		  {
			return requiredHistoricDecisionInstances;
		  }
		  set
		  {
			this.requiredHistoricDecisionInstances = value;
		  }
	  }


	}

}