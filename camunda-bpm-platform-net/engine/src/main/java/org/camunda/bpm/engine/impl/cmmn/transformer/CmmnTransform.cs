using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.impl.cmmn.transformer
{

	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CasePlanModelHandler = org.camunda.bpm.engine.impl.cmmn.handler.CasePlanModelHandler;
	using CmmnElementHandler = org.camunda.bpm.engine.impl.cmmn.handler.CmmnElementHandler;
	using CmmnHandlerContext = org.camunda.bpm.engine.impl.cmmn.handler.CmmnHandlerContext;
	using DefaultCmmnElementHandlerRegistry = org.camunda.bpm.engine.impl.cmmn.handler.DefaultCmmnElementHandlerRegistry;
	using ItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler;
	using SentryHandler = org.camunda.bpm.engine.impl.cmmn.handler.SentryHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using Transform = org.camunda.bpm.engine.impl.core.transformer.Transform;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using CmmnModelException = org.camunda.bpm.model.cmmn.CmmnModelException;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;
	using Definitions = org.camunda.bpm.model.cmmn.instance.Definitions;
	using EventListener = org.camunda.bpm.model.cmmn.instance.EventListener;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
	using PlanFragment = org.camunda.bpm.model.cmmn.instance.PlanFragment;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using PlanItemDefinition = org.camunda.bpm.model.cmmn.instance.PlanItemDefinition;
	using PlanningTable = org.camunda.bpm.model.cmmn.instance.PlanningTable;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CmmnTransform : Transform<CaseDefinitionEntity>
	{

	  protected internal static readonly CmmnTransformerLogger LOG = ProcessEngineLogger.CMMN_TRANSFORMER_LOGGER;

	  protected internal CmmnTransformer transformer;

	  protected internal ExpressionManager expressionManager;
	  protected internal DefaultCmmnElementHandlerRegistry handlerRegistry;
	  protected internal IList<CmmnTransformListener> transformListeners;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ResourceEntity resource_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DeploymentEntity deployment_Renamed;

	  protected internal CmmnModelInstance model;
	  protected internal CmmnHandlerContext context = new CmmnHandlerContext();
	  protected internal IList<CaseDefinitionEntity> caseDefinitions = new List<CaseDefinitionEntity>();

	  public CmmnTransform(CmmnTransformer transformer)
	  {
		this.transformer = transformer;
		this.expressionManager = transformer.ExpressionManager;
		this.handlerRegistry = transformer.CmmnElementHandlerRegistry;
		this.transformListeners = transformer.TransformListeners;
	  }

	  public virtual CmmnTransform deployment(DeploymentEntity deployment)
	  {
		this.deployment_Renamed = deployment;
		return this;
	  }

	  public virtual CmmnTransform resource(ResourceEntity resource)
	  {
		this.resource_Renamed = resource;
		return this;
	  }

	  public virtual IList<CaseDefinitionEntity> transform()
	  {
		// get name of resource
		string resourceName = resource_Renamed.Name;

		// create an input stream
		sbyte[] bytes = resource_Renamed.Bytes;
		MemoryStream inputStream = new MemoryStream(bytes);

		try
		{
		  // read input stream
		  model = Cmmn.readModelFromStream(inputStream);

		}
		catch (CmmnModelException e)
		{
		  throw LOG.transformResourceException(resourceName, e);
		}

		// TODO: use model API to validate (ie.
		// semantic and execution validation) model

		context.Model = model;
		context.Deployment = deployment_Renamed;
		context.ExpressionManager = expressionManager;

		try
		{

		   transformRootElement();

		}
		catch (Exception e)
		{
		  // ALL unexpected exceptions should bubble up since they are not handled
		  // accordingly by underlying parse-methods and the process can't be deployed
		  throw LOG.parseProcessException(resourceName, e);
		}

		return caseDefinitions;
	  }

	  protected internal virtual void transformRootElement()
	  {

		transformImports();
		transformCaseDefinitions();

		Definitions definitions = model.Definitions;
		foreach (CmmnTransformListener transformListener in transformListeners)
		{
		  transformListener.transformRootElement(definitions, caseDefinitions);
		}

	  }

	  protected internal virtual void transformImports()
	  {
		// not implemented yet
	  }

	  protected internal virtual void transformCaseDefinitions()
	  {
		Definitions definitions = model.Definitions;

		ICollection<Case> cases = definitions.Cases;

		foreach (Case currentCase in cases)
		{
		  context.CaseDefinition = null;
		  context.Parent = null;
		  CmmnCaseDefinition caseDefinition = transformCase(currentCase);
		  caseDefinitions.Add((CaseDefinitionEntity) caseDefinition);
		}
	  }

	  protected internal virtual CaseDefinitionEntity transformCase(Case element)
	  {
		// get CaseTransformer
		CmmnElementHandler<Case, CmmnActivity> caseTransformer = getDefinitionHandler(typeof(Case));
		CmmnActivity definition = caseTransformer.handleElement(element, context);

		context.CaseDefinition = (CmmnCaseDefinition) definition;
		context.Parent = definition;

		CasePlanModel casePlanModel = element.CasePlanModel;
		transformCasePlanModel(casePlanModel);

		foreach (CmmnTransformListener transformListener in transformListeners)
		{
		  transformListener.transformCase(element, (CmmnCaseDefinition) definition);
		}

		return (CaseDefinitionEntity) definition;
	  }

	  protected internal virtual void transformCasePlanModel(CasePlanModel casePlanModel)
	  {
		CasePlanModelHandler transformer = (CasePlanModelHandler) getPlanItemHandler(typeof(CasePlanModel));
		CmmnActivity newActivity = transformer.handleElement(casePlanModel, context);
		context.Parent = newActivity;

		transformStage(casePlanModel, newActivity);

		context.Parent = newActivity;
		transformer.initializeExitCriterias(casePlanModel, newActivity, context);

		foreach (CmmnTransformListener transformListener in transformListeners)
		{
		  transformListener.transformCasePlanModel((org.camunda.bpm.model.cmmn.impl.instance.CasePlanModel) casePlanModel, newActivity);
		}
	  }

	  protected internal virtual void transformStage(Stage stage, CmmnActivity parent)
	  {

		context.Parent = parent;

		// transform a sentry with it ifPart (onParts will
		// not be transformed in this step)
		transformSentries(stage);

		// transform planItems
		transformPlanItems(stage, parent);

		// transform the onParts of the existing sentries
		transformSentryOnParts(stage);

		// parse planningTable (not yet implemented)
		transformPlanningTable(stage.PlanningTable, parent);

	  }

	  protected internal virtual void transformPlanningTable(PlanningTable planningTable, CmmnActivity parent)
	  {
		// not yet implemented.

		// TODO: think about how to organize the planning tables! A tableItem or planningTable
		// can have "applicabilityRules": If the rule evaluates to "true" the the tableItem or
		// planningTable is applicable for planning otherwise it is not.
	  }

	  protected internal virtual void transformSentries(Stage stage)
	  {
		ICollection<Sentry> sentries = stage.Sentrys;

		if (sentries != null && sentries.Count > 0)
		{
		  SentryHandler handler = SentryHandler;
		  foreach (Sentry sentry in sentries)
		  {
			handler.handleElement(sentry, context);
		  }
		}
	  }

	  protected internal virtual void transformSentryOnParts(Stage stage)
	  {
		ICollection<Sentry> sentries = stage.Sentrys;

		if (sentries != null && sentries.Count > 0)
		{
		  SentryHandler handler = SentryHandler;
		  foreach (Sentry sentry in sentries)
		  {
			handler.initializeOnParts(sentry, context);
			// sentry fully transformed -> call transform listener
			CmmnSentryDeclaration sentryDeclaration = context.Parent.getSentry(sentry.Id);
			foreach (CmmnTransformListener transformListener in transformListeners)
			{
			  transformListener.transformSentry(sentry, sentryDeclaration);
			}
		  }
		}
	  }

	  protected internal virtual void transformPlanItems(PlanFragment planFragment, CmmnActivity parent)
	  {
		ICollection<PlanItem> planItems = planFragment.PlanItems;

		foreach (PlanItem planItem in planItems)
		{
		  transformPlanItem(planItem, parent);
		}

	  }

	  protected internal virtual void transformPlanItem(PlanItem planItem, CmmnActivity parent)
	  {
		PlanItemDefinition definition = planItem.Definition;

		ItemHandler planItemTransformer = null;

		if (definition is HumanTask)
		{
		  planItemTransformer = getPlanItemHandler(typeof(HumanTask));
		}
		else if (definition is ProcessTask)
		{
		  planItemTransformer = getPlanItemHandler(typeof(ProcessTask));
		}
		else if (definition is CaseTask)
		{
		  planItemTransformer = getPlanItemHandler(typeof(CaseTask));
		}
		else if (definition is DecisionTask)
		{
		  planItemTransformer = getPlanItemHandler(typeof(DecisionTask));
		}
		else if (definition is Task)
		{
		  planItemTransformer = getPlanItemHandler(typeof(Task));
		}
		else if (definition is Stage)
		{
		  planItemTransformer = getPlanItemHandler(typeof(Stage));
		}
		else if (definition is Milestone)
		{
		  planItemTransformer = getPlanItemHandler(typeof(Milestone));
		}
		else if (definition is EventListener)
		{
		  planItemTransformer = getPlanItemHandler(typeof(EventListener));
		}

		if (planItemTransformer != null)
		{
		  CmmnActivity newActivity = planItemTransformer.handleElement(planItem, context);

		  if (definition is Stage)
		  {
			Stage stage = (Stage) definition;
			transformStage(stage, newActivity);
			context.Parent = parent;

		  }
		  else if (definition is HumanTask)
		  {
			HumanTask humanTask = (HumanTask) definition;

			// According to the specification: A HumanTask can only contain
			// one planningTable, the XSD allows multiple planningTables!
			ICollection<PlanningTable> planningTables = humanTask.PlanningTables;
			foreach (PlanningTable planningTable in planningTables)
			{
			  transformPlanningTable(planningTable, parent);
			}

		  }

		  foreach (CmmnTransformListener transformListener in transformListeners)
		  {
			if (definition is HumanTask)
			{
			  transformListener.transformHumanTask(planItem, (HumanTask) definition, newActivity);
			}
			else if (definition is ProcessTask)
			{
			  transformListener.transformProcessTask(planItem, (ProcessTask) definition, newActivity);
			}
			else if (definition is CaseTask)
			{
			  transformListener.transformCaseTask(planItem, (CaseTask) definition, newActivity);
			}
			else if (definition is DecisionTask)
			{
			  transformListener.transformDecisionTask(planItem, (DecisionTask) definition, newActivity);
			}
			else if (definition is Task)
			{
			  transformListener.transformTask(planItem, (Task) definition, newActivity);
			}
			else if (definition is Stage)
			{
			  transformListener.transformStage(planItem, (Stage) definition, newActivity);
			}
			else if (definition is Milestone)
			{
			  transformListener.transformMilestone(planItem, (Milestone) definition, newActivity);
			}
			else if (definition is EventListener)
			{
			  transformListener.transformEventListener(planItem, (EventListener) definition, newActivity);
			}
		  }
		}
	  }

	  // getter/setter ////////////////////////////////////////////////////////////////////

	  public virtual DeploymentEntity Deployment
	  {
		  get
		  {
			return deployment_Renamed;
		  }
		  set
		  {
			this.deployment_Renamed = value;
		  }
	  }


	  public virtual ResourceEntity Resource
	  {
		  get
		  {
			return resource_Renamed;
		  }
		  set
		  {
			this.resource_Renamed = value;
		  }
	  }


	  public virtual DefaultCmmnElementHandlerRegistry HandlerRegistry
	  {
		  get
		  {
			return handlerRegistry;
		  }
		  set
		  {
			this.handlerRegistry = value;
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <V extends org.camunda.bpm.model.cmmn.instance.CmmnElement> org.camunda.bpm.engine.impl.cmmn.handler.CmmnElementHandler<V, org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity> getDefinitionHandler(Class<V> cls)
	  protected internal virtual CmmnElementHandler<V, CmmnActivity> getDefinitionHandler<V>(Type cls) where V : org.camunda.bpm.model.cmmn.instance.CmmnElement
	  {
			  cls = typeof(V);
		return (CmmnElementHandler<V, CmmnActivity>) HandlerRegistry.DefinitionElementHandlers[cls];
	  }

	  protected internal virtual ItemHandler getPlanItemHandler(Type cls)
	  {
		return HandlerRegistry.PlanItemElementHandlers[cls];
	  }

	  protected internal virtual ItemHandler getDiscretionaryItemHandler(Type cls)
	  {
		return HandlerRegistry.DiscretionaryElementHandlers[cls];
	  }

	  protected internal virtual SentryHandler SentryHandler
	  {
		  get
		  {
			return HandlerRegistry.SentryHandler;
		  }
	  }

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	}

}