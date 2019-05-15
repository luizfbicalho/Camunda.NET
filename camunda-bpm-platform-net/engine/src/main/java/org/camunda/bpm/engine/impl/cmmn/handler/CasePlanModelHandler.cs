﻿using System.Collections.Generic;

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
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using PlanItemDefinition = org.camunda.bpm.model.cmmn.instance.PlanItemDefinition;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CasePlanModelHandler : StageItemHandler
	{

	  protected internal override PlanItemDefinition getDefinition(CmmnElement element)
	  {
		return (PlanItemDefinition) element;
	  }

	  protected internal override IList<string> getStandardEvents(CmmnElement element)
	  {
		return CASE_PLAN_MODEL_EVENTS;
	  }

	  public virtual void initializeExitCriterias(CasePlanModel casePlanModel, CmmnActivity activity, CmmnHandlerContext context)
	  {
		ICollection<Sentry> exitCriterias = casePlanModel.ExitCriteria;
		foreach (Sentry sentry in exitCriterias)
		{
		  string sentryId = sentry.Id;
		  CmmnSentryDeclaration sentryDeclaration = activity.getSentry(sentryId);
		  activity.addExitCriteria(sentryDeclaration);
		}
	  }

	}

}