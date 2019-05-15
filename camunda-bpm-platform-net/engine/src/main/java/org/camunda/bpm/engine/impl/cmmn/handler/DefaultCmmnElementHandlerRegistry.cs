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
namespace org.camunda.bpm.engine.impl.cmmn.handler
{

	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
	using PlanItemDefinition = org.camunda.bpm.model.cmmn.instance.PlanItemDefinition;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DefaultCmmnElementHandlerRegistry
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<Class, CmmnElementHandler<? extends org.camunda.bpm.model.cmmn.instance.CmmnElement, ? extends org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity>> definitionElementHandlers;
	  protected internal IDictionary<Type, CmmnElementHandler<CmmnElement, ? extends CmmnActivity>> definitionElementHandlers;
	  protected internal IDictionary<Type, ItemHandler> planItemElementHandlers;
	  protected internal IDictionary<Type, ItemHandler> discretionaryElementHandlers;

	  protected internal CaseHandler caseHandler = new CaseHandler();

	  protected internal StageItemHandler stagePlanItemHandler = new StageItemHandler();
	  protected internal CasePlanModelHandler casePlanModelHandler = new CasePlanModelHandler();
	  protected internal TaskItemHandler taskPlanItemHandler = new TaskItemHandler();
	  protected internal HumanTaskItemHandler humanTaskPlanItemHandler = new HumanTaskItemHandler();
	  protected internal ProcessTaskItemHandler processTaskPlanItemHandler = new ProcessTaskItemHandler();
	  protected internal CaseTaskItemHandler caseTaskPlanItemHandler = new CaseTaskItemHandler();
	  protected internal DecisionTaskItemHandler decisionTaskPlanItemHandler = new DecisionTaskItemHandler();
	  protected internal MilestoneItemHandler milestonePlanItemHandler = new MilestoneItemHandler();
	  protected internal EventListenerItemHandler eventListenerPlanItemHandler = new EventListenerItemHandler();

	  protected internal StageItemHandler stageDiscretionaryItemHandler = new StageItemHandler();
	  protected internal HumanTaskItemHandler humanTaskDiscretionaryItemHandler = new HumanTaskItemHandler();

	  protected internal SentryHandler sentryHandler = new SentryHandler();

	  public DefaultCmmnElementHandlerRegistry()
	  {

		// init definition element handler
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: definitionElementHandlers = new java.util.HashMap<Class, CmmnElementHandler<? extends org.camunda.bpm.model.cmmn.instance.CmmnElement, ? extends org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity>>();
		definitionElementHandlers = new Dictionary<Type, CmmnElementHandler<CmmnElement, ? extends CmmnActivity>>();

		definitionElementHandlers[typeof(Case)] = caseHandler;

		// init plan item element handler
		planItemElementHandlers = new Dictionary<Type, ItemHandler>();

		planItemElementHandlers[typeof(Stage)] = stagePlanItemHandler;
		planItemElementHandlers[typeof(CasePlanModel)] = casePlanModelHandler;
		planItemElementHandlers[typeof(Task)] = taskPlanItemHandler;
		planItemElementHandlers[typeof(HumanTask)] = humanTaskPlanItemHandler;
		planItemElementHandlers[typeof(ProcessTask)] = processTaskPlanItemHandler;
		planItemElementHandlers[typeof(DecisionTask)] = decisionTaskPlanItemHandler;
		planItemElementHandlers[typeof(CaseTask)] = caseTaskPlanItemHandler;
		planItemElementHandlers[typeof(Milestone)] = milestonePlanItemHandler;

		// Note: EventListener is currently not supported!
		// planItemElementHandlers.put(EventListener.class, eventListenerPlanItemHandler);

		// init discretionary element handler
		discretionaryElementHandlers = new Dictionary<Type, ItemHandler>();

		discretionaryElementHandlers[typeof(Stage)] = stageDiscretionaryItemHandler;
		discretionaryElementHandlers[typeof(HumanTask)] = humanTaskDiscretionaryItemHandler;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.Map<Class, CmmnElementHandler<? extends org.camunda.bpm.model.cmmn.instance.CmmnElement, ? extends org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity>> getDefinitionElementHandlers()
	  public virtual IDictionary<Type, CmmnElementHandler<CmmnElement, ? extends CmmnActivity>> DefinitionElementHandlers
	  {
		  get
		  {
			return definitionElementHandlers;
		  }
		  set
		  {
			this.definitionElementHandlers = value;
		  }
	  }


	  public virtual IDictionary<Type, ItemHandler> PlanItemElementHandlers
	  {
		  get
		  {
			return planItemElementHandlers;
		  }
		  set
		  {
			this.planItemElementHandlers = value;
		  }
	  }


	  public virtual IDictionary<Type, ItemHandler> DiscretionaryElementHandlers
	  {
		  get
		  {
			return discretionaryElementHandlers;
		  }
		  set
		  {
			this.discretionaryElementHandlers = value;
		  }
	  }


	  public virtual SentryHandler SentryHandler
	  {
		  get
		  {
			return sentryHandler;
		  }
		  set
		  {
			this.sentryHandler = value;
		  }
	  }


	}

}