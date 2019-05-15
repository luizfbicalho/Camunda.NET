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
namespace org.camunda.bpm.engine.test.standalone.deploy
{

	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using CmmnTransformListener = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformListener;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using CmmnModelElementInstance = org.camunda.bpm.model.cmmn.instance.CmmnModelElementInstance;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;
	using Definitions = org.camunda.bpm.model.cmmn.instance.Definitions;
	using EventListener = org.camunda.bpm.model.cmmn.instance.EventListener;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class TestCmmnTransformListener : CmmnTransformListener
	{

	  public static ISet<CmmnModelElementInstance> modelElementInstances = new HashSet<CmmnModelElementInstance>();
	  public static ISet<CmmnActivity> cmmnActivities = new HashSet<CmmnActivity>();
	  public static ISet<CmmnSentryDeclaration> sentryDeclarations = new HashSet<CmmnSentryDeclaration>();

	  public virtual void transformRootElement<T1>(Definitions definitions, IList<T1> caseDefinitions) where T1 : org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition
	  {
		modelElementInstances.Add(definitions);
		foreach (CmmnCaseDefinition caseDefinition in caseDefinitions)
		{
		  CaseDefinitionEntity entity = (CaseDefinitionEntity) caseDefinition;
		  entity.Key = entity.Key + "-modified";
		}
	  }

	  public virtual void transformCase(Case element, CmmnCaseDefinition caseDefinition)
	  {
		modelElementInstances.Add(element);
		cmmnActivities.Add(caseDefinition);
	  }

	  public virtual void transformCasePlanModel(org.camunda.bpm.model.cmmn.impl.instance.CasePlanModel casePlanModel, CmmnActivity caseActivity)
	  {
		transformCasePlanModel((CasePlanModel) casePlanModel, caseActivity);
	  }

	  public virtual void transformCasePlanModel(CasePlanModel casePlanModel, CmmnActivity activity)
	  {
		modelElementInstances.Add(casePlanModel);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformHumanTask(PlanItem planItem, HumanTask humanTask, CmmnActivity activity)
	  {
		modelElementInstances.Add(planItem);
		modelElementInstances.Add(humanTask);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformProcessTask(PlanItem planItem, ProcessTask processTask, CmmnActivity activity)
	  {
		modelElementInstances.Add(planItem);
		modelElementInstances.Add(processTask);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformCaseTask(PlanItem planItem, CaseTask caseTask, CmmnActivity activity)
	  {
		modelElementInstances.Add(planItem);
		modelElementInstances.Add(caseTask);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformDecisionTask(PlanItem planItem, DecisionTask decisionTask, CmmnActivity activity)
	  {
		modelElementInstances.Add(planItem);
		modelElementInstances.Add(decisionTask);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformTask(PlanItem planItem, Task task, CmmnActivity activity)
	  {
		modelElementInstances.Add(planItem);
		modelElementInstances.Add(task);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformStage(PlanItem planItem, Stage stage, CmmnActivity activity)
	  {
		modelElementInstances.Add(planItem);
		modelElementInstances.Add(stage);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformMilestone(PlanItem planItem, Milestone milestone, CmmnActivity activity)
	  {
		modelElementInstances.Add(planItem);
		modelElementInstances.Add(milestone);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformEventListener(PlanItem planItem, EventListener eventListener, CmmnActivity activity)
	  {
		modelElementInstances.Add(planItem);
		modelElementInstances.Add(eventListener);
		cmmnActivities.Add(activity);
	  }

	  public virtual void transformSentry(Sentry sentry, CmmnSentryDeclaration sentryDeclaration)
	  {
		modelElementInstances.Add(sentry);
		sentryDeclarations.Add(sentryDeclaration);
	  }

	  protected internal virtual string getNewName(string name)
	  {
		if (name.EndsWith("-modified", StringComparison.Ordinal))
		{
		  return name + "-again";
		}
		else
		{
		  return name + "-modified";
		}
	  }

	  public static void reset()
	  {
		modelElementInstances = new HashSet<CmmnModelElementInstance>();
		cmmnActivities = new HashSet<CmmnActivity>();
		sentryDeclarations = new HashSet<CmmnSentryDeclaration>();
	  }

	  public static int numberOfRegistered(Type modelElementInstanceClass)
	  {
		int count = 0;
		foreach (CmmnModelElementInstance element in modelElementInstances)
		{
		  if (modelElementInstanceClass.IsInstanceOfType(element))
		  {
			count++;
		  }
		}
		return count;
	  }

	}

}