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
namespace org.camunda.bpm.engine
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class MismatchingMessageCorrelationException : ProcessEngineException
	{

	  private const long serialVersionUID = 1L;

	  public MismatchingMessageCorrelationException(string message) : base(message)
	  {
	  }

	  public MismatchingMessageCorrelationException(string messageName, string reason) : this("Cannot correlate message '" + messageName + "': " + reason)
	  {
	  }

	  public MismatchingMessageCorrelationException(string messageName, string businessKey, IDictionary<string, object> correlationKeys) : this("Cannot correlate message '" + messageName + "' with process instance business key '" + businessKey + "' and correlation keys " + correlationKeys)
	  {
	  }

	  public MismatchingMessageCorrelationException(string messageName, string businessKey, IDictionary<string, object> correlationKeys, string reason) : this("Cannot correlate message '" + messageName + "' with process instance business key '" + businessKey + "' and correlation keys " + correlationKeys + ": " + reason)
	  {
	  }
	}

}