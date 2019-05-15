﻿using System;

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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class BpmnParseLogger : ProcessEngineLogger
	{

	  // LOGGING

	  public virtual void parsingElement(string elementType, string elementId)
	  {
		logDebug("001", "Parsing element from type '{}' with id '{}'", elementType, elementId);
	  }

	  public virtual void ignoringNonExecutableProcess(string elementId)
	  {
		logInfo("002", "Ignoring non-executable process with id '{}'. Set the attribute isExecutable=\"true\" to deploy " + "this process.", elementId);
	  }

	  public virtual void missingIsExecutableAttribute(string elementId)
	  {
		logInfo("003", "Process with id '{}' has no attribute isExecutable. Better set the attribute explicitly, " + "especially to be compatible with future engine versions which might change the default behavior.", elementId);
	  }

	  public virtual void parsingFailure(Exception cause)
	  {
		logError("004", "Unexpected Exception with message: {} ", cause.Message);
	  }

	  // EXCEPTIONS

	  public virtual ProcessEngineException parsingProcessException(Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("009", "Error while parsing process. {}.", cause.Message), cause);
	  }

	  public virtual void exceptionWhileGeneratingProcessDiagram(Exception t)
	  {
		logError("010", "Error while generating process diagram, image will not be stored in repository", t);
	  }

	  public virtual ProcessEngineException messageEventSubscriptionWithSameNameExists(string resourceName, string eventName)
	  {
		throw new ProcessEngineException(exceptionMessage("011", "Cannot deploy process definition '{}': there already is a message event subscription for the message with name '{}'.", resourceName, eventName));
	  }

	}

}