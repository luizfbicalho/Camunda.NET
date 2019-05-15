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
namespace org.camunda.bpm.engine.impl.cmmn.transformer
{

	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
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
	/// Listener which can be registered within the engine to receive events during transforming (and
	/// maybe influence it). Instead of implementing this interface you might consider to extend
	/// the <seealso cref="AbstractCmmnTransformListener"/>, which contains an empty implementation for all methods
	/// and makes your implementation easier and more robust to future changes.
	/// 
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public interface CmmnTransformListener
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: void transformRootElement(org.camunda.bpm.model.cmmn.instance.Definitions definitions, java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition> caseDefinitions);
	  void transformRootElement<T1>(Definitions definitions, IList<T1> caseDefinitions);

	  void transformCase(Case element, CmmnCaseDefinition caseDefinition);

	  /// @deprecated use <seealso cref="#transformCasePlanModel(org.camunda.bpm.model.cmmn.instance.CasePlanModel, CmmnActivity)"/> 
	  [Obsolete("use <seealso cref="#transformCasePlanModel(org.camunda.bpm.model.cmmn.instance.CasePlanModel, org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity)"/>")]
	  void transformCasePlanModel(org.camunda.bpm.model.cmmn.impl.instance.CasePlanModel casePlanModel, CmmnActivity caseActivity);

	  void transformCasePlanModel(CasePlanModel casePlanModel, CmmnActivity caseActivity);

	  void transformHumanTask(PlanItem planItem, HumanTask humanTask, CmmnActivity caseActivity);

	  void transformProcessTask(PlanItem planItem, ProcessTask processTask, CmmnActivity caseActivity);

	  void transformCaseTask(PlanItem planItem, CaseTask caseTask, CmmnActivity caseActivity);

	  void transformDecisionTask(PlanItem planItem, DecisionTask decisionTask, CmmnActivity caseActivity);

	  void transformTask(PlanItem planItem, Task task, CmmnActivity caseActivity);

	  void transformStage(PlanItem planItem, Stage stage, CmmnActivity caseActivity);

	  void transformMilestone(PlanItem planItem, Milestone milestone, CmmnActivity caseActivity);

	  void transformEventListener(PlanItem planItem, EventListener eventListener, CmmnActivity caseActivity);

	  void transformSentry(Sentry sentry, CmmnSentryDeclaration sentryDeclaration);

	}

}