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
namespace org.camunda.bpm.engine.impl.bpmn.helper
{

	using ErrorEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ErrorEventDefinition;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;

	public class ErrorDeclarationForProcessInstanceFinder : TreeVisitor<PvmScope>
	{
	  protected internal Exception exception;
	  protected internal string errorCode;
	  protected internal PvmActivity errorHandlerActivity;
	  protected internal ErrorEventDefinition errorEventDefinition;
	  protected internal PvmActivity currentActivity;

	  public ErrorDeclarationForProcessInstanceFinder(Exception exception, string errorCode, PvmActivity currentActivity)
	  {
		this.exception = exception;
		this.errorCode = errorCode;
		this.currentActivity = currentActivity;
	  }

	  public virtual void visit(PvmScope scope)
	  {
		IList<ErrorEventDefinition> errorEventDefinitions = scope.Properties.get(BpmnProperties.ERROR_EVENT_DEFINITIONS);
		foreach (ErrorEventDefinition errorEventDefinition in errorEventDefinitions)
		{
		  PvmActivity activityHandler = scope.ProcessDefinition.findActivity(errorEventDefinition.HandlerActivityId);
		  if ((!isReThrowingErrorEventSubprocess(activityHandler)) && ((exception != null && errorEventDefinition.catchesException(exception)) || (exception == null && errorEventDefinition.catchesError(errorCode))))
		  {

			errorHandlerActivity = activityHandler;
			this.errorEventDefinition = errorEventDefinition;
			break;
		  }
		}
	  }

	  protected internal virtual bool isReThrowingErrorEventSubprocess(PvmActivity activityHandler)
	  {
		ScopeImpl activityHandlerScope = (ScopeImpl)activityHandler;
		return activityHandlerScope.isAncestorFlowScopeOf((ScopeImpl)currentActivity);
	  }

	  public virtual PvmActivity ErrorHandlerActivity
	  {
		  get
		  {
			return errorHandlerActivity;
		  }
	  }

	  public virtual ErrorEventDefinition ErrorEventDefinition
	  {
		  get
		  {
			return errorEventDefinition;
		  }
	  }

	}

}